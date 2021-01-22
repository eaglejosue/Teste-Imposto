using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TesteImposto.Domain.Entitie;

namespace TesteImposto.Domain.Data
{
    public class NotaFiscalRepository : IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly SqlCommand _sqlCommand;

        public NotaFiscalRepository()
        {
            _sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            _sqlConnection.Open();
            _sqlCommand = new SqlCommand { Connection = _sqlConnection };
        }

        public void AdicionarNotaFiscal(NotaFiscal nfe)
        {
            // Adiciona nota
            _sqlCommand.CommandText = "P_NOTA_FISCAL";
            _sqlCommand.CommandType = CommandType.StoredProcedure;

            _sqlCommand.Parameters.Clear();
            _sqlCommand.Parameters.Add(AddParameter("@pId", 0, DbType.Int32, ParameterDirection.Output));
            _sqlCommand.Parameters.Add(AddParameter("@pNumeroNotaFiscal", nfe.NumeroNotaFiscal, DbType.Int32));
            _sqlCommand.Parameters.Add(AddParameter("@pSerie", nfe.Serie, DbType.Int32));
            _sqlCommand.Parameters.Add(AddParameter("@pNomeCliente", nfe.NomeCliente, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pEstadoOrigem", nfe.EstadoOrigem, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pEstadoDestino", nfe.EstadoDestino, DbType.String));

            var idNotaFiscal = (int)_sqlCommand.ExecuteScalar();

            // Adiciona itens da nota
            foreach (var item in nfe.ItensDaNotaFiscal) AdicionarNotaFiscalItem(idNotaFiscal, item);
        }

        private void AdicionarNotaFiscalItem(int idNotaFiscal, NotaFiscalItem item)
        {
            _sqlCommand.CommandText = "P_NOTA_FISCAL_ITEM";
            _sqlCommand.CommandType = CommandType.StoredProcedure;

            _sqlCommand.Parameters.Clear();
            _sqlCommand.Parameters.Add(AddParameter("@pId", 0, DbType.Int32, ParameterDirection.Output));
            _sqlCommand.Parameters.Add(AddParameter("@pIdNotaFiscal", idNotaFiscal, DbType.Int32));
            _sqlCommand.Parameters.Add(AddParameter("@pCfop", item.Cfop, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pTipoIcms", item.TipoIcms, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pBaseIcms", item.BaseIcms, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pAliquotaIcms", item.AliquotaIcms, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pValorIcms", item.ValorIcms, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pNomeProduto", item.NomeProduto, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pCodigoProduto", item.CodigoProduto, DbType.String));
            _sqlCommand.Parameters.Add(AddParameter("@pBaseIpi", item.BaseIpi, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pAliquotaIpi", item.AliquotaIpi, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pValorIpi", item.ValorIpi, DbType.Double));
            _sqlCommand.Parameters.Add(AddParameter("@pDesconto", item.Desconto, DbType.Double));

            _sqlCommand.ExecuteNonQuery();
        }

        private SqlParameter AddParameter(string nomeParametro, object valor, DbType type, ParameterDirection direcao = ParameterDirection.Input) =>
            new SqlParameter(nomeParametro, valor) { DbType = type, Direction = direcao };

        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Close();
                _sqlConnection.Dispose();
            }
        }
    }
}