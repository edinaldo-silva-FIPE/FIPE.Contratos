using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTipoComprovacaoValor
    {
        public FIPEContratosContext db { get; set; }

        public bTipoComprovacaoValor(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoComprovacaoValor> Get()
        {
            var tipoComprovacaoValor = db.TipoComprovacaoValor.ToList();

            return tipoComprovacaoValor;
        }

    }
}