using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Controllers
{
    [RequireManager(typeof(ToolsManager))]
    public abstract class ToolController : ToolMB
    {
    }
}
