using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.SituacaoController;

namespace ApiFipe.Models
{
    public class bSituacao
    {
        public FIPEContratosContext db { get; set; }

        public bSituacao(FIPEContratosContext db)
        {
            this.db = db;
        }

        public bool AddSituacao(Situacao item)
        {
            try
            {
                db.Situacao.Add(item);
                db.SaveChanges();
                var idCli = item.IdSituacao;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Situacao> BuscaSituacoesContrato()
        {
            var situacoes = db.Situacao.Where(w => w.IcEntidade == "C").ToList();
            return situacoes;
        }

        public List<Situacao> Get()
        {
            var situacao = db.Situacao.ToList();

            return situacao;
        }

        public Situacao GetById(int id)
        {
            var situacao = db.Situacao.Where(w => w.IdSituacao == id).FirstOrDefault();

            return situacao;
        }

        public bool UpdateSituacao(Situacao item)
        {

            try
            {
                db.Situacao.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveSituacaoId(int id)
        {
            try
            {
                var situacao = db.Situacao.Where(w => w.IdSituacao == id).FirstOrDefault();

                db.Situacao.Remove(situacao);
                db.SaveChanges();

                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

        public List<Situacao> BuscaSituacoTomadorServico()
        {
            var situacoes = db.Situacao.Where(w => w.IcEntidade == "F").ToList();
            return situacoes;
        }

        public List<SituacaoDTO> ObterSituacoes(string entidade_)
        {
            return db.Situacao.Where(_ => _.IcEntidade.ToUpper() == entidade_.ToUpper())?.Select(_ => new SituacaoDTO()
            {
                Id = _.IdSituacao,
                Nome = _.DsSituacao,
                IcEntregue = _.IcEntregue.GetValueOrDefault(),
                IcNFEmitida = _.IcNfemitida.GetValueOrDefault()
            }).ToList();
        }

        public SituacaoDTO ConsultarSituacao(int idSituacao_)
        {
            return db.Situacao.Where(_ => _.IdSituacao == idSituacao_)?.Select(_ => new SituacaoDTO()
            {
                Id = _.IdSituacao,
                Nome = _.DsSituacao,
                IcEntregue = _.IcEntregue.GetValueOrDefault(),
                IcNFEmitida = _.IcNfemitida.GetValueOrDefault()
            }).FirstOrDefault();
        }


       /* ===========================================================================================
        *  Edinaldo FIPE
        *  Setembro/2020 
        *  Pesquisa Situacao pelo filtro, pela descriçao, e não pelo ID
        ===========================================================================================*/
        public int _PesquisaSituacoes(string pDsSituacao, string pDsSubArea)
        {
            var bRetorno = db.Situacao.Where(w => w.DsSituacao == pDsSituacao && w.DsSubArea == pDsSubArea).FirstOrDefault();
            if (bRetorno == null)
            {
                return 1;  //Nao encontrado
            }
            return bRetorno.IdSituacao;
        }

    }
}