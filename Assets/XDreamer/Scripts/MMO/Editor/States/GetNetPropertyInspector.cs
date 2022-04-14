using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginMMO.States;

namespace XCSJ.EditorMMO.States
{
    /// <summary>
    /// 获取网络属性检查器
    /// </summary>
    [CustomEditor(typeof(GetNetProperty), true)]
    public class GetNetPropertyInspector : StateComponentInspector<GetNetProperty>
    {

    }
}
