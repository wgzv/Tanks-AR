using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条XYZ分量控制类
    /// </summary>
    public abstract class SliderXYZBind : View
    {
        /// <summary>
        /// XYZ滑动条
        /// </summary>
        [Name("XYZ滑动条")]
        public XYZSlider xyzSlider;

        /// <summary>
        /// xyz滑动条变化前的上一次值
        /// </summary>
        protected Vector3 xyzSliderLastValue = Vector3.zero;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (xyzSlider.x) xyzSlider.x.onValueChanged.AddListener(OnSliderXValueChanged);
            if (xyzSlider.y) xyzSlider.y.onValueChanged.AddListener(OnSliderYValueChanged);
            if (xyzSlider.z) xyzSlider.z.onValueChanged.AddListener(OnSliderZValueChanged);

            InitSliderValue();

            RecordLastSliderValue();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (xyzSlider.x) xyzSlider.x.onValueChanged.RemoveListener(OnSliderXValueChanged);
            if (xyzSlider.y) xyzSlider.y.onValueChanged.RemoveListener(OnSliderYValueChanged);
            if (xyzSlider.z) xyzSlider.z.onValueChanged.RemoveListener(OnSliderZValueChanged);
        }

        /// <summary>
        /// 记录上次滑竿值
        /// </summary>
        protected void RecordLastSliderValue()
        {
            if (xyzSlider.x) xyzSliderLastValue.x = xyzSlider.x.value;
            if (xyzSlider.y) xyzSliderLastValue.y = xyzSlider.y.value;
            if (xyzSlider.z) xyzSliderLastValue.z = xyzSlider.z.value;
        }

        /// <summary>
        /// 初始化滑竿值
        /// </summary>
        protected abstract void InitSliderValue();

        /// <summary>
        /// X滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderXValueChanged(float value);

        /// <summary>
        /// Y滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderYValueChanged(float value);

        /// <summary>
        /// Z滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderZValueChanged(float value);
    }
}
