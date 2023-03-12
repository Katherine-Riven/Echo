namespace Echo
{
    public static class Extension
    {
        /// <summary>
        /// 转换feature类型
        /// </summary>
        public static T CastFeature<T>(this IGameEntityFeature feature) where T : class, IGameEntityFeature
        {
            return feature as T;
        }

        /// <summary>
        /// 转换feature类型
        /// </summary>
        public static bool TryCastFeature<T>(this IGameEntityFeature feature, out T otherFeature) where T : class, IGameEntityFeature
        {
            if (feature is T result)
            {
                otherFeature = result;
                return true;
            }

            otherFeature = default;
            return false;
        }
    }
}