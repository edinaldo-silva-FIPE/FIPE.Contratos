using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bEntidade
    {
        public FIPEContratosContext db { get; set; }

        public bEntidade(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddEntidade(Entidade item)
        {
            try
            {
                db.Entidade.Add(item);
                db.SaveChanges();
                var idCli = item.IdEntidade;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Entidade> Get()
        {
            var entidade = db.Entidade.ToList();

            return entidade;
        }

        public List<TipoEntidade> GetTipoEntidade()
        {
            var tipoEntidade = db.TipoEntidade.ToList();

            return tipoEntidade;
        }

        public Entidade GetById(int id)
        {
            var entidade = db.Entidade.Where(w => w.IdEntidade == id).FirstOrDefault();

            return entidade;
        }

        public bool UpdateEntidade(Entidade item)
        {

            try
            {
                db.Entidade.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveEntidadeId(int id)
        {
            try
            {
                var entidade = db.Entidade.Where(w=>w.IdEntidade == id).FirstOrDefault();

                db.Entidade.Remove(entidade);
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