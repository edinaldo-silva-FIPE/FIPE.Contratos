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
    public class EntidadeController : ControllerBase
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
                    var entidades = new bEntidade(db).Get().Select(s => new OutputGet()
                    {
                        IdEntidade = s.IdEntidade,
                        DsEntidade = s.DsEntidade,
                        DsTipoEntidade = new bEntidade(db).GetTipoEntidade().Where(w => w.IdTipoEntidade == s.IdTipoEntidade).Single().DsTipoEntidade
                    }).OrderBy(o => o.DsEntidade).ToList();

                    return entidades;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-Get");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetAllTipoEntidade")]
        public List<OutputGetTipoEntidade> GetAllTipoEntidade()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var tipoEntidades = new bEntidade(db).GetTipoEntidade().Select(s => new OutputGetTipoEntidade()
                    {
                        IdTipoEntidade = s.IdTipoEntidade,
                        DsTipoEntidade = s.DsTipoEntidade,
                    }).OrderBy(o => o.DsTipoEntidade).ToList();

                    return tipoEntidades;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-GetAllTipoEntidade");

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
                    var entidade = new OutputGetId();
                    var ent = new bEntidade(db).GetById(id);
                    entidade.IdEntidade = ent.IdEntidade;
                    entidade.DsEntidade = ent.DsEntidade;
                    entidade.IdTipoEntidade = ent.IdTipoEntidade.Value;

                    return entidade;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-GetById");

                    throw;
                }

            }
        }

        [HttpPost]
        [Route("AddEntidade")]
        public OutPutAddEntidade Add([FromBody] InputAddEntidade item)
        {
            var retorno = new OutPutAddEntidade();

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
                            

                            var entidade = new Entidade();
                            entidade.DsEntidade = item.DsEntidade;
                            entidade.IdTipoEntidade = item.IdTipoEntidade;
                            var addRetorno = new bEntidade(db).AddEntidade(entidade);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-Add");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateEntidade")]
        public OutPutUpDateEntidade Update([FromBody] InputUpDateEntidade item)
        {
            var retorno = new OutPutUpDateEntidade();

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
                            

                            var entidade = new Entidade();
                            entidade.IdEntidade = item.IdEntidade;
                            entidade.DsEntidade = item.DsEntidade;
                            entidade.IdTipoEntidade = item.IdTipoEntidade;

                            var updateRetorno = new bEntidade(db).UpdateEntidade(entidade);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-Update");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveEntidade/{id}")]
        public OutPutRemoveEntidade Remove(int id)
        {
            var retorno = new OutPutRemoveEntidade();

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
                            

                            retorno.Result = new bEntidade(db).RemoveEntidadeId(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntidadeController-Remove");
                            
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
            public int IdEntidade { get; set; }
            public string DsEntidade { get; set; }
            public string DsTipoEntidade { get; set; }
        }

        public class OutputGetId
        {
            public int IdEntidade { get; set; }
            public string DsEntidade { get; set; }
            public int IdTipoEntidade { get; set; }
        }

        public class InputAddEntidade
        {
            public string DsEntidade { get; set; }
            public int IdTipoEntidade { get; set; }
        }

        public class InputUpDateEntidade
        {
            public int IdEntidade { get; set; }
            public string DsEntidade { get; set; }
            public int IdTipoEntidade { get; set; }

        }

        public class OutPutAddEntidade
        {
            public bool Result { get; set; }
            public int IdEntidade { get; set; }
        }

        public class OutPutUpDateEntidade
        {
            public bool Result { get; set; }
            public int IdEntidade { get; set; }
        }

        public class OutPutRemoveEntidade
        {
            public bool Result { get; set; }
            public int IdEntidade { get; set; }
        }

        public class OutputGetTipoEntidade
        {
            public int IdTipoEntidade { get; set; }
            public string DsTipoEntidade { get; set; }
        }
        #endregion
    }
}