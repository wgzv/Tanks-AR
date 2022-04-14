using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.PluginHoloLens
{
    public enum EFocus
    {
        [Name("进入")]
        Enter,

        [Name("退出")]
        Exit,
    }

    public enum EClick
    {
        [Name("按下")]
        Down,

        [Name("弹起")]
        Up,

        [Name("点击")]
        Click,
    }
}
