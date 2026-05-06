using System.Windows;
using TechVendas.Views; // Supondo que você salvou as telas na pasta Views

namespace TechVendas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Isso faz a HomeView carregar automaticamente assim que o programa abre!
            MainFrame.Content = new Views.HomeView();
        }

        // NOVO: Botão Voltar (Usa o histórico do Frame)
        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica se existe alguma tela anterior no histórico antes de voltar
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }

        // NOVO: Botão Início (Joga a Tela Inicial no Frame)
        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Views.HomeView();
        }


        private void BtnProdutos_Click(object sender, RoutedEventArgs e)
        {
            // Instancia a tela de produtos e joga dentro do Frame
            MainFrame.Content = new ClientView();
        }

        private void BtnPedidos_Click(object sender, RoutedEventArgs e)
        {
            // Instancia a tela de pedidos e joga dentro do Frame
            MainFrame.Content = new ClientView();
        }

        private void BtnClients_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainFrame.Content = new TechVendas.Views.ClientView(); // Ou o nome que você deu
        }
    }
}