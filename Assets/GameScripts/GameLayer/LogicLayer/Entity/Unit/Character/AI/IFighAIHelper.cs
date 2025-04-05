using GameFramework;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 战斗AI辅助类
    /// </summary>
    public interface IFighAIHelper : IReference
    {
        public static T Create<T>() where T : class, IFighAIHelper, new()
        {
            return ReferencePool.Acquire<T>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init(UnitCharacterCtrl chaLogic);

        /// <summary>
        /// 更新AI
        /// </summary>
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
    }
}
