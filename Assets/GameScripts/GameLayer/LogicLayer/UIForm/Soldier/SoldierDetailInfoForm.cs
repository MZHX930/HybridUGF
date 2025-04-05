using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 士兵详情
    /// </summary>
    public partial class SoldierDetailInfoForm : UGuiForm<SoldierDetailInfoFormData>
    {
        #region 组件
        public RectTransform TrsSkillContainer;
        public GameObject ObjSkillDescGrid;
        public UIFormBtn BtnClose;
        public UIFormBtn BtnUpgrade;
        #endregion

        #region  public data
        #endregion

        #region private data
        private const int c_SkillCount = 7;
        private List<SoldierSkillDescGrid> m_SkillDescGridList = new List<SoldierSkillDescGrid>();
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_SkillDescGridList.Add(ObjSkillDescGrid.GetComponent<SoldierSkillDescGrid>());
            for (int i = 1; i < c_SkillCount; i++)
            {
                var grid = Instantiate(ObjSkillDescGrid, TrsSkillContainer);
                m_SkillDescGridList.Add(grid.GetComponent<SoldierSkillDescGrid>());
            }

            BtnClose.SetClickEvent(OnClickBtnClose);
            BtnUpgrade.SetClickEvent(OnClickBtnUpgrade);
        }

        protected override void OnFinalOpen()
        {
            // UIFormData.DtrDefineSoldier;
        }

        protected override void OnFinalClose()
        {
        }

        private void OnClickBtnClose(object[] args)
        {
            Close();
        }

        private void OnClickBtnUpgrade(object[] args)
        {

        }
    }
}
