using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContratoEquipeTecnica
    {
        public FIPEContratosContext db { get; set; }

        public bContratoEquipeTecnica(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutPutAddContratoEquipeTecnica Add(InputAddContratoEquipeTecnica item)
        {
            var retorno = new OutPutAddContratoEquipeTecnica();

            var contratoEquipeTecnica = new ContratoEquipeTecnica();
            contratoEquipeTecnica.IdContrato = item.IdContrato;
            contratoEquipeTecnica.IdPessoaFisica = item.IdPessoaFisica;
            contratoEquipeTecnica.IdPessoaJuridica = item.IdPessoaJuridica;
            contratoEquipeTecnica.IdTaxaInstitucional = item.IdTaxaInstitucional;
            contratoEquipeTecnica.IdTipoContratacao = item.IdTipoContratacao;
            contratoEquipeTecnica.IdFormacaoProfissional = item.IdFormacaoProfissional;
            contratoEquipeTecnica.VlCustoProjeto = item.VlCustoProjeto;
            contratoEquipeTecnica.VlTotalAreceber = item.VlTotalAReceber;
            contratoEquipeTecnica.DsAtividadeDesempenhada = item.DsAtividadeDesempenhada;

            db.ContratoEquipeTecnica.Add(contratoEquipeTecnica);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public OutPutUpdateContratoEquipeTecnica Update(InputUpdateContratoEquipeTecnica item)
        {
            var retorno = new OutPutUpdateContratoEquipeTecnica();

            var contratoEquipeTecnica = new bContratoEquipeTecnica(db).BuscaContratoEquipeTecnicaId(item.IdContratoEquipeTecnica);
            contratoEquipeTecnica.IdContrato = item.IdContrato;
            contratoEquipeTecnica.IdPessoaFisica = item.IdPessoaFisica;
            contratoEquipeTecnica.IdPessoaJuridica = item.IdPessoaJuridica;
            contratoEquipeTecnica.IdTaxaInstitucional = item.IdTaxaInstitucional;
            contratoEquipeTecnica.IdTipoContratacao = item.IdTipoContratacao;
            contratoEquipeTecnica.IdFormacaoProfissional = item.IdFormacaoProfissional;
            contratoEquipeTecnica.VlCustoProjeto = item.VlCustoProjeto;
            contratoEquipeTecnica.VlTotalAreceber = item.VlTotalAReceber;
            contratoEquipeTecnica.DsAtividadeDesempenhada = item.DsAtividadeDesempenhada;

            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public List<ContratoEquipeTecnica> BuscaContratoEquipeTecnicaIdContrato(int idContrato)
        {
            var equipesTecnicas = db.ContratoEquipeTecnica.Where(w => w.IdContrato == idContrato).ToList();

            return equipesTecnicas;
        }

        public ContratoEquipeTecnica BuscaContratoEquipeTecnicaId(int id)
        {
            var contratoEquipeTecnica = db.ContratoEquipeTecnica.Where(w => w.IdContratoEquipeTecnica == id).FirstOrDefault();

            return contratoEquipeTecnica;
        }

        public OutPutRemoveContratoEquipeTecnica RemoveContratoEquipeTecnicaId(int id)
        {
            var retorno = new OutPutRemoveContratoEquipeTecnica();

            var contratoEquipeTecnica = BuscaContratoEquipeTecnicaId(id);

            db.ContratoEquipeTecnica.Remove(contratoEquipeTecnica);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }
    }
}
