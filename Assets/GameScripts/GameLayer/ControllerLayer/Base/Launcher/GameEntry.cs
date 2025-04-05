using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public delegate void OnApplicationFocusDelegate(bool hasFocus);
        public delegate void OnApplicationPauseDelegate(bool pauseStatus);
        public delegate void OnApplicationQuitDelegate();
        public delegate void UpdateDelegate(float deltaTime, float unscaledDeltaTime);
        public delegate void FixedUpdateDelegate(float fixedDeltaTime, float unscaledFixedDeltaTime);
        public delegate void LateUpdateDelegate();

        public static event OnApplicationFocusDelegate OnApplicationFocusEvent;
        public static event OnApplicationPauseDelegate OnApplicationPauseEvent;
        public static event OnApplicationQuitDelegate OnApplicationQuitEvent;
        public static event UpdateDelegate OnUpdateEvent;
        public static event FixedUpdateDelegate OnFixedUpdateEvent;
        public static event LateUpdateDelegate OnLateUpdateEvent;

        void Awake()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Invoke(hasFocus);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }

        void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
        }

        void Update()
        {
            OnUpdateEvent?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }

        void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
        }

        void LateUpdate()
        {
            OnLateUpdateEvent?.Invoke();
        }
    }
}
