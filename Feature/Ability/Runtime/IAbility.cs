namespace Echo.Abilities
{
    public interface IAbility
    {
        /// <summary>
        /// 是否生效中
        /// </summary>
        public bool IsEnable { get; protected internal set; }

        /// <summary>
        /// 持有者
        /// </summary>
        public IAbilityOwner Owner { get; protected internal set; }

        /// <summary>
        /// 标签
        /// </summary>
        AbilityTag Tag { get; }

        /// <summary>
        /// 添加标签
        /// </summary>
        void AddTag(in AbilityTag tag);

        /// <summary>
        /// 移除标签
        /// </summary>
        void RemoveTag(in AbilityTag tag);

        /// <summary>
        /// 查找变量
        /// </summary>
        AbilityVariable<T> GetVariable<T>(string variableName);

        /// <summary>
        /// 查找功能
        /// </summary>
        AbilityFeature GetFeature(string featureName);

        /// <summary>
        /// 当启用时
        /// </summary>
        protected internal void OnEnable();

        /// <summary>
        /// 当更新时
        /// </summary>
        protected internal void OnUpdate();

        /// <summary>
        /// 当禁用时
        /// </summary>
        protected internal void OnDisable();
    }
}