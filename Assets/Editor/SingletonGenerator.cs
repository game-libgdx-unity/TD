using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace UnitedSolution
{
    public static class SingletonGenerator
    {
        private const string PRIVATE_METHOD_NAME_PATTERN = "private void _";
        private const string METHOD_NAME_PATTERN = "void _";
        private const string START_PARAMETER_PATTERN = "(";

        [MenuItem("Code Generator/SingletonGenerator")]
        public static void Generate()
        {
            //enable Smart Logger
            SmartLogger.Initialize(LogLevel.ALL, LogDetails.ALL);
            // If no such file exists already, use the save panel to get a folder in which the file will be placed.
            string filePath = EditorUtility.OpenFilePanel("Select singleton to generate code", Application.dataPath, "cs");
            string className = Path.GetFileNameWithoutExtension(filePath);

            Logger.info("Run on " + filePath);
            Logger.info("Class: " + className);

            StringBuilder sourceBuilder = new StringBuilder();
            string content = null;

            Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();
            using (var reader = new StreamReader(filePath))
            {
                content = reader.ReadToEnd().Replace("public class ", "public partial class ");
            }

            // save our changes
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(content);
            }

            //find out the method should be generate
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    int private_method_index = line.IndexOf(PRIVATE_METHOD_NAME_PATTERN);
                    int public_method_index = line.IndexOf(METHOD_NAME_PATTERN);

                    int index = private_method_index;
                    string pattern = PRIVATE_METHOD_NAME_PATTERN;
                    if (index == -1) //switch to "public method" pattern
                    {
                        index = public_method_index;
                        pattern = METHOD_NAME_PATTERN;
                    }

                    if (index != -1)
                    {
                        int length = 1 + Mathf.Abs(index + pattern.Length - line.LastIndexOf(")"));
                        string methodName = line.Substring(index + pattern.Length, length).Trim().FirstLetterToUpperCase();
                        List<string> temp_parameters = new List<string>();
                        Logger.info("Method definination: " + methodName);
                        if (!line.Contains("()") && !line.Contains("( )"))
                        {
                            int startIndex = line.IndexOf(START_PARAMETER_PATTERN);
                            int paramLength = Mathf.Abs(startIndex + START_PARAMETER_PATTERN.Length - line.LastIndexOf(")"));
                            string parameter = line.Substring(startIndex + START_PARAMETER_PATTERN.Length, paramLength).Trim();

                            string[] param = parameter.Split(new string[] { ", " }, StringSplitOptions.None);

                            foreach (string p in param)
                            {
                                Logger.info("Parameter Definination: " + p.ToString());
                                string[] pp = p.Trim().Split(' ');
                                if (pp.Length > 1)
                                {
                                    Logger.info("Variable name: " + pp[1].ToString());
                                    temp_parameters.Add(pp[1]);
                                }
                            }
                            parameters.Add(methodName, temp_parameters);
                        }
                        else
                        {
                            parameters.Add(methodName, temp_parameters);
                        }
                    }
                }
            }
            //write to auto-generated *.cs file
            string generatedFilePath = Application.dataPath + "/Gen/" + className + ".cs";
            using (var writer = new StreamWriter(generatedFilePath))
            {
                writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
                writer.WriteLine();
                writer.WriteLine("using UnitedSolution;\n");

                writer.WriteLine("    public partial class " + className);
                writer.WriteLine("    {");
                foreach (var methodName in parameters)
                {
                    List<string> temp_parameters = methodName.Value;
                    string parameter = "(";
                    for (int i = 0; i < temp_parameters.Count; i++)
                    {
                        parameter += temp_parameters[i];
                        if (i != temp_parameters.Count - 1)
                        {
                            parameter += ", ";
                        }
                    }
                    parameter += ");";
                    Logger.info("parameter: " + parameter);

                    writer.WriteLine("        public static void " + methodName.Key + " {");

                    string methodParam = "_" + methodName.Key.FirstLetterToLowerCase();
                    //if(usePublicMethodPattern)
                    //{
                    //} 
                    methodParam = methodParam.Substring(0, methodParam.LastIndexOf('('));
                    writer.WriteLine("            " + className + ".Instance." + methodParam + parameter);
                    writer.WriteLine("        }");
                }

                writer.WriteLine("    }");
                writer.WriteLine();
            }
            // Refresh
            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.ImportAsset(generatedFilePath);
            SyncSolution.Sync();
        }
    }
}
