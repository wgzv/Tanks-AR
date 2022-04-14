using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.EditorExtension.XAssets.Compilers
{
    public class XmlDocKeyProvider
    {
        public static string GetKey(MemberInfo member)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (member is Type)
            {
                stringBuilder.Append("T:");
                AppendTypeName(stringBuilder, (Type)member);
            }
            else
            {
                if (member is FieldInfo)
                {
                    stringBuilder.Append("F:");
                }
                else if (member is PropertyInfo)
                {
                    stringBuilder.Append("P:");
                }
                else if (member is EventInfo)
                {
                    stringBuilder.Append("E:");
                }
                else if (member is MethodInfo)
                {
                    stringBuilder.Append("M:");
                }
                AppendTypeName(stringBuilder, member.DeclaringType);
                stringBuilder.Append('.');
                stringBuilder.Append(member.Name.Replace('.', '#'));
                Type type = null;
                IList<ParameterInfo> list;
                if (member is PropertyInfo)
                {
                    list = ((PropertyInfo)member).GetIndexParameters();
                }
                else if (member is MethodInfo)
                {
                    var methodReference = (MethodInfo)member;
                    if (methodReference.ContainsGenericParameters)
                    {
                        stringBuilder.Append("``");
                        stringBuilder.Append(methodReference.GetGenericArguments().Length);
                    }
                    list = methodReference.GetParameters();
                    if (methodReference.Name == "op_Implicit" || methodReference.Name == "op_Explicit")
                    {
                        type = methodReference.ReturnType;
                    }
                }
                else
                {
                    list = null;
                }
                if (list != null && list.Count > 0)
                {
                    stringBuilder.Append('(');
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i > 0)
                        {
                            stringBuilder.Append(',');
                        }
                        AppendTypeName(stringBuilder, list[i].ParameterType);
                    }
                    stringBuilder.Append(')');
                }
                if (type != null)
                {
                    stringBuilder.Append('~');
                    AppendTypeName(stringBuilder, type);
                }
            }
            return stringBuilder.ToString().Replace('&', '@');
        }

        private static void AppendTypeName(StringBuilder b, Type type)
        {
            if (type == null) return;

            if (type.DeclaringType != null)
            {
                AppendTypeName(b, type.DeclaringType);
                b.Append('.');
                b.Append(type.Name);
                return;
            }

            if(type.IsGenericType)
            {
                b.Append(type.Namespace);
                b.Append('.');
                b.Append(type.Name);
                return;
            }

            b.Append(type.FullName);
        }

        private static void AppendTypeNameX(StringBuilder b, Type type)
        {
            if (type != null)
            {
                if(type.IsGenericType)
                {
                    AppendTypeNameWithArguments(b, type, type.GenericTypeArguments);
                }
                else if(type.IsSpecialName)
                {
                    AppendTypeName(b, type.GetElementType());
                    //if(type.IsArray)
                    //{
                    //    b.Append('[');
                    //    for (int i = 0; i < arrayType.Dimensions.Count; i++)
                    //    {
                    //        if (i > 0)
                    //        {
                    //            b.Append(',');
                    //        }
                    //        ArrayDimension arrayDimension = arrayType.Dimensions[i];
                    //        if (arrayDimension.IsSized)
                    //        {
                    //            b.Append(arrayDimension.LowerBound);
                    //            b.Append(':');
                    //            b.Append(arrayDimension.UpperBound);
                    //        }
                    //    }
                    //    b.Append(']');
                    //}
                    //ByReferenceType byReferenceType = type as ByReferenceType;
                    //if (byReferenceType != null)
                    //{
                    //    b.Append('@');
                    //}
                    //PointerType pointerType = type as PointerType;
                    //if (pointerType != null)
                    //{
                    //    b.Append('*');
                    //}
                }
                else
                {
                    //GenericParameter genericParameter = type as GenericParameter;
                    //if (genericParameter != null)
                    //{
                    //    b.Append('`');
                    //    if (genericParameter.Owner.GenericParameterType == GenericParameterType.Method)
                    //    {
                    //        b.Append('`');
                    //    }
                    //    b.Append(genericParameter.Position);
                    //}
                    //else 
                    if (type.DeclaringType != null)
                    {
                        AppendTypeName(b, type.DeclaringType);
                        b.Append('.');
                        b.Append(type.Name);
                    }
                    else
                    {
                        b.Append(type.FullName);
                    }
                }
            }
        }

        private static int AppendTypeNameWithArguments(StringBuilder b, Type type, IList<Type> genericArguments)
        {
            int num = 0;
            if (type.DeclaringType != null)
            {
                Type declaringType = type.DeclaringType;
                num = AppendTypeNameWithArguments(b, declaringType, genericArguments);
                b.Append('.');
            }
            else if (!string.IsNullOrEmpty(type.Namespace))
            {
                b.Append(type.Namespace);
                b.Append('.');
            }
            int num2 = 0;
            b.Append(SplitTypeParameterCountFromReflectionName(type.Name, out num2));
            if (num2 > 0)
            {
                int num3 = num + num2;
                b.Append('{');
                for (int i = num; i < num3 && i < genericArguments.Count; i++)
                {
                    if (i > num)
                    {
                        b.Append(',');
                    }
                    AppendTypeName(b, genericArguments[i]);
                }
                b.Append('}');
            }
            return num + num2;
        }

        public static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
        {
            int num = reflectionName.LastIndexOf('`');
            if (num < 0)
            {
                typeParameterCount = 0;
                return reflectionName;
            }
            string s = reflectionName.Substring(num + 1);
            if (int.TryParse(s, out typeParameterCount))
            {
                return reflectionName.Substring(0, num);
            }
            return reflectionName;
        }
    }
}
