namespace TechVendas.Models
{
    public class ItemPedido   
    {
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal SubTotal => (Produto?.Valor ?? 0) * Quantidade;
    }
}
