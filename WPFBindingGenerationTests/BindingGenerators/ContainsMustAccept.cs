using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFExperiment.Properties;

namespace WPFExperimentTests.BindingGenerators
{
	class ContainsMustAccept : INotifyPropertyChanged
	{
		public virtual bool MustAccept
		{
			get { return true; }
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