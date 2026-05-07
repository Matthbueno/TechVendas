using System.Windows;
using TechVendas.Views; 

namespace TechVendas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Content = new Views.HomeView();
        }

        //Botão Voltar 
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se existe alguma tela anterior no histórico antes de voltar
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        //Botão Início (Joga a Tela Inicial no Frame)
        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Views.HomeView();
        }

        private void BtnProdutos_Click(object sender, RoutedEventArgs e)
        {
            // Instancia a tela de produtos e joga dentro do Frame
            MainFrame.Content = new ProdutoView();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            // Instancia a tela de pedidos e joga dentro do Frame
            MainFrame.Content = new PedidoView();
        }

        private void BtnClients_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new TechVendas.Views.ClientView();
        }
    }
}