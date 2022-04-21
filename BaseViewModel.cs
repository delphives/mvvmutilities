using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace yyy
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void FirePropertyChanged([CallerMemberName] object propertyName = null)
        {
            if (PropertyChanged != null)
            {
                //si le nameof ramène le nom de la variable et pas du param
                string property = nameof(propertyName);
                if (property == "propertyName")
                    property = propertyName.ToString();
                PropertyChanged(this, new PropertyChangedEventArgs(property));

            }
        }
    }
}
