using System;
using System.Text;

namespace Echo
{
    public sealed class Coder
    {
        private const    string        IndentText = "    ";
        private readonly StringBuilder m_Code = new StringBuilder();

        public int IndentLevel = 0;

        #region Writer

        public void Clear()
        {
            m_Code.Clear();
            IndentLevel = 0;
        }

        public override string ToString()
        {
            return m_Code.ToString();
        }

        public Coder AppendLine(string text)
        {
            for (int i = 0; i < IndentLevel; i++)
            {
                text = IndentText + text;
            }

            m_Code.AppendLine(text);
            return this;
        }

        public Coder AppendLine()
        {
            m_Code.AppendLine();
            return this;
        }

        public Coder PushBlock()
        {
            AppendLine("{");
            IndentLevel++;
            return this;
        }

        public Coder PopBlock()
        {
            IndentLevel--;
            AppendLine("}");
            return this;
        }

        #endregion

        #region Namespace

        public Namespace NewNamespace(string name)
        {
            return new Namespace(this, name);
        }

        public struct Namespace : IDisposable
        {
            private readonly Coder m_Coder;

            public Namespace(Coder coder, string name)
            {
                m_Coder = coder;
                m_Coder
                    .AppendLine($"namespace {name}")
                    .PushBlock();
            }

            public void Dispose()
            {
                m_Coder.PopBlock();
            }
        }

        #endregion

        #region Region

        public Region NewRegion(string name)
        {
            return new Region(this, name);
        }

        public readonly struct Region : IDisposable
        {
            private readonly Coder m_Coder;

            public Region(Coder coder, string name)
            {
                m_Coder = coder;
                m_Coder
                    .AppendLine($"#region {name}")
                    .AppendLine();
            }

            public void Dispose()
            {
                m_Coder
                    .AppendLine()
                    .AppendLine($"#endregion");
            }
        }

        #endregion

        #region Class

        public Class NewClass(string modifier, string className, string basicClass = null, params string[] interfaces)
        {
            return new Class(this, modifier, className, basicClass, interfaces);
        }

        public readonly struct Class : IDisposable
        {
            private readonly Coder m_Coder;

            public Class(Coder coder, string modifier, string className, string basicClass = null, params string[] interfaces)
            {
                m_Coder = coder;
                string text = $"{modifier} class {className}";
                if (string.IsNullOrEmpty(basicClass) == false)
                {
                    text += $" : {basicClass}";
                }

                if (interfaces != null)
                {
                    for (int i = 0, length = interfaces.Length; i < length; i++)
                    {
                        if (i == 0 && string.IsNullOrEmpty(basicClass))
                        {
                            text += $" : {interfaces[i]}";
                        }
                        else
                        {
                            text += $", {interfaces[i]}";
                        }
                    }
                }

                m_Coder.AppendLine(text);
                m_Coder.PushBlock();
            }

            public void Dispose()
            {
                m_Coder.PopBlock();
            }
        }

        #endregion

        #region Class

        public Struct NewStruct(string modifier, string structName, params string[] interfaces)
        {
            return new Struct(this, modifier, structName, interfaces);
        }

        public readonly struct Struct : IDisposable
        {
            private readonly Coder m_Coder;

            public Struct(Coder coder, string modifier, string structName, params string[] interfaces)
            {
                m_Coder = coder;
                string text = $"{modifier} struct {structName}";
                if (interfaces != null && interfaces.Length > 0)
                {
                    text += $" : {interfaces[0]}";
                    for (int i = 1, length = interfaces.Length; i < length; i++)
                    {
                        text += $", {interfaces[i]}";
                    }
                }

                m_Coder.AppendLine(text);
                m_Coder.PushBlock();
            }

            public void Dispose()
            {
                m_Coder.PopBlock();
            }
        }

        #endregion

        #region Method

        public Method NewMethod(string modifier, string returnType, string name, params string[] parameters)
        {
            return new Method(this, modifier, returnType, name, parameters);
        }

        public readonly struct Method : IDisposable
        {
            private readonly Coder m_Coder;

            public Method(Coder coder, string modifier, string returnType, string name, params string[] parameters)
            {
                m_Coder = coder;
                string text = string.IsNullOrEmpty(modifier) ? $"{returnType} {name}(" : $"{modifier} {returnType} {name}(";
                if (parameters != null && parameters.Length > 0)
                {
                    text += parameters[0];
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        text += $", {parameters[i]}";
                    }
                }

                text += ")";
                m_Coder
                    .AppendLine(text)
                    .PushBlock();
            }

            public void Dispose()
            {
                m_Coder.PopBlock();
            }
        }

        #endregion
    }
}