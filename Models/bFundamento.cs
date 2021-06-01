using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.SituacaoController;

namespace ApiFipe.Models
{
    public class bFundamento
    {
        public FIPEContratosContext db { get; set; }

        public bFundamento(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddFundamento(FundamentoContratacao item)
        {
            try
            {
                db.FundamentoContratacao.Add(item);
                db.SaveChanges();
                var idCli = item.IdFundamento;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

        public List<FundamentoContratacao> Get()
        {
            var fundamento = db.FundamentoContratacao.ToList();

            return fundamento;
        }

        public FundamentoContratacao GetById(int id)
        {
            var fundamento = db.FundamentoContratacao.Where(w => w.IdFundamento == id).FirstOrDefault();

            return fundamento;
        }

        public bool UpdateFundamento(FundamentoContratacao item)
        {

            try
            {
                db.FundamentoContratacao.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveFundamentoId(int id)
        {
            try
            {
                var fundamento = db.FundamentoContratacao.Where(w=>w.IdFundamento == id).FirstOrDefault();

                db.FundamentoContratacao.Remove(fundamento);
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