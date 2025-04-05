/*脚本生成，请勿手动修改*/
namespace GameDevScript
{
    public enum EnumTutorialTriggerType : int
    {
        /// <summary>
        /// 点击按钮
        /// </summary>
        ClickBtn = 1,
        /// <summary>
        /// UI的显示动画结束后
        /// </summary>
        ShowUI = 2,
        /// <summary>
        /// UI的关闭动画结束后
        /// </summary>
        CloseUI = 3,
        /// <summary>
        /// 等待结束上一步引导
        /// </summary>
        WaitPreStep = 101,
        /// <summary>
        /// 点击引导的全遮罩任意地方
        /// </summary>
        ClickFullMask = 201,
    }
}
