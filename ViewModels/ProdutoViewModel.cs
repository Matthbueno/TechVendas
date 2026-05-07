using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TechVendas.Helpers;
using TechVendas.Models;
using TechVendas.Services;

namespace TechVendas.ViewModels
{
    public class ProdutoViewModel : ViewModelBase
    {
        private JsonDataService<Produto> _dataService;
        private ObservableCollection<Produto> _produtos;
        private Produto _produtoSelecionado;

        // Lista que aparece no Grid
        public ObservableCollection<Produto> Produtos
        {
            get => _produtos;
            set { _produtos = value; OnPropertyChanged(); }
        }

        // Produto que o usuário clicou no Grid ou está incluindo
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(); }
        }

        // --- VARIÁVEIS DE BUSCA (FILTROS) ---
        private string _filtroNome;
        public string FiltroNome { get => _filtroNome; set { _filtroNome = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroCodigo;
        public string FiltroCodigo { get => _filtroCodigo; set { _filtroCodigo = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroValorMin;
        public string FiltroValorMin { get => _filtroValorMin; set { _filtroValorMin = value; OnPropertyChanged(); FiltrarProdutos(); } }

        private string _filtroValorMax;
        public string FiltroValorMax { get => _filtroValorMax; set { _filtroValorMax = value; OnPropertyChanged(); FiltrarProdutos(); } }

        // --- LÓGICA DO LINQ PARA BUSCAR ---
        private void FiltrarProdutos()
        {
            var todosProdutos = _dataService.LoadData();
            var query = todosProdutos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroNome))
                query = query.Where(p => p.Nome != null && p.Nome.ToLower().Contains(FiltroNome.ToLower()));

            if (!string.IsNullOrWhiteSpace(FiltroCodigo))
                query = query.Where(p => p.Codigo != null && p.Codigo.ToLower().Contains(FiltroCodigo.ToLower()));

            // Filtro de Faixa de Valor (Tenta converter o texto digitado para número decimal)
            if (decimal.TryParse(FiltroValorMin, out decimal min))
                query = query.Where(p => p.Valor >= min);

            if (decimal.TryParse(FiltroValorMax, out decimal max))
                query = query.Where(p => p.Valor <= max);

            // Atualiza o Grid
            Produtos = new ObservableCollection<Produto>(query.ToList());
        }

        // Comandos dos botões
        public ICommand IncluirCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand ExcluirCommand { get; }

        public ProdutoViewModel()
        {
            _dataService = new JsonDataService<Produto>("produtos");
            CarregarDados();

            IncluirCommand = new RelayCommand(o => Incluir());
            SalvarCommand = new RelayCommand(o => Salvar(), o => ProdutoSelecionado != null);
            ExcluirCommand = new RelayCommand(o => Excluir(), o => ProdutoSelecionado != null);

            // Apenas para garantir a seleção e fluxo no MVVM
            EditarCommand = new RelayCommand(o => { }, o => ProdutoSelecionado != null);
        }

        private void CarregarDados()
        {
            var dados = _dataService.LoadData();
            Produtos = new ObservableCollection<Produto>(dados);
        }

        private void Incluir()
        {
            // Cria um novo produto com um ID gerado automaticamente via LINQ
            int novoId = Produtos.Any() ? Produtos.Max(p => p.Id) + 1 : 1;
            ProdutoSelecionado = new Produto { Id = novoId, Valor = 0 };
        }

        private void Salvar()
        {
            // 1. BARREIRA DE VALIDAÇÃO
            // Verifica se os campos obrigatórios foram preenchidos
            if (string.IsNullOrWhiteSpace(ProdutoSelecionado.Nome) || string.IsNullOrWhiteSpace(ProdutoSelecionado.Codigo))
            {
                MessageBox.Show("O Nome e o Código do produto são obrigatórios!", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ProdutoSelecionado.Valor <= 0)
            {
                MessageBox.Show("O Valor do produto deve ser maior que zero!", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Carrega a lista e salva
            var lista = _dataService.LoadData();

            // Verifica se já existe um produto com este ID
            var existente = lista.FirstOrDefault(p => p.Id == ProdutoSelecionado.Id);

            if (existente != null)
            {
                // Atualiza (Edição)
                existente.Nome = ProdutoSelecionado.Nome;
                existente.Codigo = ProdutoSelecionado.Codigo;
                existente.Valor = ProdutoSelecionado.Valor;
            }
            else
            {
                // Inclui novo
                lista.Add(ProdutoSelecionado);
            }

            // 3. Salva no JSON e limpa a tela
            _dataService.SaveData(lista);
            MessageBox.Show("Produto salvo com sucesso!");

            CarregarDados(); // Atualiza o DataGrid
            ProdutoSelecionado = null; // Limpa as caixinhas de texto
        }

        private void Excluir()
        {
            if (MessageBox.Show("Deseja realmente excluir este produto?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var lista = _dataService.LoadData();
                var existente = lista.FirstOrDefault(p => p.Id == ProdutoSelecionado.Id);

                if (existente != null)
                {
                    lista.Remove(existente);
                    _dataService.SaveData(lista);
                    CarregarDados();
                    ProdutoSelecionado = null;
                }
            }
        }
    }
}