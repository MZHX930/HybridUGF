using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 场景流程基类
    /// </summary>
    public abstract class ProcedureChangeScene : ProcedureBase
    {
        /// <summary>
        /// 背景音乐
        /// </summary>
        protected int BackgroundMusicId { get; private set; } = 0;
        /// <summary>
        /// 场景配置
        /// </summary>
        protected DRDefineScene SceneConfig { get; private set; } = null;

        protected Texture2D[][] GroundTextures { get; private set; } = null;

        /// <summary>
        /// 场景是否加载完成
        /// </summary>
        protected bool IsSceneLoadOk = false;
        /// <summary>
        /// 预加载的资源
        /// </summary>
        protected Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            IsSceneLoadOk = false;
            LoadedFlag.Clear();

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnLoadEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnLoadEntityFailure);

            // 停止所有声音
            GameEntry.Sound.StopAllLoadingSounds();
            GameEntry.Sound.StopAllLoadedSounds();

            // 隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HideAllLoadedEntities();

            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            }

            // 还原游戏速度
            GameEntry.Base.ResetNormalGameSpeed();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnLoadEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnLoadEntityFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        /// <summary>
        /// 开始加载场景
        /// </summary>
        protected void LoadSceneAssets(int sceneDrId)
        {
            SceneConfig = GameEntry.DataTable.GetDataTable<DRDefineScene>().GetDataRow(sceneDrId);
            if (SceneConfig == null)
            {
                Log.Error("Can not load scene '{0}' from data table.", sceneDrId.ToString());
                return;
            }

            GameEntry.Scene.LoadScene(AssetPathUtility.GetSceneAsset(SceneConfig.SceneName), Constant.AssetPriority.SceneAsset, this);
            PlayBackgroundMusic(SceneConfig.MusicId);

            List<DRDefineTerrain> terrainStyleList = GameEntry.DataTable.GetSceneTerrainStyle(SceneConfig.StyleId);
            GroundTextures = new Texture2D[terrainStyleList.Count][];

            foreach (var styleConfig in terrainStyleList)
            {
                GroundTextures[styleConfig.Scroll - 1] = new Texture2D[3];
                for (int i = 0; i < 3; i++)
                {
                    LoadSceneTerrainGrid(styleConfig, i);
                }
            }
        }

        private void LoadSceneTerrainGrid(DRDefineTerrain config, int arrayIndex)
        {
            string spritePath = "Terrain/";
            switch (arrayIndex)
            {
                case 0:
                    spritePath += config.Layer0;
                    break;
                case 1:
                    spritePath += config.Layer2;
                    break;
                case 2:
                    spritePath += config.Layer7;
                    break;
                default:
                    Log.Error($"加载场景地图方块时参数错误,arrayIndex={arrayIndex}");
                    break;
            }

            string loadTag = $"{config.Id}_{arrayIndex}";
            AddLoadFlag(loadTag);

            GameEntry.Sprites.LoadTexture2D(spritePath, (texture, userData) =>
            {
                TerrainLoadUserData tmpData = userData as TerrainLoadUserData;
                GroundTextures[tmpData.Scroll - 1][tmpData.ArrayIndex] = texture;
                UpdateAssetLoadFlag(tmpData.LoadTag);
                ReferencePool.Release(tmpData);

            }, TerrainLoadUserData.Create(loadTag, config.Scroll, arrayIndex));
        }

        /// <summary>
        /// 当资源全部加载结束时
        /// </summary>
        protected virtual void OnLoadAssetsComplete() { }

        protected virtual void OnLoadEntitySuccess(ShowEntitySuccessEventArgs args)
        {
            UpdateAssetLoadFlag(args.Entity.Id.ToString());
        }

        #region 事件监听函数
        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
            IsSceneLoadOk = true;
            CheckLoadAssetsComplete();
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
            IsSceneLoadOk = false;
        }

        private void OnLoadEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Error("Load entity '{0}' failure, error message '{1}'.", ne.EntityAssetName, ne.ErrorMessage);
        }

        private void OnLoadEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            OnLoadEntitySuccess(ne);
        }
        #endregion

        protected void AddLoadFlag(string assetName)
        {
            if (!LoadedFlag.ContainsKey(assetName))
            {
                LoadedFlag.Add(assetName, false);
            }
        }

        private void UpdateAssetLoadFlag(string assetName)
        {
            if (LoadedFlag.ContainsKey(assetName))
            {
                LoadedFlag[assetName] = true;
            }
            else
            {
                return;
            }

            CheckLoadAssetsComplete();
        }

        private void CheckLoadAssetsComplete()
        {
            if (!IsSceneLoadOk)
                return;

            foreach (var item in LoadedFlag)
            {
                if (!item.Value)
                    return;
            }

            OnLoadAssetsComplete();
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        private void PlayBackgroundMusic(int musicId)
        {
            if (BackgroundMusicId == musicId)
                return;

            BackgroundMusicId = musicId;
            GameEntry.Sound.PlayMusic(BackgroundMusicId);
        }
    }

    public class TerrainLoadUserData : IReference
    {
        public int Scroll;
        public int ArrayIndex;
        public string LoadTag;

        public void Clear()
        {
            Scroll = default;
            ArrayIndex = default;
            LoadTag = null;
        }

        public static TerrainLoadUserData Create(string loadTag, int scroll, int arrayIndex)
        {
            var data = ReferencePool.Acquire<TerrainLoadUserData>();
            data.LoadTag = loadTag;
            data.Scroll = scroll;
            data.ArrayIndex = arrayIndex;

            return data;
        }
    }
}
