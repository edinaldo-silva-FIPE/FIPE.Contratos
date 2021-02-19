using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApiFipe.Models
{
    public class bTipoParametro
    {
        public FIPEContratosContext db { get; set; }

        public bTipoParametro(FIPEContratosContext db)
        {
            this.db = db;
        }     

        public List<Parametro> Get()
        {
            var tipoParametro = db.Parametro.ToList();

            return tipoParametro;
        }

        public Parametro GetById(int id)
        {
            var tipoParametro = db.Parametro.Where(w => w.IdParametro == id).FirstOrDefault();

            return tipoParametro;
        }

        public bool UpdateTipoParametro(Parametro item)
        {

            try
            {
                db.Parametro.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}