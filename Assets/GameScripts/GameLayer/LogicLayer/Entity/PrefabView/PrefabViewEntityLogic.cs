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
    public class PrefabViewEntityLogic : EntityLogic
    {
        #region 组件
        #endregion

        #region private data
        #endregion

        #region public data
        public PrefabViewEntityData EntityData { get; private set; }
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            EntityData = userData as PrefabViewEntityData;

            if (EntityData.AttachedParentTransform != null)
            {
                //附着到父级
                Entity.transform.position = EntityData.AttachedParentTransform.position;
                GameEntry.Entity.AttachEntity(this.Entity, EntityData.AttachedParentEntity, EntityData.AttachedParentTransform);
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

            if (EntityData != null)
                ReferencePool.Release(EntityData);
            EntityData = null;
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            Entity.transform.localPosition = EntityData.AttachedLocalPos;
            Entity.transform.localEulerAngles = EntityData.AttachedLocalEulerAngles;
        }
    }
}
