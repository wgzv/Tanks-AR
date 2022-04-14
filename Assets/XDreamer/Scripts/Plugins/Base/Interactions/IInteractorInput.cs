using XCSJ.Algorithms;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Interactions
{
    /// <summary>
    /// 交互器输入接口：用于描述交互时具体的交互方式或方法
    /// </summary>
    public interface IInteractorInput
    {
        /// <summary>
        /// 用途：交互方式的描述
        /// </summary>
        string usage { get; set; }
    }

    /// <summary>
    /// 可交互上下文接口：用于描述交互时的上下文环境，即在何种环境下执行交互；
    /// </summary>
    public interface IInteractableContext : IContext { }

    /// <summary>
    /// 可交互上下文
    /// </summary>
    public class InteractableContext : Context, IInteractableContext
    {
        /// <summary>
        /// 默认
        /// </summary>
        public static InteractableContext Default => Default<InteractableContext>.Instance;
    }
}
