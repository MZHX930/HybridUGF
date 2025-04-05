using UnityEngine;

namespace GameDevScript
{
    [DisallowMultipleComponent]
    public class UnitRedDot : MonoBehaviour
    {
        public string RedDotKey = null;
        public GameObject ObjRedDot;

        void Awake()
        {
            if (string.IsNullOrEmpty(RedDotKey))
                return;

            GameEntry.UIRedDot.BindUINode(this);
        }

        void OnDestroy()
        {
            if (string.IsNullOrEmpty(RedDotKey))
                return;

            if (GameEntry.UIRedDot != null)
                GameEntry.UIRedDot.UnbindUINode(this);
        }

        public void SetState(bool state)
        {
            if (state)
            {
                if (ObjRedDot == null)
                {
                    ObjRedDot = new GameObject("RedDot");
                    ObjRedDot.transform.SetParent(transform);
                    ObjRedDot.transform.localPosition = Vector3.zero;
                }
                ObjRedDot.SetActive(true);
            }
            else
            {
                ObjRedDot?.SetActive(false);
            }
        }
    }
}
