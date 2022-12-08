using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Imfact;

internal class DebugHelper
{
    private static bool IsAttached = false;

    public static void Attach()
    {
        if (!Debugger.IsAttached && !IsAttached)
        {
            Debugger.Launch();

            IsAttached = true;
        }
		Debugger.Break();
    }

    public static Diagnostic Info(string id, string title, string message)
    {
        return DiagnosticsFallback(id, title, message, DiagnosticSeverity.Info);
    }

    public static Diagnostic Error(string id, string title, string message)
    {
        return DiagnosticsFallback(id, title, message, DiagnosticSeverity.Error);
    }

    private static Diagnostic DiagnosticsFallback(
        string id,
        string title,
        string message,
        DiagnosticSeverity severity)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(id, title, message, "Imfact", severity, true), null);
    }
}
