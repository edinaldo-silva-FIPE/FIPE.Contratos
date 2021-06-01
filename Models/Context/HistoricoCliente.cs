using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class HistoricoCliente
    {
        public int IdHistCliente { get; set; }
        public int IdCliente { get; set; }
        public int IdUsuarioAlteracao { get; set; }
        public DateTime DtAlteracao { get; set; }
        public int IdCidade { get; set; }
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public short IdEsfera { get; set; }
        public string Uf { get; set; }
        public string Endereco { get; set; }
        public string NmBairro { get; set; }
        public string Cep { get; set; }
        public string NmFantasia { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }
        public int? IdEsferaEmpresa { get; set; }
        public int? IdTipoAdministracao { get; set; }
        public int? IdEntidade { get; set; }
        public byte[] DsCv { get; set; }
        public string NmCv { get; set; }
        public string DsInternacional { get; set; }
        public int? IdPais { get; set; }
        public string NuEndereco { get; set; }
        public string Complemento { get; set; }

        public virtual ClassificacaoEmpresa IdClassificacaoEmpresaNavigation { get; set; }
        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Entidade IdEntidadeNavigation { get; set; }
        public virtual EsferaEmpresa IdEsferaEmpresaNavigation { get; set; }
        public virtual TipoAdministracao IdTipoAdministracaoNavigation { get; set; }
        public virtual Usuario IdUsuarioAlteracaoNavigation { get; set; }
    }
}
