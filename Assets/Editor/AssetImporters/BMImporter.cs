//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine.UI;
//using System.Linq;
//using Newtonsoft.Json;

//public class BMImporter
//{

//    [MenuItem("Assets/BitmapFontImporter/Import from text file")]
//    public static void ImportBitmapFont()
//    {
//        var selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
//        if (selectedObjects.Length > 0 && selectedObjects[0] is TextAsset)
//        {
//            var text = (selectedObjects[0] as TextAsset).text;
//            var lines = text.Split('\n');

//            string selectionPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedObjects[0]));

//            var textureName = selectedObjects[0].name;
//            var texturePath = Path.Combine(selectionPath, textureName + ".png");
//            Texture2D texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;


//            string[] infos = null;
//            float x, y, w, h, px, py;
//            var spriteName = "";
//            var charInfos = lines.Where(line =>
//                                !line.Trim().Equals("") &&
//                                !line.StartsWith("#") &&
//                                !line.StartsWith(":format") &&
//                                !line.StartsWith(":texture") &&
//                                !line.StartsWith(":size"))
//                .Select(line =>
//                {

//                    infos = line.Split(';');
//                    spriteName = infos[0].Replace("/", "-");
//                    x = float.Parse(infos[1]);
//                    y = float.Parse(infos[2]);
//                    w = float.Parse(infos[3]);
//                    h = float.Parse(infos[4]);
//                    px = float.Parse(infos[5]);
//                    py = float.Parse(infos[6]);

//                    int asciiIndex = -1;
//                    byte[] bytes = null;
//                    spriteName = spriteName.Length > 1 ? spriteName.Replace("'", "") : spriteName;
//                    if (int.TryParse(spriteName, out asciiIndex))
//                    {
//                        if (asciiIndex > 9)
//                        {
//                            bytes = new[] { (byte)asciiIndex };
//                        }
//                    }

//                    if (bytes == null)
//                    {
//                        bytes = Encoding.ASCII.GetBytes(spriteName);
//                    }

//                    return new CharacterInfo
//                    {
//                        index = bytes[0],
//                        flipped = false,
//                        uv = new Rect(x / texture.width, y / texture.height, w / texture.width, h / texture.height),
//                        vert = new Rect(0, h / 2, w, -h),
//                        width = w
//                    };
//                }).ToArray();

//            Font font = new Font(selectedObjects[0].name);


//            font.characterInfo = charInfos;

//            Shader shader = Shader.Find("Sprites/Default");
//            Material material = new Material(shader);
//            material.mainTexture = texture;

//            font.material = material;

//            AssetDatabase.CreateAsset(material, Path.Combine(selectionPath, selectedObjects[0].name + ".mat"));
//            AssetDatabase.CreateAsset(font, Path.Combine(selectionPath, selectedObjects[0].name + ".fontsettings"));
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }
//    }

//    [MenuItem("Assets/BitmapFontImporter/Import from texture meta")]
//    public static void ImportBitmapFontFromMeta()
//    {
//        var selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
//        if (selectedObjects.Length > 0 && selectedObjects[0] is Texture2D)
//        {
//            var path = AssetDatabase.GetAssetPath(selectedObjects[0].GetInstanceID());
//            var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(path);
//            var text = File.ReadAllText(metaPath);

//            var lines = text.Split('\n');

//            string selectionPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedObjects[0]));

//            var textureName = selectedObjects[0].name;
//            var texturePath = Path.Combine(selectionPath, textureName + ".png");
//            Texture2D texture = selectedObjects[0] as Texture2D;

//            float x, y, w, h, px, py;
//            var spriteName = "";

//            int index = 0;
//            var pivotDefinition = new { x = 0.0, y = 0.0 };

//            List<CharacterInfo> charInfos = new List<CharacterInfo>();
//            while (index < lines.Length)
//            {
//                var line = lines[index].Trim();
//                // Sprite name
//                if (line.IndexOf('-') == 0)
//                {
//                    spriteName = line.Split(':')[1].Trim();

//                    index += 3;
//                    line = lines[index].Trim();
//                    x = float.Parse(line.Split(':')[1].Trim());

//                    index++;
//                    line = lines[index].Trim();
//                    y = float.Parse(line.Split(':')[1].Trim());

//                    index++;
//                    line = lines[index].Trim();
//                    w = float.Parse(line.Split(':')[1].Trim());

//                    index++;
//                    line = lines[index].Trim();
//                    h = float.Parse(line.Split(':')[1].Trim());

//                    index += 2;
//                    line = lines[index].Trim();
//                    var pivotIndex = line.IndexOf(':') + 1;
//                    var strPivot = line.Substring(pivotIndex, line.Length - pivotIndex);
//                    var pivot = JsonConvert.DeserializeAnonymousType(strPivot, pivotDefinition);
//                    px = (float)pivot.x;
//                    py = (float)pivot.y;

//                    int asciiIndex = -1;
//                    byte[] bytes = null;
//                    spriteName = spriteName.Length > 1 ? spriteName.Replace("'", "") : spriteName;
//                    if (int.TryParse(spriteName, out asciiIndex))
//                    {
//                        if (asciiIndex > 9)
//                        {
//                            bytes = new[] { (byte)asciiIndex };
//                        }
//                    }
//                    if (bytes == null)
//                    {
//                        bytes = Encoding.ASCII.GetBytes(spriteName);
//                    }

//                    charInfos.Add(new CharacterInfo
//                    {
//                        index = bytes[0],
//                        flipped = false,
//                        uv = new Rect(x / texture.width, y / texture.height, w / texture.width, h / texture.height),
//                        vert = new Rect(0, h / 2, w, -h),
//                        width = w
//                    });
//                }

//                index++;
//            }

//            Font font = new Font(selectedObjects[0].name);

//            charInfos.Sort( (ci1, ci2) =>
//            {
//                return ci1.index.CompareTo(ci2.index);
//            });
//            font.characterInfo = charInfos.ToArray();

//            Shader shader = Shader.Find("Sprites/Default");
//            Material material = new Material(shader);
//            material.mainTexture = texture;

//            font.material = material;

//            AssetDatabase.CreateAsset(material, Path.Combine(selectionPath, selectedObjects[0].name + ".mat"));
//            AssetDatabase.CreateAsset(font, Path.Combine(selectionPath, selectedObjects[0].name + ".fontsettings"));
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//        }
//    }
//}
