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

namespace TechVendas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // O erro acontece porque este método provavelmente está faltando:
        private void BtnCadastros_Click(object sender, RoutedEventArgs e)
        {
            // Sua lógica de navegação aqui
            // Exemplo: MainFrame.Navigate(new PaginaCadastro());
        }

        private void BtnVendas_Click(object sender, RoutedEventArgs e)
        {
            // Sua lógica de navegação aqui
        }
    }
}