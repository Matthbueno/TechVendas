using System;
using System.Collections.Generic;
using System.Linq;

namespace TechVendas.Models
{
    public class Pedido
    {
        //Dados cadastrais do produto e do cliente, além dos itens do pedido e
        // valor total, data da venda, forma de pagamento e status do pedido
        public int Id { get; set; }
        public Client Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public decimal ValorTotal { get; set; }
        public DateTime DataDaVenda { get; set; }
        public string FormaPagamento { get; set; }
        public string Status { get; set; }
    }
}
