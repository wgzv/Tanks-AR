using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditor.Web;
using XCSJ.EditorExtension.EditorWindows.Web;
using XCSJ.EditorExtension.XAssets.Manuals;
using XCSJ.EditorTools.Windows.RichTexts;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets
{
#if UNITY_2020_1_OR_NEWER//在Unity2020.1版本中资源商店被使用网页与包管理器替代，同时Unity中本窗口类依赖的诸多底层类也被移除，那么本类没有存在的意义了！

    [Name(Title)]
    [Tip("用于访问XDreamer相关资源的窗口")]
    [XCSJ.Attributes.Icon(EIcon.Store)]
    public sealed class XAssetStoreWindow : EditorWindow<XAssetStoreWindow>
    {
        public const string Title = "资源商店";

        [MenuItem(XDreamerMenu.NamePath + Product.Name + "手册", priority = 8001)]
        public static void InitManual() => OpenURL(XAssetStore.ManualPageOnline);

        public static void OpenURL(string url) => Application.OpenURL(url);

        public static void OpenManual(MemberInfo member, bool indexOrApi = true)
        {
            member = member != null ? (member is Type ? member : member.ReflectedType) : null;
            if (CommonFun.NetIsOK())
            {
                OpenURL(ManualHelper.ToPageUrl(XAssetStore.ManualPageOnline, ManualHelper.GetValidPathOfManualInAssets(member, indexOrApi)));
            }
            else
            {
                Debug.LogError("网络无法访问，在线手册无法打开！");
            }
        }
        public static void OpenManual(object obj, bool indexOrApi = true)
        {
            if (Equals(obj, null)) return;
            //Debug.Log("OpenManual:" + obj);
            if (obj is MemberInfo memberInfo)
            {
                OpenManual(memberInfo, indexOrApi);
            }
            else
            {
                OpenManual(obj.GetType(), indexOrApi);
            }
        }
        public static void OpenManual(object obj, object tag) => OpenManual(obj, tag is bool b ? b : true);

        public override void OnGUI()
        {
            if (UICommonFun.NotificationButtonLayout("请使用浏览器替代"))
            {
                OpenURL(Product.WebSite);
            }
        }
    }

#else

    [Name(Title)]
    [Tip("用于访问XDreamer相关资源的窗口")]
    [XCSJ.Attributes.Icon(EIcon.Store)]
    //[XDreamerEditorWindow("常用", index = 1)]
    public sealed class XAssetStoreWindow : WebWiewWindow<XAssetStoreWindow>
    {
        public const string Title = "资源商店";

        //[MenuItem(XDreamerMenu.NamePath + Product.Name + Title, priority = 8000)]
        //public static void Init() => OpenAndFocus();

        [MenuItem(XDreamerMenu.NamePath + Product.Name + "手册", priority = 8001)]
        public static void InitManual() => OpenManual();

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
            //Init();
            //if (instance.webView)
            //{
            //    instance.Load(url);
            //}
            //else
            //{
            //    instance.willOpenUri = url;
            //}
        }
        public static void OpenManual()
        {
            if (XAssetStoreOption.weakInstance.onlineManualFirst && CommonFun.NetIsOK())
            {
                OpenURL(XAssetStore.ManualPageOnline);
            }
            else
            {
                OpenURL(XAssetStore.ManualPage);
            }
        }
        public static void OpenManual(MemberInfo member, bool indexOrApi = true)
        {
            member = member != null ? (member is Type ? member : member.ReflectedType) : null;
            if (XAssetStoreOption.weakInstance.onlineManualFirst && CommonFun.NetIsOK())
            {
                OpenURL(ManualHelper.ToPageUrl(XAssetStore.ManualPageOnline, ManualHelper.GetValidPathOfManualInAssets(member, indexOrApi)));
            }
            else
            {
                OpenURL(ManualHelper.ToPageUrl(ManualHelper.ManualPage, ManualHelper.GetValidPathOfManualInAssets(member, indexOrApi)));
            }
        }
        public static void OpenManual(object obj, bool indexOrApi = true)
        {
            if (Equals(obj, null)) return;
            //Debug.Log("OpenManual:" + obj);
            if (obj is MemberInfo memberInfo)
            {
                OpenManual(memberInfo, indexOrApi);
            }
            else
            {
                OpenManual(obj.GetType(), indexOrApi);
            }
        }
        public static void OpenManual(object obj, object tag)
        {
            OpenManual(obj, tag is bool b ? b : true);
        }

    #region 按钮GUI内容

        /// <summary>
        /// 向后导航
        /// </summary>
        [Name("向后")]
        [Tip("向后导航")]
        [XCSJ.Attributes.Icon(EIcon.Backward)]
        public static XGUIContent backwordGUIContent { get; } = GetXGUIContent(nameof(backwordGUIContent));

        /// <summary>
        /// 向前导航
        /// </summary>
        [Name("向前")]
        [Tip("向前导航")]
        [XCSJ.Attributes.Icon(EIcon.Forward)]
        public static XGUIContent forwordGUIContent { get; } = GetXGUIContent(nameof(forwordGUIContent));

        /// <summary>
        /// 重新加载
        /// </summary>
        [Name("重新加载")]
        [Tip("'点击'重新加载此页面\n'Ctrl+点击'强制重新加载此页面")]
        [XCSJ.Attributes.Icon(EIcon.Reset)]
        public static XGUIContent reloadGUIContent { get; } = GetXGUIContent(nameof(reloadGUIContent));

        /// <summary>
        /// 跳转
        /// </summary>
        [Name("跳转")]
        [Tip("跳转")]
        [XCSJ.Attributes.Icon(EIcon.Goto)]
        public static XGUIContent gotoGUIContent { get; } = GetXGUIContent(nameof(gotoGUIContent));

        /// <summary>
        /// 主页
        /// </summary>
        [Name("主页")]
        [Tip("主页")]
        [XCSJ.Attributes.Icon(EIcon.Home)]
        public static XGUIContent homeGUIContent { get; } = GetXGUIContent(nameof(homeGUIContent));

        /// <summary>
        /// 外部打开
        /// </summary>
        [Name("外部打开")]
        [Tip("外部打开")]
        [XCSJ.Attributes.Icon(EIcon.Open)]
        public static XGUIContent openGUIContent { get; } = GetXGUIContent(nameof(openGUIContent));

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(XAssetStoreWindow), propertyName, true);

    #endregion

    #region EditorWindow重载函数

        public override void OnEnable()
        {
            base.OnEnable();

            OnOptionModified(XAssetStoreOption.weakInstance);
            XAssetStoreOption.onModified += OnOptionModified;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            XAssetStoreOption.onModified -= OnOptionModified;
        }

        public override void OnGUI()
        {
            DrawTopMenu();
            DrawWebView(GUIClip.Unclip(new Rect(0f, menuHeight, base.position.width, base.position.height - menuHeight)));
        }

        public XAssetStoreOption assetStoreOption { get; private set; }

        private void OnOptionModified(XAssetStoreOption option)
        {
            assetStoreOption = option;
            OnInitWebView();
            Repaint();
        }

    #endregion

    #region 顶部菜单

        private float menuHeight = 30;

        private void DrawTopMenu()
        {
            EditorGUI.BeginChangeCheck();

            var h = GUILayout.Height(menuHeight);
            EditorGUILayout.BeginHorizontal(GUI.skin.box, h);

            var style = XGUIStyleLib.Get(EGUIStyle.Button_NoneBackground);

            GUILayoutOption[] buttonOption = new GUILayoutOption[] { GUILayout.Width(24), GUILayout.Height(20) };

            if (GUILayout.Button(backwordGUIContent, style, buttonOption))
            {
                webView.Back();
            }
            if (GUILayout.Button(forwordGUIContent, style, buttonOption))
            {
                webView.Forward();
            }
            if (GUILayout.Button(reloadGUIContent, style, buttonOption))
            {
                if (Event.current.control)
                {
                    ForceReload();
                }
                else
                {
                    Reload();
                }
            }

            for (int i = 0; i < assetStoreOption.addresses.Count; i++)
            {
                var address = assetStoreOption.addresses[i];

                if (address.enable)
                {
                    if (string.IsNullOrEmpty(address.iconName))
                    {
                        if (EditorRichText.Button(CommonFun.TempContent(address.name, address.address)))
                        {
                            Load(address.GetValidAddress());
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(CommonFun.TempContent(EditorIconHelper.GetIconInLib(address.iconName), address.name), style, buttonOption))
                        {
                            Load(address.GetValidAddress());
                        }
                    }
                }
            }

            if (assetStoreOption.displayAddress)
            {
                EditorGUI.BeginChangeCheck();
                var newUrl = EditorGUILayout.DelayedTextField(uri, GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck())
                {
                    Load(newUrl);
                }
                if (GUILayout.Button(gotoGUIContent, style, buttonOption))
                {
                    Load(newUrl);
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button(openGUIContent, style, buttonOption))
            {
                if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri uriObj) && uriObj.IsFile)
                {
                    //对URL尝试做离线手册情况的分析
                    var mainPageUrl = ManualHelper.GetMainPageUrl(uri);
                    if (mainPageUrl != uri)
                    {
                        //打开主页
                        Application.OpenURL(mainPageUrl);

                        //尝试打开子页面，主要是手册的具体页面；
                        if (ManualHelper.GetSubPageUrl(uri) is string subPageUrl && !string.IsNullOrEmpty(subPageUrl))
                        {
                            Application.OpenURL(subPageUrl);
                        }
                    }
                }
                else
                {
                    //直接打开
                    Application.OpenURL(uri);
                }
            }
            //if (EditorRichText.Button("打开文件所在文件夹"))
            //{
            //    SetScriptObject();
            //    XAssetStoreWindowContent.instance.Test();
            //    //EditorUtility.RevealInFinder(XAssetStore.ToPageUrl(url));
            //}
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl("");
            }
        }

    #endregion

    #region WebView

        public string uri = "";

        private string willOpenUri = null;

        public string mainPage => CommonFun.NetIsOK() ? XAssetStore.MainPageOnline : XAssetStore.MainPage;

        public void Load(string uri)
        {
            this.uri = uri;
            webView.Load(uri);
        }

        /// <summary>
        /// 初始化打开的Uri字符串
        /// </summary>
        protected override string initialOpenUri
        {
            get
            {
                var initUri = mainPage;
                if (!string.IsNullOrEmpty(willOpenUri))
                {
                    initUri = willOpenUri;
                    willOpenUri = null;
                }
                else if (!string.IsNullOrEmpty(uri))
                {
                    initUri = uri;
                }
                return initUri;
            }
        }

        /// <summary>
        /// 强制重新加载此页面后回调，打开之前的页面
        /// </summary>
        protected override void OnForceReload()
        {
            UICommonFun.DelayCall(() =>
            {
                //Debug.Log("OnForceReload: " + uri);
                OpenURL(uri);
            });
        }

        /// <summary>
        /// 执行InitWebView后的回调函数,对WebView对象做额外的初始化
        /// </summary>
        protected override void OnInitWebView()
        {
            if (webView)
            {
                webView.AllowRightClickMenu(assetStoreOption.allowRightClickMenu);
            }
        }

        /// <summary>
        /// <p>WebView在执行LoadURL,LoadFile时底层上自动调用本函数</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        public override void OnInitScripting()
        {
            UICommonFun.DelayCall(() =>
            {
                XAssetStoreWindowTestContent.instance.Test();
            }, 1);

            base.OnInitScripting();
        }

        /// <summary>
        /// <p>WebView加载无效或错误页面时回调</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">发生错误的Url</param>
        public override void OnLoadError(string url)
        {
            if (assetStoreOption.outputErrorUrl)
            {
                Debug.LogError("加载URL错误:" + url);
            }
        }

        /// <summary>
        /// <p>WebView的定位路径发生变化时回调；可捕获框架内子页面的URL</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">定位后的Url</param>
        public override void OnLocationChanged(string url)
        {
            //Debug.Log("OnLocationChanged: " + url);
            HandleManualUri(url);
            Repaint();
        }

    #endregion

    #region 手册处理

        /// <summary>
        /// 仅针对离线手册的Uri做特殊处理
        /// </summary>
        /// <param name="uri">待处理的Uri字符串</param>
        private void HandleManualUri(string uri)
        {
            try
            {
                Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri uriObj);
                var manualUrl = uriObj.IsFile ? XAssetStore.ManualPage : XAssetStore.ManualPageOnline;
                if (Uri.TryCreate(manualUrl, UriKind.RelativeOrAbsolute, out Uri manualUri))
                {
                    if (uriObj.ToString().StartsWith(manualUri.ToString()))
                    {
                        this.uri = uriObj.ToString();
                        return;
                    }

                    var index = manualUrl.LastIndexOf("/");
                    var manualUrlDir = manualUrl.Substring(0, index > 0 ? index + 1 : manualUrl.Length);
                    Uri.TryCreate(manualUrlDir, UriKind.RelativeOrAbsolute, out Uri manualUrlDirUri);

                    if (uriObj.ToString().StartsWith(manualUrlDirUri.ToString()))
                    {
                        var relative = manualUrlDirUri.MakeRelativeUri(uriObj).ToString();

                        this.uri = ManualHelper.ToPageUrl(manualUri.ToString(), relative);
                        UpdateManualSubPage(relative);
                        return;
                    }
                }
                //需要忽略的协议
                if (uriObj.Scheme == "about")
                {
                    //
                }
                else
                {
                    this.uri = uriObj.ToString();
                }
            }
            catch { }
            finally
            {
                this.uri = this.uri.Replace("file:///", "");
            }
        }

        private void UpdateManualSubPage(string id)
        {
            //Debug.Log("UpdateManualSubPage: " + id);
            InvokeJSMethod("window", "re_load_id", id);
        }

    #endregion
    }

    /// <summary>
    /// XDreamer资产商店测试内容；用于测试JS调用C#；
    /// </summary>
    public class XAssetStoreWindowTestContent : InstanceClass<XAssetStoreWindowTestContent>
    {
        internal void Test()
        {
            JSProxyMgr.GetInstance().AddGlobalObject(nameof(XAssetStoreWindowTestContent), XAssetStoreWindowTestContent.instance);
        }

        public void Test0()
        {
            Debug.Log("Test0");
        }

        public void Test1(string value)
        {
            Debug.Log("Test1: " + value);
        }

        public static void STest0()
        {
            Debug.Log("sTest0");
        }

        public static void STest1(string value)
        {
            Debug.Log("sTest1: " + value);
        }
    }

#endif

}
