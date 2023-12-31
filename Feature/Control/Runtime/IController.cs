﻿namespace Echo.Control
{
    /// <summary>
    /// 控制器
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 当控制器启用
        /// </summary>
        protected internal void OnEnable();
        
        /// <summary>
        /// 当控制器更新
        /// </summary>
        protected internal void OnUpdate();
        
        /// <summary>
        /// 当控制器禁用
        /// </summary>
        protected internal void OnDisable();
    }
}