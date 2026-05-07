using TechVendas.Models;

public class ItemPedido
{
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public decimal Subtotal => (Produto?.Valor ?? 0) * Quantidade;
}