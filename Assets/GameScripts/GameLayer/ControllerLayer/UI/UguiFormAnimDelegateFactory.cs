using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Cysharp.Threading.Tasks;

namespace GameDevScript
{
    public static class UguiFormAnimDelegateFactory
    {
        /// <summary>
        /// 打开时的动画
        /// </summary>
        public static Dictionary<string, OpenAnimFunc> OpenAnimFuncDic = new Dictionary<string, OpenAnimFunc>()
        {
            {"ScaleAnim", ScaleAnim},
        };

        /// <summary>
        /// 关闭时的动画
        /// </summary>
        public static Dictionary<string, CloseAnimFunc> CloseAnimFuncDic = new Dictionary<string, CloseAnimFunc>()
        {
            {"ScaleAnim", ScaleAnim},
        };

        /// <summary>
        /// 由小放大缩放动画
        /// </summary>
        /// <param name="components">基础组件</param>
        /// <param name="animParam">动画参数[0]时间[1]开始值[2]结束值</param>
        private static async UniTask ScaleAnim(UGuiFormComponents components, string[] animParam)
        {
            if (animParam == null || animParam.Length != 3)
            {
                Debug.LogError("动画参数错误");
                return;
            }
            float time = float.Parse(animParam[0]);
            float startRatio = float.Parse(animParam[1]);
            float endRatio = float.Parse(animParam[2]);
            await Tween.Scale(components.TrsWindow, Vector3.one * startRatio, Vector3.one * endRatio, time, useUnscaledTime: true);
            // await UniTask.Delay(TimeSpan.FromSeconds(time));
        }
    }

    public delegate UniTask OpenAnimFunc(UGuiFormComponents components, string[] animParam);
    public delegate UniTask CloseAnimFunc(UGuiFormComponents components, string[] animParam);
}

