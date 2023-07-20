using System;
using System.ComponentModel;

namespace PerrijosGatijos.Models
{
    public class LoginModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string username;
        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
                }
            }
        }

        bool biometricAvailable;
        public bool BiometricAvailable
        {
            get { return biometricAvailable; }
            set
            {
                if (biometricAvailable != value)
                {
                    biometricAvailable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BiometricAvailable)));
                }
            }
        }

        string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
                }
            }
        }

        bool saveCredentials;
        public bool SaveCredentials
        {
            get { return saveCredentials; }
            set
            {
                if (saveCredentials != value)
                {
                    saveCredentials = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveCredentials)));
                }
            }
        }
    }
}

