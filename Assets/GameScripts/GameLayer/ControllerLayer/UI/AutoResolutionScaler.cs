using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自适应调整CanvasScale的Match
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasScaler))]
public class AutoResolutionScaler : MonoBehaviour
{
    [HideInInspector]
    public CanvasScaler BindCanvasScaler;
    [HideInInspector]
    public Vector2 DevelopReferenceResolution;
    [HideInInspector]
    public float DevelopResolutionRatio = 1;

    private void Awake()
    {
        BindCanvasScaler = GetComponent<CanvasScaler>();

        if (null == BindCanvasScaler)
        {
            Debug.LogError($"{gameObject.name} need CanvasScaler !");
            return;
        }

        if (BindCanvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            Debug.LogError($"{gameObject.name} CanvasScaler uiScaleMode is not ScaleWithScreenSize !");
            return;
        }

        if (BindCanvasScaler.screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
        {
            Debug.LogError($"{gameObject.name} CanvasScaler screenMatchMode is not MatchWidthOrHeight !");
            return;
        }

        //x与y的比例
        DevelopReferenceResolution = BindCanvasScaler.referenceResolution;
        DevelopResolutionRatio = DevelopReferenceResolution.y / DevelopReferenceResolution.x;

        float deviceResolutionRatio = Screen.height / Screen.width;

        if (deviceResolutionRatio >= DevelopResolutionRatio)
        {
            BindCanvasScaler.matchWidthOrHeight = 0;
        }
        else
        {
            BindCanvasScaler.matchWidthOrHeight = 1;
        }
    }
}
