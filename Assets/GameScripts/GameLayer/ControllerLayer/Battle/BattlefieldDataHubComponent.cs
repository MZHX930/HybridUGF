using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 战场数据中心
    /// 负责记录变化的零散数据
    /// > 强化列表
    /// > 事件列表
    /// </summary>
    [DisallowMultipleComponent]
    public class BattlefieldDataHubComponent : GameFrameworkComponent
    {
        #region 资源相关
        /// <summary>
        /// 银币
        /// </summary>
        public int SilverCount { get; private set; } = 0;

        /// <summary>
        /// 选项经验值
        /// 满了后显示强化面板，重置
        /// </summary>
        public int CurOptionExp { get; private set; } = 0;

        private int m_CurOptionExpLv = 0;
        /// <summary>
        /// 当前选项经验等级
        /// </summary>
        public int CurOptionExpLv
        {
            get
            {
                return m_CurOptionExpLv;
            }
            private set
            {
                DRDataIntensifyOptionExp dtrOptionExp = GameEntry.DataTable.GetDataTable<DRDataIntensifyOptionExp>().GetDataRow(value);
                if (dtrOptionExp == null)
                {
                    MaxOptionExp = int.MaxValue;
                    m_CurOptionExpLv = value;
                }
                else
                {
                    MaxOptionExp = dtrOptionExp.MaxExp;
                }
            }
        }

        /// <summary>
        /// 当前最大选项经验值
        /// </summary>
        public int MaxOptionExp { get; private set; } = 999999;
        #endregion


        #region 强化数据

        #endregion

        #region 事件数据

        #endregion

        /// <summary>
        /// 开始一个新的战场
        /// </summary>
        public void StartNew()
        {
            SilverCount = 0;
            CurOptionExp = 0;
            CurOptionExpLv = 0;
        }

        /// <summary>
        /// 退出战场，关闭中心
        /// </summary>
        public void CloseHub()
        {
        }

        #region 资源相关
        /// <summary>
        /// 变更银币
        /// </summary>
        /// <param name="modifyValue">变更值</param>
        /// <returns>变更后银币</returns>
        public int ModifySilverCount(int modifyValue)
        {
            return SilverCount += modifyValue;
        }

        /// <summary>
        /// 变更选项经验值
        /// </summary>
        /// <param name="modifyValue">变更值</param>
        /// <returns>变更后选项经验值</returns>
        public int ModifyOptionExp(int modifyValue)
        {
            CurOptionExp += modifyValue;
            if (CheckOptionExp())
            {
                GameEntry.Base.PauseGame();
            }
            return CurOptionExp;
        }

        private bool CheckOptionExp()
        {
            if (CurOptionExp >= MaxOptionExp)
            {
                CurOptionExp -= MaxOptionExp;
                CurOptionExpLv++;

                //显示强化选项选择面板
                int[] options = GameEntry.BDH.RandomIntensifyOptionIds();
                GameEntry.UI.OpenUIForm(SelectBuffsFormData.Create(options));

                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭强化选项选择面板
        /// </summary>
        public async UniTaskVoid OnCloseSelectBuffsForm()
        {
            await UniTask.DelayFrame(2);

            if (!CheckOptionExp())
            {
                GameEntry.Base.ResumeGame();
                GameEntry.UI.SetFullScreenMask(false);
            }
        }

        /// <summary>
        /// 设置选项经验等级
        /// </summary>
        public void SetOptionExpLv(int optionExpLv)
        {
            CurOptionExpLv = optionExpLv;
        }
        #endregion

        #region 三选一强化
        /// <summary>
        /// 强化选项的随机池
        /// </summary>
        private List<DRDefineIntensifyOption> m_IntenfiyOptionRandomPool = new List<DRDefineIntensifyOption>();
        //随机池中随机范围
        private int m_IntensifyOptionMaxRandomProb = 0;
        /// <summary>
        /// 已选的强化选项
        /// </summary>
        private Queue<int> m_SelectedOptionIds = new Queue<int>();
        /// <summary>
        /// 持续生效中的选项
        /// </summary>
        private Dictionary<EnumCharacterTag, List<int>> m_PermanentOptionDict = new Dictionary<EnumCharacterTag, List<int>>();

        /// <summary>
        /// 在BDH中生效的buff
        /// </summary>
        private List<BuffRTData> m_BDHPermanentBuffList = new List<BuffRTData>();

        /// <summary>
        /// 随机获得强化选项id组
        /// </summary>
        public int[] RandomIntensifyOptionIds()
        {
            return new int[] { 100001, 100002, 100003 };
        }

        /// <summary>
        /// 选择强化
        /// </summary>
        /// <param name="intensifyOptionId">DefineIntensifyOption表的id</param>
        public void SelectIntensifyOption(int intensifyOptionId)
        {
            Log.EDebug($"选择强化：{intensifyOptionId}");
            m_SelectedOptionIds.Enqueue(intensifyOptionId);
            var drtOptionConfig = GameEntry.DataTable.GetDataTable<DRDefineIntensifyOption>().GetDataRow(intensifyOptionId);

            //以第一个作为主检查标签
            EnumCharacterTag mainCheckTag = drtOptionConfig.BuffTargetTypes[0];
            switch (mainCheckTag)
            {
                case EnumCharacterTag.BDH:
                    {
                        AddBuff2BDH(drtOptionConfig);
                        break;
                    }
                case EnumCharacterTag.Monster:
                    {
                        break;
                    }
                case EnumCharacterTag.Hero:
                    {
                        break;
                    }
                case EnumCharacterTag.Soldier:
                    {
                        break;
                    }
                default:
                    {
                        Log.Error($"强化选项[{intensifyOptionId}]的主检查标签[{mainCheckTag}]未解析，请检查！");
                        return;
                    }
            }

            // //创建buff注入器
            // var addBuffInfo = new AddBuffInfo(
            //     GameEntry.DataTable.ToBuffModel(drtOptionConfig.BuffTemplateId),
            //     null,
            //     null,
            //     1,
            //     0,
            //     true,
            //     drtOptionConfig.BuffIsPermanent,
            //     drtOptionConfig.BuffParam
            // );
        }


        private void BuildAddBuffInfo(DRDefineIntensifyOption drtOptionConfig)
        {

        }

        /// <summary>
        /// 获取已选择的强化
        /// </summary>
        public List<int> GetSelectedIntensifyOptionIds()
        {
            return null;
        }


        ///<summary>
        ///为BDH添加加buff
        ///</summary>
        private void AddBuff2BDH(DRDefineIntensifyOption drtOptionConfig)
        {
            BuffRTData modifyBuffRtData = null;

            foreach (var buff in m_BDHPermanentBuffList)
            {
                if (buff.Model.BuffKey.Equals(drtOptionConfig.BuffTemplateId.ToString()))
                {
                    modifyBuffRtData = buff;
                    break;
                }
            }

            if (modifyBuffRtData != null)
            {
                //存在同类的buff
                modifyBuffRtData.BuffParam = new Dictionary<string, string>();
                if (drtOptionConfig.BuffParam != null)
                {
                    foreach (KeyValuePair<string, string> kv in drtOptionConfig.BuffParam)
                    {
                        modifyBuffRtData.BuffParam[kv.Key] = kv.Value;
                    }
                }

                modifyBuffRtData.Stack = 1;
                modifyBuffRtData.IsPermanent = drtOptionConfig.BuffIsPermanent;
            }


            //新建Buff
            BuffRTData newBuffRtData = new BuffRTData(
                  GameEntry.DataTable.ToBuffModel(drtOptionConfig.BuffTemplateId),
                  this,
                  this,
                  1,
                  1,
                  drtOptionConfig.BuffIsPermanent,
                  drtOptionConfig.BuffParam
              );

            if (drtOptionConfig.BuffIsPermanent)
            {
                m_BDHPermanentBuffList.Add(newBuffRtData);


            }
            else
            {

            }

            // List<BuffRTData> hasOnes = GetBuffById(buff.BuffModel.BuffKey, bCaster);
            // int modStack = Mathf.Min(buff.AddStack, buff.BuffModel.MaxStack);
            // bool toRemove = false;
            // BuffRTData newBuffRtData = null;
            // if (hasOnes.Count > 0)
            // {

            // }
            // else
            // {

            //     CtrlData.Buffs.Add(newBuffRtData);
            //     CtrlData.Buffs.Sort((a, b) =>
            //     {
            //         return a.model.Priority.CompareTo(b.model.Priority);
            //     });
            // }
            // if (toRemove == false && buff.BuffModel.OnOccurrence != null)
            // {
            //     buff.BuffModel.OnOccurrence(newBuffRtData, modStack);
            // }
            // CtrlData.AttrRecheck();
        }

        ///<summary>
        ///获取角色身上对应的buffObj
        ///<param name="buffKey">buff的model的id</param>
        ///<param name="caster">如果caster不是空，那么就代表只有buffObj.caster在caster里面的才符合条件</param>
        ///<return>符合条件的buffObj数组</return>
        ///</summary>
        private List<BuffRTData> GetBuffById(string buffKey, List<Entity> caster = null)
        {
            List<BuffRTData> res = new List<BuffRTData>();
            for (int i = 0; i < m_BDHPermanentBuffList.Count; i++)
            {
                if (m_BDHPermanentBuffList[i].Model.BuffKey == buffKey && (caster == null || caster.Count <= 0 || caster.Contains(m_BDHPermanentBuffList[i].Caster) == true))
                {
                    res.Add(m_BDHPermanentBuffList[i]);
                }
            }
            return res;
        }

        #endregion
    }


    public static class EnumUtility
    {
    }
}
