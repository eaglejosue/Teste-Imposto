using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TesteImposto.Domain.Entitie;
using TesteImposto.Domain.Model;
using TesteImposto.Domain.Util;

namespace TesteImposto.Test
{
    [TestClass]
    public class UnitTest
    {
        // Red, Green, Refactor

        [TestMethod]
        public void PedidoInvalido()
        {
            var pedido = new Pedido();
            Assert.IsTrue(!pedido.ValidarPedido(out var mensagemValidacao));
            Assert.IsTrue(!string.IsNullOrEmpty(mensagemValidacao));
        }

        [TestMethod]
        public void PedidoValido()
        {
            var pedido = RetornaPedidoValido();
            Assert.IsTrue(pedido.ValidarPedido(out var mensagemValidacao));
            Assert.IsTrue(string.IsNullOrEmpty(mensagemValidacao));
        }

        [TestMethod]
        public void NotaFiscalInvalida()
        {
            var itensDoPedido = new List<PedidoItem> { new PedidoItem("Produto A", "789123465", 10.50M, true) };
            var pedido = new Pedido("Josué Monteiro", "SP", "ES", itensDoPedido);
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);

            Assert.IsFalse(EstadosDisponiveis.ListaDeEstadosDeDestino.Contains(notaFiscal.EstadoDestino));
            Assert.AreNotEqual("Produto B", notaFiscal.ItensDaNotaFiscal.FirstOrDefault().NomeProduto);
            Assert.AreNotEqual("789465123", notaFiscal.ItensDaNotaFiscal.FirstOrDefault().CodigoProduto);
            Assert.AreNotEqual("5.650", notaFiscal.ItensDaNotaFiscal.FirstOrDefault().Cfop);
            Assert.AreNotEqual(0.1M, notaFiscal.ItensDaNotaFiscal.FirstOrDefault().Desconto);
        }

        [TestMethod]
        public void NotaFiscalValida()
        {
            var pedido = RetornaPedidoValido();
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.IsTrue(EstadosDisponiveis.ListaDeEstadosDeDestino.Contains(notaFiscal.EstadoDestino));
            Assert.AreEqual("Produto A", primeiroItemDaNota.NomeProduto);
            Assert.AreEqual("789123465", primeiroItemDaNota.CodigoProduto);
            Assert.AreEqual("6.000", primeiroItemDaNota.Cfop);
            Assert.AreEqual(0.1M, primeiroItemDaNota.Desconto);
        }

        [TestMethod]
        public void CfopValido()
        {
            var pedido = RetornaPedidoValido();
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.IsTrue(!string.IsNullOrWhiteSpace(primeiroItemDaNota.Cfop));
            Assert.AreEqual("6.000", primeiroItemDaNota.Cfop);
        }

        [TestMethod]
        public void DescontoValido()
        {
            var pedido = RetornaPedidoValido();
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.IsTrue(primeiroItemDaNota.Desconto > decimal.Zero);
            Assert.AreEqual(0.1M, primeiroItemDaNota.Desconto);
        }

        [TestMethod]
        public void TipoIcmsParaEstadosIguaisInvalido()
        {
            var pedido = RetornaPedidoValido();
            pedido.EstadoDestino = pedido.EstadoOrigem;

            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.AreNotEqual("10", primeiroItemDaNota.TipoIcms);
        }

        [TestMethod]
        public void TipoIcmsParaEstadosIguaisValido()
        {
            var pedido = RetornaPedidoValido();
            pedido.EstadoDestino = pedido.EstadoOrigem;

            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.AreEqual("60", primeiroItemDaNota.TipoIcms);
        }

        [TestMethod]
        public void AliquotaIcmsInvalida()
        {
            var pedido = RetornaPedidoValido();
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.AreNotEqual(0.18M, primeiroItemDaNota.AliquotaIcms);
        }

        [TestMethod]
        public void AliquotaIcmsValida()
        {
            var pedido = RetornaPedidoValido();
            pedido.EstadoDestino = pedido.EstadoOrigem;

            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.AreEqual(0.18M, primeiroItemDaNota.AliquotaIcms);
        }

        [TestMethod]
        public void AliquotaIcmsParaBrindeValido()
        {
            var pedido = RetornaPedidoValido(true);
            var notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);
            
            var primeiroItemDaNota = notaFiscal.ItensDaNotaFiscal.FirstOrDefault();

            Assert.AreEqual(0.18M, primeiroItemDaNota.AliquotaIcms);
        }

        [TestMethod]
        public void BrindeValido()
        {
            var pedido = RetornaPedidoValido(true);
            Assert.IsTrue(pedido.ItensDoPedido.FirstOrDefault().Brinde);
        }

        #region Métodos privados

        private static Pedido RetornaPedidoValido(bool primeiroItemEhBrinde = false)
        {
            var itensDoPedido = new List<PedidoItem>
            {
                new PedidoItem("Produto A", "789123465", 10.50M, primeiroItemEhBrinde),
                new PedidoItem("Produto B", "789465123", 5M, true),
                new PedidoItem("Produto C", "789412563", 2.25M, true)
            };
            var pedido = new Pedido("Josué Monteiro", "SP", "RJ", itensDoPedido);
            return pedido;
        }

        #endregion
    }
}
