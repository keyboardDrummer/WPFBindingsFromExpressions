using System.ComponentModel;

namespace WPFBindingGeneration
{
	// ReSharper disable once ConvertToStaticClass
	public sealed class Unit
	{
		public static readonly Unit Instance = new Unit();

		private Unit()
		{
		}
	}
}