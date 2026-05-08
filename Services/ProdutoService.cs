using System.Collections.Generic;
using System.Linq;
using TechVendas.Models;
using TechVendas.Data;

namespace TechVendas.Services
{
    public class ProdutoService
    {
        private readonly JsonDataService<Produto> _dataService;

        public ProdutoService()
        {
            _dataService = new JsonDataService<Produto>("produtos");
        }

        public List<Produto> ObterTodos()
        {
            return _dataService.LoadData() ?? new List<Produto>();
        }

        public List<Produto> FiltrarProdutos(string nome, string codigo, string valorMin, string valorMax)
        {
            var query = ObterTodos().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Nome != null && p.Nome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(codigo))
                query = query.Where(p => p.Codigo != null && p.Codigo.ToLower().Contains(codigo.ToLower()));

            if (decimal.TryParse(valorMin, out decimal min))
                query = query.Where(p => p.Valor >= min);

            if (decimal.TryParse(valorMax, out decimal max))
                query = query.Where(p => p.Valor <= max);

            return query.ToList();
        }

        public (bool Sucesso, string Mensagem) Salvar(Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome) || string.IsNullOrWhiteSpace(produto.Codigo))
                return (false, "O Nome e o Código do produto são obrigatórios!");

            if (produto.Valor <= 0)
                return (false, "O Valor do produto deve ser maior que zero!");

            var lista = ObterTodos();
            var existente = lista.FirstOrDefault(p => p.Id == produto.Id);

            if (existente != null)
            {
                existente.Nome = produto.Nome;
                existente.Codigo = produto.Codigo;
                existente.Valor = produto.Valor;
            }
            else
            {
                produto.Id = lista.Any() ? lista.Max(p => p.Id) + 1 : 1;
                lista.Add(produto);
            }

            _dataService.SaveData(lista);
            return (true, "Produto salvo com sucesso!");
        }

        public void Excluir(int id)
        {
            var lista = ObterTodos();
            var existente = lista.FirstOrDefault(p => p.Id == id);

            if (existente != null)
            {
                lista.Remove(existente);
                _dataService.SaveData(lista);
            }
        }
    }
}