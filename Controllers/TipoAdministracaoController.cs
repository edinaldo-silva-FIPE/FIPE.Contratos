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
    public class TipoAdministracaoController : ControllerBase
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
                    var tipoAdm = new bTipoAdministracao(db).Get().Select(s => new OutputGet()
                    {
                        IdTipoAdministracao = s.IdTipoAdministracao,
                        DsTipoAdministracao = s.DsTipoAdministracao,

                    }).OrderBy(o => o.DsTipoAdministracao).ToList();

                    return tipoAdm;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAdministracaoController-Get");



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
                    var tipoAdm = new OutputGetId();
                    var adm = new bTipoAdministracao(db).GetById(id);
                    tipoAdm.IdTipoAdministracao = adm.IdTipoAdministracao;
                    tipoAdm.DsTipoAdministracao = adm.DsTipoAdministracao;

                    return tipoAdm;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAdministracaoController-GetById");



                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoAdministracao")]
        public OutPutAddTipoAdministracao Add([FromBody] InputAddTipoAdministracao item)
        {
            var retorno = new OutPutAddTipoAdministracao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var tipoAdm = new TipoAdministracao();
                            tipoAdm.DsTipoAdministracao = item.DsTipoAdministracao;

                            var addRetorno = new bTipoAdministracao(db).AddTipoAdministracao(tipoAdm);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAdministracaoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoAdministracao")]
        public OutPutUpDateTipoAdministracao Update([FromBody] InputUpDateTipoAdministracao item)
        {
            var retorno = new OutPutUpDateTipoAdministracao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var tipoAdm = new TipoAdministracao();
                            tipoAdm.IdTipoAdministracao = item.IdTipoAdministracao;
                            tipoAdm.DsTipoAdministracao = item.DsTipoAdministracao;

                            var updateRetorno = new bTipoAdministracao(db).UpdateTipoAdministracao(tipoAdm);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAdministracaoController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoAdministracao/{id}")]
        public OutPutRemoveTipoAdministracao Remove(int id)
        {
            var retorno = new OutPutRemoveTipoAdministracao();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoAdministracao(db).RemoveTipoAdministracao(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAdministracaoController-Remove");

                            throw;
                        }
                    }
                });

            }

            return retorno;
        }

        #endregion

        #region Retornos
        public class OutputGet
        {
            public int IdTipoAdministracao { get; set; }
            public string DsTipoAdministracao { get; set; }

        }

        public class OutputGetId
        {
            public int IdTipoAdministracao { get; set; }
            public string DsTipoAdministracao { get; set; }
        }

        public class InputAddTipoAdministracao
        {
            public string DsTipoAdministracao { get; set; }
        }

        public class InputUpDateTipoAdministracao
        {
            public int IdTipoAdministracao { get; set; }
            public string DsTipoAdministracao { get; set; }

        }

        public class OutPutAddTipoAdministracao
        {
            public bool Result { get; set; }
            public int IdTipoAdministracao { get; set; }

        }

        public class OutPutUpDateTipoAdministracao
        {
            public bool Result { get; set; }
            public int IdTipoAdministracao { get; set; }

        }

        public class OutPutRemoveTipoAdministracao
        {
            public bool Result { get; set; }
            public int IdTipoAdministracao { get; set; }

        }
        #endregion
    }
}