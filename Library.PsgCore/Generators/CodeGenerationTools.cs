using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Copyright (c) Microsoft Corporation.  All rights reserved.
// The code from this file was taken verbatim from EF.Utility.CS.ttinclude
namespace PhillipScottGivens.Library.PsgCore.Generators
{
    /// <summary>
    /// Responsible for helping to create source code that is
    /// correctly formated and functional
    /// </summary>
    public class CodeGenerationTools
    {
        private readonly CSharpCodeProvider _code;

        /// <summary>
        /// Initializes a new CodeGenerationTools object with the TextTransformation (T4 generated class)
        /// that is currently running
        /// </summary>
        public CodeGenerationTools()
        {
            _code = new CSharpCodeProvider();
            FullyQualifySystemTypes = false;
            CamelCaseFields = true;
        }

        /// <summary>
        /// When true, all types that are not being generated
        /// are fully qualified to keep them from conflicting with
        /// types that are being generated. Useful when you have
        /// something like a type being generated named System.
        ///
        /// Default is false.
        /// </summary>
        public bool FullyQualifySystemTypes { get; set; }

        /// <summary>
        /// When true, the field names are Camel Cased,
        /// otherwise they will preserve the case they
        /// start with.
        ///
        /// Default is true.
        /// </summary>
        public bool CamelCaseFields { get; set; }

        ///// <summary>
        ///// Returns the NamespaceName suggested by VS if running inside VS.  Otherwise, returns
        ///// null.
        ///// </summary>
        //public string VsNamespaceSuggestion(ITextTransform textTransformation)
        //{
        //    string suggestion = textTransformation.Host.ResolveParameterValue("directiveId", "namespaceDirectiveProcessor", "namespaceHint");
        //    if (String.IsNullOrEmpty(suggestion))
        //    {
        //        return null;
        //    }

        //    return suggestion;
        //}

        /// <summary>
        /// Returns a string that is safe for use as an identifier in C#.
        /// Keywords are escaped.
        /// </summary>
        public string Escape(string name)
        {
            if (name == null)
            {
                return null;
            }

            return _code.CreateEscapedIdentifier(name);
        }

        /// <summary>
        /// Returns the NamespaceName with each segment safe to
        /// use as an identifier.
        /// </summary>
        public string EscapeNamespace(string namespaceName)
        {
            if (String.IsNullOrEmpty(namespaceName))
            {
                return namespaceName;
            }

            string[] parts = namespaceName.Split('.');
            namespaceName = String.Empty;
            foreach (string part in parts)
            {
                if (namespaceName != String.Empty)
                {
                    namespaceName += ".";
                }

                namespaceName += Escape(part);
            }

            return namespaceName;
        }

        public string FieldName(string name)
        {
            if (CamelCaseFields)
            {
                return "_" + CamelCase(name);
            }
            else
            {
                return "_" + name;
            }
        }

        /// <summary>
        /// Returns the name of the Type object formatted for
        /// use in source code.
        ///
        /// This method changes behavior based on the FullyQualifySystemTypes
        /// setting.
        /// </summary>
        public string Escape(Type clrType)
        {
            return Escape(clrType, FullyQualifySystemTypes);
        }

        /// <summary>
        /// Returns the name of the Type object formatted for
        /// use in source code.
        /// </summary>
        public string Escape(Type clrType, bool fullyQualifySystemTypes)
        {
            if (clrType == null)
            {
                return null;
            }

            string typeName;
            if (fullyQualifySystemTypes)
            {
                typeName = "global::" + clrType.FullName;
            }
            else
            {
                typeName = _code.GetTypeOutput(new CodeTypeReference(clrType));
            }
            return typeName;
        }

