
using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContaCorrente
    {
        public FIPEContratosContext db { get; set; }

        public bContaCorrente(FIPEContratosContext db)
        {
            this.db = db;
        }     

        public List<ContaCorrente> ListaContasCorrente()
        {
            var lstContas = db.ContaCorrente.ToList();

            return lstContas;
        }     
    }
}
