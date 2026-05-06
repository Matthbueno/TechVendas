using System;
using System.Collections.Generic;
using System.Linq;

namespace TechVendas.Models
{
    public class Pedido
    {
        //Dados cadastrais do produto e do cliente, além dos itens do pedido
        public Guid Id { get; set; } = Guid.NewGuid();
        public Client Client { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();

        //Valor Total é calculado somando o subtotal de cada item do pedido
        public decimal ValorTotal => Itens.Sum(item => item.SubTotal);

        //Dados gerais do pedido, como data, forma de pagamento e status
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public string FormaDePagamento { get; set; }
        public string Status { get; set; } = "Pendente";
    }
}
