namespace XCSJ.PluginPeripheralDevice
{
    public enum ButtonPressState
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,

        /// <summary>
        /// 按下
        /// </summary>
        Pressed,

        /// <summary>
        /// 持续按下
        /// </summary>
        Pressing,

        /// <summary>
        /// 释放
        /// </summary>
        Released,
    }

    /// <summary>
    /// 基础上数据
    /// </summary>
    public class BaseData
    {
        /// <summary>
        /// 数据名称
        /// </summary>
        protected string _name;

        /// <summary>
        /// BaseData构造函数
        /// </summary>
        /// <param name="name">名称</param>
        public BaseData(string name)
        {
            this._name = name;
        }
    }

    /// <summary>
    /// 按钮数据
    /// </summary>
    public class ButtonData : BaseData
    {
        /// <summary>
        /// 按钮状态
        /// </summary>
        public ButtonPressState buttonPressState;

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string buttonName { get => _name; set => _name = value; }

        /// <summary>
        /// ButtonData构造函数
        /// </summary>
        /// <param name="btnName"></param>
        public ButtonData(string btnName) : base(btnName)
        {
            this.buttonPressState = ButtonPressState.None;
        }

        /// <summary>
        /// 处理按钮状态
        /// </summary>
        public void ProcessData()
        {
            switch(buttonPressState)
            {
                case ButtonPressState.Pressed:
                    buttonPressState = ButtonPressState.Pressing;
                    break;
                case ButtonPressState.Released:
                    buttonPressState = ButtonPressState.None;
                    break;
                default:
                    break;
            }
        }
    }
    
    /// <summary>
    /// 按键数据
    /// </summary>
    public class KeyData : ButtonData
    {
        /// <summary>
        /// 按键名称
        /// </summary>
        public string keyName { get => _name; set => _name = value; }

        /// <summary>
        /// KeyData构造函数
        /// </summary>
        /// <param name="keyName"></param>
        public KeyData(string keyName) : base(keyName)
        {
            this.buttonPressState = ButtonPressState.None;
        }
    }

    /// <summary>
    /// 轴数据
    /// </summary>
    public class AxisData: BaseData
    {
        /// <summary>
        /// 轴名称
        /// </summary>
        public string axisName { get => _name; set => _name = value; }

        /// <summary>
        /// 轴的值
        /// </summary>
        public float axisValue;

        /// <summary>
        /// AxisData构造函数
        /// </summary>
        /// <param name="axisName"></param>
        public AxisData(string axisName) : base(axisName)
        {

        }
    }
}
