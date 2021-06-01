using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTipoAditivo
    {
        public FIPEContratosContext db { get; set; }

        public bTipoAditivo(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoAditivo> Get()
        {
            var aditivos = db.TipoAditivo.ToList();

            return aditivos;
        }       
    }
}