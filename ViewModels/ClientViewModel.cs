using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechVendas.Helpers;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        //DEPENDÊNCIAS E VARIÁVEIS
        private readonly ClientService _clientService;
        private readonly PedidoService _pedidoService;

        private ObservableCollection<Client> _clients;
        private ObservableCollection<Pedido> _pedidosDoClient;
        private Client _clientSelecionado;

        private string _filtroNome;
        private string _filtroCpf;
        private bool _filtroPedidosPendentes;
        private bool _filtroPedidosPagos;
        private bool _filtroPedidosEntregues;

        //BINDINGS DA TELA
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set { _clients = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Pedido> PedidosDoClient
        {
            get => _pedidosDoClient;
            set { _pedidosDoClient = value; OnPropertyChanged(); }
        }

        public Client ClientSelecionado
        {
            get => _clientSelecionado;
            set { _clientSelecionado = value; OnPropertyChanged(); FiltrarPedidos(); }
        }

        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); FiltrarClientes(); }
        }

        public string FiltroCpf
        {
            get => _filtroCpf;
            set { _filtroCpf = value; OnPropertyChanged(); FiltrarClientes(); }
        }

        public bool FiltroPedidosPendentes
        {
            get => _filtroPedidosPendentes;
            set { _filtroPedidosPendentes = value; OnPropertyChanged(); FiltrarPedidos(); }
        }

        public bool FiltroPedidosPagos
        {
            get => _filtroPedidosPagos;
            set { _filtroPedidosPagos = value; OnPropertyChanged(); FiltrarPedidos(); }
        }

        public bool FiltroPedidosEntregues
        {
            get => _filtroPedidosEntregues;
            set { _filtroPedidosEntregues = value; OnPropertyChanged(); FiltrarPedidos(); }
        }

        //ICOMMANDS
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand AbrirPedidoCommand { get; }
        public ICommand MarcarPagoCommand { get; }
        public ICommand MarcarEnviadoCommand { get; }
        public ICommand MarcarRecebidoCommand { get; }

        //CONSTRUTOR
        public ClientViewModel()
        {
            _clientService = new ClientService();
            _pedidoService = new PedidoService();

            CarregarDados();

            IncluirCommand = new RelayCommand(o => Incluir());
            SalvarCommand = new RelayCommand(o => Salvar(), o => ClientSelecionado != null);
            ExcluirCommand = new RelayCommand(o => Excluir(), o => ClientSelecionado != null);
            EditarCommand = new RelayCommand(o => { }, o => ClientSelecionado != null); 

            AbrirPedidoCommand = new RelayCommand(o => AbrirNovoPedido(), o => ClientSelecionado != null && ClientSelecionado.Id > 0);
            MarcarPagoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Pago"));
            MarcarEnviadoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Enviado"));
            MarcarRecebidoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Recebido"));
        }

        //MÉTODOS DA TELA (SOMENTE UI LÓGICA)
        private void CarregarDados()
        {
            Clients = new ObservableCollection<Client>(_clientService.ObterTodos());
        }

        private void FiltrarClientes()
        {
            var resultado = _clientService.FiltrarClientes(FiltroNome, FiltroCpf);
            Clients = new ObservableCollection<Client>(resultado);
        }

        private void Incluir()
        {
            ClientSelecionado = new Client();
        }

        private void Salvar()
        {
            var resultado = _clientService.Salvar(ClientSelecionado);

            if (!resultado.Sucesso)
            {
                MessageBox.Show(resultado.Mensagem, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(resultado.Mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            CarregarDados();
            ClientSelecionado = null;
        }

        private void Excluir()
        {
            if (MessageBox.Show("Deseja realmente excluir este cliente?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _clientService.Excluir(ClientSelecionado.Id);
                CarregarDados();
                ClientSelecionado = null;
            }
        }

        private void FiltrarPedidos()
        {
            if (ClientSelecionado == null || ClientSelecionado.Id == 0)
            {
                PedidosDoClient = new ObservableCollection<Pedido>();
                return;
            }

            var resultado = _pedidoService.FiltrarPorCliente(
                ClientSelecionado.Id, FiltroPedidosPendentes, FiltroPedidosPagos, FiltroPedidosEntregues);

            PedidosDoClient = new ObservableCollection<Pedido>(resultado);
        }

        private void AlterarStatusPedido(Pedido pedido, string novoStatus)
        {
            if (pedido == null) return;

            if (_pedidoService.AlterarStatus(pedido.Id, novoStatus))
            {
                FiltrarPedidos();
                MessageBox.Show($"Status alterado para '{novoStatus}'.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AbrirNovoPedido()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = new Views.PedidoView(ClientSelecionado);
            }
        }
    }
}