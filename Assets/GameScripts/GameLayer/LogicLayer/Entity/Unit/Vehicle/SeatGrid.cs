using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 车座位上的网格
    /// </summary>
    public class SeatGrid : MonoBehaviour
    {
        [Header("网格实际状态")]
        public GameObject ObjLayer1;
        /// <summary>
        /// 未解锁
        /// </summary>
        public GameObject ObjLock;
        /// <summary>
        /// 等待使用
        /// </summary>
        public GameObject ObjIdle;
        /// <summary>
        /// 使用中
        /// </summary>
        public GameObject ObjUsed;

        [Header("覆盖提示")]
        public GameObject ObjLayer2;
        public GameObject ObjHintType1;
        public GameObject ObjHintType2;

        /// <summary>
        /// 当前状态
        /// </summary>
        public SeatGridState CurState { get; private set; }

        #region private data
        public Vector2Int SeatPosIndex { get; private set; }
        /// <summary>
        /// 占用网格的士兵
        /// </summary>
        public InputDragArea PassengerArea { get; private set; }
        #endregion

        void Awake()
        {
            SetHintState(false, false);
            SetState(SeatGridState.Idle);
        }

        public void SetState(SeatGridState state)
        {
            CurState = state;

            switch (state)
            {
                case SeatGridState.Lock:
                    ObjLock.gameObject.SetActive(true);
                    ObjIdle.gameObject.SetActive(false);
                    ObjUsed.gameObject.SetActive(false);
                    break;
                case SeatGridState.Idle:
                    ObjLock.gameObject.SetActive(false);
                    ObjIdle.gameObject.SetActive(true);
                    ObjUsed.gameObject.SetActive(false);
                    break;
                case SeatGridState.Used:
                    ObjLock.gameObject.SetActive(false);
                    ObjIdle.gameObject.SetActive(false);
                    ObjUsed.gameObject.SetActive(true);
                    break;
            }
        }

        public void SetGridSeatPos(int x, int y)
        {
            SeatPosIndex = new Vector2Int(x, y);
            gameObject.name = $"{x:00}_{y:00}";
        }

        public void ClearPassenger()
        {
            PassengerArea = null;
            SetState(SeatGridState.Idle);
        }

        public void AddPassenger(InputDragArea passenger)
        {
            if (CurState == SeatGridState.Lock)
            {
                Debug.LogError($"座位{SeatPosIndex}被锁定，不能添加士兵");
                return;
            }
            PassengerArea = passenger;
            SetState(SeatGridState.Used);
        }

        /// <summary>
        /// 设置提醒状态
        /// </summary>
        public void SetHintState(bool isHint, bool isVaild)
        {
            if (isHint == ObjLayer2.activeSelf && isHint != ObjHintType1.activeSelf)
            {
                return;
            }

            ObjLayer1.SetActive(!isHint);
            ObjLayer2.SetActive(isHint);
            if (isHint)
            {
                ObjHintType1.SetActive(isVaild);
                ObjHintType2.SetActive(!isVaild);
            }
        }
    }


    public enum SeatGridState
    {
        Lock,
        Idle,
        Used,
    }
}
