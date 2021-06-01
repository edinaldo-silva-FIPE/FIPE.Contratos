using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Esfera
    {
        public Esfera()
        {
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public short IdEsfera { get; set; }
        public string DsEsfera { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }
        public int? IdEsferaEmpresa { get; set; }
        public int? IdTipoAdm { get; set; }
        public int? IdEntidade { get; set; }

        public virtual ClassificacaoEmpresa IdClassificacaoEmpresaNavigation { get; set; }
        public virtual Entidade IdEntidadeNavigation { get; set; }
        public virtual EsferaEmpresa IdEsferaEmpresaNavigation { get; set; }
        public virtual TipoAdministracao IdTipoAdmNavigation { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
