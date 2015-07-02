using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFExperiment.Properties;

namespace WPFExperimentTests
{
	public class Item : INotifyPropertyChanged
	{
		Item childItem;
		bool isChecked;

		public Item(bool isChecked)
		{
			this.isChecked = isChecked;
		}

		public bool IsChecked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
				OnPropertyChanged();
			}
		}

		public Item ChildItem
		{
			get { return childItem; }
			set
			{
				childItem = value;
				childItem.PropertyChanged += PropertyChanged;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}