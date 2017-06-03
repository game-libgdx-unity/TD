using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace UnitedSolution
{
    public static class SoundManagerGenerator
    {
        [MenuItem("Code Generator/SoundManager.cs")]
        public static void Generate()
        {
            // Try to find an existing file in the project called "UnityConstants.cs"
            string filePath = string.Empty;
            foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(file) == "GameAudio")
                {
                    filePath = file;
                    break;
                }
            }

            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            if (string.IsNullOrEmpty(filePath))
            {
                string directory = Application.dataPath + "/Gen/";
                filePath = Path.Combine(directory, "SoundManager.cs");
            }

            // Write out our file
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
                writer.WriteLine();
                writer.WriteLine("namespace UnitedSolution");
                writer.WriteLine("{");
                
                writer.WriteLine("    public partial class SoundManager");
                writer.WriteLine("    {");
                AudioClip[] Songs = Resources.LoadAll<AudioClip>("Audio/BackgroundMusic");
                foreach (var song in Songs)
                {
                    string name = song.name.MakeSafeForCode().FirstLetterToUpperCase();
                    writer.WriteLine("        public static void Play_" + name + "() {");
                    writer.WriteLine("            SoundManager.Instance.PlaySong(\"" + song.name + "\");");
                    writer.WriteLine("        }");
                }

                AudioClip[] SFXs = Resources.LoadAll<AudioClip>("Audio/EffectSounds");
                foreach (var sfx in SFXs)
                {
                    string name = sfx.name.MakeSafeForCode().FirstLetterToUpperCase();
                    writer.WriteLine("        public static void Play_" + name + "() {");
                    writer.WriteLine("            SoundManager.Instance.PlaySfx(\"" + sfx.name + "\");");
                    writer.WriteLine("        }");
                }


                writer.WriteLine("    }");
                writer.WriteLine();
                writer.WriteLine("}");
            }
            // Refresh
            AssetDatabase.Refresh();
            SyncSolution.Sync();
        }

        public static string MakeSafeForCode(this string str)
        {
            str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled).Replace(' ', '_');

            if (char.IsDigit(str[0]))
            {
                str = "_" + str;
            }
            return str;
        }

        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new System.ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string FirstLetterToLowerCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new System.ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}
