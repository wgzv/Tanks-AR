using System.Collections.Generic;
using UnityEditor;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorSMS;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.CNScripts;
using XCSJ.PluginRepairman.Devices;
using XCSJ.PluginRepairman.Exam;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.Study;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorRepairman.CNScripts
{
    /// <summary>
    /// 为中文脚本弹出选择事件菜单
    /// </summary>
    [CommonEditor(typeof(ForPartState))]
    public class OnGUIForPartStateName : StringScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            EditorGUI.indentLevel = 2;
            List<string> epartList = new List<string>();
            EnumHelper.Enums<EPartState>().ForEach(epartType =>
            {
                epartList.Add(CommonFun.Name(epartType));
            });
            paramObject = UICommonFun.Popup(paramObject as string, epartList.ToArray());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [CommonEditor(typeof(ForRepairmanTeaching))]
    public class ForRepairmanTeachingName : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            EditorGUI.indentLevel = 2;
            switch (paramTypeInt)
            {
                case ForRepairmanTeaching.RepairStudy:
                    {
                        paramObject = ForRepairmanName.PopupStateComponent<RepairStudy>(paramObject);
                        break;
                    }
                case ForRepairmanTeaching.RepairExam:
                    {
                        paramObject = ForRepairmanName.PopupStateComponent<RepairExam>(paramObject);
                        break;
                    }
            }
        }
    }

    [CommonEditor(typeof(ForRepairman))]
    public class ForRepairmanName : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制内容
        /// </summary>
        public override void OnDrawContent()
        {
            EditorGUI.indentLevel = 2;
            switch (paramTypeInt)
            {
                case ForRepairman.Device:
                    {
                        paramObject = PopupStateComponent<Device>(paramObject);
                        break;
                    }
            }
        }

        public static T PopupStateComponent<T>(object obj) where T : StateComponent
        {
            var objects = RootStateMachine.instance.GetStateComponents(typeof(T)).ToList(o => (T)o);
            return EditorSMSHelper.Popup<T>(objects, (T)obj, true);
        }
    }
}
