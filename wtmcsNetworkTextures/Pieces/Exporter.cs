using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using WhatThe.Mods.CitiesSkylines.NetworkTextures.Util;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Pieces
{
    /// <summary>
    /// Texture export class.
    /// </summary>
    internal class Exporter
    {
        /*
         *
         * Use textures InstanceID as map temp key, since several materials share some textures but not all!
         * Use texture name, with counter, as real map key.
         * 
         * Read existing map in order to not export duplicates?
         * Export to stamped dir?
         *
         */

        /// <summary>
        /// The current export mode.
        /// </summary>
        private ExportMode currentExportMode = ExportMode.Log;

        /// <summary>
        /// The current information log line.
        /// </summary>
        private Log.InfoList currentInfoLogLine = null;

        /// <summary>
        /// The default export mode.
        /// </summary>
        private ExportMode defaultExportMode = ExportMode.Log;

        /// <summary>
        /// Export he ACI map.
        /// </summary>
        private bool exportACIMap = true;

        /// <summary>
        /// The export log.
        /// </summary>
        private List<string> exportLog = new List<string>();

        /// <summary>
        /// Export the diffuse texture.
        /// </summary>
        private bool exportDiffuse = true;

        /// <summary>
        /// Export the XYS map.
        /// </summary>
        private bool exportXYSMap = true;

        /// <summary>
        /// The exported net info set.
        /// </summary>
        private HashSet<int> netInfos = new HashSet<int>();

        /// <summary>
        /// The exported textures.
        /// </summary>
        private Dictionary<int, Dictionary<string, List<string>>> textures = new Dictionary<int, Dictionary<string, List<string>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Exporter"/> class.
        /// </summary>
        /// <param name="defaultExportMode">The default export mode.</param>
        public Exporter(ExportMode? defaultExportMode = null)
        {
            if (defaultExportMode != null && defaultExportMode.HasValue)
            {
                this.defaultExportMode = defaultExportMode.Value;
            }
        }

        /// <summary>
        /// The export modes.
        /// </summary>
        public enum ExportMode
        {
            /// <summary>
            /// Only log what would have been exported.
            /// </summary>
            Log = 1,

            /// <summary>
            /// Export textures separately for all nets.
            /// </summary>
            Separate = 2,

            /// <summary>
            /// Export mapped textures.
            /// </summary>
            Mapped = 3
        }

        /// <summary>
        /// The texture types.
        /// </summary>
        private enum TextureType
        {
            /// <summary>
            /// The diffuse texture.
            /// </summary>
            MainTex = 1,

            /// <summary>
            /// The ACI map.
            /// </summary>
            ACIMap = 2,

            /// <summary>
            /// The XYS map.
            /// </summary>
            XYSMap = 3
        }

        /// <summary>
        /// Gets the current sub directory.
        /// </summary>
        /// <value>
        /// The current sub directory.
        /// </value>
        private string CurrentSubDirectory
        {
            get
            {
                return ".export" + Path.DirectorySeparatorChar + this.currentExportMode.ToString();
            }
        }

        /// <summary>
        /// Exports the textures.
        /// </summary>
        /// <param name="exportMode">The export mode.</param>
        public void ExportTextures(ExportMode? exportMode = null)
        {
            Log.Debug(this, "ExportTextures", exportMode);

            this.currentExportMode = (exportMode != null && exportMode.HasValue) ? exportMode.Value : this.defaultExportMode;

            try
            {
                this.exportLog.Clear();
                this.textures.Clear();
                this.netInfos.Clear();

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
                                if (netInfo != null && !this.netInfos.Contains(netInfo.GetInstanceID()))
                                {
                                    this.netInfos.Add(netInfo.GetInstanceID());
                                    Log.DevDebug(this, "ExportTextures", netInfo, netInfo.name, netInfo.m_class.name);

                                    if (netInfo.m_segments != null)
                                    {
                                        Log.DevDebug(this, "ExportTextures", netInfo.m_segments, netInfo.m_segments.Length);

                                        if (netInfo.m_segments.Length > 0 && netInfo.m_segments[0].m_material != null)
                                        {
                                            Log.DevDebug(this, "ExportTextures", netInfo.m_segments[0].m_material, netInfo.m_segments[0].m_material.name);

                                            this.currentInfoLogLine = new Log.InfoList();

                                            this.currentInfoLogLine.Add("netCollectionName", netCollection.name);
                                            this.currentInfoLogLine.Add("netInfoName", netInfo.name);
                                            this.currentInfoLogLine.Add("netInfoClassName", netInfo.m_class.name);
                                            this.currentInfoLogLine.Add("netInfoTag", netInfo.tag);
                                            this.currentInfoLogLine.Add("netInfoHashCode", netInfo.GetHashCode());
                                            this.currentInfoLogLine.Add("netInfoInstanceID", netInfo.GetInstanceID());

                                            this.currentInfoLogLine.Add("materialName", netInfo.m_segments[0].m_material.name);
                                            this.currentInfoLogLine.Add("materialHashCode", netInfo.m_segments[0].m_material.GetHashCode());
                                            this.currentInfoLogLine.Add("materialInstanceID", netInfo.m_segments[0].m_material.GetInstanceID());

                                            this.ExportTexture(netCollection, netInfo, netInfo.m_segments[0].m_material);

                                            this.exportLog.Add(this.currentInfoLogLine.ToLine());
                                            this.currentInfoLogLine = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (this.currentExportMode == ExportMode.Mapped)
                {
                    this.SaveTextureMap();
                }

                this.SaveExportLog();
            }
            catch (Exception ex)
            {
                Log.Error(this, "ExportTextures", ex);
            }
        }

        /// <summary>
        /// Exports the texture.
        /// </summary>
        /// <param name="netCollection">The net collection.</param>
        /// <param name="netInfo">The net information.</param>
        /// <param name="material">The material.</param>
        private void ExportTexture(NetCollection netCollection, NetInfo netInfo, Material material)
        {
            if (this.exportDiffuse)
            {
                this.ExportTexture(netCollection, netInfo, netInfo.m_segments[0].m_material, TextureType.MainTex);
            }

            if (this.exportACIMap)
            {
                this.ExportTexture(netCollection, netInfo, netInfo.m_segments[0].m_material, TextureType.ACIMap);
            }

            if (this.exportXYSMap)
            {
                this.ExportTexture(netCollection, netInfo, netInfo.m_segments[0].m_material, TextureType.XYSMap);
            }
        }

        /// <summary>
        /// Exports the texture.
        /// </summary>
        /// <param name="netCollection">The net collection.</param>
        /// <param name="netInfo">The net information.</param>
        /// <param name="material">The material.</param>
        /// <param name="textureType">Type of the texture.</param>
        /// <exception cref="NotImplementedException">Export not implemented for export mode.</exception>
        private void ExportTexture(NetCollection netCollection, NetInfo netInfo, Material material, TextureType textureType)
        {
            string textureName = textureType.ToString();
            Texture texture = material.GetTexture("_" + textureName);

            if (texture == null)
            {
                return;
            }

            this.currentInfoLogLine.Add(textureName + "Name", texture.name);
            this.currentInfoLogLine.Add(textureName + "HashCode", texture.GetHashCode());
            this.currentInfoLogLine.Add(textureName + "InstanceID", texture.GetInstanceID());
            this.currentInfoLogLine.Add(textureName + "ClassType", texture.GetType());

            bool export;
            string fileName;

            switch (this.currentExportMode)
            {
                case ExportMode.Log:
                    export = false;
                    fileName = null;
                    break;

                case ExportMode.Mapped:
                    Dictionary<string, List<string>> netLists;
                    List<string> netList;

                    int id = material.GetInstanceID();
                    string netName = String.Join(";", new string[] { netCollection.name, netInfo.name, netInfo.m_class.name });

                    if (!this.textures.TryGetValue(id, out netLists))
                    {
                        export = true;

                        netList = new List<string>();
                        netLists = new Dictionary<string, List<string>>();

                        netLists[textureName] = netList;
                        this.textures[id] = netLists;
                    }
                    else if (!netLists.TryGetValue(textureName, out netList))
                    {
                        export = true;

                        netList = new List<string>();
                        netLists[textureName] = netList;
                    }
                    else
                    {
                        export = false;
                    }

                    netList.Add(netName);
                    fileName = this.GetMappedFileName(id, textureName);
                    break;

                case ExportMode.Separate:
                    export = true;
                    fileName = String.Join("; ", new string[] { netCollection.name, netInfo.name, netInfo.m_class.name, textureName }).CleanFileName();
                    break;

                default:
                    throw new NotImplementedException("Export not implemented for export mode: " + this.currentExportMode.ToString());
            }

            if (export && fileName != null)
            {
                this.SaveTexture((Texture2D)texture, textureType, fileName);
            }
        }

        /// <summary>
        /// Gets the name of the mapped file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="textureName">Name of the texture.</param>
        /// <returns>The file name.</returns>
        private string GetMappedFileName(int id, string textureName = null)
        {
            string fileName;

            if (id < 0)
            {
                id *= -1;
                fileName = "N";
            }
            else
            {
                fileName = "P";
            }

            fileName += id.ToString("D16");

            if (!String.IsNullOrEmpty(textureName))
            {
                fileName += "_" + textureName;
            }

            return fileName;
        }

        /// <summary>
        /// Saves the export log.
        /// </summary>
        private void SaveExportLog()
        {
            Log.Debug(this, "SaveExportLog");

            if (FileSystem.SaveList(this.CurrentSubDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log", this.exportLog, true))
            {
                this.exportLog.Clear();
            }
        }

        /// <summary>
        /// Saves the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="textureType">Type of the texture.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True on success.</returns>
        private bool SaveTexture(Texture2D texture, TextureType textureType, string fileName)
        {
            string textureName = textureType.ToString();

            try
            {
                int area = texture.width * texture.height;

                switch (textureType)
                {
                    case TextureType.MainTex:
                        Color32[] diffuse = new Color32[area].Invert();

                        texture.ExtractChannels(TextureExtensions.ChannelOptions.ExtractMainTex, diffuse, diffuse, diffuse);

                        this.currentInfoLogLine.Add(textureName + "File", fileName + ".png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + ".png",
                            diffuse.ToTexture2D(texture.width, texture.height).ToPNG());

                        break;

                    case TextureType.XYSMap:
                        Color32[] xy = new Color32[area].Invert();
                        Color32[] s = new Color32[area].Invert();

                        texture.ExtractChannels(TextureExtensions.ChannelOptions.ExtractXYSMap, xy, xy, s);

                        this.currentInfoLogLine.Add(textureName + "XYFile", fileName + "XY.png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + "XY.png",
                            xy.ToTexture2D(texture.width, texture.height).ToPNG());

                        this.currentInfoLogLine.Add(textureName + "SFile", fileName + "S.png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + "S.png",
                            s.ToTexture2D(texture.width, texture.height).ToPNG());

                        break;

                    case TextureType.ACIMap:
                        Color32[] alpha;
                        Color32[] color;
                        Color32[] illumination;

                        alpha = new Color32[area].Invert();
                        color = new Color32[area].Invert();
                        illumination = new Color32[area].Invert();
                        texture.ExtractChannels(TextureExtensions.ChannelOptions.ExtractACIMap, alpha, color, illumination);

                        this.currentInfoLogLine.Add(textureName + "AFile", fileName + "A.png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + "A.png",
                            alpha.ToTexture2D(texture.width, texture.height).ToPNG());

                        this.currentInfoLogLine.Add(textureName + "CFile", fileName + "C.png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + "C.png",
                            color.ToTexture2D(texture.width, texture.height).ToPNG());

                        this.currentInfoLogLine.Add(textureName + "IFile", fileName + "I.png");
                        FileSystem.SaveBytes(
                            this.CurrentSubDirectory,
                            fileName + "I.png",
                            illumination.ToTexture2D(texture.width, texture.height).ToPNG());

                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(this, "SaveTexture", ex, texture, fileName);
                return false;
            }
        }

        /// <summary>
        /// Saves the texture map.
        /// </summary>
        private void SaveTextureMap()
        {
            Log.Debug(this, "SaveTextureMap");

            List<string> map = new List<string>();

            foreach (KeyValuePair<int, Dictionary<string, List<string>>> textureLists in this.textures)
            {
                foreach (KeyValuePair<string, List<string>> textureList in textureLists.Value)
                {
                    foreach (string netName in textureList.Value)
                    {
                        StringBuilder line = new StringBuilder();
                        line.Append(this.GetMappedFileName(textureLists.Key)).Append(": ").Append(netName).Append("\n");

                        map.Add(line.ConformNewlines());
                    }
                }
            }

            FileSystem.SaveList(this.CurrentSubDirectory, "Textures.map", map, false);
        }
    }
}