using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorXGUI.Styles.Base;
using XCSJ.Helper;
using XCSJ.PluginXGUI.Styles.Elements;

namespace XCSJ.EditorXGUI.Styles.Elements
{
    /// <summary>
    /// 样式元素集合属性绘制器
    /// </summary>
    [CustomEditor(typeof(ImageStyleElement))]
    public class ImageStyleElementInspector : BaseStyleElementInspector
    {
        public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            var imageStyle = targetObject as ImageStyleElement;
            switch (memberProperty.name)
            {
                case nameof(ImageStyleElement._fillCenter):
                    {
                        switch (imageStyle._imageFillType)
                        {
                            case Image.Type.Simple:
                            case Image.Type.Filled: return false;
                            case Image.Type.Sliced:
                            case Image.Type.Tiled: return true;
                        }
                        break;
                    }
                case nameof(ImageStyleElement._fillOrigin):
                    {
                        switch (imageStyle._fillMethod)
                        {
                            case Image.FillMethod.Horizontal:
                                {
                                    imageStyle._fillOrigin = UICommonFun.EnumPopup(new GUIContent("水平"), (Image.OriginHorizontal)imageStyle._fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Vertical:
                                {
                                    imageStyle._fillOrigin = UICommonFun.EnumPopup(new GUIContent("垂直"), (Image.OriginVertical)imageStyle._fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial90:
                                {
                                    imageStyle._fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向90"), (Image.Origin90)imageStyle._fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial180:
                                {
                                    imageStyle._fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向180"), (Image.Origin180)imageStyle._fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial360:
                                {
                                    imageStyle._fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向360"), (Image.Origin360)imageStyle._fillOrigin).Int();
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    break;
            }
            return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        }
    }
}
