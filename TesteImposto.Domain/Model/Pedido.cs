using System.Collections.Generic;
using TesteImposto.Domain.Util;

namespace TesteImposto.Domain.Model
{
    public class Pedido
    {
        public Pedido()
        {
            ItensDoPedido = new List<PedidoItem>();
        }

        public Pedido(string nomeCliente = null, string estadoOrigem = null, string estadoDestino = null, List<PedidoItem> itensDoPedido = null)
        {
            NomeCliente = nomeCliente ?? string.Empty;
            EstadoOrigem = estadoOrigem ?? string.Empty;
            EstadoDestino = estadoDestino ?? string.Empty;
            ItensDoPedido = itensDoPedido ?? new List<PedidoItem>();
        }

        public string NomeCliente { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoDestino { get; set; }
        public List<PedidoItem> ItensDoPedido { get; set; }

        public bool ValidarPedido(out string mensagemValidacao)
        {
            mensagemValidacao = string.Empty;

            // Exercício 6

            if (string.IsNullOrEmpty(NomeCliente))
            {
                mensagemValidacao = "O nome do cliente é obrigatório.";
                return false;
            }

            if (string.IsNullOrEmpty(EstadoOrigem))
            {
                mensagemValidacao = "O estado origem é obrigatório.";
                return false;
            }
            else if (EstadoOrigem.Length > 2 || !EstadosDisponiveis.ListaDeEstadosDeOrigem.Contains(EstadoOrigem))
            {
                mensagemValidacao = "Selecione um estado de origem.";
                return false;
            }

            if (string.IsNullOrEmpty(EstadoDestino))
            {
                mensagemValidacao = "O estado destino é obrigatório.";
                return false;
            }
            else if (EstadoDestino.Length > 2 || !EstadosDisponiveis.ListaDeEstadosDeDestino.Contains(EstadoDestino))
            {
                mensagemValidacao = "Selecione um estado de destino.";
                return false;
            }

            foreach (var item in ItensDoPedido)
            {
                if (string.IsNullOrEmpty(item.NomeProduto))
                {
                    mensagemValidacao = "O nome do ítem é obrigatório.";
                    return false;
                }

                if (string.IsNullOrEmpty(item.CodigoProduto))
                {
                    mensagemValidacao = "O código do ítem é obrigatório.";
                    return false;
                }

                if (item.ValorItemPedido == decimal.Zero)
                {
                    mensagemValidacao = $"O campo valor do item {item.NomeProduto} deve conter um valor maior que zero.";
                    return false;
                }
            }

            return true;
        }
    }
}
