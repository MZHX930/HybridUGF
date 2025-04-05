using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 货币显示
    /// </summary>
    public class UICurrencyElement : MonoBehaviour
    {
        #region 组件
        public Image ImgCurrency;
        public TextMeshProUGUI TxtCount;
        #endregion

        #region 数据
        [SerializeField]
        [Header("货币ID")]
        private int m_CurrencyId;
        #endregion

        /// <summary>
        /// 设置货币id
        /// </summary>
        public void SetCurrencyId(int currencyId)
        {
            m_CurrencyId = currencyId;

            //TODO: 根据货币id设置图片和货币数量
            TxtCount.text = "0";
        }

        /// <summary>
        /// 设置数量
        /// </summary>
        public void SetCount(int count)
        {
            TxtCount.text = count.ToString();
        }

    }
}