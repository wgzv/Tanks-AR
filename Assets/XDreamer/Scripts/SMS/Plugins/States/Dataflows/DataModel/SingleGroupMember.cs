using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 单一组成员接口
    /// </summary>
    public interface ISingleGroupMember
    {
        ISingleGroup group { get; }
    }

    /// <summary>
    /// 单一组成员：将当前对象设置为单一组成员
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SingleGroupMember))]
    [XCSJ.Attributes.Icon(EIcon.Clip)]
    [Tip("将当前对象设置为单一组成员")]
    public class SingleGroupMember : StateComponent<SingleGroupMember>, ISingleGroupMember
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组成员";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SingleGroupMember))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 组
        /// </summary>
        [Name("组")]
        [StateComponentPopup(typeof(SingleGroup), stateCollectionType = EStateCollectionType.Root)]
        [Readonly(EEditorMode.Runtime)]
        public SingleGroup _group = null;

        /// <summary>
        /// 加入组规则
        /// </summary>
        [Name("加入组规则")]
        [EnumPopup]
        public EAddGroupRule _addGroupRule = EAddGroupRule.AddOnInit_RemoveOnDelete;

        /// <summary>
        /// 组接口
        /// </summary>
        public ISingleGroup group => _group;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public override bool Init(StateData stateData)
        {
            if (_addGroupRule == EAddGroupRule.AddOnEntry_RemoveOnExit)
            {
                AddGroup();
            }
            return base.Init(stateData);
        }

        /// <summary>
        /// 删除时
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deleteObject"></param>
        /// <returns></returns>
        public override bool OnDelete(IModel obj, bool deleteObject)
        {
            if (_addGroupRule == EAddGroupRule.AddOnEntry_RemoveOnExit)
            {
                RemoveGroup();
            }
            return base.OnDelete(obj, deleteObject);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            if (_addGroupRule == EAddGroupRule.AddOnEntry_RemoveOnExit)
            {
                AddGroup();
            }
            base.OnEntry(stateData);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            if (_addGroupRule == EAddGroupRule.AddOnEntry_RemoveOnExit)
            {
                RemoveGroup();
            }
            base.OnExit(stateData);
        }

        private void AddGroup()
        {
            if (_group)
            {
                _group.AddGroupMember(this);
            }
        }

        private void RemoveGroup()
        {
            if (_group)
            {
                _group.RemoveGroupMember(this);
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _group;
        }

        /// <summary>
        /// 加入组规则
        /// </summary>
        public enum EAddGroupRule
        {
            [Name("无")]
            None,

            [Name("初始化添加销毁移除")]
            AddOnInit_RemoveOnDelete,

            [Name("进入添加退出移除")]
            AddOnEntry_RemoveOnExit,
        }
    }
}
