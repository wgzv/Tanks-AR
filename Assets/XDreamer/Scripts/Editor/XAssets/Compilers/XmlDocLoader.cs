using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    public static class XmlDocLoader
    {
        private static readonly Lazy<XmlDocumentationProvider> mscorlibDocumentation = new Lazy<XmlDocumentationProvider>(XmlDocLoader.LoadMscorlibDocumentation);

        //private static readonly ConditionalWeakTable<Module, XmlDocumentationProvider> cache = new ConditionalWeakTable<Module, XmlDocumentationProvider>();

        private static readonly Dictionary<Module, XmlDocumentationProvider> cache = new Dictionary<Module, XmlDocumentationProvider>();

        private static readonly string referenceAssembliesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Reference Assemblies\\Microsoft\\\\Framework");

        private static readonly string frameworkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET\\Framework");

        public static XmlDocumentationProvider MscorlibDocumentation
        {
            get
            {
                return XmlDocLoader.mscorlibDocumentation.Value;
            }
        }

        private static XmlDocumentationProvider LoadMscorlibDocumentation()
        {
            string text = XmlDocLoader.FindXmlDocumentation("mscorlib.dll", TargetRuntime.Net_4_0) ?? XmlDocLoader.FindXmlDocumentation("mscorlib.dll", TargetRuntime.Net_2_0);
            if (text != null)
            {
                return new XmlDocumentationProvider(text);
            }
            return null;
        }

        public static XmlDocumentationProvider LoadDocumentation(Module module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }
            lock (XmlDocLoader.cache)
            {
                XmlDocumentationProvider xmlDocumentationProvider = default(XmlDocumentationProvider);
                if (!XmlDocLoader.cache.TryGetValue(module, out xmlDocumentationProvider))
                {
                    string text = XmlDocLoader.LookupLocalizedXmlDoc(module.Assembly.Location);
                    if (text == null)
                    {
                        //text = XmlDocLoader.FindXmlDocumentation(Path.GetFileName(module.Assembly.Location), module.Runtime);
                    }
                    if (text != null)
                    {
                        xmlDocumentationProvider = new XmlDocumentationProvider(text);
                        XmlDocLoader.cache.Add(module, xmlDocumentationProvider);
                    }
                    else
                    {
                        XmlDocLoader.cache.Add(module, null);
                        xmlDocumentationProvider = null;
                    }
                }
                return xmlDocumentationProvider;
            }
        }

        private static string FindXmlDocumentation(string assemblyFileName, TargetRuntime runtime)
        {
            switch (runtime)
            {
                case TargetRuntime.Net_1_0:
                    return XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.frameworkPath, "v1.0.3705", assemblyFileName));
                case TargetRuntime.Net_1_1:
                    return XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.frameworkPath, "v1.1.4322", assemblyFileName));
                case TargetRuntime.Net_2_0:
                    return XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.frameworkPath, "v2.0.50727", assemblyFileName)) ?? XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.referenceAssembliesPath, "v3.5")) ?? XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.referenceAssembliesPath, "v3.0")) ?? XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.referenceAssembliesPath, ".NETFramework\\v3.5\\Profile\\Client"));
                default:
                    return XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.referenceAssembliesPath, ".NETFramework\\v4.0", assemblyFileName)) ?? XmlDocLoader.LookupLocalizedXmlDoc(Path.Combine(XmlDocLoader.frameworkPath, "v4.0.30319", assemblyFileName));
            }
        }

        private static string LookupLocalizedXmlDoc(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            string text = Path.ChangeExtension(fileName, ".xml");
            string twoLetterISOLanguageName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            string localizedName = XmlDocLoader.GetLocalizedName(text, twoLetterISOLanguageName);
            if (File.Exists(localizedName))
            {
                return localizedName;
            }
            if (File.Exists(text))
            {
                return text;
            }
            if (twoLetterISOLanguageName != "en")
            {
                string localizedName2 = XmlDocLoader.GetLocalizedName(text, "en");
                if (File.Exists(localizedName2))
                {
                    return localizedName2;
                }
            }
            return null;
        }

        private static string GetLocalizedName(string fileName, string language)
        {
            string directoryName = Path.GetDirectoryName(fileName);
            directoryName = Path.Combine(directoryName, language);
            return Path.Combine(directoryName, Path.GetFileName(fileName));
        }

        public static string RuntimeVersionString(TargetRuntime runtime)
        {
            switch (runtime)
            {
                case TargetRuntime.Net_1_0:
                    return "v1.0.3705";
                case TargetRuntime.Net_1_1:
                    return "v1.1.4322";
                case TargetRuntime.Net_2_0:
                    return "v2.0.50727";
                default:
                    return "v4.0.30319";
            }
        }

        public static void Init()
        {
            cache.Clear();
        }
    }

    public enum TargetRuntime
    {
        Net_1_0,
        Net_1_1,
        Net_2_0,
        Net_4_0
    }
}
