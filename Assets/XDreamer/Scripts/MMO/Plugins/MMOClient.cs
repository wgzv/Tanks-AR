using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO
{
    /// <summary>
    /// MMO客户端
    /// </summary>
    public class MMOClient : CrossPlatformTcpClient, IMMOClient
    {
        /// <summary>
        /// MMO管理器对象
        /// </summary>
        public MMOManager manager { get; private set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="manager"></param>
        public MMOClient(MMOManager manager)
        {
            if (!manager) throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
        }
    }
}
