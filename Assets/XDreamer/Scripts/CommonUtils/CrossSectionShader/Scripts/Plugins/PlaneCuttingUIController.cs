using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Sliders;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    [RequireComponent(typeof(PlaneCuttingController))]
    public class PlaneCuttingUIController : BasePlaneCuttingMB
    {
        [Name("位置")]
        public XYZSlider position = null;

        [Name("X")]
        public ToggleRectTransformInfo planeXInfo = null;

        [Name("Y")]
        public ToggleRectTransformInfo planeYInfo = null;

        [Name("Z")]
        public ToggleRectTransformInfo planeZInfo = null;

        [Name("面板")]
        public ToggleXYZSlider planeX = null;

        [Name("面板")]
        public ToggleXYZSlider planeY = null;

        [Name("面板")]
        public ToggleXYZSlider planeZ = null;

        [Group("显隐", needBoundBox = true)]
        [Name("轴")]
        public Toggle aixs = null;

        [Name("面")]
        public Toggle plane = null;

        [Name("剖面")]
        public Toggle cuttingPlane = null;

        public PlaneCuttingController planeCuttingCtrl => GetComponent<PlaneCuttingController>();

        public ThreePlanesCuttingController threePlaneCtrl
        {
            get
            {
                var pcc = planeCuttingCtrl;
                if (pcc)
                {
                    return pcc.threePlanesCuttingController;
                }
                return null;
            }
        }

        /// <summary>
        /// 启用初始化
        /// </summary>
        public void Awake()
        {
            ActiveUI();

            RecordData();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            ActiveUI();
        }

        #region 设置对象

        public void ActiveUI()
        {
            if (planeX.active) SetPlaneXActive(planeX.active.isOn);
            if (planeY.active) SetPlaneYActive(planeY.active.isOn);
            if (planeZ.active) SetPlaneZActive(planeZ.active.isOn);

            if (aixs) SetAixsActive(aixs.isOn);
            if (plane) SetPlaneActive(plane.isOn);
            if (cuttingPlane) SetCuttingPlaneActive(cuttingPlane.isOn);
        }

        public void SetPositionRange()
        {
            var controller = planeCuttingCtrl;
            if (CommonFun.GetBounds(out Bounds bounds, controller.cuttedObjects, controller.includeChildren, controller.includeInactiveGameObject, controller.includeDisableRenderer))
            {
                SetPositionSliderRange(new Vector2(bounds.min.x, bounds.max.x), new Vector2(bounds.min.y, bounds.max.y), new Vector2(bounds.min.z, bounds.max.z));
            }
        }

        public void SetPositionSliderRange(Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
        {
            position?.SetRange(rangeX, rangeY, rangeZ);
        }

        public void SetPosition(Vector3 position)
        {
            this.position?.SetValue(position);
        }

        public void SetPlaneAngle(Vector3 angleX, Vector3 angleY, Vector3 angleZ)
        {
            planeX?.SetValue(angleX);
            planeY?.SetValue(angleY);
            planeZ?.SetValue(angleZ);
        }

        public void SetPlaneInfoActive(bool activeX, bool activeY, bool activeZ)
        {
            planeXInfo.SetActive(activeX);
            planeYInfo.SetActive(activeY);
            planeZInfo.SetActive(activeZ);
        }
        
        public void SetPlaneXActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActivePlaneX(active);
            }

            planeX?.SetRootActive(active);
            planeX?.SetActive(active);
        }

        public void SetPlaneYActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActivePlaneY(active);
            }

            planeY?.SetRootActive(active);
            planeY?.SetActive(active);
        }

        public void SetPlaneZActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActivePlaneZ(active);
            }

            planeZ?.SetRootActive(active);
            planeZ?.SetActive(active);
        }

        public void SetAixsActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActiveAixs(active);
            }

            if (aixs)
            {
                aixs.isOn = active;
            }
        }

        public void SetPlaneActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActivePlane(active);
            }

            if (plane)
            {
                plane.isOn = active;
            }
        }

        public void SetCuttingPlaneActive(bool active)
        {
            var tpcc = threePlaneCtrl;
            if (tpcc)
            {
                tpcc.ActiveCuttingPlane(active);
            }

            if (cuttingPlane)
            {
                cuttingPlane.isOn = active;
            }
        }

        #endregion

        #region 重置数据

        public void RecordData()
        {
            position?.RecordData();

            planeX?.RecordData();
            planeY?.RecordData();
            planeZ?.RecordData();            
        }

        public void ResetData()
        {
            ResetPosition();
            ResetPlanesAngle();
        }

        public void ResetPosition()
        {
            position?.RecoverData();
        }

        public void ResetPlanesAngle()
        {
            if (planeX!=null && planeX.root && planeX.root.gameObject.activeInHierarchy)
            {
                planeX.RecoverData();
            }

            if (planeY != null && planeY.root && planeY.root.gameObject.activeInHierarchy)
            {
                planeY.RecoverData();
            }

            if (planeZ != null && planeZ.root && planeZ.root.gameObject.activeInHierarchy)
            {
                planeZ.RecoverData();
            }
        }

        #endregion
    }
    
    [Serializable]
    public class ToggleXYZSlider : XYZSlider
    {
        [Name("激活")]
        public Toggle active = null;

        public void GetValue(ref Vector3 xyzValue, ref bool activeValue)
        {
            GetValue(ref xyzValue);

            if (active)
            {
                activeValue = active.isOn;
            }
        }

        public void SetValue(Vector3 xyzValue, bool activeValue)
        {
            SetValue(xyzValue);

            SetActive(activeValue);
        }

        public void SetActive(bool activeValue)
        {
            if (active)
            {
                active.isOn = activeValue;
            }
        }
    }

    [Serializable]
    public class ToggleRectTransformInfo
    {
        [Name("切换")]
        public Toggle toggle = null;

        [Name("面板")]
        public RectTransform rectTransform = null;

        public void SetActive(bool isOn)
        {
            if(toggle)
            {
                toggle.isOn = isOn;
                if (rectTransform)
                {
                    rectTransform.gameObject.SetActive(isOn);
                }
            }
        }
    }
}
