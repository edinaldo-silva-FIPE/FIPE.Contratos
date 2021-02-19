using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApiFipe.Models
{
    public class bTipoDocumento
    {
        public FIPEContratosContext db { get; set; }

        public bTipoDocumento(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddTipoDocumento(TipoDocumento item)
        {
            try
            {
                db.TipoDocumento.Add(item);
                db.SaveChanges();
                var idCli = item.IdTipoDoc;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

        public List<TipoDocumento> Get()
        {
            var tipoDocumento = db.TipoDocumento.OrderBy(o => o.IdEntidade).ToList();

            return tipoDocumento;
        }

        public TipoDocumento GetById(int id)
        {
            var tipoDocumento = db.TipoDocumento.Where(w => w.IdTipoDoc == id).FirstOrDefault();

            return tipoDocumento;
        }

        public bool UpdateTipoDocumento(TipoDocumento item)
        {

            try
            {
                db.TipoDocumento.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveTipoDocumento(int id)
        {
            try
            {
                var tipoDocumento = db.TipoDocumento.Where(w=>w.IdTipoDoc == id).FirstOrDefault();

                db.TipoDocumento.Remove(tipoDocumento);
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