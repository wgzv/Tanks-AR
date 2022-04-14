using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    public class UnityEventBase_LinkType<T> : LinkType<T>
        where T : UnityEventBase_LinkType<T>
    {
        public UnityEventBase_LinkType(UnityEventBase obj) : base(obj) { }

        public UnityEventBase_LinkType(object obj) : base(obj) { }

        protected UnityEventBase_LinkType() { }

        #region AddCall

        public static XMethodInfo AddCall_MethodInfo { get; } = GetXMethodInfo(nameof(AddCall));

        public void AddCall(BaseInvokableCall call)
        {
            AddCall_MethodInfo?.Invoke(obj, new object[] { call.obj });
        }

        #endregion

        #region RemoveListener

        public static XMethodInfo RemoveListener_MethodInfo { get; } = GetXMethodInfo(nameof(RemoveListener), new Type[] { typeof(object), typeof(MethodInfo) });

        public void RemoveListener(object targetObj, MethodInfo method)
        {
            RemoveListener_MethodInfo?.Invoke(obj, new object[] { targetObj, method });
        }

        #endregion
    }

    [LinkType(typeof(UnityEventBase))]
    public class UnityEventBase_LinkType : UnityEventBase_LinkType<UnityEventBase_LinkType>
    {
        public UnityEventBase_LinkType(UnityEventBase obj) : base(obj) { }
        public UnityEventBase_LinkType(object obj) : base(obj) { }
        protected UnityEventBase_LinkType() { }
    }

    public static class UnityEventBaseExtension
    {
        public static void AddCall(this UnityEventBase unityEventBase, UnityAction call)
        {
            if (unityEventBase == null || call == null) return;
            AddCall(unityEventBase, UnityEvent_LinkType.GetDelegate(call));
        }

        public static void AddCall(this UnityEventBase unityEventBase, BaseInvokableCall call)
        {
            if (unityEventBase == null || call == null) return;
            new UnityEventBase_LinkType(unityEventBase).AddCall(call);
        }

        public static void RemoveCall(this UnityEventBase unityEventBase, UnityAction call)
        {
            if (unityEventBase == null || call == null) return;
            RemoveCall(unityEventBase, call.Target, call.Method);
        }

        public static void RemoveCall(this UnityEventBase unityEventBase, object targetObj, MethodInfo method)
        {
            new UnityEventBase_LinkType(unityEventBase).RemoveListener(targetObj, method);
        }
    }
}
