using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 立体相机检查器
    /// </summary>
    [CustomEditor(typeof(StereoCamera))]
    public class StereoCameraInspector : MBInspector<StereoCamera>
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            var stereoCamera = targetObject;
            if (stereoCamera.stereoMode == EStereoMode1.LeftRight && (!stereoCamera.leftEye || !stereoCamera.rightEye))
            {
                if (GUILayout.Button("创建左右眼相机"))
                {
                    targetObject.CreateLeftRight();
                }
            }
        }
    }
}
