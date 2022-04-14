using UnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginMMO.Transitions;

namespace XCSJ.EditorMMO.Transitions
{
    /// <summary>
    /// 网络属性比较检查器
    /// </summary>
    [CustomEditor(typeof(NetPropertyCompare), true)]
    public class NetPropertyCompareInspector : TransitionComponentInspector<NetPropertyCompare>
    {

    }
}
