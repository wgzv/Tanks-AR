﻿using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO.Tests
{
    /// <summary>
    /// 测试:用于输出MMO网络环境下的各种命令与数据信息
    /// </summary>
    [Name("测试")]
    [Tip("用于输出MMO网络环境下的各种命令与数据信息")]
    [RequireManager(typeof(MMOManager))]
    public class Test : MB, IOnEnable, IOnDisable
    {
        private static void OnCmd(Cmd cmd)
        {
            Log.DebugFormat("OnCmd 用户GUID:{0}=命令={1}=数据={2}", cmd.userGuid, cmd.cmd, cmd.data);
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            var manager = MMOManager.instance;
            if (!manager) return;
            manager.cmdHandler.Register(OnCmd);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            var manager = MMOManager.instance;
            if (!manager) return;

            manager.cmdHandler.Unregister(OnCmd);
        }
    }
}
