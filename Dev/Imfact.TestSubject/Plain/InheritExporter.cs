using Imfact.Annotations;

namespace Imfact.TestSubject.InheritExporter;

internal class GoldService
{
}

internal class SilverService
{
}

[Factory]
internal partial class InheritExporterFactoryBase
{
	public partial GoldService ResolveService();
}


[Factory]
internal partial class InheritExporterFactory : InheritExporterFactoryBase
{
	protected internal partial SilverService ResolveSilverService();

	partial void ManuallyExport(IServiceImporter importer)
	{
		importer.Import<SilverService>(() => ResolveSilverService());
	}
}

internal class Factory : InheritExporterFactory
{
	internal override void Export(IServiceImporter importer)
	{
		base.Export(importer);
	}
}