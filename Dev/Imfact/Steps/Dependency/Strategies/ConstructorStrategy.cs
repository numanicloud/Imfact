﻿using System.Collections.Generic;
using System.Linq;
using Imfact.Steps.Dependency.Components;
using Imfact.Steps.Dependency.Interfaces;
using Imfact.Steps.Semanticses.Records;

namespace Imfact.Steps.Dependency.Strategies
{
	class ConstructorStrategy : IExpressionStrategy
	{
		public ICreationNode? GetExpression(CreationContext context)
		{
			var ttr = context.TypeToResolve[0].Id;

			var resolution = context.Caller.Resolutions
				.FirstOrDefault(x => x.TypeName.Id == ttr);

			if (resolution is null && context.Caller is Resolver single
				&& single.ActualResolution.TypeName.Id == ttr)
			{
				resolution = single.ActualResolution;
			}

			if (resolution is not null)
			{
				return new Invocation($"new {ttr.Name}", GetArgs(resolution, context).ToArray());
			}

			return null;
		}

		private IEnumerable<ICreationNode> GetArgs(Resolution resolution, CreationContext context)
		{
			var childContext = context with
			{
				TypeToResolve = resolution.Dependencies
			};
			return context.Injector.GetExpression(childContext);
		}
	}
}
