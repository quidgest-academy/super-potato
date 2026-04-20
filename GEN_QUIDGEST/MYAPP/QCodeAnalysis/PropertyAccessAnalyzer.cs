using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;

namespace QCodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PropertyAccessAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "QUID001";

        private static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(
                DiagnosticId,
                "Review Direct Property Access",
                "Direct assignment to property '{0}' may not persist. Review or modify form definition.",
                "Usage",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(StartCompilation);
        }

        private static void StartCompilation(CompilationStartAnalysisContext context)
        {
            var targetProperties =
                new ConcurrentDictionary<IPropertySymbol, byte>(SymbolEqualityComparer.Default);

            var formViewModelCache =
                new ConcurrentDictionary<INamedTypeSymbol, bool>(SymbolEqualityComparer.Default);

            // Collect target properties
            context.RegisterSymbolAction(symbolContext =>
            {
                var property = (IPropertySymbol)symbolContext.Symbol;

                // Check if the left-hand side of the assignment is a property with public setter.
                if (property.SetMethod?.DeclaredAccessibility != Accessibility.Public)
                    return;
                // Check only FormViewModels carry these properties.
                if (!IsFormViewModel(property.ContainingType, formViewModelCache))
                    return;
                // Only properties marked with this attribute require the check
                if (!property.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == "ValidateSetAccessAttribute"))
                    return;

                targetProperties.TryAdd(property, 0);
            }, SymbolKind.Property);

            // Analyze assignments via IOperation
            context.RegisterOperationAction(
                operationContext =>
                    AnalyzeAssignment(operationContext, targetProperties),
                OperationKind.SimpleAssignment);
        }

        /// <summary>
        /// Analyzes assignment expressions to identify direct access to properties that require review due to potential persistence issues.
        /// </summary>
        /// <param name="context">Operation context</param>
        /// <param name="targetProperties">A cache of the properties that require checking</param>
        private static void AnalyzeAssignment(
            OperationAnalysisContext context,
            ConcurrentDictionary<IPropertySymbol, byte> targetProperties)
        {
            var assignment = (ISimpleAssignmentOperation)context.Operation;

            if (assignment.Target is not IPropertyReferenceOperation propertyRef)
                return;

            var property = propertyRef.Property;

            // O(1) check, no symbol resolution
            if (!targetProperties.ContainsKey(property))
                return;

            // Skip assignments inside the declaring type
            var containingType = context.ContainingSymbol?.ContainingType;
            if (SymbolEqualityComparer.Default.Equals(containingType, property.ContainingType))
                return;

            // Issue a warning if the assignment is not within the allowed context.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Rule,
                    assignment.Syntax.GetLocation(),
                    property.Name));
        }

        // Check if that the type containing the property is derived from FormViewModel.
        private static bool IsFormViewModel(
            INamedTypeSymbol type,
            ConcurrentDictionary<INamedTypeSymbol, bool> cache)
        {
            return cache.GetOrAdd(type, static t =>
            {
                for (var current = t; current != null; current = current.BaseType)
                {
                    if (current.Name == "FormViewModel")
                        return true;
                }
                return false;
            });
        }
    }
}
