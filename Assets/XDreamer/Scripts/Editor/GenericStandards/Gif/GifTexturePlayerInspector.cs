using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.GenericStandards.Gif;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.GenericStandards.Gif
{
    [CustomEditor(typeof(GifTexturePlayer), true)]
    public class GifTexturePlayerInspector : BaseInspectorWithLimit<GifTexturePlayer>
    {
        private static string[] imageExtends = new string[] { ".png", ".jpg", ".jpeg" };

        //public override bool OnNeedDraw(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        //{
        //    switch (memberProperty.name)
        //    {
        //        //case nameof(targetObject.gifTexture.assetName):
        //        //case nameof(targetObject.gifTexture.textureList):
        //        //    {
        //        //        return targetObject.gifTexture.gifTextureAssetBytes ? false : true;
        //        //    }
        //        default:
        //            break;
        //    }
        //    return base.OnNeedDraw(type, memberProperty, arrayElementInfo);
        //}

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(targetObject.gifTexture.textureList):
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            if (GUILayout.Button(CommonFun.NameTip(EIcon.Folder), UICommonOption.WH24x16))
                            {
                                string path = EditorUtility.OpenFolderPanel("选择导入图片文件夹", Application.dataPath, "");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    string[] filePaths = Directory.GetFiles(path, "*.*").Where(s => imageExtends.ToList().Exists(i => s.EndsWith(i))).ToArray();

                                    foreach (var fp in filePaths)
                                    {
                                        var relativePath = fp.Substring(fp.IndexOf("Assets"));
                                        var rawImg = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
                                        targetObject.gifTexture.textureList.Add(new FrameTexture(rawImg, 0.04f));// 按一秒25张图的效果
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override bool OnModifiedArraySize(Type type, SerializedProperty memberProperty, int newSize)
        {
            if (!Application.isPlaying)
            {
                //非播放状态
                if (targetObject.gifTexture.gifTextureAssetBytes != null && memberProperty.name == "textureList")
                {
                    Debug.LogWarning("非运行态，当 " + CommonFun.Name(typeof(GifTexture), "gifTextureAssetBytes") + " 不为 null 时，不可修改 " + CommonFun.Name(type, memberProperty.name) + " 的数组/队列元素数目！");
                    return false;
                }
            }
            return base.OnModifiedArraySize(type, memberProperty, newSize);
        }

        public override bool OnModifiedPropertyField(System.Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            if (type == typeof(GifTexture) && memberProperty.name == "gifTextureAssetBytes" && targetObject.gifTexture.textureList.Count != 0)
            {
                Debug.LogWarning("当 " + CommonFun.Name(type, memberProperty.name) + " 的数组/队列元素数目不为 0 时，不可修改" + CommonFun.Name(typeof(GifTexture), "gifTextureAssetBytes") + " 对象！");
                return false;
            }
            return base.OnModifiedPropertyField(type, memberProperty, arrayElementInfo);
        }
    }
}
