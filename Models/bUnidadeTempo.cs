using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bUnidadeTempo
    {
        public FIPEContratosContext db { get; set; }

        public bUnidadeTempo(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<UnidadeDeTempo> Get()
        {
            var unidadesTempo = db.UnidadeDeTempo.ToList();

            return unidadesTempo;
        }
    }
}