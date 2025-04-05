//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.444
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// DefineBulletVFX。
    /// </summary>
    public class DRDefineBulletVFX : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取预制件名。
        /// </summary>
        public string Prefab
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取移动速度因子。
        /// </summary>
        public int MoveSpeedFactor
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取生命周期 秒。
        /// </summary>
        public float LifeTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取碰撞半径。
        /// </summary>
        public float Radius
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取创建后延迟多久可以碰撞。
        /// </summary>
        public float DelayCollision
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取特殊的移动轨迹。
        /// </summary>
        public BulletTween MoveTween
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹可以碰触的次数，每次碰到合理目标-1，到0的时候子弹就结束了。。
        /// </summary>
        public int HitTimes
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹碰触同一个目标的延迟，单位：秒，最小值是Time.fixedDeltaTime（每帧发生一次）。
        /// </summary>
        public float SameTargetDelay
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹被创建的事件名。
        /// </summary>
        public BulletOnCreate OnCreate
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹被创建的事件参数。
        /// </summary>
        public string[] OnCreateParam
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹击中时的事件名。
        /// </summary>
        public BulletOnHit OnHit
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹击中时的事件参数。
        /// </summary>
        public string[] OnHitParam
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹生命周期结束时候发生的事件名。
        /// </summary>
        public BulletOnRemoved OnRemoved
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取子弹生命周期结束时候发生的事件参数。
        /// </summary>
        public string[] OnRemovedParam
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableRuntimeParseTools.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableRuntimeParseTools.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            Prefab = columnStrings[index++];
            MoveSpeedFactor = int.Parse(columnStrings[index++]);
            LifeTime = float.Parse(columnStrings[index++]);
            Radius = float.Parse(columnStrings[index++]);
            DelayCollision = float.Parse(columnStrings[index++]);
            MoveTween = DataTableRuntimeParseTools.ParseBulletTween(columnStrings[index++]);
            HitTimes = int.Parse(columnStrings[index++]);
            SameTargetDelay = float.Parse(columnStrings[index++]);
            OnCreate = DataTableRuntimeParseTools.ParseBulletOnCreate(columnStrings[index++]);
            OnCreateParam = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnHit = DataTableRuntimeParseTools.ParseBulletOnHit(columnStrings[index++]);
            OnHitParam = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnRemoved = DataTableRuntimeParseTools.ParseBulletOnRemoved(columnStrings[index++]);
            OnRemovedParam = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
