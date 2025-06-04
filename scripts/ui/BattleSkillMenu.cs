// File: c:\Users\nihan\Documents\Projetos\the-ring-goes-south\scripts\ui\SkillSelectionMenu.cs
using Godot;
using System;
using System.Collections.Generic;
using TheRingGoesSouth.scripts.data; // Assuming CharacterData is here
using TheRingGoesSouth.scripts.data.skill; // Assuming SkillDataLoader and SkillData are here

namespace TheRingGoesSouth.scripts.ui
{
	/// <summary>
	/// A simple UI menu to display a character's skills and allow selection.
	/// </summary>
	public partial class BattleSkillMenu : Control // Or CanvasLayer if you want it always on top
	{
		// Export a PackedScene for a button if you want custom button styles
		// Otherwise, we'll create default Buttons programmatically.
		[Export] public PackedScene SkillButtonPrefab { get; set; }

		private VBoxContainer _skillButtonContainer; // Reference to the container node in the scene

		// Signal to emit when a skill is selected by the player
		[Signal]
		public delegate void SkillSelectedEventHandler(string skillId);
		[Signal]
        public delegate void MenuEnteredEventHandler(bool entered);

		public override void _Ready()
		{
			// Get the container node where buttons will be added.
			// Make sure you have a VBoxContainer named "SkillButtonContainer" in your scene.
			_skillButtonContainer = GetNode<VBoxContainer>("SkillButtonContainer");
			_skillButtonContainer.MouseFilter = MouseFilterEnum.Stop;

			// Ensure the container exists
			if (_skillButtonContainer == null)
			{
				GD.PrintErr("SkillSelectionMenu: VBoxContainer node 'SkillButtonContainer' not found!");
				// Consider disabling the script or handling this error appropriately
				SetProcess(false);
				return;
			}

			Hide(); // Start hidden
		}

		/// <summary>
		/// Populates the menu with skill buttons for the given character's current skills.
		/// </summary>
		/// <param name="character">The character whose skills to display.</param>
		public void ShowSkills(CharacterData character)
		{
			if (_skillButtonContainer == null)
			{
				GD.PrintErr("SkillSelectionMenu: Cannot show skills, container is null.");
				return;
			}

			// Clear any existing buttons from a previous display
			foreach (Node child in _skillButtonContainer.GetChildren())
			{
				child.QueueFree();
			}

			// Check if the character or their skills list is valid/empty
			if (character == null || character.CurrentSkills == null || character.CurrentSkills.Count == 0)
			{
				GD.Print("SkillSelectionMenu: No skills to display for character.");
				// Optionally display a message like "No skills available"
				Hide();
				return;
			}

			// Populate with new buttons based on the character's skills
			foreach (SkillData skillData in character.CurrentSkills)
			{
				if (skillData != null)
				{
					// Create a new button instance
					Button skillButton;
					if (SkillButtonPrefab != null)
					{
						skillButton = SkillButtonPrefab.Instantiate<Button>();
					}
					else
					{
						// Create a default button if no prefab is provided
						skillButton = new Button();
						skillButton.SizeFlagsHorizontal = SizeFlags.Fill; // Make it fill the container width
						
					}
					skillButton.MouseFilter = MouseFilterEnum.Stop;
					// Set button text and tooltip (description)
					skillButton.Text = skillData.Name;
					skillButton.TooltipText = skillData.Description;

					// Store the skill ID on the button using metadata for easy retrieval later
					skillButton.SetMeta("skill_id", skillData.Id);

					// Connect the button's Pressed signal to our handler method
					// Using a lambda to pass the button instance to the handler
					skillButton.Pressed += () => _OnSkillButtonPressed(skillButton);

					skillButton.MouseEntered += () => _OnMouseEnteredButton(true);
					skillButton.MouseExited  += () => _OnMouseEnteredButton(false);

					// Add the button to the container
					_skillButtonContainer.AddChild(skillButton);
				}
				else
				{
					// Log an error if a skill ID in the character's list doesn't exist in the database
					GD.PrintErr($"SkillSelectionMenu: Could not find SkillData for ID '{skillData.Name}' listed in character's skills.");
				}
			}

			// Make the menu visible
			Show();
		}

        private void _OnMouseEnteredButton(bool isMouseInside)
		{
			EmitSignal(SignalName.MenuEntered, isMouseInside);
		}


        /// <summary>
        /// Hides the skill selection menu.
        /// </summary>
        public void HideMenu()
		{
			Hide();
		}

		/// <summary>
		/// Handles a skill button being pressed.
		/// </summary>
		private void _OnSkillButtonPressed(Button button)
		{
			// Retrieve the skill ID from the button's metadata
			if (button.HasMeta("skill_id"))
			{
				string selectedSkillId = button.GetMeta("skill_id").AsString();
				GD.Print($"SkillSelectionMenu: Skill selected: {selectedSkillId}");

				// Emit the signal so other parts of the game can react
				EmitSignal(SignalName.SkillSelected, selectedSkillId);

				// Hide the menu after a skill is selected
				HideMenu();
			}
			else
			{
				// This shouldn't happen if buttons are created correctly
				GD.PrintErr("SkillSelectionMenu: Button pressed without 'skill_id' metadata!");
			}
		}

		// Optional: Add input handling to close the menu (e.g., pressing Escape)
		public override void _Input(InputEvent @event)
		{
			// Check if the menu is visible and the cancel action is pressed
			if (Visible && @event.IsActionPressed("ui_cancel")) // Assuming "ui_cancel" is mapped to Escape in Input Map
			{
				GetViewport().SetInputAsHandled(); // Consume the event so it doesn't affect other nodes
				HideMenu();
			}
		}
		
		public void FocusOnUnit(PlayerUnit unit)
		{
			GlobalPosition = unit.GlobalPosition;
		}
	}
}
