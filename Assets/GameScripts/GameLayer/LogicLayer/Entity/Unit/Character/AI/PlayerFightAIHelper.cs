using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 友军AI辅助类
    /// </summary>
    public class PlayerFightAIHelper : IFighAIHelper
    {
        private UnitCharacterCtrl m_ChaLogic;

        public void Clear()
        {
            m_ChaLogic = null;
        }

        public void Init(UnitCharacterCtrl chaLogic)
        {
            m_ChaLogic = chaLogic;
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            AutoCastSkill();
        }

        private void AutoCastSkill()
        {
            //如果没有敌人，则跳过释放特效过程
            UnitCharacterCtrl targetCha;
            Vector3 firePos = m_ChaLogic.GetBindPoint(Constant.GameLogic.BindPoint_FirePoint).transform.position;

            //同一帧内只释放一个技能
            foreach (var skillShellData in m_ChaLogic.CtrlData.ChaSkillList)
            {
                if (skillShellData.CoolDown > 0)
                    continue;

                //寻找敌人，如果没有则不创建技能特效实例
                if (skillShellData.Model.SearchType == 1)
                {
                    //随机目标
                    if (!EnemyCampMgr.Ins.SearchRandomMonster(out targetCha))
                        continue;
                }
                else if (skillShellData.Model.SearchType == 2)
                {
                    //最近目标
                    if (!EnemyCampMgr.Ins.SearchNearestMonster(firePos, out targetCha))
                        continue;
                }
                else
                {
                    Debug.LogError($"技能索敌类型错误 {skillShellData.Model.SearchType}");
                    continue;
                }

                if (targetCha == null)
                {
                    Log.EDebug($"没有找到目标，跳过释放技能 {skillShellData.Model.DtId}");
                    continue;
                }

                UnitBindPoint targetPoint;
                if ((targetPoint = targetCha.GetBindPoint(Constant.GameLogic.BindPoint_CenterPoint)) != null)
                {
                    Vector3 targetPos = targetPoint.transform.position;
                    Vector3 fireDire = (targetPos - firePos).normalized;

                    m_ChaLogic.AutoCastSkill(skillShellData.Model.DtId, firePos, fireDire.normalized, targetPos, m_ChaLogic.ShellEntityLogic.Entity.Id);
                    return;
                }
            }
        }
    }
}
