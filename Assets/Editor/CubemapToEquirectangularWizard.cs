using System.IO;
using UnityEditor;
using UnityEngine;

class CubemapToEquirectangularWizard : ScriptableWizard
{
    public Cubemap cubemap = null;
    public int outputWidth = 4096;
    public int outputHeight = 2048;

    private Shader conversionShader;
    private Material conversionMaterial;

    [MenuItem("Tools/Cubemap to Equirectangular")]
    static void CreateWizard()
    {
        DisplayWizard<CubemapToEquirectangularWizard>("Cubemap to Equirectangular", "Convert");
    }

    void OnWizardCreate()
    {
        bool valid = true;

        conversionShader = Shader.Find("Conversion/CubemapToEquirectangular");
        if (conversionShader == null)
        {
            Debug.LogWarning("Unable to find shader");
            valid = false;
        }
        else
        {
            conversionMaterial = new Material(conversionShader);
        }

        if (cubemap == null)
        {
            Debug.LogWarning("You must specify a cubemap");
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
            //Change to gamma color space
            //http://docs.unity3d.com/Manual/LinearLighting.html
            ColorSpace initialColorSpace = PlayerSettings.colorSpace;
            PlayerSettings.colorSpace = ColorSpace.Gamma;

            RenderTexture renderTexture = new RenderTexture(outputWidth, outputHeight, 24);
            Graphics.Blit(cubemap, renderTexture, conversionMaterial);

            Texture2D equirectangularTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);

            equirectangularTexture.ReadPixels(new Rect(0, 0, outputWidth, outputHeight), 0, 0, false);
            equirectangularTexture.Apply();

            byte[] bytes = equirectangularTexture.EncodeToPNG();

            DestroyImmediate(equirectangularTexture);

            string assetPath = AssetDatabase.GetAssetPath(cubemap);
            string assetDir = Path.GetDirectoryName(assetPath);
            string assetName = Path.GetFileNameWithoutExtension(assetPath) + "_equirectangular.png";
            string textureAsset = Path.Combine(assetDir, assetName);
            File.WriteAllBytes(textureAsset, bytes);

            AssetDatabase.ImportAsset(textureAsset);

            Debug.Log("Equirectangular asset successfully saved to " + textureAsset);

            //Revert color space
            PlayerSettings.colorSpace = initialColorSpace;
        }
    }
}