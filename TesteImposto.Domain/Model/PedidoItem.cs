namespace TesteImposto.Domain.Model
{
    public class PedidoItem
    {
        public PedidoItem() { }

        public PedidoItem(string nomeProduto = "Produto", string codigoProduto = "789456123", decimal valorItemPedido = 0M, bool brinde = false)
        {
            NomeProduto = nomeProduto;
            CodigoProduto = codigoProduto;
            ValorItemPedido = valorItemPedido;
            Brinde = brinde;
        }

        public string NomeProduto { get; set; }
        public string CodigoProduto { get; set; }        
        public decimal ValorItemPedido { get; set; }
        public bool Brinde { get; set; }        
    }
}
