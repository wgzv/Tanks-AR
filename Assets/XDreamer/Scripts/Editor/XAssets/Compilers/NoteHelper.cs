using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    public class NoteHelper
    {
        public static bool outMissNoteLog = false;

        public static void Init()
        {
            XmlDocLoader.Init();
        }

        public static void Release()
        {
            XmlDocLoader.Init();
        }

        public static Note GetNote(MemberInfo member)
        {
            try
            {
                return Note.Create(GetNodeXmlDocumentation(member)) ?? Note.Default;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return Note.Default;
        }

        private static string GetNodeXmlDocumentation(MemberInfo member)
        {
            if (member == null) return null;
            XmlDocumentationProvider xmlDocumentationProvider = XmlDocLoader.LoadDocumentation(member.Module);
            if (xmlDocumentationProvider == null) return null;
            var key = XmlDocKeyProvider.GetKey(member);
            var note = xmlDocumentationProvider.GetDocumentation(key);
            if (note == null && outMissNoteLog)
            {
                Log.ErrorFormat("查找[{0}]的Key[{1}]失败!", member.ToString(), key);
            }
            return note;
        }
    }
}
