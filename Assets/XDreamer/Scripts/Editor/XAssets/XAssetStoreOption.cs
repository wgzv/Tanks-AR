using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets
{
    [XDreamerPreferences(index = IDRange.Begin + 50)]
    [Name(Product.Name + "-" + XAssetStoreWindow.Title)]
    [Import]
    public class XAssetStoreOption : XDreamerOption<XAssetStoreOption>
    {
        [Name("在线手册优先")]
#if UNITY_2020_1_OR_NEWER
        [Tip("因在Unity2020.1版本中资源商店被使用网页与包管理器替代，同时Unity中本窗口类依赖的诸多底层类也被移除,离线手册本地无法打开，本参数值将一直为True")]
        public bool onlineManualFirst => true;
#else
        [Tip("勾选,优先打开网络版本的手册；不勾选，打开本地手册，需要用户导入离线手册包；")]
        public bool onlineManualFirst = true;
#endif

        [Name("离线手册目录")]
        [Tip("相对Assets的路径或全路径")]
        public string offlineManualDirectory = "Assets/../../Docs/Manual";

        [Name("显示地址栏")]
        public bool displayAddress = true;

        [Name("输出错误URL")]
        public bool outputErrorUrl = false;

        [Name("允许右键菜单")]
        public bool allowRightClickMenu = true;

        [Name("地址列表")]
        public List<Address> addresses = new List<Address>();

        public Address address = new Address();

        public bool addressesExpanded = true;

        protected override void OnInit()
        {
            base.OnInit();
            InitIfNeed();
        }

        public override void OnModified()
        {
            base.OnModified();
            InitIfNeed();
        }

        public void InitIfNeed()
        {
            if (addresses.Count > 0) return;
            //直接调用版本变更事件，升级到最新的配置
            OnVersionChanged(0);
        }

        private void Add(string name, string address, string iconName = "")
        {
            addresses.Add(new Address(name, address, iconName));
        }

        public void Add(Address address)
        {
            if (address != null
                && !string.IsNullOrEmpty(address.name)
                && !addresses.Exists(a => a.name == address.name))
            {
                addresses.Add(new Address(address));
            }
        }

        #region 版本更新

        /// <summary>
        /// 最新的版本号
        /// </summary>
        protected override int newVersion => -4;
        protected override void OnVersionChanged(int lastVersion)
        {
            Init();
        }

        private void Init()
        {
            addresses.Clear();
            Add("主页", XAssetStore.MainPageOnline, EIcon.Home.ToString());
            Add("手册", XAssetStore.ManualPageOnline, EIcon.Manual.ToString());
            //Add("离线主页", UICommonFun.ToAssetsPath(XAssetStore.MainPage), EIcon.HomeOffline.ToString());
            //Add("离线手册", UICommonFun.ToAssetsPath(XAssetStore.ManualPage), EIcon.ManualOffline.ToString());
        }

        #endregion

        [Import]
        public class Address : Option
        {
            public string name = "";

            public string address = "";

            public string iconName = "";

            public string GetValidAddress() => UICommonFun.ToFullPath(address);

            public Address() { this.enable = true; }
            public Address(Address address) : this()
            {
                Set(address);
            }
            public Address(string name, string address, string iconName = "") : this()
            {
                Set(name, address, iconName);
            }

            public void Set(Address address)
            {
                this.name = address.name;
                this.address = address.address;
                this.expand = address.expand;
                this.enable = address.enable;
                this.active = address.active;
            }
            public void Set(string name, string address, string iconName = "")
            {
                this.name = name;
                this.address = address;
                this.iconName = iconName;
            }
        }
    }

    [CommonEditor(typeof(XAssetStoreOption))]
    public class XAssetStoreOptionEditor : XDreamerOptionEditor<XAssetStoreOption>
    {
        private const int IndexWidth = PluginCommonUtilsRootInspector.IndexWidth;
        private const int EnableWidth = UICommonOption._32;
        private const int NameWidth = UICommonOption._80;
        private const int TipWidth = UICommonOption._80;
        private const int OperationWidth = UICommonOption._24 * 5;

        private string[] filters = new string[] { "html", "html", "htm", "htm", "所有文件", "*" };

        private void SelectLocalAddress(ref string address)
        {
            UICommonFun.SelectFileButtonInAssets(ref address, CommonFun.TempContent("...", "点击选择文件"), EditorStyles.miniButtonRight, "选择文件", filters, GUILayout.Width(25));
        }

        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.addressesExpanded):
                case nameof(preference.address):
                    {
                        return true;
                    }
                case nameof(preference.addresses):
                    {
                        if (!(preference.addressesExpanded = UICommonFun.Foldout(preference.addressesExpanded, CommonFun.NameTip(preference, nameof(preference.addresses))))) return true;
                        CommonFun.BeginLayout();

                        XEditorGUI.DrawList(preference.addresses, () =>
                        {
                            GUILayout.Label("启用", GUILayout.Width(EnableWidth));
                            GUILayout.Label("名称", GUILayout.Width(NameWidth));
                            GUILayout.Label("图标名", GUILayout.Width(NameWidth));
                            GUILayout.Label("地址");
                        }, (a, i) =>
                        {
                            a.enable = EditorGUILayout.Toggle(a.enable, GUILayout.Width(EnableWidth));
                            EditorGUILayout.LabelField(CommonFun.TempContent(a.name, a.name), GUILayout.Width(NameWidth));
                            EditorGUILayout.LabelField(CommonFun.TempContent(a.iconName, a.iconName), GUILayout.Width(NameWidth));

                            a.address = EditorGUILayout.DelayedTextField(a.address);
                            SelectLocalAddress(ref a.address);
                        },
                        IndexWidth,
                        OperationWidth, UICommonOption.WH24x16);

                        #region 信息添加

                        UICommonFun.BeginHorizontal(false);
                        GUILayout.Label("信息", GUILayout.Width(IndexWidth));

                        var address = preference.address;
                        address.enable = EditorGUILayout.Toggle(address.enable, GUILayout.Width(EnableWidth));

                        address.name = EditorGUILayout.TextField(address.name, GUILayout.Width(NameWidth));
                        address.iconName = EditorGUILayout.TextField(address.iconName, GUILayout.Width(NameWidth));

                        address.address = EditorGUILayout.DelayedTextField(address.address);
                        SelectLocalAddress(ref address.address);

                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, GUILayout.Width(OperationWidth), UICommonOption.MaxHeight16))
                        {
                            preference.Add(address);
                        }
                        UICommonFun.EndHorizontal();

                        #endregion

                        CommonFun.EndLayout();

                        return true;
                    }
            }
            return base.OnGUI(obj, fieldInfo);
        }
    }
}
