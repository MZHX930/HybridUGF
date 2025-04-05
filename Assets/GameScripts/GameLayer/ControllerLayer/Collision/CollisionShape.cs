#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 定义碰撞形状
    /// </summary>
    [ExecuteAlways]
    public sealed class CollisionShape : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color GizmosColor = new Color(1, 0, 0, 0.5f);
        public bool IsShowGizmos = true;
#endif

        /// <summary>
        /// 顶点的局部坐标。只考虑XY平面的多边形顶点
        /// </summary>
        /// 
        [SerializeField]
        private Vector3[] m_VertexLocalPosArray = new Vector3[3] { new Vector3(-1f, -1, 0), new Vector3(0, 1, 0), new Vector3(1f, -1, 0) };
        [HideInInspector]
        private Vector3[] m_VertexWorldPosArray = new Vector3[0];

        void Awake()
        {
#if UNITY_EDITOR
            IsShowGizmos = EditorPrefs.GetBool("ShowColliderRange", true);
#endif

            if (m_VertexLocalPosArray == null || m_VertexLocalPosArray.Length < 3 || m_VertexLocalPosArray.Length > 5)
            {
                Debug.LogError($"{gameObject.name} CollisionShape: ObjPoints is error length", gameObject);
                return;
            }

            //检测线段之间是否相交
            if (HasIntersectingLines())
            {
                Debug.LogError($"{gameObject.name} CollisionShape: 检测到线段相交，多边形无效！", gameObject);
                return;
            }

            //将顶点坐标转换为XY平面
            for (int i = 0; i < m_VertexLocalPosArray.Length; i++)
            {
                m_VertexLocalPosArray[i].z = 0;
            }

            m_VertexWorldPosArray = new Vector3[m_VertexLocalPosArray.Length];
        }

        /// <summary>
        /// 获取顶点的XY平面世界坐标
        /// </summary>
        public Vector3[] GetVertexXYPosArray()
        {
            if (m_VertexWorldPosArray.Length != m_VertexLocalPosArray.Length)
            {
                m_VertexWorldPosArray = new Vector3[m_VertexLocalPosArray.Length];
            }

            for (int i = 0; i < m_VertexLocalPosArray.Length; i++)
            {
                m_VertexWorldPosArray[i] = transform.localToWorldMatrix.MultiplyPoint3x4(m_VertexLocalPosArray[i]);
                m_VertexWorldPosArray[i].z = 0;
            }

            return m_VertexWorldPosArray;
        }

        /// <summary>
        /// 检测多边形线段之间是否存在相交
        /// </summary>
        /// <returns>如果有线段相交返回true，否则返回false</returns>
        private bool HasIntersectingLines()
        {
            int pointCount = m_VertexLocalPosArray.Length;

            // 检查每对非相邻线段是否相交
            for (int i = 0; i < pointCount; i++)
            {
                int next_i = (i + 1) % pointCount;
                Vector3 line1Start = m_VertexLocalPosArray[i];
                Vector3 line1End = m_VertexLocalPosArray[next_i];

                // 检查当前线段与所有其他非相邻线段
                for (int j = i + 2; j < pointCount + i - 1; j++)
                {
                    int jMod = j % pointCount;
                    int next_j = (jMod + 1) % pointCount;

                    // 如果j的下一个点是i，则跳过（相邻线段）
                    if (next_j == i)
                        continue;

                    Vector3 line2Start = m_VertexLocalPosArray[jMod];
                    Vector3 line2End = m_VertexLocalPosArray[next_j];

                    // 检测线段相交（只考虑XY平面，忽略Z值）
                    if (LinesIntersect(
                        new Vector2(line1Start.x, line1Start.y),
                        new Vector2(line1End.x, line1End.y),
                        new Vector2(line2Start.x, line2Start.y),
                        new Vector2(line2End.x, line2End.y)))
                    {
                        Debug.Log($"线段{i}_{next_i}和线段{j}_{next_j}相交");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断两条线段是否相交
        /// </summary>
        private bool LinesIntersect(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End)
        {
            // 计算方向向量
            Vector2 dir1 = line1End - line1Start;
            Vector2 dir2 = line2End - line2Start;

            // 计算分母
            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;

            // 如果分母为0，则线段平行或共线
            if (Mathf.Approximately(denominator, 0f))
                return false;

            // 计算线段参数t和s
            Vector2 delta = line2Start - line1Start;
            float t = (delta.x * dir2.y - delta.y * dir2.x) / denominator;
            float s = (delta.x * dir1.y - delta.y * dir1.x) / denominator;

            // 如果0<=t<=1且0<=s<=1，则线段相交
            return t >= 0f && t <= 1f && s >= 0f && s <= 1f;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (IsShowGizmos == false)
                return;

            if (m_VertexLocalPosArray == null || m_VertexLocalPosArray.Length < 3)
                return;

            Vector3[] worldVertices = GetVertexXYPosArray();
            Gizmos.color = GizmosColor;
            int startIndex = worldVertices.Length - 1;
            for (int i = 0; i < worldVertices.Length; i++)
            {
                Gizmos.DrawLine(worldVertices[startIndex], worldVertices[i]);
                Gizmos.DrawSphere(worldVertices[i], 0.1f);
                startIndex = i;
            }

            Handles.color = GizmosColor;
            Handles.DrawAAConvexPolygon(worldVertices);
        }
#endif
    }
}
