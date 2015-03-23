using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicExplorer.Models
{
    /// <summary>
    /// Data model to hold all the needed data for a mix group.
    /// </summary>
    public class MixGroupModel : INotifyPropertyChanged
    {
        private string _name;
        /// <summary>
        /// MixGroup's Name property.
        /// This property is used in the UI to display its value using a Binding.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _id;
        /// <summary>
        /// MixGroup's Id property.
        /// This property is used in some of MixRadio API methods.
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

