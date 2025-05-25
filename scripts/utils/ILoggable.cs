using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace TheRingGoesSouth.scripts.utils
{
    public interface ILoggable
    {
        public bool DEBUG_TAG { get; set; }
    }
}