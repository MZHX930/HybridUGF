using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    public partial class UIRedDotComponent
    {
        /// <summary>
        /// 红点父子关系
        /// (子节点key，父节点key)
        /// </summary>
        private static Dictionary<EnumRedDotKey, EnumRedDotKey> m_RedDotRelationMap = new Dictionary<EnumRedDotKey, EnumRedDotKey>()
        {
            {EnumRedDotKey.B_1,EnumRedDotKey.B},
            {EnumRedDotKey.B_2, EnumRedDotKey.B},
            {EnumRedDotKey.B_2_1, EnumRedDotKey.B_2},
            {EnumRedDotKey.A,EnumRedDotKey.Null},
            {EnumRedDotKey.B,EnumRedDotKey.Null},
        };
    }

    /// <summary>
    /// 红点Key
    /// </summary>
    public enum EnumRedDotKey
    {
        Null,

        A,
        B,

        A_1,
        A_1_1,
        A_1_2,
        B_1,
        B_2,
        B_2_1,
    }
}