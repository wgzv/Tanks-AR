using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据图像:将天气数据显示到图像中
    /// </summary>
    [Name("天气数据图像")]
    [Tip("将天气数据显示到图像中")]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class WeatherDataImage : WeatherDataUI
    {
        /// <summary>
        /// 图像:期望显示天气数据的图像对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的的组件；
        /// </summary>
        [Name("图像")]
        [Tip("期望显示天气数据的图像对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的的组件；")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Image image;

        /// <summary>
        /// 默认精灵:当天气数据与数据精灵列表无任何数据匹配时，图像上的显示精灵会修改为本参数对应的精灵；
        /// </summary>
        [Name("默认精灵")]
        [Tip("当天气数据与数据精灵列表无任何数据匹配时，图像上的显示精灵会修改为本参数对应的精灵；")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Sprite defaultSprite;

        /// <summary>
        /// 数据精灵列表:当天气数据与本列表中某项元素的数据一致时，图像上的显示精灵会修改为该数据对应的精灵；
        /// </summary>
        [Name("数据精灵列表")]
        [Tip("当天气数据与本列表中某项元素的数据一致时，图像上的显示精灵会修改为该数据对应的精灵；")]
        public List<DataSpritePair> dataTexturePairs = new List<DataSpritePair>();

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (!image)
            {
                image = GetComponent<Image>();
            }
        }

        private bool TryGetSprite(out Sprite sprite)
        {
            if (TryGetData(out string data))
            {
                sprite = dataTexturePairs.FirstOrDefault(pair => pair.data == data)?.sprite;
                if (!sprite)
                {
                    sprite = defaultSprite;
                }
                return true;
            }
            sprite = null;
            return false;
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        protected override void UpdateUI()
        {
            if (!image) return;
            if (TryGetSprite(out Sprite sprite))
            {
                image.sprite = sprite;
            }
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// 数据精灵对
    /// </summary>
    [Name("数据精灵对")]
    [Serializable]
    public class DataSpritePair
    {
        /// <summary>
        /// 数据
        /// </summary>
        [Name("数据")]
        public string data = "";

        /// <summary>
        /// 精灵
        /// </summary>
        [Name("精灵")]
        public Sprite sprite;
    }
}
