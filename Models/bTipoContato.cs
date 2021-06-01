using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApiFipe.Models
{
    public class bTipoContato
    {
        public FIPEContratosContext db { get; set; }

        public bTipoContato(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddTipoContato(TipoContato item)
        {
            try
            {
                db.TipoContato.Add(item);
                db.SaveChanges();
                var idCli = item.IdTipoContato;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

        public List<TipoContato> Get()
        {
            var tipoContato = db.TipoContato.ToList();

            return tipoContato;
        }

        public TipoContato GetById(int id)
        {
            var tipoContato = db.TipoContato.Where(w => w.IdTipoContato == id).FirstOrDefault();

            return tipoContato;
        }

        public bool UpdateTipoContato(TipoContato item)
        {

            try
            {
                db.TipoContato.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveTipoContato(int id)
        {
            try
            {
                var tipoContato = db.TipoContato.Where(w=>w.IdTipoContato == id).FirstOrDefault();

                db.TipoContato.Remove(tipoContato);
                db.SaveChanges();

                return true;

            }
            catch (Exception)
            {

                return false;
            }        
        }
    }
}