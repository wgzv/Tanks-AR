using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件简版枚举
    /// </summary>
    public enum EMonoBehaviourScriptEventLiteType
    {
        /// <summary>
        /// 启动时
        /// </summary>
        [Name("启动时执行")]
        Start = 0,

        /// <summary>
        /// 更新时
        /// </summary>
        [Name("更新时执行")]
        Update,
    }

    /// <summary>
    /// MonoBehaviour脚本事件简版集合类
    /// </summary>
    [Serializable]
    public class MonoBehaviourScriptEventLiteSet : BaseScriptEventSet<EMonoBehaviourScriptEventLiteType> { }

    /// <summary>
    /// MonoBehaviour脚本事件简版
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class MonoBehaviourScriptEventLite : BaseScriptEvent<EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteSet>, IMonoBehaviourLite
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "MonoBehaviour脚本事件简版";

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            RunScriptEvent(EMonoBehaviourScriptEventLiteType.Start);
        }

        /// <summary>
        /// 更新（Update is called once per frame)
        /// </summary>
        public override void Update()
        {
            RunScriptEvent(EMonoBehaviourScriptEventLiteType.Update);
        }
    }
}
