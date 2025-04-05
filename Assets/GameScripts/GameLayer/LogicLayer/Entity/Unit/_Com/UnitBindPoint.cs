using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    ///<summary>
    ///在一个gameObject下添加这个，让这个gameObject成为一个“绑点”，这样就可以在这东西里面管理一些挂载的gameObject
    ///最常见的用途是角色身上某个点播放视觉特效什么的。
    ///</summary>
    public class UnitBindPoint : MonoBehaviour
    {
        ///<summary>
        ///绑点的名称
        ///</summary>
        public string key;

        ///<summary>
        ///偏移坐标
        ///</summary>
        public Vector3 offset;

        ///<summary>
        ///已经挂着的gameobject信息
        ///key就是一个索引，便于找到
        ///</summary>
        private Dictionary<string, BindGameObjectInfo> bindGameObject = new Dictionary<string, BindGameObjectInfo>();
    }

    ///<summary>
    ///被挂载的gameobject的记录
    ///</summary>
    public class BindGameObjectInfo
    {
        ///<summary>
        ///gameObject的地址
        ///</summary>
        public GameObject gameObject;

        ///<summary>
        ///还有多少时间之后被销毁，单位：秒
        ///</summary>
        public float duration;

        ///<summary>
        ///有些是不能被销毁的，得外部控制销毁，所以永久存在
        ///</summary>
        public bool forever;

        ///<summary>
        ///<param name="gameObject">要挂载的gameObject</param>
        ///<param name="duration">挂的时间，时间到了销毁，[Magic]如果<=0则代表永久</param>
        ///</summary>
        public BindGameObjectInfo(GameObject gameObject, float duration)
        {
            this.gameObject = gameObject;
            this.duration = Mathf.Abs(duration);
            this.forever = duration <= 0;
        }
    }
}