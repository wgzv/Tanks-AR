﻿using UnityEngine;
using UnityEngine.UI;

namespace XCSJ.PluginSMS.States.Show.UI
{
    [RequireComponent(typeof(Text))]
    public class GUICurrentStepIndex : GUIStepGroupInfo
    {
        private Text text = null;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            text = GetComponent<Text>();
        }

        /// <summary>
        /// 当步骤变化时回调
        /// </summary>
        /// <param name="group"></param>
        protected override void OnStepChanged(StepGroup group)
        {
            if (!group) return;

            var selectedIndex = StepGroupHelper.FindSelectedIndexInGlobal(group);
            if (selectedIndex > 0)
            {
                text.text = selectedIndex.ToString();
            }
        }
    }
}
