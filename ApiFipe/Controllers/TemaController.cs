using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TemaController : ControllerBase
    {
        #region Métodos
        [HttpGet]
        [Route("ListaTema")]
        public List<OutPutGetTema> Get()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var temas = new bTema(db).Get().Select(s => new OutPutGetTema()
                    {
                        IdTema = s.IdTema,
                        DsTema = s.DsTema
                    }).ToList();
                    return temas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TemaController-Get");



                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTema")]
        public OutPutAddTema Add([FromBody] InputAddTema item)
        {
            var retorno = new OutPutAddTema();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var addRetorno = new bTema(db).AddTema(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TemaController-Add");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTema")]
        public OutPutUpdateTema UpdateTema([FromBody] InputUpdateTema item)
        {
            var retorno = new OutPutUpdateTema();

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


                            var updateRetorno = new bTema(db).UpdateTema(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = updateRetorno;

                            return retorno;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TemaController-UpdateTema");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaTemaId/{id}")]
        public OutPutGetTema BuscaTemaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoTema = new OutPutGetTema();
                    var tema = new bTema(db).BuscaTemaId(id);

                    if (tema != null)
                    {
                        retornoTema.DsTema = tema.DsTema;
                    }

                    return retornoTema;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TemaController-BuscaTemaId");



                    throw;
                }
            }
        }

        [HttpDelete]
        [Route("RemoveTemaId/{id}")]
        public OutPutRemoveTema RemoveTemaId(int id)
        {
            var retorno = new OutPutRemoveTema();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            retorno = new bTema(db).RemoveTemaId(id);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TemaController-RemoveTemaId");

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
            public int IdTema { get; set; }
            public string DsTema { get; set; }
        }

        public class OutPutGetTema
        {
            public int IdTema { get; set; }
            public string DsTema { get; set; }
        }

        public class OutPutAddTema
        {
            public bool Result { get; set; }
        }

        public class OutPutUpdateTema
        {
            public bool Result { get; set; }
        }

        public class OutPutRemoveTema
        {
            public bool Result { get; set; }
        }

        public class InputAddTema
        {
            public string DsTema { get; set; }
        }

        public class InputUpdateTema
        {
            public int IdTema { get; set; }
            public string DsTema { get; set; }
        }
        #endregion
    }
}
