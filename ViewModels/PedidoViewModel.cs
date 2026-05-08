using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TechVendas.Helpers;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.ViewModels
{
    public class PedidoViewModel : ViewModelBase
    {
        //SERVIÇOS DO DOMÍNIO
        private readonly PedidoService _pedidoService;
        private readonly ClientService _clienteService;
        private readonly ProdutoService _produtoService;

        //LISTAS DOS COMBOBOXES
        public ObservableCollection<Client> Clientes { get; set; }
        public ObservableCollection<Produto> Produtos { get; set; }
        public ObservableCollection<string> FormasPagamento { get; set; }

        //VARIÁVEIS DA TELA
        private Client _clienteSelecionado;
        public Client ClienteSelecionado { get => _clienteSelecionado; set { _clienteSelecionado = value; OnPropertyChanged(); } }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado { get => _produtoSelecionado; set { _produtoSelecionado = value; OnPropertyChanged(); } }

        private int _quantidade = 1;
        public int Quantidade { get => _quantidade; set { _quantidade = value; OnPropertyChanged(); } }

        private string _formaPagamentoSelecionada;
        public string FormaPagamentoSelecionada { get => _formaPagamentoSelecionada; set { _formaPagamentoSelecionada = value; OnPropertyChanged(); } }

        private ObservableCollection<ItemPedido> _itensPedido;
        public ObservableCollection<ItemPedido> ItensPedido { get => _itensPedido; set { _itensPedido = value; OnPropertyChanged(); } }

        public decimal ValorTotal => ItensPedido?.Sum(i => i.Subtotal) ?? 0;

        //COMANDOS
        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand SalvarPedidoCommand { get; }

        //CONSTRUTOR
        public PedidoViewModel(Client clientePreSelecionado = null)
        {
            // Instancia os nossos serviços de negócio
            _pedidoService = new PedidoService();
            _clienteService = new ClientService();
            _produtoService = new ProdutoService();

            // Pede as listas para as Services em vez de ler direto do JSON
            Clientes = new ObservableCollection<Client>(_clienteService.ObterTodos());
            Produtos = new ObservableCollection<Produto>(_produtoService.ObterTodos());
            FormasPagamento = new ObservableCollection<string> { "Dinheiro", "PIX", "Cartão de Crédito", "Cartão de Débito", "Boleto" };

            ItensPedido = new ObservableCollection<ItemPedido>();

            if (clientePreSelecionado != null)
            {
                ClienteSelecionado = Clientes.FirstOrDefault(c => c.Id == clientePreSelecionado.Id);
            }

            AdicionarItemCommand = new RelayCommand(o => AdicionarItem(), o => ProdutoSelecionado != null && Quantidade > 0);
            RemoverItemCommand = new RelayCommand(o => RemoverItem(o as ItemPedido));
            SalvarPedidoCommand = new RelayCommand(o => SalvarPedido(), o => ClienteSelecionado != null && ItensPedido.Any() && !string.IsNullOrWhiteSpace(FormaPagamentoSelecionada));
        }

        //MÉTODOS DE TELA (LÓGICA VISUAL)
        private void AdicionarItem()
        {
            if (ProdutoSelecionado == null || Quantidade <= 0) return;

            var itemExistente = ItensPedido.FirstOrDefault(i => i.Produto.Id == ProdutoSelecionado.Id);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += Quantidade;
                var index = ItensPedido.IndexOf(itemExistente);
                ItensPedido[index] = null;
                ItensPedido[index] = itemExistente;
            }
            else
            {
                ItensPedido.Add(new ItemPedido { Produto = ProdutoSelecionado, Quantidade = Quantidade });
            }

            OnPropertyChanged(nameof(ValorTotal));
            ProdutoSelecionado = null;
            Quantidade = 1;
        }

        private void RemoverItem(ItemPedido item)
        {
            if (item != null)
            {
                ItensPedido.Remove(item);
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        private void SalvarPedido()
        {
            // O ViewModel anota o que o usuário quer...
            var novoPedido = new Pedido
            {
                Cliente = ClienteSelecionado,
                Itens = ItensPedido.ToList(),
                ValorTotal = ValorTotal,
                FormaPagamento = FormaPagamentoSelecionada
            };

            // ... e repassa para a Service criar de fato (gerar ID, Data, e Salvar no JSON)
            var resultado = _pedidoService.Salvar(novoPedido);

            if (!resultado.Sucesso)
            {
                MessageBox.Show(resultado.Mensagem, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(resultado.Mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpa a tela para a próxima venda
            ItensPedido.Clear();
            ClienteSelecionado = null;
            FormaPagamentoSelecionada = null;
            OnPropertyChanged(nameof(ValorTotal));
        }
    }
}