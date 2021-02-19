using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.Models
{
    public class bFrenteCoordenador
    {
        public FIPEContratosContext db { get; set; }

        public bFrenteCoordenador(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutPutAddFrenteCoordeandor AddFrenteCoordenador(InputAddFrenteCoordenador item)
        {
            var retorno = new OutPutAddFrenteCoordeandor();
            var frenteCoord = new FrenteCoordenador();

            frenteCoord.IdFrente = item.IdFrente;
            frenteCoord.IdPessoaFisica = item.IdPessoa;

            var frenteCoordExiste = db.FrenteCoordenador.Where(w => w.IdFrente == frenteCoord.IdFrente && w.IdPessoaFisica == frenteCoord.IdPessoaFisica)
                .FirstOrDefault();

            if (frenteCoordExiste == null)
            {
                db.FrenteCoordenador.Add(frenteCoord);
                db.SaveChanges();
            }

            retorno.Result = true;

            return retorno;
        }

        public List<FrenteCoordenador> BuscaFrenteCoordenadorIdFrente(int idFrente)
        {
            var frenteCoordenadores = db.FrenteCoordenador.Where(w => w.IdFrente == idFrente).ToList();

            return frenteCoordenadores;
        }

        public bool RemoveFrenteCoordenadorIdFrente(int idFrente)
        {
            var retorno = false;

            var lstFrenteCoordenador = db.FrenteCoordenador.Where(w => w.IdFrente == idFrente).ToList();

            foreach (var frenteCoord in lstFrenteCoordenador)
            {
                db.FrenteCoordenador.Remove(frenteCoord);
            }
            db.SaveChanges();

            retorno = true;

            return retorno;
        }

        public bool RemoveContratoEntregavelIdFrente(int idFrente)
        {
            var retorno = false;

            var lstContratoEntregavel = db.ContratoEntregavel.Where(w => w.IdFrente == idFrente).ToList();

            foreach (var contratoEntregavel in lstContratoEntregavel)
            {
                db.ContratoEntregavel.Remove(contratoEntregavel);
            }
            db.SaveChanges();

            retorno = true;

            return retorno;
        }

        public bool RemoveFrenteCoordenadoresExcluidos(List<InputAddFrentePessoaFisica> coordenadores, int idFrente)
        {
            var retorno = false;

            var listFrenteCoordenadores = db.FrenteCoordenador.Where(w => w.IdFrente == idFrente).ToList();
            foreach (var frenteCoord in listFrenteCoordenadores)
            {
                var coordenador = coordenadores.Where(w => w.IdPessoa == frenteCoord.IdPessoaFisica).FirstOrDefault();

                if (coordenador == null)
                {
                    db.FrenteCoordenador.Remove(frenteCoord);
                }
            }
            db.SaveChanges();

            retorno = true;

            return retorno;
        }

        #region Retornos
        public class InputAddFrenteCoordenador
        {
            public int IdFrente { get; set; }
            public int IdPessoa { get; set; }
        }

        public class OutPutAddFrenteCoordeandor
        {
            public bool Result { get; set; }
        }
        #endregion

    }
}
