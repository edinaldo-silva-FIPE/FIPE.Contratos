using ApiFipe.Controllers;
using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Models
{
    public class bFrente
    {
        public FIPEContratosContext db { get; set; }

        public bFrente(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutPutAddFrente AddFrente(InputAddFrente item)
        {
            var retorno = new OutPutAddFrente();

            var frente = new Frente();
            frente.NmFrente = item.NmFrente;
            frente.IdContrato = item.IdContrato;
            frente.CdFrente = Convert.ToInt32(item.CdFrenteTexto);
            frente.CdFrenteTexto = item.CdFrenteTexto;
            while (frente.CdFrenteTexto.Length < 2)
            {
                frente.CdFrenteTexto = "0" + item.CdFrenteTexto;
            }

            db.Frente.Add(frente);
            db.SaveChanges();

            foreach (var coord in item.Coordenadores)
            {
                var inputAddFrenteCoordenador = new InputAddFrenteCoordenador();
                inputAddFrenteCoordenador.IdFrente = frente.IdFrente;
                inputAddFrenteCoordenador.IdPessoa = coord.IdPessoa;

                new bFrenteCoordenador(db).AddFrenteCoordenador(inputAddFrenteCoordenador);
            }

            retorno.Result = true;

            return retorno;
        }

        public OutPutUpdateFrente UpdateFrente(InputUpdateFrente item)
        {
            var retorno = new OutPutUpdateFrente();

            var frente = new bFrente(db).BuscaFrenteId(item.IdFrente);
            frente.NmFrente = item.NmFrente;
            frente.CdFrente = Convert.ToInt32(item.CdFrenteTexto);
            frente.CdFrenteTexto = item.CdFrenteTexto;
            while (frente.CdFrenteTexto.Length < 2)
            {
                frente.CdFrenteTexto = "0" + item.CdFrenteTexto;
            }

            db.SaveChanges();

            foreach (var coord in item.Coordenadores)
            {
                var inputAddFrenteCoordenador = new InputAddFrenteCoordenador();
                inputAddFrenteCoordenador.IdFrente = frente.IdFrente;
                inputAddFrenteCoordenador.IdPessoa = coord.IdPessoa;

                new bFrenteCoordenador(db).AddFrenteCoordenador(inputAddFrenteCoordenador);
            }

            new bFrenteCoordenador(db).RemoveFrenteCoordenadoresExcluidos(item.Coordenadores, item.IdFrente);

            retorno.Result = true;

            return retorno;
        }

        public List<Frente> BuscaFrenteIdContrato(int idContrato)
        {
            var frentes = db.Frente.Where(w => w.IdContrato == idContrato).ToList();

            return frentes;
        }

        public Frente BuscaFrenteId(int id)
        {
            var frente = db.Frente.Where(w => w.IdFrente == id).FirstOrDefault();

            return frente;
        }

        public OutPutRemoveFrente RemoveFrenteId(int id)
        {
            var retorno = new OutPutRemoveFrente();

            retorno.Result = new bFrenteCoordenador(db).RemoveFrenteCoordenadorIdFrente(id);
            retorno.Result = new bFrenteCoordenador(db).RemoveContratoEntregavelIdFrente(id);

            if (retorno.Result)
            {
                var frente = BuscaFrenteId(id);

                db.Frente.Remove(frente);
                db.SaveChanges();
            }

            return retorno;
        }

        public FrenteDTO ConsultarFrente(int id_)
        {
            return db.Frente.Where(_ => _.IdFrente == id_)?.Select(_ => new FrenteDTO()
            {
                Id = _.IdFrente,
                Nome = _.NmFrente
            }).FirstOrDefault();
        }
    }
}
