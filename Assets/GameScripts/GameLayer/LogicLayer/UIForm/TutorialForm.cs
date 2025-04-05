using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 新手引导
    /// </summary>
    public class TutorialForm : UGuiForm<TutorialFormData>
    {
        #region 组件
        public GuideMask RectangularMask;
        public GuideMask CircleMask;
        public Image FullMask;
        public Button BtnFullMask;
        public TextMeshProUGUI TxtDesc;
        #endregion

        #region  public data
        #endregion

        #region private data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            BtnFullMask.onClick.AddListener(OnClickFullMask);
        }

        protected override void OnFinalOpen()
        {
            try
            {
                Debug.Log($"动画时间Guide=  {Time.realtimeSinceStartup}");
                SetGuideMask();
                SetGuideDesc();
            }
            catch (Exception e)
            {
                Debug.LogError($"新手引导错误:dtId={UIFormData.DataRow.Id}\n{e.Message}\n{e.StackTrace}");
                Close();
            }
        }

        protected override void OnFinalClose()
        {
        }

        /// <summary>
        /// 设置引导遮罩
        /// </summary>
        private void SetGuideMask()
        {
            RectangularMask.gameObject.SetActive(UIFormData.DataRow.FocusType == 1);
            CircleMask.gameObject.SetActive(UIFormData.DataRow.FocusType == 2);
            FullMask.gameObject.SetActive(UIFormData.DataRow.FocusType == 3);

            // 设置引导区域
            if (UIFormData.DataRow.FocusType == 1)
            {
                var trsRect = GameEntry.UI.UIRoot.Find(UIFormData.DataRow.FocusPath).GetComponent<RectTransform>();
                //矩形遮罩
                RectangularMask.SetRectangularPenetration(trsRect);
            }
            else if (UIFormData.DataRow.FocusType == 2)
            {
                var trsRect = GameEntry.UI.UIRoot.Find(UIFormData.DataRow.FocusPath).GetComponent<RectTransform>();
                //圆形遮罩
                CircleMask.SetCirclePenetration(trsRect);
            }
            else
            {
                //全屏遮罩
            }
        }

        /// <summary>
        /// 设置引导描述
        /// </summary>
        private void SetGuideDesc()
        {
            TxtDesc.text = UIFormData.DataRow.DescId;
        }


        private void OnClickFullMask()
        {
            GameEntry.Event.Fire(GameEntry.Tutorial, TutorialTriggerPointEventArgs.Create(EnumTutorialTriggerType.ClickFullMask));
        }
    }
}
