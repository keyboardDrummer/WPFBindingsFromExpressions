namespace WPFExperimentTests
{
	class SubItem : Item
	{
		int anotherProperty;
		
		public SubItem(bool isChecked)
			: base(isChecked)
		{
		}

		public int AnotherProperty
		{
			get { return anotherProperty; }
			set { anotherProperty = value; }
		}
	}
}