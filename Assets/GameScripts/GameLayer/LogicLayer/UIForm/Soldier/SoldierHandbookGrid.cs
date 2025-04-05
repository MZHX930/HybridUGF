using System;
using TMPro;
using UnityEngine;

namespace GameDevScript
{
    public class SoldierHandbookGrid : MonoBehaviour
    {
        #region 组件
        public TextMeshProUGUI TxtName;
        public UIFormBtn BtnLook;
        public UIFormBtn BtnActive;
        #endregion

        #region Data
        public DRDefineSoldier DtrConfig;
        private UnitSoldierArchiveData ArchiveData;
        #endregion


        void Awake()
        {
            BtnLook.SetClickEvent(OnClickBtnLook);
            BtnActive.SetClickEvent(OnClickBtnActive);
        }

        public void Init(DRDefineSoldier dtrConfig)
        {
            DtrConfig = dtrConfig;
            ArchiveData = GameEntry.GameArchive.GetSoldierArchiveData(DtrConfig.Id);

            ShowGridInfo();
        }

        private void ShowGridInfo()
        {
            TxtName.text = DtrConfig.Id.ToString();
            if (ArchiveData.Lv <= 0)
            {
                //解锁
                BtnActive.SetLanguageKey("ToUnlcok");
            }
            else
            {
                if (ArchiveData.IsActivated)
                {
                    //下阵
                    BtnActive.SetLanguageKey("ToUnactive");
                }
                else
                {
                    //上阵
                    BtnActive.SetLanguageKey("ToActive");
                }
            }
        }

        private void OnClickBtnActive(object[] args)
        {
            if (ArchiveData.Lv <= 0)
            {
                //解锁
                GameEntry.GameArchive.UpgradeSoliderLv(DtrConfig.Id);
            }
            else
            {
                GameEntry.GameArchive.ModifySoldierActivatedState(DtrConfig.Id, !ArchiveData.IsActivated);
            }
            ShowGridInfo();
        }

        private void OnClickBtnLook(object[] args)
        {
            GameEntry.UI.OpenUIForm(SoldierDetailInfoFormData.Create(DtrConfig));
        }
    }
}
