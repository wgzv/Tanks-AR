using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// 自定义参数枚举;在Animator中已经自定义的参数名枚举
    /// </summary>
    [Name("自定义参数枚举")]
    public enum ECustomParameter
    {
        [Name("前进")]
        [Tip("用于控制角色的移动；类型：Float")]
        Forward,

        [Name("转向")]
        [Tip("用于控制角色的旋转；类型：Float")]
        Turn,

        [Name("下蹲")]
        [Tip("用于控制角色的下蹲；类型：Bool")]
        Crouch,

        [Name("地面上")]
        [Tip("控制角色是否在地面；类型：Bool")]
        OnGround,

        [Name("跳跃")]
        [Tip("控制角色是否跳跃；类型：Float")]
        Jump,

        [Name("跳跃腿")]
        [Tip("控制角色的腿；类型：Float")]
        JumpLeg
    }
}
