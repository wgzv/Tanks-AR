using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 基础状态组件代码生成器
    /// </summary>
    public abstract class BaseStateComponentCodeCreater : BaseCodeCreater
    {
        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            category = "其它";
            _title = "";
            ClearManagerDatas();
            InitManagerDatas();
        }

        /// <summary>
        /// 基础类型定义字符串
        /// </summary>
        protected override string baseTypeDefineString => "StateComponent<" + name + ">";

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        public override void InitByCreateType(Type createType)
        {
            if (createType == null) return;
            base.InitByCreateType(createType);
            if (AttributeCache<ComponentMenuAttribute>.Get(createType) is ComponentMenuAttribute attribute)
            {
                var index = attribute.itemName.IndexOf("/");
                if (index > 0)
                {
                    category = attribute.itemName.Substring(0, index);
                }
                attribute.managerTypes?.Foreach(t => SetRequireManger(t));
            }
            if (AttributeCache<NameAttribute>.Exist(createType))
            {
                title = CommonFun.Name(createType);
            }
            if (AttributeCache<TipAttribute>.Exist(createType))
            {
                tipAttribute = CommonFun.Tip(createType);
            }
        }

        #region 代码生成  

        /// <summary>
        /// 当开始生成代码之前调用
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeforeBeginCreateCode(ICodeWirter codeWirter)
        {
            ClearRequiredManagerTypes();
            base.OnBeforeBeginCreateCode(codeWirter);
        }

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginCreateCode(ICodeWirter codeWirter)
        {
            base.OnBeginCreateCode(codeWirter);
            AddUsedType(typeof(ComponentMenuAttribute),
                typeof(NameAttribute),
                typeof(Attributes.IconAttribute),
                typeof(EIcon),
                typeof(string),
                typeof(StateComponent<>),
                typeof(StateLibAttribute),
                typeof(StateComponentMenuAttribute),
                typeof(ObjectPopupAttribute),
                typeof(EMemberRule),
                typeof(State),
                typeof(IGetStateCollection),
                typeof(ComponentCollection),
                typeof(SO),
                typeof(ScriptableObject));

            InitRequiredManagerTypes();
        }

        /// <summary>
        /// 当开始类型定义
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginTypeDefine(ICodeWirter codeWirter)
        {
            var managerTypesString = GetRequiredManagerTypesStringFormat();

            codeWirter.WriteSummaryFormat("{0}: {1}", title, tipAttribute);
            codeWirter.WriteFormat("[ComponentMenu(\"{0}/\" + Title{1})]", category, managerTypesString);
            codeWirter.WriteFormat("[Name(Title, nameof({0}))]", name);
            codeWirter.WriteFormat("[Tip(\"{0}\")]", tipAttribute);
            codeWirter.WriteFormat("[XCSJ.Attributes.Icon(EIcon.{0})]", icon.ToString());

            base.OnBeginTypeDefine(codeWirter);

            codeWirter.WriteSummary("标题");
            codeWirter.WriteFormat("public const string Title = \"{0}\";", title);
            codeWirter.Write();

            codeWirter.WriteSummary("创建");
            codeWirter.WriteSummaryParam("obj");
            codeWirter.WriteSummaryReturns("");
            codeWirter.WriteFormat("[StateLib(\"{0}\"{1})]", category, managerTypesString);
            codeWirter.WriteFormat("[StateComponentMenu(\"{0}/\" + Title{1})]", category, managerTypesString);
            codeWirter.WriteFormat("[Name(Title, nameof({0}))]", name);
            codeWirter.WriteFormat("[Tip(\"{0}\")]", tipAttribute);
            codeWirter.WriteFormat("[XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]");
            codeWirter.WriteFormat("public static State Create(IGetStateCollection obj) => CreateNormalState(obj);");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数ToFriendlyString
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFunc_ToFriendlyString(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary("输出友好字符串");
            codeWirter.WriteSummaryReturns();
            codeWirter.WriteFormat("public override string ToFriendlyString()");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            OnCreateFuncContent_ToFriendlyString(codeWirter);
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数ToFriendlyString内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFuncContent_ToFriendlyString(ICodeWirter codeWirter) => codeWirter.WriteFormat("return base.ToFriendlyString();");

        /// <summary>
        /// 当创建函数DataValidity
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFunc_DataValidity(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary("数据有效性");
            codeWirter.WriteSummaryReturns();
            codeWirter.WriteFormat("public override bool DataValidity()");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            OnCreateFuncContent_DataValidity(codeWirter);
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数DataValidity内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFuncContent_DataValidity(ICodeWirter codeWirter) => codeWirter.WriteFormat("return base.DataValidity();");

        /// <summary>
        /// 当创建函数Reset
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFunc_Reset(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary("重置");
            codeWirter.WriteSummaryReturns();
            codeWirter.WriteFormat("public override void Reset()");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            OnCreateFuncContent_Reset(codeWirter);
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数Reset内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFuncContent_Reset(ICodeWirter codeWirter) => codeWirter.WriteFormat("base.Reset();");

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制基础信息身体内容
        /// </summary>
        public override void OnDrawBaseInfoBodyContent()
        {
            category = EditorGUILayout.TextField(CommonFun.TempContent("目录", "目录"), category);
            title = EditorGUILayout.TextField(CommonFun.TempContent("标题", "标题"), title);
            tipAttribute = EditorGUILayout.TextField(CommonFun.TempContent("提示特性", "提示特性"), tipAttribute);

            base.OnDrawBaseInfoBodyContent();
            DrawManagers();            
        }

        #endregion

        #region 分类图标

        /// <summary>
        /// 分类
        /// </summary>
        public virtual string category { get; set; } = "其它";

        /// <summary>
        /// 图标
        /// </summary>
        public virtual EIcon icon => EIcon.State;

        #endregion

        #region 标题提示

        /// <summary>
        /// 标题
        /// </summary>
        protected string _title = "";

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = name;
                }
                return _title;
            }
            set => _title = value;
        }

        /// <summary>
        /// 提示特性
        /// </summary>
        protected string _tipAttribute = "";

        /// <summary>
        /// 提示特性
        /// </summary>
        public virtual string tipAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_tipAttribute))
                {
                    _tipAttribute = title;
                }
                return _tipAttribute;
            }
            set => _tipAttribute = value;
        }

        #endregion

        #region 管理器

        private bool displayManagers = true;

        private bool required = false;

        private void DrawManagers()
        {
            displayManagers = UICommonFun.Foldout(displayManagers, "依赖管理器列表");
            if (!displayManagers) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label("管理器名称");
            if (GUILayout.Button(CommonFun.TempContent("依赖", "当前管理器是否是被依赖的；点击本标题可切换所有的输出状态；"), GUI.skin.label, UICommonOption.Width32))
            {
                required = !required;
                managerDatas.ForEach(d => d.required = required);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < managerDatas.Count; i++)
            {
                var data = managerDatas[i];
                UICommonFun.BeginHorizontal(i);
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
                EditorGUILayout.LabelField(data.content);
                data.required = EditorGUILayout.Toggle(data.required, UICommonOption.Width32);
                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        private void InitManagerDatas()
        {
            managerDatas.Clear();
            foreach (var info in PluginCommonUtilsRoot.GetManagerTypeInfoInAppWithSort())
            {
                managerDatas.Add(new ManagerData() { managerType = info.type, content = CommonFun.NameTip(info.type) });
            }
        }

        class ManagerData
        {
            public Type managerType;
            public GUIContent content;
            public bool required = false;

            public void SetRequired(bool required) => this.required = required;
        }
        private List<ManagerData> managerDatas = new List<ManagerData>();

        /// <summary>
        /// 设置依赖管理器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="required"></param>
        public void SetRequireManger(Type type, bool required = true)
        {
            if (type == null) return;
            managerDatas.FirstOrDefault(d => d.managerType == type)?.SetRequired(required);
        }

        /// <summary>
        /// 设置依赖管理器
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="required"></param>
        public void SetRequireManger(string typeFullName, bool required = true)
        {
            if (string.IsNullOrEmpty(typeFullName)) return;
            managerDatas.FirstOrDefault(d => d.managerType.FullName == typeFullName)?.SetRequired(required);
        }

        /// <summary>
        /// 清理管理器类型列表
        /// </summary>
        public void ClearManagerDatas() => managerDatas.Clear();

        private HashSet<Type> managerTypes = new HashSet<Type>();
        
        /// <summary>
        /// 初始化依赖管理器类型列表
        /// </summary>
        private void InitRequiredManagerTypes()
        {
            foreach (var data in managerDatas.Where(d => d.required))
            {
                AddUsedType(data.managerType);
                managerTypes.Add(data.managerType);
            }
        }

        /// <summary>
        /// 清理依赖管理器类型列表
        /// </summary>
        public void ClearRequiredManagerTypes() => managerTypes.Clear();

        /// <summary>
        /// 获取依赖管理器类型列表字符串格式化
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRequiredManagerTypesStringFormat() => managerTypes?.ToString(t => ", typeof(" + t.Name + ")", "") ?? "";

        #endregion
    }
}
