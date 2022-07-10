using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NotifyPropertyChangeSourceGenerator
{
    [Generator]
    public class NotifyPropertyChangeGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var classesToGenerate = (context.SyntaxContextReceiver as NotifyPropertyChangeSyntaxReceiver)?.AutoImplementClassDescriptors;
            if (classesToGenerate == null)
                return;

            var additionalSource = new StringBuilder();

            foreach (var cls in classesToGenerate)
            {
                if (cls.ClassUsings != null)
                {
                    foreach (var namespaceUsing in cls.ClassUsings)
                    {
                        additionalSource.AppendFormat("using {0};\n", namespaceUsing);
                    }
                }
                    
                additionalSource.AppendLine();

                additionalSource.AppendFormat("namespace {0}\n", cls.Namespace);
                additionalSource.AppendLine("{");

                additionalSource.AppendFormat("\t{0} class {1}\n", cls.ClassModifiers, cls.ClassName);
                additionalSource.AppendLine("\t{");

                if (cls.NeedsProtectedEventTrigerMethod)
                {
                    additionalSource.AppendFormat("\t\tprotected void OnPropertyChanged(string propertyName)\n");
                    additionalSource.AppendLine("\t\t{");
                    additionalSource.AppendFormat("\t\t\tPropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));\n");
                    additionalSource.AppendLine("\t\t}");
                }
                if (cls.AutoImplementFields != null)
                {
                    foreach (var prop in cls.AutoImplementFields)
                    {
                        additionalSource.AppendFormat("\t\t{0} {1} {2}\n", prop.PropertyAccessibility, prop.FieldType, prop.PropertyName);
                        additionalSource.AppendLine("\t\t{");

                        additionalSource.AppendLine("\t\t\tget");
                        additionalSource.AppendLine("\t\t\t{");
                        additionalSource.AppendFormat("\t\t\t\treturn {0};\n", prop.FieldName);
                        additionalSource.AppendLine("\t\t\t}");

                        additionalSource.AppendLine("\t\t\tset");
                        additionalSource.AppendLine("\t\t\t{");
                        additionalSource.AppendFormat("\t\t\t\t{0} = value;\n", prop.FieldName);
                        additionalSource.AppendFormat("\t\t\t\tOnPropertyChanged(nameof({0}));\n", prop.PropertyName);
                        additionalSource.AppendLine("\t\t\t}");

                        additionalSource.AppendLine("\t\t}");
                    }
                }

                additionalSource.AppendLine("\t}");
                additionalSource.AppendLine("}");
            }

            context.AddSource("AutoNotifyPropertyChangeImpl.g.cs", additionalSource.ToString());

            
            // shamelessly stolen from https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new NotifyPropertyChangeSyntaxReceiver());
            context.RegisterForPostInitialization(ctx =>
            {
                var attributeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NotifyPropertyChangeSourceGenerator.NotifyPropertyChangeAttribute.cs");
                using (var reader = new StreamReader(attributeStream))
                {
                    var attributeSource = reader.ReadToEnd();
                    ctx.AddSource("NotifyPropertyChangeAttribute.g.cs", attributeSource);
                }
            });
        }
    }
}
