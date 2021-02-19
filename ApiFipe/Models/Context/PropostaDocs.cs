﻿using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaDocs
    {
        public int IdPropostaDocs { get; set; }
        public int IdProposta { get; set; }
        public short IdTipoDoc { get; set; }
        public string DsDoc { get; set; }
        public string NmDocumento { get; set; }
        public DateTime DtUpLoad { get; set; }
        public string NmCriador { get; set; }
        public Guid DocFisicoId { get; set; }
        public byte[] DocFisico { get; set; }

        public virtual Proposta IdPropostaNavigation { get; set; }
        public virtual TipoDocumento IdTipoDocNavigation { get; set; }
    }
}
