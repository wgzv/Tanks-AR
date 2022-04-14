using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorExtension.EditorWindows;

namespace XCSJ.EditorTools.Windows.CodeCreaters
{
    /// <summary>
    /// 代码预览
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Script)]
    public class CodePreviewWindow : XEditorWindowWithScrollView<CodePreviewWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "代码预览";

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="codeText"></param>
        public static void Open(string codeText = "")
        {
            OpenAndFocus();
            instance.UpdateCodeText(codeText);
        }

        /// <summary>
        /// 代码文本
        /// </summary>
        public string codeText = "";

        private void UpdateCodeText(string codeText)
        {
            this.codeText = codeText ;

            codeTexts = codeText.Split('\n').ToList();

            var sb = new StringBuilder();
            var count = codeTexts.Count;
            for (int i = 0; i < count; i++)
            {
                sb.AppendLine((i + 1).ToString());
            }
            codeNumText = sb.ToString();
            codeNumWidth = 32 + (Math.Max(count.ToString().Length, 3) - 3) * 8;
        }

        public string codeNumText = "";

        public float codeNumWidth = 32;

        private List<string> codeTexts = new List<string>();

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(codeNumText, GUILayout.ExpandHeight(true), GUILayout.Width(codeNumWidth));
            EditorGUILayout.TextArea(codeText, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }
    }
}
