using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using Spine;
using UnityGameFramework.Runtime;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GameDevScript
{
    /// <summary>
    /// 拖拽Spine角色管理器
    /// 将拖拽信息整合后传递给CharacterLogic
    /// </summary>
    public sealed class InputDragArea : MonoBehaviour, IShellShowHandler
    {
        #region 组件
        public UnitCharacterCtrl ChaCtrlLogic { get; private set; }
        #endregion

        #region 数据
        /// <summary>
        /// 网格尺寸
        /// </summary>
        private Vector2 UnitGridVirtualSize = default;
        /// <summary>
        /// 以shell为坐标原点时，拖拽面积的中心点坐标
        /// </summary>
        private Vector3 m_ShapeCenterPosInShell = default;
        /// <summary>
        /// 网格信息
        /// </summary>
        public CharacterAreaInfo GridInfo { get; private set; }
        /// <summary>
        /// shape中网格的局部坐标
        /// </summary>
        private Vector3[,] m_GridCenterPosInShellArray = null;
        /// <summary>
        /// 模糊判定中的半径
        /// </summary>
        private float m_BlurryRadius = 0;
        /// <summary>
        /// 多边形的顶点局部坐标
        /// </summary>
        public Vector3[] VertexLocalPosArray { get; private set; }
        // /// <summary>
        // /// 首格(0,0)中心点到Shape中心点的矢量距离
        // /// </summary>
        // public Vector3 ShapeCenter2ZeroGridCenterDistance { get; private set; }
        /// <summary>
        /// 拖拽点
        /// </summary>
        private Vector3 m_CarryLocalPos = default;
        #endregion

        void Awake()
        {
            UnitGridVirtualSize = Constant.GameLogic.VehicleUnitGridSize;
            ChaCtrlLogic = GetComponentInParent<UnitCharacterCtrl>();
        }

        public void OnShowShell(ShellEntityLogic shellEntityLogic) { }

        public void OnShowView(PrefabViewEntityLogic viewEntityLogic)
        {
            SkeletonAnimation skeletonAnimation = viewEntityLogic.GetComponentInChildren<SkeletonAnimation>();
            skeletonAnimation.skeleton.UpdateWorldTransform();

            //获取拖拽点
            Bone carryBone = skeletonAnimation.skeleton.FindBone("dian2");
            m_CarryLocalPos = carryBone.GetSkeletonSpacePosition();

            //获取质点
            Bone centerBone = skeletonAnimation.skeleton.FindBone("dian");
            Vector3 centerLocalPos = centerBone.GetSkeletonSpacePosition();
            SetGridInfo(centerLocalPos, ChaCtrlLogic.CtrlData.GridInfo);
        }

        public void OnHideShell(ShellEntityLogic shellEntityLogic) { }

        public void OnHideView(PrefabViewEntityLogic viewEntityLogic) { }

        /// <summary>
        /// 设置网格信息
        /// </summary>
        public void SetGridInfo(Vector3 centerLocalPos, CharacterAreaInfo info)
        {
            m_ShapeCenterPosInShell = centerLocalPos;
            GridInfo = info;

            //计算出占用的最大面积（实际占用格和未占用格）
            Vector2 maxShapeSize = new Vector2(
                info.Size.x * UnitGridVirtualSize.x,
                info.Size.y * UnitGridVirtualSize.y
            );
            //模糊半径，用于初步判定有效区域时使用
            m_BlurryRadius = Mathf.Sqrt(maxShapeSize.y * maxShapeSize.y * 0.25f + maxShapeSize.x * maxShapeSize.x * 0.25f);

            //计算出每个网格在shell为原点的坐标系下的局部坐标
            m_GridCenterPosInShellArray = new Vector3[info.Size.x, info.Size.y];
            //在以shapeCenter为原点时的，首格(0,0)中心坐标
            Vector3 zeroGridCenterPosInShaperCenter = new Vector3(UnitGridVirtualSize.x * 0.5f - maxShapeSize.x * 0.5f, UnitGridVirtualSize.y * 0.5f - maxShapeSize.y * 0.5f, 0);
            //从左到右，从下到上
            for (int gridIndexY = 0; gridIndexY < info.Size.y; gridIndexY++)
            {
                for (int gridIndexX = 0; gridIndexX < info.Size.x; gridIndexX++)
                {
                    Vector3 gridCenterInShaperCenter = new Vector3(
                        zeroGridCenterPosInShaperCenter.x + gridIndexX * UnitGridVirtualSize.x,
                        zeroGridCenterPosInShaperCenter.y + gridIndexY * UnitGridVirtualSize.y,
                        0
                    );
                    m_GridCenterPosInShellArray[gridIndexX, gridIndexY] = gridCenterInShaperCenter + m_ShapeCenterPosInShell;
                }
            }
            PickValidFrame(info);
        }

        /// <summary>
        /// 点是否在有效区域内
        /// </summary>
        /// <param name="touchPointXYPos">XY平面世界坐标</param>
        /// <returns></returns>
        public bool TouchPointIsInValidArea(Vector2 touchPointXYPos)
        {
            //使用模糊半径判定
            if (Vector2.Distance(touchPointXYPos, transform.position) > m_BlurryRadius)
            {
                return false;
            }

            //使用多边形判定
            return GraphUtils.CheckPointInPolygonShape(touchPointXYPos, transform.position, VertexLocalPosArray);
        }

        /// <summary>
        /// 解析有效边框（多边形）
        /// </summary>
        private void PickValidFrame(CharacterAreaInfo info)
        {
            //提取最外部边框
            List<GridLine> outLines = new List<GridLine>();
            for (int y = 0; y < info.Size.y; y++)
            {
                for (int x = 0; x < info.Size.x; x++)
                {
                    int gridListIndex = y * info.Size.x + x;
                    if (!info.FillState[gridListIndex])
                        continue;
                    Vector2Int gridPosIndex = new Vector2Int(x, y);

                    //下->右->上->左
                    Vector2Int downPosIndex = gridPosIndex + new Vector2Int(0, -1);
                    Vector2Int rightPosIndex = gridPosIndex + new Vector2Int(1, 0);
                    Vector2Int upPosIndex = gridPosIndex + new Vector2Int(0, 1);
                    Vector2Int leftPosIndex = gridPosIndex + new Vector2Int(-1, 0);
                    int downIndex = downPosIndex.y * info.Size.x + downPosIndex.x;
                    int rightIndex = rightPosIndex.y * info.Size.x + rightPosIndex.x;
                    int upIndex = upPosIndex.y * info.Size.x + upPosIndex.x;
                    int leftIndex = leftPosIndex.y * info.Size.x + leftPosIndex.x;

                    //4个顶点的坐标
                    Vector2 vertex0Pos = new Vector2(x - 0.5f, y - 0.5f);
                    Vector2 vertex1Pos = new Vector2(x + 0.5f, y - 0.5f);
                    Vector2 vertex2Pos = new Vector2(x + 0.5f, y + 0.5f);
                    Vector2 vertex3Pos = new Vector2(x - 0.5f, y + 0.5f);

                    if (downPosIndex.x < 0 || downPosIndex.x >= info.Size.x || downPosIndex.y < 0 || downPosIndex.y >= info.Size.y || !info.FillState[downIndex])
                    {
                        //下
                        GridLine line = new GridLine(gridPosIndex, gridListIndex, 0, 1, vertex0Pos, vertex1Pos);
                        outLines.Add(line);
                    }

                    if (rightPosIndex.x < 0 || rightPosIndex.x >= info.Size.x || rightPosIndex.y < 0 || rightPosIndex.y >= info.Size.y || !info.FillState[rightIndex])
                    {
                        //右
                        GridLine line = new GridLine(gridPosIndex, gridListIndex, 1, 2, vertex1Pos, vertex2Pos);
                        outLines.Add(line);
                    }

                    if (upPosIndex.x < 0 || upPosIndex.x >= info.Size.x || upPosIndex.y < 0 || upPosIndex.y >= info.Size.y || !info.FillState[upIndex])
                    {
                        //上
                        GridLine line = new GridLine(gridPosIndex, gridListIndex, 2, 3, vertex2Pos, vertex3Pos);
                        outLines.Add(line);
                    }

                    if (leftPosIndex.x < 0 || leftPosIndex.x >= info.Size.x || leftPosIndex.y < 0 || leftPosIndex.y >= info.Size.y || !info.FillState[leftIndex])
                    {
                        //左
                        GridLine line = new GridLine(gridPosIndex, gridListIndex, 3, 0, vertex3Pos, vertex0Pos);
                        outLines.Add(line);
                    }
                }
            }

            // 合并相邻的边
            List<GridLine> polygonalLines = new List<GridLine>();
            polygonalLines.Add(outLines[0]);
            outLines.RemoveAt(0);
            int checkCount = outLines.Count;
            while (checkCount-- > 0)
            {
                GridLine line = polygonalLines[polygonalLines.Count - 1];
                foreach (var item in outLines)
                {
                    if (line.Equals(item))
                        continue;

                    if (line.EndPoint.Equals(item.StartPoint))
                    {
                        polygonalLines.Add(item);
                        outLines.Remove(item);
                        break;
                    }
                }
            }

            List<Vector3> vertexLocalPosList = new List<Vector3>();
            for (int i = 0; i < polygonalLines.Count; i++)
            {
                GridLine line = polygonalLines[i];
                vertexLocalPosList.Add(CalPolygonVertexLocalPos(line.GridPosIndex, line.StartInnerIndex));
                vertexLocalPosList.Add(CalPolygonVertexLocalPos(line.GridPosIndex, line.EndInnerIndex));
            }
            VertexLocalPosArray = vertexLocalPosList.ToArray();
        }

        private Vector3 CalPolygonVertexLocalPos(Vector2Int gridPosIndex, int innerIndex)
        {
            Vector3 gridLocalPos = m_GridCenterPosInShellArray[gridPosIndex.x, gridPosIndex.y];
            if (innerIndex == 0)
            {
                return gridLocalPos + new Vector3(UnitGridVirtualSize.x * -0.5f, UnitGridVirtualSize.y * -0.5f, 0);
            }
            else if (innerIndex == 1)
            {
                return gridLocalPos + new Vector3(UnitGridVirtualSize.x * 0.5f, UnitGridVirtualSize.y * -0.5f, 0);
            }
            else if (innerIndex == 2)
            {
                return gridLocalPos + new Vector3(UnitGridVirtualSize.x * 0.5f, UnitGridVirtualSize.y * 0.5f, 0);
            }
            else
            {
                return gridLocalPos + new Vector3(UnitGridVirtualSize.x * -0.5f, UnitGridVirtualSize.y * 0.5f, 0);
            }
        }

        /// <summary>
        /// 获取网格中心点世界坐标
        /// </summary>
        public Vector3 GetGridCenterWorldPos(int gridListIndex)
        {
            int gridX = gridListIndex % GridInfo.Size.x;
            int gridY = gridListIndex / GridInfo.Size.x;
            return m_GridCenterPosInShellArray[gridX, gridY] + transform.position;
        }
        /// <summary>
        /// 获取网格中心点世界坐标
        /// </summary>
        public Vector3 GetGridCenterWorldPos(int gridPosIndexX, int gridPosIndexY)
        {
            return m_GridCenterPosInShellArray[gridPosIndexX, gridPosIndexY] + transform.position;
        }

        /// <summary>
        /// 获取第一个索引格子的中心世界坐标
        /// </summary>
        public Vector3 GetZeroGridWorldPos()
        {
            return m_GridCenterPosInShellArray[0, 0] + transform.position;
        }

        /// <summary>
        /// 获取最后一个索引格子的中心世界坐标
        /// </summary>
        public Vector3 GetLastGridWorldPos()
        {
            return m_GridCenterPosInShellArray[GridInfo.Size.x - 1, GridInfo.Size.y - 1] + transform.position;
        }


        /// <summary>
        /// 当拖拽开始时，播放拎起动画
        /// </summary>
        public void OnStartTouch(Vector3 worldPos)
        {
            //播放拎起动画
            ChaCtrlLogic.Play(EnumSpineAnimKey.DragStart);

            //计算出拎起点的相对矢量距离
            worldPos.z = transform.position.z;
            transform.position = worldPos - m_CarryLocalPos;
        }

        /// <summary>
        /// 当拖拽移动时
        /// </summary>
        public void OnTouchMove(Vector3 worldPos)
        {
            worldPos.z = transform.position.z;
            transform.position = worldPos - m_CarryLocalPos;
            ChaCtrlLogic.Play(EnumSpineAnimKey.DragStart);
        }

        /// <summary>
        /// 一个手指在触碰屏幕，但没有移动。
        /// </summary>
        public void OnTouchStationary()
        {
            ChaCtrlLogic.Play(EnumSpineAnimKey.DragStart);
        }

        /// <summary>
        /// 当拖拽结束时
        /// </summary>
        public void OnEndTouch()
        {
            ChaCtrlLogic.Play(EnumSpineAnimKey.DragEnd);
        }

        /// <summary>
        /// 是否覆盖在其它士兵上
        /// </summary>
        /// <param name="otherSoldier">其它士兵实体</param>
        /// <returns>是否覆盖在其它士兵上</returns>
        public bool IsOverlapOtherSoldier(InputDragArea otherSoldier)
        {
            return GraphUtils.CheckPolygonOverlap(transform.position, VertexLocalPosArray, otherSoldier.transform.position, otherSoldier.VertexLocalPosArray);
        }

        /// <summary>
        /// 触碰点是否在有效区域内
        /// </summary>
        public bool IsTouchVaildArea(Vector3 touchScreenPos)
        {
            return TouchPointIsInValidArea(FightSceneRoot.Ins.FightCamera.ScreenToWorldPoint(touchScreenPos));
        }


        /// <summary>
        /// 是否可以合并升级
        /// </summary>
        public bool IsCanMergeUp(InputDragArea otherSoldier)
        {
            if (ChaCtrlLogic.CtrlData.MergeLv >= Constant.GameLogic.SoldierMaxMergeLv)
                return false;

            //类型
            if (ChaCtrlLogic.CtrlData.DtId != otherSoldier.ChaCtrlLogic.CtrlData.DtId)
                return false;

            //等级
            if (ChaCtrlLogic.CtrlData.MergeLv != otherSoldier.ChaCtrlLogic.CtrlData.MergeLv)
                return false;

            return true;
        }

        /// <summary>
        /// 合成升级
        /// </summary>
        public void ToMergeUp()
        {
            int newMergeLv = ChaCtrlLogic.CtrlData.MergeLv + 1;
            if (newMergeLv > Constant.GameLogic.SoldierMaxMergeLv)
            {
                return;
            }

            DRDefineSoldier configData = GameEntry.DataTable.GetDataTable<DRDefineSoldier>().GetDataRow(ChaCtrlLogic.CtrlData.DtId);
            string modelPath = configData.ViewPrefixPath + newMergeLv.ToString();
            ChaCtrlLogic.UpgradeMergeLv(newMergeLv, modelPath);
        }

        /// <summary>
        /// 返回站立点
        /// </summary>
        public async UniTask GoBackStandPoint()
        {
            transform.position = ChaCtrlLogic.CtrlData.BornWorldPos;
            await UniTask.Delay(1000);
        }

        /// <summary>
        /// 获取四个角落的方块中心世界坐标。以左下、右下、右上、左上为顺序存储结果
        /// </summary>
        public Vector3[] GetSquareVertexGridWorldPos()
        {
            Vector3[] posArray = new Vector3[4];
            posArray[0] = GetGridCenterWorldPos(0, 0);
            posArray[1] = GetGridCenterWorldPos(ChaCtrlLogic.CtrlData.GridInfo.Size.x - 1, 0);
            posArray[2] = GetGridCenterWorldPos(ChaCtrlLogic.CtrlData.GridInfo.Size.x - 1, ChaCtrlLogic.CtrlData.GridInfo.Size.y - 1);
            posArray[3] = GetGridCenterWorldPos(0, ChaCtrlLogic.CtrlData.GridInfo.Size.y - 1);
            return posArray;
        }

        /// <summary>
        /// 士兵网格是否填充了
        /// </summary>
        public bool ShapeGridIsFill(int shapeGridIndexX, int shapeGridIndexY)
        {
            if (shapeGridIndexX < 0 || shapeGridIndexX >= ChaCtrlLogic.CtrlData.GridInfo.Size.x ||
                shapeGridIndexY < 0 || shapeGridIndexY >= ChaCtrlLogic.CtrlData.GridInfo.Size.y)
                return false;
            int listIndex = shapeGridIndexX + shapeGridIndexY * ChaCtrlLogic.CtrlData.GridInfo.Size.x;
            return ChaCtrlLogic.CtrlData.GridInfo.FillState[listIndex];
        }

        /// <summary>
        /// 与载具的网格坐标对齐
        /// </summary>
        public async UniTask AlignGridWorldPos(Vector3 seatGridCenterWorldPos)
        {
            transform.position = seatGridCenterWorldPos - m_GridCenterPosInShellArray[0, 0];
            await UniTask.Yield();
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (VertexLocalPosArray == null || VertexLocalPosArray.Length <= 2)
                return;

            Gizmos.color = Color.green;
            int j = VertexLocalPosArray.Length - 1;
            for (int i = 0; i < VertexLocalPosArray.Length; i++)
            {
                Vector3 startPos = VertexLocalPosArray[j] + transform.position;
                Vector3 endPos = VertexLocalPosArray[i] + transform.position;
                Gizmos.DrawLine(startPos, endPos);

                j = i;
            }
        }
#endif
    }


    public struct GridLine
    {
        public Vector2Int GridPosIndex;
        public int GridListIndex;
        public int StartInnerIndex;
        public int EndInnerIndex;
        public Vector2 StartPoint;
        public Vector2 EndPoint;

        public GridLine(Vector2Int gridPosIndex, int gridListIndex, int startInnerIndex, int endInnerIndex, Vector2 startPoint, Vector2 endPoint)
        {
            GridPosIndex = gridPosIndex;
            GridListIndex = gridListIndex;
            StartInnerIndex = startInnerIndex;
            EndInnerIndex = endInnerIndex;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public bool Equals(GridLine other)
        {
            return other.GridPosIndex == GridPosIndex && StartInnerIndex == other.StartInnerIndex && EndInnerIndex == other.EndInnerIndex;
        }

    }


    /// <summary>
    /// 角色网格信息
    /// </summary>
    public struct CharacterAreaInfo
    {
        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2Int Size;
        /// <summary>
        /// 填充情况
        /// </summary>
        public bool[] FillState;

        /// <summary>
        /// 空值
        /// </summary>
        public readonly static CharacterAreaInfo None = new CharacterAreaInfo()
        {
            Size = new Vector2Int(0, 0),
            FillState = null,
        };

    }
}
