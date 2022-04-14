using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    public class BaseInvokableCall<T> : LinkType<T>
       where T : BaseInvokableCall<T>
    {
        public BaseInvokableCall(object obj) : base(obj) { }

        protected BaseInvokableCall() { }
    }

    [LinkType("")]
    public class BaseInvokableCall : BaseInvokableCall<BaseInvokableCall>
    {
        public BaseInvokableCall(object obj) : base(obj) { }
        protected BaseInvokableCall() { }
    }
}
