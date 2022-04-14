using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.DataBase;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils.Safety.XR;

namespace XCSJ.PluginXRSpaceSolution.Base
{
    /// <summary>
    /// XR扩展问题
    /// </summary>
    public abstract class XRExtensionQ : XRQuestion { }

    /// <summary>
    /// XR扩展答案
    /// </summary>
    public abstract class XRExtensionA : XRAnswer { }

    /// <summary>
    /// XR空间配置答案
    /// </summary>
    public class XRSpaceConfigA : XRExtensionA
    {
        #region 空间偏移

        /// <summary>
        /// 空间偏移
        /// </summary>
        [Browsable(false)]
        public Pose spaceOffset { get; set; } = new Pose();

        [Category("空间偏移")]
        [DisplayName("位置")]
        [Json(false)]
        public V3F spaceOffset_Position => spaceOffset.position;

        [Category("空间偏移")]
        [DisplayName("位置X")]
        [Json(false)]
        public float spaceOffset_Position_x { get => spaceOffset.position.x; set => spaceOffset._position.x = value; }

        [Category("空间偏移")]
        [DisplayName("位置Y")]
        [Json(false)]
        public float spaceOffset_Position_y { get => spaceOffset.position.y; set => spaceOffset._position.y = value; }

        [Category("空间偏移")]
        [DisplayName("位置Z")]
        [Json(false)]
        public float spaceOffset_Position_z { get => spaceOffset.position.z; set => spaceOffset._position.z = value; }

        [Category("空间偏移")]
        [DisplayName("旋转")]
        [Json(false)]
        public V3F spaceOffset_Rotation => spaceOffset.rotation;

        [Category("空间偏移")]
        [DisplayName("旋转X")]
        [Json(false)]
        public float spaceOffset_Rotation_x { get => spaceOffset.rotation.x; set => spaceOffset._rotation.x = value; }

        [Category("空间偏移")]
        [DisplayName("旋转Y")]
        [Json(false)]
        public float spaceOffset_Rotation_y { get => spaceOffset.rotation.y; set => spaceOffset._rotation.y = value; }

        [Category("空间偏移")]
        [DisplayName("旋转Z")]
        [Json(false)]
        public float spaceOffset_Rotation_z { get => spaceOffset.rotation.z; set => spaceOffset._rotation.z = value; }

        #endregion

        #region 屏幕组偏移

        /// <summary>
        /// 屏幕组偏移
        /// </summary>
        [Browsable(false)]
        public Pose screenGroupOffset { get; set; } = new Pose();

        [Category("屏幕组偏移")]
        [DisplayName("位置")]
        [Json(false)]
        public V3F screenGroupOffset_Position => screenGroupOffset.position;

        [Category("屏幕组偏移")]
        [DisplayName("位置X")]
        [Json(false)]
        public float screenGroupOffset_Position_x { get => screenGroupOffset.position.x; set => screenGroupOffset._position.x = value; }

        [Category("屏幕组偏移")]
        [DisplayName("位置Y")]
        [Json(false)]
        public float screenGroupOffset_Position_y { get => screenGroupOffset.position.y; set => screenGroupOffset._position.y = value; }

        [Category("屏幕组偏移")]
        [DisplayName("位置Z")]
        [Json(false)]
        public float screenGroupOffset_Position_z { get => screenGroupOffset.position.z; set => screenGroupOffset._position.z = value; }

        [Category("屏幕组偏移")]
        [DisplayName("旋转")]
        [Json(false)]
        public V3F screenGroupOffset_Rotation => screenGroupOffset.rotation;

        [Category("屏幕组偏移")]
        [DisplayName("旋转X")]
        [Json(false)]
        public float screenGroupOffset_Rotation_x { get => screenGroupOffset.rotation.x; set => screenGroupOffset._rotation.x = value; }

        [Category("屏幕组偏移")]
        [DisplayName("旋转Y")]
        [Json(false)]
        public float screenGroupOffset_Rotation_y { get => screenGroupOffset.rotation.y; set => screenGroupOffset._rotation.y = value; }

        [Category("屏幕组偏移")]
        [DisplayName("旋转Z")]
        [Json(false)]
        public float screenGroupOffset_Rotation_z { get => screenGroupOffset.rotation.z; set => screenGroupOffset._rotation.z = value; }

        #endregion

        #region 相机偏移：装备偏移

        /// <summary>
        /// 相机偏移：装备偏移
        /// </summary>
        [Browsable(false)]
        public Pose cameraOffset { get; set; } = new Pose();

