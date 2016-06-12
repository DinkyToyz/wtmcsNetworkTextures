using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace WhatThe.Mods.CitiesSkylines.NetworkTextures.Util
{
    /// <summary>
    /// Type extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// The bad file name characters.
        /// </summary>
        private static Regex badFileNameChars = null;

        /// <summary>
        /// Get only ASCII capitals.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The ASCII capitals.</returns>
        public static string ASCIICapitals(this string text)
        {
            return Regex.Replace(text, "[^A-Z]", "");
        }

        /// <summary>
        /// Invokes method in base class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The return object.</returns>
        public static object BaseInvoke(this object instance, string methodName, object[] parameters)
        {
            Type baseType = instance.GetType().BaseType;

            if (baseType == null)
            {
                throw new MethodAccessException("Base type not found");
            }

            MethodInfo methodInfo = baseType.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new MethodAccessException("Base method not found");
            }

            return methodInfo.Invoke(instance, parameters);
        }

        /// <summary>
        /// Casts object to type.
        /// </summary>
        /// <typeparam name="T">Type to cast to.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>Cast object.</returns>
        public static object CastTo<T>(this object obj)
        {
            try
            {
                return (T)obj;
            }
            catch
            {
                return obj;
            }
        }

        /// <summary>
        /// Casts object to type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <returns>Cast object.</returns>
        public static object CastTo(this object obj, Type type)
        {
            MethodInfo castMethod = obj.GetType().GetMethod("CastTo").MakeGenericMethod(type);
            return castMethod.Invoke(null, new object[] { obj });
        }

        /// <summary>
        /// Casts object to base class.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Cast object.</returns>
        public static object CastToBase(this object obj)
        {
            return obj.CastTo(obj.GetType().BaseType);
        }

        /// <summary>
        /// Cleans the file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="replacement">The replacement string for bad characters.</param>
        /// <returns>The clean file name.</returns>
        public static string CleanFileName(this string fileName, string replacement = "!")
        {
            if (fileName == null)
            {
                return null;
            }

            if (badFileNameChars == null)
            {
                badFileNameChars = new Regex("([" + Regex.Escape(new String(Path.GetInvalidFileNameChars())) + "])", RegexOptions.CultureInvariant);
            }

            return badFileNameChars.Replace(fileName, replacement);
        }

        /// <summary>
        /// Cleans the file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="replacement">The replacement string for bad characters.</param>
        /// <returns>The clean file path.</returns>
        public static string CleanFilePath(this string filePath, string replacement = "!")
        {
            if (filePath == null)
            {
                return null;
            }

            int i = 0;
            StringBuilder cleanPath = new StringBuilder();

            int p = (i >= filePath.Length) ? -1 : filePath.IndexOf(Path.VolumeSeparatorChar, i);
            if (p >= i)
            {
                cleanPath.Append(filePath.Substring(i, p - i).CleanFileName(replacement)).Append(Path.VolumeSeparatorChar);
                i = p + 1;
            }

            while (i < filePath.Length)
            {
                p = filePath.IndexOf(Path.DirectorySeparatorChar, i);

                if (Path.AltDirectorySeparatorChar == Path.DirectorySeparatorChar)
                {
                    int pa = filePath.IndexOf(Path.AltDirectorySeparatorChar, i);

                    if (pa >= i && (pa < p || p < i))
                    {
                        p = pa;
                    }
                }

                if (p < i)
                {
                    break;
                }
                else
                {
                    cleanPath.Append(filePath.Substring(i, p - i).CleanFileName(replacement)).Append(Path.DirectorySeparatorChar);
                    i = p + 1;
                }
            }

            if (i < filePath.Length)
            {
                cleanPath.Append(filePath.Substring(i).CleanFileName(replacement));
            }

            return cleanPath.ToString();
        }

        /// <summary>
        /// Cleans the newlines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The clean text.</returns>
        public static string CleanNewLines(this string text)
        {
            return Regex.Replace(text, "[\r\n]+", "\n");
        }

        /// <summary>
        /// Cleans the newlines.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The clean text.</returns>
        public static string CleanNewLines(this StringBuilder text)
        {
            return text.ToString().CleanNewLines();
        }

        /// <summary>
        /// Conforms the newlines to the environment.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The conforming text.</returns>
        public static string ConformNewlines(this string text)
        {
            return Regex.Replace(text, "[\r\n]+", Environment.NewLine);
        }

        /// <summary>
        /// Conforms the newlines to the environment.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The conforming text.</returns>
        public static string ConformNewlines(this StringBuilder text)
        {
            return text.ToString().ConformNewlines();
        }
    }
}