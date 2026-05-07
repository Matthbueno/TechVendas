using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TechVendas.Helpers;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.ViewModels
{
    public class ProdutoViewModel : ViewModelBase
    {
        private readonly ProdutoService _produtoService;

        private ObservableCollection<Produto> _produtos;
        private Produto _produtoSelecionado;

        public ObservableCollection<Produto> Produtos
        {
            get => _produtos;
            set { _produtos = value; OnPropertyChanged(); }
        }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(); }
        }

        //VARIÁVEIS DE BUSCA
        private string _filtroNome;
        public string FiltroNome { get => _filtroNome; set { _filtroNome = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroCodigo;
        public string FiltroCodigo { get => _filtroCodigo; set { _filtroCodigo = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroValorMin;
        public string FiltroValorMin { get => _filtroValorMin; set { _filtroValorMin = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroValorMax;
        public string FiltroValorMax { get => _filtroValorMax; set { _filtroValorMax = value; OnPropertyChanged(); FiltrarProdutos(); } }

        //COMANDOS DOS BOTÕES
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ProdutoViewModel()
        {
            _produtoService = new ProdutoService();
            CarregarDados();

            IncluirCommand = new RelayCommand(o => Incluir());
            SalvarCommand = new RelayCommand(o => Salvar(), o => ProdutoSelecionado != null);
            ExcluirCommand = new RelayCommand(o => Excluir(), o => ProdutoSelecionado != null);
            EditarCommand = new RelayCommand(o => { }, o => ProdutoSelecionado != null);
        }

        // --- MÉTODOS DA TELA (SOMENTE UI LÓGICA) ---
        private void CarregarDados()
        {
            Produtos = new ObservableCollection<Produto>(_produtoService.ObterTodos());
        }

        private void FiltrarProdutos()
        {
            var resultado = _produtoService.FiltrarProdutos(FiltroNome, FiltroCodigo, FiltroValorMin, FiltroValorMax);
            Produtos = new ObservableCollection<Produto>(resultado);
        }

        private void Incluir()
        {
            // O ViewModel apenas cria o objeto para preencher na tela. A Service gera o ID depois.
            ProdutoSelecionado = new Produto { Valor = 0 };
        }

        private void Salvar()
        {
            var resultado = _produtoService.Salvar(ProdutoSelecionado);

            if (!resultado.Sucesso)
            {
                MessageBox.Show(resultado.Mensagem, "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(resultado.Mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            CarregarDados();
            ProdutoSelecionado = null;
        }

        private void Excluir()
        {
            if (MessageBox.Show("Deseja realmente excluir este produto?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _produtoService.Excluir(ProdutoSelecionado.Id);
                CarregarDados();
                ProdutoSelecionado = null;
            }
        }
    }
}