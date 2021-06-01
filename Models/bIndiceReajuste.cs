using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApiFipe.Models
{
    public class bIndiceReajuste
    {
        public FIPEContratosContext db { get; set; }

        public bIndiceReajuste(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddIndiceReajuste(IndiceReajuste item)
        {
            try
            {
                db.IndiceReajuste.Add(item);
                db.SaveChanges();
                var idCli = item.IdIndiceReajuste;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

        public List<IndiceReajuste> Get()
        {
            var indiceReajuste = db.IndiceReajuste.ToList();

            return indiceReajuste;
        }

        public IndiceReajuste GetById(int id)
        {
            var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == id).FirstOrDefault();

            return indiceReajuste;
        }

        public bool UpdateIndiceReajuste(IndiceReajuste item)
        {

            try
            {
                db.IndiceReajuste.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveIndiceReajuste(int id)
        {
            try
            {
                var indiceReajuste = db.IndiceReajuste.Where(w=>w.IdIndiceReajuste == id).FirstOrDefault();

                db.IndiceReajuste.Remove(indiceReajuste);
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