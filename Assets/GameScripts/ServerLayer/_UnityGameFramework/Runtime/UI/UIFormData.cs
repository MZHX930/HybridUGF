using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 所有UIForm数据的基类
    /// </summary>
    public abstract class UIFormData : IReference
    {
        /// <summary>
        /// 定义在配置表里的id
        /// </summary>
        public abstract int DtId { get; }

        public abstract void Clear();
    }
}
