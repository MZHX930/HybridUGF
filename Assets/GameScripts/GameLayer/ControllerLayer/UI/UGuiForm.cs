using Cysharp.Threading.Tasks;
using GameFramework;
using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public abstract class UGuiForm<T> : UIFormLogic where T : UIFormData
    {
        public const int DepthFactor = 100;
        private List<Canvas> m_CachedCanvasContainer = new List<Canvas>();
        /// <summary>
        /// 通用组件
        /// </summary>
        public UGuiFormComponents BaseComponents;
        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return BaseComponents.Canvas.sortingOrder;
            }
        }
        /// <summary>
        /// 界面数据
        /// </summary>
        public T UIFormData { get; private set; }

        /// <summary>
        /// 对应的UI配置
        /// </summary>
        protected DRDefineUIForm m_UIConfig;

        protected RectTransform TrsMask;
        protected RectTransform TrsWindow;


        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            var m_TrsUIForm = GetComponent<RectTransform>();
            TrsMask = transform.Find("Mask").GetComponent<RectTransform>();
            TrsWindow = transform.Find("Window").GetComponent<RectTransform>();
            var m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            var OriginalDepth = m_CachedCanvas.sortingOrder;
            var m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();

            BaseComponents = new UGuiFormComponents()
            {
                Canvas = m_CachedCanvas,
                CanvasGroup = m_CanvasGroup,
                TrsUIForm = m_TrsUIForm,
                TrsWindow = TrsWindow,
                TrsMask = TrsMask
            };

            FullExpandUIFormRect(m_TrsUIForm);
            FullExpandUIFormRect(TrsMask);
            FullExpandUIFormRect(TrsWindow);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (null != UIFormData)
                ReferencePool.Release(UIFormData);

            if (userData == null)
                UIFormData = null;
            else
                UIFormData = userData as T;

            Tween.StopAll(BaseComponents.TrsWindow);
            OnOpenWithAnim().Forget();
        }

        private async UniTaskVoid OnOpenWithAnim()
        {
            Debug.Log($"动画时间1= {gameObject.name}  {Time.realtimeSinceStartup}");
            if (UIFormData != null)
            {
                m_UIConfig = GameEntry.DataTable.GetDataTable<DRDefineUIForm>().GetDataRow(UIFormData.DtId);
                AdjusetWindowRect(m_UIConfig.IsRenderOutsideSafeArea);

                if (m_UIConfig.OpenSoundId > 0)
                    GameEntry.Sound.PlayUISound(m_UIConfig.OpenSoundId);

                //动画
                if (!string.IsNullOrEmpty(m_UIConfig.OpenAnim))
                {
                    await UguiFormAnimDelegateFactory.OpenAnimFuncDic[m_UIConfig.OpenAnim](BaseComponents, m_UIConfig.OpenAnimParam);
                }
            }
            else
            {
                AdjusetWindowRect(false);
            }

            await UniTask.SwitchToMainThread();
            OnFinalOpen();

            if (UIFormData != null)
            {
                Debug.Log($"动画时间2=  {Time.realtimeSinceStartup}");
                GameEntry.Event.Fire(GameEntry.Tutorial, TutorialTriggerPointEventArgs.Create(EnumTutorialTriggerType.ShowUI, new string[] { UIFormData.DtId.ToString() }));
            }
        }

        /// <summary>
        /// 当UI真正展开时
        /// </summary>
        protected abstract void OnFinalOpen();

        protected override void OnClose(bool isShutdown, object userData)
        {
            if (isShutdown)
            {
                base.OnClose(isShutdown, userData);
                OnFinalClose();
            }
            else
            {
                OnPlayCloseAnim().Forget();
            }
        }

        private async UniTaskVoid OnPlayCloseAnim()
        {
            string dtIdStr = UIFormData.DtId.ToString();
            //音效
            if (m_UIConfig.CloseSoundId > 0)
                GameEntry.Sound.PlayUISound(m_UIConfig.CloseSoundId);

            //动画
            if (!string.IsNullOrEmpty(m_UIConfig.CloseAnim))
            {
                await UguiFormAnimDelegateFactory.OpenAnimFuncDic[m_UIConfig.CloseAnim](BaseComponents, m_UIConfig.CloseAnimParam);
                await UniTask.SwitchToMainThread();
            }

            if (null != UIFormData)
                ReferencePool.Release(UIFormData);

            UIFormData = null;
            m_UIConfig = null;

            OnFinalClose();

            GameEntry.Event.Fire(GameEntry.Tutorial, TutorialTriggerPointEventArgs.Create(EnumTutorialTriggerType.CloseUI, new string[] { dtIdStr }));
        }

        /// <summary>
        /// 当UI真正关闭时
        /// </summary>
        protected abstract void OnFinalClose();

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        protected override void OnRecycle()
        {
            base.OnRecycle();
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            GetComponentsInChildren(true, m_CachedCanvasContainer);
            for (int i = 0; i < m_CachedCanvasContainer.Count; i++)
            {
                m_CachedCanvasContainer[i].sortingOrder += deltaDepth;
            }

            m_CachedCanvasContainer.Clear();
        }

        public void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }

        #region 适配相关
        /// <summary>
        /// 将UIForm展开适配全屏
        /// </summary>
        private void FullExpandUIFormRect(RectTransform trsRect)
        {
            if (trsRect == null)
                return;
            trsRect.anchorMin = Vector2.zero;
            trsRect.anchorMax = Vector2.one;
            trsRect.pivot = new Vector2(0.5f, 0.5f);
            trsRect.localPosition = Vector2.zero;
            trsRect.anchoredPosition = Vector2.zero;
            trsRect.sizeDelta = Vector2.zero;
        }

        private void AdjusetWindowRect(bool isRenderOutsideSafeArea)
        {
            if (BaseComponents.TrsWindow == null)
                return;

            Vector2 offsetMax = Vector2.zero;//修改right top
            Vector2 offsetMin = Vector2.zero;//修改left bottom
            if (!isRenderOutsideSafeArea)
            {
                Rect safeArea = UIResolutionUtility.GetScreenSafeArea();
                Log.InfoYellow($"设备安全区域={safeArea} ; {Screen.width}_{Screen.height}");
                switch (Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                        {
                            //设备处于⬆，主页按钮位于底部。
                            offsetMax = new Vector2(0f, safeArea.height + safeArea.y - Screen.height);
                            offsetMin = new Vector2(0f, 0f);
                            break;
                        }
                    case ScreenOrientation.PortraitUpsideDown:
                        {
                            //设备处于⬇，主页按钮位于顶部。
                            offsetMax = new Vector2(0f, 0f);
                            offsetMin = new Vector2(0f, safeArea.y);
                            break;
                        }
                    case ScreenOrientation.LandscapeLeft:
                        {
                            //设备处于←，主页按钮位于右侧。
                            offsetMax = new Vector2(-(Screen.width - (safeArea.x + safeArea.width)), 0f);
                            offsetMin = new Vector2(safeArea.x, 0f);
                            break;
                        }
                    case ScreenOrientation.LandscapeRight:
                        {
                            //设备处于→，主页按钮位于左侧。
                            offsetMax = new Vector2(-safeArea.x, 0f);
                            offsetMin = new Vector2(Screen.width - (safeArea.x + safeArea.width), 0f);
                            break;
                        }
                    case ScreenOrientation.AutoRotation:
                        {
                            UpdateAutoRotation(out offsetMax, out offsetMin);
                            break;
                        }
                    default:
                        break;
                }
            }
            BaseComponents.TrsWindow.offsetMax = offsetMax;
            BaseComponents.TrsWindow.offsetMin = offsetMin;
        }

        private void UpdateAutoRotation(out Vector2 offsetMax, out Vector2 offsetMin)
        {
            Rect safeArea = UIResolutionUtility.GetScreenSafeArea();
            offsetMax = Vector2.zero;
            offsetMin = Vector2.zero;
            //横屏或竖屏
            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.Unknown:
                    break;
                case DeviceOrientation.Portrait:
                    {
                        //设备处于纵向模式，设备直立保持，主页按钮位于底部。
                        offsetMax = new Vector2(0f, safeArea.height + safeArea.y - Screen.height);
                        offsetMin = new Vector2(0f, 0f);
                        break;
                    }
                case DeviceOrientation.PortraitUpsideDown:
                    {
                        //该设备处于纵向模式，但倒置，设备直立保持，主页按钮位于顶部。
                        offsetMax = new Vector2(0f, 0f);
                        offsetMin = new Vector2(0f, safeArea.y);
                        break;
                    }
                case DeviceOrientation.LandscapeLeft:
                    {
                        //设备处于横向模式，设备直立保持，主页按钮位于右侧。
                        offsetMax = new Vector2(-(Screen.width - (safeArea.x + safeArea.width)), 0f);
                        offsetMin = new Vector2(safeArea.x, 0f);
                        break;
                    }
                case DeviceOrientation.LandscapeRight:
                    {
                        //设备处于横向模式，设备直立保持，主页按钮位于左侧。
                        offsetMax = new Vector2(-safeArea.x, 0f);
                        offsetMin = new Vector2(Screen.width - (safeArea.x + safeArea.width), 0f);
                        break;
                    }
                case DeviceOrientation.FaceUp:
                    break;
                case DeviceOrientation.FaceDown:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// UGuiForm的基础组件信息
    /// </summary>
    public struct UGuiFormComponents
    {
        public RectTransform TrsUIForm;
        public RectTransform TrsWindow;
        public RectTransform TrsMask;
        public Canvas Canvas;
        public CanvasGroup CanvasGroup;
    }
}
