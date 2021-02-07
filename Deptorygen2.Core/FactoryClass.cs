using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deptorygen2.Core
{
	public enum AccessLevel
	{
		Public, Protected, Private, Internal,
		ProtectedInternal, PrivateProtected
	}

	public record FactoryClass(string Name, FactoryMethod[] Methods, DependencyField[] Fields);

	public record DependencyField(TypeInfo Type, string Name);

	public record FactoryMethod(AccessLevel AccessLevel,
		string Name,
		TypeInfo ReturnType,
		ResolutionConstructor Resolution,
		FactoryMethodParameter[] Parameters,
		HookAnnotation[] Hooks);

	public record TypeInfo(string Namespace, string Name);

	public record FactoryMethodParameter(TypeInfo Type, string Name);

	public record FactoryMethodAttribute(string Name, string[] Arguments);

	public record HookAnnotation(TypeInfo HookClass);

	public record ResolutionConstructor(TypeInfo ResolutionType, string[] Arguments);
}
