using System;
using System.Collections.Generic;
using TesteImposto.Domain.Util;
using TesteImposto.Domain.Model;

namespace TesteImposto.Domain.Entitie
{
    public class NotaFiscal
    {
        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();
        }

        public int Id { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int Serie { get; set; }
        public string NomeCliente { get; set; }
        public string EstadoDestino { get; set; }
        public string EstadoOrigem { get; set; }

        public List<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public void EmitirNotaFiscal(Pedido pedido)
        {
            NumeroNotaFiscal = 99999;
            Serie = new Random().Next(int.MaxValue);
            NomeCliente = pedido.NomeCliente;
            EstadoDestino = pedido.EstadoDestino;
            EstadoOrigem = pedido.EstadoOrigem;

            foreach (var itemPedido in pedido.ItensDoPedido)
            {
                var notaItem = new NotaFiscalItem(itemPedido.NomeProduto, itemPedido.CodigoProduto);

                DefineCfopPorEstados(notaItem);
                DefineValoresDeImpostos(itemPedido, notaItem);
                DefineDescontoPorRegiao(notaItem);

                ItensDaNotaFiscal.Add(notaItem);
            }
        }

        private void DefineCfopPorEstados(NotaFiscalItem notaItem)
        {
            switch (EstadoOrigem)
            {
                case "SP":
                    {
                        switch (EstadoDestino)
                        {
                            case "RJ":
                                notaItem.Cfop = "6.000";
                                break;
                            case "PE":
                                notaItem.Cfop = "6.001";
                                break;
                            case "MG":
                                notaItem.Cfop = "6.002";
                                break;
                            case "PB":
                                notaItem.Cfop = "6.003";
                                break;
                            case "PR":
                                notaItem.Cfop = "6.004";
                                break;
                            case "PI":
                                notaItem.Cfop = "6.005";
                                break;
                            case "RO":
                                // Exercício 5
                                notaItem.Cfop = "6.006";
                                break;
                            case "TO":
                                notaItem.Cfop = "6.008";
                                break;
                            case "SE":
                                notaItem.Cfop = "6.009";
                                break;
                            case "PA":
                                notaItem.Cfop = "6.010";
                                break;
                        }

                        break;
                    }

                case "MG":
                    {
                        switch (EstadoDestino)
                        {
                            case "RJ":
                                notaItem.Cfop = "6.000";
                                break;
                            case "PE":
                                notaItem.Cfop = "6.001";
                                break;
                            case "MG":
                                notaItem.Cfop = "6.002";
                                break;
                            case "PB":
                                notaItem.Cfop = "6.003";
                                break;
                            case "PR":
                                notaItem.Cfop = "6.004";
                                break;
                            case "PI":
                                notaItem.Cfop = "6.005";
                                break;
                            case "RO":
                                notaItem.Cfop = "6.006";
                                break;
                            case "TO":
                                notaItem.Cfop = "6.008";
                                break;
                            case "SE":
                                notaItem.Cfop = "6.009";
                                break;
                            case "PA":
                                notaItem.Cfop = "6.010";
                                break;
                        }

                        break;
                    }
            }
        }

        private void DefineValoresDeImpostos(PedidoItem itemPedido, NotaFiscalItem notaItem)
        {
            if (EstadoDestino == EstadoOrigem)
            {
                notaItem.TipoIcms = "60";
                notaItem.AliquotaIcms = 0.18M;
            }
            else
            {
                notaItem.TipoIcms = "10";
                notaItem.AliquotaIcms = 0.17M;
            }

            if (notaItem.Cfop == "6.009") notaItem.BaseIcms = itemPedido.ValorItemPedido * 0.90M;
            else notaItem.BaseIcms = itemPedido.ValorItemPedido;

            notaItem.ValorIcms = notaItem.BaseIcms * notaItem.AliquotaIcms;

            if (itemPedido.Brinde)
            {
                notaItem.TipoIcms = "60";
                notaItem.AliquotaIcms = 0.18M;
                notaItem.ValorIcms = notaItem.BaseIcms * notaItem.AliquotaIcms;
            }

            // Exercício 3
            notaItem.BaseIpi = itemPedido.ValorItemPedido;

            if (itemPedido.Brinde) notaItem.AliquotaIpi = 0;
            else notaItem.AliquotaIpi = 0.1M;

            notaItem.ValorIpi = notaItem.BaseIpi * notaItem.AliquotaIpi;
        }

        private void DefineDescontoPorRegiao(NotaFiscalItem notaItem)
        {
            // Exercício 7
            if (EstadosDisponiveis.ListaDeEstadosDoSudeste.Contains(EstadoDestino)) notaItem.Desconto = 0.1M;
        }
    }
}
