using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 属性获取代码生成器
    /// </summary>
    public class PropertyGetCodeCreater : BasePropertyCodeCreater
    {
        /// <summary>
        /// 基础类型定义字符串
        /// </summary>
        protected override string baseTypeDefineString => "BasePropertyGet<" + name + ">";

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = targetObjectNameAttribute + "属性获取";
                }
                return _title;
            }
            set => _title = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public override string defaultName => targetObjectTypeName + "PropertyGet";

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="code"></param>
        protected override void OnBeginCreateCode(ICodeWirter code)
        {
            base.OnBeginCreateCode(code);

            AddUsedType(typeof(BasePropertyGet<>), typeof(ScriptOption), typeof(Converter));
        }

        /// <summary>
        /// 当生成字段属性值
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreate_Field_PropertyValue(ICodeWirter codeWirter)
        {
            base.OnCreate_Field_PropertyValue(codeWirter);

            //定义存储变量
            codeWirter.WriteSummary("变量名:将获取到的属性值存储在变量名对应的全局变量中");
            codeWirter.Write("[Name(\"变量名\")]");
            codeWirter.Write("[Tip(\"将获取到的属性值存储在变量名对应的全局变量中\")]");
            codeWirter.Write("[ValidityCheck(EValidityCheckType.NotNullOrEmpty)]");
            codeWirter.Write("[GlobalVariable(true)]");
            codeWirter.Write("public string _variableName;");
            codeWirter.Write();

            //定义将值设置到变量的方法
            codeWirter.WriteSummary("将值设置到变量");
            codeWirter.WriteSummaryParam("value", "值");
            codeWirter.Write("protected void SetToVariable(object value)");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            codeWirter.Write("if (Converter.instance.TryConvertTo<string>(value, out string output)) ScriptManager.TrySetGlobalVariableValue(_variableName, output);");
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数ToFriendlyString内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_ToFriendlyString(ICodeWirter codeWirter)
        {
            codeWirter.Write("return ScriptOption.VarFlag + _variableName + \" = \" + CommonFun.Name(_propertyName);");
            //base.OnCreateFuncContent_ToFriendlyString(codeWirter);
        }

        /// <summary>
        /// 当创建函数DataValidity内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_DataValidity(ICodeWirter codeWirter)
        {
            codeWirter.Write("return base.DataValidity();");
            //base.OnCreateFuncContent_DataValidity(codeWirter);
        }

        #region 字段

        class FieldInfoData_Get : FieldInfoData
        {
            public FieldInfoData_Get(FieldInfo fieldInfo) : base(fieldInfo) { }

            public override void OnCreateCode_Field_PropertyValue(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                //base.OnCreateCode_Field_PropertyValue(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("SetToVariable({0}.{1});",
                        isStaticMemberInfo ? codeCreater.targetObjectTypeName : codeCreater.targetObjectFieldName,
                        name);
                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_ToFriendlyString(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("return CommonFun.Name(_propertyName) + \" = \" + {0}.ToFriendlyString();", propertyFieldName);
                return false;
                //return base.OnCreateCode_Method_ToFriendlyString(codeCreater, codeWirter);
            }

            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this) TrySetPropertyEnumNameValue(PropertyHelper.FieldFirstEnumValue);
            }
        }

        List<FieldInfoData_Get> fieldInfoData_Gets = new List<FieldInfoData_Get>();

        /// <summary>
        /// 字段信息列表
        /// </summary>
        public override List<FieldInfoData> fieldInfoDatas => fieldInfoData_Gets.ToList(d => (FieldInfoData)d);

        /// <summary>
        /// 当查找属性时
        /// </summary>
        protected override void OnFindFields()
        {
            base.OnFindFields();
            if (targetObjectType == null) return;

            fieldInfoData_Gets.Clear();
            fieldInfoData_Gets.AddRange(targetObjectType.GetFields().Where(IsValidFieldInfo).Cast(fi =>
            {
                var data = new FieldInfoData_Get(fi);
                data.Init(this);
                return data;
            }));
            fieldInfoData_Gets.Sort((x, y) => string.Compare(x.name, y.name));
        }

        #endregion

        #region 属性

        class PropertyInfoData_Get : PropertyInfoData
        {
            public PropertyInfoData_Get(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override bool isStaticMemberInfo
            {
                get
                {
                    if (!_isStaticMemberInfo.HasValue)
                    {
                        _isStaticMemberInfo = propertyInfo.GetMethod is MethodInfo getMethodInfo && getMethodInfo.IsStatic;
                    }
                    return _isStaticMemberInfo.Value;
                }
            }

            public override void OnCreateCode_Field_PropertyValue(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                //base.OnCreateCode_Field_PropertyValue(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("SetToVariable({0}.{1});",
                        isStaticMemberInfo ? codeCreater.targetObjectTypeName : codeCreater.targetObjectFieldName,
                        name);
                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this) TrySetPropertyEnumNameValue(PropertyHelper.PropertyFirstEnumValue);
            }
        }


        List<PropertyInfoData_Get> propertyInfoData_Gets = new List<PropertyInfoData_Get>();

        /// <summary>
        /// 属性信息列表
        /// </summary>
        public override List<PropertyInfoData> propertyInfoDatas => propertyInfoData_Gets.ToList(d => (PropertyInfoData)d);

        /// <summary>
        /// 是否是有效的属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected override bool IsValidPropertyInfo(PropertyInfo propertyInfo)
        {
            return base.IsValidPropertyInfo(propertyInfo) && propertyInfo.GetMethod is MethodInfo getMethod && getMethod.IsPublic;
        }

        /// <summary>
        /// 当查找属性时
        /// </summary>
        protected override void OnFindProperties()
        {
            base.OnFindProperties();
            if (targetObjectType == null) return;

            propertyInfoData_Gets = targetObjectType.GetProperties().Where(mi => IsValidPropertyInfo(mi)).Cast(mi =>
            {
                var data = new PropertyInfoData_Get(mi);
                data.Init(this);
                this.InitPropertyByField(data);
                return data;
            }).ToList();
            propertyInfoDatas.Sort((x, y) => string.Compare(x.name, y.name)); 
        }

        #endregion

        #region 方法

        class MethodInfoData_Get : MethodInfoData
        {
            public MethodInfoData_Get(MethodInfo methodInfo) : base(methodInfo) { }

            /// <summary>
            /// 当生成代码字段属性值
            /// </summary>
            /// <param name="codeCreater"></param>
            /// <param name="codeWirter"></param>
            public override void OnCreateCode_Field_PropertyValue(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                WriteFieldRegion(codeWirter);

                foreach (var p in parameterInfoDatas)
                {
                    p.OnCreateCode_Field_PropertyValue(codeCreater, codeWirter);
                }

                WriteFieldEndRegion(codeWirter);
                //base.CreateCode_Field_PropertyValue(codeCreater, codeWirter);
            }

            /// <summary>
            /// 当生成代码方法Execute
            /// </summary>
            /// <param name="codeCreater"></param>
            /// <param name="codeWirter"></param>
            /// <returns></returns>
            public override bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                var paramString = parameterInfoDatas.ToString(p => p.OnCreateCode_Call_GetValue(codeCreater, codeWirter), ", ");
                codeWirter.WriteFormat("SetToVariable({0}.{1}({2}));",
                        isStaticMemberInfo ? codeCreater.targetObjectTypeName : codeCreater.targetObjectFieldName,
                        methodName,
                        paramString);
                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this) TrySetPropertyEnumNameValue(PropertyHelper.MethodFirstEnumValue);
            }
        }

        List<MethodInfoData_Get> methodInfoData_Gets = new List<MethodInfoData_Get>();

        /// <summary>
        /// 方法信息数据列表
        /// </summary>
        public override List<MethodInfoData> methodInfoDatas => methodInfoData_Gets.ToList(d => (MethodInfoData)d);

        /// <summary>
        /// 当查找方法时
        /// </summary>
        protected override void OnFindMethods()
        {
            base.OnFindMethods();
            if (targetObjectType == null) return;

            methodInfoData_Gets = targetObjectType.GetMethods().Where(mi => !mi.IsSpecialName && !mi.IsGenericMethod && mi.ReturnType != typeof(void) && !AttributeCache<ObsoleteAttribute>.Exist(mi)).Cast(mi =>
            {
                var data = new MethodInfoData_Get(mi);
                data.Init(this);
                return data;
            }).ToList();
            methodInfoData_Gets.Sort((x, y) => string.Compare(x.name, y.name));
        }

        #endregion
    }
}
