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
    public class IndiceReajusteController : ControllerBase
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
                    var indiceReajuste = new bIndiceReajuste(db).Get().Select(s => new OutputGet()
                    {
                        IdIndiceReajuste = s.IdIndiceReajuste,
                        DsIndiceReajuste = s.DsIndiceReajuste,

                    }).OrderBy(o => o.DsIndiceReajuste).ToList();

                    return indiceReajuste;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "IndiceReajusteController-Get");



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
                    var indiceReajuste = new OutputGetId();
                    var reaj = new bIndiceReajuste(db).GetById(id);
                    indiceReajuste.IdIndiceReajuste = reaj.IdIndiceReajuste;
                    indiceReajuste.DsIndiceReajuste = reaj.DsIndiceReajuste;

                    return indiceReajuste;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "IndiceReajusteController-GetById");
		

                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddIndiceReajuste")]
        public OutPutAddIndiceReajuste Add([FromBody] InputAddIndiceReajuste item)
        {
            var retorno = new OutPutAddIndiceReajuste();

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
                            

                            var indiceReajuste = new IndiceReajuste();
                            indiceReajuste.DsIndiceReajuste = item.DsIndiceReajuste;

                            var addRetorno = new bIndiceReajuste(db).AddIndiceReajuste(indiceReajuste);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "IndiceReajusteController-Add");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateindiceReajuste")]
        public OutPutUpDateIndiceReajuste Update([FromBody] InputUpDateIndiceReajuste item)
        {
            var retorno = new OutPutUpDateIndiceReajuste();

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
                            

                            var indiceReajuste = new IndiceReajuste();
                            indiceReajuste.IdIndiceReajuste = item.IdIndiceReajuste;
                            indiceReajuste.DsIndiceReajuste = item.DsIndiceReajuste;

                            var updateRetorno = new bIndiceReajuste(db).UpdateIndiceReajuste(indiceReajuste);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "IndiceReajusteController-Update");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveIndiceReajuste/{id}")]
        public OutPutRemoveIndiceReajuste Remove(int id)
        {
            var retorno = new OutPutRemoveIndiceReajuste();

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
                            

                            retorno.Result = new bIndiceReajuste(db).RemoveIndiceReajuste(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "IndiceReajusteController-Remove");
                            
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
            public int IdIndiceReajuste { get; set; }
            public string DsIndiceReajuste { get; set; }

        }

        public class OutputGetId
        {
            public int IdIndiceReajuste { get; set; }
            public string DsIndiceReajuste { get; set; }
        }

        public class InputAddIndiceReajuste
        {
            public string DsIndiceReajuste { get; set; }
        }

        public class InputUpDateIndiceReajuste
        {
            public short IdIndiceReajuste { get; set; }
            public string DsIndiceReajuste { get; set; }

        }

        public class OutPutAddIndiceReajuste
        {
            public bool Result { get; set; }
            public int IdIndiceReajuste { get; set; }

        }

        public class OutPutUpDateIndiceReajuste
        {
            public bool Result { get; set; }
            public int IdIndiceReajuste { get; set; }

        }

        public class OutPutRemoveIndiceReajuste
        {
            public bool Result { get; set; }
            public int IdIndiceReajuste { get; set; }

        }
        #endregion
    }
}
