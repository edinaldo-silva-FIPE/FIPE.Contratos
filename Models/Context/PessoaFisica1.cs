using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PessoaFisica1
    {
        public PessoaFisica1()
        {
            ContratoCoordenador = new HashSet<ContratoCoordenador>();
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
            HistoricoPessoaFisica = new HashSet<HistoricoPessoaFisica>();
            OportunidadeResponsavel = new HashSet<OportunidadeResponsavel>();
            PropostaCoordenador = new HashSet<PropostaCoordenador>();
            Usuario = new HashSet<Usuario>();
        }

        public int IdPessoaFisica1 { get; set; }
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
        public string SgUf { get; set; }
        public string CdLinkedIn { get; set; }
        public string CdCvLattes { get; set; }
        public int? IdCidade { get; set; }
        public byte[] DsCv { get; set; }
        public string NmCv { get; set; }

        public virtual Cidade IdCidadeNavigation { get; set; }
        public virtual ICollection<ContratoCoordenador> ContratoCoordenador { get; set; }
        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
        public virtual ICollection<HistoricoPessoaFisica> HistoricoPessoaFisica { get; set; }
        public virtual ICollection<OportunidadeResponsavel> OportunidadeResponsavel { get; set; }
        public virtual ICollection<PropostaCoordenador> PropostaCoordenador { get; set; }
        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}
