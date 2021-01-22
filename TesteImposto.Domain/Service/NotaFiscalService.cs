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

                // Verifica se diretório existe
                var urlDiretorioArquivosXml = ConfigurationManager.AppSettings["UrlArquivo"];
                if (!Directory.Exists(urlDiretorioArquivosXml)) Directory.CreateDirectory(urlDiretorioArquivosXml);

                var urlArquivoXml = $"{urlDiretorioArquivosXml}/{notaFiscal.Serie}_{DateTime.Now:ddMMyyyyHHmmss}.xml";
                GerarXmlNotaFiscal(notaFiscal, urlArquivoXml);

                // Verifica se arquivo existe
                if (!File.Exists(urlArquivoXml)) return false;

                // Exercício 2 - persiste dados no SQL
                _notaFiscalRepository.AdicionarNotaFiscal(notaFiscal);

                return true;
            }
            catch (Exception ex)
            {
                //Registrar log
                var erro = ex.Message;
                return false;
            }
        }

        private static void GerarXmlNotaFiscal(NotaFiscal notaFiscal, string urlArquivoXml)
        {
            var xml = new XmlSerializer(typeof(NotaFiscal));
            xml.Serialize(new FileStream(urlArquivoXml, FileMode.OpenOrCreate, FileAccess.Write), notaFiscal);
        }
    }
}
