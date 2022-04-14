using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    [Serializable]
    public class XmlDocumentationProvider : IDeserializationCallback
    {
        private sealed class XmlDocumentationCache
        {
            private readonly KeyValuePair<string, string>[] entries;

            private int pos;

            public XmlDocumentationCache(int size = 50)
            {
                if (size <= 0)
                {
                    throw new ArgumentOutOfRangeException("size", size, "Value must be positive");
                }
                this.entries = new KeyValuePair<string, string>[size];
            }

            internal bool TryGet(string key, out string value)
            {
                KeyValuePair<string, string>[] array = this.entries;
                for (int i = 0; i < array.Length; i++)
                {
                    KeyValuePair<string, string> keyValuePair = array[i];
                    if (keyValuePair.Key == key)
                    {
                        value = keyValuePair.Value;
                        return true;
                    }
                }
                value = null;
                return false;
            }

            internal void Add(string key, string value)
            {
                this.entries[this.pos++] = new KeyValuePair<string, string>(key, value);
                if (this.pos == this.entries.Length)
                {
                    this.pos = 0;
                }
            }
        }

        [Serializable]
        private struct IndexEntry : IComparable<IndexEntry>
        {
            internal readonly int HashCode;

            internal readonly int PositionInFile;

            internal IndexEntry(int hashCode, int positionInFile)
            {
                this.HashCode = hashCode;
                this.PositionInFile = positionInFile;
            }

            public int CompareTo(IndexEntry other)
            {
                return this.HashCode.CompareTo(other.HashCode);
            }
        }

        private sealed class LinePositionMapper
        {
            private readonly FileStream fs;

            private readonly Decoder decoder;

            private int currentLine = 1;

            private byte[] input = new byte[1];

            private char[] output = new char[1];

            public LinePositionMapper(FileStream fs, Encoding encoding)
            {
                this.decoder = encoding.GetDecoder();
                this.fs = fs;
            }

            public int GetPositionForLine(int line)
            {
                while (line > this.currentLine)
                {
                    int num = this.fs.ReadByte();
                    if (num < 0)
                    {
                        throw new EndOfStreamException();
                    }
                    this.input[0] = (byte)num;
                    int num2 = default(int);
                    int num3 = default(int);
                    bool flag = default(bool);
                    this.decoder.Convert(this.input, 0, 1, this.output, 0, 1, false, out num2, out num3, out flag);
                    if (num3 == 1 && this.output[0] == '\n')
                    {
                        this.currentLine++;
                    }
                }
                return checked((int)this.fs.Position);
            }
        }

        [NonSerialized]
        private XmlDocumentationCache cache = new XmlDocumentationCache(50);

        private readonly string fileName;

        private readonly Encoding encoding;

        private volatile IndexEntry[] index;

        public XmlDocumentationProvider(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (FileStream input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(input))
                {
                    xmlTextReader.XmlResolver = null;
                    xmlTextReader.MoveToContent();
                    if (string.IsNullOrEmpty(xmlTextReader.GetAttribute("redirect")))
                    {
                        this.fileName = fileName;
                        this.encoding = xmlTextReader.Encoding;
                        this.ReadXmlDoc(xmlTextReader);
                        goto end_IL_0032;
                    }
                    string redirectionTarget = XmlDocumentationProvider.GetRedirectionTarget(fileName, xmlTextReader.GetAttribute("redirect"));
                    if (redirectionTarget != null)
                    {
                        using (FileStream input2 = new FileStream(redirectionTarget, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
                        {
                            using (XmlTextReader xmlTextReader2 = new XmlTextReader(input2))
                            {
                                xmlTextReader2.XmlResolver = null;
                                xmlTextReader2.MoveToContent();
                                this.fileName = redirectionTarget;
                                this.encoding = xmlTextReader2.Encoding;
                                this.ReadXmlDoc(xmlTextReader2);
                            }
                        }
                        goto end_IL_0032;
                    }
                    throw new XmlException("XmlDoc " + fileName + " is redirecting to " + xmlTextReader.GetAttribute("redirect") + ", but that file was not found.");
                end_IL_0032:;
                }
            }
        }

        private static string GetRedirectionTarget(string xmlFileName, string target)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            folderPath = XmlDocumentationProvider.AppendDirectorySeparator(folderPath);
            string runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            runtimeDirectory = XmlDocumentationProvider.AppendDirectorySeparator(runtimeDirectory);
            string text = target.Replace("%PROGRAMFILESDIR%", folderPath).Replace("%CORSYSDIR%", runtimeDirectory);
            if (!Path.IsPathRooted(text))
            {
                text = Path.Combine(Path.GetDirectoryName(xmlFileName), text);
            }
            return XmlDocumentationProvider.LookupLocalizedXmlDoc(text);
        }

        private static string AppendDirectorySeparator(string dir)
        {
            if (!dir.EndsWith("\\", StringComparison.Ordinal) && !dir.EndsWith("/", StringComparison.Ordinal))
            {
                return dir + Path.DirectorySeparatorChar.ToString();
            }
            return dir;
        }

        public static string LookupLocalizedXmlDoc(string fileName)
        {
            string text = Path.ChangeExtension(fileName, ".xml");
            string twoLetterISOLanguageName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            string localizedName = XmlDocumentationProvider.GetLocalizedName(text, twoLetterISOLanguageName);
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
                string localizedName2 = XmlDocumentationProvider.GetLocalizedName(text, "en");
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

        private void ReadXmlDoc(XmlTextReader reader)
        {
            using (FileStream fs = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
            {
                LinePositionMapper linePosMapper = new LinePositionMapper(fs, this.encoding);
                List<IndexEntry> list = new List<IndexEntry>();
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        string localName = reader.LocalName;
                        if (localName == "members")
                        {
                            XmlDocumentationProvider.ReadMembersSection(reader, linePosMapper, list);
                        }
                    }
                }
                list.Sort();
                this.index = list.ToArray();
            }
        }

        private static void ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List<IndexEntry> indexList)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (!(reader.LocalName == "members"))
                        {
                            break;
                        }
                        return;
                    case XmlNodeType.Element:
                        if (reader.LocalName == "member")
                        {
                            int positionInFile = linePosMapper.GetPositionForLine(reader.LineNumber) + Math.Max(reader.LinePosition - 2, 0);
                            string attribute = reader.GetAttribute("name");
                            if (attribute != null)
                            {
                                indexList.Add(new IndexEntry(XmlDocumentationProvider.GetHashCode(attribute), positionInFile));
                            }
                            reader.Skip();
                        }
                        break;
                }
            }
        }

        private static int GetHashCode(string key)
        {
            int num = 0;
            foreach (char c in key)
            {
                num = (num << 5) - num + c;
            }
            return num;
        }

        public string GetDocumentation(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return this.GetDocumentation(key, true);
        }

        private string GetDocumentation(string key, bool allowReload)
        {
            int hashCode = XmlDocumentationProvider.GetHashCode(key);
            IndexEntry[] array = this.index;
            int num = Array.BinarySearch(array, new IndexEntry(hashCode, 0));
            if (num < 0)
            {
                return null;
            }
            while (--num >= 0 && array[num].HashCode == hashCode)
            {
            }
            XmlDocumentationCache xmlDocumentationCache = this.cache;
            lock (xmlDocumentationCache)
            {
                string text = default(string);
                if (!xmlDocumentationCache.TryGet(key, out text))
                {
                    try
                    {
                        while (++num < array.Length && array[num].HashCode == hashCode)
                        {
                            text = this.LoadDocumentation(key, array[num].PositionInFile);
                            if (text != null)
                            {
                                break;
                            }
                        }
                        xmlDocumentationCache.Add(key, text);
                    }
                    catch (IOException)
                    {
                        return allowReload ? this.ReloadAndGetDocumentation(key) : null;
                    }
                    catch (XmlException)
                    {
                        return allowReload ? this.ReloadAndGetDocumentation(key) : null;
                    }
                }
                return text;
            }
        }

        private string ReloadAndGetDocumentation(string key)
        {
            try
            {
                using (FileStream input = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
                {
                    using (XmlTextReader xmlTextReader = new XmlTextReader(input))
                    {
                        xmlTextReader.XmlResolver = null;
                        xmlTextReader.MoveToContent();
                        this.ReadXmlDoc(xmlTextReader);
                    }
                }
            }
            catch (IOException)
            {
                this.index = new IndexEntry[0];
                return null;
            }
            catch (XmlException)
            {
                this.index = new IndexEntry[0];
                return null;
            }
            return this.GetDocumentation(key, false);
        }

        private string LoadDocumentation(string key, int positionInFile)
        {
            using (FileStream fileStream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
            {
                fileStream.Position = positionInFile;
                XmlParserContext context = new XmlParserContext(null, null, null, XmlSpace.None)
                {
                    Encoding = this.encoding
                };
                using (XmlTextReader xmlTextReader = new XmlTextReader(fileStream, XmlNodeType.Element, context))
                {
                    xmlTextReader.XmlResolver = null;
                    while (xmlTextReader.Read())
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.Element)
                        {
                            string attribute = xmlTextReader.GetAttribute("name");
                            if (attribute == key)
                            {
                                return xmlTextReader.ReadInnerXml();
                            }
                            return null;
                        }
                    }
                    return null;
                }
            }
        }

        public virtual void OnDeserialization(object sender)
        {
            this.cache = new XmlDocumentationCache(50);
        }
    }

}
