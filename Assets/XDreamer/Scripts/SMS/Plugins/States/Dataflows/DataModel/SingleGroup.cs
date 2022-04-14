using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 组接口
    /// </summary>
    public interface ISingleGroup
    {
        /// <summary>
        /// 组名
        /// </summary>
        string groupName { get; }

        /// <summary>
        /// 数量
        /// </summary>
        int count { get; }

        /// <summary>
        /// 成员
        /// </summary>
        List<ISingleGroupMember> members { get; }
    }

    /// <summary>
    /// 用于归类的组对象：用于归类的组对象，不支持组嵌套组
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SingleGroup))]
    [XCSJ.Attributes.Icon(EIcon.Category)]
    [Tip("用于归类的组对象")]
    public class SingleGroup : StateComponent<SingleGroup>, ISingleGroup
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组";

        /// <summary>
        /// Title
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SingleGroup))]
        [Tip("创建组")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 组名
        /// </summary>
        [Name("组名")]
        public string _groupName = Title;

        /// <summary>
        /// 组名
        /// </summary>
        public string groupName { get => _groupName; }

        /// <summary>
        /// 组内成员数量
        /// </summary>
        public int count => members.Count;

        /// <summary>
        /// 单一组成员
        /// </summary>
        public List<ISingleGroupMember> members { get; } = new List<ISingleGroupMember>();

        /// <summary>
        /// 添加组成员
        /// </summary>
        /// <param name="member"></param>
        public void AddGroupMember(ISingleGroupMember member)
        {
            if (member!=null && !members.Contains(member))
            {
                members.Add(member);
            }
        }

        /// <summary>
        /// 移除组成员
        /// </summary>
        /// <param name="member"></param>
        public void RemoveGroupMember(ISingleGroupMember member)
        {
            if (member!=null)
            {
                members.Remove(member);
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;
    }

    /// <summary>
    /// 单一组缓存：管理组与组件对象的关系
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleGroupCache<T> where T : StateComponent
    {
        /// <summary>
        /// 组-成员 字典
        /// </summary>
        public Dictionary<ISingleGroup, List<T>> groupMemberMap { get; private set; } = new Dictionary<ISingleGroup, List<T>>();

        /// <summary>
        /// 成员-组 字典
        /// </summary>
        public Dictionary<T, ISingleGroup> memberGroupMap { get; private set; } = new Dictionary<T, ISingleGroup>();

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="items"></param>
        public void Create(IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                var gc = i.GetComponent<SingleGroupMember>();
                if (gc && gc.group != null)
                {
                    memberGroupMap.Add(i, gc.group);
                }
            }
            foreach (var i in memberGroupMap)
            {
                if (!groupMemberMap.TryGetValue(i.Value, out List<T> list))
                {
                    groupMemberMap[i.Value] = list = new List<T>();
                }
                list.Add(i.Key);
            }
        }

        /// <summary>
        /// 清除分组信息
        /// </summary>
        public void Clear()
        {
            memberGroupMap.Clear();
            groupMemberMap.Clear();
        }
    }
}
