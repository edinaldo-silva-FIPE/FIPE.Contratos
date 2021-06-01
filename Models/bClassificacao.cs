using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.SituacaoController;

namespace ApiFipe.Models
{
    public class bClassificacao
    {
        public FIPEContratosContext db { get; set; }

        public bClassificacao(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddClassificacao(ClassificacaoEmpresa item)
        {
            try
            {
                db.ClassificacaoEmpresa.Add(item);
                db.SaveChanges();
                var idCli = item.IdClassificacaoEmpresa;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }      

        public List<ClassificacaoEmpresa> Get()
        {
            var classificacao = db.ClassificacaoEmpresa.ToList();

            return classificacao;
        }

        public ClassificacaoEmpresa GetById(int id)
        {
            var classificacao = db.ClassificacaoEmpresa.Where(w => w.IdClassificacaoEmpresa == id).FirstOrDefault();

            return classificacao;
        }

        public bool UpdateClassificacao(ClassificacaoEmpresa item)
        {

            try
            {
                db.ClassificacaoEmpresa.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveClassificacaoId(int id)
        {
            try
            {
                var classificacao = db.ClassificacaoEmpresa.Where(w=>w.IdClassificacaoEmpresa == id).FirstOrDefault();

                db.ClassificacaoEmpresa.Remove(classificacao);
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