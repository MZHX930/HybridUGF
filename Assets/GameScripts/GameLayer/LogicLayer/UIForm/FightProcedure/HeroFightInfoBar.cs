using TMPro;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 英雄的战斗信息栏
    /// </summary>
    public class HeroFightInfoBar : MonoBehaviour
    {
        #region  组件
        public GameObject ObjHp;
        public TextMeshProUGUI TxtHpCount;
        public GameObject ObjShield;
        public TextMeshProUGUI TxtShieldCount;
        #endregion


        public void SetHp(int hp)
        {
            TxtHpCount.text = hp.ToString();
        }

        public void SetShield(int shield)
        {
            TxtShieldCount.text = shield.ToString();
        }

    }
}
