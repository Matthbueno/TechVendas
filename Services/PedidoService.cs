using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using TechVendas.Models;

namespace TechVendas.Services
{
    public class PedidoService  
    {
        private readonly string filePath = @"Data\pedidos.json";

        public List<Pedido> ObterTodas()
        {
            if (!File.Exists(filePath)) return new List<Pedido>();
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Pedido>>(json);
        }

        public void Salvar(List<Pedido> pedidos)
        {
            string json = JsonConvert.SerializeObject(pedidos, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        //Filtro de pedidos por nome do cliente ou id do pedido
        public List<Pedido> FiltrarPorNome(string nome)
        {
            var pedidos = ObterTodas();
            return pedidos.Where(pedido => pedido.Cliente.Nome.ToLower().Contains(nome.ToLower())).ToList();
        }

    }
}