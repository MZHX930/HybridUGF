using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 座位排序
    /// </summary>
    public class SeatGridLayout : MonoBehaviour
    {
        /// <summary>
        /// 战车座位数量
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public Vector2Int SeatCount = new Vector2Int(5, 4);

        /// <summary>
        /// 座位尺寸（米）
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public Vector2 SeatGridSize = new Vector2(1, 1);

        /// <summary>
        /// 座位状态模板
        /// </summary>
        public GameObject ObjSeat;

        /// <summary>
        /// 座位列表 从左下开始，从左到右，从下到上
        /// </summary>
        public SeatGrid[,] SeatGridMap { get; private set; }

        /// <summary>
        /// 所有座位和座位间隙组成的有效区域面积
        /// </summary>
        private Rect m_ShapeLocalMaxRect;

        public void SetInfo(Vector2Int seatCount, Vector2 seatGridSize)
        {
            SeatCount = seatCount;
            SeatGridSize = seatGridSize;
            Sort();
        }

        [ContextMenu("Sort")]
        private void MannualSort()
        {
            SetInfo(Constant.GameLogic.VehicleMaxGridShape, Constant.GameLogic.VehicleUnitGridSize);
        }

        private void Sort()
        {
            SeatGridMap = new SeatGrid[SeatCount.x, SeatCount.y];
            //补充
            int count = SeatCount.x * SeatCount.y;
            for (int i = transform.childCount; i < count; i++)
            {
                GameObject obj = Instantiate(ObjSeat, transform);
            }
            //删除多余
            for (int i = count; i < transform.childCount; i++)
            {
#if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
#else
                transform.GetChild(i).gameObject.SetActive(false);
#endif
            }

            //排序，居中，从左下开始，从左到右，从下到上
            float shapeWidth = SeatGridSize.x * SeatCount.x;
            float shapeHeight = SeatGridSize.y * SeatCount.y;
            var shapeSize = new Vector2(shapeWidth, shapeHeight);
            m_ShapeLocalMaxRect = new Rect(shapeSize * -0.5f, shapeSize);

            for (int y = 0; y < SeatCount.y; y++)
            {
                for (int x = 0; x < SeatCount.x; x++)
                {
                    int index = y * SeatCount.x + x;
                    Vector2 localPos = m_ShapeLocalMaxRect.min + new Vector2(
                        x * SeatGridSize.x + 0.5f * SeatGridSize.x,
                        y * SeatGridSize.y + 0.5f * SeatGridSize.y
                    );

                    transform.GetChild(index).localPosition = localPos;

                    var seatGrid = transform.GetChild(index).GetComponent<SeatGrid>();
                    SeatGridMap[x, y] = seatGrid;
                    seatGrid.SetGridSeatPos(x, y);
                }
            }
        }

        /// <summary>
        /// 坐标点是否在有效方位内
        /// </summary>
        public bool ContainPointPos(Vector3 point)
        {
            float minX = m_ShapeLocalMaxRect.min.x + transform.position.x;
            float maxX = minX + m_ShapeLocalMaxRect.size.x;
            float minY = m_ShapeLocalMaxRect.min.y + transform.position.y;
            float maxY = minY + m_ShapeLocalMaxRect.size.y;

            if (point.x <= minX || point.x >= maxX || point.y <= minY || point.y >= maxY)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 根据最小和最大坐标点来检索合适的座位
        /// </summary>
        /// <param name="squareVertexGridWorldPos">获取四个角落的方块中心世界坐标。以左下、右下、右上、左上为顺序存储结果</param>
        /// <param name="shapeSize">形状的尺寸</param>
        /// <returns></returns>
        public SeatGrid[,] SearchValidSeatGrids(Vector3[] squareVertexGridWorldPos, Vector2Int shapeSize)
        {
            SeatGrid[,] searchSeatGrids = new SeatGrid[shapeSize.x, shapeSize.y];
            Vector2Int startZeroGridPosIndex = default;

            #region 以左下、右上为基准来判定
            Vector3 _LBPos = squareVertexGridWorldPos[0];//左下
            SeatGrid _LBNearestSeatGrid = SearchNearestSeatGrid(_LBPos);
            Vector3 _RTPos = squareVertexGridWorldPos[2];//右上
            SeatGrid _RTNearestSeatGrid = SearchNearestSeatGrid(_RTPos);
            //以min点为基准，判断max是否在范围内。如果在，则向max点方向检索合适的座位
            Vector2Int calRTSeatGridPosIndex = new Vector2Int(
                _LBNearestSeatGrid.SeatPosIndex.x + shapeSize.x - 1,
                _LBNearestSeatGrid.SeatPosIndex.y + shapeSize.y - 1
            );
            if (calRTSeatGridPosIndex.x >= 0 && calRTSeatGridPosIndex.x < SeatCount.x &&
                calRTSeatGridPosIndex.y >= 0 && calRTSeatGridPosIndex.y < SeatCount.y)
            {
                startZeroGridPosIndex = _LBNearestSeatGrid.SeatPosIndex;
                for (int y = 0; y < shapeSize.y; y++)
                {
                    for (int x = 0; x < shapeSize.x; x++)
                    {
                        searchSeatGrids[x, y] = SeatGridMap[startZeroGridPosIndex.x + x, startZeroGridPosIndex.y + y];
                    }
                }
                return searchSeatGrids;
            }

            //以max点为基准，判断min是否在范围内。如果在，则向min点方向检索合适的座位
            Vector2Int calLBSeatGridPosIndex = new Vector2Int(
                _RTNearestSeatGrid.SeatPosIndex.x - shapeSize.x + 1,
                _RTNearestSeatGrid.SeatPosIndex.y - shapeSize.y + 1
            );
            if (calLBSeatGridPosIndex.x >= 0 && calLBSeatGridPosIndex.x < SeatCount.x &&
                calLBSeatGridPosIndex.y >= 0 && calLBSeatGridPosIndex.y < SeatCount.y)
            {
                startZeroGridPosIndex = calLBSeatGridPosIndex;
                for (int y = 0; y < shapeSize.y; y++)
                {
                    for (int x = 0; x < shapeSize.x; x++)
                    {
                        searchSeatGrids[x, y] = SeatGridMap[startZeroGridPosIndex.x + x, startZeroGridPosIndex.y + y];
                    }
                }
                return searchSeatGrids;
            }
            #endregion

            #region 以右下、左上为基准来判定
            Vector3 _RBPos = squareVertexGridWorldPos[1];//右下
            SeatGrid _RBNearestSeatGrid = SearchNearestSeatGrid(_RBPos);
            Vector3 _LTPos = squareVertexGridWorldPos[3];//左上
            SeatGrid _LTNearestSeatGrid = SearchNearestSeatGrid(_LTPos);
            //从右下到左上
            Vector2Int calLTSeatGridPosIndex = new Vector2Int(
                _RBNearestSeatGrid.SeatPosIndex.x - shapeSize.x + 1,
                _RBNearestSeatGrid.SeatPosIndex.y + shapeSize.y - 1
            );
            if (calLTSeatGridPosIndex.x >= 0 && calLTSeatGridPosIndex.x < SeatCount.x &&
                calLTSeatGridPosIndex.y >= 0 && calLTSeatGridPosIndex.y < SeatCount.y)
            {
                startZeroGridPosIndex = new Vector2Int(
                    _RBNearestSeatGrid.SeatPosIndex.x - shapeSize.x + 1,
                    _RBNearestSeatGrid.SeatPosIndex.y
                );
                for (int y = 0; y < shapeSize.y; y++)
                {
                    for (int x = 0; x < shapeSize.x; x++)
                    {
                        searchSeatGrids[x, y] = SeatGridMap[startZeroGridPosIndex.x + x, startZeroGridPosIndex.y + y];
                    }
                }
                return searchSeatGrids;
            }

            //从左上到右下
            Vector2Int calRBSeatGridPosIndex = new Vector2Int(
                _LTNearestSeatGrid.SeatPosIndex.x + shapeSize.x - 1,
                _LTNearestSeatGrid.SeatPosIndex.y - shapeSize.y + 1
            );
            if (calRBSeatGridPosIndex.x >= 0 && calRBSeatGridPosIndex.x < SeatCount.x &&
                calRBSeatGridPosIndex.y >= 0 && calRBSeatGridPosIndex.y < SeatCount.y)
            {
                startZeroGridPosIndex = new Vector2Int(
                    _LTNearestSeatGrid.SeatPosIndex.x,
                    _LTNearestSeatGrid.SeatPosIndex.y - shapeSize.y + 1
                );
                for (int y = 0; y < shapeSize.y; y++)
                {
                    for (int x = 0; x < shapeSize.x; x++)
                    {
                        searchSeatGrids[x, y] = SeatGridMap[startZeroGridPosIndex.x + x, startZeroGridPosIndex.y + y];
                    }
                }
                return searchSeatGrids;
            }
            #endregion

            return null;
        }

        public SeatGrid SearchNearestSeatGrid(Vector3 point)
        {
            SeatGrid nearestSeatGrid = null;
            float minDis = float.MaxValue;
            for (int y = 0; y < SeatCount.y; y++)
            {
                for (int x = 0; x < SeatCount.x; x++)
                {
                    SeatGrid seatGrid = SeatGridMap[x, y];
                    Vector2 vd = seatGrid.transform.position - point;
                    float sqrDis = vd.sqrMagnitude;
                    if (sqrDis < minDis)
                    {
                        minDis = sqrDis;
                        nearestSeatGrid = seatGrid;
                    }
                }
            }
            return nearestSeatGrid;
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireCube(transform.position, new Vector3(m_ShapeLocalMaxRect.size.x, m_ShapeLocalMaxRect.size.y, 0));
        // }
    }
}