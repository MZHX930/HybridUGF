using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;
using UnityGameFramework.Runtime;


namespace GameDevScript
{
    /// <summary>
    /// 用于控制角色的Spine动画
    /// attack 攻击
    /// carry land 拎起时
    /// idle idle2 站立
    /// walk 行走
    /// skill 释放技能
    /// start 车停止时的抖动
    /// victory 胜利
    /// </summary>
    public class UnitSpineAnim : MonoBehaviour
    {
        /// <summary>
        /// spine控制器
        /// </summary>
        private SkeletonAnimation m_Animator;

        ///<summary>
        ///播放的倍速，作用于每个信息的duration减少速度
        ///</summary>
        public float m_TimeScale = 1;

        ///<summary>
        ///动画的逻辑信息
        ///key其实就是要播放的动画的key，比如“attack”等。
        ///value则是一个animInfo，取其RandomKey()的值就可以得到要播放的动画在animator中的名称（play()的参数）
        ///</summary>
        private Dictionary<EnumSpineAnimKey, AnimInfo> m_AnimInfoDic;

        //当前正在播放的动画的权重，只有权重>=这个值才会切换动画
        private AnimInfo m_PlayingAnim = null;

        //当前权重持续时间（单位秒），归0后，currentPriority归0
        private float m_PriorityDuration = 0;

        private int CurrentAnimPriority
        {
            get
            {
                return m_PlayingAnim == null ? 0 : (m_PriorityDuration <= 0 ? 0 : m_PlayingAnim.Priority);
            }
        }

        ///<summary>
        ///设置Animator为对象
        ///<param name="animator">要被这个unitAnim所管理的animator</param>
        ///</summary>
        public void SetAnimator(SkeletonAnimation animator)
        {
            this.m_Animator = animator;
            //更新动画信息
            m_AnimInfoDic = new Dictionary<EnumSpineAnimKey, AnimInfo>();
            // 获取骨骼数据（自动初始化未手动禁用时）
            var skeletonData = m_Animator.skeletonDataAsset.GetSkeletonData(true);
            // 遍历所有动画
            foreach (var anim in skeletonData.Animations)
            {
                string animName = anim.Name;
                float animTime = anim.Duration;

                EnumSpineAnimKey animKey = EnumSpineAnimKey.Null;
                foreach (var item in DefineSpineAnimRelation.AnimContainerDic)
                {
                    if (item.Value.Contains(animName))
                    {
                        animKey = item.Key;
                    }
                }

                if (animKey == EnumSpineAnimKey.Null)
                {
                    Log.Error("<b>{0}</b>的动画<b>{1}</b>没有找到对应的动画类型EnumSpineAnimKey", animator.name, animName);
                    continue;
                }

                int animRandomPriorty = DefineSpineAnimRelation.AnimRandomPriority.ContainsKey(animName) ? DefineSpineAnimRelation.AnimRandomPriority[animName] : 0;
                if (m_AnimInfoDic.ContainsKey(animKey))
                {
                    m_AnimInfoDic[animKey].AnimationList.Add(new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo(anim.Name, animTime), animRandomPriorty));
                }
                else
                {
                    int animPriority = (int)animKey;
                    AnimInfo _animInfo = new AnimInfo(
                        animKey.ToString(),
                        new List<KeyValuePair<SingleAnimInfo, int>>
                        {
                            new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo(anim.Name, animTime), animRandomPriorty)
                        },
                        animPriority
                    );
                    m_AnimInfoDic.Add(animKey, _animInfo);
                }
            }
            IsClear = false;
        }

        public bool IsClear = false;
        /// <summary>
        /// 清理
        /// </summary>
        public void ClearAnimator()
        {
            m_Animator = null;
            m_AnimInfoDic = null;
            IsClear = true;
        }

        void FixedUpdate()
        {
            if (!m_Animator || m_AnimInfoDic == null || m_AnimInfoDic.Count <= 0 || m_PlayingAnim == null)
                return;

            if (m_PriorityDuration > 0)
                m_PriorityDuration -= Time.fixedDeltaTime * m_TimeScale;
            else
                m_PlayingAnim = null;
        }

        ///<summary>
        ///申请播放某个动画，不是你申请就鸟你了，要看有什么正在播放的
        ///<param name="animName">动画的名称，对应animInfo的key</param>
        ///</summary>
        public float Play(EnumSpineAnimKey animKey)
        {
            if (m_Animator == null || m_AnimInfoDic == null)
                return -1;
            if (m_AnimInfoDic.ContainsKey(animKey) == false)
                return -1;
            if (m_PlayingAnim != null && m_PlayingAnim.Key == animKey.ToString())
                return -1;  //已经在播放了
            AnimInfo toPlay = m_AnimInfoDic[animKey];
            if (CurrentAnimPriority > toPlay.Priority)
                return -1;   //优先级不够不放
            SingleAnimInfo playOne = toPlay.RandomKey();

            bool isLoop = playOne.duration <= 0.1f;
            m_Animator.AnimationState.SetAnimation(0, playOne.animName, isLoop);
            m_PlayingAnim = toPlay;
            if (isLoop)
                m_PriorityDuration = 120f;
            else
                m_PriorityDuration = playOne.duration;
            return m_PriorityDuration;
        }
    }
}
