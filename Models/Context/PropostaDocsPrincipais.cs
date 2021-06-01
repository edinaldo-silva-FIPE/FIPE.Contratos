using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaDocsPrincipais
    {
        public int IdPropostaDocsPrincipais { get; set; }
        public int IdProposta { get; set; }
        public short IdTipoDoc { get; set; }
        public string DsDoc { get; set; }
        public string NmDocumento { get; set; }
        public DateTime DtUpLoad { get; set; }
        public string NmCriador { get; set; }
        public Guid DocFisicoId { get; set; }
        public byte[] DocFisico { get; set; }
        public bool? ParaAjustes { get; set; }
    }
}
