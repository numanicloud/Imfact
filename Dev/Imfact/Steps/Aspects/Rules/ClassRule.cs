using System.Collections.Generic;
using System.Linq;
using Imfact.Entities;
using Imfact.Main;
using Imfact.Steps.Ranking;
using Imfact.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Imfact.Steps.Aspects.Rules
{
	internal class ClassRule
	{
		private readonly GenerationContext _genContext;
		private readonly MethodRule _methodRule;
		private readonly PropertyRule _propertyRule;

		public ClassRule(GenerationContext genContext, MethodRule methodRule,
			PropertyRule propertyRule)
		{
			_genContext = genContext;
			_methodRule = methodRule;
			_propertyRule = propertyRule;
		}

		public ClassAspect Aggregate(RankedClass root)
		{
			return ExtractThis(root.Syntax, root.Symbol, ExtractBases(root.Symbol));
		}

		private ClassAspect ExtractThis
			(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol, ClassAspect[] baseClasses)
		{
			return new(TypeAnalysis.FromSymbol(symbol),
				baseClasses,
				GetMethodAspects(symbol, partialOnly: true),
				GetPropertyAspects(syntax), null);
		}

		// NOTE: 基底クラスのプロパティを追う必要は無いのでは？
		// 逆に、コンストラクタはこちらにしか無いので、BaseClassAspect みたいな型で区別してもいいかも
		private ClassAspect ExtractBase
			(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol)
		{
			return new(
				TypeAnalysis.FromSymbol(symbol),
				new ClassAspect[0],
				GetMethodAspects(symbol, false),
				GetPropertyAspects(syntax),
				GetConstructor(symbol));
		}

		private ClassAspect[] ExtractBases(INamedTypeSymbol thisSymbol)
		{
			return TraverseBase(thisSymbol)
				.Select(x => GetSyntax(x)?.Pair(x))
				.FilterNull()
				.Select(x => ExtractBase(x.Item1, x.Item2))
				.ToArray();

			static ClassDeclarationSyntax? GetSyntax(INamedTypeSymbol x)
			{
				return x.DeclaringSyntaxReferences
					.Select(y => y.GetSyntax())
					.OfType<ClassDeclarationSyntax>()
					.FirstOrDefault();
			}

			static IEnumerable<INamedTypeSymbol> TraverseBase(INamedTypeSymbol pivot)
			{
				if (pivot.BaseType is not null)
				{
					yield return pivot.BaseType;
					foreach (var baseSymbol in TraverseBase(pivot.BaseType))
					{
						yield return baseSymbol;
					}
				}
			}
		}

		private MethodAspect[] GetMethodAspects(INamedTypeSymbol @class, bool partialOnly)
		{
			return @class.GetMembers()
				.OfType<IMethodSymbol>()
				.Select(x =>
				{
					var mm = x.DeclaringSyntaxReferences
						.Select(m => m.GetSyntax())
						.OfType<MethodDeclarationSyntax>()
						.FirstOrDefault();
					return mm is null ? null : _methodRule.ExtractAspect(mm, x, partialOnly);
				})
				.FilterNull()
				.ToArray();
		}

		private PropertyAspect[] GetPropertyAspects(ClassDeclarationSyntax @class)
		{
			return @class.Members
				.OfType<PropertyDeclarationSyntax>()
				.Select(_propertyRule.ExtractAspect)
				.FilterNull()
				.ToArray();
		}

		private ConstructorAspect GetConstructor(INamedTypeSymbol symbol)
		{
			var classType = TypeAnalysis.FromSymbol(symbol);
			var ctor = _genContext.Constructors[classType.Id];
			var parameters = ctor.Parameters
				.Select(x => new ParameterAspect(x.Type, x.Name))
				.ToArray();

			return new ConstructorAspect(ctor.Accessibility, parameters);
		}
	}
}
