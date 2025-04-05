
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;
using Cysharp;
using Cysharp.Threading.Tasks;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 图集池组件
    /// https://docs.unity3d.com/ScriptReference/U2D.SpriteAtlasManager.html
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/SpriteAtlasComponent")]
    public sealed class SpritesComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 图集辅助器
        /// </summary>
        private AtlasLoadHepler mAtlasLoadHelper;
        /// <summary>
        /// 图集Assets路径
        /// </summary>
        [SerializeField]
        private string m_AtlasSearchPath;
        [SerializeField]
        /// <summary>
        /// 散图Assets路径
        /// </summary>
        private string m_UISpritesSearchPath;

        /// <summary>
        /// 图片辅助器
        /// </summary>
        private Texture2DLoadHelper mTexture2DLoadHelper;
        /// <summary>
        /// 2d图片资源路径
        /// </summary>
        [SerializeField]
        private string m_Texture2DAssetsRootPath;
        /// <summary>
        /// 清理缓存间隔
        /// </summary>
        [SerializeField]
        private float m_ClearCacheTime = 600;
        private float m_ClearTimeElapsed = 0;

        protected override void Awake()
        {
            base.Awake();
            if (null == mAtlasLoadHelper)
            {
                var objHelper = new GameObject("SpriteAtlasLoadHelper");
                objHelper.transform.SetParent(transform);
                mAtlasLoadHelper = objHelper.AddComponent<AtlasLoadHepler>();
                mAtlasLoadHelper.SetSearchPath(m_AtlasSearchPath, m_UISpritesSearchPath);
            }

            if (null == mTexture2DLoadHelper)
            {
                var objHelper = new GameObject("Texture2DLoadHelper");
                objHelper.transform.SetParent(transform);
                mTexture2DLoadHelper = objHelper.AddComponent<Texture2DLoadHelper>();
                mTexture2DLoadHelper.SetSearchPath(m_Texture2DAssetsRootPath);
            }
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        public void LoadAtlas(string atlasName, Action<SpriteAtlas> callback)
        {
            if (null == mAtlasLoadHelper)
            {
                Log.Error("SpriteAtlasLoadHelper is null");
                return;
            }
            mAtlasLoadHelper.LoadAtlas(atlasName, callback);
        }

        /// <summary>
        /// 获取图集中的图片
        /// </summary>
        /// <param name="atlasName">图集名称</param>
        /// <param name="spriteName">图片名称</param>
        /// <param name="callBack">回调</param>
        public void LoadAtlasSprite(string atlasName, string spriteName, Action<Sprite> callBack)
        {
            if (null == mAtlasLoadHelper)
            {
                Log.Error("SpriteAtlasLoadHelper is null");
                return;
            }
            mAtlasLoadHelper.LoadAtlasSprite(atlasName, spriteName, callBack);
        }

        /// <summary>
        /// 加载2d图片
        /// </summary>
        public void LoadTexture2D(string textureName, Action<Texture2D, System.Object> callBack, System.Object userData)
        {
            if (null == mAtlasLoadHelper)
            {
                Log.Error("mTexture2DLoadHelper is null");
                return;
            }
            mTexture2DLoadHelper.LoadTexture2D(textureName, callBack, userData);
        }

        void Update()
        {
            m_ClearTimeElapsed += Time.unscaledDeltaTime;
            if (m_ClearTimeElapsed >= m_ClearCacheTime)
            {
                m_ClearTimeElapsed = 0;
                mTexture2DLoadHelper.ClearCache();
            }
        }
    }
}
