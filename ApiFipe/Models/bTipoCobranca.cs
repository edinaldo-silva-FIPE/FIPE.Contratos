using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTipoCobranca
    {
        public FIPEContratosContext db { get; set; }

        public bTipoCobranca(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoCobranca> Get()
        {
            var cobrancas = db.TipoCobranca.ToList();

            return cobrancas;
        }       
    }
}