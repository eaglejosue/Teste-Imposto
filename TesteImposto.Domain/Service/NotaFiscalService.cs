using System;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using TesteImposto.Domain.Data;
using TesteImposto.Domain.Entitie;
using TesteImposto.Domain.Model;

namespace TesteImposto.Domain.Service
{
    public class NotaFiscalService
    {
        private readonly NotaFiscalRepository _notaFiscalRepository;

        public NotaFiscalService()
        {
            _notaFiscalRepository = new NotaFiscalRepository();
        }

        public bool GerarNotaFiscal(Pedido pedido)
        {
            try
            {
                // Exercício 1
                var notaFiscal = new NotaFiscal();
                notaFiscal.EmitirNotaFiscal(pedido);

                var urlArquivoXml = $"{ConfigurationManager.AppSettings["UrlArquivo"]}/{notaFiscal.Serie}_{DateTime.Now:ddMMyyyyHHmmss}.xml";
                GerarXmlNotaFiscal(notaFiscal, urlArquivoXml);

                if (!File.Exists(urlArquivoXml)) return false;

                // Exercício 2
                _notaFiscalRepository.AdicionarNotaFiscal(notaFiscal);

                return true;
            }
            catch (Exception ex)
            {
                //Registrar log
                var erro = ex.Message;
                return false;
            }
            finally
            {
                if (_notaFiscalRepository != null) _notaFiscalRepository.Dispose();
            }
        }

        private static void GerarXmlNotaFiscal(NotaFiscal notaFiscal, string urlArquivoXml)
        {
            var xml = new XmlSerializer(typeof(NotaFiscal));
            xml.Serialize(new FileStream(urlArquivoXml, FileMode.OpenOrCreate, FileAccess.Write), notaFiscal);
        }
    }
}
