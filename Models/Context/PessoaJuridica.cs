using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PessoaJuridica
    {
        public PessoaJuridica()
        {
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
            Pessoa = new HashSet<Pessoa>();
        }

        public int IdPessoaJuridica { get; set; }
        public string NmFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public int? IdCidade { get; set; }
        public string Cnpj { get; set; }
        public string Uf { get; set; }
        public string Endereco { get; set; }
        public string NmBairro { get; set; }
        public string Cep { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }
        public int? IdEsferaEmpresa { get; set; }
        public int? IdTipoAdministracao { get; set; }
        public int? IdEntidade { get; set; }
        public string DsInternacional { get; set; }
        public int? IdPais { get; set; }
        public string NuEndereco { get; set; }
        public string Complemento { get; set; }
        public DateTime? DtCriacao { get; set; }
        public int? IdUsuarioCriacao { get; set; }

        public virtual Cidade IdCidadeNavigation { get; set; }
        public virtual ClassificacaoEmpresa IdClassificacaoEmpresaNavigation { get; set; }
        public virtual Entidade IdEntidadeNavigation { get; set; }
        public virtual EsferaEmpresa IdEsferaEmpresaNavigation { get; set; }
        public virtual Pais IdPaisNavigation { get; set; }
        public virtual TipoAdministracao IdTipoAdministracaoNavigation { get; set; }
        public virtual Usuario IdUsuarioCriacaoNavigation { get; set; }
        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
        public virtual ICollection<Pessoa> Pessoa { get; set; }
    }
}
