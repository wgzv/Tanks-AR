using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorExtension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.TimeLine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Helper;
using System.Text;
using XCSJ.PluginSMS.States.TimeLine.UI;
using System.Linq;
using XCSJ.EditorTools;

namespace XCSJ.EditorSMS.States.TimeLine
{
    [CustomEditor(typeof(TimeLinePlayer))]
    public class TimeLinePlayerInspector : StateComponentInspector<TimeLinePlayer>
    {
        protected TimeLinePlayer player => target as TimeLinePlayer;

        private SerializedProperty useContentTimeLength;

        public override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            useContentTimeLength = serializedObject.FindProperty(nameof(TimeLinePlayer.useContentTimeLength));
            EditorApplication.hierarchyChanged += FindObject;
            FindObject();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            timeLineUIRoot = null;
            EditorApplication.hierarchyChanged += FindObject;
        }

        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(TimeLinePlayer.useContentTimeLength):
                    {
                        return false;
                    }
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(TimeLinePlayer.playContent):
                    {
                        EditorGUI.BeginDisabledGroup(player.playContent);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            memberProperty.objectReferenceValue = TimeLinePlayContent.CreateTimeLinePlayContent(player).GetComponent<TimeLinePlayContent>();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
                case nameof(TimeLinePlayer.duration):
                    {
                        if (useContentTimeLength == null) break;
                        if ((useContentTimeLength.boolValue = UICommonFun.ButtonToggle(CommonFun.NameTooltip(typeof(TimeLinePlayer), nameof(TimeLinePlayer.useContentTimeLength)), useContentTimeLength.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(60))) && player.playContent)
                        {
                            //player.UseContentTimeLength();
                            memberProperty.floatValue = player.playContent.GetTimeLength();
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private void FindObject()
        {
            timeLineUIRoot = CommonFun.GetComponentsInChildren<TimeLineUIRoot>(true).FirstOrDefault(ui => ui.useCurrentActivePlayer || ui._timeLinePlayer == player);
        }

        private TimeLineUIRoot timeLineUIRoot = null;

        public override void OnAfterVertical()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("界面");
            if (GUILayout.Button(CommonFun.NameTip(MethodInfoCache.Get(typeof(CreateTimeUITool), nameof(CreateTimeUITool.CreateTimeLinePlayer), TypeHelper.DefaultLookup))))
            {
                if (timeLineUIRoot)
                {
                    EditorGUIUtility.PingObject(timeLineUIRoot.gameObject);
                }
                else
                {
                    var go = CreateTimeUITool.CreateTimeLinePlayer(ToolContext.Get(typeof(CreateTimeUITool), nameof(CreateTimeUITool.CreateTimeLinePlayer)));
                    timeLineUIRoot = FindObjectOfType<TimeLineUIRoot>();
                    if (timeLineUIRoot)
                    {
                        timeLineUIRoot._timeLinePlayer = player;
                        EditorGUIUtility.PingObject(timeLineUIRoot.gameObject);
                    }
                }
               
            }
            EditorGUILayout.EndHorizontal();

            DrawPlayerController();

            base.OnAfterVertical();
        }

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (player.playContent)
            {
                info.AppendFormat("播放内容:\t{0}", player.playContent.GetNamePath());
            }
            else
            {
                info.Append("<color=red>播放内容无效</color>");
            }
            return info;
        }

        public override StringBuilder GetRuntimeHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (player.playContent)
            {
                info.AppendFormat("播放状态:\t{0}", player.playerState);
                info.AppendFormat("\n当前进度:\t{0}", player.percent);
                info.AppendFormat("\n当前时间:\t{0}", player.curTime);
            }
            return info;
        }

        [Name("播放器控制器")]
        private bool playerController = true;

        private void DrawPlayerController()
        {
            if (!(playerController = UICommonFun.Foldout(playerController, CommonFun.NameTip(GetType(), nameof(playerController))))) return;

            CommonFun.BeginLayout();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying || !player.parent.isActive);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("控制", UICommonOption.Width48, UICommonOption.Height32);
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Play), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.PlayOrResume();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Pause), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.Pause();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Stop), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.Stop();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("进度", UICommonOption.Width48);
            EditorGUI.BeginChangeCheck();
            var percent = EditorGUILayout.Slider(player.percent, 0, 1);
            if (EditorGUI.EndChangeCheck())
            {
                player.SetPercent(percent);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("时间", UICommonOption.Width48);
            EditorGUI.BeginChangeCheck();
            var time = EditorGUILayout.Slider(player.curTime, 0, player.duration);
            if (EditorGUI.EndChangeCheck())
            {
                player.SetTime(time);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            CommonFun.EndLayout();
        }
    }
}
