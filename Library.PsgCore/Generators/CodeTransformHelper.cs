using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.PsgCore.Generators
{
    public class CodeTransformHelper
    {
        #region Fields
        private readonly ITextTransform transform;
        private readonly CodeGenerationTools code;
        #endregion

        public CodeTransformHelper(CodeGenerationTools code, ITextTransform transform)
        {
            this.code = code;
            this.transform = transform;
        }

        public string AccessibilityAndVirtual(string accessibility)
        {
            return accessibility + (accessibility != "private" ? " virtual" : "");
        }

        public IDisposable BeginNamespace(string codeNamespace)
        {
            transform.WriteLine(string.Format("namespace {0}{1}{{",
                code.EscapeNamespace(codeNamespace),
                Environment.NewLine));
            transform.PushIndent("    ");
            return new CodeBlock(EndBlock);
        }

        public IDisposable BeginEmitBlock(string blockHeader, string postCurly)
        {
            transform.WriteLine(string.Join(Environment.NewLine,
                    blockHeader, "{"));
            transform.PushIndent("    ");
            return new CodeBlock(() =>
            {
                PopIndent();
                transform.WriteLine(string.Format("}}{0}", postCurly));
            });
        }

        private void EndBlock()
        {
            PopIndent();
            transform.WriteLine("}");
        }

        public IDisposable BeginEmitBlock(string blockHeader)
        {
            return BeginEmitBlock(blockHeader, string.Empty);
        }

        public class CodeBlock : IDisposable
        {
            private readonly System.Action endBlock;
            public CodeBlock(System.Action endblock)
            {
                this.endBlock = endblock;
            }
            void IDisposable.Dispose()
            {
                endBlock();
            }
        }

        public class NonBlock : IDisposable
        {
            void IDisposable.Dispose()
            {
            }
        }

        private string PopIndent()
        {
            return transform.PopIndent();
        }

        public IDisposable BeginEmitGet()
        {
            return BeginEmitBlock("get");
        }

        public void EmitUsings(IEnumerable<string> namespaces)
        {
            foreach (var @namespace in namespaces)
                transform.WriteLine(string.Format("using {0};", @namespace));
        }
        
        public IDisposable BeginEmitPartialClass(Type type)
        {
            string fullName = type.Name;
            var builder = new StringBuilder(string.Format(
                "{0} {1}partial class {2}",
                type.IsPublic ? "public" : "private",
                code.SpaceAfter(type.IsAbstract ? "abstract" : string.Empty),
                type.IsGenericTypeDefinition
                    ? fullName.Substring(0, fullName.IndexOf('`'))
                    : fullName));

            if (!type.IsGenericType)
                return BeginEmitBlock(builder.ToString());

            #region <TOne, TTwo, Tetc>
            builder.Append('<');
            var typeArguments = type.GetGenericArguments();
            int index = 0;
            while (true)
            {
                builder.Append(typeArguments[index].Name);
                if (++index >= typeArguments.Length)
                {
                    builder.Append('>');
                    break;
                }
                builder.Append(',');
            }
            #endregion

            #region where TOne: SomeClass
            Func<Type, Type[], string> action = (argument, typeParameters) =>
            {
                bool hasParameters = typeParameters.Length > 0;
                bool hasDefaultConstructor = ((argument.GenericParameterAttributes
                    & System.Reflection.GenericParameterAttributes.DefaultConstructorConstraint) > 0);

                if (!hasParameters && !hasDefaultConstructor)
                    return string.Empty;

                builder = new StringBuilder("where ");
                builder.Append(argument.Name);
                builder.Append(" : ");
                index = 0;
                while (hasParameters)
                {
                    builder.Append(code.GetCSharpTypeName(typeParameters[index]));
                    if (++index >= typeParameters.Length)
                        break;

                    builder.Append(", ");
                }

                if (hasDefaultConstructor)
                {
                    if (hasParameters)
                        builder.Append(", ");
                    builder.Append("new()");
                }
                return builder.ToString();
            };
            #endregion

            transform.WriteLine(builder.ToString());
            transform.PushIndent("    ");

            int outer = 0;
            while (true)
            {
                var argument = typeArguments[outer];
                var typeParameters = argument.GetGenericParameterConstraints();
                string constraint = action(argument, typeParameters);
                if (++outer >= typeArguments.Length)
                {
                    transform.PopIndent();
                    return BeginEmitBlock(string.Format("    {0}", constraint));
                }
                transform.WriteLine(constraint);
            }
        }
    }
}
