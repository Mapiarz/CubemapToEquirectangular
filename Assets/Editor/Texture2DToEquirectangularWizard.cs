using System.IO;
using UnityEditor;
using UnityEngine;

class Texture2DToEquirectangularWizard : ScriptableWizard
{
    public Texture2D texture = null;
    public int outputWidth = 4096;
    public int outputHeight = 2048;

    [MenuItem("Tools/Texture2D to Equirectangular")]
    static void CreateWizard()
    {
        DisplayWizard<Texture2DToEquirectangularWizard>("Texture2D to Equirectangular", "Convert");
    }

    void OnWizardCreate()
    {
        bool valid = true;

        if (texture == null)
        {
            Debug.LogWarning("You must specify a texture");
            valid = false;
        }
        else if (outputWidth < 1)
        {
            Debug.LogWarning("Width must be greater than 0");
            valid = false;
        }
        else if (outputHeight < 1)
        {
            Debug.LogWarning("Height must be greater than 0");
            valid = false;
        }

        if (valid)
        {
            byte[] bytes = CubemapConverter.ConvertToEquirectangular(texture, outputWidth, outputHeight);

            string assetPath = AssetDatabase.GetAssetPath(texture);
            string assetDirectory = Path.GetDirectoryName(assetPath);
            string assetName = Path.GetFileNameWithoutExtension(assetPath) + "_equirectangular.png";
            string textureAsset = Path.Combine(assetDirectory, assetName);
            File.WriteAllBytes(textureAsset, bytes);

            AssetDatabase.ImportAsset(textureAsset);

            Debug.Log("Equirectangular asset successfully saved to " + textureAsset);
        }
    }
}