using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// 循环循环滚动
/// 垂直滚动时，是以顶部为锚点计算坐标
/// 水平滚动时，是以左侧为锚点计算坐标
/// </summary>
[AddComponentMenu("UI/Scroll Rect Simple Extension", 37)]
public class ScrollRectSimpleEx : ScrollRect
{
    [SerializeField] private int mMaxDataCount = 0;//数据数量
    [SerializeField] private int mShowCellCount = 0;//显示的实体数量
    [SerializeField] private float mCellWidth = 0;
    [SerializeField] private float mCellHeight = 0;

    /// <summary>
    /// 游戏实例的顺序
    /// </summary>
    private RectTransform[] mCachedCellRectTrsArray;

    /// <summary>
    /// 参数值<dataIndex , cellIndex>
    /// </summary>
    private Action<int, int> mOnUpdatePosAction;

    private bool mIsInited = false;

    /// <summary>
    /// 当前显示的数据列表中的第一个数据索引
    /// </summary>
    [SerializeField] private int mCurHeadmostDataIndex = 0;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <typeparam name="T">格子类型</typeparam>
    /// <param name="dataCount">数据总量</param>
    /// <param name="cellWidth">格子宽度</param>
    /// <param name="cellHeight">格子高度</param>
    /// <param name="onScrollAction">滚动时的回调(dataIndex , cellIndex)</param>
    /// <returns></returns>
    public T[] InitScrollRect<T>(int dataCount, int cellWidth, int cellHeight, Action<int, int> onScrollAction) where T : MonoBehaviour
    {
        mMaxDataCount = dataCount;
        mCellWidth = cellWidth;
        mCellHeight = cellHeight;
        mOnUpdatePosAction = onScrollAction;

        mIsInited = true;
        return SetContentSize<T>();
    }

    private T[] SetContentSize<T>() where T : MonoBehaviour
    {
        float contenWidth = 0;
        float contentHeight = 0;
        if (horizontal)
        {
            contenWidth = mMaxDataCount * mCellWidth;
            contentHeight = mCellHeight;

            mShowCellCount = (int)(viewport.rect.width / mCellWidth) + 2;

            content.pivot = new Vector2(0, 0.5f);
            content.anchorMin = new Vector2(0, 0);
            content.anchorMax = new Vector2(0, 1);
        }
        else
        {
            contenWidth = mCellWidth;
            contentHeight = mMaxDataCount * mCellHeight;

            mShowCellCount = (int)(viewport.rect.height / mCellHeight) + 2;

            content.pivot = new Vector2(0.5f, 1);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
        }

        if (mShowCellCount > mMaxDataCount)
            mShowCellCount = mMaxDataCount;

        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contenWidth);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        Rebuild(CanvasUpdate.Layout);

        GameObject objTemplate = content.GetChild(0).gameObject;
        RectTransform rectTrsTemplate = content.GetChild(0).gameObject.transform as RectTransform;
        //左上角为锚点
        rectTrsTemplate.pivot = new Vector2(0, 1);
        rectTrsTemplate.anchorMin = new Vector2(0, 1);
        rectTrsTemplate.anchorMax = new Vector2(0, 1);

        for (int i = content.childCount; i < mShowCellCount; i++)
        {
            GameObject cloneObj = GameObject.Instantiate<GameObject>(objTemplate, content, true);
            RectTransform cloneRectTrs = cloneObj.transform as RectTransform;
            cloneRectTrs.pivot = new Vector2(0, 1);
            cloneRectTrs.anchorMin = new Vector2(0, 1);
            cloneRectTrs.anchorMax = new Vector2(0, 1);
        }

        mCachedCellRectTrsArray = new RectTransform[mShowCellCount];
        T[] cellMonoArray = new T[mShowCellCount];
        for (int i = 0; i < mShowCellCount; i++)
        {
            var trsChild = content.GetChild(i);
            mCachedCellRectTrsArray[i] = trsChild as RectTransform;
            cellMonoArray[i] = trsChild.GetComponent<T>();
            trsChild.gameObject.SetActive(true);
        }
        for (int i = mShowCellCount; i < content.childCount; i++)
            content.GetChild(i).gameObject.SetActive(false);

