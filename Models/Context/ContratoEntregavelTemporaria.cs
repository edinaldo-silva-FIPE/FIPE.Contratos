using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoEntregavelTemporaria
    {
        public ContratoEntregavelTemporaria()
        {
            ContratoParcelaEntregavelTemporaria = new HashSet<ContratoParcelaEntregavelTemporaria>();
        }

        public int IdContratoEntregavel { get; set; }
        public int IdContrato { get; set; }
        public int? IdContratoCliente { get; set; }
        public int IdSituacao { get; set; }
        public int IdFrente { get; set; }
        public bool? IcAtraso { get; set; }
        public string DsProduto { get; set; }
        public DateTime? DtProduto { get; set; }
        public int VlOrdem { get; set; }
        public int? IdEntregavel { get; set; }

        public virtual ContratoCliente IdContratoClienteNavigation { get; set; }
        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Frente IdFrenteNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual ICollection<ContratoParcelaEntregavelTemporaria> ContratoParcelaEntregavelTemporaria { get; set; }
    }
}
