using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 游戏数据存档静态管理器
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/GameArchiveComponent")]
    public partial class GameArchiveComponent : GameFrameworkComponent
    {
        private string m_GameArchiveDataKey = "GameArchiveDataKey";

        /// <summary>
        /// 存档事件
        /// </summary>
        private event Action<SaveArchiveReasonTypeEnum> m_OnSaveEvent;

        /// <summary>
        /// 游戏存档数据
        /// </summary>
        public GameArchiveData Data { get; private set; }

        /// <summary>
        /// 注册存档事件，当触发存档时调用这些函数
        /// </summary>
        public void RegisterSaveEvent(System.Action<SaveArchiveReasonTypeEnum> onSaveFunc)
        {
            m_OnSaveEvent += onSaveFunc;
        }

        /// <summary>
        /// 注销存档事件
        /// </summary>
        public void UnregisterArchiveEvent(System.Action<SaveArchiveReasonTypeEnum> onSaveFunc)
        {
            m_OnSaveEvent -= onSaveFunc;
        }

        /// <summary>
        /// 加载本地存档数据
        /// </summary>
        public void LoadLocalArchiveData()
        {
            if (GameEntry.Setting.HasSetting(m_GameArchiveDataKey))
            {
                Data = GameEntry.Setting.GetObject<GameArchiveData>(m_GameArchiveDataKey);
            }
            else
            {
                Data = new GameArchiveData();
                Data.LastSaveAppVersion = Application.version;
                Data.GlobalRandomSeed = System.DateTime.Now.Millisecond * (SystemInfo.graphicsDeviceID == 0 ? Utility.Random.GetRandom(100, 1000) : SystemInfo.graphicsDeviceID);
                Data.GlobalRandomState = UnityEngine.Random.state;
            }
            //设置随机
            UnityEngine.Random.InitState(Data.GlobalRandomSeed);
            UnityEngine.Random.state = Data.GlobalRandomState;

            GameEntry.Event.Fire(this, SuccessLoadArchiveEventArgs.Create());
        }

        /// <summary>
        /// 在重要节点处记录随机进程
        /// 比如每章节的小节结束时
        /// </summary>
        public void RecordRandomState()
        {
            Data.GlobalRandomState = UnityEngine.Random.state;
        }

        /// <summary>
        /// 保存到本地存档
        /// </summary>
        private void SaveToLocalArchiveData(SaveArchiveReasonTypeEnum reasonType)
        {
            if (null == Data)
                return;

            m_OnSaveEvent?.Invoke(reasonType);

            //写入本地
            GameEntry.Setting.SetObject(m_GameArchiveDataKey, Data);
        }


        private void OnApplicationQuit()
        {
            SaveToLocalArchiveData(SaveArchiveReasonTypeEnum.OnApplicationQuit);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SaveToLocalArchiveData(pauseStatus ? SaveArchiveReasonTypeEnum.OnApplicationPause_True : SaveArchiveReasonTypeEnum.OnApplicationPause_False);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SaveToLocalArchiveData(hasFocus ? SaveArchiveReasonTypeEnum.OnApplicationFocus_True : SaveArchiveReasonTypeEnum.OnApplicationFocus_False);
        }
    }
}
