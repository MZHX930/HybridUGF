using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 载具座位容器
    /// </summary>
    public class VehicleSeatContainer : MonoBehaviour
    {
        #region 组件
        public SeatGridLayout SeatGridCtrl;
        #endregion

        #region 数据
        /// <summary>
        /// 是否处于战斗中
        /// </summary>
        public bool IsFighting { get; private set; } = false;
        #endregion

        private void Start()
        {
            SeatGridCtrl.SetInfo(Constant.GameLogic.VehicleMaxGridShape, Constant.GameLogic.VehicleUnitGridSize);
        }

        /// <summary>
        /// 从触摸处获取士兵网格控制器
        /// </summary>
        public InputDragArea GetSoldierInTouchPos(Vector3 touchWorldPos)
        {
            if (!SeatGridCtrl.ContainPointPos(touchWorldPos))
            {
                return null;
            }

            SeatGrid seatGrid = SeatGridCtrl.SearchNearestSeatGrid(touchWorldPos);
            if (seatGrid.CurState == SeatGridState.Used)
            {
                var touchedSolider = seatGrid.PassengerArea;
                RemoveSoliderArea(touchedSolider);
                return touchedSolider;
            }

            return null;
        }

        /// <summary>
        /// 清理提示状态
        /// </summary>
        public void ClearHintState()
        {
            foreach (var seatGrid in SeatGridCtrl.SeatGridMap)
            {
                seatGrid.SetHintState(false, false);
            }
        }

        /// <summary>
        /// 当触摸屏幕移动手指时，给与颜色变化提示
        /// </summary>
        public void OnTouchMoveWithSolider(InputDragArea touchedDragArea)
        {
            bool isOutArea = true;
            //粗略判断士兵的顶点是否在载具的座位区域内
            foreach (var soliderVertexLocalPos in touchedDragArea.VertexLocalPosArray)
            {
                Vector3 soliderVertexWorldPos = touchedDragArea.transform.position + soliderVertexLocalPos;
                if (SeatGridCtrl.ContainPointPos(soliderVertexWorldPos))
                {
                    isOutArea = false;
                    break;
                }
            }
            if (isOutArea)
                return;//不满足条件

            //选出士兵最大形状的占位点。通过首个网格和最后网格来索引最小值
            SeatGrid[,] searchSeatGrids = SeatGridCtrl.SearchValidSeatGrids(touchedDragArea.GetSquareVertexGridWorldPos(), touchedDragArea.ChaCtrlLogic.CtrlData.GridInfo.Size);
            if (null == searchSeatGrids || searchSeatGrids.Length == 0)
                return;//不满足条件

            bool isOverlopLockGrid = false;//士兵的有效网格是否覆盖了未解锁网格
            List<SeatGrid> hintSeatGridList = new List<SeatGrid>();
            for (int y = 0; y < searchSeatGrids.GetLength(1); y++)
            {
                for (int x = 0; x < searchSeatGrids.GetLength(0); x++)
                {
                    if (!touchedDragArea.ShapeGridIsFill(x, y))
                        continue;

                    SeatGrid seatGrid = searchSeatGrids[x, y];
                    if (seatGrid.CurState == SeatGridState.Lock)
                    {
                        isOverlopLockGrid = true;
                    }
                    hintSeatGridList.Add(seatGrid);
                }
            }
            foreach (var seatGrid in hintSeatGridList)
            {
                seatGrid.SetHintState(true, !isOverlopLockGrid);
            }
        }

        /// <summary>
        /// 当触摸屏幕结束时
        /// </summary>
        /// <param name="touchedDragArea">触摸的士兵</param>
        /// <returns>
        /// 0: 全部条件都不满足，则返回原位
        /// 1: 招募
        /// 2: 替换
        /// 3: 合并升级
        /// </returns>
        public async UniTask<TouchEndResultInfo> OnTouchEndWithSolider(InputDragArea touchedDragArea)
        {
            bool isOutArea = true;
            //粗略判断士兵的顶点是否在载具的座位区域内
            foreach (var soliderVertexLocalPos in touchedDragArea.VertexLocalPosArray)
            {
                Vector3 soliderVertexWorldPos = touchedDragArea.transform.position + soliderVertexLocalPos;
                if (SeatGridCtrl.ContainPointPos(soliderVertexWorldPos))
                {
                    isOutArea = false;
                    break;
                }
            }
            if (isOutArea)
                return new TouchEndResultInfo(0, null);//不满足条件

            //选出士兵可能的占位点。通过首个网格和最后网格来索引最小值
            SeatGrid[,] searchSeatGrids = SeatGridCtrl.SearchValidSeatGrids(touchedDragArea.GetSquareVertexGridWorldPos(), touchedDragArea.ChaCtrlLogic.CtrlData.GridInfo.Size);
            if (null == searchSeatGrids || searchSeatGrids.Length == 0)
                return new TouchEndResultInfo(0, null);//不满足条件

            bool isOverlopLockGrid = false;//士兵的有效网格是否覆盖了未解锁网格
            List<InputDragArea> existSoldierDragAreaList = new List<InputDragArea>();
            for (int y = 0; y < searchSeatGrids.GetLength(1); y++)
            {
                for (int x = 0; x < searchSeatGrids.GetLength(0); x++)
                {
                    if (!touchedDragArea.ShapeGridIsFill(x, y))
                        continue;

                    SeatGrid seatGrid = searchSeatGrids[x, y];
                    if (seatGrid.CurState == SeatGridState.Lock)
                    {
                        isOverlopLockGrid = true;
                        continue;
                    }

                    if (seatGrid.CurState == SeatGridState.Used)
                    {
                        //如果有被占用，那就是替换、合并
                        if (seatGrid.PassengerArea != null)
                        {
                            if (!existSoldierDragAreaList.Contains(seatGrid.PassengerArea))
                            {
                                existSoldierDragAreaList.Add(seatGrid.PassengerArea);
                            }
                        }
                    }
                    // else
                    // {
                    //     //如果没有被占用，那么就是招募
                    // }
                }
            }

            if (existSoldierDragAreaList.Count > 0)
            {
                //如果存在士兵，那么就替换、合并
                foreach (var existSoliderLogic in existSoldierDragAreaList)
                {
                    if (existSoliderLogic.IsCanMergeUp(touchedDragArea))
                    {
                        existSoliderLogic.ToMergeUp();
                        GameEntry.Entity.HideEntity(touchedDragArea.ChaCtrlLogic.ShellEntityLogic.Entity);
                        return new TouchEndResultInfo(3, null);//合并
                    }
                }

                //替换
                List<InputDragArea> returnList = new List<InputDragArea>();
                List<UniTask> animTaskList = new List<UniTask>();
                foreach (var soldierDragArea in existSoldierDragAreaList)
                {
                    RemoveSoliderArea(soldierDragArea);
                    animTaskList.Add(soldierDragArea.GoBackStandPoint());

                    if (!returnList.Contains(soldierDragArea))
                        returnList.Add(soldierDragArea);
                }
                animTaskList.Add(AddSoliderArea(touchedDragArea, searchSeatGrids));
                await UniTask.WhenAll(animTaskList);

                return new TouchEndResultInfo(2, returnList);//替换
            }
            else
            {
                if (isOverlopLockGrid)
                {
                    await touchedDragArea.GoBackStandPoint();
                    return new TouchEndResultInfo(0, null);//返回原位
                }
                else
                {
                    await AddSoliderArea(touchedDragArea, searchSeatGrids);
                    return new TouchEndResultInfo(1, null);//招募
                }
            }
        }

        /// <summary>
        /// 从座位上移走士兵
        /// </summary>
        public void RemoveSoliderArea(InputDragArea dragArea)
        {
            foreach (var seatGrid in SeatGridCtrl.SeatGridMap)
            {
                if (seatGrid.PassengerArea == dragArea)
                {
                    seatGrid.ClearPassenger();
                }
            }
        }

        public async UniTask AddSoliderArea(InputDragArea addDragArea, SeatGrid[,] searchSeatGrids)
        {
            for (int y = 0; y < searchSeatGrids.GetLength(1); y++)
            {
                for (int x = 0; x < searchSeatGrids.GetLength(0); x++)
                {
                    var seatGrid = searchSeatGrids[x, y];
                    if (seatGrid.CurState == SeatGridState.Lock || !addDragArea.ShapeGridIsFill(x, y))
                        continue;

                    seatGrid.AddPassenger(addDragArea);
                }
            }
            await addDragArea.AlignGridWorldPos(searchSeatGrids[0, 0].transform.position);
        }


        /// <summary>
        /// 获取载具上的全部士兵
        /// </summary>
        public List<InputDragArea> GetSoldierAreasInVihicle()
        {
            List<InputDragArea> existSoldierList = new List<InputDragArea>();
            foreach (var seatGrid in SeatGridCtrl.SeatGridMap)
            {
                if (seatGrid.CurState == SeatGridState.Used && seatGrid.PassengerArea != null && !existSoldierList.Contains(seatGrid.PassengerArea))
                    existSoldierList.Add(seatGrid.PassengerArea);
            }
            return existSoldierList;
        }

        /// <summary>
        /// 切换战斗状态
        /// </summary>
        /// <param name="isFighting">是否处于战斗中</param>
        public void SwitchFightState(bool isFighting)
        {
            if (IsFighting == isFighting)
                return;

            IsFighting = isFighting;

            if (IsFighting)
            {
                //激活士兵战斗状态
                foreach (var soldier in GetSoldierAreasInVihicle())
                {
                    soldier.ChaCtrlLogic.SwitchFightState(true);
                }
            }
        }
    }

    public struct TouchEndResultInfo
    {
        /// <summary>
        /// 结果码
        /// 0: 全部条件都不满足，则返回原位
        /// 1: 招募
        /// 2: 替换
        /// 3: 合并升级
        /// </summary>
        public int ResultCode;
        /// <summary>
        /// 返回军营的士兵
        /// </summary>
        public List<InputDragArea> ReturnCampSoliderList;


        public TouchEndResultInfo(int code, List<InputDragArea> returnList)
        {
            ResultCode = code;
            ReturnCampSoliderList = returnList;
        }
    }

}