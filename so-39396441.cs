using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;
using System;

namespace so39396441
{
    internal class Walker : CSharpSyntaxWalker
    {
        public SemanticModel Model { get; set; }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var o = (IAssignmentExpression)Model.GetOperation(node);
            if (o.Value.Kind == OperationKind.ConversionExpression)
            {
                var c = (IConversionExpression)o.Value;
                Console.WriteLine($"{(c.IsExplicit ? "explicit" : "implicit")} conversion from {c.Operand.ConstantValue.Value} via {(c.UsesOperatorMethod ? c.OperatorMethod.ToString() : "?")}");
            }
            base.VisitAssignmentExpression(node);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
public class Program
{
    public static void Main()
    {
        int? i;
        i = 1;
    }
}");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(null, new[] { tree }, new[] { mscorlib });
            var walker = new Walker { Model = compilation.GetSemanticModel(tree) };
            walker.Visit(tree.GetRoot());
        }
    }
}
