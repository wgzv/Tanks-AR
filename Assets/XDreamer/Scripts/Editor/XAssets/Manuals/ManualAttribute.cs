using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Manuals
{
    /// <summary>
    /// 手册特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ManualAttribute : LinkedTypeAttribute
    {
        public ManualAttribute(Type type, bool forChildClasses = false) : base(type, forChildClasses, nameof(ManualAttribute)) { }
    }
}
