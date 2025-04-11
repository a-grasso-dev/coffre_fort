using System.Windows;
using System.Windows.Controls;

namespace Coffre_fort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PasswordViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new PasswordViewModel();
            DataContext = _viewModel;
        }

        private void MotDePasseBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PasswordViewModel vm && sender is PasswordBox pb)
            {
                vm.MotDePasse = pb.Password;
            }
        }

    }
}