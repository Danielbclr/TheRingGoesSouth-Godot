using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;

namespace TheRingGoesSouth.scripts.utils
{
	public class Logger
	{
		public static void Log(Object node, string message)
		{
			if (node is not ILoggable lgb || !lgb.DEBUG_TAG)
			{
				return;
			}
			var type = node.GetType();
			GD.Print($"[{type}]: {message}");
		}


		public static void Log(String className, string message, bool debugTag = true)
		{
			if (debugTag)
			{
				return;
			}
			GD.Print($"[{className}]: {message}");
		}
		
	}
}
