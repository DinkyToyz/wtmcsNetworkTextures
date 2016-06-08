using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Pieces
{
    /// <summary>
    /// Mod info.
    /// </summary>
    internal static class Library
    {
        /// <summary>
        /// The description.
        /// </summary>
        public const string Description = "Does things with Cities: Skylines network textures.";

        /// <summary>
        /// The name.
        /// </summary>
        public const string Name = "wtmcsNetworkTextures";

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "Network Textures (WtM)";

        /// <summary>
        /// Gets a value indicating whether this is a debug build.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is a debug build; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
