using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaHistorico
    {
        public int       IdPropostaHistorico         { get; set; }
        public DateTime  DtInicio                    { get; set; }
        public DateTime? DtFim                       { get; set; }
        public int       IdSituacao                  { get; set; }
        public int       IdUsuario                   { get; set; }
        public int       IdProposta                  { get; set; }
        public string    EmailObserva                { get; set; }              //EGS 30.08.2020 Email ou Observacao da Proposta ou Contrato

        public virtual Proposta IdPropostaNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation   { get; set; }
    }
}
