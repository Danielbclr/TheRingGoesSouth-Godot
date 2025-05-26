using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;

namespace TheRingGoesSouth.scripts.utils
{
	/// <summary>
	/// Utility class for logging debug messages in Godot projects.
	/// Supports both instance-based and static logging, with optional debug tag checks.
	/// </summary>
	public class Logger
	{
		private Object _node;

		/// <summary>
		/// Initializes a new instance of the <see cref="Logger"/> class, associating it with a Godot object.
		/// </summary>
		/// <param name="node">The Godot object to associate with this logger.</param>
		public Logger(Object node)
		{
			_node = node;
		}

		/// <summary>
		/// Logs a message if the associated node implements <see cref="ILoggable"/> and its DEBUG_TAG is enabled.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Log(string message)
		{
			if (_node is not ILoggable lgb || !lgb.DEBUG_TAG)
			{
				return;
			}
			var type = _node.GetType();
			GD.Print($"[{type}]: {message}");
		}

		/// <summary>
		/// Logs a message for the specified node if it implements <see cref="ILoggable"/> and its DEBUG_TAG is enabled.
		/// </summary>
		/// <param name="node">The Godot object to log for.</param>
		/// <param name="message">The message to log.</param>
		public static void Log(Object node, string message)
		{
			if (node is not ILoggable lgb || !lgb.DEBUG_TAG)
			{
				return;
			}
			var type = node.GetType();
			GD.Print($"[{type}]: {message}");
		}

		/// <summary>
		/// Logs a message for the specified class name if <paramref name="debugTag"/> is <c>false</c>.
		/// </summary>
		/// <param name="Name">The class name to log for.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="debugTag">If <c>true</c>, logging is skipped. If <c>false</c>, the message is logged.</param>
		public static void Log(String Name, string message, bool debugTag = true)
		{
			if (debugTag)
			{
				return;
			}
			GD.Print($"[{Name}]: {message}");
		}

	}
}
