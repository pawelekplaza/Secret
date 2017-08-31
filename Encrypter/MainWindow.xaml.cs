using Encrypter.ViewModels;
using System;
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

namespace Encrypter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

        }

        private void secretKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            var s = sender as PasswordBox;
            if (s == secretReadKeyPasswordBox || s == secondSecretReadKeyPasswordBox)
            {
                viewModel.InvokeReadTextFileCommand();
            }

            if (s == secretWriteKeyPasswordBox || s == secondSecretWriteKeyPasswordBox)
            {
                viewModel.InvokeSaveFileCommand();
            }
        }

        private void secretKeyChanged(object sender, RoutedEventArgs e)
        {
            var s = sender as PasswordBox;
            if (s == secretReadKeyPasswordBox)
            {
                viewModel.SecretReadKey = s.Password;
            }

            if (s == secondSecretReadKeyPasswordBox)
            {
                viewModel.SecondSecretReadKey = s.Password;
            }

            if (s == secretWriteKeyPasswordBox)
            {
                viewModel.SecretWriteKey = s.Password;
            }

            if (s == secondSecretWriteKeyPasswordBox)
            {
                viewModel.SecondSecretWriteKey = s.Password;
            }            
        }
    }
}
