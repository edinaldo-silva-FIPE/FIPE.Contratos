using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class HistoricoPessoaFisica
    {
        public int IdHstPessoaFisica { get; set; }
        public int IdPessoaFisica { get; set; }
        public DateTime DtAlteracao { get; set; }
        public int IdUsuarioAlteracao { get; set; }
        public string NmPessoa { get; set; }
        public string CdSexo { get; set; }
        public string CdEmail { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string NuCpf { get; set; }
        public string NuTelefoneFixo { get; set; }
        public string NuTelefoneComercial { get; set; }
        public string NuCelular { get; set; }
        public string NuCep { get; set; }
        public string DsEndereco { get; set; }
        public string NuEndereco { get; set; }
        public string DsComplemento { get; set; }
        public string NmBairro { get; set; }
        public int? IdCidade { get; set; }
        public string SgUf { get; set; }
        public string CdLinkedIn { get; set; }
        public string CdCvLattes { get; set; }
        public int? IdTipoVinculo { get; set; }

        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
        public virtual TipoVinculo IdTipoVinculoNavigation { get; set; }
    }
}
