using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTipoDocAcompanhaNF
    {
        public FIPEContratosContext db { get; set; }

        public bTipoDocAcompanhaNF(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoDocsAcompanhaNf> Get()
        {
            var tipoDocAcompanhaNFs = db.TipoDocsAcompanhaNf.ToList();

            return tipoDocAcompanhaNFs;
        }

        public bool AddTipoDocsAcompanhaNf(TipoDocsAcompanhaNf item)
        {
            try
            {
                db.TipoDocsAcompanhaNf.Add(item);
                db.SaveChanges();
                var idCli = item.IdTipoDocsAcompanhaNf;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public TipoDocsAcompanhaNf GetById(int id)
        {
            var tipoDoc = db.TipoDocsAcompanhaNf.Where(w => w.IdTipoDocsAcompanhaNf == id).FirstOrDefault();

            return tipoDoc;
        }

        public bool UpdateTipoDocsAcompanhaNf(TipoDocsAcompanhaNf item)
        {

            try
            {
                db.TipoDocsAcompanhaNf.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveTipoDocsAcompanhaNf(int id)
        {
            try
            {
                var TipoDocsAcompanhaNf = db.TipoDocsAcompanhaNf.Where(w => w.IdTipoDocsAcompanhaNf == id).FirstOrDefault();

                db.TipoDocsAcompanhaNf.Remove(TipoDocsAcompanhaNf);
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