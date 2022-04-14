using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 基础属性代码生成器
    /// </summary>
    public class BasePropertyCodeCreater : BaseLifecycleExecutorCodeCreater
    {
        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            targetObjectType = null;
            propertyNameEnumCodeCreater?.Reset();
            enumMemberInfoDatas.Clear();
            propertyValueLinkTypeMap.Clear();
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = targetObjectNameAttribute;
                }
                return _title;
            }
            set => _title = value;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public override EIcon icon => EIcon.Property;

        /// <summary>
        /// 隐藏宏
        /// </summary>
        public string hideMacro { get; set; } = "!XDREAMER_EDITION_DEVELOPER";

        /// <summary>
        /// 使用<see cref="EnumPopupAttribute"/>特性类
        /// </summary>
        public bool useEnumPopupAttribute { get; set; } = false;

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginCreateCode(ICodeWirter codeWirter)
        {
            base.OnBeginCreateCode(codeWirter);

            propertyValueLinkTypeMap.Clear();

            AddUsedType(targetObjectType,
                typeof(HideInSuperInspectorAttribute),
                typeof(EValidityCheckType),
                typeof(BasePropertyValue),
                typeof(EnumFieldNameAttribute),
                typeof(UnityObjectExtension));

            enumMemberInfoDatas.Clear();
            enumMemberInfoDatas.AddRange(fieldInfoDatas);
            enumMemberInfoDatas.AddRange(propertyInfoDatas);
            enumMemberInfoDatas.AddRange(methodInfoDatas);

            //添加各种引用类
            foreach (var i in enumMemberInfoDatas)
            {
                i.OnBeginCreateCode(this, codeWirter);
            }

            propertyNameEnumCodeCreater.Init(this, enumMemberInfoDatas);
        }

        /// <summary>
        /// 当开始类型内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateTypeContent(ICodeWirter codeWirter)
        {
            OnTargetObjectDefine(codeWirter);

            OnCreate_Enum_EPropertyName(codeWirter);

            OnCreate_Field_PropertyValue(codeWirter);

            base.OnCreateTypeContent(codeWirter);
        }

        /// <summary>
        /// 当创建函数Execute内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_Execute(ICodeWirter codeWirter)
        {
            base.OnCreateFuncContent_Execute(codeWirter);
            OnSwitchCaseEPropertyNameEnum(codeWirter, data => data.OnCreateCode_Method_Execute(this, codeWirter));
        }

        /// <summary>
        /// 当创建函数ToFriendlyString内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_ToFriendlyString(ICodeWirter codeWirter)
        {
            OnSwitchCaseEPropertyNameEnum(codeWirter, data => data.OnCreateCode_Method_ToFriendlyString(this, codeWirter));
            base.OnCreateFuncContent_ToFriendlyString(codeWirter);
        }

        /// <summary>
        /// 当创建函数DataValidity内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_DataValidity(ICodeWirter codeWirter)
        {
            OnSwitchCaseEPropertyNameEnum(codeWirter, data => data.OnCreateCode_Method_DataValidity(this, codeWirter));
            base.OnCreateFuncContent_DataValidity(codeWirter);
        }

        /// <summary>
        /// 当创建函数Reset内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateFuncContent_Reset(ICodeWirter codeWirter)
        {
            base.OnCreateFuncContent_Reset(codeWirter);
            if(hasTargetObjectProperty) codeWirter.WriteFormat("if ({0}) {{ }}", targetObjectPropertyName);
        }

        /// <summary>
        /// 当处理属性名称枚举项
        /// </summary>
        /// <param name="codeWirter"></param>
        /// <param name="caseFunc">返回true，则添加break;返回false，无处理；</param>
        private void OnSwitchCaseEPropertyNameEnum(ICodeWirter codeWirter, Func<MemberInfoData, bool> caseFunc)
        {
            if (caseFunc == null) caseFunc = d => true;
            codeWirter.WriteFormat("switch (_propertyName)");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            {
                foreach (var data in enumMemberInfoDatas)
                {
                    if (!data.wantOutput) continue;

                    codeWirter.WriteFormat("case EPropertyName.{0}:", data.propertyEnumName);
                    codeWirter.IncreaseIndent();
                    codeWirter.Write("{");
                    codeWirter.IncreaseIndent();
                    {
                        if (caseFunc(data))
                        {
                            codeWirter.WriteFormat("break;");
                        }
                    }
                    codeWirter.DecreaseIndent();
                    codeWirter.Write("}");
                    codeWirter.DecreaseIndent();
                }
            }
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
        }

        /// <summary>
        /// 当查找成员
        /// </summary>
        protected virtual void OnFindMembers()
        {
            OnFindFields();
            OnFindProperties();
            OnFindMethods();
        }

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        public override void InitByCreateType(Type createType)
        {
            if (createType == null) return;
            base.InitByCreateType(createType);

            var targetObjectFieldInfo = FieldInfoCache.Get(createType, targetObjectFieldName);
            if (targetObjectFieldInfo != null)
            {
                targetObjectNameAttribute = CommonFun.Name(targetObjectFieldInfo);
                targetObjectTipAttribute = CommonFun.Tip(targetObjectFieldInfo);
            }

            var enumType = createType.GetNestedType("EPropertyName");
            if(enumType!=null)
            {
                foreach (var i in fieldInfoDatas) i.InitByCreateType(createType, enumType);
                foreach (var i in propertyInfoDatas) i.InitByCreateType(createType, enumType);
                foreach (var i in methodInfoDatas) i.InitByCreateType(createType, enumType);
            }
        }

        #region 界面绘制

        /// <summary>
        /// 绘制基础信息身体内容
        /// </summary>
        public override void OnDrawBaseInfoBodyContent()
        {
            hideMacro = EditorGUILayout.TextField("隐藏宏", hideMacro);
            useEnumPopupAttribute = EditorGUILayout.Toggle("使用枚举弹出特性", useEnumPopupAttribute);
            base.OnDrawBaseInfoBodyContent();
        }

        /// <summary>
        /// 重置命名空间
        /// </summary>
        public override void ResetNameSpace()
        {
            //base.ResetNameSpace();
            nameSpace = "";
            if (targetObjectType != null  && !string.IsNullOrEmpty(targetObjectType.Namespace))
            {
                nameSpace = targetObjectType.Namespace + ".States";
            }
            else
            {
                nameSpace = "XCSJ.PluginSMS.States.Others";
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            base.OnGUIWithScrollView();

            DrawFields();
            DrawProperties();
            DrawMethods();
        }

        #endregion

        #region 目标对象

        bool hasTargetObjectProperty = false;

        /// <summary>
        /// 创建目标对象列表字段
        /// </summary>
        public bool createtTargetObjectListField { get; set; } = true;

        /// <summary>
        /// 当目标对象定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnTargetObjectDefine(ICodeWirter codeWirter)
        {
            hasTargetObjectProperty = false;
            if (!hasTargetObjectInstance) return;

            codeWirter.WriteSummaryFormat("{0}:{1}", targetObjectNameAttribute, targetObjectTipAttribute);
            codeWirter.WriteFormat("[Name(\"{0}\")]", targetObjectNameAttribute);
            codeWirter.WriteFormat("[Tip(\"{0}\")]", targetObjectTipAttribute);
            codeWirter.WriteFormat("[ValidityCheck(EValidityCheckType.NotNull)]");
            var type = GetTargetObjectTypePopupAttribute();
            if (type != null)
            {
                codeWirter.WriteFormat("[{0}]", type.Name.Replace(nameof(Attribute), ""));
            }
            codeWirter.WriteFormat("public {0} {1};", targetObjectTypeName, targetObjectFieldName);
            codeWirter.Write();

            if (type == typeof(ComponentPopupAttribute))
            {
                hasTargetObjectProperty = true;
                codeWirter.WriteSummaryFormat("{0}:{1}", targetObjectNameAttribute, targetObjectTipAttribute);
                codeWirter.WriteFormat("public {0} {1} => this.XGetComponentInGlobal(ref {2}, true);", targetObjectTypeName, targetObjectPropertyName, targetObjectFieldName);
                codeWirter.Write();
            }
        }

        /// <summary>
        /// 目标对象的名称特性
        /// </summary>
        protected string _targetObjectNameAttribute = null;

        /// <summary>
        /// 目标对象的名称特性
        /// </summary>
        public string targetObjectNameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_targetObjectNameAttribute))
                {
                    _targetObjectNameAttribute = CommonFun.Name(targetObjectType);
                }
                return _targetObjectNameAttribute;
            }
            set => _targetObjectNameAttribute = value;
        }

        /// <summary>
        /// 目标对象的提示特性
        /// </summary>
        protected string _targetObjectTipAttribute = null;

        /// <summary>
        /// 目标对象的提示特性
        /// </summary>
        public string targetObjectTipAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_targetObjectTipAttribute))
                {
                    _targetObjectTipAttribute = CommonFun.Tip(targetObjectType);
                }
                return _targetObjectTipAttribute;
            }
            set => _targetObjectTipAttribute = value;
        }

        /// <summary>
        /// 目标对象类型名称
        /// </summary>
        public string targetObjectTypeName => targetObjectType?.Name ?? "";

        /// <summary>
        /// 目标对象属性名称
        /// </summary>
        public string targetObjectPropertyName => targetObjectTypeName;

        /// <summary>
        /// 目标对象字段名称
        /// </summary>
        public string targetObjectFieldName => "_" + targetObjectPropertyName;

        /// <summary>
        /// 有无目标对象实例
        /// </summary>
        public bool hasTargetObjectInstance
        {
            get
            {
                if (targetObjectType == null) return false;

                //如果不是Unity对象子级类型不可以存在实例
                if (!typeof(UnityEngine.Object).IsAssignableFrom(targetObjectType)) return false;

                //静态目标类型不可以存在实例
                if (TypeHelper.IsStatic(targetObjectType)) return false;

                return true;
            }
        }

        /// <summary>
        /// 重置目标对象类型信息
        /// </summary>
        /// <param name="targetObjectType"></param>
        public override void ResetTargetObjectTypeInfo(Type targetObjectType)
        {
            base.ResetTargetObjectTypeInfo(targetObjectType);
            OnFindMembers();
        }

        /// <summary>
        /// 有效目标对象类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool ValidTargetObjectType(Type type)
        {
            return base.ValidTargetObjectType(type) && type.IsPublic && !type.IsAbstract && !type.IsGenericType && typeof(Component).IsAssignableFrom(type);
        }

        #endregion

        #region 属性名称枚举-内部定义

        /// <summary>
        /// 枚举成员信息数据
        /// </summary>
        public List<MemberInfoData> enumMemberInfoDatas = new List<MemberInfoData>();

        /// <summary>
        /// 属性名称枚举代码生成器
        /// </summary>
        public PropertyNameEnumCodeCreater propertyNameEnumCodeCreater { get; } = new PropertyNameEnumCodeCreater();

        /// <summary>
        /// 属性名称枚举代码生成器
        /// </summary>
        public class PropertyNameEnumCodeCreater : BaseEnumCodeCreater
        {
            /// <summary>
            /// 代码生成器
            /// </summary>
            public BasePropertyCodeCreater codeCreater { get; private set; }

            /// <summary>
            /// 枚举成员信息数据
            /// </summary>
            public List<MemberInfoData> enumMemberInfoDatas { get; private set; }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="codeCreater"></param>
            /// <param name="enumMemberInfoDatas"></param>
            public void Init(BasePropertyCodeCreater codeCreater, List<MemberInfoData> enumMemberInfoDatas)
            {
                this.codeCreater = codeCreater;
                this.enumMemberInfoDatas = enumMemberInfoDatas;
                outputNameSpace = false;
                outputUsedNameSpaces = false;
            }

            /// <summary>
            /// 默认名称
            /// </summary>
            public override string defaultName => "EPropertyName";

            /// <summary>
            /// 当开始类型定义
            /// </summary>
            /// <param name="codeWirter"></param>
            protected override void OnBeginTypeDefine(ICodeWirter codeWirter)
            {
                codeWirter.WriteSummary("属性名称");
                codeWirter.WriteFormat("[Name(\"属性名称\")]");
                base.OnBeginTypeDefine(codeWirter);
            }

            /// <summary>
            /// 当创建类型内容
            /// </summary>
            /// <param name="codeWirter"></param>
            protected override void OnCreateTypeContent(ICodeWirter codeWirter)
            {
                base.OnCreateTypeContent(codeWirter);

                codeWirter.WriteSummary("无");
                codeWirter.WriteFormat("[Name(\"无\")]");
                codeWirter.WriteFormat("[Tip(\"无\")]");
                codeWirter.WriteFormat("[EnumFieldName(\"无\")]");
                codeWirter.WriteFormat("None,");
                codeWirter.Write();

                var type = default(Type);
                MemberInfoData first = default(MemberInfoData);
                MemberInfoData last = default(MemberInfoData);

                List<MemberInfoData> list = new List<MemberInfoData>();

                foreach (var i in enumMemberInfoDatas)
                {
                    if (!i.wantOutput) continue;

                    i.hasEnumRegion = false;
                    i.hasEnumEndRegion = false;

                    if (type != i.GetType())
                    {
                        type = i.GetType();
                        first = i;

                        first.hasEnumRegion = true;
                        if (last != null)
                        {
                            last.hasEnumEndRegion = true;
                        }
                    }
                    i.Before_OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter, first, last);
                    list.Add(i);
                    last = i;
                }
                if (last != null)
                {
                    last.hasEnumEndRegion = true;
                }

                foreach (var c in list)
                {
                    c.OnCreateCode_Enum_EPropertyName(codeCreater, codeWirter);
                }
            }
        }

        /// <summary>
        /// 当生成枚举属性名称
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreate_Enum_EPropertyName(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary("属性名称");
            codeWirter.WriteFormat("[Name(\"属性名称\")]");
            codeWirter.WriteMacro_if("UNITY_2019_3_OR_NEWER");
            if (useEnumPopupAttribute)
            {
                codeWirter.WriteFormat("[EnumPopup]");
            }
            else
            {
                codeWirter.WriteFormat("//[EnumPopup]");
            }
            codeWirter.WriteMacro_else();
            codeWirter.WriteFormat("[EnumPopup]");
            codeWirter.WriteMacro_endif();
            codeWirter.WriteFormat("public EPropertyName _propertyName = EPropertyName.None;");
            codeWirter.Write();

            propertyNameEnumCodeCreater.CreateCode(codeWirter);

            codeWirter.Write();
        }

        /// <summary>
        /// 当生成字段属性值
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreate_Field_PropertyValue(ICodeWirter codeWirter)
        {
            var type = default(Type);
            MemberInfoData first = default(MemberInfoData);
            MemberInfoData last = default(MemberInfoData);

            List<MemberInfoData> list = new List<MemberInfoData>();

            foreach (var i in enumMemberInfoDatas)
            {
                if (!i.wantOutput) continue;

                i.hasFieldRegion = false;
                i.hasFieldEndRegion = false;

                if (type != i.GetType())
                {
                    type = i.GetType();
                    first = i;

                    first.hasFieldRegion = true;
                    if (last != null)
                    {
                        last.hasFieldEndRegion = true;
                    }
                }
                list.Add(i);
                last = i;
            }
            if (last != null)
            {
                last.hasFieldEndRegion = true;
            }

            foreach (var i in list)
            {
                i.OnCreateCode_Field_PropertyValue(this, codeWirter);
            }
        }


        #endregion

        #region 字段       

        /// <summary>
        /// 显示字段信息数据列表
        /// </summary>
        [Name("字段信息")]
        public bool displayFieldInfos = true;

        /// <summary>
        /// 字段信息数据列表
        /// </summary>
        public virtual List<FieldInfoData> fieldInfoDatas { get; } = new List<FieldInfoData>();

        /// <summary>
        /// 待忽略字段列表
        /// </summary>
        public HashSet<string> ignoreFields { get; } = new HashSet<string>();

        /// <summary>
        /// 是否是有效的字段信息;即确定字段信息对象能否加入字段信息列表<see cref="fieldInfoDatas"/>；
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected virtual bool IsValidFieldInfo(FieldInfo fieldInfo)
        {
            return !fieldInfo.IsSpecialName && !AttributeCache<ObsoleteAttribute>.Exist(fieldInfo) && !ignoreFields.Contains(fieldInfo.Name);
        }

        /// <summary>
        /// 当查找字段时
        /// </summary>
        protected virtual void OnFindFields() { }

        private bool outputFields = true;
        private bool createFields_PropertyValueType = true;
        private int hideFields_PropertyName = 0;

        /// <summary>
        /// 当绘制字段数据列表头部
        /// </summary>
        public virtual void OnDrawFieldsHead()
        {
            displayFieldInfos = UICommonFun.Foldout(displayFieldInfos, CommonFun.NameTip(GetType(), nameof(displayFieldInfos)));
        }

        /// <summary>
        /// 当绘制字段数据列表身体
        /// </summary>
        public virtual void OnDrawFieldsBody()
        {
            if (!displayFieldInfos) return;
            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label("名称");

            GUILayout.Label("Name特性", UICommonOption.Width200);
            GUILayout.Label("Tip特性");

            GUILayout.Label("类型", UICommonOption.Width100);
            GUILayout.Label("属性值类型", UICommonOption.Width200);

            if (GUILayout.Button(CommonFun.TempContent("生成类型", "是否生成属性值类型；点击本标题可切换所有可生成属性值类型的生成状态；"), GUI.skin.label, UICommonOption.Width48))
            {
                createFields_PropertyValueType = !createFields_PropertyValueType;
                fieldInfoDatas.ForEach(d =>
                {
                    if (d.canCreatePropertyValueType)
                    {
                        d.createPropertyValueType = createFields_PropertyValueType;
                    }
                });
            }

            if(GUILayout.Button(CommonFun.TempContent("显示枚举", "确定生成属性名枚举中对应项是否添加非开发者隐藏的修饰特性，如显示则不添加，如不显示则添加编译宏与隐藏特性；点击本标题全选、全取消、仅基类之间切换；"), GUI.skin.label, UICommonOption.Width48))
            {
                hideFields_PropertyName = (hideFields_PropertyName + 1) % 3;
                switch(hideFields_PropertyName)
                {
                    case 0:
                        {
                            fieldInfoDatas.ForEach(d => d.displayPropertyEnumName = true);
                            break;
                        }
                    case 1:
                        {
                            fieldInfoDatas.ForEach(d => d.displayPropertyEnumName = false);
                            break;
                        }
                    case 2:
                        {
                            fieldInfoDatas.ForEach(d => d.displayPropertyEnumName = d.memberInfo.DeclaringType == targetObjectType);
                            break;
                        }
                }
            }

            GUILayout.Label("忽略", UICommonOption.Width32);
            if (GUILayout.Button(CommonFun.TempContent("输出", "是否期望输出本字段；点击本标题可切换所有的输出状态；"), GUI.skin.label, UICommonOption.Width32))
            {
                outputFields = !outputFields;
                fieldInfoDatas.ForEach(d => d.output = outputFields);
            }
            GUILayout.Label(CommonFun.TempContent("确定输出", "最终确定能否输出本字段；本项只读；"), UICommonOption.Width48);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < fieldInfoDatas.Count; i++)
            {
                var info = fieldInfoDatas[i];
                UICommonFun.BeginHorizontal(i);

                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
                EditorGUILayout.SelectableLabel(info.name, UICommonOption.Height18);

                EditorGUI.BeginDisabledGroup(!info.validPropertyType);
                info.nameAttribute = EditorGUILayout.TextField(info.nameAttribute, UICommonOption.Width200);
                info.tipAttribute = EditorGUILayout.TextField(info.tipAttribute);
                EditorGUI.EndDisabledGroup();

                var memberTypeName = info.memberType.Name;
                EditorGUILayout.LabelField(CommonFun.TempContent(memberTypeName, memberTypeName), UICommonOption.Width100);
                info.propertyValueTypeName = UICommonFun.Popup(info.propertyValueTypeName, info.propertyValueTypeNames, false, UICommonOption.Width200);

                EditorGUI.BeginDisabledGroup(!info.canCreatePropertyValueType);
                info.createPropertyValueType = EditorGUILayout.Toggle(info.createPropertyValueType, UICommonOption.Width48);
                EditorGUI.EndDisabledGroup();

                info.displayPropertyEnumName = EditorGUILayout.Toggle(info.displayPropertyEnumName, UICommonOption.Width48);
                var ignore = EditorGUILayout.Toggle(ignoreProperties.Contains(info.name), UICommonOption.Width32);
                info.output = EditorGUILayout.Toggle(info.output, UICommonOption.Width32) && !ignore;
                EditorGUILayout.Toggle(info.wantOutput, UICommonOption.Width48);

                UICommonFun.EndHorizontal();
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 绘制字段数据列表
        /// </summary>
        public virtual void DrawFields()
        {
            OnDrawFieldsHead();
            OnDrawFieldsBody();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 通过属性信息查找字段信息
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public FieldInfo GetFieldInfoByPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return default;

            //找到属性对应的字段信息
            var fieldInfo = fieldInfoDatas.FirstOrDefault(d => ("_" + propertyInfo.Name) == d.fieldInfo.Name || ("m_" + propertyInfo.Name) == d.fieldInfo.Name)?.fieldInfo;
            if (fieldInfo != null) return fieldInfo;

            //可能对应的字段非公有，无法直接调用
            fieldInfo = FieldInfoCache.Get(propertyInfo.ReflectedType, "_" + propertyInfo.Name, TypeHelper.Default, true);
            if (fieldInfo != null) return fieldInfo;

            return FieldInfoCache.Get(propertyInfo.ReflectedType, "m_" + propertyInfo.Name, TypeHelper.Default, true);
        }

        /// <summary>
        /// 通过字段初始化属性
        /// </summary>
        /// <param name="propertyInfoData"></param>
        public virtual void InitPropertyByField(PropertyInfoData propertyInfoData)
        {
            if (propertyInfoData == null) return;
            var propertyInfo = propertyInfoData.propertyInfo;

            //找到属性对应的字段信息
            var fieldInfo = GetFieldInfoByPropertyInfo(propertyInfo);
            if (fieldInfo == null) return;

            if (!AttributeCache<NameAttribute>.Exist(propertyInfo))
            {
                propertyInfoData.nameAttribute = propertyInfoData.GetNameAttributeText(fieldInfo);
            }
            if (!AttributeCache<TipAttribute>.Exist(propertyInfo))
            {
                if (AttributeCache<TipAttribute>.Exist(fieldInfo))
                {
                    propertyInfoData.tipAttribute = CommonFun.Tip(fieldInfo);
                }
                else
                {
                    propertyInfoData.tipAttribute = CommonFun.Name(fieldInfo);
                }
            }
        }

        /// <summary>
        /// 显示属性信息数据列表
        /// </summary>
        [Name("属性信息")]
        public bool displayPropertyInfos = true;

        /// <summary>
        /// 属性信息列表
        /// </summary>
        public virtual List<PropertyInfoData> propertyInfoDatas { get; } = new List<PropertyInfoData>();

        /// <summary>
        /// 待忽略属性列表
        /// </summary>
        public HashSet<string> ignoreProperties { get; } = new HashSet<string>();

        /// <summary>
        /// 是否是有效的属性信息;即确定属性信息对象能否加入属性信息列表<see cref="propertyInfoDatas"/>；
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected virtual bool IsValidPropertyInfo(PropertyInfo propertyInfo)
        {
            return !propertyInfo.IsSpecialName && !AttributeCache<ObsoleteAttribute>.Exist(propertyInfo) && !ignoreProperties.Contains(propertyInfo.Name);
        }

        /// <summary>
        /// 当查找属性时
        /// </summary>
        protected virtual void OnFindProperties() { ignoreProperties.Add(nameof(MonoBehaviour.runInEditMode)); }

        private bool outputProperties = true;
        private bool createProperties_PropertyValueType = true;
        private int hideProperties_PropertyName = 0;

        /// <summary>
        /// 当绘制属性数据列表头部
        /// </summary>
        public virtual void OnDrawPropertiesHead()
        {
            displayPropertyInfos = UICommonFun.Foldout(displayPropertyInfos, CommonFun.NameTip(GetType(), nameof(displayPropertyInfos)));
        }

        /// <summary>
        /// 当绘制属性数据列表身体
        /// </summary>
        public virtual void OnDrawPropertiesBody()
        {
            if (!displayPropertyInfos) return;
            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label("名称");

            GUILayout.Label("Name特性", UICommonOption.Width200);
            GUILayout.Label("Tip特性");

            GUILayout.Label("类型", UICommonOption.Width100);
            GUILayout.Label("属性值类型", UICommonOption.Width200);

            if (GUILayout.Button(CommonFun.TempContent("生成类型", "是否生成属性值类型；点击本标题可切换所有可生成属性值类型的生成状态；"), GUI.skin.label, UICommonOption.Width48))
            {
                createProperties_PropertyValueType = !createProperties_PropertyValueType;
                propertyInfoDatas.ForEach(d =>
                {
                    if (d.canCreatePropertyValueType)
                    {
                        d.createPropertyValueType = createProperties_PropertyValueType;
                    }
                });
            }

            if (GUILayout.Button(CommonFun.TempContent("显示枚举", "确定生成属性名枚举中对应项是否添加非开发者隐藏的修饰特性，如显示则不添加，如不显示则添加编译宏与隐藏特性；点击本标题全选、全取消、仅基类之间切换；"), GUI.skin.label, UICommonOption.Width48))
            {
                hideProperties_PropertyName = (hideProperties_PropertyName + 1) % 3;
                switch (hideProperties_PropertyName)
                {
                    case 0:
                        {
                            propertyInfoDatas.ForEach(d => d.displayPropertyEnumName = true);
                            break;
                        }
                    case 1:
                        {
                            propertyInfoDatas.ForEach(d => d.displayPropertyEnumName = false);
                            break;
                        }
                    case 2:
                        {
                            propertyInfoDatas.ForEach(d => d.displayPropertyEnumName = d.memberInfo.DeclaringType == targetObjectType);
                            break;
                        }
                }
            }

            GUILayout.Label("忽略", UICommonOption.Width32);
            if (GUILayout.Button(CommonFun.TempContent("输出", "是否期望输出本属性；点击本标题可切换所有的输出状态；"), GUI.skin.label, UICommonOption.Width32))
            {
                outputProperties = !outputProperties;
                propertyInfoDatas.ForEach(d => d.output = outputProperties);
            }
            GUILayout.Label(CommonFun.TempContent("确定输出", "最终确定能否输出本属性；本项只读；"), UICommonOption.Width48);

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < propertyInfoDatas.Count; i++)
            {
                var info = propertyInfoDatas[i];
                UICommonFun.BeginHorizontal(i);

                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
                EditorGUILayout.SelectableLabel(info.name, UICommonOption.Height18);

                EditorGUI.BeginDisabledGroup(!info.validPropertyType);
                info.nameAttribute = EditorGUILayout.TextField(info.nameAttribute, UICommonOption.Width200);
                info.tipAttribute = EditorGUILayout.TextField(info.tipAttribute);
                EditorGUI.EndDisabledGroup();

                var memberTypeName = info.memberType.Name;
                EditorGUILayout.LabelField(CommonFun.TempContent(memberTypeName, memberTypeName), UICommonOption.Width100);
                info.propertyValueTypeName = UICommonFun.Popup(info.propertyValueTypeName, info.propertyValueTypeNames, false, UICommonOption.Width200);

                EditorGUI.BeginDisabledGroup(!info.canCreatePropertyValueType);
                info.createPropertyValueType = EditorGUILayout.Toggle(info.createPropertyValueType, UICommonOption.Width48);
                EditorGUI.EndDisabledGroup();

                info.displayPropertyEnumName = EditorGUILayout.Toggle(info.displayPropertyEnumName, UICommonOption.Width48);
                var ignore = EditorGUILayout.Toggle(ignoreProperties.Contains(info.name), UICommonOption.Width32);
                info.output = EditorGUILayout.Toggle(info.output, UICommonOption.Width32) && !ignore;
                EditorGUILayout.Toggle(info.wantOutput, UICommonOption.Width48);

                UICommonFun.EndHorizontal();
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 绘制属性数据列表
        /// </summary>
        public virtual void DrawProperties()
        {
            OnDrawPropertiesHead();
            OnDrawPropertiesBody();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 显示方法信息数据列表
        /// </summary>
        [Name("方法信息")]
        public bool displayMethodInfos = true;

        /// <summary>
        /// 方法信息数据列表
        /// </summary>
        public virtual List<MethodInfoData> methodInfoDatas { get; } = new List<MethodInfoData>();

        private bool outputMethods = true;
        private bool createMethods_PropertyValueType = true;
        private int hideMethods_PropertyName = 0;

        /// <summary>
        /// 待忽略方法列表
        /// </summary>
        public HashSet<string> ignoreMethods { get; } = new HashSet<string>();

        /// <summary>
        /// 是否是有效的方法信息;即确定方法信息对象能否加入方法信息列表<see cref="methodInfoDatas"/>；
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        protected virtual bool IsValidMethodInfo(MethodInfo  methodInfo)
        {
            return !methodInfo.IsSpecialName && !methodInfo.IsGenericMethod && !AttributeCache<ObsoleteAttribute>.Exist(methodInfo) && !ignoreMethods.Contains(methodInfo.Name);
        }

        /// <summary>
        /// 当查找方法时
        /// </summary>
        protected virtual void OnFindMethods()
        {
            ignoreMethods.Add(nameof(IMonoBehaviour.Awake));
            ignoreMethods.Add(nameof(IMonoBehaviour.Start));
            ignoreMethods.Add(nameof(IMonoBehaviour.Update));
            ignoreMethods.Add(nameof(IMonoBehaviour.OnDestroy));
            ignoreMethods.Add(nameof(IOnEnable.OnEnable));
            ignoreMethods.Add(nameof(IOnDisable.OnDisable));
            ignoreMethods.Add(nameof(IOnGUI.OnGUI));
            ignoreMethods.Add(nameof(IGizmos.OnDrawGizmos));
            ignoreMethods.Add(nameof(IGizmos.OnDrawGizmosSelected));
            ignoreMethods.Add(nameof(MB.EditInspectorScript));
            ignoreMethods.Add("OnValidate");
            ignoreMethods.Add("FixedUpdate");
            ignoreMethods.Add("LateUpdate");
            ignoreMethods.Add("OnWillRenderObject");
            ignoreMethods.Add("OnPreCull");
            ignoreMethods.Add("OnBecameVisible");
            ignoreMethods.Add("OnBecameInvisible");
            ignoreMethods.Add("OnPreRender");
            ignoreMethods.Add("OnRenderObject");
            ignoreMethods.Add("OnPostRender");
            ignoreMethods.Add("OnRenderImage");
            ignoreMethods.Add("OnApplicationPause");
            ignoreMethods.Add("OnApplicationQuit");
            ignoreMethods.Add("OnTriggerEnter");
            ignoreMethods.Add("OnTriggerStay");
            ignoreMethods.Add("OnTriggerExit");
            ignoreMethods.Add("OnCollisionEnter");
            ignoreMethods.Add("OnCollisionStay");
            ignoreMethods.Add("OnCollisionExit");
        }

        /// <summary>
        /// 方法信息数据列表头部
        /// </summary>
        public virtual void DrawMethodsHead()
        {
            displayMethodInfos = UICommonFun.Foldout(displayMethodInfos, CommonFun.NameTip(GetType(), nameof(displayMethodInfos)));
        }

        /// <summary>
        /// 方法信息数据列表身体
        /// </summary>
        public virtual void DrawMethodsBody()
        {
            if (!displayMethodInfos) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label("名称");

            GUILayout.Label("Name特性", UICommonOption.Width200);
            GUILayout.Label("Tip特性");

            GUILayout.Label("类型", UICommonOption.Width100);
            GUILayout.Label("属性值类型", UICommonOption.Width200);

            if (GUILayout.Button(CommonFun.TempContent("生成类型", "是否生成属性值类型；点击本标题可切换所有可生成属性值类型的生成状态；"), GUI.skin.label, UICommonOption.Width48))
            {
                createMethods_PropertyValueType = !createMethods_PropertyValueType;
                methodInfoDatas.ForEach(d =>
                {
                    if (d.canCreatePropertyValueType)
                    {
                        d.createPropertyValueType = createMethods_PropertyValueType;
                    }
                    foreach(var p in d.parameterInfoDatas)
                    {
                        if (p.canCreatePropertyValueType)
                        {
                            p.createPropertyValueType = createMethods_PropertyValueType;
                        }
                    }
                });
            }

            if (GUILayout.Button(CommonFun.TempContent("显示枚举", "确定生成属性名枚举中对应项是否添加非开发者隐藏的修饰特性，如显示则不添加，如不显示则添加编译宏与隐藏特性；点击本标题全选、全取消、仅基类之间切换；"), GUI.skin.label, UICommonOption.Width48))
            {
                hideMethods_PropertyName = (hideMethods_PropertyName + 1) % 3;
                switch (hideMethods_PropertyName)
                {
                    case 0:
                        {
                            methodInfoDatas.ForEach(d => d.displayPropertyEnumName = true);
                            break;
                        }
                    case 1:
                        {
                            methodInfoDatas.ForEach(d => d.displayPropertyEnumName = false);
                            break;
                        }
                    case 2:
                        {
                            methodInfoDatas.ForEach(d => d.displayPropertyEnumName = d.memberInfo.DeclaringType == targetObjectType);
                            break;
                        }
                }
            }

            GUILayout.Label("忽略", UICommonOption.Width32);
            if (GUILayout.Button(CommonFun.TempContent("输出", "是否期望输出本方法；点击本标题可切换所有的输出状态；"), GUI.skin.label, UICommonOption.Width32))
            {
                outputMethods = !outputMethods;
                methodInfoDatas.ForEach(d => d.output = outputMethods);
            }
            GUILayout.Label(CommonFun.TempContent("确定输出", "最终确定能否输出本方法；本项只读；"), UICommonOption.Width48);

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < methodInfoDatas.Count; i++)
            {
                var info = methodInfoDatas[i];
                UICommonFun.BeginHorizontal(i);

                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
                EditorGUILayout.SelectableLabel(info.name, UICommonOption.Height18);

                //EditorGUI.BeginDisabledGroup(!info.validPropertyType);
                info.nameAttribute = EditorGUILayout.TextField(info.nameAttribute, UICommonOption.Width200);
                info.tipAttribute = EditorGUILayout.TextField(info.tipAttribute);
                //EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField(info.memberType.Name, UICommonOption.Width100);
                info.propertyValueTypeName = UICommonFun.Popup(info.propertyValueTypeName, info.propertyValueTypeNames, false, UICommonOption.Width200);

                EditorGUI.BeginDisabledGroup(!info.canCreatePropertyValueType);
                info.createPropertyValueType = EditorGUILayout.Toggle(info.createPropertyValueType, UICommonOption.Width48);
                EditorGUI.EndDisabledGroup();

                info.displayPropertyEnumName = EditorGUILayout.Toggle(info.displayPropertyEnumName, UICommonOption.Width48);
                var ignore = EditorGUILayout.Toggle(ignoreMethods.Contains(info.name), UICommonOption.Width32);
                info.output = EditorGUILayout.Toggle(info.output, UICommonOption.Width32) && !ignore;
                EditorGUILayout.Toggle(info.wantOutput, UICommonOption.Width48);

                UICommonFun.EndHorizontal();

                if (!info.output) continue;

                for (int j = 0; j < info.parameterInfoDatas.Count; j++)
                {
                    var param = info.parameterInfoDatas[j];

                    UICommonFun.BeginHorizontal(i);
                    EditorGUILayout.LabelField("", UICommonOption.Width32);
                    EditorGUILayout.SelectableLabel((j + 1).ToString() + "." + param.parameterInfo.Name, UICommonOption.Height18);

                    EditorGUI.BeginDisabledGroup(!param.validPropertyType);
                    param.nameAttribute = EditorGUILayout.TextField(param.nameAttribute, UICommonOption.Width200);
                    param.tipAttribute = EditorGUILayout.TextField(param.tipAttribute);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.LabelField(param.parameterInfo.ParameterType.Name, UICommonOption.Width100);
                    param.propertyValueTypeName = UICommonFun.Popup(param.propertyValueTypeName, param.propertyValueTypeNames, false, UICommonOption.Width200);

                    EditorGUI.BeginDisabledGroup(!param.canCreatePropertyValueType);
                    param.createPropertyValueType = EditorGUILayout.Toggle(param.createPropertyValueType, UICommonOption.Width48);
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.LabelField("", UICommonOption.Width48);
                    EditorGUILayout.LabelField("", UICommonOption.Width32);
                    EditorGUILayout.LabelField("", UICommonOption.Width32);
                    EditorGUILayout.LabelField("", UICommonOption.Width48);

                    UICommonFun.EndHorizontal();
                }
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 方法信息数据列表
        /// </summary>
        public virtual void DrawMethods()
        {
            DrawMethodsHead();
            DrawMethodsBody();
        }

        #endregion

        #region 属性值类-内部定义

        /// <summary>
        /// 属性值关联类型与代码生成器映射:仅存储将要内部定义的属性值类型
        /// </summary>
        public Dictionary<Type, BaseCodeCreater> propertyValueLinkTypeMap = new Dictionary<Type, BaseCodeCreater>();

        /// <summary>
        /// 通过查找或新建属性值关联类型对应的代码生成器
        /// </summary>
        /// <param name="propertyValueData"></param>
        /// <param name="isNew"></param>
        /// <returns></returns>
        public BaseCodeCreater FindOrNewCodeCreater(BasePropertyValueData propertyValueData, out bool isNew)
        {
            isNew = false;
            if (propertyValueData == null || !propertyValueData.needCreatePropertyValueType) return default;

            var type = propertyValueData.memberType;
            if (type == null) return default;
            if (propertyValueLinkTypeMap.TryGetValue(type, out var codeCreater)) return codeCreater;

            if (type.IsEnum)
            {
                propertyValueLinkTypeMap[type] = codeCreater = new EnumPropertyValueCodeCreater(type) { outputNameSpace = false, outputUsedNameSpaces = false };
                isNew = true;
                return codeCreater;
            }

            return default;
        }

        #endregion
    }
}
