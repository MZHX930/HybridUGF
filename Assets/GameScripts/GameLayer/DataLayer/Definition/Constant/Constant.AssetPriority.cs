namespace GameDevScript
{
    public static partial class Constant
    {
        /// <summary>
        /// 资源优先级。
        /// 值越小越先加载
        /// </summary>
        public static class AssetPriority
        {
            public const int ConfigAsset = 0;
            public const int DataTableAsset = 100;
            public const int FontAsset = 200;
            public const int DictionaryAsset = 300;
            public const int SceneAsset = 0;
            public const int UIFormAsset = 100;
            public const int MusicAsset = 500;
            public const int SoundAsset = 600;
            public const int UISoundAsset = 700;


            public const int CharacterShellAsset = 100;
            public const int CharacterViewAsset = 200;
            public const int SkillShellAsset = 300;
            public const int SkillViewAsset = 400;
            public const int BuildingShellAsset = 300;
            public const int BuildingViewAsset = 300;
        }
    }
}
