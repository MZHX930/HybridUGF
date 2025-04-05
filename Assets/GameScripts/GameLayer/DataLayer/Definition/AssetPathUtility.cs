using GameFramework;

namespace GameDevScript
{
    public static class AssetPathUtility
    {
        public static string GetTMPSettingsAsset()
        {
            return "Assets/GameAssets/TMPResources/TMP Settings.asset";
        }

        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameAssets/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(int testGroup, string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/DataTables/{0}/{1}.txt", testGroup, assetName);
        }

        public static string GetLocalizationLanguageAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameAssets/Localization/{0}.{1}", assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/UI/UISounds/{0}.wav", assetName);
        }

        /// <summary>
        /// 获取怪物的容器
        /// </summary>
        public static string GetMonsterShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/MonsterShell.prefab";
        }

        /// <summary>
        /// 获取英雄的容器
        /// </summary>
        public static string GetHeroShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/HeroShell.prefab";
        }

        /// <summary>
        /// 获取士兵的容器
        /// </summary>
        public static string GetSoldierShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/SoldierShell.prefab";
        }

        /// <summary>
        /// 获取载具的容器
        /// </summary>
        public static string GetVehicleShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/VehicleShell.prefab";
        }

        /// <summary>
        /// 获取子弹壳体的路径
        /// </summary>
        public static string GetBulletShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/BulletShell.prefab";
        }

        /// <summary>
        /// 获取Aoe壳体的路径
        /// </summary>
        public static string GetAoEShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/AoEShell.prefab";
        }

        /// <summary>
        /// 获取建筑壳体的路径
        /// </summary>
        public static string GetBuildingShellPath()
        {
            return "Assets/GameAssets/Entities/Shell/BuildingShell.prefab";
        }

        /// <summary>
        /// 获取视图模型的路径
        /// </summary>
        public static string GetPrefabViewPath(string assetName)
        {
            return Utility.Text.Format("Assets/GameAssets/Entities/View/{0}.prefab", assetName);
        }
    }
}

