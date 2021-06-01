using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Models
{
    public class bTipoCoordenacao
    {
        public FIPEContratosContext db { get; set; }

        public bTipoCoordenacao(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddTipoCoordenacao(TipoCoordenacao item)
        {
            try
            {
                db.TipoCoordenacao.Add(item);
                db.SaveChanges();
                var idCli = item.IdTipoCoordenacao;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TipoCoordenacao> ListaTiposCoordenacoes()
        {
            var lstTiposCoordenacoes = db.TipoCoordenacao.ToList();

            return lstTiposCoordenacoes;
        }
        
        public TipoCoordenacao BuscaTipoCoordenacaoId(int id)
        {
            var tipoCoordenacao = db.TipoCoordenacao.Where(w => w.IdTipoCoordenacao == id).FirstOrDefault();

            return tipoCoordenacao;
        }

        public bool UpdateTipoCoordenacao(TipoCoordenacao item)
        {
            try
            {
                db.TipoCoordenacao.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveTipoCoordenacao(int id)
        {
            try
            {
                var tipoCoordenacao = db.TipoCoordenacao.Where(w => w.IdTipoCoordenacao == id).FirstOrDefault();

                db.TipoCoordenacao.Remove(tipoCoordenacao);
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
