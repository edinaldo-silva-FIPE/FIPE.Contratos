using ApiFipe.DTOs;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Controllers
{

    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class SituacaoController : ControllerBase
    {
        #region Métodos

        [HttpGet]
        [Route("Get")]
        public List<OutputGet> Get()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var situacoes = new bSituacao(db).Get().Select(s => new OutputGet()
                    {
                        IdSituacao = s.IdSituacao,
                        DsSituacao = s.DsSituacao,
                        DsArea = s.DsArea,
                        IcEntidade = s.IcEntidade,
                        IcEntregue = s.IcEntregue == true ? "Sim" : "Não",
                        IcFormatacao = s.IcFormatacao == true ? "Sim" : "Não",
                        IcNFEmitida = s.IcNfemitida == true ? "Sim" : "Não",
                        NuOrdem = s.NuOrdem

                    }).OrderBy(o => o.IcEntidade).ThenBy(t=>t.NuOrdem).ToList();

                    return situacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-Get");



                    throw;
                }

            }
        }

        [HttpGet]
        [Route("ListaSituacoes")]
        public List<OutPutGetSituacoes> ListaSituacoes()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var situacoes = new bSituacao(db).Get().Where(w => w.IcEntidade == "A").Select(s => new OutPutGetSituacoes()
                    {
                        IdSituacao = s.IdSituacao,
                        DsSituacao = s.DsSituacao,
                        Value = s.DsSituacao
                    }).OrderBy(o => o.DsSituacao).ToList();

                    return situacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-ListaSituacoes");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutputGetId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var situacao = new OutputGetId();
                    var sit = new bSituacao(db).GetById(id);
                    situacao.IdSituacao = sit.IdSituacao;
                    situacao.DsSituacao = sit.DsSituacao;
                    situacao.DsArea = sit.DsArea;
                    situacao.IcEntidade = sit.IcEntidade;
                    situacao.IcEntregue = sit.IcEntregue;
                    situacao.IcFormatacao = sit.IcFormatacao;
                    situacao.IcNFEmitida = sit.IcNfemitida;
                    situacao.NuOrdem = sit.NuOrdem;

                    return situacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-GetById");



                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddSituacao")]
        public OutPutAddSituacao Add([FromBody] InputAddSituacao item)
        {
            var retorno = new OutPutAddSituacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var situacao = new Situacao();
                            situacao.DsSituacao = item.DsSituacao;
                            situacao.IcEntidade = item.IcEntidade;
                            situacao.DsArea = item.DsArea;
                            situacao.IcEntregue = item.IcEntregue;
                            situacao.IcFormatacao = item.IcFormatacao;
                            situacao.IcNfemitida = item.IcNFEmitida;
                            situacao.NuOrdem = item.NuOrdem;
                            var addRetorno = new bSituacao(db).AddSituacao(situacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
            }
            return retorno;
        }

        [HttpPut]
        [Route("UpdateSituacao")]
        public OutPutUpDateSituacao Update([FromBody] InputUpDateSituacao item)
        {
            var retorno = new OutPutUpDateSituacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var sit = new Situacao();
                            sit.IdSituacao = item.IdSituacao;
                            sit.DsArea = item.DsArea;
                            sit.IcEntidade = item.IcEntidade;
                            sit.DsSituacao = item.DsSituacao;
                            sit.IcEntregue = item.IcEntregue;
                            sit.IcFormatacao = item.IcFormatacao;
                            sit.IcNfemitida = item.IcNFEmitida;
                            sit.NuOrdem = item.NuOrdem;

                            var updateRetorno = new bSituacao(db).UpdateSituacao(sit);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-Update");
                            
                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveSituacao/{id}")]
        public OutPutRemoveSituacao Remove(int id)
        {
            var retorno = new OutPutRemoveSituacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bSituacao(db).RemoveSituacaoId(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-Remove");

                            throw;
                        }
                    }
                });                        
            }

            return retorno;
        }

        [HttpGet]
        [Route("ObterSituacoes/{entidade}")]
        public List<SituacaoDTO> ObterSituacoes(string entidade)
        {
            var situacoes = new List<SituacaoDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    situacoes = new bSituacao(db).ObterSituacoes(entidade);

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-ObterSituacoes");



                    throw ex;
                }

                return situacoes;
            }
        }



        [HttpGet]
        [Route("GetTomadorServico")]
        public List<OutputGetTomadorServico> GetTomadorServico()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var situacoes = new bSituacao(db).BuscaSituacoTomadorServico().Select(s => new OutputGetTomadorServico()
                    {
                        IdSituacao = s.IdSituacao,
                        DsSituacao = s.DsSituacao

                    }).OrderBy(o => o.DsSituacao).ToList();

                    return situacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-GetTomadorServico");



                    throw;
                }
            }
        }


        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Abril/2021 
         *  Lista de Situacoes apenas de Propostas
         ===========================================================================================*/
        [HttpGet]
        [Route("ListaSituacoesPropostas")]
        public List<OutPutGetSituacoesPropostas> ListaSituacoesPropostas()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var situacoes = new bSituacao(db).Get().Where(w => w.DsArea != "Gestor do Contrato"       &&
                                                                       w.DsArea != "Gestor de Contratos"      &&
                                                                       w.DsArea != "Gestor de Contrato"       &&
                                                                       w.DsArea != "Gestor de Oportunidades"  &&
                                                                       w.DsArea != "Gestor de Equipe Técnica" &&
                                                                       w.IdSituacao != 114 && w.IdSituacao != 117).Select(s => new OutPutGetSituacoesPropostas()
                    {
                        IdSituacaoProposta = s.IdSituacao,
                        DsSituacao         = s.DsSituacao
                    }).OrderBy(o => o.DsSituacao).ToList();

                    return situacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SituacaoController-ListaSituacoesPropostas");
                    throw;
                }
            }
        }

        #endregion

        #region Retornos
        public class OutputGet
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string DsArea { get; set; }
            public string IcEntidade { get; set; }
            public string IcEntregue { get; set; }
            public string IcNFEmitida { get; set; }
            public string IcFormatacao { get; set; }
            public short? NuOrdem { get; set; }

        }

        public class OutputGetId
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string DsArea { get; set; }
            public string IcEntidade { get; set; }
            public bool? IcEntregue { get; set; }
            public bool? IcNFEmitida { get; set; }
            public bool? IcFormatacao { get; set; }
            public short? NuOrdem { get; set; }
        }

        public class InputAddSituacao
        {
            public string DsSituacao { get; set; }
            public string DsArea { get; set; }
            public string IcEntidade { get; set; }
            public bool? IcEntregue { get; set; }
            public bool? IcNFEmitida { get; set; }
            public bool? IcFormatacao { get; set; }
            public short? NuOrdem { get; set; }

        }

        public class InputUpDateSituacao
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string DsArea { get; set; }
            public string IcEntidade { get; set; }
            public bool? IcEntregue { get; set; }
            public bool? IcNFEmitida { get; set; }
            public bool? IcFormatacao { get; set; }
            public short? NuOrdem { get; set; }

        }

        public class OutPutAddSituacao
        {
            public bool Result { get; set; }
            public int IdSituacao { get; set; }

        }

        public class OutPutUpDateSituacao
        {
            public bool Result { get; set; }
            public int IdSituacao { get; set; }
        }

        public class OutPutRemoveSituacao
        {
            public bool Result { get; set; }
            public int IdSituacao { get; set; }
        }

        public class OutputGetTomadorServico
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
        }

        public class OutPutGetSituacoes
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string Value { get; set; }
        }
        public class OutPutGetSituacoesPropostas
        {
            public int IdSituacaoProposta { get; set; }
            public string DsSituacao { get; set; }
        }

        #endregion
    }
}