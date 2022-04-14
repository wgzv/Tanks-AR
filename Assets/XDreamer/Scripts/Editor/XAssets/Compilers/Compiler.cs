using System.IO;
using System.Xml;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    public class Compiler
    {
        public const string Project = nameof(Project);
        public const string PropertyGroup = nameof(PropertyGroup);
        public const string Condition = nameof(Condition);
        public const string OutputPath = nameof(OutputPath);
        public const string DocumentationFile = nameof(DocumentationFile);

        public const string Condition1 = "'Debug|AnyCPU'";
        public const string Condition2 = "'Release|AnyCPU'";

        public const string Xml = @".xml";
        public const string XmlOutputDir = @"Library\ScriptAssemblies\";

        public readonly static string[] Csprojs = new string[] { "Assembly-CSharp.csproj", "Assembly-CSharp-Editor.csproj", "Assembly-CSharp-Editor-firstpass.csproj", "Assembly-CSharp-firstpass.csproj" };

        public static void ModifyCsprojs(string dir, string documentationFileDir = XmlOutputDir)
        {
            foreach (var f in Csprojs)
            {
                ModifyCsprojOutputXml(Path.Combine(dir, f), documentationFileDir);
            }
        }

        public static bool ModifyCsprojOutputXml(string csprojFullPath, string documentationFileDir = XmlOutputDir)
        {
            try
            {
                if (!FileHelper.Exists(csprojFullPath)) return false;
                var value = documentationFileDir + Path.GetFileNameWithoutExtension(csprojFullPath) + Xml;

                XmlDocument xml = new XmlDocument();
                xml.Load(csprojFullPath);
                bool needSave = false;
                foreach (XmlNode node in xml[Project].ChildNodes)
                {
                    if (node.Name == PropertyGroup
                        && node.Attributes[Condition] is XmlAttribute xmlAttribute
                        && (xmlAttribute.Value.Contains(Condition1) || xmlAttribute.Value.Contains(Condition2)))
                    {
                        string xmlPath = null;
                        string outputPath = null;
                        foreach (XmlNode n in node.ChildNodes)
                        {
                            if (n.Name == OutputPath)
                            {
                                outputPath = n.InnerText;
                            }
                            else if (n.Name == DocumentationFile)
                            {
                                xmlPath = n.InnerText;
                            }
                        }
                        if (string.IsNullOrEmpty(xmlPath) && outputPath != null)
                        {
                            var docNode = xml.CreateElement(DocumentationFile, xml.DocumentElement.NamespaceURI);
                            docNode.InnerText = value;
                            node.AppendChild(docNode);
                            needSave = true;
                        }
                    }
                }
                if (needSave)
                {
                    xml.Save(csprojFullPath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
