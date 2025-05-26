using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace TheRingGoesSouth.scripts.utils
{
    /// <summary>
    /// Interface for objects that support debug logging.
    /// Provides a <c>DEBUG_TAG</c> property to enable or disable logging.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Gets or sets a value indicating whether debug logging is enabled for this object.
        /// </summary>
        public bool DEBUG_TAG { get; set; }
    }
}