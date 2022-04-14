using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginPeripheralDevice;
using UInput = UnityEngine.Input;

namespace XCSJ.PluginPeripheralDevice.Standalones
{
    /// <summary>
    /// 平台输入，即一般的鼠标键盘输入
    /// </summary>
    public class StandaloneInput : BaseInput
    {
        #region IInfo
        /// <summary>
        /// 继承的类，直接命名，表述输入的类型
        /// </summary>
        public override string deviceName  => "鼠标键盘"; 

        #endregion IInfo

        /// <summary>
        /// Awake函数，唤醒时执行
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 处理函数
        /// </summary>
        public override void Process()
        {
            base.Process();
            
        }

        /// <summary>
        /// 是否支持
        /// </summary>
        /// <returns></returns>
        public override bool IsInputSupported()
        {
            return UInput.mousePresent;
        }

    }
}
