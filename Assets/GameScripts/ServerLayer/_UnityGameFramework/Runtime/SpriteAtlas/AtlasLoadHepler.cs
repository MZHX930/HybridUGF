using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 图集加载辅助器
    /// </summary>
    [Serializable]
    public sealed class AtlasLoadHepler : MonoBehaviour
    {
        private ResourceComponent mResourceComponent = null;

        /// <summary>
        /// 缓存的图集
        /// </summary>
        private Dictionary<string, SpriteAtlas> m_CachedAtlasDic = new Dictionary<string, SpriteAtlas>();

        /// <summary>
        /// 缓存的图片
        /// </summary>
        private Dictionary<string, Sprite> m_CachedSpriteDic = new Dictionary<string, Sprite>();

        /// <summary>
        /// 索引路径
        /// </summary>
        private string mAtlasSearchPath;
        private string mUISpritesSearchPath;

        private void Awake()
        {
            //调用 SpriteAtlasManager.atlasRequested 后的回调注册 SpriteAtlas 时触发。
            SpriteAtlasManager.atlasRegistered += SpriteAtlasManager_AtlasRegistered;
            //当Unity实例化Sprite时，但无法找到图集资源时触发。
            SpriteAtlasManager.atlasRequested += SpriteAtlasManager_AtlasRequested;
        }

        private void Start()
        {
            mResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (mResourceComponent == null)
            {
                Log.Fatal("SpriteAtlasLoadHepler Resource component init invalid.");
                return;
            }
        }

        private void OnDestroy()
        {
            SpriteAtlasManager.atlasRegistered -= SpriteAtlasManager_AtlasRegistered;
            SpriteAtlasManager.atlasRequested -= SpriteAtlasManager_AtlasRequested;
        }

        /// <summary>
        /// 设置搜索路径
        /// </summary>
        public void SetSearchPath(string atlasAssetsPath, string uiSpritesPath)
        {
            if (string.IsNullOrEmpty(atlasAssetsPath))
            {
                Log.Fatal("AtlasLoadHepler Atlas search paths is invalid.");
            }
            else
            {
                mAtlasSearchPath = atlasAssetsPath;
            }

            if (string.IsNullOrEmpty(uiSpritesPath))
            {
                Log.Fatal("AtlasLoadHepler UISprites search paths is invalid.");
            }
            else
            {
                mUISpritesSearchPath = uiSpritesPath;
            }
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        /// <param name="atlasName">图集文件名</param>
        /// <param name="callback">加载成功后的回调</param>
        public void LoadAtlas(string atlasName, Action<SpriteAtlas> callback)
        {
            InnerLoadAtlasAssets(atlasName, callback);
        }

        /// <summary>
        /// 获取图集中的图片
        /// 实际是直接加载UISprites中Sprite，不从Atlas中Clone！
        /// </summary>
        /// <param name="atlasName">图集文件名</param>
        /// <param name="spriteName">图片名</param>
        /// <param name="callBack">加载成功后的回调</param>
        public void LoadAtlasSprite(string atlasName, string spriteName, Action<Sprite> callBack)
        {
            if (null == mResourceComponent)
            {
                Log.Fatal("AtlasLoadHepler Resource component is null.");
                return;
            }

            if (string.IsNullOrEmpty(atlasName))
            {
                Log.Fatal("AtlasLoadHepler atlasName is null.");
                return;
            }

            if (string.IsNullOrEmpty(spriteName))
            {
                Log.Fatal("AtlasLoadHepler spriteName is null.");
                return;
            }

            if (m_CachedAtlasDic.ContainsKey(atlasName))
            {
                //这里需要缓存下，否则会一直创建新的Sprite
                string spritekey = $"{atlasName}_{spriteName}";
                if (m_CachedSpriteDic.ContainsKey(spritekey))
                {
                    callBack?.Invoke(m_CachedSpriteDic[spritekey]);
                }
                else
                {
                    Sprite sprite = m_CachedAtlasDic[atlasName].GetSprite(spriteName);
                    m_CachedSpriteDic.Add(spritekey, sprite);
                    callBack?.Invoke(sprite);
                }
            }
            else
            {
                string uiSpritePath = Path.Combine(mUISpritesSearchPath, atlasName, spriteName).Replace("\\", "/");
                uiSpritePath += ".png";
                var result = mResourceComponent.HasAsset(uiSpritePath);
                if (result == HasAssetResult.NotExist)
                {
                    Log.Fatal("AtlasLoadHepler not find UISprite [{0}] ", uiSpritePath);
                }

                LoadUISpriteUserData userData = new LoadUISpriteUserData();
                userData.spriteName = spriteName;
                userData.callback = callBack;
                mResourceComponent.LoadAsset(uiSpritePath, int.MaxValue, new LoadAssetCallbacks(LoadUISpriteSuccessCallback, LoadUISpriteFailureCallback), userData);
            }
        }

        /// <summary>
        /// 请求加载图集
        /// </summary>
        /// <param name="atlasName">图集文件名</param>
        /// <param name="callback">加载成功后的回调</param>
        private void SpriteAtlasManager_AtlasRequested(string atlasName, Action<SpriteAtlas> callback)
        {
            if (m_CachedAtlasDic.ContainsKey(atlasName))
            {
                callback?.Invoke(m_CachedAtlasDic[atlasName]);
                return;
            }
            InnerLoadAtlasAssets(atlasName, callback);
        }

        /// <summary>
        /// 图集加载成功后调用
        /// InClude in Build也会调用这里
        /// </summary>
        private void SpriteAtlasManager_AtlasRegistered(SpriteAtlas loadedAtlas)
        {
            string atlasName = loadedAtlas.name;
            if (!m_CachedAtlasDic.ContainsKey(atlasName))
                m_CachedAtlasDic.Add(atlasName, loadedAtlas);
        }

        private void InnerLoadAtlasAssets(string atlasName, Action<SpriteAtlas> callback)
        {
            if (null == mResourceComponent)
            {
                Log.Fatal("AtlasLoadHepler Resource component is null.");
                return;
            }

            string atlasPath = Path.Combine(mAtlasSearchPath, atlasName);
            if (!atlasPath.EndsWith(".spriteatlasv2"))
            {
                atlasPath += ".spriteatlasv2";
            }
            atlasPath = atlasPath.Replace("\\", "/");
            var result = mResourceComponent.HasAsset(atlasPath);
            if (result == HasAssetResult.NotExist)
            {
                Log.Fatal("AtlasLoadHepler not find atlas [{0}] in path [{1}]", atlasName, atlasPath);
                return;
            }

            LoadAtlasUserData userData = new LoadAtlasUserData();
            userData.atlasName = atlasName;
            userData.callback = callback;
            mResourceComponent.LoadAsset(atlasPath, int.MaxValue, new LoadAssetCallbacks(LoadAtlasSuccessCallback, LoadAtlasFailureCallback), userData);
        }

        private void LoadAtlasSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            SpriteAtlas atlas = asset as SpriteAtlas;
            if (atlas == null)
            {
                Log.Error("AtlasLoadHepler load atlas failure: {0}.\nasset is not SpriteAtlas", assetName);
                return;
            }
            LoadAtlasUserData loadAtlasUserData = userData as LoadAtlasUserData;
            if (loadAtlasUserData.atlasName != atlas.name)
            {
                Log.Error("AtlasLoadHepler load atlas failure. Target [{0}] is not User [{1}]", assetName, loadAtlasUserData.atlasName);
                return;
            }

            if (!m_CachedAtlasDic.ContainsKey(atlas.name))
                m_CachedAtlasDic.Add(atlas.name, atlas);

            loadAtlasUserData.callback?.Invoke(atlas);
        }

        private void LoadAtlasFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error("AtlasLoadHepler load atlas failure: {0}.\nstatus={1}, errorMessage={2}", assetName, status.ToString(), errorMessage);
        }


        private void LoadUISpriteSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            Texture2D texture = asset as Texture2D;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            // Sprite sprite = asset as Sprite;
            // if (sprite == null)
            // {
            //     Log.Error("AtlasLoadHepler load uiSprite failure: {0}.\nasset is not Sprite", assetName);
            //     return;
            // }
            LoadUISpriteUserData loadUserData = userData as LoadUISpriteUserData;
            loadUserData.callback?.Invoke(sprite);
        }

        private void LoadUISpriteFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error("AtlasLoadHepler load uiSprite failure: {0}.\nstatus={1}, errorMessage={2}", assetName, status.ToString(), errorMessage);
        }

        public class LoadAtlasUserData
        {
            public string atlasName;
            public Action<SpriteAtlas> callback;
        }

        public class LoadUISpriteUserData
        {
            public string spriteName;
            public Action<Sprite> callback;
        }
    }
}