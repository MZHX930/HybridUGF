namespace GameDevScript
{
    /// <summary>
    /// 实体的业务逻辑需要继承这个
    /// </summary>
    public interface IUnitCtrl<T> : IUnitCtrl where T : IUnitCtrlData
    {
        /// <summary>
        /// 实体的业务逻辑需要继承这个
        /// </summary>
        T CtrlData { get; }
    }

    public interface IUnitCtrl
    {
        /// <summary>
        /// 实体壳的渲染控制器
        /// </summary>
        ShellEntityLogic ShellEntityLogic { get; }

        /// <summary>
        /// 业务逻辑运行时(初始)数据
        /// </summary>
        IUnitCtrlData BaseCtrlData { get; }
    }
}
