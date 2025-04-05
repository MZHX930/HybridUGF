using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
namespace GameDevScript
{
    /// <summary>
    /// 控制战斗进度条
    /// </summary>
    public class FightProgressNode : MonoBehaviour
    {
        #region 组件
        public Animator m_Animator;
        public GameObject ObjBig;
        public GameObject ObjBigTop;
        public ProgressBoutComponent[] BigBoutComponents;
        public Image[] ImgBigArrows;

        public GameObject ObjSmall;
        public TextMeshProUGUI TxtSmallProgressValue;
        public Image ImgSmallProgressIcon;
        #endregion

        #region 数据
        private int m_CurBigState = 0;
        #endregion

        public void ShowBig(UIFightBoutShowData[] showDatas)
        {
            ShowSmall(showDatas[1]);

            m_Animator.enabled = true;
            // m_Animator.speed = 0.2f;
            ObjBig.SetActive(true);
            // ObjSmall.SetActive(false);
            m_CurBigState = 0;

            for (int i = 0; i < BigBoutComponents.Length; i++)
            {
                var showData = showDatas[i];
                if (showData.BoutId < 0)
                {
                    BigBoutComponents[i].SetActive(false);
                }
                else
                {
                    BigBoutComponents[i].SetActive(true);
                    BigBoutComponents[i].TxtDesc.text = showData.BoutId.ToString();
                    GameEntry.Sprites.LoadAtlasSprite("Fight", GetProgressStateIcon(showData.BoutType), (sprite) =>
                    {
                        BigBoutComponents[i].ImgIcon.sprite = sprite;
                    });
                }
            }

            for (int i = 0; i < ImgBigArrows.Length; i++)
            {
                bool isActive = showDatas[i].BoutId > 0 && showDatas[i + 1].BoutId > 0;
                ImgBigArrows[i].gameObject.SetActive(isActive);
            }

            PlayBigProgressAnim(1);
        }

        public void ShowSmall(UIFightBoutShowData showData)
        {
            ShowSmall();

            TxtSmallProgressValue.text = showData.BoutId.ToString();
            GameEntry.Sprites.LoadAtlasSprite("Fight", GetProgressStateIcon(showData.BoutType), (sprite) =>
            {
                ImgSmallProgressIcon.sprite = sprite;
            });
        }

        private void ShowSmall()
        {
            m_Animator.enabled = false;
            ObjBig.SetActive(false);
            ObjSmall.SetActive(true);
            m_CurBigState = 0;
        }

        /// <summary>
        /// 设置状态    
        /// </summary>
        /// <param name="state">状态</param>
        private void PlayBigProgressAnim(int state)
        {
            m_CurBigState = state;
            m_Animator.SetInteger("State", state);
        }

        /// <summary>
        /// 在Animator中被调用！！！
        /// </summary>
        private void OnAnimEnd()
        {
            if (m_CurBigState >= 3)
            {
                ShowSmall();
            }
            else
            {
                ObjSmall.SetActive(m_CurBigState > 1);
                PlayBigProgressAnim(m_CurBigState + 1);
            }
        }


        private readonly static Dictionary<int, string> m_ProgressStateIconDic = new Dictionary<int, string>
        {
            { 0, "icon_levels4" },
            { 1, "icon_levels5" },
            { 2, "icon_levels3" },
        };

        /// <summary>
        /// 获取类型对应的图标
        /// </summary>
        /// <param name="boutType">类型0:招募 1：战斗 2：随机事件</param>
        /// <returns></returns>
        private string GetProgressStateIcon(int boutType)
        {
            if (m_ProgressStateIconDic.TryGetValue(boutType, out string icon))
            {
                return icon;
            }
            Log.Error($"获取类型对应的图标失败，类型={boutType}");
            return "";
        }
    }

    [Serializable]
    public class ProgressBoutComponent
    {
        public TextMeshProUGUI TxtDesc;
        public Image ImgIcon;

        public void SetActive(bool active)
        {
            TxtDesc.gameObject.SetActive(active);
            ImgIcon.gameObject.SetActive(active);
        }

    }
}
