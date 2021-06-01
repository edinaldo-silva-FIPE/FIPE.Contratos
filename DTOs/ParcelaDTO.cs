using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.DTOs
{
    public class ParcelaDTO
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public ClienteDTO Cliente { get; set; }
        public string NumerosEntregaveis { get; set; }
        public List<int> Entregaveis { get; set; }
        public decimal Valor { get; set; }
        public DateTime? DtFaturamento { get; set; }
        public string CdISS { get; set; }
        public string CdParcela { get; set; }
        public int NuParcela { get; set; }
        public string NuNotaFiscal { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public string DsObservacao { get; set; }
        public SituacaoDTO Situacao { get; set; }
        public FrenteDTO Frente { get; set; }
        public bool? IcAtraso { get; set; }
    }

    public class InputParcela
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public int IdCliente { get; set; }
        public int[] IdsEntregaveis { get; set; }
        public decimal Valor { get; set; }
        public DateTime? DtFaturamento { get; set; }
        public string CdISS { get; set; }
        public string CdParcela { get; set; }
        public int? NuParcela { get; set; }
        public int Situacao { get; set; }
        public int NumeroAditivo { get; set; }
        public string NuNotaFiscal { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public string DsObservacao { get; set; }
        public FrenteDTO Frente { get; set; }       
    }
}
