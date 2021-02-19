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
    public class TipoOportunidadeController : ControllerBase
    {
        #region Métodos
        [HttpGet]
        [Route("ListaTipoOportunidade")]
        public List<OutPutGetTipoOportunidade> Get()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var TipoOportunidades = new bTipoOportunidade(db).Get().Select(s => new OutPutGetTipoOportunidade()
                    {
                        IdTipoOportunidade = s.IdTipoOportunidade,
                        DsTipoOportunidade = s.DsTipoOportunidade,
                        IcModulo = s.IcModulo 
                    }).ToList();
                    return TipoOportunidades;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-Get");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaTodosTipoOportunidade")]
        public List<OutPutGetTipoOportunidade> GetAll()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var TipoOportunidades = new bTipoOportunidade(db).GetAll().Select(s => new OutPutGetTipoOportunidade()
                    {
                        IdTipoOportunidade = s.IdTipoOportunidade,
                        DsTipoOportunidade = s.DsTipoOportunidade,
                        IcModulo = s.IcModulo
                    }).ToList();
                    return TipoOportunidades;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-GetAll");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaTipoProposta")]
        public List<OutPutGetTipoOportunidade> GetTipoProposta()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var TipoOportunidades = new bTipoOportunidade(db).GetTipoProposta().Select(s => new OutPutGetTipoOportunidade()
                    {
                        IdTipoOportunidade = s.IdTipoOportunidade,
                        DsTipoOportunidade = s.DsTipoOportunidade,
                        IcModulo = s.IcModulo
                    }).ToList();
                    return TipoOportunidades;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-GetTipoProposta");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoOportunidade")]
        public OutPutAddTipoOportunidade Add([FromBody] InputAddTipoOportunidade item)
        {
            var retorno = new OutPutAddTipoOportunidade();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var addRetorno = new bTipoOportunidade(db).AddTipoOportunidade(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoOportunidade")]
        public OutPutUpdateTipoOportunidade UpdateTipoOportunidade([FromBody] InputUpdateTipoOportunidade item)
        {
            var retorno = new OutPutUpdateTipoOportunidade();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var updateRetorno = new bTipoOportunidade(db).UpdateTipoOportunidade(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-UpdateTipoOportunidade");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaTipoOportunidadeId/{id}")]
        public OutPutGetTipoOportunidade BuscaTipoOportunidadeId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoTipoOportunidade = new OutPutGetTipoOportunidade();
                    var TipoOportunidade = new bTipoOportunidade(db).BuscaTipoOportunidadeId(id);

                    if (TipoOportunidade != null)
                    {
                        retornoTipoOportunidade.DsTipoOportunidade = TipoOportunidade.DsTipoOportunidade;
                        retornoTipoOportunidade.IcModulo = TipoOportunidade.IcModulo;
                    }

                    return retornoTipoOportunidade;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-BuscaTipoOportunidadeId");



                    throw;
                }
            }
        }

        [HttpDelete]
        [Route("RemoveTipoOportunidadeId/{id}")]
        public OutPutRemoveTipoOportunidade RemoveTipoOportunidadeId(int id)
        {
            var retorno = new OutPutRemoveTipoOportunidade();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno = new bTipoOportunidade(db).RemoveTipoOportunidadeId(id);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoOportunidadeController-RemoveTipoOportunidadeId");
                            
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
            public int IdTipoOportunidade { get; set; }
            public string DsTipoOportunidade { get; set; }
            public string IcModulo { get; set; }
        }

        public class OutPutGetTipoOportunidade
        {
            public int IdTipoOportunidade { get; set; }
            public string DsTipoOportunidade { get; set; }
            public string IcModulo { get; set; }
        }

        public class OutPutAddTipoOportunidade
        {
            public bool Result { get; set; }
        }

        public class OutPutUpdateTipoOportunidade
        {
            public bool Result { get; set; }
        }

        public class OutPutRemoveTipoOportunidade
        {
            public bool Result { get; set; }
        }

        public class InputAddTipoOportunidade
        {
            public string DsTipoOportunidade { get; set; }
            public string IcModulo { get; set; }
        }

        public class InputUpdateTipoOportunidade
        {
            public int IdTipoOportunidade { get; set; }
            public string DsTipoOportunidade { get; set; }
            public string IcModulo { get; set; }
        }
        #endregion
    }
}
