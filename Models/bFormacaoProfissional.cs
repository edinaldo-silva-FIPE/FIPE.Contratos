using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ApiFipe.Models
{
    public class bFormacaoProfissional
    {
        public FIPEContratosContext db { get; set; }

        public bFormacaoProfissional(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddFormacaoProfissional(FormacaoProfissional item)
        {
            try
            {
                db.FormacaoProfissional.Add(item);
                db.SaveChanges();
                var idCli = item.IdFormacaoProfissional;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<FormacaoProfissional> ListaFormacaoProfissional()
        {
            var lstFormacaoProfissional = db.FormacaoProfissional.ToList();

            return lstFormacaoProfissional;
        }
        
        public FormacaoProfissional BuscaFormacaoProfissionalId(int id)
        {
            var formacaoProfissional = db.FormacaoProfissional.Where(w => w.IdFormacaoProfissional == id).FirstOrDefault();

            return formacaoProfissional;
        }

        public bool UpdateFormacaoProfissional(FormacaoProfissional item)
        {
            try
            {
                db.FormacaoProfissional.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveFormacaoProfissional(int id)
        {
            try
            {
                var formacaoProfissional = db.FormacaoProfissional.Where(w => w.IdFormacaoProfissional == id).FirstOrDefault();

                db.FormacaoProfissional.Remove(formacaoProfissional);
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
