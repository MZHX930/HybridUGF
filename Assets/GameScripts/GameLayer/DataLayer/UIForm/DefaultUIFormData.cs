using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 默认的UIFormData
    /// </summary>
    public sealed class DefaultUIFormData : UIFormData
    {
        public static DefaultUIFormData Create(int dtId)
        {
            var data = ReferencePool.Acquire<DefaultUIFormData>();
            data.m_DTId = dtId;
            return data;
        }

        public static DefaultUIFormData Create(UIFormId dtId)
        {
            return Create((int)dtId);
        }

        private int m_DTId;

        public override int DtId => m_DTId;

        public override void Clear()
        {
            m_DTId = 0;
        }
    }
}
