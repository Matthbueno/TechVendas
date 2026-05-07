using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TechVendas.Helpers;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.ViewModels
{
    public class ClientViewModel : ViewModelBase
    {
        // ==========================================
        // 1. SERVIÇOS E COLEÇÕES (VARIÁVEIS PRIVADAS)
        // ==========================================
        private JsonDataService<Client> _dataService;
        private JsonDataService<Pedido> _pedidoService;
        private ObservableCollection<Client> _clients;
        private ObservableCollection<Pedido> _pedidosDoClient;
        private Client _clientSelecionado;

        private string _filtroNome;
        private string _filtroCpf;

        private bool _filtroPedidosPendentes;
        private bool _filtroPedidosPagos;
        private bool _filtroPedidosEntregues;

        // ==========================================
        // 2. PROPRIEDADES DA TELA (BINDINGS)
        // ==========================================

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
            set
            {
                _clientSelecionado = value;
                OnPropertyChanged();
                FiltrarPedidos(); 
            }
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

        // ==========================================
        // 3. COMANDOS (BOTÕES DA TELA)
        // ==========================================
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ICommand AbrirPedidoCommand { get; }
        public ICommand MarcarPagoCommand { get; }
        public ICommand MarcarEnviadoCommand { get; }
        public ICommand MarcarRecebidoCommand { get; }

        // ==========================================
        // 4. CONSTRUTOR
        // ==========================================
        public ClientViewModel()
        {
            _dataService = new JsonDataService<Client>("clientes");
            _pedidoService = new JsonDataService<Pedido>("pedidos");

            CarregarDados();

            // Comandos de Clientes
            IncluirCommand = new RelayCommand(o => Incluir());
            SalvarCommand = new RelayCommand(o => Salvar(), o => ClientSelecionado != null);
            ExcluirCommand = new RelayCommand(o => Excluir(), o => ClientSelecionado != null);
            EditarCommand = new RelayCommand(o => { }, o => ClientSelecionado != null);

            // Comandos de Pedidos
            AbrirPedidoCommand = new RelayCommand(o => AbrirNovoPedido(), o => ClientSelecionado != null && ClientSelecionado.Id > 0);
            MarcarPagoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Pago"));
            MarcarEnviadoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Enviado"));
            MarcarRecebidoCommand = new RelayCommand(p => AlterarStatusPedido(p as Pedido, "Recebido"));
        }

        // ==========================================
        // 5. MÉTODOS DE LÓGICA DE CLIENTES
        // ==========================================
        private void CarregarDados()
        {
            var dados = _dataService.LoadData();
            Clients = new ObservableCollection<Client>(dados);
        }

        private void FiltrarClientes()
        {
            var todosClientes = _dataService.LoadData();
            var query = todosClientes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
            {
                query = query.Where(c => c.Nome != null && c.Nome.ToLower().Contains(FiltroNome.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(FiltroCpf))
            {
                query = query.Where(c => c.CPF != null && c.CPF.Contains(FiltroCpf));
            }

            Clients = new ObservableCollection<Client>(query.ToList());
        }

        private void Incluir()
        {
            int novoId = Clients.Any() ? Clients.Max(client => client.Id) + 1 : 1;
            ClientSelecionado = new Client { Id = novoId };
        }

        private void Salvar()
        {
            if (!IsCpfValido(ClientSelecionado.CPF))
            {
                MessageBox.Show("O CPF digitado é inválido! Por favor, verifique.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var lista = _dataService.LoadData();
            var existente = lista.FirstOrDefault(c => c.Id == ClientSelecionado.Id);

            if (existente != null)
            {
                existente.Nome = ClientSelecionado.Nome;
                existente.CPF = ClientSelecionado.CPF;
            }
            else
            {
                lista.Add(ClientSelecionado);
            }

            _dataService.SaveData(lista);
            MessageBox.Show("Cliente salvo com sucesso!");

            CarregarDados();
            ClientSelecionado = null;
        }

        private void Excluir()
        {
            if (MessageBox.Show("Deseja realmente excluir este cliente?", "Aviso", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var lista = _dataService.LoadData();
                var existente = lista.FirstOrDefault(c => c.Id == ClientSelecionado.Id);

                if (existente != null)
                {
                    lista.Remove(existente);
                    _dataService.SaveData(lista);
                    CarregarDados();
                    ClientSelecionado = null;
                }
            }
        }

        private bool IsCpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            // 1. Limpeza Garantida: Remove TUDO que não for número (pontos, traços, espaços, letras)
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // 2. Verifica se sobraram exatamente 11 números
            if (cpf.Length != 11) return false;

            // 3. Bloqueia CPFs com todos os números iguais (ex: 11111111111)
            if (new string(cpf[0], 11) == cpf) return false;

            // 4. Cálculo do 1º dígito verificador
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2) resto = 0;
            else resto = 11 - resto;

            // Verifica o primeiro dígito (posição 9)
            if (int.Parse(cpf[9].ToString()) != resto) return false;

            // 5. Cálculo do 2º dígito verificador
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2) resto = 0;
            else resto = 11 - resto;

            // Verifica o segundo dígito (posição 10)
            return int.Parse(cpf[10].ToString()) == resto;
        }

        // ==========================================
        // 6. MÉTODOS DE LÓGICA DE PEDIDOS
        // ==========================================
        private void FiltrarPedidos()
        {
            if (ClientSelecionado == null || ClientSelecionado.Id == 0)
            {
                PedidosDoClient = new ObservableCollection<Pedido>();
                return;
            }

            var todosPedidos = _pedidoService.LoadData();

            var query = todosPedidos.Where(p => p.Cliente != null && p.Cliente.Id == ClientSelecionado.Id).AsEnumerable();

            if (FiltroPedidosPendentes || FiltroPedidosPagos || FiltroPedidosEntregues)
            {
                query = query.Where(p =>
                    (FiltroPedidosPendentes && p.Status == "Pendente") ||
                    (FiltroPedidosPagos && p.Status == "Pago") ||
                    (FiltroPedidosEntregues && (p.Status == "Enviado" || p.Status == "Recebido"))
                );
            }

            PedidosDoClient = new ObservableCollection<Pedido>(query.ToList());
        }

        private void AlterarStatusPedido(Pedido pedido, string novoStatus)
        {
            if (pedido == null) return;

            var todosPedidos = _pedidoService.LoadData();
            var pedidoExistente = todosPedidos.FirstOrDefault(p => p.Id == pedido.Id);

            if (pedidoExistente != null)
            {
                pedidoExistente.Status = novoStatus;
                _pedidoService.SaveData(todosPedidos);
                FiltrarPedidos();
                MessageBox.Show($"Status alterado para '{novoStatus}'!");
            }
        }

        private void AbrirNovoPedido()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Content = new Views.PedidoView(ClientSelecionado);
            }
        }
    }
}