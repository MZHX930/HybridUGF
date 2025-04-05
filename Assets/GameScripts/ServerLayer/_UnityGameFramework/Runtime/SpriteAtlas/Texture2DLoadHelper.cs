using GameFramework;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = System.Object;

namespace UnityGameFramework.Runtime
{
    public sealed class Texture2DLoadHelper : MonoBehaviour
    {
        private string m_Texture2dRootPath;
        private ResourceComponent m_ResourceComponent = null;

        /// <summary>
        /// 正在加载中
        /// </summary>
        private Dictionary<string, List<CachedInvokeInfo>> m_WaitLoadTextureDic = new Dictionary<string, List<CachedInvokeInfo>>();
        /// <summary>
        /// 缓存的
        /// </summary>
        private Dictionary<string, Texture2D> m_CachedTexture2DDic = new Dictionary<string, Texture2D>();

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("TextureLoadHelper Resource component init invalid.");
                return;
            }
        }

        /// <summary>
        /// 设置搜索路径
        /// </summary>
        public void SetSearchPath(string searchPath)
        {
            if (string.IsNullOrEmpty(searchPath))
            {
                Log.Fatal("Texture2DLoadHelper search paths is invalid.");
            }
            else
            {
                m_Texture2dRootPath = searchPath;
            }
        }

        /// <summary>
        /// 加载2d图片
        /// </summary>
        /// <param name="textureName">图片名称</param>
        /// <param name="callBack">加载成功后的回调</param>
        public void LoadTexture2D(string textureName, Action<Texture2D, System.Object> callBack, System.Object userData)
        {
            if (null == m_ResourceComponent)
            {
                Log.Fatal("Texture2DLoadHelper Resource component is null.");
                return;
            }

            if (m_CachedTexture2DDic.ContainsKey(textureName))
            {
                callBack?.Invoke(m_CachedTexture2DDic[textureName], userData);
            }
            else
            {
                string uiSpritePath = Path.Combine(m_Texture2dRootPath, textureName).Replace("\\", "/");
                uiSpritePath += ".png";

                var result = m_ResourceComponent.HasAsset(uiSpritePath);
                if (result == HasAssetResult.NotExist)
                {
                    Log.Fatal("AtlasLoadHepler not find UISprite [{0}] ", uiSpritePath);
                    return;
                }

                if (m_WaitLoadTextureDic.ContainsKey(uiSpritePath))
                {
                    m_WaitLoadTextureDic[uiSpritePath].Add(new CachedInvokeInfo(callBack, userData));
                }
                else
                {
                    //新增缓存      
                    m_WaitLoadTextureDic.Add(uiSpritePath, new List<CachedInvokeInfo>());
                    m_WaitLoadTextureDic[uiSpritePath].Add(new CachedInvokeInfo(callBack, userData));

                    m_ResourceComponent.LoadAsset(uiSpritePath, int.MaxValue,
                        new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback));
                }
            }
        }

        private void LoadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            Texture2D texture = asset as Texture2D;
            if (texture == null)
            {
                Log.Error("TextureLoadHelper load failure: {0}.\nasset is not Texture2D", assetName);
                return;
            }

            if (!m_CachedTexture2DDic.ContainsKey(assetName))
            {
                m_CachedTexture2DDic.Add(assetName, texture);
            }

            if (m_WaitLoadTextureDic.ContainsKey(assetName))
            {
                var list = m_WaitLoadTextureDic[assetName];
                foreach (var item in list)
                {
                    item.CallBack?.Invoke(texture, item.UserData);
                }
                m_WaitLoadTextureDic.Remove(assetName);
            }
        }

        private void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error("Texture2D helper load atlas failure: {0}.\nstatus={1}, errorMessage={2}", assetName, status.ToString(), errorMessage);
        }

        public void ClearCache()
        {
            if (m_CachedTexture2DDic.Count > 30)
            {
                m_CachedTexture2DDic.Clear();
                m_ResourceComponent.UnloadUnusedAssets(true);
            }
        }

        public struct CachedInvokeInfo
        {
            public Action<Texture2D, Object> CallBack;
            public Object UserData;

            public CachedInvokeInfo(Action<Texture2D, Object> callBack, Object userData)
            {
                CallBack = callBack;
                UserData = userData;
            }
        }
    }
}