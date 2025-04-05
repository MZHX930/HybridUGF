//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using GameFramework.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public static class UIExtension
    {
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }

        public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId)
        {
            IDataTable<DRDefineUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRDefineUIForm>();
            var drUIForm = dtUIForm.GetDataRow((int)uiFormId);
            if (drUIForm == null)
            {
                return false;
            }

            string assetName = AssetPathUtility.GetUIFormAsset(drUIForm.AssetName);
            IUIGroup uiGroup = uiComponent.GetUIGroup(drUIForm.UIGroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(assetName);
        }

        public static UIFormLogic GetUIForm(this UIComponent uiComponent, UIFormId uiFormId)
        {
            int formId = (int)uiFormId;
            var dtUIForm = GameEntry.DataTable.GetDataTable<DRDefineUIForm>();
            var drUIForm = dtUIForm.GetDataRow(formId);
            if (drUIForm == null)
            {
                return null;
            }

            string assetName = AssetPathUtility.GetUIFormAsset(drUIForm.AssetName);
            IUIGroup uiGroup = uiComponent.GetUIGroup(drUIForm.UIGroupName);
            if (uiGroup == null)
            {
                return null;
            }

            var uiForm = (UIForm)uiGroup.GetUIForm(assetName);
            if (uiForm == null)
            {
                return null;
            }

            return uiForm.Logic;
        }

        public static void CloseUIForm(this UIComponent uiComponent, UIFormLogic uiForm)
        {
            uiComponent.CloseUIForm(uiForm.UIForm);
        }

        public static void CloseUIForm(this UIComponent uiComponent, UIFormId uiFormId)
        {
            var uiForm = uiComponent.GetUIForm(uiFormId);
            if (uiForm == null)
            {
                return;
            }

            CloseUIForm(uiComponent, uiForm);
        }

        public static int? OpenUIForm(this UIComponent uiComponent, UIFormData userData)
        {
            int uiFormId = userData.DtId;
            IDataTable<DRDefineUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRDefineUIForm>();
            var drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
                return null;
            }

            string assetName = AssetPathUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)
            {
                if (uiComponent.IsLoadingUIForm(assetName))
                {
                    return null;
                }

                if (uiComponent.HasUIForm(assetName))
                {
                    return null;
                }
            }

            return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }
    }
}
