using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 战斗胜利界面
    /// </summary>
    public class FightVictoryForm : UGuiForm<FightVictoryFormData>
    {
        #region 组件
        public UIFormBtn BtnSure;
        #endregion

        #region  public data
        #endregion

        #region private data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            BtnSure.SetClickEvent(OnBtnSureClick);
        }

        protected override void OnFinalClose()
        {

        }

        protected override void OnFinalOpen()
        {
        }

        private void OnBtnSureClick(params object[] args)
        {
            GameEntry.Event.Fire(this, GoBackMainSceneEventArgs.Create());
        }
    }
}
