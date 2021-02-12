using System.Collections.Generic;
using Deptorygen2.Core.Steps.Definitions;
using Deptorygen2.Core.Steps.Semanticses;
using Deptorygen2.Core.Utilities;
using NacHelpers.Extensions;

namespace Deptorygen2.Core.Steps.Instantiation.CreationMethods
{
	internal class FactoryItselfCreation : CreationMethodBase<FactorySemantics>
	{
		public FactoryItselfCreation(SourceCodeDefinition definition) : base(factory, fields)
		{
		}

		public override InstantiationMethod Method => InstantiationMethod.FactoryItself;

		protected override string GetCreationCode(FactorySemantics resolution,
			ResolverParameterDefinition[] given,
			IInstantiationResolver resolver)
		{
			return "this";
		}

		protected override IEnumerable<FactorySemantics> GetSource(SourceCodeDefinition definition)
		{
			return factory.WrapByArray();
		}

		protected override TypeName GetTypeInfo(FactorySemantics source)
		{
			return TypeName.FromSymbol(source.ItselfSymbol);
		}
	}
}
