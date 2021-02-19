using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bPropostaCoordenador
    {
        public FIPEContratosContext db { get; set; }

        public bPropostaCoordenador(FIPEContratosContext db)
        {
            this.db = db;
        }

        public PropostaCoordenador GetById(int id)
        {
            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.IdPropostaCoordenador == id)
                .Single();

            return propostaCoordenador;
        }

        public PropostaCoordenador GetByGuid(string guid)
        {
            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.GuidPropostaCoordenador.ToString() == guid)
                .Single();

            return propostaCoordenador;
        }

        public PropostaCoordenador GetByPropostaPessoa(int idProposta, int idPessoa)
        {
            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.IdProposta == idProposta && w.IdPessoa == idPessoa)
                .FirstOrDefault();

            return propostaCoordenador;
        }

        public bool VerificaUltimoCoordenador(int idProposta)
        {
            var retorno = true;

            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.IdProposta == idProposta && w.IcPropostaAprovada == null && w.IcAprovado == true).ToList();

            if (propostaCoordenador.Count > 0)
            {
                retorno = false;
            }

            return retorno;
        }

        public bool VerificaSolicitacaoAjustes(int idProposta)
        {
            var retorno = false;

            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.IdProposta == idProposta && w.IcPropostaAprovada == false)
                .FirstOrDefault();

            if (propostaCoordenador != null)
            {
                retorno = true;
            }

            return retorno;
        }

        public List<PropostaCoordenador> GetByProposta(int idProposta)
        {
            var propostaCoordenador = db.PropostaCoordenador
                .Where(w => w.IdProposta == idProposta)
                .ToList();

            return propostaCoordenador;
        }
    }
}