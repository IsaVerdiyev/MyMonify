﻿using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace MyMonify.Tools
{
    abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string prop = "")
        {
            field = value;
            OnPropertyChanged(prop);
        }
    }
}
