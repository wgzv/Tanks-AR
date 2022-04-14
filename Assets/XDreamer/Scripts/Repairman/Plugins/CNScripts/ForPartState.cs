using XCSJ.Helper;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginRepairman.CNScripts
{
    /// <summary>
    /// 用于零件状态
    /// </summary>
    public class ForPartState : StringScriptParam
    {
        /// <summary>
        /// 脚本零件状态类型
        /// </summary>
        public const int ScriptPartStateType = IDRange.Begin;

        /// <summary>
        /// 获取扩展类型
        /// </summary>
        /// <returns></returns>
        public override int GetParamType() => ScriptPartStateType;

        /// <summary>
        /// 字符创转真实对象
        /// </summary>
        /// <param name="exScriptParamType"></param>
        /// <param name="scriptParamString"></param>
        /// <returns></returns>
        public override object StringToRealObject(int exScriptParamType, string scriptParamString)
        {
            return EnumHelper.StringToEnum<EPartState>(scriptParamString, EPartState.None);
        }
    }

    public class ForRepairman : ScriptParam
    {
        public const int Device = IDRange.Begin + 10;

        public override object DefaultObject(int exParamType, object defaultObject)
        {
            if (defaultObject is string)
            {
                string s = (string)defaultObject;
                return StringToRealObject(exParamType, s);
            }
            return null;
        }

        public override int[] GetParamTypes()
        {
            return new int[] { Device };
        }

        public override string ObjectToString(int exParamType, object tmpObject)
        {
            if (tmpObject is IVirtual)
            {
                IVirtual vir = (IVirtual)tmpObject;
                return vir.GetNamePath();
            }
            return "";
        }

        public override object StringToObject(int exParamType, string paramString)
        {
            return StringToRealObject(exParamType, paramString);
        }

        public override bool CheckRealObject(int exParamType, string paramString, object realObject)
        {

            if (realObject is IVirtual)
            {
                IVirtual vir = (IVirtual)realObject;
                return vir.GetNamePath() == paramString;
            }
            return false;
        }

        public override object StringToRealObject(int exParamType, string paramString)
        {
            switch (exParamType)
            {
                case Device: return SMSHelper.StringToStateComponent(paramString);
            }
            return null;
        }
    }

    public class ForRepairmanTeaching : ScriptParam
    {
        public const int RepairStudy = IDRange.Begin + 100;
        public const int RepairExam = IDRange.Begin + 101;

        public override object DefaultObject(int exParamType, object defaultObject)
        {
            if (defaultObject is string)
            {
                string s = (string)defaultObject;
                return StringToRealObject(exParamType, s);
            }
            return null;
        }

        public override int[] GetParamTypes()
        {
            return new int[] { RepairStudy, RepairExam };
        }

        public override string ObjectToString(int exParamType, object tmpObject)
        {
            if (tmpObject is IVirtual)
            {
                IVirtual vir = (IVirtual)tmpObject;
                return vir.GetNamePath();
            }
            return "";
        }

        public override object StringToObject(int exParamType, string paramString)
        {
            return StringToRealObject(exParamType, paramString);
        }

        public override bool CheckRealObject(int exParamType, string paramString, object realObject)
        {

            if (realObject is IVirtual)
            {
                IVirtual vir = (IVirtual)realObject;
                return vir.GetNamePath() == paramString;
            }
            return false;
        }

        public override object StringToRealObject(int exParamType, string paramString)
        {
            switch (exParamType)
            {
                case RepairStudy:
                case RepairExam:
                    {
                        return SMSHelper.StringToStateComponent(paramString);
                    }
            }
            return null;
        }
    }
}
