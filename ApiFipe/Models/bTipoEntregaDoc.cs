using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTipoEntregaDoc
    {
        public FIPEContratosContext db { get; set; }

        public bTipoEntregaDoc(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoEntregaDocumento> Get()
        {
            var tipoEntregaDocs = db.TipoEntregaDocumento.ToList();

            return tipoEntregaDocs;
        }       
    }
}