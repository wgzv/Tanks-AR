﻿using UnityEngine;
using UnityEngine.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginRepairman.UI
{
    [RequireComponent(typeof(Toggle))]
    public class GUIItemToggle : GUIItem
    {
        private Toggle toggle;

        protected override void Awake()
        {
            base.Awake();

            if (!toggle)
            {
                toggle = GetComponent<Toggle>();
            }
            if (toggle)
            {
                toggle.onValueChanged.AddListener(OnToggleValueChange);
                toggle.isOn = false;
            }
        }

        protected void OnDestroy()
        {
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChange);
            }
        }

        protected virtual void OnToggleValueChange(bool value)
        {
            if (item != null)
            {
                item.selected = value;
            }
        }

        protected void Update()
        {
            if (item != null && toggle && item.selected != toggle.isOn)
            {
                toggle.isOn = item.selected;
            }
        }

        protected override void SetIcon(Texture2D texture2D)
        {
            if (texture2D)
            {
                toggle.targetGraphic.gameObject.GetComponent<Image>().sprite = XGUIHelper.Texture2DToSprite(item.texture2D);
            }
        }
    }
}
