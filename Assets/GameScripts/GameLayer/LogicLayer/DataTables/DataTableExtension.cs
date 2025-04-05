using GameFramework.DataTable;
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public static class DataTableExtension
    {
        private const string DataRowClassPrefixName = "GameDevScript.DR";
        internal static readonly char[] DataSplitSeparators = new char[] { '\t' };
        internal static readonly char[] DataTrimSeparators = new char[] { '\"' };

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, object userData)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string dataRowClassName = DataRowClassPrefixName + splitedNames[0];
            Type dataRowType = Type.GetType(dataRowClassName);
            if (dataRowType == null)
            {
                Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
                return;
            }

            string name = splitedNames.Length > 1 ? splitedNames[1] : null;
            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
            dataTable.ReadData(dataTableAssetName, Constant.AssetPriority.DataTableAsset, userData);
        }


        #region Define
        /// <summary>
        /// 将DRDataCostMulRes转为ChaResource
        /// </summary>
        public static ChaResource ToChaResource(this DRDataCostMulRes data)
        {
            return new ChaResource(
                data.HP,
                data.MP,
                0
            );
        }
        /// <summary>
        /// 将DRDataCostMulRes转为ChaResource
        /// </summary>
        public static ChaResource ToChaResource(this DataTableComponent dataTableComponent, int id)
        {
            return dataTableComponent.GetDataTable<DRDataCostMulRes>().GetDataRow(id).ToChaResource();
        }
        #endregion


        #region Character
        /// <summary>
        /// 将DRDataCharacterProperty转为ChaProperty
        /// </summary>
        public static ChaProperty ToChaProperty(this DRDataCharacterProperty data)
        {
            return new ChaProperty(
                data.MoveSpeedFactor,
                data.HP,
                data.MP,
                data.Attack,
                data.ActionSpeedFactor
            );
        }
        /// <summary>
        /// 将DRDataCharacterProperty转为ChaProperty
        /// </summary>
        public static ChaProperty ToChaProperty(this DataTableComponent dataTableComponent, int id)
        {
            return dataTableComponent.GetDataTable<DRDataCharacterProperty>().GetDataRow(id).ToChaProperty();
        }
        #endregion


        #region Buff
        /// <summary>
        /// 将DRDefineBuffModel转为BuffModel
        /// </summary>
        public static BuffModel ToBuffModel(this DRDefineBuffTemplate config)
        {
            ChaControlState stateMod = new ChaControlState(
                config.CanMove,
                config.CanRotate,
                config.CanUseSkill
            );

            ChaProperty[] propMod = new ChaProperty[config.CharacterPropIds.Length];
            for (int i = 0; i < config.CharacterPropIds.Length; i++)
            {
                propMod[i] = GameEntry.DataTable.ToChaProperty(config.CharacterPropIds[i]);
            }

            return new BuffModel(
                config.Id.ToString(),
                config.Tags,
                config.Priority,
                config.MaxStack,
                config.TickTime,
                string.IsNullOrEmpty(config.OnOccurrence) ? null : BuffDelegateFactory.OnOccurFuncDic[config.OnOccurrence],
                config.OnOccurrenceParams,
                string.IsNullOrEmpty(config.OnRemoved) ? null : BuffDelegateFactory.OnRemovedFuncDic[config.OnRemoved],
                config.OnRemovedParams,
                string.IsNullOrEmpty(config.OnTick) ? null : BuffDelegateFactory.OnTickFuncDic[config.OnTick],
                config.OnTickParams,
                string.IsNullOrEmpty(config.OnCast) ? null : BuffDelegateFactory.OnCastFuncDic[config.OnCast],
                config.OnCastParams,
                string.IsNullOrEmpty(config.OnHit) ? null : BuffDelegateFactory.OnHitFuncDic[config.OnHit],
                config.OnHitParams,
                string.IsNullOrEmpty(config.OnBeHurt) ? null : BuffDelegateFactory.BeHurtFuncDic[config.OnBeHurt],
                config.OnBeHurtParams,
                string.IsNullOrEmpty(config.OnKill) ? null : BuffDelegateFactory.OnKillFuncDic[config.OnKill],
                config.OnKillParams,
                string.IsNullOrEmpty(config.OnBeKilled) ? null : BuffDelegateFactory.BeKilledFuncDic[config.OnBeKilled],
                config.OnBeKilledParams,
                stateMod,
                propMod
            );
        }
        /// <summary>
        /// 将DRDefineBuffModel转为BuffModel
        /// </summary>
        public static BuffModel ToBuffModel(this DataTableComponent dataTableComponent, int id)
        {
            return dataTableComponent.GetDataTable<DRDefineBuffTemplate>().GetDataRow(id).ToBuffModel();
        }

        /// <summary>
        /// 将DRDefineBuffModel转为AddBuffInfo
        /// </summary>
        public static AddBuffInfo ToAddBuffInfo(this DRDefineBuffBuilder config)
        {
            return new AddBuffInfo(
                GameEntry.DataTable.ToBuffModel(config.TemplateId),
                null,
                null,
                config.AddStack,
                config.Duration,
                config.DurationSetTo,
                config.Permanent,
                config.BuffParam
            );
        }
        /// <summary>
        /// 将DRDefineBuffModel转为AddBuffInfo
        /// </summary>
        public static AddBuffInfo ToAddBuffInfo(this DataTableComponent dataTableComponent, int id)
        {
            return dataTableComponent.GetDataTable<DRDefineBuffBuilder>().GetDataRow(id).ToAddBuffInfo();
        }
        #endregion


        #region 关卡
        /// <summary>
        /// 获取场景循环地形配置列表
        /// </summary>
        public static List<DRDefineTerrain> GetSceneTerrainStyle(this DataTableComponent dataTableComponent, int styleId)
        {
            int checkIndex = 1;
            List<DRDefineTerrain> styleConfigList = new List<DRDefineTerrain>();
            while (true)
            {
                int configId = styleId * 1000 + checkIndex;
                var style = dataTableComponent.GetDataTable<DRDefineTerrain>().GetDataRow(configId);
                if (style == null)
                {
                    break;
                }

                styleConfigList.Add(style);
                checkIndex++;
            }

            return styleConfigList;
        }
        #endregion


        #region 时间轴TimeLine

        /// <summary>
        /// 将DRDefineTimeLineInfo转为LogicTimeLineModel
        /// </summary>
        public static LogicTimeLineModel ToLogicTimeLineModel(this DataTableComponent dataTableComponent, int dtId)
        {
            DRDefineTimeLineInfo dtData = dataTableComponent.GetDataTable<DRDefineTimeLineInfo>().GetDataRow(dtId);
            LogicTimeLineNode[] nodes = new LogicTimeLineNode[dtData.NodeIds.Length];
            for (int i = 0; i < dtData.NodeIds.Length; i++)
            {
                nodes[i] = dataTableComponent.ToLogicTimeLineNode(dtData.NodeIds[i]);
            }
            return new LogicTimeLineModel(dtData.Id.ToString(), nodes, dtData.Duration, LogicTimeLineGoTo.Null);
        }

        /// <summary>
        /// 将DRDefineTimeLineNode转为LogicTimeLineNode
        /// </summary>
        public static LogicTimeLineNode ToLogicTimeLineNode(this DataTableComponent dataTableComponent, int dtId)
        {
            DRDefineTimeLineNode dtData = dataTableComponent.GetDataTable<DRDefineTimeLineNode>().GetDataRow(dtId);
            return new LogicTimeLineNode(dtData.TimeElapsed, dtData.DoEventName, dtData.EventArgs);
        }
        #endregion



    }
}
