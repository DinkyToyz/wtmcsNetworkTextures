using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WhatThe.Mods.CitiesSkylines.NetworkTextures.Util;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Pieces
{
    /// <summary>
    /// Texture export class.
    /// </summary>
    internal class Exporter
    {
        /// <summary>
        /// The export log.
        /// </summary>
        private List<string> exportLog = new List<string>();

        /// <summary>
        /// Exports the mapped textures.
        /// </summary>
        public void ExportMappedTextures()
        {
            this.ExportTextures();
        }

        /// <summary>
        /// Exports the separate textures.
        /// </summary>
        public void ExportSeparateTextures()
        {
            this.ExportTextures();
        }

        /// <summary>
        /// Exports the texture.
        /// </summary>
        /// <param name="logInfo">The log information.</param>
        /// <param name="netInfo">The net information.</param>
        /// <param name="material">The material.</param>
        /// <param name="textureName">Name of the texture.</param>
        private void ExportTexture(Log.InfoList logInfo, NetInfo netInfo, Material material, string textureName)
        {
            Texture texture = material.GetTexture("_" + textureName);

            if (texture != null)
            {
                logInfo.Add(textureName + "HashCode", texture.GetHashCode());
                logInfo.Add(textureName + "InstanceID", texture.GetInstanceID());
                logInfo.Add(textureName + "ClassType", texture.GetType());
            }
        }

        /// <summary>
        /// Exports the textures.
        /// </summary>
        private void ExportTextures()
        {
            Log.Debug(this, "ExportTextures");

            try
            {
                this.exportLog.Clear();

                foreach (NetCollection netCollection in UnityEngine.Object.FindObjectsOfType<NetCollection>())
                {
                    if (netCollection != null)
                    {
                        Log.DevDebug(this, "ExportTextures", netCollection, netCollection.name, netCollection);

                        if (netCollection.m_prefabs != null)
                        {
                            Log.DevDebug(this, "ExportTextures", netCollection.m_prefabs, netCollection.m_prefabs.Length);

                            foreach (NetInfo netInfo in netCollection.m_prefabs)
                            {
                                if (netInfo != null)
                                {
                                    Log.DevDebug(this, "ExportTextures", netInfo, netInfo.name, netInfo.m_class.name);

                                    if (netInfo.m_segments != null)
                                    {
                                        Log.DevDebug(this, "ExportTextures", netInfo.m_segments, netInfo.m_segments.Length);

                                        if (netInfo.m_segments.Length > 0 && netInfo.m_segments[0].m_material != null)
                                        {
                                            Log.DevDebug(this, "ExportTextures", netInfo.m_segments[0].m_material, netInfo.m_segments[0].m_material.name);

                                            Log.InfoList logInfo = new Log.InfoList();

                                            logInfo.Add("netInfoName", netInfo.name);
                                            logInfo.Add("netInfoClassName", netInfo.m_class.name);
                                            logInfo.Add("netInfoTag", netInfo.tag);
                                            logInfo.Add("netInfoHashCode", netInfo.GetHashCode());
                                            logInfo.Add("netInfoInstanceID", netInfo.GetInstanceID());

                                            logInfo.Add("netInfoName", netInfo.m_segments[0].m_material.name);
                                            logInfo.Add("netInfoHashCode", netInfo.m_segments[0].m_material.GetHashCode());
                                            logInfo.Add("netInfoInstanceID", netInfo.m_segments[0].m_material.GetInstanceID());

                                            this.ExportTexture(logInfo, netInfo, netInfo.m_segments[0].m_material, "MainTex");
                                            this.ExportTexture(logInfo, netInfo, netInfo.m_segments[0].m_material, "ACIMap");
                                            this.ExportTexture(logInfo, netInfo, netInfo.m_segments[0].m_material, "XYSMap");

                                            this.exportLog.Add(logInfo.ToLine());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                this.SaveExportLog();
            }
            catch (Exception ex)
            {
                Log.Error(this, "ExportTextures", ex);
            }
        }

        /// <summary>
        /// Saves the export log.
        /// </summary>
        private void SaveExportLog()
        {
            Log.Debug(this, "SaveExportLog");

            if (this.exportLog.Count > 0)
            {

                try
                {
                    using (StreamWriter logFile = FileSystem.OpenStreamWriter(".export", "Export.log", false))
                    {
                        logFile.Write(String.Join("", this.exportLog.ToArray()));
                        logFile.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(this, "SaveExportLog", ex);
                }
                finally
                {
                    this.exportLog.Clear();
                }
            }
        }
    }
}