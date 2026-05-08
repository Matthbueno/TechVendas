using System;
using System.Collections.Generic;
using System.Linq;

namespace TechVendas.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public Client Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public decimal ValorTotal { get; set; }
        public DateTime DataDaVenda { get; set; }
        public string FormaPagamento { get; set; }
        public string Status { get; set; }
    }
}
