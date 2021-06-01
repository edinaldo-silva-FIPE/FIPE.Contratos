using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoHistorico
    {
        public int       IdContratoHistorico         { get; set; }
        public int       IdContrato                  { get; set; }
        public DateTime  DtInicio                    { get; set; }
        public DateTime? DtFim                       { get; set; }
        public int       IdSituacao                  { get; set; }
        public int       IdUsuario                   { get; set; }
        public string    EmailObserva                { get; set; }              //EGS 30.08.2020 Email ou Observacao da Proposta ou Contrato

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation   { get; set; }
    }
}
