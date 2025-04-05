using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///如果一个gameobject下1个或多个子gameobject装上了UnitBindPoint，但是又希望只管理这个gameobject，那就添加这个
    ///</summary>
    public class UnitBindManager : MonoBehaviour
    {
        private Dictionary<string, UnitBindPoint> mBindPointDic = new Dictionary<string, UnitBindPoint>();
        void Start()
        {
            UpdatePoints();
        }

        public void UpdatePoints()
        {
            mBindPointDic.Clear();
            var bindPoints = this.gameObject.GetComponentsInChildren<UnitBindPoint>();
            for (int i = 0; i < bindPoints.Length; i++)
            {
                if (bindPoints[i].key == null)
                {
                    Log.Error("UnitBindManager: UpdatePoints: 没有key: {0}", bindPoints[i].gameObject.name);
                    continue;
                }
                if (mBindPointDic.ContainsKey(bindPoints[i].key))
                {
                    Log.Error("UnitBindManager: UpdatePoints: 重复的key: {0}", bindPoints[i].key);
                    continue;
                }
                mBindPointDic.Add(bindPoints[i].key, bindPoints[i]);
            }
        }

        ///<summary>
        ///获得子GameObject下的某个UnitBindPoint
        ///<param name="key">这个UnitBindPoint的key</param>
        ///<return>如果找到就return，否则为null</return>
        ///</summary>
        public UnitBindPoint GetBindPointByKey(string key)
        {
            if (mBindPointDic.TryGetValue(key, out var bindPoint))
                return bindPoint;
            Log.Warning("UnitBindManager: GetBindPointByKey: 没有找到key: {0}", key);
            return null;
        }
    }
}