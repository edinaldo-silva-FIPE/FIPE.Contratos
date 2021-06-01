using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCoordenacaoController : ControllerBase
    {
        [HttpGet]
        [Route("ListaTiposCoordenacoes")]
        public List<OutPutGetTipoCoordenacao> ListaTiposCoordenacoes(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTiposCoordenacoes = new List<OutPutGetTipoCoordenacao>();
                    var lstTiposCoordenacoes = new bTipoCoordenacao(db).ListaTiposCoordenacoes();

                    foreach (var coo in lstTiposCoordenacoes)
                    {
                        var tipoCoordenacao = new OutPutGetTipoCoordenacao();
                        tipoCoordenacao.IdTipoCoordenacao = coo.IdTipoCoordenacao;
                        tipoCoordenacao.DsTipoCoordenacao = coo.DsTipoCoordenacao;

                        lstRetornoTiposCoordenacoes.Add(tipoCoordenacao);
                    }

                    return lstRetornoTiposCoordenacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCoordenacaoController-ListaTiposCoordenacoes");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutputGetCoordenacaoId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var coordenacao = new OutputGetCoordenacaoId();
                    var coo = new bTipoCoordenacao(db).BuscaTipoCoordenacaoId(id);
                    coordenacao.IdTipoCoordenacao = coo.IdTipoCoordenacao;
                    coordenacao.DsTipoCoordenacao = coo.DsTipoCoordenacao;

                    return coordenacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCoordenacaoController-GetById");



                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoCoordenacao")]
        public OutPutAddTipoCoordenacao Add([FromBody] InputAddTipoCoordenacao item)
        {
            var retorno = new OutPutAddTipoCoordenacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var coordenacao = new TipoCoordenacao();
                            coordenacao.DsTipoCoordenacao = item.DsTipoCoordenacao;

                            var addRetorno = new bTipoCoordenacao(db).AddTipoCoordenacao(coordenacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCoordenacaoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoCoordenacao")]
        public OutPutUpDateTipoCoordenacao Update([FromBody] InputUpDateTipoCoordenacao item)
        {
            var retorno = new OutPutUpDateTipoCoordenacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var coordenacao = new TipoCoordenacao();
                            coordenacao.IdTipoCoordenacao = item.IdTipoCoordenacao;
                            coordenacao.DsTipoCoordenacao = item.DsTipoCoordenacao;

                            var updateRetorno = new bTipoCoordenacao(db).UpdateTipoCoordenacao(coordenacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCoordenacaoController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoCoordenacao/{id}")]
        public OutPutRemoveTipoCoordenacao Remove(int id)
        {
            var retorno = new OutPutRemoveTipoCoordenacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoCoordenacao(db).RemoveTipoCoordenacao(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCoordenacaoController-Remove");



                            throw;
                        }
                    }
                });
            }

            return retorno;
        }
    }

    #region Retornos
    public class OutPutGetTipoCoordenacao
    {
        public int IdTipoCoordenacao { get; set; }
        public string DsTipoCoordenacao { get; set; }
    }

    public class OutputGetCoordenacaoId
    {
        public int IdTipoCoordenacao { get; set; }
        public string DsTipoCoordenacao { get; set; }
    }

    public class InputAddTipoCoordenacao
    {
        public string DsTipoCoordenacao { get; set; }
    }

    public class InputUpDateTipoCoordenacao
    {
        public int IdTipoCoordenacao { get; set; }
        public string DsTipoCoordenacao { get; set; }

    }

    public class OutPutAddTipoCoordenacao
    {
        public bool Result { get; set; }
        public int IdTipoCoordenacao { get; set; }

    }

    public class OutPutUpDateTipoCoordenacao
    {
        public bool Result { get; set; }
        public int IdTipoCoordenacao { get; set; }

    }

    public class OutPutRemoveTipoCoordenacao
    {
        public bool Result { get; set; }
        public int TipoCoordenacao { get; set; }

    }

    #endregion
}