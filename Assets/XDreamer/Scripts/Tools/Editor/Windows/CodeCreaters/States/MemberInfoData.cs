using System;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 成员信息数据
    /// </summary>
    public abstract class MemberInfoData : BasePropertyValueData
    {
        /// <summary>
        /// 成员信息
        /// </summary>
        public abstract MemberInfo memberInfo { get; }

        /// <summary>
        /// 是否是静态成员信息
        /// </summary>
        public abstract bool isStaticMemberInfo { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public override string name => memberInfo?.Name ?? "";

        /// <summary>
        /// 名称特性
        /// </summary>
        public override string nameAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_nameAttribute))
                {
                    _nameAttribute = GetNameAttributeText(memberInfo);
                }
                return _nameAttribute;
            }
            set => _nameAttribute = value;
        }

        /// <summary>
        /// 提示特性
        /// </summary>
        public override string tipAttribute
        {
            get
            {
                if (string.IsNullOrEmpty(_tipAttribute))
                {
                    var text = AttributeCache<TipAttribute>.Exist(memberInfo) ? CommonFun.Tip(memberInfo) : null;
                    _tipAttribute = string.IsNullOrEmpty(text) ? nameAttribute : text;
                }
                return _tipAttribute;
            }
            set => _tipAttribute = value;
        }

        /// <summary>
        /// 属性枚举名
        /// </summary>
        public override string propertyEnumName { get => name; set { } }

        /// <summary>
        /// 是否输出
        /// </summary>
        public bool output { get; set; } = true;

        /// <summary>
        /// 能否输出
        /// </summary>
        public virtual bool canOutput => output && canValid;

        /// <summary>
        /// 期望输出并且能输出
        /// </summary>
        public virtual bool wantOutput=> output && wantValid;

        /// <summary>
        /// 属性枚举名的值
        /// </summary>
        public int? propertyEnumNameValue { get; set; } = default;

        /// <summary>
        /// 设置属性枚举名的值
        /// </summary>
        /// <param name="value"></param>
        public void SetPropertyEnumNameValue(int? value)
        {
            propertyEnumNameValue = value;
        }

        /// <summary>
        /// 尝试设置属性枚举名的值，即如果没有值，则设置；
        /// </summary>
        /// <param name="value"></param>
        public void TrySetPropertyEnumNameValue(int? value)
        {
            if(!propertyEnumNameValue.HasValue)
            {
                propertyEnumNameValue = value;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="codeCreater"></param>
        public override void Init(BaseCodeCreater codeCreater)
        {
            base.Init(codeCreater);
            var type = memberInfo?.DeclaringType;
            displayPropertyEnumName = type != null && type == codeCreater.targetObjectType;
        }

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        public override void OnBeginCreateCode(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            //if (!wantOutput) return;
            codeCreater.AddUsedType(memberType, typeof(NameAttribute), typeof(TipAttribute), typeof(HideInSuperInspectorAttribute));
            base.OnBeginCreateCode(codeCreater, codeWirter);
        }

        /// <summary>
        /// 当生成代码枚举属性名称前回调
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        /// <param name="first">与当前对象类型相同的第一个对象；如果本参数对象与当前对象一致时，表示当前对象是期望输出的第一个当前类型的对象；</param>
        /// <param name="last">上次的对象，可能与当前对象类型不同</param>
        public virtual void Before_OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter, MemberInfoData first, MemberInfoData last) { }

        /// <summary>
        /// 有枚举区域
        /// </summary>
        public bool hasEnumRegion { get; set; } = false;

        /// <summary>
        /// 有枚举结束区域
        /// </summary>
        public bool hasEnumEndRegion { get; set; } = false;

        /// <summary>
        /// 当生成代码枚举属性名称
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        public virtual void OnCreateCode_Enum_EPropertyName(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            if(hasEnumRegion)
            {
                codeWirter.WriteRegion(propertyEnumFieldNameCategory);
                codeWirter.Write();
            }

            codeWirter.WriteSummaryFormat("{0}:{1}", nameAttributeWithType, tipAttribute);
            codeWirter.WriteFormat("[Name(\"{0}\")]", nameAttributeWithType);
            codeWirter.WriteFormat("[Tip(\"{0}\")]", tipAttribute);
            if (string.IsNullOrEmpty(propertyEnumFieldNameCategory))
            {
                codeWirter.WriteFormat("[EnumFieldName(\"{0}\")]", nameAttribute);
            }
            else
            {
                codeWirter.WriteFormat("[EnumFieldName(\"{0}/{1}\")]", propertyEnumFieldNameCategory, nameAttribute);
            }
            if (!displayPropertyEnumName)
            {
                codeWirter.WriteMacro_if(codeCreater.hideMacro);
                codeWirter.Write("[HideInSuperInspector]");
                codeWirter.WriteMacro_endif();
            }
            if (propertyEnumNameValue.HasValue)
            {
                codeWirter.WriteFormat("{0} = {1},", propertyEnumName, propertyEnumNameValue.Value);
                propertyEnumNameValue = default;//重置
            }
            else
            {
                codeWirter.WriteFormat("{0},", propertyEnumName);
            }
            codeWirter.Write();

            if(hasEnumEndRegion)
            {
                codeWirter.WriteEndRegion();
                codeWirter.Write();
            }
        }

        /// <summary>
        /// 当生成代码方法Execute
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public virtual bool OnCreateCode_Method_Execute(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter) => true;

        /// <summary>
        /// 当生成代码方法ToFriendlyString
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public virtual bool OnCreateCode_Method_ToFriendlyString(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            codeWirter.WriteFormat("return CommonFun.Name(_propertyName);");
            return false;
        }

        /// <summary>
        /// 当生成代码方法DataValidity
        /// </summary>
        /// <param name="codeCreater"></param>
        /// <param name="codeWirter"></param>
        /// <returns></returns>
        public virtual bool OnCreateCode_Method_DataValidity(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter) => true;
    }
}
