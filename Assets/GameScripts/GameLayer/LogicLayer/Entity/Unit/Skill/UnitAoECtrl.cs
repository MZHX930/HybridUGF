using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///AoE捕获器控制
    ///</summary>
    public class UnitAoECtrl : MonoBehaviour, IUnitCtrl<UnitAoECtrlData>, IShellShowHandler
    {
        public UnitAoECtrlData CtrlData { get; private set; }

        #region 组件
        private UnitMove m_UnitMove;
        private UnitRotate m_UnitRotate;
        private GameObject m_ViewContainer;
        private UnitCollider m_UnitCollider;
        public ShellEntityLogic ShellEntityLogic { get; private set; }
        #endregion

        #region 变量
        ///<summary>
        ///是否被视作刚创建
        ///</summary>
        private bool m_JustCreated = true;

        ///<summary>
        ///现在aoe范围内的所有角色的gameobject
        ///</summary>
        private List<UnitCharacterCtrl> m_CharacterInRange = new List<UnitCharacterCtrl>();

        /// <summary>
        /// 矢量速度
        /// </summary>
        private Vector3 m_Velocity = new Vector3();

        /// <summary>
        /// 是否加载视图中
        /// </summary>
        private bool m_WaitLoadView = true;
        #endregion

        ///<summary>
        ///移动信息
        ///</summary>
        public Vector3 Velocity
        {
            get { return this.m_Velocity; }
        }

        public IUnitCtrlData BaseCtrlData => CtrlData;

        private void Awake()
        {
            m_UnitMove = this.gameObject.GetComponent<UnitMove>();
            m_UnitRotate = this.gameObject.GetComponent<UnitRotate>();
            m_ViewContainer = this.gameObject.GetComponentInChildren<ViewContainer>().gameObject;
            m_UnitCollider = gameObject.GetComponent<UnitCollider>();
        }

        public void OnShowShell(ShellEntityLogic shellEntityLogic)
        {
            ShellEntityLogic = GetComponent<ShellEntityLogic>();
            CtrlData = ShellEntityLogic.ShellData.UnitCtrlData as UnitAoECtrlData;

            this.transform.position = CtrlData.FirePos;
            this.transform.eulerAngles.Set(0, CtrlData.Degree, 0);

            m_UnitMove.BodyRadius = 1f;
            m_UnitMove.SmoothMove = false;

            m_JustCreated = true;
        }

        public void OnShowView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_UnitCollider.ActiveCollision(CtrlData.GameSide == GameSideEnum.Player ? CollisionLayer.PlayerAoe : CollisionLayer.EnemyAoe);
        }

        public void OnHideShell(ShellEntityLogic shellEntityLogic)
        {
        }

        public void OnHideView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_UnitCollider.RemoveCollision();
        }

        private void Update()
        {
            float elapseSeconds = Time.deltaTime;
            float realElapseSeconds = Time.unscaledDeltaTime;

            if (m_WaitLoadView)
                return;

            //首先是aoe的移动
            if (CtrlData.Duration > 0 && CtrlData.MoveTween != null)
            {
                AoeMoveInfo aoeMoveInfo = CtrlData.MoveTween(this, CtrlData.MoveTweenRunnedTime);
                CtrlData.MoveTweenRunnedTime += elapseSeconds;
                SetMoveAndRotate(aoeMoveInfo);
            }

            if (GameEntry.CollisionCal.CheckAdverseCollisions(m_UnitCollider, out List<UnitCollider> adverseColliderList))
            {
                var capturedChaList = new List<UnitCharacterCtrl>();
                foreach (var adverseCollider in adverseColliderList)
                {
                    if (adverseCollider.ShellEntityLogic != null)
                    {
                        capturedChaList.Add(adverseCollider.ShellEntityLogic.UnitCtrl as IUnitCtrl<UnitCharacterCtrlData> as UnitCharacterCtrl);
                    }
                }
                if (m_JustCreated == true)
                {
                    //刚创建的，走onCreate
                    m_JustCreated = false;
                    //捕获的角色
                    m_CharacterInRange = capturedChaList;
                    //执行OnCreate
                    if (CtrlData.Model.OnCreate != null)
                    {
                        CtrlData.Model.OnCreate(this);
                    }
                }
                else
                {
                    //已经创建完成的
                    //先抓角色离开事件
                    List<UnitCharacterCtrl> leaveCha = new List<UnitCharacterCtrl>();
                    List<UnitCharacterCtrl> toRemove = new List<UnitCharacterCtrl>();
                    for (int m = 0; m < m_CharacterInRange.Count; m++)
                    {
                        if (m_CharacterInRange[m] != null)
                        {
                            if (capturedChaList.Contains(m_CharacterInRange[m]) == false)
                            {
                                leaveCha.Add(m_CharacterInRange[m]);
                                toRemove.Add(m_CharacterInRange[m]);
                            }
                        }
                        else
                        {
                            toRemove.Add(m_CharacterInRange[m]);
                        }
                    }
                    for (int m = 0; m < toRemove.Count; m++)
                    {
                        m_CharacterInRange.Remove(toRemove[m]);
                    }
                    if (CtrlData.Model.OnChaLeave != null)
                    {
                        CtrlData.Model.OnChaLeave(this, leaveCha);
                    }

                    //再看进入的角色
                    List<UnitCharacterCtrl> enterCha = new List<UnitCharacterCtrl>();
                    for (int m = 0; m < capturedChaList.Count; m++)
                    {
                        if (
                            capturedChaList[m] &&
                            m_CharacterInRange.IndexOf(capturedChaList[m]) < 0
                        )
                        {
                            enterCha.Add(capturedChaList[m]);
                        }
                    }
                    if (CtrlData.Model.OnChaEnter != null)
                    {
                        CtrlData.Model.OnChaEnter(this, enterCha);
                    }
                    for (int m = 0; m < enterCha.Count; m++)
                    {
                        if (enterCha[m] != null && enterCha[m].IsDead == false)
                        {
                            m_CharacterInRange.Add(enterCha[m]);
                        }
                    }
                    toRemove = null;
                }
                //然后是aoe的duration
                CtrlData.Duration -= elapseSeconds;
                CtrlData.TimeElapsed += elapseSeconds;
                if (CtrlData.Duration <= 0 || HitObstacle() == true)
                {
                    if (CtrlData.Model.OnRemoved != null)
                    {
                        CtrlData.Model.OnRemoved(this);
                    }
                    GameEntry.Entity.HideEntity(ShellEntityLogic.Entity);
                    return;
                }
                else
                {
                    //最后是onTick，remove
                    if (
                        CtrlData.Model.TickTime > 0 && CtrlData.Model.OnTick != null &&
                        Mathf.RoundToInt(CtrlData.Duration * 1000) % Mathf.RoundToInt(CtrlData.Model.TickTime * 1000) == 0
                    )
                    {
                        CtrlData.Model.OnTick(this);
                    }
                }
            }
        }

        ///<summary>
        ///设置移动和旋转的信息，用于执行
        ///</summary>
        public void SetMoveAndRotate(AoeMoveInfo aoeMoveInfo)
        {
            if (aoeMoveInfo != null)
            {
                if (m_UnitMove)
                {
                    m_UnitMove.moveType = aoeMoveInfo.MoveType;
                    m_UnitMove.BodyRadius = 1f;
                    m_Velocity = aoeMoveInfo.Velocity / Time.fixedDeltaTime;
                    m_UnitMove.MoveBy(m_Velocity);
                }
                if (m_UnitRotate)
                {
                    m_UnitRotate.RotateTo(aoeMoveInfo.RotateToDegree);
                }
            }
        }

        public bool HitObstacle()
        {
            return m_UnitMove == null ? false : m_UnitMove.hitObstacle;
        }

        ///<summary>
        ///改变aoe视觉的尺寸
        ///</summary>
        public void SetViewScale(float scaleX = 1, float scaleY = 1, float scaleZ = 1)
        {
            m_ViewContainer.transform.localScale.Set(scaleX, scaleY, scaleZ);
        }
    }
}