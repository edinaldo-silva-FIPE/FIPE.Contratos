using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class EsferaEmpresa
    {
        public EsferaEmpresa()
        {
            HistoricoPessoaJuridica = new HashSet<HistoricoPessoaJuridica>();
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdEsferaEmpresa { get; set; }
        public string DsEsferaEmpresa { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }

        public virtual ClassificacaoEmpresa IdClassificacaoEmpresaNavigation { get; set; }
        public virtual ICollection<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
