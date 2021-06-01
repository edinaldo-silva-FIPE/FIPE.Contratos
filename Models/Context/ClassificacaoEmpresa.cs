using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ClassificacaoEmpresa
    {
        public ClassificacaoEmpresa()
        {
            EsferaEmpresa = new HashSet<EsferaEmpresa>();
            HistoricoPessoaJuridica = new HashSet<HistoricoPessoaJuridica>();
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdClassificacaoEmpresa { get; set; }
        public string DsClassificacaoEmpresa { get; set; }

        public virtual ICollection<EsferaEmpresa> EsferaEmpresa { get; set; }
        public virtual ICollection<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
