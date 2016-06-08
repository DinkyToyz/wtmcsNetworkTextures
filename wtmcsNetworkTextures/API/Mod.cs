using System;
using ICities;
using WhatThe.Mods.CitiesSkylines.NetworkTextures.Pieces;
using WhatThe.Mods.CitiesSkylines.NetworkTextures.Util;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.API
{
    /// <summary>
    /// Mod interface.
    /// </summary>
    public class Mod : IUserMod
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get
            {
                return Library.Description;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return Library.Title;
            }
        }

        /// <summary>
        /// Called when initializing mod settings UI.
        /// </summary>
        /// <param name="helper">The helper.</param>
        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelperBase group = helper.AddGroup("Export");

                group.AddButton(
                    "Save Textures, Mapped",
                    () =>
                    {
                        Log.Debug(this, "OnSettingsUI", "ExportMappedTextures");

                        Exporter exporter = new Exporter();
                        exporter.ExportMappedTextures();
                    });

                group.AddButton(
                    "Save Textures, Separate",
                    () =>
                    {
                        Log.Debug(this, "OnSettingsUI", "ExportSeparateTextures");

                        Exporter exporter = new Exporter();
                        exporter.ExportSeparateTextures();
                    });
            }
            catch (Exception ex)
            {
                Log.Error(this, "OnSettingsUI", ex);
            }
        }
    }
}