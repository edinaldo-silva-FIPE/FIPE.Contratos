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
    public class ClassificacaoController : ControllerBase
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
                    var classificacoes = new bClassificacao(db).Get().Select(s => new OutputGet()
                    {
                        IdClassificacao = s.IdClassificacaoEmpresa,
                        DsClassificacao = s.DsClassificacaoEmpresa
                    }).OrderBy(o => o.DsClassificacao).ToList();

                    return classificacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ClassificacaoController-Get");


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
                    var classificacao = new OutputGetId();
                    var cla = new bClassificacao(db).GetById(id);
                    classificacao.IdClassificacao = cla.IdClassificacaoEmpresa;
                    classificacao.DsClassificacao = cla.DsClassificacaoEmpresa;

                    return classificacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ClassificacaoController-GetById");

                    throw;
                }

            }
        }

        [HttpPost]
        [Route("AddClassificacao")]
        public OutPutAddClassificacao Add([FromBody] InputAddClassificacao item)
        {
            var retorno = new OutPutAddClassificacao();
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Inicia transação
                            

                            var classificacao = new ClassificacaoEmpresa();
                            classificacao.DsClassificacaoEmpresa = item.DsClassificacao;
                            var addRetorno = new bClassificacao(db).AddClassificacao(classificacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ClassificacaoController-Add");
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateClassificacao")]
        public OutPutUpDateClassificacao Update([FromBody] InputUpDateClassificacao item)
        {
            var retorno = new OutPutUpDateClassificacao();
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Inicia transação
                            

                            var cla = new ClassificacaoEmpresa();
                            cla.IdClassificacaoEmpresa = item.IdClassificacao;
                            cla.DsClassificacaoEmpresa = item.DsClassificacao;

                            var updateRetorno = new bClassificacao(db).UpdateClassificacao(cla);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }

                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ClassificacaoController-Update");


                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveClassificacao/{id}")]
        public OutPutRemoveClassificacao Remove(int id)
        {
            var retorno = new OutPutRemoveClassificacao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            // Inicia transação
                            

                            retorno.Result = new bClassificacao(db).RemoveClassificacaoId(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ClassificacaoController-Remove");
                            throw;
                        }
                    }

                    return retorno;
                });
                return retorno;
            }
        }

        #endregion

        #region Retornos
        public class OutputGet
        {
            public int IdClassificacao { get; set; }
            public string DsClassificacao { get; set; }
        }

        public class OutputGetId
        {
            public int IdClassificacao { get; set; }
            public string DsClassificacao { get; set; }
        }

        public class InputAddClassificacao
        {
            public string DsClassificacao { get; set; }
        }

        public class InputUpDateClassificacao
        {
            public int IdClassificacao { get; set; }
            public string DsClassificacao { get; set; }

        }

        public class OutPutAddClassificacao
        {
            public bool Result { get; set; }
            public int IdClassificacao { get; set; }
        }

        public class OutPutUpDateClassificacao
        {
            public bool Result { get; set; }
            public int IdClassificacao { get; set; }
        }

        public class OutPutRemoveClassificacao
        {
            public bool Result { get; set; }
            public int IdClassificacao { get; set; }
        }
        #endregion
    }
}