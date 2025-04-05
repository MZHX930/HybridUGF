using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 提示信息界面
    /// </summary>
    public partial class FloatTipsForm : UGuiForm<FloatTipsFormData>
    {
        /// <summary>
        /// 显示tips
        /// </summary>
        public static void ShowTips(string showStr)
        {
            if (m_Ins)
            {
                m_Ins.PlayTips(showStr);
            }
        }

        public static void ShowTipsKey(string languageKey)
        {
            if (m_Ins)
            {
                m_Ins.PlayTips(GameEntry.Localization.GetString(languageKey));
            }
        }

        #region 组件
        #endregion

        #region  public data
        #endregion

        #region private data
        private static FloatTipsForm m_Ins;
        private Queue<GridComponent> m_ShowGridQueue = new Queue<GridComponent>();
        private Queue<GridComponent> m_Pool = new Queue<GridComponent>();
        private Queue<string> m_WaitShowStrQueue = new Queue<string>();
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            for (int i = 0; i < TrsWindow.childCount; i++)
            {
                m_Pool.Enqueue(new GridComponent(TrsWindow.GetChild(i).gameObject));
            }
            foreach (var item in m_Pool)
            {
                item.Hide();
            }

            m_Ins = this;
        }

        protected override void OnFinalOpen()
        {
        }

        protected override void OnFinalClose()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            // 检查并清理已完成的提示
            while (m_ShowGridQueue.Count > 0)
            {
                var grid = m_ShowGridQueue.Peek();
                if (grid.IsFinished())
                {
                    m_ShowGridQueue.Dequeue();
                    grid.Hide();
                    m_Pool.Enqueue(grid);
                }
                else
                    break;
            }

            // 显示等待中的提示
            while (m_WaitShowStrQueue.Count > 0 && m_Pool.Count > 0)
            {
                var grid = m_Pool.Dequeue();
                grid.Show(m_WaitShowStrQueue.Dequeue());
                m_ShowGridQueue.Enqueue(grid);
            }

            // 更新所有显示中提示的动画
            float deltaTime = Time.deltaTime;
            foreach (var grid in m_ShowGridQueue)
            {
                grid.UpdateAnim(deltaTime);
            }
        }

        public void PlayTips(string showStr)
        {
            m_WaitShowStrQueue.Enqueue(showStr);
        }

        [System.Serializable]
        public class GridComponent
        {
            private static Vector3 m_HideLocalPos = new Vector3(-10000, 0, 0);
            private const float m_MaxAnimTime = 2f;

            public RectTransform TrsGrid;
            public TextMeshProUGUI TxtDesc;

            private float m_ElapsedTime = 0;

            public GridComponent(GameObject objGrid)
            {
                TrsGrid = objGrid.GetComponent<RectTransform>();
                TxtDesc = TrsGrid.Find("Desc").GetComponent<TextMeshProUGUI>();
            }


            public void Hide()
            {
                TrsGrid.localPosition = m_HideLocalPos;
            }

            public void Show(string str)
            {
                TxtDesc.text = str;
                m_ElapsedTime = 0;

                TrsGrid.localPosition = Vector3.zero;
                TrsGrid.localScale = Vector3.one;

                TrsGrid.SetAsLastSibling();
            }

            public void UpdateAnim(float deltaTimeSec)
            {
                m_ElapsedTime += deltaTimeSec;

                TrsGrid.anchoredPosition += new Vector2(0, 150 * deltaTimeSec);
                if (m_ElapsedTime > 1)
                {
                    float scale = Mathf.Lerp(2f, 0.2f, m_ElapsedTime / m_MaxAnimTime);
                    TrsGrid.localScale = new Vector3(scale, scale, 1);
                }
            }

            public bool IsFinished()
            {
                return m_ElapsedTime >= m_MaxAnimTime;
            }
        }
    }
}
