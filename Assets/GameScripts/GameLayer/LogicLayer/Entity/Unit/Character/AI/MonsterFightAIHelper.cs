using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 怪物战斗AI辅助类
    /// </summary>
    public class MonsterFightAIHelper : IFighAIHelper
    {
        private UnitCharacterCtrl m_ChaLogic;

        private UnitBindPoint m_OwnFirePoint;
        private UnitBindPoint m_OwnCenterPoint;
        private UnitBindPoint m_AttackTargetPoint;

        private float m_SqrSkillSearchDistance = 0;
        private float m_MinStandY = 0;
        private Vector3 m_NormalizedMoveDirection = Vector3.down;

        public void Clear()
        {
            m_ChaLogic = null;
            m_SqrSkillSearchDistance = 0;
            m_OwnFirePoint = null;
            m_OwnCenterPoint = null;
            m_AttackTargetPoint = null;
        }

        public void Init(UnitCharacterCtrl chaLogic)
        {
            m_ChaLogic = chaLogic;

            m_OwnFirePoint = chaLogic.GetBindPoint(Constant.GameLogic.BindPoint_FirePoint);
            m_OwnCenterPoint = chaLogic.GetBindPoint(Constant.GameLogic.BindPoint_CenterPoint);
            m_AttackTargetPoint = SceneEntityHelper.CurActivedVehicle.GetBindPoint(Constant.GameLogic.BindPoint_DriverPoint);

            m_SqrSkillSearchDistance = 0;
            m_NormalizedMoveDirection = Vector3.down;
            if (m_ChaLogic.CtrlData.ChaSkillList.Count > 0)
            {
                float minDistance = int.MaxValue;
                foreach (var skill in m_ChaLogic.CtrlData.ChaSkillList)
                {
                    minDistance = Mathf.Min(minDistance, skill.Model.SearchRadius);
                }
                m_SqrSkillSearchDistance = minDistance * minDistance;
            }
            m_SqrSkillSearchDistance = Mathf.Max(Constant.GameLogic.Skill_Min_Search_Distance * Constant.GameLogic.Skill_Min_Search_Distance, m_SqrSkillSearchDistance);

            m_MinStandY = m_AttackTargetPoint.transform.position.y + Constant.GameLogic.Skill_Min_Search_Distance * 0.5f;

            //计算位移的x轴偏差
            Vector2 xRange = new Vector2(m_AttackTargetPoint.transform.position.x - Constant.GameLogic.Skill_Min_Search_Distance, m_AttackTargetPoint.transform.position.x + Constant.GameLogic.Skill_Min_Search_Distance) * 0.8f;
            if (m_OwnCenterPoint.transform.position.x < xRange.x)
            {
                //向右移动
                m_NormalizedMoveDirection += new Vector3(0.2f, 0, 0);
            }
            else if (m_OwnCenterPoint.transform.position.x > xRange.y)
            {
                //向左移动
                m_NormalizedMoveDirection += new Vector3(-0.2f, 0, 0);
            }
            // m_NormalizedMoveDirection = m_NormalizedMoveDirection.normalized;
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Vector3 distance = (m_AttackTargetPoint.transform.position + m_AttackTargetPoint.offset) - (m_OwnCenterPoint.transform.position + m_OwnCenterPoint.offset);
            if (distance.sqrMagnitude <= m_SqrSkillSearchDistance || m_ChaLogic.transform.position.y <= m_MinStandY)
            {
                AutoCastSkill(distance);
            }
            else
            {
                //向玩家移动
                float timePassed = Time.fixedDeltaTime;
                m_ChaLogic.OrderMove(m_NormalizedMoveDirection * m_ChaLogic.MoveSpeed);
            }
        }

        private void AutoCastSkill(Vector3 distance)
        {
            //如果没有敌人，则跳过释放特效过程。同一帧内只释放一个技能
            foreach (var skillShellData in m_ChaLogic.CtrlData.ChaSkillList)
            {
                if (skillShellData.CoolDown > 0)
                    continue;

                m_ChaLogic.AutoCastSkill(
                    skillShellData.Model.DtId,
                    m_OwnFirePoint.transform.position + m_OwnFirePoint.offset,
                    distance.normalized,
                    m_AttackTargetPoint.transform.position + m_AttackTargetPoint.offset,
                    m_ChaLogic.ShellEntityLogic.Entity.Id
                );
                return;
            }
        }
    }
}