        return cellMonoArray;
    }


    /// <summary>
    /// 调整数据数量
    /// </summary>
    public T[] AdjustDataCount<T>(int dataCount) where T : MonoBehaviour
    {
        if (!mIsInited)
        {
            Log.Error("ScrollRectSimpleEx 还未初始化");
            return null;
        }

        mMaxDataCount = dataCount;
        return SetContentSize<T>();
    }

    /// <summary>
    /// 根据当前坐标值刷新数据
    /// </summary>
    public void Refresh(bool resetPos)
    {
        StopMovement();

        if (resetPos)
        {
            content.anchoredPosition = new Vector2(0, 0);
        }

        if (horizontal)
        {
            mCurHeadmostDataIndex = (int)(-1 * content.anchoredPosition.x / mCellHeight);
        }
        else
        {
            mCurHeadmostDataIndex = (int)(content.anchoredPosition.y / mCellHeight);
        }

        int startIndex = Mathf.Max(mCurHeadmostDataIndex, 0);
        int endIndex = mCurHeadmostDataIndex + mShowCellCount;
        for (int dataIndex = startIndex; dataIndex < endIndex; dataIndex++)
        {
            DisplayDataCell(dataIndex);
        }
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        base.SetContentAnchoredPosition(position);

        if (mIsInited)
        {
            if (horizontal)
            {
                int newHeadmostDataIndex = (int)(-1 * position.x / mCellWidth);
                AdjustCellPos(newHeadmostDataIndex);
            }
            else
            {
                int newHeadmostDataIndex = (int)(position.y / mCellHeight);
                AdjustCellPos(newHeadmostDataIndex);
            }
        }
    }

    private void AdjustCellPos(int newHeadmostDataIndex)
    {
        if (newHeadmostDataIndex == mCurHeadmostDataIndex)
            return;

        //向上(右)滑动 or 向下(左)滑动
        if (newHeadmostDataIndex > mCurHeadmostDataIndex)
        {
            for (int i = mCurHeadmostDataIndex; i <= newHeadmostDataIndex; i++)
            {
                int updateDataIndex = i + mShowCellCount - 1;
                if (updateDataIndex < mMaxDataCount)
                    DisplayDataCell(updateDataIndex);
            }
        }
        else
        {
            for (int i = mCurHeadmostDataIndex; i >= newHeadmostDataIndex; i--)
            {
                int updateDataIndex = i;
                if (updateDataIndex >= 0)
                    DisplayDataCell(updateDataIndex);
            }
        }
        mCurHeadmostDataIndex = newHeadmostDataIndex;
    }

    /// <summary>
    /// 显示第n个数据
    /// </summary>
    /// <param name="dataIndex">从0开始</param>
    private void DisplayDataCell(int dataIndex)
    {
        int cellIndex = dataIndex % mCachedCellRectTrsArray.Length;
        RectTransform rectTrsCell = mCachedCellRectTrsArray[cellIndex];
        rectTrsCell.gameObject.name = $"{dataIndex}_{cellIndex}";

        if (dataIndex < mMaxDataCount && dataIndex >= 0)
        {
            //属于正常数据范围内，更新位置和数据
            if (!rectTrsCell.gameObject.activeSelf)
                rectTrsCell.gameObject.SetActive(true);

            if (horizontal)
                rectTrsCell.anchoredPosition = new Vector3(dataIndex * mCellWidth, 0, 0);
            else
                rectTrsCell.anchoredPosition = new Vector3(0, -1 * dataIndex * mCellHeight, 0);

            mOnUpdatePosAction?.Invoke(dataIndex, cellIndex);
        }
        else
        {
            //超出数据范畴， 不显示游戏对象
            if (rectTrsCell.gameObject.activeSelf)
                rectTrsCell.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 立即显示至指定位置
    /// </summary>
    public void InstantlyDisplayToPos(float normalizedPos)
    {
        if (horizontal)
            horizontalNormalizedPosition = normalizedPos;
        else
            verticalNormalizedPosition = normalizedPos;

        Refresh(false);
    }

    #region  可视化滚动动画
    //是否正在缓慢滚动中
    private bool mIsSlowScroll = false;
    private float mSlowStartPos = 0;
    private float mSlowEndPos = 0;
    private float mSlowDuration = 0;
    private float mSlowCostTime = 0;
    private Coroutine mSlowAnimCoroutine = null;

    /// <summary>
    /// 缓慢滚动至x位置
    /// </summary>
    public void SlowScrollToPos(float normalizedPos, float duration)
    {
        if (duration <= 0)
        {
            InstantlyDisplayToPos(normalizedPos);
            return;
        }

        mIsSlowScroll = true;
        if (horizontal)
            mSlowStartPos = horizontalNormalizedPosition;
        else
            mSlowStartPos = verticalNormalizedPosition;

        mSlowEndPos = normalizedPos;
        mSlowDuration = duration;
        mSlowCostTime = 0;
        Refresh(false);

        if (mSlowAnimCoroutine != null)
            StopCoroutine(mSlowAnimCoroutine);

        mSlowAnimCoroutine = StartCoroutine(SlowUpdatePos());
    }

    private IEnumerator SlowUpdatePos()
    {
        while (mIsSlowScroll)
        {
            if (horizontal)
                horizontalNormalizedPosition = Mathf.Lerp(mSlowStartPos, mSlowEndPos, mSlowCostTime / mSlowDuration);
            else
                verticalNormalizedPosition = Mathf.Lerp(mSlowStartPos, mSlowEndPos, mSlowCostTime / mSlowDuration);
            Refresh(false);

            if (mSlowCostTime >= mSlowDuration)
                break;

            yield return new WaitForFixedUpdate();
            mSlowCostTime += Time.fixedDeltaTime;
        }

        mSlowAnimCoroutine = null;
        mIsSlowScroll = false;
        Refresh(false);
    }

    public void StopSlowScroll()
    {
        if (mIsSlowScroll == false)
            return;

        mIsSlowScroll = false;
        StopCoroutine(mSlowAnimCoroutine);
        mSlowAnimCoroutine = null;
    }
    #endregion

    public override void OnDrag(PointerEventData eventData)
    {
        StopSlowScroll();
        base.OnDrag(eventData);
    }

    protected override void OnDisable()
    {
        StopSlowScroll();
        base.OnDisable();
    }
}
