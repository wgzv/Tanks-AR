using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginDataBase.Base;
using XCSJ.PluginDataBase.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.States
{
    /// <summary>
    /// 执行SQL
    /// </summary>
    [Name(Title)]
    [Tip("执行SQL并等待执行完成")]
    [ComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class ExecuteSQL : Trigger<ExecuteSQL>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "执行SQL";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DBHelper.Title, typeof(DBManager))]
        [StateComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
        [Name(Title, nameof(ExecuteSQL))]
        [Tip("执行SQL并等待执行完成")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 数据库
        /// </summary>
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBMB _dbMB;

        /// <summary>
        /// 数据库
        /// </summary>
        public DBMB dbMB => this.XGetComponentInGlobal(ref _dbMB);

        /// <summary>
        /// Sql类型
        /// </summary>
        public enum ESqlType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 非查询
            /// </summary>
            [Name("非查询")]
            NonQuery,

            /// <summary>
            /// 查询
            /// </summary>
            [Name("查询")]
            Query,

            /// <summary>
            /// 条件查询
            /// </summary>
            [Name("条件查询")]
            ConditionQuery
        }

        /// <summary>
        /// Sql类型
        /// </summary>
        [Name("Sql类型")]
        [EnumPopup]
        public ESqlType _sqlType = ESqlType.Query;

        /// <summary>
        /// 非查询SQL:非查询SQL语句
        /// </summary>
        [Name("非查询SQL")]
        [Tip("非查询SQL语句")]
        [HideInSuperInspector(nameof(_sqlType), EValidityCheckType.NotEqual, ESqlType.NonQuery)]
        [OnlyMemberElements]
        public StringPropertyValue _nonQuerySql = new StringPropertyValue();

        /// <summary>
        /// 查询SQL:查询SQL语句
        /// </summary>
        [Name("查询SQL")]
        [Tip("查询SQL语句")]
        [HideInSuperInspector(nameof(_sqlType), EValidityCheckType.NotEqual, ESqlType.Query)]
        [OnlyMemberElements]
        public StringPropertyValue _querySql = new StringPropertyValue();

        /// <summary>
        /// 条件查询:条件查询
        /// </summary>
        [Name("条件查询")]
        [Tip("条件查询")]
        [HideInSuperInspector(nameof(_sqlType), EValidityCheckType.NotEqual, ESqlType.ConditionQuery)]
        [OnlyMemberElements]
        public ConditionQueryData _conditionQueryData = new ConditionQueryData();

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_sqlType)
            {
                case ESqlType.NonQuery: return _nonQuerySql.ToFriendlyString();
                case ESqlType.Query: return _querySql.ToFriendlyString();
                case ESqlType.ConditionQuery: return _conditionQueryData.ToFriendlyString();
            }
            return base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_dbMB) return false;
            switch (_sqlType)
            {
                case ESqlType.NonQuery: return _nonQuerySql.DataValidity();
                case ESqlType.Query: return _querySql.DataValidity();
                case ESqlType.ConditionQuery: return _conditionQueryData.DataValidity();
            }
            return base.DataValidity();
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            var dbMB = this.dbMB;
            if (!dbMB) return;
            switch (_sqlType)
            {
                case ESqlType.NonQuery:
                    {
                        dbMB.TryExecuteNonQuery(_nonQuerySql.GetValue(), (ir, r) =>
                        {
                            finished = true;
                        });
                        break;
                    }
                case ESqlType.Query:
                    {
                        dbMB.TryExecuteQuery(_querySql.GetValue(), (ir, rs) =>
                        {
                            finished = true;
                        });
                        break;
                    }
                case ESqlType.ConditionQuery:
                    {
                        dbMB.TryExecuteQuery(_conditionQueryData.GetSql(), (ir, rs) =>
                        {
                            finished = true;
                        });
                        break;
                    }
            }
        }
    }
}
