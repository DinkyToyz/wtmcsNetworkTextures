using System;
using UnityEngine;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Util
{
    /// <summary>
    /// Texture extensions.
    /// </summary>
    public static class TextureExtensions
    {
        /// <summary>
        /// Channel manipulation options.
        /// </summary>
        [Flags]
        public enum ChannelOptions
        {
            /// <summary>
            /// Red uses alpha.
            /// </summary>
            RedAlpha = 1 << 0,

            /// <summary>
            /// Green uses alpha.
            /// </summary>
            GreenAlpha = 1 << 1,

            /// <summary>
            /// Blue uses alpha.
            /// </summary>
            BlueAlpha = 1 << 2,

            /// <summary>
            /// Invert red channel.
            /// </summary>
            InvertRed = 1 << 3,

            /// <summary>
            /// Invert green channel.
            /// </summary>
            InvertGreen = 1 << 4,

            /// <summary>
            /// Invert blue channel.
            /// </summary>
            InvertBlue = 1 << 5,

            /// <summary>
            /// Invert alpha channel.
            /// </summary>
            InvertAlpha = 1 << 6,

            /// <summary>
            /// No options set.
            /// </summary>
            None = 0,

            /// <summary>
            /// The diffuse texture extraction options.
            /// </summary>
            ExtractMainTex = None,

            /// <summary>
            /// The XYS map extraction options.
            /// </summary>
            ExtractXYSMap = BlueAlpha | InvertBlue,

            /// <summary>
            /// The ACI map extraction options.
            /// </summary>
            ExtractACIMap = RedAlpha | GreenAlpha | BlueAlpha | InvertRed | InvertGreen
        }

        /// <summary>
        /// Convert colors to Texture2D.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The converted texture.</returns>
        public static Texture2D ToTexture2D(this Color32[] colors, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);

            texture2D.SetPixels32(colors);
            texture2D.Apply();

            return texture2D;
        }

        /// <summary>
        /// Extracts the channels.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="options">The options.</param>
        /// <param name="redChannel">The red channel.</param>
        /// <param name="greenChannel">The green channel.</param>
        /// <param name="blueChannel">The blue channel.</param>
        /// <param name="alphaChannel">The alpha channel.</param>
        public static void ExtractChannels(
            this Texture2D texture,
            ChannelOptions options,
            Color32[] redChannel,
            Color32[] greenChannel,
            Color32[] blueChannel,
            Color32[] alphaChannel = null)
        {
            Color32[] colors = texture.ToColors();

            for (int i = 0; i < colors.Length; ++i)
            {
                byte red = colors[i].r;
                byte green = colors[i].g;
                byte blue = colors[i].b;
                byte alpha = colors[i].a;

                if ((options & ChannelOptions.InvertRed) != ChannelOptions.None)
                {
                    red = (byte)~red;
                }

                if ((options & ChannelOptions.InvertGreen) != ChannelOptions.None)
                {
                    green = (byte)~green;
                }

                if ((options & ChannelOptions.InvertBlue) != ChannelOptions.None)
                {
                    blue = (byte)~blue;
                }

                if ((options & ChannelOptions.InvertAlpha) != ChannelOptions.None)
                {
                    alpha = (byte)~alpha;
                }

                if (redChannel != null)
                {
                    redChannel[i].r = red;

                    if ((options & ChannelOptions.RedAlpha) != ChannelOptions.None)
                    {
                        redChannel[i].g = red;
                        redChannel[i].b = red;
                    }
                }

                if (greenChannel != null)
                {
                    greenChannel[i].g = green;

                    if ((options & ChannelOptions.GreenAlpha) != ChannelOptions.None)
                    {
                        greenChannel[i].r = green;
                        greenChannel[i].b = green;
                    }
                }

                if (blueChannel != null)
                {
                    blueChannel[i].b = blue;

                    if ((options & ChannelOptions.BlueAlpha) != ChannelOptions.None)
                    {
                        blueChannel[i].r = blue;
                        blueChannel[i].g = blue;
                    }
                }

                if (alphaChannel != null)
                {
                    alphaChannel[i].a = alpha;
                }
            }
        }

        /// <summary>
        /// Inverts the specified colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>The inverted colors.</returns>
        public static Color32[] Invert(this Color32[] colors)
        {
            var inverted = new Color32[colors.Length];

            for (var i = 0; i < colors.Length; i++)
            {
                inverted[i].r = (byte)~colors[i].r;
                inverted[i].g = (byte)~colors[i].g;
                inverted[i].b = (byte)~colors[i].b;
                inverted[i].a = (byte)~colors[i].a;
            }

            return inverted;
        }

        /// <summary>
        /// Converts Texture2D to Colors.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>The colors.</returns>
        public static Color32[] ToColors(this Texture2D texture)
        {
            Color32[] colors;

            try
            {
                colors = texture.GetPixels32();
            }
            catch
            {
                colors = texture.RenderToTexture2D().GetPixels32();
            }

            return colors;
        }

        /// <summary>
        /// Converts Texture2D to PNG file data.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>The PNG file data.</returns>
        public static byte[] ToPNG(this Texture2D texture)
        {
            byte[] png;

            try
            {
                png = texture.EncodeToPNG();
            }
            catch
            {
                png = texture.RenderToTexture2D().EncodeToPNG();
            }

            return png;
        }

        /// <summary>
        /// Renders texture to Texture2D.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>The rendered texture.</returns>
        private static Texture2D RenderToTexture2D(this Texture texture)
        {
            Log.Debug(typeof(TextureExtensions), "MakeReadable", texture.GetInstanceID(), texture.name);

            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0);
            Graphics.Blit(texture, renderTexture);
            Texture2D texture2D = ToTexture2D(renderTexture);
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        /// <summary>
        /// Converts RenderTexture to Texture2D.
        /// </summary>
        /// <param name="renderTexture">The render texture.</param>
        /// <returns>The converted texture.</returns>
        private static Texture2D ToTexture2D(this RenderTexture renderTexture)
        {
            RenderTexture oldTexture = RenderTexture.active;

            RenderTexture.active = renderTexture;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = oldTexture;

            return texture2D;
        }
    }
}