using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 批注基类
    /// </summary>
    public abstract class BaseLineNote : ToolMB
    {
        /// <summary>
        /// 被标注对象
        /// </summary>
        public INoted noted { get; }

        /// <summary>
        /// 线
        /// </summary>
        public ILine line { get; }

        /// <summary>
        /// 标注
        /// </summary>
        public INote note { get; }
    }

    /// <summary>
    /// 线接口
    /// </summary>
    public interface ILine
    {
        /// <summary>
        /// 起点
        /// </summary>
        ILinePoint begin { get; }

        /// <summary>
        /// 终点
        /// </summary>
        ILinePoint end { get; }

        /// <summary>
        /// 点列表
        /// </summary>
        List<ILinePoint> points { get; }
    }

    /// <summary>
    /// 点
    /// </summary>
    public interface ILinePoint
    {

    }

    /// <summary>
    /// 标注
    /// </summary>
    public interface INote
    {

    }

    /// <summary>
    /// 被标注对象
    /// </summary>
    public interface INoted
    {

    }
}
