
namespace GameDevScript
{
    /// <summary>
    /// 触发存档的原因
    /// </summary>
    [System.Flags]
    public enum SaveArchiveReasonTypeEnum
    {
        /// <summary>
        /// 自动存档时间
        /// </summary>
        AutoSaveTime = 1,
        /// <summary>
        /// 当应用退出时
        /// </summary>
        OnApplicationQuit = 2,
        /// <summary>
        /// 应用暂停时
        /// </summary>
        OnApplicationPause_True = 4,
        /// <summary>
        /// 应用恢复时
        /// </summary>
        OnApplicationPause_False = 8,
        /// <summary>
        /// 应用获得聚焦时
        /// </summary>
        OnApplicationFocus_True = 16,
        /// <summary>
        /// 应用失去聚焦时
        /// </summary>
        OnApplicationFocus_False = 32,
    }
}