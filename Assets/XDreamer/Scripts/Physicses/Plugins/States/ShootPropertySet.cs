using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Tools.Interactors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 发射属性设置: 发射属性设置
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(ShootPropertySet))]
    [Tip("发射属性设置")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class ShootPropertySet : BasePropertySet<ShootPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "发射属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(ShootPropertySet))]
        [Tip("发射属性设置")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// 发射:用于模拟枪械射击或将一个游戏对象发射出去
        /// </summary>
        [Name("发射")]
        [Tip("用于模拟枪械射击或将一个游戏对象发射出去")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Shoot _Shoot;
        
        /// <summary>
        /// 发射:用于模拟枪械射击或将一个游戏对象发射出去
        /// </summary>
        public Shoot Shoot => this.XGetComponentInGlobal(ref _Shoot, true);
        
        /// <summary>
        /// 发射列表:用于模拟枪械射击或将一个游戏对象发射出去列表
        /// </summary>
        [Name("发射列表")]
        [Tip("用于模拟枪械射击或将一个游戏对象发射出去列表")]
        public List<Shoot> _Shoots = new List<Shoot>();
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.None;
        
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName 
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("无")]
            [EnumFieldName("无")]
            None,
            
            #region 字段
            
            /// <summary>
            /// 自动销毁发射对象:发射对象在运行时动态产生，为了效率可定时销毁该对象
            /// </summary>
            [Name("自动销毁发射对象")]
            [Tip("发射对象在运行时动态产生，为了效率可定时销毁该对象")]
            [EnumFieldName("字段/自动销毁发射对象")]
            Field__autoDestoryFireObject = 1,
            
            /// <summary>
            /// 自动发射:Auto Fire
            /// </summary>
            [Name("自动发射")]
            [Tip("Auto Fire")]
            [EnumFieldName("字段/自动发射")]
            Field__autoFire,
            
            /// <summary>
            /// 自动发射时间间隔:Auto Fire Interval Time
            /// </summary>
            [Name("自动发射时间间隔")]
            [Tip("Auto Fire Interval Time")]
            [EnumFieldName("字段/自动发射时间间隔")]
            Field__autoFireIntervalTime,
            
            /// <summary>
            /// 炮弹容量:炮弹容量
            /// </summary>
            [Name("炮弹容量")]
            [Tip("炮弹容量")]
            [EnumFieldName("字段/炮弹容量")]
            Field__capacity,
            
            /// <summary>
            /// 发射对象生命时长:Life Time
            /// </summary>
            [Name("发射对象生命时长")]
            [Tip("Life Time")]
            [EnumFieldName("字段/发射对象生命时长")]
            Field__lifeTime,
            
            #endregion
            
            #region 属性
            
            /// <summary>
            /// Enabled:Enabled
            /// </summary>
            [Name("Enabled")]
            [Tip("Enabled")]
            [EnumFieldName("属性/Enabled")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled = 1000,
            
            /// <summary>
            /// Hide Flags:Hide Flags
            /// </summary>
            [Name("Hide Flags")]
            [Tip("Hide Flags")]
            [EnumFieldName("属性/Hide Flags")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// Name:Name
            /// </summary>
            [Name("Name")]
            [Tip("Name")]
            [EnumFieldName("属性/Name")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// Tag:Tag
            /// </summary>
            [Name("Tag")]
            [Tip("Tag")]
            [EnumFieldName("属性/Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// Use GUI Layout:Use GUI Layout
            /// </summary>
            [Name("Use GUI Layout")]
            [Tip("Use GUI Layout")]
            [EnumFieldName("属性/Use GUI Layout")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// Broadcast Message:Broadcast Message
            /// </summary>
            [Name("Broadcast Message")]
            [Tip("Broadcast Message")]
            [EnumFieldName("方法/Broadcast Message")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String = 10000,
            
            /// <summary>
            /// Broadcast Message:Broadcast Message
            /// </summary>
            [Name("Broadcast Message")]
            [Tip("Broadcast Message")]
            [EnumFieldName("方法/Broadcast Message")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,
            
            /// <summary>
            /// Cancel Invoke:Cancel Invoke
            /// </summary>
            [Name("Cancel Invoke")]
            [Tip("Cancel Invoke")]
            [EnumFieldName("方法/Cancel Invoke")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke,
            
            /// <summary>
            /// Cancel Invoke:Cancel Invoke
            /// </summary>
            [Name("Cancel Invoke")]
            [Tip("Cancel Invoke")]
            [EnumFieldName("方法/Cancel Invoke")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke_String,
            
            /// <summary>
            /// Compare Tag:Compare Tag
            /// </summary>
            [Name("Compare Tag")]
            [Tip("Compare Tag")]
            [EnumFieldName("方法/Compare Tag")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,
            
            /// <summary>
            /// Edit Inspector Script:Edit Inspector Script
            /// </summary>
            [Name("Edit Inspector Script")]
            [Tip("Edit Inspector Script")]
            [EnumFieldName("方法/Edit Inspector Script")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_EditInspectorScript,
            
            /// <summary>
            /// Fire:Fire
            /// </summary>
            [Name("Fire")]
            [Tip("Fire")]
            [EnumFieldName("方法/Fire")]
            Method_Fire,
            
            /// <summary>
            /// Get Component:Get Component
            /// </summary>
            [Name("Get Component")]
            [Tip("Get Component")]
            [EnumFieldName("方法/Get Component")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// Get Hash Code:Get Hash Code
            /// </summary>
            [Name("Get Hash Code")]
            [Tip("Get Hash Code")]
            [EnumFieldName("方法/Get Hash Code")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// Get Instance ID:Get Instance ID
            /// </summary>
            [Name("Get Instance ID")]
            [Tip("Get Instance ID")]
            [EnumFieldName("方法/Get Instance ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// Get Type:Get Type
            /// </summary>
            [Name("Get Type")]
            [Tip("Get Type")]
            [EnumFieldName("方法/Get Type")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// Invoke:Invoke
            /// </summary>
            [Name("Invoke")]
            [Tip("Invoke")]
            [EnumFieldName("方法/Invoke")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Invoke_String_Single,
            
            /// <summary>
            /// Invoke Repeating:Invoke Repeating
            /// </summary>
            [Name("Invoke Repeating")]
            [Tip("Invoke Repeating")]
            [EnumFieldName("方法/Invoke Repeating")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_InvokeRepeating_String_Single_Single,
            
            /// <summary>
            /// Is Invoking:Is Invoking
            /// </summary>
            [Name("Is Invoking")]
            [Tip("Is Invoking")]
            [EnumFieldName("方法/Is Invoking")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// Is Invoking:Is Invoking
            /// </summary>
            [Name("Is Invoking")]
            [Tip("Is Invoking")]
            [EnumFieldName("方法/Is Invoking")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// On Disable:On Disable
            /// </summary>
            [Name("On Disable")]
            [Tip("On Disable")]
            [EnumFieldName("方法/On Disable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_OnDisable,
            
            /// <summary>
            /// On Enable:On Enable
            /// </summary>
            [Name("On Enable")]
            [Tip("On Enable")]
            [EnumFieldName("方法/On Enable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_OnEnable,

            /// <summary>
            /// 发送消息(方法名)(方法):发送消息(方法名)
            /// </summary>
            [Name("发送消息(方法名)(方法)")]
            [Tip("发送消息(方法名)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,

            /// <summary>
            /// 发送消息(方法名+发送消息选项)(方法):发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)(方法)")]
            [Tip("发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,

            /// <summary>
            /// 向上发送消息(方法名)(方法):向上发送消息(方法名)
            /// </summary>
            [Name("向上发送消息(方法名)(方法)")]
            [Tip("向上发送消息(方法名)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,

            /// <summary>
            /// 向上发送消息(方法名+发送消息选项)(方法):向上发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)(方法)")]
            [Tip("向上发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,

            /// <summary>
            /// Start Coroutine:Start Coroutine
            /// </summary>
            [Name("Start Coroutine")]
            [Tip("Start Coroutine")]
            [EnumFieldName("方法/Start Coroutine")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// Stop All Coroutines:Stop All Coroutines
            /// </summary>
            [Name("Stop All Coroutines")]
            [Tip("Stop All Coroutines")]
            [EnumFieldName("方法/Stop All Coroutines")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopAllCoroutines,
            
            /// <summary>
            /// Stop Coroutine:Stop Coroutine
            /// </summary>
            [Name("Stop Coroutine")]
            [Tip("Stop Coroutine")]
            [EnumFieldName("方法/Stop Coroutine")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopCoroutine_String,
            
            /// <summary>
            /// To String:To String
            /// </summary>
            [Name("To String")]
            [Tip("To String")]
            [EnumFieldName("方法/To String")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            #endregion
            
        }
        
        #region 字段
        
        /// <summary>
        /// 自动销毁发射对象:发射对象在运行时动态产生，为了效率可定时销毁该对象
        /// </summary>
        [Name("自动销毁发射对象")]
        [Tip("发射对象在运行时动态产生，为了效率可定时销毁该对象")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__autoDestoryFireObject)]
        [OnlyMemberElements]
        public BoolPropertyValue _Field__autoDestoryFireObject = new BoolPropertyValue();
        
        /// <summary>
        /// 自动发射:Auto Fire
        /// </summary>
        [Name("自动发射")]
        [Tip("Auto Fire")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__autoFire)]
        [OnlyMemberElements]
        public BoolPropertyValue _Field__autoFire = new BoolPropertyValue();
        
        /// <summary>
        /// 自动发射时间间隔:Auto Fire Interval Time
        /// </summary>
        [Name("自动发射时间间隔")]
        [Tip("Auto Fire Interval Time")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__autoFireIntervalTime)]
        [OnlyMemberElements]
        public FloatPropertyValue _Field__autoFireIntervalTime = new FloatPropertyValue();
        
        /// <summary>
        /// 炮弹容量:炮弹容量
        /// </summary>
        [Name("炮弹容量")]
        [Tip("炮弹容量")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__capacity)]
        [OnlyMemberElements]
        public IntPropertyValue _Field__capacity = new IntPropertyValue();
        
        /// <summary>
        /// 发射对象生命时长:Life Time
        /// </summary>
        [Name("发射对象生命时长")]
        [Tip("Life Time")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__lifeTime)]
        [OnlyMemberElements]
        public FloatPropertyValue _Field__lifeTime = new FloatPropertyValue();
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// Enabled:Enabled
        /// </summary>
        [Name("Enabled")]
        [Tip("Enabled")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_enabled)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_enabled = new BoolPropertyValue();
        
        /// <summary>
        /// Hide Flags:Hide Flags
        /// </summary>
        [Name("Hide Flags")]
        [Tip("Hide Flags")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_hideFlags)]
        [OnlyMemberElements]
        public HideFlagsPropertyValue _Property_hideFlags = new HideFlagsPropertyValue();
        
        /// <summary>
        /// Name:Name
        /// </summary>
        [Name("Name")]
        [Tip("Name")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_name)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_name = new StringPropertyValue();
        
        /// <summary>
        /// Tag:Tag
        /// </summary>
        [Name("Tag")]
        [Tip("Tag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_tag)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_tag = new StringPropertyValue();
        
        /// <summary>
        /// Use GUI Layout:Use GUI Layout
        /// </summary>
        [Name("Use GUI Layout")]
        [Tip("Use GUI Layout")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_useGUILayout)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_useGUILayout = new BoolPropertyValue();
        
        #endregion
        
        #region 方法
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CancelInvoke_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CancelInvoke_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// tag:tag
        /// </summary>
        [Name("tag")]
        [Tip("tag")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// type:type
        /// </summary>
        [Name("type")]
        [Tip("type")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_Invoke_String_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// time:time
        /// </summary>
        [Name("time")]
        [Tip("time")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_Invoke_String_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_InvokeRepeating_String_Single_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// time:time
        /// </summary>
        [Name("time")]
        [Tip("time")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// repeatRate:repeatRate
        /// </summary>
        [Name("repeatRate")]
        [Tip("repeatRate")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__repeatRate = new FloatPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// options:options
        /// </summary>
        [Name("options")]
        [Tip("options")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// methodName:methodName
        /// </summary>
        [Name("methodName")]
        [Tip("methodName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StopCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StopCoroutine_String__methodName = new StringPropertyValue();
        
        #endregion
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__autoDestoryFireObject:
                    {
                        var value = _Field__autoDestoryFireObject.GetValue();
                        if (_Shoot != null) _Shoot._autoDestoryFireObject = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj._autoDestoryFireObject = value;
                        }
                        break;
                    }
                case EPropertyName.Field__autoFire:
                    {
                        var value = _Field__autoFire.GetValue();
                        if (_Shoot != null) _Shoot._autoFire = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj._autoFire = value;
                        }
                        break;
                    }
                case EPropertyName.Field__autoFireIntervalTime:
                    {
                        var value = _Field__autoFireIntervalTime.GetValue();
                        if (_Shoot != null) _Shoot._autoFireIntervalTime = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj._autoFireIntervalTime = value;
                        }
                        break;
                    }
                case EPropertyName.Field__capacity:
                    {
                        var value = _Field__capacity.GetValue();
                        if (_Shoot != null) _Shoot._capacity = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj._capacity = value;
                        }
                        break;
                    }
                case EPropertyName.Field__lifeTime:
                    {
                        var value = _Field__lifeTime.GetValue();
                        if (_Shoot != null) _Shoot._lifeTime = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj._lifeTime = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_Shoot != null) _Shoot.enabled = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_Shoot != null) _Shoot.hideFlags = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_Shoot != null) _Shoot.name = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_Shoot != null) _Shoot.tag = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_Shoot != null) _Shoot.useGUILayout = value;
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.BroadcastMessage(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_Shoot != null) _Shoot.BroadcastMessage(methodName, options);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_Shoot != null) _Shoot.CancelInvoke();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.CancelInvoke(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_Shoot != null) _Shoot.CompareTag(tag);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_EditInspectorScript:
                    {
                        if (_Shoot != null) _Shoot.EditInspectorScript();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.EditInspectorScript();
                        }
                        break;
                    }
                case EPropertyName.Method_Fire:
                    {
                        if (_Shoot != null) _Shoot.Fire();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.Fire();
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_Shoot != null) _Shoot.GetComponent(type);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_Shoot != null) _Shoot.GetHashCode();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_Shoot != null) _Shoot.GetInstanceID();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_Shoot != null) _Shoot.GetType();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_Shoot != null) _Shoot.Invoke(methodName, time);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.Invoke(methodName, time);
                        }
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        var methodName = _Method_InvokeRepeating_String_Single_Single__methodName.GetValue();
                        var time = _Method_InvokeRepeating_String_Single_Single__time.GetValue();
                        var repeatRate = _Method_InvokeRepeating_String_Single_Single__repeatRate.GetValue();
                        if (_Shoot != null) _Shoot.InvokeRepeating(methodName, time, repeatRate);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_Shoot != null) _Shoot.IsInvoking();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.IsInvoking(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_OnDisable:
                    {
                        if (_Shoot != null) _Shoot.OnDisable();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.OnDisable();
                        }
                        break;
                    }
                case EPropertyName.Method_OnEnable:
                    {
                        if (_Shoot != null) _Shoot.OnEnable();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.OnEnable();
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.SendMessage(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_Shoot != null) _Shoot.SendMessage(methodName, options);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.SendMessageUpwards(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_Shoot != null) _Shoot.SendMessageUpwards(methodName, options);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.StartCoroutine(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_Shoot != null) _Shoot.StopAllCoroutines();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_Shoot != null) _Shoot.StopCoroutine(methodName);
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_Shoot != null) _Shoot.ToString();
                        foreach(var obj in _Shoots)
                        {
                            if (obj != null) obj.ToString();
                        }
                        break;
                    }
            }
        }
        
        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__autoDestoryFireObject:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__autoDestoryFireObject.ToFriendlyString();
                    }
                case EPropertyName.Field__autoFire:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__autoFire.ToFriendlyString();
                    }
                case EPropertyName.Field__autoFireIntervalTime:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__autoFireIntervalTime.ToFriendlyString();
                    }
                case EPropertyName.Field__capacity:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__capacity.ToFriendlyString();
                    }
                case EPropertyName.Field__lifeTime:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__lifeTime.ToFriendlyString();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_enabled.ToFriendlyString();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_hideFlags.ToFriendlyString();
                    }
                case EPropertyName.Property_name:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_name.ToFriendlyString();
                    }
                case EPropertyName.Property_tag:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_tag.ToFriendlyString();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_useGUILayout.ToFriendlyString();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_EditInspectorScript:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Fire:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetType:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_OnDisable:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_OnEnable:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_ToString:
                    {
                        return CommonFun.Name(_propertyName);
                    }
            }
            return base.ToFriendlyString();
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__autoDestoryFireObject:
                    {
                        return _Shoot && _Field__autoDestoryFireObject.DataValidity();
                    }
                case EPropertyName.Field__autoFire:
                    {
                        return _Shoot && _Field__autoFire.DataValidity();
                    }
                case EPropertyName.Field__autoFireIntervalTime:
                    {
                        return _Shoot && _Field__autoFireIntervalTime.DataValidity();
                    }
                case EPropertyName.Field__capacity:
                    {
                        return _Shoot && _Field__capacity.DataValidity();
                    }
                case EPropertyName.Field__lifeTime:
                    {
                        return _Shoot && _Field__lifeTime.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _Shoot && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _Shoot && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _Shoot && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _Shoot && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _Shoot && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _Shoot && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if(!_Shoot) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _Shoot && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _Shoot && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_EditInspectorScript:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_Fire:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _Shoot && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if(!_Shoot) return false;
                        if(!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if(!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if(!_Shoot) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _Shoot && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_OnDisable:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_OnEnable:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _Shoot && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if(!_Shoot) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _Shoot && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if(!_Shoot) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _Shoot && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _Shoot;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _Shoot && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _Shoot;
                    }
            }
            return base.DataValidity();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
            if (Shoot) { }
        }
        
    }
}
