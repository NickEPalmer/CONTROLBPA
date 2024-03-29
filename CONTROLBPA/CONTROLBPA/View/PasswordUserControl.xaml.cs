﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security;

namespace CONTROLBPA
{
    /// <summary>
    /// Interaction logic for PasswordUserControl.xaml
    /// https://stackoverflow.com/questions/15390727/passwordbox-and-mvvm/15391318#15391318
    /// </summary>
    public partial class PasswordUserControl : UserControl
    {
        public SecureString Password
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(SecureString), typeof(PasswordUserControl),
                new PropertyMetadata(default(SecureString)));


        public PasswordUserControl()
        {
            InitializeComponent();

            // Update DependencyProperty whenever the password changes
            PasswordBox.PasswordChanged += (sender, args) => {
                Password = ((PasswordBox)sender).SecurePassword;
            };
        }
    }
}
