using System;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.Abilities
{
    /// <summary>
    /// 能力上下文
    /// </summary>
    public interface IAbilityContext : IDisposable
    {
        /// <summary>
        /// 当前能力
        /// </summary>
        Ability Ability { get; }

        /// <summary>
        /// 持有者
        /// </summary>
        IAbilityDriver Driver { get; }

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key">值的key，即GetProperty的名字</param>
        /// <typeparam name="T">值类型</typeparam>
        /// <returns>值</returns>
        T GetValue<T>(string key);
    }

    /// <summary>
    /// 能力上下文基类
    /// </summary>
    /// <typeparam name="TContext">具体类型</typeparam>
    public abstract class AbilityContext<TContext> : IAbilityContext
        where TContext : AbilityContext<TContext>, new()
    {
        private static readonly Stack<TContext> s_Pool = new Stack<TContext>();

        /// <summary>
        /// 从缓存池取出一个
        /// </summary>
        /// <param name="ability">当前能力</param>
        /// <returns>上下文实例</returns>
        public static TContext GetPooled(Ability ability)
        {
            TContext context;
            if (s_Pool.TryPop(out context) == false)
            {
                context = new TContext();
            }

            context.Ability = ability;
            return context;
        }

        /// <summary>
        /// 释放一个回到缓存池
        /// </summary>
        /// <param name="context">上下文实例</param>
        public static void Release(TContext context)
        {
            context.Ability = null;
            s_Pool.Push(context);
        }

        protected AbilityContext()
        {
            FindGetters(GetType());

            void FindGetters(Type type)
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                foreach (PropertyInfo property in properties)
                {
                    if (m_GetterMap.ContainsKey(property.Name))
                    {
                        continue;
                    }

                    Delegate func = Delegate.CreateDelegate(typeof(Func<>).MakeGenericType(property.PropertyType), this, property.GetMethod);
                    m_GetterMap.Add(property.Name, func);
                }

                if (type.BaseType != null)
                {
                    FindGetters(type.BaseType);
                }
            }
        }

        private readonly Dictionary<string, Delegate> m_GetterMap = new Dictionary<string, Delegate>();

        /// <summary>
        /// 当前能力
        /// </summary>
        public Ability Ability { get; private set; }

        /// <summary>
        /// 持有者
        /// </summary>
        public IAbilityDriver Driver => Ability.Driver;

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key">值的key，即GetProperty的名字</param>
        /// <typeparam name="T">值类型</typeparam>
        /// <returns>值</returns>
        public T GetValue<T>(string key) => ((Func<T>) m_GetterMap[key]).Invoke();

        void IDisposable.Dispose() => Release((TContext) this);
    }

    /// <summary>
    /// 仅包含基础信息的上下文
    /// </summary>
    public sealed class AbilityContext : AbilityContext<AbilityContext>
    {
    }
}