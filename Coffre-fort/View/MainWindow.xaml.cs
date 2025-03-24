using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Coffre_fort.View_model;

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

        private void AddPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddPassword(NomCompteBox.Text, MotDePasseBox.Password);
            NomCompteBox.Clear();
            MotDePasseBox.Clear();
        }

        private void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordListBox.SelectedItem is PasswordEntry selected)
            {
                _viewModel.UpdatePassword(selected, NewPasswordBox.Password);
                NewPasswordBox.Clear();
            }
        }
    }
}