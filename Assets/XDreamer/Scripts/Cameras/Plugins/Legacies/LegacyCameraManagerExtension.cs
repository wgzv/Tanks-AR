using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCamera;

namespace XCSJ.PluginsCameras.Legacies
{
    /// <summary>
    /// 旧版相机管理器扩展：用于扩展提供旧版相机的各种接口
    /// </summary>
    public static class LegacyCameraManagerExtension
    {
#pragma warning disable CS0618 // 类型或成员已过时

        /// <summary>
        /// 获取旧版提供者
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static LegacyCameraManagerProvider GetLegacyProvider(this CameraManager manager)
        {
            return (LegacyCameraManagerProvider)manager.legacyCameraManagerProvider;
        }

        public static BaseCamera GetCurrentCamera(this CameraManager manager)
        {
            return manager.legacyCameraManagerProvider.currentCamera;
        }

        public static BaseCamera SetCurrentCamera(this CameraManager manager, BaseCamera baseCamera)
        {
            return manager.legacyCameraManagerProvider.currentCamera = baseCamera;
        }

        public static BaseCamera GetInitCamera(this CameraManager manager)
        {
            return manager.GetLegacyProvider()._initCamera;
        }

        public static void SetInitCamera(this CameraManager manager, BaseCamera baseCamera)
        {
            manager.GetLegacyProvider().SetInitCamera(baseCamera);
        }

        public static bool SwitchCamera(this CameraManager manager, GameObject target, float duration, string funcCallback, bool mustSwitch)
        {
            return manager.GetLegacyProvider().SwitchCamera(target, duration, funcCallback, mustSwitch);
        }

        public static bool SwitchCamera(this CameraManager manager, BaseCamera bc, float duration, string funcCallback, bool mustSwitch)
        {
            return manager.GetLegacyProvider().SwitchCamera(bc, duration, funcCallback, mustSwitch);
        }

        public static bool SwitchCameraByName(this CameraManager manager, string cameraName, float duration, string funcCallback, bool mustSwitch)
        {
            return manager.GetLegacyProvider().SwitchCameraByName(cameraName, duration, funcCallback, mustSwitch);
        }

        public static bool SetCameraMoveSpeed(this CameraManager manager, GameObject targetCamera, float speed)
        {
            return manager.GetLegacyProvider().SetCameraMoveSpeed(targetCamera, speed);
        }

        public static bool SetCameraRotateSpeed(this CameraManager manager, GameObject targetCamera, float speed)
        {
            return manager.GetLegacyProvider().SetCameraRotateSpeed(targetCamera, speed);
        }

        public static bool SetCurrentCameraMoveSpeed(this CameraManager manager, float speed)
        {
            return manager.GetLegacyProvider().SetCurrentCameraMoveSpeed(speed);
        }

        public static bool SetCurrentCameraRotateSpeed(this CameraManager manager, float speed)
        {
            return manager.GetLegacyProvider().SetCurrentCameraRotateSpeed(speed);
        }

        public static bool SetCameraNearClipPlane(this CameraManager manager, GameObject targetCamera, float nearClipPlane)
        {
            return manager.GetLegacyProvider().SetCameraNearClipPlane(targetCamera, nearClipPlane);
        }

        public static bool SetCurrentCameraNearClipPlane(this CameraManager manager, float nearClipPlane)
        {
            return manager.GetLegacyProvider().SetCurrentCameraNearClipPlane(nearClipPlane);
        }

        public static bool SetCameraFarClipPlane(this CameraManager manager, GameObject targetCamera, float farClipPlane)
        {
            return manager.GetLegacyProvider().SetCameraFarClipPlane(targetCamera, farClipPlane);
        }

        public static bool SetCurrentCameraFarClipPlane(this CameraManager manager, float farClipPlane)
        {
            return manager.GetLegacyProvider().SetCurrentCameraFarClipPlane(farClipPlane);
        }

        public static bool SetCameraFieldOfView(this CameraManager manager, GameObject targetCamera, float fieldOfView)
        {
            return manager.GetLegacyProvider().SetCameraFieldOfView(targetCamera, fieldOfView);
        }

        public static bool SetCurrentCameraFieldOfView(this CameraManager manager, float fieldOfView)
        {
            return manager.GetLegacyProvider().SetCurrentCameraFieldOfView(fieldOfView);
        }

        public static bool MoveCamera(this CameraManager manager, GameObject go, Vector3 dir)
        {
            return manager.GetLegacyProvider().MoveCamera(go, dir);
        }
        public static bool MoveCamera(this CameraManager manager, BaseCamera bc, Vector3 dir)
        {
            return manager.GetLegacyProvider().MoveCamera(bc, dir);
        }

        public static void ResetCurrentCamera(this CameraManager manager, float duration, string fun = "", string param = "")
        {
            manager.GetLegacyProvider().ResetCurrentCamera(duration, fun, param);
        }

        public static BaseCamera CreateCamera<T>(this CameraManager manager) where T : BaseCamera
        {
            return manager.GetLegacyProvider().CreateCamera<T>();
        }

        public static BaseCamera CreateCamera(this CameraManager manager, Type t)
        {
            return manager.GetLegacyProvider().CreateCamera(t);
        }

        public static List<string> GetCameras(this CameraManager manager)
        {
            return manager.GetLegacyProvider().GetCameras();
        }

#pragma warning restore CS0618 // 类型或成员已过时
    }
}
