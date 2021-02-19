using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.DTOs
{
    public class ParcelaHistoricoDTO
    {
        public long Id { get; set; }
        public int IdAditivo { get; set; }
        public ClienteDTO Cliente { get; set; }
        public string NumerosEntregaveis { get; set; }
        public List<EntregavelHistoricoDTO> Entregaveis { get; set; }
        public decimal Valor { get; set; }
        public DateTime? DtFaturamento { get; set; }
        public string CdISS { get; set; }
        public string CdParcela { get; set; }
        public short NuParcela { get; set; }
        public SituacaoDTO Situacao { get; set; }
        public string NuNotaFiscal { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public string DsObservacao { get; set; }
    }
}
