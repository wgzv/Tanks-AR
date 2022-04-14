using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.PluginMMO
{
    [RequireManager(typeof(MMOManager))]
    [Name("网络玩家起始位置")]
    [Tip("将当前游戏对象的变换加入MMO玩家生成器的玩家起始位置列表")]
    [DisallowMultipleComponent]
    public sealed class NetPlayerStartPosition : MB, IAwake, IOnDestroy
    {
        public void Awake()
        {
            var creater = MMOPlayerCreater.instance;
            if (creater)
            {
                creater.AddStartPosition(transform);
            }
        }

        public void OnDestroy()
        {
            var creater = MMOPlayerCreater.instance;
            if (creater)
            {
                creater.RemoveStartPosition(transform);
            }
        }
    }
}
