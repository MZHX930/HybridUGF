using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    public class LoadAssetsForm : UGuiForm<LoadAssetsFormData>
    {
        #region 组件
        public RectTransform TrsProgressValue;
        #endregion

        #region private data
        #endregion

        #region  public data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnFinalOpen()
        {
            SetProgress(0);
        }
        protected override void OnFinalClose()
        {
        }

        public void SetProgress(float progress)
        {
            TrsProgressValue.localScale = new Vector3(Mathf.Clamp01(progress), 1, 1);
        }
    }
}
