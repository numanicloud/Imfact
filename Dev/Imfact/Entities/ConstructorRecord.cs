using Microsoft.CodeAnalysis;

namespace Imfact.Entities;

internal record ConstructorRecord(TypeAnalysis ClassType, Accessibility Accessibility, ParameterRecord[] Parameters);
