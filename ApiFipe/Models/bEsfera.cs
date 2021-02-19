using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bEsfera
    {
        public FIPEContratosContext db { get; set; }

        public bEsfera(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddEsfera(EsferaEmpresa item)
        {
            try
            {
                db.EsferaEmpresa.Add(item);
                db.SaveChanges();
                var idCli = item.IdEsferaEmpresa;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<EsferaEmpresa> Get()
        {
            var esferaEmpresa = db.EsferaEmpresa.ToList();

            return esferaEmpresa;
        }

        public List<EsferaEmpresa> GetEsferaEmpresa()
        {
            var esferaEmpresa = db.EsferaEmpresa.ToList();

            return esferaEmpresa;
        }

        public EsferaEmpresa GetById(int id)
        {
            var esferaEmpresa = db.EsferaEmpresa.Where(w => w.IdEsferaEmpresa == id).FirstOrDefault();

            return esferaEmpresa;
        }

        public bool UpdateEsferaEmpresa(EsferaEmpresa item)
        {
            try
            {
                db.EsferaEmpresa.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveEsferaEmpresaId(int id)
        {
            try
            {
                var esferaEmpresa = db.EsferaEmpresa.Where(w=>w.IdEsferaEmpresa == id).FirstOrDefault();

                db.EsferaEmpresa.Remove(esferaEmpresa);
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