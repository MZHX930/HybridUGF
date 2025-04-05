using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    public static class UIResolutionUtility
    {
        /// <summary>
        /// 获取屏幕安全渲染区域
        /// </summary>
        public static Rect GetScreenSafeArea()
        {
            //对于Unity无法识别的设备，可通过SystemInfo.deviceModel来判定
            //比如HUAWEI MYA-AL10；Xiaomi M2007J1SC
            //还可以通过SystemInfo.graphicsDeviceName来判定
            //比如Mali-T720；Adreno (TM) 650
            return Screen.safeArea;
        }

        public static void AdjusetRectTransformAnchors(RectTransform transform, bool isRenderOutsideSafeArea)
        {
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.localPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            Vector2 offsetMax = Vector2.zero;//修改right top
            Vector2 offsetMin = Vector2.zero;//修改left bottom

            if (!isRenderOutsideSafeArea)
            {
                Rect safeArea = UIResolutionUtility.GetScreenSafeArea();
                switch (Screen.orientation)
                {
                    case ScreenOrientation.Portrait:
                        {
                            //设备处于⬆，主页按钮位于底部。
                            offsetMax = new Vector2(0f, safeArea.height - Screen.height);
                            offsetMin = new Vector2(0f, 0f);
                            break;
                        }
                    case ScreenOrientation.PortraitUpsideDown:
                        {
                            //设备处于⬇，主页按钮位于顶部。
                            offsetMax = new Vector2(0f, 0f);
                            offsetMin = new Vector2(0f, safeArea.height - Screen.height);
                            break;
                        }
                    case ScreenOrientation.LandscapeLeft:
                        {
                            //设备处于←，主页按钮位于右侧。
                            offsetMax = new Vector2(0f, 0f);
                            offsetMin = new Vector2(safeArea.width - Screen.width, 0f);
                            break;
                        }
                    case ScreenOrientation.LandscapeRight:
                        {
                            //设备处于→，主页按钮位于左侧。
                            offsetMax = new Vector2(safeArea.width - Screen.width, 0f);
                            offsetMin = new Vector2(0f, 0f);
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

            transform.offsetMax = offsetMax;
            transform.offsetMin = offsetMin;
        }

        private static void UpdateAutoRotation(out Vector2 offsetMax, out Vector2 offsetMin)
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
                        offsetMax = new Vector2(0f, safeArea.height - Screen.height);
                        offsetMin = new Vector2(0f, 0f);
                        break;
                    }
                case DeviceOrientation.PortraitUpsideDown:
                    {
                        //该设备处于纵向模式，但倒置，设备直立保持，主页按钮位于顶部。
                        offsetMax = new Vector2(0f, 0f);
                        offsetMin = new Vector2(0f, safeArea.height - Screen.height);
                        break;
                    }
                case DeviceOrientation.LandscapeLeft:
                    {
                        //设备处于横向模式，设备直立保持，主页按钮位于右侧。
                        offsetMax = new Vector2(0f, 0f);
                        offsetMin = new Vector2(safeArea.height - Screen.height, 0f);
                        break;
                    }
                case DeviceOrientation.LandscapeRight:
                    {
                        //设备处于横向模式，设备直立保持，主页按钮位于左侧。
                        offsetMax = new Vector2(safeArea.height - Screen.height, 0f);
                        offsetMin = new Vector2(0f, 0f);
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

    }
}
