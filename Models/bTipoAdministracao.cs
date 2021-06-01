using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApiFipe.Models
{
    public class bTipoAdministracao
    {
        public FIPEContratosContext db { get; set; }

        public bTipoAdministracao(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddTipoAdministracao(TipoAdministracao item)
        {
            try
            {
                db.TipoAdministracao.Add(item);
                db.SaveChanges();
                var idCli = item.IdTipoAdministracao;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

        public List<TipoAdministracao> Get()
        {
            var tipoAdministracao = db.TipoAdministracao.ToList();

            return tipoAdministracao;
        }

        public TipoAdministracao GetById(int id)
        {
            var tipoAdministracao = db.TipoAdministracao.Where(w => w.IdTipoAdministracao == id).FirstOrDefault();

            return tipoAdministracao;
        }

        public bool UpdateTipoAdministracao(TipoAdministracao item)
        {

            try
            {
                db.TipoAdministracao.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveTipoAdministracao(int id)
        {
            try
            {
                var tipoAdministracao = db.TipoAdministracao.Where(w=>w.IdTipoAdministracao == id).FirstOrDefault();

                db.TipoAdministracao.Remove(tipoAdministracao);
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