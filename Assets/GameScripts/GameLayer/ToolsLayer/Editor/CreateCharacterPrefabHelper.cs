using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 创建角色视图预制件
    /// 模板：Assets/Editor default resources/CharacterViewPrefab.prefab
    /// 存放路径：Assets/GameAssets/Entities/View
    /// spine二进制文件：Assets/GameAssets/Spine
    /// spine的定义配置表：...\rumblepaws-plan\配置表\0\定义\DefineSpineAsset_spine资源.xlsx
    /// 
    /// 功能：
    /// 一键创建缺少的CharacterViewPrefab
    /// </summary>
    public class CreateCharacterPrefabHelper : EditorToolsRootWindow
    {
        #region 常量
        private const string c_TemplatePrefabPath = "Assets/Editor default resources/CharacterViewPrefab.prefab";
        private const string c_SpineFolderPath = "Assets/GameAssets/Spine";
        private const string c_PrefabFolderPath = "Assets/GameAssets/Entities/View";

        /// <summary>
        /// 角色的层级关系<角色组，层级>
        /// </summary>
        private readonly static Dictionary<int, int> m_PrefabLayerDic = new Dictionary<int, int>()
        {
            { 1, 100 },//士兵
            { 2, 200 },//英雄
            { 3, 0 },//怪物
            { 4, -400 },//载具
            { 5, 300 },//NPC
        };
        #endregion

        #region 私有变量
        [SerializeField] private Vector2 m_ScrollPosition;
        [SerializeField] private List<CharacterAssetInfo> m_AssetList = new List<CharacterAssetInfo>();
        [SerializeField] private bool m_SelectAll = false;
        [SerializeField] private float m_RowHeight = 20f;
        [SerializeField] private float m_HeaderHeight = 25f;

        // 表格列宽定义
        private readonly float[] m_ColumnWidths = new float[] { 50f, 50f, 100f, 150f, 80f };
        private readonly string[] m_ColumnTitles = new string[] { "序号", "勾选", "二进制", "prefab", "引用相同" };

        // 用于拖拽的变量
        private bool m_IsDragging = false;
        private int m_DraggedRowIndex = -1;
        private int m_DraggedTargetIndex = -1;
        #endregion

        [MenuItem("工具/创建CharacterViewPrefab", priority = 1001)]
        public static void OpenHelper()
        {
            var window = GetWindow<CreateCharacterPrefabHelper>("创建CharacterViewPrefab");
            // window.minSize = new Vector2(500, 500);
            window.Show();
        }

        #region Unity生命周期函数
        private void OnEnable()
        {
            RefreshAssetList();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.Space(10);

            GUI.enabled = !EditorApplication.isCompiling;

            DrawToolBar();
            DrawTable();

            GUI.enabled = true;
        }
        #endregion

        #region 私有函数
        private void RefreshAssetList()
        {
            m_AssetList.Clear();

            // 确保目录存在
            if (!Directory.Exists(c_SpineFolderPath))
            {
                Debug.LogError($"目录不存在: {c_SpineFolderPath}");
                return;
            }

            // 获取所有Spine文件夹
            string[] spineFolders = Directory.GetDirectories(c_SpineFolderPath);

            // 按文件夹名称排序（假设文件夹名是数字或者有序的）
            System.Array.Sort(spineFolders, (a, b) =>
            {
                string nameA = Path.GetFileName(a);
                string nameB = Path.GetFileName(b);

                // 尝试将文件夹名转换为数字进行比较
                if (int.TryParse(nameA, out int numA) && int.TryParse(nameB, out int numB))
                {
                    return numA.CompareTo(numB);
                }

                // 如果不能转换为数字，则按字符串比较
                return string.Compare(nameA, nameB);
            });

            // 遍历每个Spine文件夹，检查是否有对应的Prefab
            for (int i = 0; i < spineFolders.Length; i++)
            {
                string folderName = Path.GetFileName(spineFolders[i]);
                string prefabPath = Path.Combine(c_PrefabFolderPath, $"{folderName}.prefab");

                // 转换为Unity路径格式
                prefabPath = prefabPath.Replace("\\", "/");

                // 检查对应的prefab是否存在
                bool prefabExists = File.Exists(prefabPath);

                // 检查二进制文件是否相等（只有当prefab存在时才需要检查）
                bool isSpineRefEqual = prefabExists && CompareSpineReference(folderName);

                // 添加到资产列表
                m_AssetList.Add(new CharacterAssetInfo
                {
                    Index = i + 1,
                    IsSelected = false,
                    SpineFolderName = folderName,
                    PrefabPath = prefabExists ? prefabPath : string.Empty,
                    HasPrefab = prefabExists,
                    IsSpineRefEqual = isSpineRefEqual
                });
            }
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // 全选/取消全选按钮
            bool newSelectAll = EditorGUILayout.ToggleLeft("全选", m_SelectAll, GUILayout.Width(50));
            if (newSelectAll != m_SelectAll)
            {
                m_SelectAll = newSelectAll;
                foreach (var asset in m_AssetList)
                {
                    asset.IsSelected = m_SelectAll;
                }
            }

            // 刷新按钮
            if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                RefreshAssetList();
            }

            // 创建所选Prefab按钮
            GUI.enabled = m_AssetList.Any(a => a.IsSelected);
            if (GUILayout.Button("创建所选Prefab", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                CreateOrUpdatePrefabs();
            }
            GUI.enabled = !EditorApplication.isCompiling;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawTable()
        {
            // 表格区域
            Rect tableRect = EditorGUILayout.GetControlRect(false, position.height - 100);

            // 绘制表头
            Rect headerRect = new Rect(tableRect.x, tableRect.y, tableRect.width, m_HeaderHeight);
            DrawTableHeader(headerRect);

            // 计算内容区域
            Rect contentRect = new Rect(tableRect.x, tableRect.y + m_HeaderHeight,
                                        tableRect.width, tableRect.height - m_HeaderHeight);

            // 计算内容总宽度（固定列宽总和）
            float totalWidth = GetTotalWidth();

            // 开始滚动视图
            m_ScrollPosition = GUI.BeginScrollView(contentRect, m_ScrollPosition,
                new Rect(0, 0, Mathf.Max(totalWidth, contentRect.width - 20), m_AssetList.Count * m_RowHeight));

            // 绘制表格行
            for (int i = 0; i < m_AssetList.Count; i++)
            {
                Rect rowRect = new Rect(0, i * m_RowHeight, Mathf.Max(totalWidth, contentRect.width - 20), m_RowHeight);

                // 如果当前行正在被拖拽，使用不同的颜色
                if (i == m_DraggedRowIndex && m_IsDragging)
                {
                    EditorGUI.DrawRect(rowRect, new Color(0.8f, 0.8f, 0.8f, 0.3f));
                }
                // 奇偶行使用不同的背景色
                else if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(rowRect, new Color(0.95f, 0.95f, 0.95f, 0.2f));
                }

                // 插入目标位置指示器
                if (m_IsDragging && i == m_DraggedTargetIndex)
                {
                    Rect indicatorRect = new Rect(rowRect.x, rowRect.y - 1, rowRect.width, 2);
                    EditorGUI.DrawRect(indicatorRect, new Color(0.4f, 0.6f, 1f, 0.7f));
                }

                DrawTableRow(rowRect, i);

                // 处理拖拽
                HandleRowDrag(rowRect, i);
            }

            GUI.EndScrollView();

            // 完成拖拽操作
            if (m_IsDragging && Event.current.type == EventType.MouseUp)
            {
                FinishDrag();
            }
        }

        // 计算表格总宽度
        private float GetTotalWidth()
        {
            float total = 0f;
            foreach (float width in m_ColumnWidths)
            {
                total += width;
            }
            return total;
        }

        private void DrawTableHeader(Rect rect)
        {
            // 绘制表头背景
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 0.3f));

            // 绘制表头文本
            float xPos = rect.x;
            for (int i = 0; i < m_ColumnWidths.Length; i++)
            {
                Rect headerRect = new Rect(xPos, rect.y, m_ColumnWidths[i], rect.height);
                GUI.Label(headerRect, m_ColumnTitles[i], EditorStyles.boldLabel);
                xPos += m_ColumnWidths[i];
            }
        }

        private void DrawTableRow(Rect rect, int index)
        {
            if (index < 0 || index >= m_AssetList.Count)
                return;

            CharacterAssetInfo asset = m_AssetList[index];

            // 计算每个单元格的位置
            float xPos = rect.x;

            // 序号列
            Rect idRect = new Rect(xPos, rect.y, m_ColumnWidths[0], rect.height);
            GUI.Label(idRect, asset.Index.ToString());
            xPos += m_ColumnWidths[0];

            // 勾选列
            Rect checkboxRect = new Rect(xPos, rect.y, m_ColumnWidths[1], rect.height);
            asset.IsSelected = EditorGUI.Toggle(checkboxRect, asset.IsSelected);
            xPos += m_ColumnWidths[1];

            // 二进制文件夹名列
            Rect binaryRect = new Rect(xPos, rect.y, m_ColumnWidths[2], rect.height);
            GUI.Label(binaryRect, asset.SpineFolderName);
            xPos += m_ColumnWidths[2];

            // Prefab路径列
            Rect prefabRect = new Rect(xPos, rect.y, m_ColumnWidths[3], rect.height);
            Color origColor = GUI.backgroundColor;
            GUI.backgroundColor = asset.HasPrefab ? new Color(0.6f, 0.9f, 0.6f, 0.6f) : new Color(0.9f, 0.6f, 0.6f, 0.6f);
            GUI.Label(prefabRect, asset.HasPrefab ? Path.GetFileName(asset.PrefabPath) : "无");
            GUI.backgroundColor = origColor;
            xPos += m_ColumnWidths[3];

            // 是否相等列
            Rect equalRect = new Rect(xPos, rect.y, m_ColumnWidths[4], rect.height);
            if (asset.HasPrefab)
            {
                GUI.backgroundColor = asset.IsSpineRefEqual ? new Color(0.6f, 0.9f, 0.6f, 0.6f) : new Color(0.9f, 0.6f, 0.6f, 0.6f);
                GUI.Label(equalRect, asset.IsSpineRefEqual ? "✔️" : "❌");
            }
            else
            {
                GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
                GUI.Label(equalRect, "❌");
            }
            GUI.backgroundColor = origColor;
        }

        private void HandleRowDrag(Rect rect, int index)
        {
            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.MouseDown:
                    if (rect.Contains(evt.mousePosition) && evt.button == 0)
                    {
                        m_DraggedRowIndex = index;
                        m_IsDragging = true;
                        evt.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (m_IsDragging)
                    {
                        // 计算目标位置
                        m_DraggedTargetIndex = Mathf.Clamp(Mathf.RoundToInt(evt.mousePosition.y / m_RowHeight), 0, m_AssetList.Count);
                        Repaint();
                        evt.Use();
                    }
                    break;
            }
        }

        private void FinishDrag()
        {
            if (m_DraggedRowIndex != -1 && m_DraggedTargetIndex != -1 && m_DraggedRowIndex != m_DraggedTargetIndex)
            {
                // 移动列表项
                CharacterAssetInfo item = m_AssetList[m_DraggedRowIndex];
                m_AssetList.RemoveAt(m_DraggedRowIndex);

                // 调整目标索引
                int targetIndex = m_DraggedTargetIndex;
                if (m_DraggedRowIndex < m_DraggedTargetIndex)
                    targetIndex--;

                m_AssetList.Insert(targetIndex, item);

                // 更新序号
                for (int i = 0; i < m_AssetList.Count; i++)
                {
                    m_AssetList[i].Index = i + 1;
                }
            }

            m_IsDragging = false;
            m_DraggedRowIndex = -1;
            m_DraggedTargetIndex = -1;
            Repaint();
        }
        #endregion


        /// <summary>
        /// 比较Spine二进制与预制件中的是否相等
        /// </summary>
        private bool CompareSpineReference(string spineName)
        {
            string spinePath = Path.Combine(c_SpineFolderPath, spineName, $"{spineName}_SkeletonData.asset");
            string prefabPath = Path.Combine(c_PrefabFolderPath, spineName) + ".prefab";
            // 判断预制件是否存在
            GameObject prefabIns = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabIns == null)
            {
                return false;
            }

            SkeletonAnimation spineAnimation = prefabIns.transform.Find("Spine").GetComponent<SkeletonAnimation>();
            if (spineAnimation == null)
            {
                return false;
            }

            return spineAnimation.skeletonDataAsset == AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(spinePath);
        }


        /// <summary>
        /// 创建或者更新预制件
        /// </summary>
        private void CreateOrUpdatePrefabs()
        {
            foreach (var asset in m_AssetList)
            {
                if (!asset.IsSelected)
                    continue;

                string spineName = asset.SpineFolderName;
                string spinePath = Path.Combine(c_SpineFolderPath, spineName, $"{spineName}_SkeletonData.asset");
                string prefabPath = Path.Combine(c_PrefabFolderPath, spineName) + ".prefab";

                GameObject prefabIns;

                if (asset.HasPrefab)
                {
                    // 更新预制件，只是替换Spine资源
                    prefabIns = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                }
                else
                {
                    // 创建预制件
                    AssetDatabase.CopyAsset(c_TemplatePrefabPath, prefabPath);
                    prefabIns = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    prefabIns.name = spineName;
                }

                SkeletonAnimation spineAnimation = prefabIns.transform.Find("Spine").GetComponent<SkeletonAnimation>();
                spineAnimation.skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(spinePath);
                // 刷新Spine资源
                spineAnimation.Initialize(true);
                spineAnimation.skeleton.SetSlotsToSetupPose();
                spineAnimation.state.ClearTracks();
                spineAnimation.state.SetAnimation(0, "idle", true);

                // 设置Spine组件的Order in Layer
                // 根据角色类型设置不同的层级
                spineAnimation.GetComponent<MeshRenderer>().sortingLayerName = "Character";
                int characterGroup = 0;
                if (int.TryParse(spineName, out int characterId))
                {
                    // 根据角色ID的第一位数字判断角色组
                    characterGroup = characterId / 1000000;
                }

                // 如果能在层级字典中找到对应的组，则设置对应的Order in Layer
                if (m_PrefabLayerDic.TryGetValue(characterGroup, out int layerOrder))
                {
                    spineAnimation.GetComponent<MeshRenderer>().sortingOrder = layerOrder;
                }
                else
                {
                    // 默认设置为0
                    spineAnimation.GetComponent<MeshRenderer>().sortingOrder = 0;
                    Debug.LogError($"未找到角色组 {characterGroup} 的层级设置，将 {spineName} 的Order in Layer设为默认值0");
                }


                EditorUtility.SetDirty(prefabIns);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshAssetList();
        }
    }

    /// <summary>
    /// 角色资产信息类
    /// </summary>
    [System.Serializable]
    public class CharacterAssetInfo
    {
        public int Index;
        public bool IsSelected;
        public string SpineFolderName;
        public string PrefabPath;
        public bool HasPrefab;
        public bool IsSpineRefEqual;
    }
}
