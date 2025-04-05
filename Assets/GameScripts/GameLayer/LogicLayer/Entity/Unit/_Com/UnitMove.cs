using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    ///<summary>
    ///单位移动控件，所有需要移动的单位都应该添加这个来控制它的移动，不论是角色，aoe还是子弹，但是地形不能用这个，因为地形是terrain不是unit
    ///这里负责的是每一帧往一个方向移动，至于往什么方向移动，这应该是其他控件的事情，比如角色是由操作来决定的，子弹则是轨迹决定的
    ///在这游戏里，角色只有x,z方向移动，依赖于地形的y移动如果有，也不归这个逻辑来管理，而是由视觉元素（比如小土坡）自身决定的
    ///</summary>
    public class UnitMove : MonoBehaviour
    {
        //是否有权移动
        private bool m_CanMove = true;

        /// <summary>
        /// 单位的移动类型，根据游戏设计不同，这个值也可以不同
        /// </summary>
        public MoveType moveType = MoveType.ground;

        /// <summary>
        /// 单位的移动体型碰撞圆形的半径，单位：米
        /// </summary>
        public float BodyRadius = 0.25f;

        /// <summary>
        /// 当单位移动被地图阻挡的时候，是选择一个更好的落脚点（true）还是直接停止移动（false），如果直接停止移动，那么停下的时候访问hitObstacle的时候就是true，否则hitObstacle永远是false
        /// </summary>
        public bool SmoothMove = true;

        /// <summary>
        /// 是否会忽略关卡外围，即飞行（只有飞行允许）到地图外的地方全部视作可过
        /// </summary>
        public bool ignoreBorder = true;

        public bool hitObstacle
        {
            get
            {
                return m_HitObstacle;
            }
        }
        private bool m_HitObstacle = false;

        //要移动的方向的力，单位：米/秒。
        private Vector3 m_Velocity = Vector3.zero;

        void FixedUpdate()
        {
            if (m_CanMove == false || m_Velocity == Vector3.zero)
                return;

            Vector3 targetPos = new Vector3(
                m_Velocity.x * Time.fixedDeltaTime + transform.position.x,
                m_Velocity.y * Time.fixedDeltaTime + transform.position.y,
                0
            );
            transform.position = targetPos;
            m_Velocity = Vector3.zero;
        }

        private void StopMoving()
        {
            m_Velocity = Vector3.zero;
        }

        ///<summary>
        ///当前的移动方向
        ///</summary>
        public Vector3 GetMoveDirection()
        {
            return m_Velocity;
        }

        ///<summary>
        ///移动向某个方向，距离也决定了速度，距离单位是米，1秒内移动的量
        ///<param name="moveForce">移动方向和力，单位：米/秒</param>
        ///</summary>
        public void MoveBy(Vector3 moveForce)
        {
            if (m_CanMove == false)
                return;

            m_Velocity = moveForce;
        }

        ///<summary>
        ///禁止角色可以移动能力，会停止当前的移动
        ///终止当前移动看起来是一个side-effect，但是依照游戏规则设计来说，他只是"配套功能"所以严格的说并不是side-effect
        ///</summary>
        public void DisableMove()
        {
            StopMoving();
            m_CanMove = false;
        }

        ///<summary>
        ///开启角色可以移动的能力
        ///</summary>
        public void EnableRotate()
        {
            m_CanMove = true;
        }
    }
}