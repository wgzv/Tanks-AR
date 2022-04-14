using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using XCSJ.Algorithms;
using XCSJ.Collections;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    /// <summary>
    /// 注释
    /// </summary>
    public class Note
    {
        public Section summary => this[nameof(summary)];

        public Section filterpriority => this[nameof(filterpriority)];
        public Section remarks => this[nameof(remarks)];
        public Section example => this[nameof(example)];
        public Section exception => this[nameof(exception)];
        public Section returns => this[nameof(returns)];
        public Section see => this[nameof(see)];
        public Section seealso => this[nameof(seealso)];
        public Section paramref => this[nameof(paramref)];
        public Section param => this[nameof(param)];
        public Section typeparam => this[nameof(typeparam)];
        public Section value => this[nameof(value)];
        private Section br => null;
        private Section para => null;

        public readonly Dictionary<string, Section> notes = new Dictionary<string, Section>();

        public Section this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return null;
                if (notes.TryGetValue(name, out Section value)) return value;
                return notes[name] = new Section() { name = name };
            }
        }

        private Section section => this[current];

        private string current = null;

        public static Note Default { get; } = Default<Note>.Instance;

        public static Note Create(string xmlDocumentation)
        {
            if (string.IsNullOrEmpty(xmlDocumentation)) return null;
            var note = new Note();
            note.AddXmlDocumentation(xmlDocumentation);
            return note;
        }

        private static readonly Regex whitespace = new Regex("\\s+");

        private void AddXmlDocumentation(string xmlDocumentation)
        {
            if (xmlDocumentation != null)
            {
                try
                {
                    XmlTextReader xmlTextReader = new XmlTextReader(new StringReader("<docroot>" + xmlDocumentation + "</docroot>"));
                    xmlTextReader.XmlResolver = null;
                    this.AddXmlDocumentation(xmlTextReader);
                }
                catch (XmlException)
                {
                }
            }
        }

        private void AppendText(string text) => this[current].AppendText(text);

        private void AppendNewLine() => this[current].AppendNewLine();

        private void AddXmlDocumentation(XmlReader xml)
        {
            //Log.Debug("AddXmlDocumentation 0: ");
            current = nameof(summary);
            while (xml.Read())
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    var last = current;
                    switch (current = xml.Name.ToLowerInvariant())
                    {
                        case nameof(filterpriority):
                        case nameof(remarks):
                            {
                                xml.Skip();
                                break;
                            }
                        case nameof(example):
                        case nameof(returns):
                            {
                                break;
                            }
                        case nameof(exception):
                            {
                                AppendText(GetCref(xml["cref"]));
                                break;
                            }
                        case nameof(see):
                            {
                                AppendText(GetCref(xml["cref"]));
                                AppendNewLine();
                                AppendText(xml["langword"]);
                                break;
                            }
                        case nameof(seealso):
                            {
                                AppendText(GetCref(xml["cref"]));
                                break;
                            }
                        case nameof(paramref):
                            {
                                AppendText(xml["name"]);
                                break;
                            }
                        case nameof(param):
                            {
                                section.current = whitespace.Replace(xml["name"].Trim(), " ");
                                break;
                            }
                        case nameof(typeparam):
                            {
                                section.current = whitespace.Replace(xml["name"].Trim(), " ");
                                break;
                            }
                        case nameof(value):
                            {
                                AppendNewLine();
                                break;
                            }
                        case nameof(br):
                        case nameof(para):
                            {
                                current = last;
                                AppendNewLine();
                                break;
                            }
                    }
                }
                else if (xml.NodeType == XmlNodeType.Text)
                {
                    AppendText(whitespace.Replace(xml.Value, " "));
                }
            }
        }

        private static string GetCref(string cref)
        {
            if (cref != null && cref.Trim().Length != 0)
            {
                if (cref.Length < 2)
                {
                    return cref;
                }
                if (cref.Substring(1, 1) == ":")
                {
                    return cref.Substring(2, cref.Length - 2);
                }
                return cref;
            }
            return "";
        }
    }

    /// <summary>
    /// 节
    /// </summary>
    public class Section
    {
        public string name = "";
        public string current = "";

        public Dictionary<string, Para> paras = new Dictionary<string, Para>();

        public Para this[string name]
        {
            get
            {
                if (name == null) return para;
                if (paras.TryGetValue(name, out Para value)) return value;
                return paras[name] = new Para() { name = name };
            }
        }

        private Para para => this[current];

        public void AppendText(string text) => para.AppendText(text);

        public void AppendNewLine() => para.AppendNewLine();

        public override string ToString() => this[""].ToString();
    }

    /// <summary>
    /// 段落
    /// </summary>
    public class Para
    {
        public string name = "";

        public List<StringBuilder> paras = new List<StringBuilder>();

        public StringBuilder sb
        {
            get
            {
                if (paras.Count == 0) AppendNewLine();
                return paras[paras.Count - 1];
            }
        }

        public void AppendText(string text) => sb.Append(text);

        public void AppendNewLine() => paras.Add(new StringBuilder());

        public override string ToString() => paras.ToStringDirect("\n");
    }
}
