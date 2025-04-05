using UnityEngine;

namespace GameDevScript
{
    public static partial class Constant
    {
        public static partial class GameLogic
        {
            /// <summary>
            /// 开发分辨率
            /// </summary>
            public readonly static Vector2 DevelopmentResolution = new Vector2(1080, 2176);
            /// <summary>
            /// 场景中Sprite的像素点转换比例。100像素点等于标准单位长度
            /// </summary>
            public const int SceneSpritePixelsPerUnit = 100;
            /// <summary>
            /// 战斗场景相机休憩时的缩放
            /// </summary>
            public const float FightCameraFocusRestSize = 8.6f;
            /// <summary>
            /// 战斗场景相机休憩时的Y坐标
            /// </summary>
            public const float FightCameraFocusRestWorldY = -0.3f;
            /// <summary>
            /// 战斗场景相机战斗时的缩放
            /// </summary>
            public const float FightCameraFocusNormalSize = 11.5f;
            /// <summary>
            /// 战斗场景相机战斗时的Y坐标
            /// </summary>
            public const float FightCameraFocusWarWorldY = 3.6f;

            #region 战车座位网格大小
            /// <summary>
            /// 战车（士兵）单位网格的尺寸（单位：米）
            /// 网格大小+网格间距*0.5f
            /// </summary>
            public readonly static Vector2 VehicleUnitGridSize = new Vector2(1.04f, 1.04f);
            // /// <summary>
            // /// 战车（士兵）单位网格间距（单位：米）
            // /// </summary>
            // public readonly static Vector2 VehicleUnitGridSpace = new Vector2(0.08f, 0.08f);
            /// <summary>
            /// 战车最大空间容纳网格数量
            /// </summary>
            public readonly static Vector2Int VehicleMaxGridShape = new Vector2Int(7, 5);
            #endregion

            /// <summary>
            /// 士兵的最高等级
            /// </summary>
            public const int SoldierMaxMergeLv = 5;




            #region 技能
            /// <summary>
            /// 释放技能的位置
            /// </summary>
            public const string Skill_FirePos = "Skill_FirePos";
            /// <summary>
            /// 释放技能的方向
            /// </summary>
            public const string Skill_FireDire = "Skill_FireDire";
            /// <summary>
            /// 技能预期目标位置
            /// </summary>
            public const string Skill_TargetPos = "Skill_TargetPos";
            /// <summary>
            /// 技能预期目标实体编号
            /// </summary>
            public const string Skill_TargetSerialId = "Skill_TargetSerialId";

            /// <summary>
            /// 技能最小搜索距离
            /// </summary>
            public const float Skill_Min_Search_Distance = 4f;
            #endregion


            #region bindPoint
            /// <summary>
            /// 技能实体发射点
            /// </summary>
            public const string BindPoint_FirePoint = "FirePort";
            /// <summary>
            /// 载具驾驶员点
            /// </summary>
            public const string BindPoint_DriverPoint = "DriverPoint";
            /// <summary>
            /// 中心点
            /// </summary>
            public const string BindPoint_CenterPoint = "CenterPoint";
            #endregion


            #region 士兵
            /// <summary>
            /// 士兵最大等级
            /// </summary>
            public const int Soldier_Max_Lv = 10;
            #endregion

        }
    }
}
