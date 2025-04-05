using GameFramework.Event;
using GameFramework.Resource;
using System.Collections.Generic;
using TMPro;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 加载基础资源：
    /// 1.配置表
    /// 2.多语言
    /// 3.图集
    /// </summary>
    public class ProcedurePreloadAssets : ProcedureBase
    {
        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            m_LoadedFlag.Clear();
            PreloadResources();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
            GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
            GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            foreach (KeyValuePair<string, bool> loadedFlag in m_LoadedFlag)
            {
                if (!loadedFlag.Value)
                {
                    return;
                }
            }

            ChangeState<ProcedurePreloadGameDataBeforeArchive>(procedureOwner);
        }

        private void PreloadResources()
        {
            //加载TMP_Settings
            LoadTMPSettings();

            //加载配置表
            string[] dataTableNames = GameEntry.Config.GetString("DataTableNames").Split(',');
            foreach (string dataTableName in dataTableNames)
            {
                LoadDataTable(dataTableName);
            }

            //加载多语言
            LoadLocalizationLanguage("English");

            //加载图集
            string[] preSpriteAtlasNames = GameEntry.Config.GetString("PreSpriteAtlasNames").Split(',');
            foreach (string atlasName in preSpriteAtlasNames)
            {
                m_LoadedFlag.Add(atlasName, false);
                GameEntry.Sprites.LoadAtlas(atlasName, v =>
                {
                    m_LoadedFlag[atlasName] = true;
                });
            }
        }

        private void LoadTMPSettings()
        {
            string tmpSettingsAssetName = AssetPathUtility.GetTMPSettingsAsset();
            m_LoadedFlag.Add(tmpSettingsAssetName, false);

            void OnLoadTMPSettingsSuccess(string assetName, object asset, float duration, object userData)
            {
                m_LoadedFlag[assetName] = true;
                Log.Info("Load TMP_Settings OK.");

                TMP_Settings tmpSettings = asset as TMP_Settings;
                if (tmpSettings != null)
                {
                    TMP_Settings.instance = tmpSettings;
                }
                else
                {
                    throw new System.Exception("Load TMP_Settings Failure!!!");
                }
            }

            void OnLoadTMPSettingsFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
            {
                Log.Error("Can not load TMP_Settings from '{0}' with error message '{1}'.", assetName, errorMessage);
            }

            GameEntry.Resource.LoadAsset(tmpSettingsAssetName, new LoadAssetCallbacks(OnLoadTMPSettingsSuccess, OnLoadTMPSettingsFailure));
        }

        private void LoadDataTable(string dataTableName)
        {
            if (string.IsNullOrEmpty(dataTableName) || string.IsNullOrWhiteSpace(dataTableName))
                return;

            string dataTableAssetName = AssetPathUtility.GetDataTableAsset(0, dataTableName);
            m_LoadedFlag.Add(dataTableAssetName, false);
            GameEntry.DataTable.LoadDataTable(dataTableName, dataTableAssetName, this);
        }

        private void LoadLocalizationLanguage(string dictionaryName)
        {
            string dictionaryAssetName = AssetPathUtility.GetLocalizationLanguageAsset(dictionaryName, false);
            m_LoadedFlag.Add(dictionaryAssetName, false);
            GameEntry.Localization.ReadData(dictionaryAssetName, this);
        }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DataTableAssetName] = true;
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        private void OnLoadDataTableFailure(object sender, GameEventArgs e)
        {
            LoadDataTableFailureEventArgs ne = (LoadDataTableFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName, ne.DataTableAssetName, ne.ErrorMessage);
        }

        private void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DictionaryAssetName] = true;
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }
    }
}
