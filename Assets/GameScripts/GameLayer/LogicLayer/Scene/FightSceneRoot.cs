using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 控制场景
    /// 在休憩阶段时，需要预留高2个单元格(200像素)的区域，用于显示按钮。
    /// </summary>
    public class FightSceneRoot : MonoBehaviour
    {
        public static FightSceneRoot Ins { get; private set; }

        public Camera FightCamera { get { return m_FightCamera; } }

        private Queue<ScrollGroundGrid> m_GroundGridQueue;

        /// <summary>
        /// 边界信息，用于检测技能是否反弹和销毁
        /// </summary>
        public FightSceneBorderInfo BorderInfo { get; private set; }


        #region 组件
        [SerializeField]
        [Header("战斗摄像机")]
        private Camera m_FightCamera;
        #endregion

        #region 点位
        /// <summary>
        /// 中心点
        /// </summary>
        [SerializeField]
        [Header("中心点")]
        private Transform m_TrsCenter;

        [Header("地面")]
        public Transform TrsGround;

        /// <summary>
        /// 战车点位
        /// </summary>
        [SerializeField]
        [Header("战车点位")]
        private Transform[] m_TrsVehiclePoints;
        /// <summary>
        /// 怪物点位容器
        /// </summary>
        [SerializeField]
        [Header("怪物点位容器")]
        private Transform m_TrsMonsterBornCotainer;
        /// <summary>
        /// 怪物点位
        /// </summary>
        private Transform[] m_TrsMonsterBornPoints = new Transform[0];
        /// <summary>
        /// 军营点位
        /// </summary>
        [SerializeField]
        [Header("军营点位")]
        private Transform m_TrsMilitaryCamp;
        /// <summary>
        /// 地面区域点
        /// </summary>
        [SerializeField]
        [Header("地面区域点")]
        private Transform[] m_GroundAreaPoints = new Transform[4];

        /// <summary>
        /// 地板最底部的坐标Y值
        /// </summary>
        private float m_GroundDownPointY = 0;

        [SerializeField]
        [Header("建筑点位")]
        private Transform[] m_BuildingPoints = new Transform[0];

        #endregion

        #region  运行时数据
        private bool m_IsScrollGround = false;
        /// <summary>
        /// 本次滚动多久后暂停
        /// </summary>
        private float m_ResidualGroundScrollTime = 0;
        /// <summary>
        /// 地面滚动结束回调
        /// </summary>
        private Action m_OnGroundScrollEnd = null;
        #endregion

        void Awake()
        {
            Ins = this;

            FightCamera.transform.position = new Vector3(0, 0, -10);

            float borderUp = FightCamera.orthographicSize + 1f;
            float borderRight = borderUp / Screen.height * Screen.width;
            BorderInfo = new FightSceneBorderInfo(transform.position.y + borderUp, transform.position.x + borderRight, transform.position.y - borderUp, transform.position.x - borderRight);
            // Log.Info(BorderInfo.ToString());

            m_TrsMonsterBornPoints = new Transform[m_TrsMonsterBornCotainer.childCount];
            for (int i = 0; i < m_TrsMonsterBornCotainer.childCount; i++)
            {
                m_TrsMonsterBornPoints[i] = m_TrsMonsterBornCotainer.GetChild(i);
            }

            m_IsScrollGround = false;
        }

        void Start()
        {
            SetCameraFocusMode(FightCameraFocusModeEnum.Rest).Forget();

            float cameraSize = FightCamera.orthographicSize;
            GameEntry.CollisionCal.Init(FightCamera.transform.position, (cameraSize + 2) * 2, (cameraSize + 2) * 2);

            m_GroundDownPointY = m_TrsCenter.position.y - Constant.GameLogic.FightCameraFocusNormalSize;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            GameEntry.CollisionCal.Clear();
        }

        void FixedUpdate()
        {
            if (!m_IsScrollGround || SceneEntityHelper.CurActivedVehicle == null)
                return;

            if (m_ResidualGroundScrollTime > 0)
            {
                m_ResidualGroundScrollTime -= Time.fixedDeltaTime;
                //更新坐标
                float posY = m_GroundGridQueue.Peek().GetLocalPosY();
                float decayFactor;
                if (m_ResidualGroundScrollTime > 0.5f)
                {
                    decayFactor = 1;
                }
                else
                {
                    decayFactor = m_ResidualGroundScrollTime / 0.6f;
                }

                posY -= Time.fixedDeltaTime * SceneEntityHelper.CurActivedVehicle.MoveSpeed * decayFactor * decayFactor;


                foreach (var grid in m_GroundGridQueue)
                {
                    grid.SetLocalPosY(posY);
                    posY += grid.GridHeight;
                }
                //判断首个Grid是否已在摄像机外;获取队列第二个Grid的世界坐标
                float checkPosY = m_GroundGridQueue.ElementAt(1).GetLocalPosY();
                if (checkPosY < m_GroundDownPointY)
                {
                    //将首个Grid移到队尾
                    var firstGrid = m_GroundGridQueue.Dequeue();
                    firstGrid.SetLocalPosY(posY);
                    m_GroundGridQueue.Enqueue(firstGrid);
                }
            }
            else
            {
                PauseScrollGround();
            }
        }

        /// <summary>
        /// 设置地板，从屏幕顶部往底部铺设
        /// </summary>
        public void SetGroundSprites(Texture2D[][] totalTextures)
        {
            //设置地面方块
            int scrollGridCount = totalTextures.GetLength(0);
            for (int i = TrsGround.childCount; i < scrollGridCount; i++)
            {
                GameObject.Instantiate<GameObject>(TrsGround.GetChild(0).gameObject, TrsGround, false);
            }
            for (int i = 0; i < TrsGround.childCount; i++)
            {
                TrsGround.GetChild(i).gameObject.SetActive(scrollGridCount > i);
            }

            m_GroundGridQueue = new Queue<ScrollGroundGrid>();
            //美术切图的顺序是从上到下，最顶部是1。
            for (int gridIndex = scrollGridCount - 1; gridIndex >= 0; gridIndex--)
            {
                Texture2D[] gridTextures = totalTextures[gridIndex];
                ScrollGroundGrid grid = TrsGround.GetChild(gridIndex).GetComponent<ScrollGroundGrid>();
                Sprite[] layerSprites = new Sprite[gridTextures.Length];
                for (int layerIndex = 0; layerIndex < gridTextures.Length; layerIndex++)
                {
                    Texture2D sourceTexture = gridTextures[layerIndex];
                    Sprite sprite = Sprite.Create(sourceTexture, new Rect(0, 0, sourceTexture.width, sourceTexture.height), new Vector2(0.5f, 0f), Constant.GameLogic.SceneSpritePixelsPerUnit);
                    sprite.name = sourceTexture.name;
                    layerSprites[layerIndex] = sprite;
                }
                grid.Set(layerSprites);
                m_GroundGridQueue.Enqueue(grid);
            }

            float posY = m_GroundDownPointY;
            var queueEnumerator = m_GroundGridQueue.GetEnumerator();
            while (queueEnumerator.MoveNext())
            {
                queueEnumerator.Current.SetLocalPosY(posY);
                posY += queueEnumerator.Current.GridHeight;
            }
        }

        /// <summary>
        /// 开始滚动地面
        /// </summary>
        public void StartScrollGround(float scrollTime, Action onGroundScrollEnd)
        {
            m_IsScrollGround = true;
            m_ResidualGroundScrollTime = scrollTime;
            m_OnGroundScrollEnd = onGroundScrollEnd;
            SceneEntityHelper.CurActivedVehicle?.Play(EnumSpineAnimKey.VehicleWalk);
        }

        /// <summary>
        /// 暂停滚动地面
        /// </summary>
        public void PauseScrollGround()
        {
            SceneEntityHelper.CurActivedVehicle?.Play(EnumSpineAnimKey.Idle);
            m_IsScrollGround = false;
            m_ResidualGroundScrollTime = 0;
            m_OnGroundScrollEnd?.Invoke();
            m_OnGroundScrollEnd = null;
        }

        #region 摄像机
        /// <summary>
        /// 设置聚焦模式
        /// </summary>
        public async UniTask SetCameraFocusMode(FightCameraFocusModeEnum focusMode, float animTime = 0f)
        {
            switch (focusMode)
            {
                case FightCameraFocusModeEnum.Rest:

                    if (animTime > 0 && Mathf.Abs(FightCamera.orthographicSize - Constant.GameLogic.FightCameraFocusRestSize) > 0.1f)
                    {
                        StartCoroutine(PlayCameraFocusAnim(Constant.GameLogic.FightCameraFocusRestSize, Constant.GameLogic.FightCameraFocusRestWorldY, animTime));
                        await UniTask.Delay(TimeSpan.FromSeconds(animTime));
                    }
                    else
                    {
                        FightCamera.transform.position = new Vector3(0, Constant.GameLogic.FightCameraFocusRestWorldY, -10);
                        FightCamera.orthographicSize = Constant.GameLogic.FightCameraFocusRestSize;
                        await UniTask.Yield();
                    }
                    break;
                case FightCameraFocusModeEnum.Normal:

                    if (animTime > 0 && Mathf.Abs(FightCamera.orthographicSize - Constant.GameLogic.FightCameraFocusNormalSize) > 0.1f)
                    {
                        StartCoroutine(PlayCameraFocusAnim(Constant.GameLogic.FightCameraFocusNormalSize, Constant.GameLogic.FightCameraFocusWarWorldY, animTime));
                        await UniTask.Delay(TimeSpan.FromSeconds(animTime));
                    }
                    else
                    {
                        FightCamera.transform.position = new Vector3(0, Constant.GameLogic.FightCameraFocusWarWorldY, -10);
                        FightCamera.orthographicSize = Constant.GameLogic.FightCameraFocusNormalSize;
                        await UniTask.Yield();
                    }
                    break;
                default:
                    await UniTask.Yield();
                    break;
            }
        }

        private IEnumerator PlayCameraFocusAnim(float sizeEndValue, float posYEndValue, float duration)
        {
            float posYStartValue = FightCamera.transform.position.y;
            float sizeStartValue = FightCamera.orthographicSize;
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < duration)
            {
                FightCamera.transform.position = new Vector3(0, Mathf.Lerp(posYStartValue, posYEndValue, (Time.realtimeSinceStartup - startTime) / duration), -10);
                FightCamera.orthographicSize = Mathf.Lerp(sizeStartValue, sizeEndValue, (Time.realtimeSinceStartup - startTime) / duration);
                yield return null;
            }
            FightCamera.orthographicSize = sizeEndValue;
        }
        #endregion


        #region 边界

        public bool IsInBorder(Vector2 pos)
        {
            return pos.x > BorderInfo.Down && pos.x < BorderInfo.Right && pos.y > BorderInfo.Down && pos.y < BorderInfo.Up;
        }

        /// <summary>
        /// 检查是否碰撞到边界并计算反弹方向
        /// </summary>
        /// <param name="_pos">当前位置</param>
        /// <param name="_dir">当前方向</param>
        /// <returns>是否触发反弹计算了？</returns>
        public bool InnerReflect(Vector2 _pos, Vector2 _dir, out Vector2[] results)
        {
            results = new Vector2[2];

            if (_pos.y >= BorderInfo.Up)
            {
                if (_dir.y > 0)
                {
                    results[0] = new Vector2(Mathf.Clamp(_pos.x, BorderInfo.Left, BorderInfo.Right), Mathf.Clamp(_pos.y, BorderInfo.Up, BorderInfo.Down));
                    results[1] = new Vector2(_dir.x, -_dir.y);
                    return true;
                }
            }
            else if (_pos.y <= BorderInfo.Down)
            {
                if (_dir.y < 0)
                {
                    results[0] = new Vector2(Mathf.Clamp(_pos.x, BorderInfo.Left, BorderInfo.Right), Mathf.Clamp(_pos.y, BorderInfo.Down, BorderInfo.Up));
                    results[1] = new Vector2(_dir.x, -_dir.y);
                    return true;
                }
            }
            else if (_pos.x >= BorderInfo.Right)
            {
                if (_dir.x > 0)
                {
                    results[0] = new Vector2(Mathf.Clamp(_pos.x, BorderInfo.Right, BorderInfo.Left), Mathf.Clamp(_pos.y, BorderInfo.Down, BorderInfo.Up));
                    results[1] = new Vector2(-_dir.x, _dir.y);
                    return true;
                }
            }
            else if (_pos.x <= BorderInfo.Left)
            {
                if (_dir.x < 0)
                {
                    results[0] = new Vector2(Mathf.Clamp(_pos.x, BorderInfo.Left, BorderInfo.Right), Mathf.Clamp(_pos.y, BorderInfo.Down, BorderInfo.Up));
                    results[1] = new Vector2(-_dir.x, _dir.y);
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region 坐标点
        /// <summary>
        /// 获取载具点
        /// 0:招募时点
        /// </summary>
        public Vector3 GetVehiclePointPos(int pointType)
        {
            return m_TrsVehiclePoints[pointType].position;
        }

        /// <summary>
        /// 获取怪物点位数量
        /// </summary>
        /// <returns></returns>
        public int GetMonsterBornPointCount()
        {
            return m_TrsMonsterBornPoints.Length;
        }

        /// <summary>
        /// 获取第index个怪物点位
        /// </summary>
        public Vector3 GetMonsterBornWorldPos(int index)
        {
            if (index < 0 || index >= m_TrsMonsterBornPoints.Length)
            {
                return default;
            }
            return m_TrsMonsterBornPoints[index].position;
        }

        /// <summary>
        /// 获取怪物出生点坐标列表
        /// </summary>
        public Vector3[] GetMonsterBornWorldPosArray()
        {
            return m_TrsMonsterBornPoints.Select(t => t.position).ToArray();
        }

        /// <summary>
        /// 获取酒馆士兵停留水平中心点
        /// </summary>
        public Vector3 GetMilitaryCampWorldPos()
        {
            return m_TrsMilitaryCamp.position;
        }

        /// <summary>
        /// 获取酒馆建筑摆放位置
        /// </summary>
        public Vector3 GetTavernWorldPos()
        {
            return m_BuildingPoints[0].position;
        }

        #endregion


        void OnDrawGizmos()
        {
            //绘制中心点
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(m_TrsCenter.position, Vector3.one * 0.2f);

            //战车坐标点
            foreach (var point in m_TrsVehiclePoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
            }

            //怪物出生点坐标点
            Gizmos.color = Color.blue;
            foreach (var point in m_TrsMonsterBornPoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
            }

            //地面区域点
            Gizmos.color = Color.yellow;
            foreach (var point in m_GroundAreaPoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
            }

            for (int i = 0; i < m_GroundAreaPoints.Length; i++)
            {
                Gizmos.DrawLine(m_GroundAreaPoints[i].position, m_GroundAreaPoints[(i + 1) % m_GroundAreaPoints.Length].position);
            }

            //绘制建筑点位
            Gizmos.color = Color.white;
            foreach (var point in m_BuildingPoints)
            {
                Gizmos.DrawSphere(point.position, 0.2f);
            }

            //绘制摄像机的视图
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(FightCamera.transform.position, new Vector3(FightCamera.orthographicSize * 2 * FightCamera.aspect, FightCamera.orthographicSize * 2, 0));
        }
    }


    /// <summary>
    /// 战斗摄像机聚焦模式
    /// </summary>
    public enum FightCameraFocusModeEnum
    {
        Normal,
        Rest,
    }

    public struct FightSceneBorderInfo
    {
        public float Up;
        public float Right;
        public float Down;
        public float Left;

        public FightSceneBorderInfo(float up, float right, float down, float left)
        {
            Up = up;
            Right = right;
            Down = down;
            Left = left;
        }

        public override string ToString()
        {
            return $"Up: {Up}, Right: {Right}, Down: {Down}, Left: {Left}";
        }
    }


}
