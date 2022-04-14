using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.LineNotes
{
    [Tool("标注", rootType = typeof(ToolsManager))]
    [Name("批注-2D-GUI")]
    [XCSJ.Attributes.Icon(EIcon.Note)]
    public class GUILineNote2D : LineNote2D, IOnGUI
    {
        [Name("GUI样式")]
        public GUIStyle style;

        [Name("文本")]
        public string text;

        protected override void Start()
        {
            base.Start();

            target = gameObject;
        }

        protected override void Reset()
        {
            base.Reset();

            target = gameObject;
        }

        public void OnGUI()
        {
            if (valid)
            {
                var pos = cam.WorldToScreenPoint(endPoint);
                pos.y = Screen.height - pos.y;
                var content = CommonFun.TempContent(text);
                var size = style.CalcSize(content);

                var dir = lineAngle > 180 ? 0 : 1;
                GUI.Button(new Rect(pos.x - size.x / 2, pos.y - dir * size.y, size.x, size.y), content, style);
            }
        }
    }
}
