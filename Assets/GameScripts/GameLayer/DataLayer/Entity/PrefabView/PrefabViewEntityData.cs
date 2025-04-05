using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 预制件模型
    /// </summary>
    public class PrefabViewEntityData : GameEntityData
    {
        public static PrefabViewEntityData Create(Entity parentEntity, Transform attachedParentTransform, Vector3 attachedLocalPos)
        {
            PrefabViewEntityData characterViewEntityData = ReferencePool.Acquire<PrefabViewEntityData>();
            characterViewEntityData.AttachedParentEntity = parentEntity;
            characterViewEntityData.AttachedParentTransform = attachedParentTransform;
            characterViewEntityData.AttachedLocalPos = attachedLocalPos;

            return characterViewEntityData;
        }

        /// <summary>
        /// 附着对象
        /// </summary>
        public Entity AttachedParentEntity = null;
        /// <summary>
        /// 附着路径
        /// </summary>
        public Transform AttachedParentTransform = null;
        /// <summary>
        /// 附着后调整的坐标偏差值
        /// </summary>
        public Vector3 AttachedLocalPos = Vector3.zero;
        /// <summary>
        /// 附着后调整的旋转角度
        /// </summary>
        public Vector3 AttachedLocalEulerAngles = Vector3.zero;

        public override void Clear()
        {
            AttachedParentEntity = null;
            AttachedParentTransform = null;
            AttachedLocalPos = Vector3.zero;
            AttachedLocalEulerAngles = Vector3.zero;
        }
    }
}
