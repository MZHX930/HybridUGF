using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    ///<summary>
    ///单位旋转控件，如果一个单位要通过游戏逻辑来进行旋转，就应该用它，不论是角色还是aoe还是bullet什么的
    ///</summary>
    public class UnitRotate : MonoBehaviour
    {
        ///<summary>
        ///单位当前是否可以旋转角度
        ///</summary>
        private bool canRotate = true;

        ///<summary>
        ///旋转的速度，1秒只能转这么多度（角度）
        ///每帧转动的角度上限是这个数字 * Time.fixedDeltaTime得来的。
        ///</summary>
        ///[Tooltip("旋转的速度，1秒只能转这么多度（角度），每帧转动的角度上限是这个数字*Time.fixedDeltaTime得来的。")]
        public float rotateSpeed;

        private float m_TargetDegree = 0;
        //目标转到多少度，将值处理到-180~180之间
        private float TargetDegree
        {
            get { return m_TargetDegree; }
            set
            {
                m_TargetDegree = value;
                while (m_TargetDegree > 180.0f)
                    m_TargetDegree -= 360.0f;
                while (m_TargetDegree < -180.0f)
                    m_TargetDegree += 360.0f;
            }
        }

        void FixedUpdate()
        {
            if (this.canRotate == false || DoneRotate() == true)
                return;

            float curDeg = transform.rotation.eulerAngles.z;
            if (curDeg > 180.00f)
                curDeg -= 360.00f;
            //计算正向旋转角度
            float forwardDis = TargetDegree - curDeg;
            if (forwardDis < 0)
                forwardDis += 360;
            //计算逆向旋转角度
            float reverseDis = forwardDis - 360;
            //是否逆向旋转更短？
            bool isReverseQuick = Mathf.Abs(forwardDis) >= Mathf.Abs(reverseDis);
            float rotSpeed = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, Mathf.Abs(forwardDis), Mathf.Abs(reverseDis));  //选择其中最短的一个，作为一个移动角度

            if (isReverseQuick)
                rotSpeed *= -1;
            transform.Rotate(new Vector3(0, 0, rotSpeed));
        }

        //判断是否完成了旋转
        private bool DoneRotate()
        {
            float rotSpeed = this.rotateSpeed * Time.fixedDeltaTime;
            return Mathf.Abs(transform.rotation.eulerAngles.z - TargetDegree) < Mathf.Min(0.01f, rotSpeed); //允许一定的误差也当是达成了。
        }

        ///<summary>
        ///旋转到指定角度
        ///<param name="degree">需要旋转到的角度</param>
        ///</summary>
        public void RotateTo(float degree)
        {
            TargetDegree = degree;
        }

        ///<summary>
        ///指定两个点，旋转到对应角度
        ///<param name="x">目标点x-起点x</param>
        ///<param name="y">目标点y-起点y</param>
        ///</summary>
        public void RotateTo(float x, float y)
        {
            TargetDegree = Mathf.Atan2(x, y) * 180.00f / Mathf.PI;
        }

        ///<summary>
        ///旋转指定角度
        ///<param name="degree">需要旋转到的角度</param>
        ///</summary>
        public void RotateBy(float degree)
        {
            TargetDegree = transform.rotation.eulerAngles.z + degree;
        }

        ///<summary>
        ///禁止单位可以旋转的能力，这会终止当前正在进行的旋转
        ///终止当前的旋转看起来是一个side-effect，但是依照游戏规则设计来说，他只是“配套功能”所以严格的说并不是side-effect
        ///</summary>
        public void DisableRotate()
        {
            canRotate = false;
            TargetDegree = transform.rotation.eulerAngles.z;
        }

        ///<summary>
        ///开启单位可以旋转的能力
        ///</summary>
        public void EnableRotate()
        {
            canRotate = true;
        }
    }
}