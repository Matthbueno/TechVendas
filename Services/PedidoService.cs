using System;
using System.Collections.Generic;
using System.Linq;
using TechVendas.Models;
using TechVendas.Data;

namespace TechVendas.Services
{
    public class PedidoService
    {
        private readonly JsonDataService<Pedido> _dataService;

        public PedidoService()
        {
            _dataService = new JsonDataService<Pedido>("pedidos");
        }

        public List<Pedido> ObterTodos()
        {
            return _dataService.LoadData() ?? new List<Pedido>();
        }

        public List<Pedido> FiltrarPorCliente(int clienteId, bool pendentes, bool pagos, bool entregues)
        {
            var query = ObterTodos().Where(p => p.Cliente != null && p.Cliente.Id == clienteId);

            if (pendentes || pagos || entregues)
            {
                query = query.Where(p =>
                    (pendentes && p.Status == "Pendente") ||
                    (pagos && p.Status == "Pago") ||
                    (entregues && (p.Status == "Enviado" || p.Status == "Recebido"))
                );
            }

            return query.ToList();
        }

        public bool AlterarStatus(int pedidoId, string novoStatus)
        {
            var lista = ObterTodos();
            var pedido = lista.FirstOrDefault(p => p.Id == pedidoId);

            if (pedido != null)
            {
                pedido.Status = novoStatus;
                _dataService.SaveData(lista);
                return true;
            }
            return false;
        }

        public (bool Sucesso, string Mensagem) Salvar(Pedido pedido)
        {
            // Barreira de segurança na Service
            if (pedido.Cliente == null) return (false, "Selecione um cliente.");
            if (pedido.Itens == null || !pedido.Itens.Any()) return (false, "O pedido deve ter pelo menos um item.");
            if (string.IsNullOrWhiteSpace(pedido.FormaPagamento)) return (false, "Selecione uma forma de pagamento.");

            var lista = ObterTodos();

            // Regras de Negócio: O sistema preenche o que o usuário não dita
            pedido.Id = lista.Any() ? lista.Max(p => p.Id) + 1 : 1;
            pedido.DataDaVenda = DateTime.Now;
            pedido.Status = "Pendente";

            lista.Add(pedido);
            _dataService.SaveData(lista);

            return (true, "Venda registrada com sucesso!");
        }
    }
}