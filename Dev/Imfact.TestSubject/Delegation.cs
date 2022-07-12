using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imfact.Annotations;

namespace Sample;

class IdContext
{
	public IdContext(int id)
	{
	}
}

class SilverPhase
{
}

class GoldPhase
{
	public GoldPhase(SilverPhase silver, IdContext context)
	{
	}
}

[Factory]
partial class SilverFactory
{
	[Cache]
	public partial SilverPhase GetSilver();
}

[Factory]
partial class GoldFactory
{
	private SilverFactory BaseFactory { get; }
	[Cache]
	public partial GoldPhase GetGold();
}