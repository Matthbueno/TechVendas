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
    public class PedidoViewModel : ViewModelBase
    {
        // Serviços de Arquivo (Precisa ler Cliente e Produto, e Salvar Pedido)
        private JsonDataService<Pedido> _pedidoService;
        private JsonDataService<Client> _clienteService;
        private JsonDataService<Produto> _produtoService;

        // Listas para os ComboBoxes (Menus suspensos) da tela
        public ObservableCollection<Client> Clientes { get; set; }
        public ObservableCollection<Produto> Produtos { get; set; }
        public ObservableCollection<string> FormasPagamento { get; set; }


        // Variáveis da tela
        private Client _clienteSelecionado;
        public Client ClienteSelecionado { get => _clienteSelecionado; set { _clienteSelecionado = value; OnPropertyChanged(); } }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado { get => _produtoSelecionado; set { _produtoSelecionado = value; OnPropertyChanged(); } }

        private int _quantidade = 1;
        public int Quantidade { get => _quantidade; set { _quantidade = value; OnPropertyChanged(); } }

        private string _formaPagamentoSelecionada;
        public string FormaPagamentoSelecionada { get => _formaPagamentoSelecionada; set { _formaPagamentoSelecionada = value; OnPropertyChanged(); } }

        // Carrinho de Compras
        private ObservableCollection<ItemPedido> _itensPedido;
        public ObservableCollection<ItemPedido> ItensPedido { get => _itensPedido; set { _itensPedido = value; OnPropertyChanged(); } }

        // Toda vez que a tela pedir o ValorTotal, ele faz a soma de todas as linhas do carrinho
        public decimal ValorTotal => ItensPedido?.Sum(i => i.Subtotal) ?? 0;

        // Comandos dos Botões
        public ICommand AdicionarItemCommand { get; }
        public ICommand RemoverItemCommand { get; }
        public ICommand SalvarPedidoCommand { get; }

        // Construtor: Prepara a tela quando ela abre
        public PedidoViewModel(Client clientePreSelecionado = null)
        {
            _pedidoService = new JsonDataService<Pedido>("pedidos");
            _clienteService = new JsonDataService<Client>("clientes");
            _produtoService = new JsonDataService<Produto>("produtos");

            // Carrega os dados salvos nos JSONs
            Clientes = new ObservableCollection<Client>(_clienteService.LoadData());
            Produtos = new ObservableCollection<Produto>(_produtoService.LoadData());
            FormasPagamento = new ObservableCollection<string> { "Dinheiro", "PIX", "Cartão de Crédito", "Cartão de Débito", "Boleto" };

            ItensPedido = new ObservableCollection<ItemPedido>();

            // Se a tela foi aberta clicando num cliente específico, já deixa ele selecionado!
            if (clientePreSelecionado != null)
            {
                ClienteSelecionado = Clientes.FirstOrDefault(c => c.Id == clientePreSelecionado.Id);
            }

            // Comandos
            AdicionarItemCommand = new RelayCommand(o => AdicionarItem(), o => ProdutoSelecionado != null && Quantidade > 0);
            RemoverItemCommand = new RelayCommand(o => RemoverItem(o as ItemPedido));
            SalvarPedidoCommand = new RelayCommand(o => SalvarPedido(), o => ClienteSelecionado != null && ItensPedido.Any() && !string.IsNullOrWhiteSpace(FormaPagamentoSelecionada));
        }

        private void AdicionarItem()
        {
            if (ProdutoSelecionado == null || Quantidade <= 0) return;

            var itemExistente = ItensPedido.FirstOrDefault(i => i.Produto.Id == ProdutoSelecionado.Id);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += Quantidade;
                // Truque para o DataGrid atualizar a linha que já existe:
                var index = ItensPedido.IndexOf(itemExistente);
                ItensPedido[index] = null; // Reseta brevemente
                ItensPedido[index] = itemExistente;
            }
            else
            {
                ItensPedido.Add(new ItemPedido
                {
                    Produto = ProdutoSelecionado,
                    Quantidade = Quantidade
                });
            }

            // MANDATÓRIO: Avisa a tela que o Total do Pedido mudou e precisa ser redesenhado
            OnPropertyChanged(nameof(ValorTotal));

            // Limpa os campos de seleção
            ProdutoSelecionado = null;
            Quantidade = 1;
        }

        private void RemoverItem(ItemPedido item)
        {
            if (item != null)
            {
                ItensPedido.Remove(item);

                // Sempre que remover, tem que avisar a tela pra recalcular o total!
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        private void SalvarPedido()
        {
            var todosPedidos = _pedidoService.LoadData();
            int novoId = todosPedidos.Any() ? todosPedidos.Max(p => p.Id) + 1 : 1;

            var novoPedido = new Pedido
            {
                Id = novoId,
                Cliente = ClienteSelecionado,
                Itens = ItensPedido.ToList(),
                ValorTotal = ValorTotal,
                DataDaVenda = DateTime.Now,
                FormaPagamento = FormaPagamentoSelecionada,
                Status = "Pendente"
            };

            todosPedidos.Add(novoPedido);
            _pedidoService.SaveData(todosPedidos);

            MessageBox.Show("Venda registrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpa a tela para a próxima venda
            ItensPedido.Clear();
            ClienteSelecionado = null;
            FormaPagamentoSelecionada = null;
            OnPropertyChanged(nameof(ValorTotal));
        }
    }
}