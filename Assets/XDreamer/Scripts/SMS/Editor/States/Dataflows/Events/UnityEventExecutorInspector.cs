using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Dataflows.Events;

namespace XCSJ.EditorSMS.States.Dataflows.Events
{
    /// <summary>
    /// Unity事件执行器检查器
    /// </summary>
    [CustomEditor(typeof(UnityEventExecutor), true)]
    public class UnityEventExecutorInspector : StateComponentInspector
    {
        private UnityEventDrawer _eventDrawer;
        private GUIContent _content;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            _eventDrawer = new UnityEventDrawer();
            _content = CommonFun.NameTip(typeof(UnityEventExecutor), nameof(UnityEventExecutor.unityEvent));
        }

        /// <summary>
        /// 是否需要绘制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        /// <returns></returns>
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(UnityEventExecutor.unityEvent):
                {
                    var rect = EditorGUILayout.GetControlRect(true, _eventDrawer.GetPropertyHeight(memberProperty, _content));
                    _eventDrawer.OnGUI(rect, memberProperty, _content);
                    return false;
                }
                   
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
