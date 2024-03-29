﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NotifyPropertyChangeSourceGenerator
{
    public class NotifyPropertyChangeSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<AutoImplementClassDescriptor> AutoImplementClassDescriptors { get; } = new List<AutoImplementClassDescriptor>();
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var inpcInterface = context.SemanticModel.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
            if (context.Node.IsKind(SyntaxKind.ClassDeclaration))
            {
                var cd = context.Node as ClassDeclarationSyntax;
                if (cd == null)
                    return;

                var classModel = context.SemanticModel.GetDeclaredSymbol(cd);
                if (classModel == null)
                    return;

                if (cd.BaseList != null && cd.Modifiers.Any(x => x.Text == "partial"))
                {
                    if (classModel.AllInterfaces.Contains(inpcInterface, SymbolEqualityComparer.Default))
                    {
                        var autoClass = new AutoImplementClassDescriptor()
                        {
                            ClassName = classModel.Name,
                            Namespace = classModel.ContainingNamespace.ToDisplayString(),
                            ClassModifiers = cd.Modifiers.ToString(),
                            NeedsEventTriggerMethod = ClassGeneratesEventAndEventTriggerMethod(classModel, inpcInterface)
                        };

                        var fieldMembers = classModel.GetMembers()
                            .Where(m => m.Kind == SymbolKind.Field && !m.IsStatic && m.GetAttributes().Any(a => a.AttributeClass?.Name == "NotifyPropertyChangeAttribute"))
                            .Cast<IFieldSymbol>();

                        if (fieldMembers.Any())
                        {
                            var classAccessibility = string.Join(" ", cd.Modifiers.Where(x => x.Text != "partial"));
                            autoClass.AutoImplementFields = fieldMembers.Select(f => new AutoImplementFieldDescriptor() { FieldName = f.Name, FieldType = f.Type.ContainingNamespace.ToDisplayString() + "." + f.Type.Name, PropertyName = GetAnnotatedPropertyName(f), PropertyAccessibility = classAccessibility });
                        }

                        AutoImplementClassDescriptors.Add(autoClass);
                    }
                }
            }
        }

        private bool ClassGeneratesEventAndEventTriggerMethod(INamedTypeSymbol classModel, INamedTypeSymbol inpcInterface)
        {
            return classModel.Interfaces.Contains(inpcInterface, SymbolEqualityComparer.Default);
        }

        private static string GetAnnotatedPropertyName(IFieldSymbol field)
        {
            var attr = field.GetAttributes().First(a => a.AttributeClass.Name == "NotifyPropertyChangeAttribute");
            return attr.ConstructorArguments.First().Value.ToString();
        }
    }
}
