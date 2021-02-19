using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoContatos
    {
        public int IdContratoContato { get; set; }
        public int IdContrato { get; set; }
        public int? IdTipoContato { get; set; }
        public int? IdContratoCliente { get; set; }
        public string NmContato { get; set; }
        public string CdEmail { get; set; }
        public string NuTelefone { get; set; }
        public string NuCelular { get; set; }
        public string DsEndereco { get; set; }
        public string NmDepartamento { get; set; }
        public bool? IcApareceFichaResumo { get; set; }

        public virtual ContratoCliente IdContratoClienteNavigation { get; set; }
        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual TipoContato IdTipoContatoNavigation { get; set; }
    }
}
