using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bPropostaDocs
    {
        public FIPEContratosContext db { get; set; }

        public bPropostaDocs(FIPEContratosContext db)
        {
            this.db = db;
        }

        public PropostaDocs GetById(int id)
        {
            var propostaDoc = db.PropostaDocs
                .Where(w => w.IdPropostaDocs == id)
                .Single();

            return propostaDoc;
        }

        public PropostaDocs GetPropostaMinutaByProposta(int idProposta)
        {
            var propostaDoc = db.PropostaDocs
                         .Where(p => p.IdProposta == idProposta && p.IdTipoDoc == 3)
                         .FirstOrDefault();

            return propostaDoc;
        }

        public PropostaDocsPrincipais GetPropostaFinalByProposta(int idProposta)
        {
            var propostaDoc = db.PropostaDocsPrincipais
                .Where(p => p.IdProposta == idProposta && p.IdTipoDoc == 4)
                .FirstOrDefault();

            return propostaDoc;
        }

        public PropostaDocsPrincipais BuscarTermoId(int id)
        {

            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 1).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscarFinalId(int id)
        {

            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 4).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscarMinutaId(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 3).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaPropostaAditivoId(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 27).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaContratoAssinado(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 7).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaOrdemInicio(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 16).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaPropostaAditivo(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 27).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaPropostaAditivoFinal(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 28).FirstOrDefault();

            return item;
        }

        public PropostaDocsPrincipais BuscaDocumentoAditivo(int id)
        {
            var item = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id && w.IdTipoDoc == 23).FirstOrDefault();

            return item;
        }
    }
}