        /// <summary>
        /// Returns the passed in identifier with the first letter changed to lowercase
        /// </summary>
        public string CamelCase(string identifier)
        {
            if (String.IsNullOrEmpty(identifier))
            {
                return identifier;
            }

            if (identifier.Length == 1)
            {
                return identifier[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            }

            return identifier[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + identifier.Substring(1);
        }

        /// <summary>
        /// If the value parameter is null or empty an empty string is returned,
        /// otherwise it retuns value with a single space concatenated on the end.
        /// </summary>
        public string SpaceAfter(string value)
        {
            return StringAfter(value, " ");
        }

        /// <summary>
        /// If the value parameter is null or empty an empty string is returned,
        /// otherwise it retuns value with a single space concatenated on the end.
        /// </summary>
        public string SpaceBefore(string value)
        {
            return StringBefore(" ", value);
        }

        /// <summary>
        /// If the value parameter is null or empty an empty string is returned,
        /// otherwise it retuns value with append concatenated on the end.
        /// </summary>
        public string StringAfter(string value, string append)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            return value + append;
        }

        /// <summary>
        /// If the value parameter is null or empty an empty string is returned,
        /// otherwise it retuns value with prepend concatenated on the front.
        /// </summary>
        public string StringBefore(string prepend, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            return prepend + value;
        }

        /// <summary>
        /// Retuns as full of a name as possible, if a namespace is provided
        /// the namespace and name are combined with a period, otherwise just
        /// the name is returned.
        /// </summary>
        public string CreateFullName(string namespaceName, string name)
        {
            if (String.IsNullOrEmpty(namespaceName))
            {
                return name;
            }

            return namespaceName + "." + name;
        }

        /// <summary>
        /// Retuns a literal representing the supplied value.
        /// </summary>
        public string CreateLiteral(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            Type type = value.GetType();
            if (type.IsEnum)
            {
                return type.FullName + "." + value.ToString();
            }
            if (type == typeof(Guid))
            {
                return string.Format(CultureInfo.InvariantCulture, "new Guid(\"{0}\")",
                                     ((Guid)value).ToString("D", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(DateTime))
            {
                return string.Format(CultureInfo.InvariantCulture, "new DateTime({0}, DateTimeKind.Unspecified)",
                                     ((DateTime)value).Ticks);
            }
            else if (type == typeof(byte[]))
            {
                var arrayInit = string.Join(", ", ((byte[])value).Select(b => b.ToString(CultureInfo.InvariantCulture)).ToArray());
                return string.Format(CultureInfo.InvariantCulture, "new Byte[] {{{0}}}", arrayInit);
            }
            else if (type == typeof(DateTimeOffset))
            {
                var dto = (DateTimeOffset)value;
                return string.Format(CultureInfo.InvariantCulture, "new DateTimeOffset({0}, new TimeSpan({1}))",
                                     dto.Ticks, dto.Offset.Ticks);
            }
            else if (type == typeof(TimeSpan))
            {
                return string.Format(CultureInfo.InvariantCulture, "new TimeSpan({0})",
                                     ((TimeSpan)value).Ticks);
            }

            var expression = new CodePrimitiveExpression(value);
            var writer = new StringWriter();
            CSharpCodeProvider code = new CSharpCodeProvider();
            code.GenerateCodeFromExpression(expression, writer, new CodeGeneratorOptions());
            return writer.ToString();
        }

        static System.Resources.ResourceManager _resourceManager;

        private const string ExternalTypeNameAttributeName = @"http://schemas.microsoft.com/ado/2006/04/codegeneration:ExternalTypeName";

        #region From Pandora (Pandora.ApplicationServices.Proxy/Proxies.tt)
        // TODO: Perhaps we do not need a ProxyTemplate.tt and a Proxies.tt template parameters

        //// Set up some local constants
        //public string ClassName = "NotImportant";
        //public string BaseClassName = "object";
        //public string NamespaceName = "Pandora.ApplicationServices.Proxy";
        //public string InterfaceAssemblyName = @"..\Pandora.ApplicationServices\bin\Debug\Pandora.ApplicationServices.dll";
        //public string InterfaceTypeName = "Pandora.ApplicationServices";
        //public string WorkflowComponentPath = @"..\WorkflowComponent\bin\Debug\WorkflowComponent.dll";

        public MethodInfo[] GetMethods(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var value = new List<MethodInfo>();
            foreach (MethodInfo item in methods)
                if (item.DeclaringType.Assembly == type.Assembly)
                    value.Add(item);
            return value.ToArray();
        }

        // TODO: Get this method to work.
        public PropertyInfo[] GetAbstractProperties(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var value = new List<PropertyInfo>();
            foreach (PropertyInfo item in properties)
                if (item.GetGetMethod().IsAbstract && item.GetSetMethod().IsAbstract)
                    value.Add(item);
            return value.ToArray();
        }

        public string OverrideMethodSignature(MethodInfo method)
        {
            StringBuilder value = new StringBuilder(method.IsPublic
                ? "public"
                : "protected");
            value.Append(" override ");
            value.Append(GetCSharpTypeName(method.ReturnType));
            value.Append(' ');
            value.Append(method.Name);
            value.Append('(');
            value.Append(GetMethodParameterDeclaration(method));
            value.Append(')');
            return value.ToString();
        }

        public string OverrideConstructorSignature(Type type, ConstructorInfo constructor) {
            StringBuilder value = new StringBuilder(constructor.IsPublic
                ? "public"
                : "protected");
            value.Append(' ');
            value.Append(type.Name);
            value.Append('(');
            value.Append(GetMethodParameterDeclaration(constructor));
            value.AppendLine(")");
            value.Append("\t: ");
            value.Append(CallBaseConstructor(constructor));
            return value.ToString();
        }

        public string GetMethodParameterDeclaration(MethodBase method) {
            StringBuilder value = new StringBuilder();
            var parameters = method.GetParameters();
            int length = parameters.Length;
            for (int index = 0; index < length; index++) {
                value.Append(GetCSharpTypeName(parameters[index].ParameterType));
                value.Append(" " + parameters[index].Name);
                if (index < length - 1)
                    value.Append(',');
            }
            return value.ToString();
        }


        // TODO: Get this method to work.
        public string OverridePropertySignature(PropertyInfo property)
        {
            var method = property.GetGetMethod();
            StringBuilder value = new StringBuilder(method.IsPublic
                ? "public"
                : "protected");
            value.Append(" override ");
            value.Append(GetCSharpTypeName(method.ReturnType));
            value.Append(' ');
            value.Append(property.Name);
            return value.ToString();
        }

        public string CallBaseMethod(MethodInfo method)
        {
            StringBuilder value = new StringBuilder("base.");
            value.Append(method.Name);
            value.Append(GetMethodParameterCalls(method));
            return value.ToString();
        }

        public string CallBaseConstructor(ConstructorInfo constructor) {
            StringBuilder value = new StringBuilder("base");
            value.Append(GetMethodParameterCalls(constructor));
            return value.ToString();
        }

        public string GetMethodParameterCalls(MethodBase method) {
            StringBuilder value = new StringBuilder();
            value.Append('(');
            var parameters = method.GetParameters();
            int length = parameters.Length;
            for (int index = 0; index < length; index++) {
                value.Append(parameters[index].Name);
                if (index < length - 1)
                    value.Append(", ");
            }
            value.Append(')');
            return value.ToString();
        }

        public string GetCSharpTypeName(Type type)
        {
            switch (type.FullName)
            {
                case "System.Boolean": return "bool";
                case "System.Byte": return "byte";
                case "System.Char": return "char";
                case "System.DateTime": return "DateTime";
                case "System.Decimal": return "decimal";
                case "System.Double": return "double";
                case "System.Empty": return "void";
                case "System.Int16": return "short";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "System.SByte": return "sbyte";
                case "System.Single": return "float";
                case "System.String": return "string";
                case "System.UInt16": return "ushort";
                case "System.UInt32": return "uint";
                case "System.UInt64": return "ulong";
                case "System.Void": return "void";
                default: return GetCSharpFromGeneric(type);
            }
        }

        public string GetCSharpFromGeneric(Type type)
        {
            if (type.IsGenericParameter)
                return type.Name;

            if (!type.IsGenericType)
                return type.FullName;

            string fullName = type.FullName ?? string.Format("{0}.{1}", type.Namespace, type.Name);
            var builder = new StringBuilder(fullName.Substring(0, fullName.IndexOf('`')));
            builder.Append('<');
            var typeParameters = type.GetGenericArguments();
            int index = 0;
            for (; index < typeParameters.Length - 1; index++)
            {
                builder.Append(GetCSharpFromGeneric(typeParameters[index])).Append(',');
            }
            builder.Append(GetCSharpFromGeneric(typeParameters[index])).Append('>');

            return builder.ToString();
        }

        #endregion
    }

}
