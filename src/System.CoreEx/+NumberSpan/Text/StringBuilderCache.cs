namespace System.Text
{
    internal static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder CachedInstance;
        private const int MAX_BUILDER_SIZE = 360;

        /// <summary>
        /// Acquires the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <returns></returns>
        public static StringBuilder Acquire(int capacity = 0x10)
        {
            if (capacity <= MAX_BUILDER_SIZE)
            {
                StringBuilder cachedInstance = CachedInstance;
                if (cachedInstance != null && capacity <= cachedInstance.Capacity)
                {
                    CachedInstance = null;
                    cachedInstance.Length = 0;
                    return cachedInstance;
                }
            }
            return new StringBuilder(capacity);
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string str = sb.ToString();
            Release(sb);
            return str;
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= MAX_BUILDER_SIZE)
                CachedInstance = sb;
        }
    }
}