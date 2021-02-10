using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Deptorygen2.Core.Aggregation
{
	internal record ReturnTypeToAnalyze(TypeSyntax Syntax, INamedTypeSymbol Symbol)
	{
	}
}