        [Category("相机偏移")]
        [DisplayName("位置")]
        [Json(false)]
        public V3F cameraOffset_Position => cameraOffset.position;

        [Category("相机偏移")]
        [DisplayName("位置X")]
        [Json(false)]
        public float cameraOffset_Position_x { get => cameraOffset.position.x; set => cameraOffset._position.x = value; }

        [Category("相机偏移")]
        [DisplayName("位置Y")]
        [Json(false)]
        public float cameraOffset_Position_y { get => cameraOffset.position.y; set => cameraOffset._position.y = value; }

        [Category("相机偏移")]
        [DisplayName("位置Z")]
        [Json(false)]
        public float cameraOffset_Position_z { get => cameraOffset.position.z; set => cameraOffset._position.z = value; }

        [Category("相机偏移")]
        [DisplayName("旋转")]
        [Json(false)]
        public V3F cameraOffset_Rotation => cameraOffset.rotation;

        [Category("相机偏移")]
        [DisplayName("旋转X")]
        [Json(false)]
        public float cameraOffset_Rotation_x { get => cameraOffset.rotation.x; set => cameraOffset._rotation.x = value; }

        [Category("相机偏移")]
        [DisplayName("旋转Y")]
        [Json(false)]
        public float cameraOffset_Rotation_y { get => cameraOffset.rotation.y; set => cameraOffset._rotation.y = value; }

        [Category("相机偏移")]
        [DisplayName("旋转Z")]
        [Json(false)]
        public float cameraOffset_Rotation_z { get => cameraOffset.rotation.z; set => cameraOffset._rotation.z = value; }

        #endregion

        /// <summary>
        /// 屏幕列表
        /// </summary>
        [Browsable(false)]
        public List<ScreenInfo> screens { get; set; } = new List<ScreenInfo>();

        /// <summary>
        /// 添加屏幕
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public ScreenInfo AddScreen(string screenName, string screenDisplayName)
        {
            var screenInfo = new ScreenInfo() { name = screenName, displayName = screenDisplayName };
            screens.Add(screenInfo);
            return screenInfo;
        }

        /// <summary>
        /// 相机列表
        /// </summary>
        [Browsable(false)]
        public List<CameraInfo> cameras { get; set; } = new List<CameraInfo>();

        /// <summary>
        /// 添加相机
        /// </summary>
        public CameraInfo AddCamera(string cameraName, string screenName)
        {
            var cameraInfo = new CameraInfo() { name = cameraName, screen = screenName };
            cameraInfo.config = this;
            cameras.Add(cameraInfo);
            return cameraInfo;
        }

        internal bool isDirty { get; set; } = false;

        /// <summary>
        /// 标记脏
        /// </summary>
        public void MarkDirty() => isDirty = true;

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public XRSpaceConfigA Clone() => FromJson<XRSpaceConfigA>(this.ToJson());

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        /// <param name="serializeContext">序列化上下文</param>
        public override void OnAfterDeserialize(ISerializeContext serializeContext)
        {
            base.OnAfterDeserialize(serializeContext);
            cameras.ForEach(ci => ci.config = this);
        }
    }

    /// <summary>
    /// 相机信息
    /// </summary>
    [Import]
    public class CameraInfo: ICustomEnumStringConverter
    {
        /// <summary>
        /// 配置
        /// </summary>
        internal XRSpaceConfigA config { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Category("信息")]
        [DisplayName("名称")]
        [Field(index = 1)]
        public string name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [Category("视口矩形")]
        [DisplayName("视口矩形")]
        [Field(index = 2)]
        [Json(exportString = true)]
        public RectF viewportRect { get => _viewportRect; set => _viewportRect = value; }

        protected RectF _viewportRect = new RectF(0, 0, 1, 1);

        #region 视口矩形

        [Category("视口矩形")]
        [DisplayName("X")]
        [Field(ignore = true)]
        [Json(false)]
        public float viewportRect_x { get => viewportRect.x; set => _viewportRect.x = MathX.Clamp01(value); }

