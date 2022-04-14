using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 记录器：可用于批量记录数据的记录器；
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class Recorder<T, TRecord> : IBatchRecorder<T, TRecord>
        where TRecord : class, ISingleRecord<T>, new()
    {
        /// <summary>
        /// 记录信息列表
        /// </summary>
        public List<TRecord> _records = new List<TRecord>();

        /// <summary>
        /// 记录列表
        /// </summary>
        public TRecord[] records => _records.ToArray();

        /// <summary>
        /// 记录列表中第一条记录：如果无记录返回null
        /// </summary>
        public TRecord firstRecod => _records.FirstOrDefault();

        /// <summary>
        /// 记录列表中末一条记录：如果无记录返回null
        /// </summary>
        public TRecord lastRecod => _records.LastOrDefault();

        /// <summary>
        /// 有无记录
        /// </summary>
        /// <returns></returns>
        public bool HasRecord() => _records.Count > 0;

        /// <summary>
        /// 清空记录信息列表<see cref="_records"/>
        /// </summary>
        public void Clear()
        {
            _records.Clear();
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Record(T obj)
        {
            //考虑可能存在的比较函数重载情况
            if (!Equals(obj, null))
            {
                var record = new TRecord();
                record.Record(obj);
                _records.Add(record);
            }
        }

        /// <summary>
        /// 批量记录
        /// </summary>
        /// <param name="objects"></param>
        public virtual void Record(IEnumerable<T> objects)
        {
            if (objects == null) return;
            foreach (var obj in objects)
            {
                Record(obj);
            }
        }

        /// <summary>
        /// 恢复：将所有信息都执行恢复
        /// </summary>
        public virtual void Recover()
        {
            foreach (var i in _records)
            {
                try
                {
                    i.Recover();
                }
                catch { }
            }
        }

        /// <summary>
        /// 条件恢复
        /// </summary>
        /// <param name="canRecoverFunc"></param>
        public virtual void Recover(Func<TRecord, bool> canRecoverFunc)
        {
            if (canRecoverFunc == null) return;
            foreach (var i in _records)
            {
                try
                {
                    if (canRecoverFunc(i))
                    {
                        i.Recover();
                    }
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// 批量记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBatchRecorder<T, TRecord> : IBatchRecorder<T>
        where TRecord : class, ISingleRecord<T>, new()
    {
        /// <summary>
        /// 记录列表
        /// </summary>
        TRecord[] records { get; }

        /// <summary>
        /// 记录列表中第一条记录：如果无记录返回null
        /// </summary>
        TRecord firstRecod { get; }
    }

    /// <summary>
    /// 批量记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBatchRecorder<T> : IRecorder<T>, IRecord<IEnumerable<T>> { }

    /// <summary>
    /// 单一记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISingleRecord<T> : IRecord<T>, IRecover { }
}
