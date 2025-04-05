using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public static partial class BulletSkillDelegateFactory
    {

        ///<summary>
        ///Tween
        ///逐渐加速的子弹，bulletObj参数：
        ///["turningPoint"]float：在第几秒达到预设的速度（100%），并且逐渐减缓增速。
        ///</summary>
        private static Vector3 SlowlyFaster(float t, UnitBulletCtrl bullet, UnitCharacterCtrl target)
        {
            if (!bullet)
                return Vector3.forward;
            float tp = 5.0f; //默认5秒后达到100%速度
            if (bullet.CtrlData.Param.ContainsKey("turningPoint"))
                tp = (float)bullet.CtrlData.Param["turningPoint"];
            if (tp < 1.0f)
                tp = 1.0f;
            return Vector3.forward * (2 * t / (t + tp));
        }
    }
}