        [Category("视口矩形")]
        [DisplayName("Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float viewportRect_y { get => viewportRect.y; set => _viewportRect.y = MathX.Clamp01(value); }

        [Category("视口矩形")]
        [DisplayName("宽度")]
        [Field(ignore = true)]
        [Json(false)]
        public float viewportRect_width { get => viewportRect.width; set => _viewportRect.width = MathX.Clamp01(value); }

        [Category("视口矩形")]
        [DisplayName("高度")]
        [Field(ignore = true)]
        [Json(false)]
        public float viewportRect_height { get => viewportRect.height; set => _viewportRect.height = MathX.Clamp01(value); }

        #endregion

        /// <summary>
        /// 屏幕
        /// </summary>
        [Category("信息")]
        [DisplayName("屏幕")]
        [Field(index = 3)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        public string screen { get; set; } = "";

        #region 相机偏移

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose cameraOffset { get; set; } = new Pose();

        [Category("相机偏移")]
        [DisplayName("位置")]
        [Field(index = 4)]
        [Json(false)]
        public V3F cameraOffset_Position => cameraOffset.position;

        [Category("相机偏移")]
        [DisplayName("位置X")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Position_x { get => cameraOffset.position.x; set => cameraOffset._position.x = value; }

        [Category("相机偏移")]
        [DisplayName("位置Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Position_y { get => cameraOffset.position.y; set => cameraOffset._position.y = value; }

        [Category("相机偏移")]
        [DisplayName("位置Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Position_z { get => cameraOffset.position.z; set => cameraOffset._position.z = value; }

        [Category("相机偏移")]
        [DisplayName("旋转")]
        [Field(index = 5)]
        [Json(false)]
        public V3F cameraOffset_Rotation => cameraOffset.rotation;

        [Category("相机偏移")]
        [DisplayName("旋转X")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Rotation_x { get => cameraOffset.rotation.x; set => cameraOffset._rotation.x = value; }

        [Category("相机偏移")]
        [DisplayName("旋转Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Rotation_y { get => cameraOffset.rotation.y; set => cameraOffset._rotation.y = value; }

        [Category("相机偏移")]
        [DisplayName("旋转Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float cameraOffset_Rotation_z { get => cameraOffset.rotation.z; set => cameraOffset._rotation.z = value; }

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public CameraInfo Clone() => new CameraInfo().CopyDataFrom(this);

        public CameraInfo CopyDataFrom(CameraInfo cameraInfo)
        {
            if (cameraInfo != null)
            {
                this.config = cameraInfo.config;

                this.name = cameraInfo.name;
                this.viewportRect = cameraInfo.viewportRect;
                this.screen = cameraInfo.screen;
                this.cameraOffset = cameraInfo.cameraOffset.Clone();
            }
            return this;
        }

        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(screen):
                    {
                        if (config != null)
                        {
                            return config.screens.Cast(s => s.name).ToArray();
                        }
                        break;
                    }
            }
            return Empty<string>.Array;
        }
    }

    /// <summary>
    /// 屏幕信息
    /// </summary>
    [Import]
    public class ScreenInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Category("信息")]
        [DisplayName("名称")]
        [Field(index = 1)]
        public string name { get; set; } = "";

        /// <summary>
        /// 显示名
        /// </summary>
        [Category("信息")]
        [DisplayName("显示名")]
        [Field(index = 2)]
        public string displayName { get; set; } = "";

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        [Category("屏幕尺寸")]
        [DisplayName("尺寸")]
        [Field(index = 3)]
        [Json(exportString = true)]
        public V3F screenSize { get => _screenSize; set => _screenSize = value; }

        protected V3F _screenSize = new V3F(4, 2, 0.01f);

        [Category("屏幕尺寸")]
        [DisplayName("宽度")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenSize_x { get => screenSize.x; set => _screenSize.x = value; }

        [Category("屏幕尺寸")]
        [DisplayName("高度")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenSize_y { get => screenSize.y; set => _screenSize.y = value; }

        [Category("屏幕尺寸")]
        [DisplayName("厚度")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenSize_z { get => screenSize.z; set => _screenSize.z = value; }

        /// <summary>
        /// 屏幕姿态模式
        /// </summary>
        [Category("信息")]
        [DisplayName("屏幕姿态模式")]
        [ColumnHeader(100)]
        [Field(index = 4)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ScreenPoseMode screenPoseMode { get; set; } = ScreenPoseMode.ScreenPose;

        #region 屏幕姿态

        /// <summary>
        /// 屏幕姿态
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose screenPose { get; set; } = new Pose();

        /// <summary>
        /// 屏幕位置
        /// </summary>
        [Category("屏幕位置")]
        [DisplayName("位置")]
        [Description("屏幕位置,以米为单位")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F position => screenPose.position;

        [Category("屏幕位置")]
        [DisplayName("位置X")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Position_x { get => screenPose.position.x; set => screenPose._position.x = value; }

        [Category("屏幕位置")]
        [DisplayName("位置Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Position_y { get => screenPose.position.y; set => screenPose._position.y = value; }

        [Category("屏幕位置")]
        [DisplayName("位置Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Position_z { get => screenPose.position.z; set => screenPose._position.z = value; }

        /// <summary>
        /// 屏幕旋转
        /// </summary>
        [Category("屏幕旋转")]
        [DisplayName("旋转")]
        [Description("屏幕旋转欧拉角度,以度为单位")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F rotation => screenPose.rotation;

        [Category("屏幕旋转")]
        [DisplayName("旋转X")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Rotation_x { get => screenPose.rotation.x; set => screenPose._rotation.x = value; }

        [Category("屏幕旋转")]
        [DisplayName("旋转Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Rotation_y { get => screenPose.rotation.y; set => screenPose._rotation.y = value; }

        [Category("屏幕旋转")]
        [DisplayName("旋转Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenPose_Rotation_z { get => screenPose.rotation.z; set => screenPose._rotation.z = value; }

        #endregion

        #region 锚点关联

        /// <summary>
        /// 屏幕锚点关联信息
        /// </summary>
        [Category("锚点关联")]
        [DisplayName("屏幕锚点关联信息")]
        [ColumnHeader(100)]
        [Field(index = 5)]
        public ScreenAnchorLinkInfo screenAnchorLinkInfo { get; set; } = new ScreenAnchorLinkInfo();

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕")]
        [Field(ignore = true)]
        [Json(false)]
        public string standardScreen { get => screenAnchorLinkInfo.standardScreen; set => screenAnchorLinkInfo.standardScreen = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor standardScreenAnchor { get => screenAnchorLinkInfo.standardScreenAnchor; set => screenAnchorLinkInfo.standardScreenAnchor = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移空间类型")]
        [Field(ignore = true)]
        [Json(false)]
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType = value; }
      
        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F standardScreenAnchorOffset => screenAnchorLinkInfo.standardScreenAnchorOffset;

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移X")]
        [Field(ignore = true)]
        [Json(false)]
        public float standardScreenAnchorOffset_x { get => screenAnchorLinkInfo.standardScreenAnchorOffset.x; set => screenAnchorLinkInfo._standardScreenAnchorOffset.x = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float standardScreenAnchorOffset_y { get => screenAnchorLinkInfo.standardScreenAnchorOffset.y; set => screenAnchorLinkInfo._standardScreenAnchorOffset.y = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float standardScreenAnchorOffset_z { get => screenAnchorLinkInfo.standardScreenAnchorOffset.z; set => screenAnchorLinkInfo._standardScreenAnchorOffset.z = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor screenAnchor { get => screenAnchorLinkInfo.screenAnchor; set => screenAnchorLinkInfo.screenAnchor = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移空间类型")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.screenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.screenAnchorOffsetSpaceType = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F screenAnchorOffset => screenAnchorLinkInfo.screenAnchorOffset;

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移X")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenAnchorOffset_x { get => screenAnchorLinkInfo.screenAnchorOffset.x; set => screenAnchorLinkInfo._screenAnchorOffset.x = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenAnchorOffset_y { get => screenAnchorLinkInfo.screenAnchorOffset.y; set => screenAnchorLinkInfo._screenAnchorOffset.y = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenAnchorOffset_z { get => screenAnchorLinkInfo.screenAnchorOffset.z; set => screenAnchorLinkInfo._screenAnchorOffset.z = value; }

        [Category("锚点关联-旋转")]
        [DisplayName("关联旋转")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F linkRotation => screenAnchorLinkInfo.linkRotation;

        [Category("锚点关联-旋转")]
        [DisplayName("关联旋转X")]
        [Field(ignore = true)]
        [Json(false)]
        public float linkRotation_x { get => screenAnchorLinkInfo.linkRotation.x; set => screenAnchorLinkInfo._linkRotation.x = value; }

        [Category("锚点关联-旋转")]
        [DisplayName("关联旋转Y")]
        [Field(ignore = true)]
        [Json(false)]
        public float linkRotation_y { get => screenAnchorLinkInfo.linkRotation.y; set => screenAnchorLinkInfo._linkRotation.y = value; }

        [Category("锚点关联-旋转")]
        [DisplayName("关联旋转Z")]
        [Field(ignore = true)]
        [Json(false)]
        public float linkRotation_z { get => screenAnchorLinkInfo.linkRotation.z; set => screenAnchorLinkInfo._linkRotation.z = value; }

        #endregion

        [DisplayName("有效性")]
        [Field(index = 10)]
        [Json(false)]
        public bool valid
        {
            get
            {
                switch (screenPoseMode)
                {
                    case ScreenPoseMode.ScreenPose: return true;
                    case ScreenPoseMode.AnchorLink: return !string.IsNullOrEmpty(screenAnchorLinkInfo.standardScreen);
                }
                return false;
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ScreenInfo Clone() => new ScreenInfo().CopyDataFrom(this);

        public ScreenInfo CopyDataFrom(ScreenInfo screenInfo)
        {
            if (screenInfo != null)
            {
                this.name = screenInfo.name;
                this.displayName = screenInfo.displayName;
                this.screenSize = screenInfo.screenSize;
                this.screenPoseMode = screenInfo.screenPoseMode;
                this.screenPose = screenInfo.screenPose.Clone();
                this.screenAnchorLinkInfo = screenInfo.screenAnchorLinkInfo.Clone();
            }
            return this;
        }
    }

    /// <summary>
    /// 屏幕姿态模式
    /// </summary>
    public enum ScreenPoseMode
    {
        /// <summary>
        /// 场景姿态
        /// </summary>
        [Name("场景姿态")]
        ScreenPose,

        /// <summary>
        /// 锚点关联
        /// </summary>
        [Name("锚点关联")]
        AnchorLink,
    }

    /// <summary>
    /// 场景姿态
    /// </summary>
    public class Pose
    {
        /// <summary>
        /// 位置
        /// </summary>
        [Json(exportString = true)]
        public V3F position { get => _position; set => _position = value; }

        internal V3F _position = new V3F();

        /// <summary>
        /// 旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F rotation { get => _rotation; set => _rotation = value; }

        internal V3F _rotation = new V3F();

        public Pose Clone() => new Pose().CopyDataFrom(this);

        public Pose CopyDataFrom(Pose pose)
        {
            if(pose!=null)
            {
                this.position = pose.position;
                this.rotation = pose.rotation;
            }
            return pose;
        }
    }

    /// <summary>
    /// 场景关联锚点信息
    /// </summary>
    [Import]
    public class ScreenAnchorLinkInfo
    {
        /// <summary>
        /// 标准屏幕
        /// </summary>
        public string standardScreen { get; set; } = "";

        /// <summary>
        /// 标准屏幕锚点
        /// </summary>
        public ERectAnchor standardScreenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 标准屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F standardScreenAnchorOffset { get => _standardScreenAnchorOffset; set => _standardScreenAnchorOffset = value; }

        internal V3F _standardScreenAnchorOffset = new V3F();

        /// <summary>
        /// 标准屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        public ERectAnchor screenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F screenAnchorOffset { get => _screenAnchorOffset; set => _screenAnchorOffset = value; }

        internal V3F _screenAnchorOffset = new V3F();

        /// <summary>
        /// 屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 关联旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F linkRotation { get => _linkRotation; set => _linkRotation = value; }

        internal V3F _linkRotation = new V3F();

        public override string ToString()
        {
            return string.Format("{0}.{1}->{2}->当前.{3}", standardScreen, NameCache.Get(standardScreenAnchor), (string)linkRotation, NameCache.Get(screenAnchor));
        }

        public ScreenAnchorLinkInfo Clone() => new ScreenAnchorLinkInfo().CopyDataFrom(this);

        public ScreenAnchorLinkInfo CopyDataFrom(ScreenAnchorLinkInfo screenAnchorLinkInfo)
        {
            if (screenAnchorLinkInfo != null)
            {
                this.standardScreen = screenAnchorLinkInfo.standardScreen;
                this.standardScreenAnchor = screenAnchorLinkInfo.standardScreenAnchor;
                this.standardScreenAnchorOffset = screenAnchorLinkInfo.standardScreenAnchorOffset;
                this.standardScreenAnchorOffsetSpaceType = screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType;

                this.screenAnchor = screenAnchorLinkInfo.screenAnchor;
                this.screenAnchorOffset = screenAnchorLinkInfo.screenAnchorOffset;
                this.screenAnchorOffsetSpaceType = screenAnchorLinkInfo.screenAnchorOffsetSpaceType;

                this.linkRotation = screenAnchorLinkInfo.linkRotation;
            }
            return this;
        }
    }

    /// <summary>
    /// 锚点偏移空间类型
    /// </summary>
    public enum EAnchorOffsetSpaceType
    {
        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World=0,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local,
    }
         
}
