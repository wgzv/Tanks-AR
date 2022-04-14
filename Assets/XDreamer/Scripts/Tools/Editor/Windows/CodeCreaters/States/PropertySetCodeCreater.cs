using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Helper;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 属性设置代码生成器
    /// </summary>
    public class PropertySetCodeCreater : BasePropertyCodeCreater
    {
        /// <summary>
        /// 基础类型定义字符串
        /// </summary>
        protected override string baseTypeDefineString => "BasePropertySet<" + name + ">";

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = targetObjectNameAttribute + "属性设置";
                }
                return _title;
            }
            set => _title = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public override string defaultName => targetObjectTypeName + "PropertySet";

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="code"></param>
        protected override void OnBeginCreateCode(ICodeWirter code)
        {
            base.OnBeginCreateCode(code);

            AddUsedType(typeof(BasePropertySet<>), typeof(List<>));
        }

        /// <summary>
        /// 当目标对象定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnTargetObjectDefine(ICodeWirter codeWirter)
        {
            base.OnTargetObjectDefine(codeWirter);

            if (createtTargetObjectListField)
            {
                codeWirter.WriteSummaryFormat("{0}列表:{1}列表", targetObjectNameAttribute, targetObjectTipAttribute);
                codeWirter.WriteFormat("[Name(\"{0}列表\")]", targetObjectNameAttribute);
                codeWirter.WriteFormat("[Tip(\"{0}列表\")]", targetObjectTipAttribute);
                codeWirter.WriteFormat("public List<{0}> {1}s = new List<{0}>();", targetObjectTypeName, targetObjectFieldName);
                codeWirter.Write();
            }           
        }

        /// <summary>
        /// 绘制基础信息身体内容
        /// </summary>
        public override void OnDrawBaseInfoBodyContent()
        {
            base.OnDrawBaseInfoBodyContent();
            createtTargetObjectListField = EditorGUILayout.Toggle("创建目标对象列表字段", createtTargetObjectListField);
        }

        #region 字段

        class FieldInfoData_Set : FieldInfoData
        {
            public FieldInfoData_Set(FieldInfo fieldInfo) : base(fieldInfo) { }

            public override bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("var value = {0};", OnCreateCode_Call_GetValue(codeCreater, codeWirter));
                if (isStaticMemberInfo)//静态成员信息
                {
                    codeWirter.WriteFormat("{0}.{1} = value;",
                               codeCreater.targetObjectTypeName,
                               name);
                }
                else//非静态成员信息
                {
                    codeWirter.WriteFormat("if ({0} != null) {0}.{1} = value;",
                                 codeCreater.targetObjectFieldName,
                                 name);

                    if(codeCreater.createtTargetObjectListField)
                    {
                        codeWirter.WriteFormat("foreach(var obj in {0}s)", codeCreater.targetObjectFieldName);
                        codeWirter.Write("{");
                        codeWirter.IncreaseIndent();
                        codeWirter.WriteFormat("if (obj != null) obj.{0} = value;", name);
                        codeWirter.DecreaseIndent();
                        codeWirter.Write("}");
                    }                   
                }

                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_ToFriendlyString(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("return CommonFun.Name(_propertyName) + \" = \" + {0}.ToFriendlyString();", propertyFieldName);
                return false;
                //return base.OnCreateCode_Method_ToFriendlyString(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_DataValidity(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                if (codeCreater.hasTargetObjectInstance)
                {
                    codeWirter.WriteFormat("return {0} && {1}.DataValidity();", codeCreater.targetObjectFieldName, propertyFieldName);
                }
                else
                {
                    codeWirter.WriteFormat("return {0}.DataValidity();", propertyFieldName);
                }
                return false;
                //return base.OnCreateCode_Method_DataValidity(codeCreater, codeWirter);
            }

            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this)
                {
                    TrySetPropertyEnumNameValue(PropertyHelper.FieldFirstEnumValue);
                }
            }
        }

        List<FieldInfoData_Set> fieldInfoData_Sets = new List<FieldInfoData_Set>();

        /// <summary>
        /// 字段信息列表
        /// </summary>
        public override List<FieldInfoData> fieldInfoDatas => fieldInfoData_Sets.ToList(d => (FieldInfoData)d);

        /// <summary>
        /// 是否是有效的字段信息
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected override bool IsValidFieldInfo(FieldInfo fieldInfo)
        {
            return base.IsValidFieldInfo(fieldInfo) && !fieldInfo.IsConst();
        }

        /// <summary>
        /// 当查找属性时
        /// </summary>
        protected override void OnFindFields()
        {
            base.OnFindFields();
            if (targetObjectType == null) return;

            fieldInfoData_Sets.Clear();
            fieldInfoData_Sets.AddRange(targetObjectType.GetFields().Where(IsValidFieldInfo).Cast(fi =>
            {
                var data = new FieldInfoData_Set(fi);
                data.Init(this);
                return data;
            }));
            fieldInfoData_Sets.Sort((x, y) => string.Compare(x.name, y.name));
        }

        #endregion

        #region 属性

        class PropertyInfoData_Set : PropertyInfoData
        {
            public PropertyInfoData_Set(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override bool isStaticMemberInfo
            {
                get
                {
                    if (!_isStaticMemberInfo.HasValue)
                    {
                        _isStaticMemberInfo = propertyInfo.SetMethod is MethodInfo setMethodInfo && setMethodInfo.IsStatic;
                    }
                    return _isStaticMemberInfo.Value;
                }
            }

            public override bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("var value = {0};", OnCreateCode_Call_GetValue(codeCreater, codeWirter));
                if (isStaticMemberInfo)//静态成员信息
                {
                    codeWirter.WriteFormat("{0}.{1} = value;",
                               codeCreater.targetObjectTypeName,
                               name);
                }
                else//非静态成员信息
                {
                    codeWirter.WriteFormat("if ({0} != null) {0}.{1} = value;",
                                 codeCreater.targetObjectFieldName,
                                 name);

                    if (codeCreater.createtTargetObjectListField)
                    {
                        codeWirter.WriteFormat("foreach(var obj in {0}s)", codeCreater.targetObjectFieldName);
                        codeWirter.Write("{");
                        codeWirter.IncreaseIndent();
                        codeWirter.WriteFormat("if (obj != null) obj.{0} = value;", name);
                        codeWirter.DecreaseIndent();
                        codeWirter.Write("}");
                    }
                }

                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_ToFriendlyString(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                codeWirter.WriteFormat("return CommonFun.Name(_propertyName) + \" = \" + {0}.ToFriendlyString();", propertyFieldName);
                return false;
                //return base.OnCreateCode_Method_ToFriendlyString(codeCreater, codeWirter);
            }

            public override bool OnCreateCode_Method_DataValidity(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                if (codeCreater.hasTargetObjectInstance)
                {
                    codeWirter.WriteFormat("return {0} && {1}.DataValidity();", codeCreater.targetObjectFieldName, propertyFieldName);
                }
                else
                {
                    codeWirter.WriteFormat("return {0}.DataValidity();", propertyFieldName);
                }
                return false;
                //return base.OnCreateCode_Method_DataValidity(codeCreater, codeWirter);
            }
            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this) TrySetPropertyEnumNameValue(PropertyHelper.PropertyFirstEnumValue);
            }
        }

        List<PropertyInfoData_Set> propertyInfoData_Sets = new List<PropertyInfoData_Set>();

        /// <summary>
        /// 属性信息列表
        /// </summary>
        public override List<PropertyInfoData> propertyInfoDatas => propertyInfoData_Sets.ToList(d => (PropertyInfoData)d);

        /// <summary>
        /// 是否是有效的属性
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected override bool IsValidPropertyInfo(PropertyInfo propertyInfo)
        {
            return base.IsValidPropertyInfo(propertyInfo) && propertyInfo.SetMethod is MethodInfo setMethodInfo && setMethodInfo.IsPublic;
        }

        /// <summary>
        /// 当查找属性时
        /// </summary>
        protected override void OnFindProperties()
        {
            base.OnFindProperties();
            if (targetObjectType == null) return;

            propertyInfoData_Sets = targetObjectType.GetProperties().Where(IsValidPropertyInfo).Cast(mi =>
            {
                var data = new PropertyInfoData_Set(mi);
                data.Init(this);
                this.InitPropertyByField(data);
                return data;
            }).ToList();
            propertyInfoData_Sets.Sort((x, y) => string.Compare(x.name, y.name));
        }

        #endregion

        #region 方法

        class MethodInfoData_Set : MethodInfoData
        {
            public MethodInfoData_Set(MethodInfo methodInfo) : base(methodInfo) { }

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
                foreach (var p in parameterInfoDatas)
                {
                    codeWirter.WriteFormat("var {0} = {1};", p.name, p.OnCreateCode_Call_GetValue(codeCreater, codeWirter));
                }
                var paramString = parameterInfoDatas.ToString(p => p.name, ", ");

                if (isStaticMemberInfo)//静态成员信息
                {
                    codeWirter.WriteFormat("{0}.{1}({2});",
                              codeCreater.targetObjectTypeName,
                              methodName,
                              paramString);
                }
                else//非静态成员信息
                {
                    codeWirter.WriteFormat("if ({0} != null) {0}.{1}({2});",
                                 codeCreater.targetObjectFieldName,
                                 methodName,
                                 paramString);

                    if (codeCreater.createtTargetObjectListField)
                    {
                        codeWirter.WriteFormat("foreach(var obj in {0}s)", codeCreater.targetObjectFieldName);
                        codeWirter.Write("{");
                        codeWirter.IncreaseIndent();
                        codeWirter.WriteFormat("if (obj != null) obj.{0}({1});", methodName, paramString);
                        codeWirter.DecreaseIndent();
                        codeWirter.Write("}");
                    }
                }                
                
                return true;
                //return base.OnCreateCode_Method_Execute(codeCreater, codeWirter);
            }

            /// <summary>
            /// 当生成代码方法DataValidity
            /// </summary>
            /// <param name="codeCreater"></param>
            /// <param name="codeWirter"></param>
            /// <returns></returns>
            public override bool OnCreateCode_Method_DataValidity(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
            {
                switch (parameterInfoDatas.Count)
                {
                    case 0:
                        {
                            if (codeCreater.hasTargetObjectInstance)
                            {
                                codeWirter.WriteFormat("return {0};", codeCreater.targetObjectFieldName);
                                return false;
                            }
                            return true;
                        }
                    case 1:
                        {
                            if (codeCreater.hasTargetObjectInstance)
                            {
                                codeWirter.WriteFormat("return {0} && {1}.DataValidity();", codeCreater.targetObjectFieldName, parameterInfoDatas[0].propertyFieldName);
                            }
                            else
                            {
                                codeWirter.WriteFormat("return {0}.DataValidity();", parameterInfoDatas[0].propertyFieldName);
                            }
                            return false;
                        }
                    default:
                        {
                            if (codeCreater.hasTargetObjectInstance)
                            {
                                codeWirter.WriteFormat("if(!{0}) return false;", codeCreater.targetObjectFieldName);
                            }
                            foreach (var p in parameterInfoDatas)
                            {
                                codeWirter.WriteFormat("if(!{0}.DataValidity()) return false;", p.propertyFieldName);
                            }
                            return true;
                        }
                }
                //return base.OnCreateCode_Method_DataValidity(codeCreater, codeWirter);
            }

            public override void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last)
            {
                base.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                if (first == this) TrySetPropertyEnumNameValue(PropertyHelper.MethodFirstEnumValue);
            }
        }

        List<MethodInfoData_Set> methodInfoData_Sets = new List<MethodInfoData_Set>();

        /// <summary>
        /// 方法信息数据列表
        /// </summary>
        public override List<MethodInfoData> methodInfoDatas => methodInfoData_Sets.ToList(d => (MethodInfoData)d);

        /// <summary>
        /// 当查找方法时
        /// </summary>
        protected override void OnFindMethods()
        {
            base.OnFindMethods();
            if (targetObjectType == null) return;

            methodInfoData_Sets = targetObjectType.GetMethods().Where(mi => !mi.IsSpecialName && !mi.IsGenericMethod && !AttributeCache<ObsoleteAttribute>.Exist(mi)).Cast(mi =>
            {
                var data = new MethodInfoData_Set(mi);
                data.Init(this);
                return data;
            }).ToList();
            methodInfoData_Sets.Sort((x, y) => string.Compare(x.name, y.name));
        }

        #endregion
    }
}
