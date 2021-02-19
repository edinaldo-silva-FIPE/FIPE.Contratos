using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Oportunidade
    {
        public Oportunidade()
        {
            OportunidadeCliente = new HashSet<OportunidadeCliente>();
            OportunidadeDocs = new HashSet<OportunidadeDocs>();
            OportunidadeResponsavel = new HashSet<OportunidadeResponsavel>();
            Proposta = new HashSet<Proposta>();
        }

        public int IdOportunidade { get; set; }
        public int IdSituacao { get; set; }
        public int IdTipoOportunidade { get; set; }
        public string DsAssunto { get; set; }
        public string DsObservacao { get; set; }
        public DateTime? DtLimiteEntregaProposta { get; set; }
        public DateTime DtCriacao { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public DateTime? DtUltimaAlteracao { get; set; }
        public int? IdUsuarioUltimaAlteracao { get; set; }
        public int? IdProposta { get; set; }
        public int? Score { get; set; }
        public string DsCoordenacao { get; set; }

        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual TipoOportunidade IdTipoOportunidadeNavigation { get; set; }
        public virtual ICollection<OportunidadeCliente> OportunidadeCliente { get; set; }
        public virtual ICollection<OportunidadeDocs> OportunidadeDocs { get; set; }
        public virtual ICollection<OportunidadeResponsavel> OportunidadeResponsavel { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
