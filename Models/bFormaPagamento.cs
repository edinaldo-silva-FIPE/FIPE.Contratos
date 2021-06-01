using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bFormaPagamento
    {
        public FIPEContratosContext db { get; set; }

        public bFormaPagamento(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<FormaPagamento> Get()
        {
            var formaPagamentos = db.FormaPagamento.ToList();

            return formaPagamentos;
        }       
    }
}