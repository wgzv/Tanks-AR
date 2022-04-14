using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;
using UnityEngine;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 抽象型字段绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FieldBindOfVirtual<T> : FieldBind<T> where T : FieldBindOfVirtual<T>
    {
        /// <summary>
        /// 能否绑定
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public override bool CanBind(FieldInfo fieldInfo)
        {
            switch (fieldInfo.Name)
            {
                case "_" + nameof(enable):
                case "_" + nameof(parent):
                case nameof(WorkClip.workRange):
                    {
                        return false;
                    }
            }
            return base.CanBind(fieldInfo);
        }
    }
}
