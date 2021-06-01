using System;

namespace ApiFipe.DTOs
{
    public class AditivoDTO
    {
        public int Id { get; set; }
        public short Numero { get; set; }
        public short NumeroCliente { get; set; }
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public int IdProposta { get; set; }
        public short? TipoAditivo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime? DataExecucaoAditivo { get; set; }
        public DateTime? DataFimAditada { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal PercentualValorAditado { get; set; }
        public decimal ValorContratoAditado { get; set; }
        public string Resumo { get; set; }
        public string CriadoPor { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Situacao { get; set; }
        public bool? IcAditivoData { get; set; }
        public bool? IcAditivoEscopo { get; set; }
        public bool? IcAditivoValor { get; set; }
    }

    public class TipoAditivoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class ContratoDTO
    {
        public int Id { get; set; }
        public int IdProposta { get; set; }
        public decimal Valor { get; set; }
    }
}
