using UnityEngine;
using System;
using System.Text;

namespace GameDevScript
{
    public static class UITools
    {
        /// <summary>
        /// 获取UIRoot下的查询路径
        /// </summary>
        /// <param name="trs">查找对象</param>
        /// <returns>以UIRoot为根节点的路径/</returns>
        public static string GetUIRootPath(Transform trs)
        {
            if (trs.name.Equals("UIRoot"))
                return "";

            Transform trsFind = trs.transform;
            StringBuilder path = new StringBuilder(trsFind.name);
            trsFind = trsFind.parent;

            while (trsFind != null)
            {
                path.Insert(0, trsFind.name + "/");

                trsFind = trsFind.parent;
                if (trsFind.name.Equals("UIRoot"))
                    break;
            }
            return path.ToString();
        }
    }
}
