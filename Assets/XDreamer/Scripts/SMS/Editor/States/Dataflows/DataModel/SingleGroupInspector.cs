using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.EditorSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 组检查器
    /// </summary>
    [CustomEditor(typeof(SingleGroup), true)]
    public class SingleGroupInspector : StateComponentInspector<SingleGroup>
    {
        private List<SingleGroupMember> groupMembers = new List<SingleGroupMember>();

        /// <summary>
        /// 组成员列表
        /// </summary>
        [Name("组成员列表")]
        public bool _display = true;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            UpdateGroupMembers();
        }

        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            DrawMemberList();
        }

        private void DrawMemberList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)), true, null, ()=> 
            {
                if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add), "为选中状态添加组成员"), EditorStyles.miniButtonLeft, UICommonOption.WH20x16))
                {
                    foreach (var node in NodeSelection.selections)
                    {
                        if (node is StateNode sn)
                        {
                            var s = sn.state;
                            if (s != stateComponent) // 不能把自己加入组内
                            {
                                s.XModifyProperty(s.name, () =>
                                {
                                    var groupMember = s.AddComponent<SingleGroupMember>();
                                    if (groupMember)
                                    {
                                        groupMember._group = stateComponent;
                                    }
                                });
                            }
                        }
                    }
                    UpdateGroupMembers();
                }
                if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Delete), "清除所有成员"), EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                {
                    for (int i = groupMembers.Count-1; i>=0; i--)
                    {
                        var m = groupMembers[i];
                        m.parent.XModifyProperty(m.parent.name, () => m.parent.RemoveComponent(m));
                    }
                    UpdateGroupMembers();
                }
            });
            if (!_display) return;

            CommonFun.BeginLayout();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("NO.", UICommonOption.Width32);
                    GUILayout.Label("状态");
                    GUILayout.Label(CommonFun.TempContent("组成员", "组成员成对象"), UICommonOption.Width100);
                    GUILayout.Label("删除", UICommonOption.Width32);
                }
                EditorGUILayout.EndHorizontal();

                UICommonFun.SeparatorLine(Color.black, true, 1, 1);

                bool delete = false;
                for (int i = 0; i < groupMembers.Count; i++)
                {
                    var m = groupMembers[i];

                    UICommonFun.BeginHorizontal(i);

                    // 编号
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                    // 状态名称
                    GUILayout.Label(m.parent.name, GUILayout.ExpandWidth(true));

                    // 组成员
                    EditorGUILayout.ObjectField(m, typeof(SingleGroupMember), true, UICommonOption.Width100);

                    // 删除
                    if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Delete), "移除组成员"), UICommonOption.WH20x16))
                    {
                        m.parent.XModifyProperty(m.parent.name, () => m.parent.RemoveComponent(m));
                        delete = true;
                        break;
                    }

                    UICommonFun.EndHorizontal();
                }
                if (delete)
                {
                    UpdateGroupMembers();
                }
            }
            CommonFun.EndLayout();
        }

        private void UpdateGroupMembers()
        {
            groupMembers.Clear();
            groupMembers.AddRange(SMSHelper.GetStateComponents<SingleGroupMember>().Where(m => m._group == stateComponent));
        }
    }
}
