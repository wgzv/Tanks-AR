using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.EditorCommonUtils;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 类型宏辅助类，可用于检测某个期望类型是否存在，并创建以XDREAMER_TYPE_为前缀连接对应全名称的编译宏；
    /// </summary>
    public class TypeMacroHelper : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>
        /// 编译宏前缀
        /// </summary>
        public const string CompileMacroPrefix = XDreamerEditor.CompileMacroPrefix + Macro.ConnectorString + nameof(TYPE);

        /// <summary>
        /// 类型宏对象
        /// </summary>
        public static readonly Macro TYPE = new Macro(CompileMacroPrefix, XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 初始化
        /// </summary>
        [Macro]
        public static void Init()
        {
            //Debug.Log("更新XDreamer类型宏");
            UpdateTypeMacro();
        }

        private static string TypeStringToMacro(string typeString)
        {
            return Macro.CreateDefineName(CompileMacroPrefix, Macro.TypeStringToDefineName(typeString, false));
        }

        private static void GetTypeMacro(out List<string> defines, out List<string> undefines)
        {
            var tmpDefines = new List<string>();
            var tmpUndefines = new List<string>();
            Types.Foreach(type =>
            {
                if (Caches.TypeCache.Get(type) != null)
                {
                    tmpDefines.Add(TypeStringToMacro(type));
                }
                else
                {
                    tmpUndefines.Add(TypeStringToMacro(type));
                }
            });
            defines = tmpDefines;
            undefines = tmpUndefines;
        }

        private static void UpdateTypeMacro()
        {
            try
            {
                GetTypeMacro(out List<string> defines, out List<string> undefines);

                if (undefines != null && undefines.Count > 0)
                {
                    Macro.RemoveMacro((group, name) => undefines.Contains(name), XDreamerEditor.DefaultBuildTargetGroups);
                }

                if (defines != null && defines.Count > 0)
                {
                    foreach (var macroString in defines)
                    {
                        var currentMacro = new Macro(macroString, XDreamerEditor.DefaultBuildTargetGroups);
                        currentMacro.DefineIfNoDefined();
                    }
                }
            }
            catch { }
        }

        private readonly static HashSet<string> Types = new HashSet<string>()
        {
            "UnityEngine.SpatialTracking.TrackedPoseDriver"
        };

        #region 编译事件

        /// <summary>
        /// 接口<see cref="IOrderedCallback"/>的实现
        /// </summary>
        public int callbackOrder => 0;

        private DateTime beginTime;

        /// <summary>
        /// 编译开始之前的回调事件，接口<see cref="IPreprocessBuildWithReport"/>的实现
        /// </summary>
        /// <param name="report"></param>
        public void OnPreprocessBuild(BuildReport report)
        {
            beginTime = DateTime.Now;
            Debug.Log("编译开始: " + beginTime.ToDefaultFormat());
        }

        /// <summary>
        /// 编译完成之后的回调事件，接口<see cref="IPostprocessBuildWithReport"/>的实现
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            var endTime = DateTime.Now;
            Debug.Log("编译结束: " + endTime.ToDefaultFormat());
            Debug.Log("编译总时长: " + (endTime - beginTime).TotalSeconds + " 秒");
            Debug.LogFormat("编译结束！编译结果: {0}, 总耗时 {1} 秒", report.summary.result, report.summary.totalTime.TotalSeconds);
        }

        #endregion
    }
}
