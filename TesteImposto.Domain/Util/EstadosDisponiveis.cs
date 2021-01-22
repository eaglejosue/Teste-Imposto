using System.Collections.Generic;

namespace TesteImposto.Domain.Util
{
    public static class EstadosDisponiveis
    {
        public static List<string> ListaDeEstadosDeOrigem = new List<string> { "Selecione", "SP", "MG" };
        public static List<string> ListaDeEstadosDeDestino = new List<string> { "Selecione", "RJ", "PE", "MG", "PB", "PR", "PI", "RO", "TO", "SE", "PA" };
        public static List<string> ListaDeEstadosDoSudeste = new List<string> { "RJ", "MG" };
    }
}
