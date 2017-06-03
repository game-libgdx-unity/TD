using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("2d/Effect2D"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePackingTag = "effect";
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Trilinear;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }
        else if (assetPath.Contains("Textures/Gamble"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePackingTag = "gamble";
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Trilinear;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }
        else if (assetPath.Contains("Textures/Bonus"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePackingTag = "bonus";
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Trilinear;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }
        else if (assetPath.Contains("Textures/Jackpot"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Sprite;
            importer.spritePackingTag = "jackpot";
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = false;
            importer.isReadable = false;
            importer.filterMode = FilterMode.Trilinear;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }

    }
}