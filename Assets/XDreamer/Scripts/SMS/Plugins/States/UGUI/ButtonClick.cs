using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// 按钮点击:按钮点击组件是按钮点击事件的触发器。当按钮被点击时，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(ButtonClick))]
    [Tip("按钮点击组件是按钮点击事件的触发器。当按钮被点击时，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(EIcon.Button, index = 33604)]
    public class ButtonClick : Trigger<ButtonClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "按钮点击";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ButtonClick))]
        [Tip("按钮点击组件是按钮点击事件的触发器。当按钮被点击时，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateButtonClick(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 批量
        /// </summary>
        [Name("批量")]
        public bool batch = false;

        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.True)]
        [ComponentPopup]
        public Button button;

        /// <summary>
        /// 批量规则
        /// </summary>
        [Name("批量规则")]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.False)]
        [EnumPopup]
        public EBatchRule batchRule = EBatchRule.Any;

        /// <summary>
        /// 按钮列表
        /// </summary>
        [Name("按钮列表")]
        [HideInSuperInspector(nameof(batch), EValidityCheckType.False)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Button> buttons = new List<Button>();

        /// <summary>
        /// 变量:将最后一个点击的按钮名称路径存储到本变量
        /// </summary>
        [Name("变量")]
        [Tip("将最后一个点击的按钮名称路径存储到本变量")]
        [GlobalVariable]
        public string _variable = "";

        private float _progress = 0;
        public override float progress { get => _progress; set => _progress = value; }

        private ButtonInfoList buttonInfoList = new ButtonInfoList();

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            _progress = 0;
            buttonInfoList.RemoveAllListener();
            AddAllListener();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            buttonInfoList.RemoveAllListener();
            base.OnExit(data);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        private void AddAllListener()
        {
            if (batch)
            {
                buttonInfoList.AddListener(OnClicked, buttons.ToArray());
            }
            else
            {
                buttonInfoList.AddListener(OnClicked, button);
            }
        }

        private void OnClicked(ButtonInfo buttonInfo)
        {
            switch (batchRule)
            {
                case EBatchRule.Any:
                    {
                        _progress = 1;
                        finished = true;
                        ScriptManager.TrySetGlobalVariableValue(_variable, CommonFun.GameObjectComponentToStringWithoutAlias(buttonInfo.button));
                        break;
                    }
                case EBatchRule.All:
                    {
                        var count = buttonInfoList.buttonInfos.Count(info => info.count > 0);
                        if (count == buttonInfoList.buttonInfos.Count)
                        {
                            _progress = 1;
                            finished = true;
                            ScriptManager.TrySetGlobalVariableValue(_variable, CommonFun.GameObjectComponentToStringWithoutAlias(buttonInfo.button));
                        }
                        else
                        {
                            _progress = count * 1f / buttonInfoList.buttonInfos.Count;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 有效
        /// </summary>
        protected void OnValidate()
        {
            if (parent.isActive)
            {
                buttonInfoList.RemoveAllListener();
                AddAllListener();
            }
        }

        public override bool DataValidity()
        {
            return batch ? (buttons.Count > 0) : button;
        }

        public override string ToFriendlyString() => batch? buttons.Count.ToString() : (button ? button.name : "");
    }

    /// <summary>
    /// 批量规则
    /// </summary>
    [Name("批量规则")]
    public enum EBatchRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不作任何处理;")]
        None,

        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        [Tip("按钮列表中任意一个按钮被点击，本组件均会被标记为完成态；")]
        Any,

        /// <summary>
        /// 所有
        /// </summary>
        [Name("所有")]
        [Tip("按钮列表中所有按钮都被点击至少一次后，本组件才被标记为完成态；")]
        All,
    }

    /// <summary>
    /// 按钮信息列表
    /// </summary>
    public class ButtonInfoList
    {
        /// <summary>
        /// 按钮信息列表
        /// </summary>
        public List<ButtonInfo> buttonInfos { get; private set; } = new List<ButtonInfo>();

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="onClicked"></param>
        /// <param name="buttons"></param>
        public void AddListener(Action<ButtonInfo> onClicked, params Button[] buttons)
        {
            if (buttons==null || buttons.Length==0) return;

            foreach (var button in buttons)
            {
                var info = new ButtonInfo(button, onClicked);
                info.AddListener();
                buttonInfos.Add(info);
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (var info in buttonInfos)
            {
                info.RemoveListener();
            }
            buttonInfos.Clear();
        }
    }

    /// <summary>
    /// 按钮信息
    /// </summary>
    public class ButtonInfo
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public Button button { get; private set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int count { get; private set; } = 0;

        /// <summary>
        /// 点击
        /// </summary>
        public Action<ButtonInfo> onClicked { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="button"></param>
        /// <param name="onClicked"></param>
        public ButtonInfo(Button button, Action<ButtonInfo> onClicked)
        {
            this.button = button;
            this.onClicked = onClicked;
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddListener() => button.onClick.AddListener(OnClicked);

        /// <summary>
        /// 停止监听
        /// </summary>
        public void RemoveListener() => button.onClick.RemoveListener(OnClicked);

        /// <summary>
        /// 点击
        /// </summary>
        public void OnClicked()
        {
            count++;
            onClicked?.Invoke(this);
        }
    }

}
