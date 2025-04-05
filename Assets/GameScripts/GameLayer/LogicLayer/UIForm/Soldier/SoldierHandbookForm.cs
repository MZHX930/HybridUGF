using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameDevScript
{
    /// <summary>
    /// 士兵手册
    /// </summary>
    public partial class SoldierHandbookForm : UGuiForm<SoldierHandbookFormData>
    {
        #region 组件
        public ScrollRect ScrollView;
        public RectTransform TrsContent;
        public GameObject ObjGrid;
        #endregion

        #region  public data
        public List<SoldierHandbookGrid> GridList;
        #endregion

        #region private data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            GridList = new List<SoldierHandbookGrid>();
            GridList.Add(ObjGrid.GetComponent<SoldierHandbookGrid>());

            var dataRows = GameEntry.DataTable.GetDataTable<DRDefineSoldier>().GetAllDataRows();
            for (int i = 1; i < dataRows.Length; i++)
            {
                var grid = Instantiate(ObjGrid, TrsContent);
                GridList.Add(grid.GetComponent<SoldierHandbookGrid>());
            }

            int dataIndex = 0;
            foreach (var dtrConfig in dataRows)
            {
                GridList[dataIndex++].Init(dtrConfig);
            }
        }

        protected override void OnFinalOpen()
        {
        }

        protected override void OnFinalClose()
        {
        }
    }
}
