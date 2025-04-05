using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public static class ImportAssetsHelper
    {
        #region 纹理导入处理器
        public class TextureImportProcessor : AssetPostprocessor
        {
            private const string c_TextureRootPath = "Assets/GameAssets/Textures";
            private const string c_UISpriteRootPath = "Assets/GameAssets/UISprites";

            private void OnPreprocessTexture()
            {
                var importer = assetImporter as TextureImporter;
                if (importer == null)
                    return;

                if (!string.IsNullOrEmpty(importer.userData))
                    return;
                if (!IsInTargetFolder(importer.assetPath))
                    return;

                SetBaseTextureSettings(importer);
                SetPlatformTextureSettings(importer);

                importer.userData = "ProcessedByTextureImportProcessor";
            }

            private bool IsInTargetFolder(string assetPath)
            {
                return assetPath.StartsWith(c_TextureRootPath) ||
                       assetPath.StartsWith(c_UISpriteRootPath);
            }

            private void SetBaseTextureSettings(TextureImporter importer)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.textureShape = TextureImporterShape.Texture2D;
                importer.mipmapEnabled = false;
                // 禁用所有sprite的physics shape生成
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                // 禁用Physics Shape生成
                if (settings.spriteGenerateFallbackPhysicsShape)
                {
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    importer.SetTextureSettings(settings);

                    // 标记需要重新导入资源
                    importer.SaveAndReimport();
                }
            }


            private void SetPlatformTextureSettings(TextureImporter importer)
            {
                // Android平台设置
                var android = importer.GetPlatformTextureSettings("Android");
                android.overridden = true;
                if (importer.assetPath.StartsWith(c_UISpriteRootPath))
                    android.format = TextureImporterFormat.ASTC_8x8;
                else
                    android.format = TextureImporterFormat.ETC2_RGBA8;
                android.compressionQuality = 50;
                importer.SetPlatformTextureSettings(android);

                // iOS平台设置
                var ios = importer.GetPlatformTextureSettings("iPhone");
                ios.overridden = true;
                ios.format = TextureImporterFormat.PVRTC_RGBA4;
                ios.compressionQuality = 50;
                importer.SetPlatformTextureSettings(ios);
            }
        }
        #endregion
    }
}
