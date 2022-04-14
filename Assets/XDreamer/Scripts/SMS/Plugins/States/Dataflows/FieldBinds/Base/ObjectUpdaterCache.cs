using System;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base
{
    /// <summary>
    /// 对象更新器缓存器
    /// </summary>
    public class ObjectUpdaterCache
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public class Cache : TIVCache<Cache, Type, Value> { }

        /// <summary>
        /// 值
        /// </summary>
        public class Value : TIVCacheValue<Value, Type>
        {
            /// <summary>
            /// 对象更新器
            /// </summary>
            public IObjectUpdater objectUpdater { get; private set; }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <returns></returns>
            public override bool Init()
            {
                var type = LinkedTypeCache.Get(key1, nameof(BindObjectUpdaterAttribute));
                if (type != null && typeof(IObjectUpdater).IsAssignableFrom(type))
                {
                    objectUpdater = TypeHelper.CreateInstance(type) as IObjectUpdater;
                }
                return true;
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Value Get(Type type)
        {
            Cache.Cache.TryGetValue(type, out Value value);
            return value;
        }

        /// <summary>
        /// 获取对象更新器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IObjectUpdater GetObjectUpdater(Type type)
        {
            Cache.Cache.TryGetValue(type, out Value value);
            return value?.objectUpdater;
        }
    }
}
