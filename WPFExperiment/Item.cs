using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFExperiment.Annotations;

namespace WPFExperiment
{
    public class Item : INotifyPropertyChanged
    {
        bool isChecked;

        public Item(bool isChecked)
        {
            this.isChecked = isChecked;
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; OnPropertyChanged();}
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