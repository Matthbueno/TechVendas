using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using TechVendas.Models;

namespace TechVendas.Services
{
    public class ProdutoService 
    {
        private readonly string filePath = @"Data\produtos.json";

        public List<Produto> ObterTodas()
        {
            if (!File.Exists(filePath)) return new List<Produto>();
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Produto>>(json);
        }

        public void Salvar(List<Produto> produtos)
        {
            string json = JsonConvert.SerializeObject(produtos, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Filtro de produtos por nome ou id
        public List<Produto> FiltrarPorNome(string nome)
        {
            var produtos = ObterTodas();
            return produtos.Where(produto => produto.Nome.ToLower().Contains(nome.ToLower())).ToList();
        }

        public List<Produto> FiltrarPorId(int id)
        {
            var produtos = ObterTodas();
            return produtos.Where(produto => produto.Id.ToString().Contains(id.ToString())).ToList();
        }
    }
}