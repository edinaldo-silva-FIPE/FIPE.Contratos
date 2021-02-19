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
    public class TipoContatoController : ControllerBase
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
                    var contatos = new bTipoContato(db).Get().Select(s => new OutputGet()
                    {
                        IdTipoContato = s.IdTipoContato,
                        DsTipoContato = s.DsTipoContato,

                    }).OrderBy(o => o.DsTipoContato).ToList();

                    return contatos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-Get");



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
                    var contato = new OutputGetId();
                    var cont = new bTipoContato(db).GetById(id);
                    contato.IdTipoContato = cont.IdTipoContato;
                    contato.DsTipoContato = cont.DsTipoContato;

                    return contato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-GetById");



                    throw;
                }

            }
        }

        [HttpPost]
        [Route("AddTipoContato")]
        public OutPutAddTipoContato Add([FromBody] InputAddTipoContato item)
        {
            var retorno = new OutPutAddTipoContato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contato = new TipoContato();
                            contato.DsTipoContato = item.DsTipoContato;

                            var addRetorno = new bTipoContato(db).AddTipoContato(contato);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoContato")]
        public OutPutUpDateTipoContato Update([FromBody] InputUpDateTipoContato item)
        {
            var retorno = new OutPutUpDateTipoContato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contato = new TipoContato();
                            contato.IdTipoContato = item.IdTipoContato;
                            contato.DsTipoContato = item.DsTipoContato;

                            var updateRetorno = new bTipoContato(db).UpdateTipoContato(contato);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });                        

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoContato/{id}")]
        public OutPutRemoveTipoContato Remove(int id)
        {
            var retorno = new OutPutRemoveTipoContato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoContato(db).RemoveTipoContato(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-Remove");


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
            public int IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }

        }

        public class OutputGetId
        {
            public int IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }
        }

        public class InputAddTipoContato
        {
            public string DsTipoContato { get; set; }
        }

        public class InputUpDateTipoContato
        {
            public int IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }

        }

        public class OutPutAddTipoContato
        {
            public bool Result { get; set; }
            public int IdTipoContato { get; set; }

        }

        public class OutPutUpDateTipoContato
        {
            public bool Result { get; set; }
            public int IdTipoContato { get; set; }

        }

        public class OutPutRemoveTipoContato
        {
            public bool Result { get; set; }
            public int IdTipoContato { get; set; }

        }
        #endregion
    }
}