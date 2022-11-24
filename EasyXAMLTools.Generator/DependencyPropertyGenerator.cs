﻿using CopySourceGenerator;
using EasyCSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace EasyXAMLTools;
[CopySource("DependencyPropertyAttributeSource", typeof(DependencyPropertyAttribute))]
[Generator]
public partial class DependencyPropertyGenerator : PropertyGeneratorBase
{
    protected override void OnInitialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(
            x => x.AddSource($"{typeof(DependencyPropertyAttribute)}.g.cs", DependencyPropertyAttributeSource)
        );
    }

    const string DependencyObjectNS = "Microsoft.UI.Xaml.DependencyObject";
    const string DependencyPropertyNS = "Microsoft.UI.Xaml.DependencyProperty";
    const string PropertyMetadataNS = "Microsoft.UI.Xaml.PropertyMetadata";
    protected override string AttributeTypeName => typeof(DependencyPropertyAttribute).FullName;
    protected override bool CustomOnChange => true;
    protected override string FileName(string ClaseName) => $"{ClaseName}.GeneratedDependencyProperty.g.cs";
    protected override string GetLogic(IFieldSymbol fieldSymbol, string PropertyName, string Indent)
        => $"return {fieldSymbol.Name} = ({fieldSymbol.Type})GetValue({PropertyName}Property);";
    protected override string SetLogic(IFieldSymbol fieldSymbol, string PropertyName, string Indent)
        => $"SetValue({PropertyName}Property, {fieldSymbol.Name} = value);";
    protected override string? BeforeHeadLogic(IFieldSymbol fieldSymbol, string propertyName, string Accessbility, string? OnChange)
        => $@"/// <summary>
/// <inheritdoc cref=""{fieldSymbol.Name}""/>
/// </summary>
{Accessbility}static readonly {DependencyPropertyNS} {propertyName}Property
    = {DependencyPropertyNS}.Register(
        ""{propertyName}"",
        typeof({fieldSymbol.Type}),
        typeof({fieldSymbol.ContainingType}),
        new {PropertyMetadataNS}({
            (fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax)
            ?.Initializer?.Value.ToFullString() ?? $"default({fieldSymbol.Type})"
        }, {OnChange ?? "null"})
    );
";
    protected override string ClassHeadLogic(GeneratorExecutionContext context, INamedTypeSymbol classSymbol)
    {
        return classSymbol.BaseType?.Equals(
            context.Compilation.GetTypeByMetadataName(DependencyObjectNS),
            SymbolEqualityComparer.Default) ?? false ? "" : $" : {DependencyObjectNS}";
    }
}