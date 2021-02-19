using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Models
{
    public class bVinculoPessoaFisica
    {
        public FIPEContratosContext db { get; set; }

        public bVinculoPessoaFisica(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutPutAddVinculo AddVinculo(InputAddVinculo item)
        {
            var retorno = new OutPutAddVinculo();

            var vinculoPessoaFisica = new VinculoPessoaFisica();
            vinculoPessoaFisica.IdPessoaFisica = item.IdPessoaFisica;
            vinculoPessoaFisica.IdTipoVinculo = item.IdTipoVinculo;
            vinculoPessoaFisica.DtInicioVinculo = item.DtInicioVinculo;
            vinculoPessoaFisica.DtFimVinculo = item.DtFimVinculo;

            db.VinculoPessoaFisica.Add(vinculoPessoaFisica);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public OutPutUpdateVinculo UpdateVinculo(InputUpdateVinculo item)
        {
            var retorno = new OutPutUpdateVinculo();

            var vinculoPessoa = new bVinculoPessoaFisica(db).BuscaVinculoId(item.IdVinculoPessoaFisica);
            vinculoPessoa.IdVinculoPessoa = item.IdVinculoPessoaFisica;
            vinculoPessoa.IdTipoVinculo = item.IdTipoVinculo;
            vinculoPessoa.DtInicioVinculo = item.DtInicioVinculo;
            vinculoPessoa.DtFimVinculo = item.DtFimVinculo;

            db.SaveChanges();           

            retorno.Result = true;

            return retorno;
        }

        public List<VinculoPessoaFisica> BuscaVinculoPessoaFisicaId(int id)
        {
            var lstVinculoPessoaFisica = db.VinculoPessoaFisica.Where(w => w.IdPessoaFisica == id).ToList();

            return lstVinculoPessoaFisica;
        }

        public VinculoPessoaFisica BuscaVinculoId(int id)
        {
            var vinculoPessoa = db.VinculoPessoaFisica.Where(w => w.IdVinculoPessoa == id).FirstOrDefault();

            return vinculoPessoa;
        }

        public OutPutDeleteVinculo RemoveVinculoId(int id)
        {
            var retorno = new OutPutDeleteVinculo();
            var vinculoPessoa = db.VinculoPessoaFisica.Where(w => w.IdVinculoPessoa == id).FirstOrDefault();

            db.VinculoPessoaFisica.Remove(vinculoPessoa);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

    }
}
