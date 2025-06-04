using Godot;
using System;
using TheRingGoesSouth.scripts.actors;
using TheRingGoesSouth.scripts.data;
using TheRingGoesSouth.scripts.ui; 

namespace TheRingGoesSouth.scripts.utils.Battle
{
    public partial class BattleMenuHelper : Node
    {
        [Signal]
        public delegate void SkillSelectedEventHandler(string skillId);

        [Signal]
        public delegate void MenuEnteredEventHandler(bool entered);

        private BattleSkillMenu _skillMenuInstance;
        private PackedScene _skillMenuScene;
        private Node _parentScene;

        public BattleMenuHelper()
        {
            // Constructor for when added as a node in editor,
            // but Initialize is preferred for programmatic setup.
        }

        public BattleMenuHelper(Node parentScene)
        {
            Instantiate();
            Initialize(parentScene);
        }

        private void Instantiate()
        {
            var scenePath = "res://scenes/ui/battle_skill_menu.tscn";
            _skillMenuScene = ResourceLoader.Load<PackedScene>(scenePath);
        }

        public void Initialize(Node parentScene)
        {
            _parentScene = parentScene;
            if (_skillMenuScene == null)
            {
                GD.PrintErr("BattleMenuHelper: SkillMenuScene is null. Cannot instantiate skill menu.");
                return;
            }

            _skillMenuInstance = _skillMenuScene.Instantiate<BattleSkillMenu>();
            if (_skillMenuInstance == null)
            {
                GD.PrintErr("BattleMenuHelper: Failed to instantiate BattleSkillMenu.");
                return;
            }

            _parentScene.AddChild(_skillMenuInstance);
            _skillMenuInstance.SkillSelected += OnSkillSelectedFromMenu;
            _skillMenuInstance.MenuEntered += _OnMouseEnteredButton;
            GD.Print("BattleMenuHelper initialized and BattleSkillMenu instantiated.");
        }

        public void DisplaySkillsForCharacter(PlayerUnit activeUnit)
        {
            if (_skillMenuInstance == null)
            {
                GD.PrintErr("BattleMenuHelper: SkillMenuInstance is null. Cannot display skills.");
                return;
            }
            _skillMenuInstance.ShowSkills(activeUnit.CharacterData);
            _skillMenuInstance.FocusOnUnit(activeUnit);
        }

        private void OnSkillSelectedFromMenu(string skillId)
        {
            // Forward the event
            EmitSignal(SignalName.SkillSelected, skillId);
            // The BattleSkillMenu itself will call HideMenu() upon selection.
        }

        /// <summary>
		/// Handles mouse entering or exiting a skill button.
		/// </summary>
		/// <param name="isMouseInside">True if the mouse entered, false if it exited.</param>
        private void _OnMouseEnteredButton(bool isMouseInside)
		{
			EmitSignal(SignalName.MenuEntered, isMouseInside);
		}
    }
}