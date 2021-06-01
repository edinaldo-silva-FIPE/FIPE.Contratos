using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoEquipeTecnica
    {
        public int IdContratoEquipeTecnica { get; set; }
        public int IdContrato { get; set; }
        public int IdPessoaFisica { get; set; }
        public int? IdPessoaJuridica { get; set; }
        public int? IdFormacaoProfissional { get; set; }
        public int IdTaxaInstitucional { get; set; }
        public int? IdTipoContratacao { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public decimal? VlTotalAreceber { get; set; }
        public decimal? VlCustoProjeto { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual FormacaoProfissional IdFormacaoProfissionalNavigation { get; set; }
        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
        public virtual PessoaJuridica IdPessoaJuridicaNavigation { get; set; }
        public virtual TaxaInstitucional IdTaxaInstitucionalNavigation { get; set; }
        public virtual TipoContratacaoEquipeTecnica IdTipoContratacaoNavigation { get; set; }
    }
}
