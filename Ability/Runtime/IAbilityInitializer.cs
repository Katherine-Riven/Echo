namespace Echo.Abilities
{
    /// <summary>
    /// 能力初始化
    /// </summary>
    public interface IAbilityInitializer
    {
        /// <summary>
        /// 初始化变量
        /// </summary>
        /// <param name="table">变量表</param>
        void InitializeVariables(AbilityVariableTable table);
    }
}