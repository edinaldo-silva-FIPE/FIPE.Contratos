using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoVinculoController : ControllerBase
    {
        [HttpGet]
        [Route("ListaTiposVinculos")]
        public List<OutPutGetTipoVinculo> ListaTiposVinculos(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTiposVinculos = new List<OutPutGetTipoVinculo>();
                    var lstTiposVinculos = new bTipoVinculo(db).ListaTiposVinculos();

                    foreach (var tv in lstTiposVinculos)
                    {
                        var tipoVinculo = new OutPutGetTipoVinculo();
                        tipoVinculo.IdTipoVinculo = tv.IdTipoVinculo;
                        tipoVinculo.DsTipoVinculo = tv.DsTipoVinculo;

                        lstRetornoTiposVinculos.Add(tipoVinculo);
                    }

                    return lstRetornoTiposVinculos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoVinculoController-ListaTiposVinculos");
		

                    
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
                    var vinculo = new OutputGetId();
                    var vinc = new bTipoVinculo(db).BuscaTipoVinculoId(id);
                    vinculo.IdTipoVinculo = vinc.IdTipoVinculo;
                    vinculo.DsTipoVinculo = vinc.DsTipoVinculo;

                    return vinculo;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoVinculoController-GetById");
		

                    
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoVinculo")]
        public OutPutAddTipoVinculo Add([FromBody] InputAddTipoVinculo item)
        {
            var retorno = new OutPutAddTipoVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var vinculo = new TipoVinculo();
                            vinculo.DsTipoVinculo = item.DsTipoVinculo;

                            var addRetorno = new bTipoVinculo(db).AddTipoVinculo(vinculo);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoVinculoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                        
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoVinculo")]
        public OutPutUpDateTipoVinculo Update([FromBody] InputUpDateTipoVinculo item)
        {
            var retorno = new OutPutUpDateTipoVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var vinculo = new TipoVinculo();
                            vinculo.IdTipoVinculo = item.IdTipoVinculo;
                            vinculo.DsTipoVinculo = item.DsTipoVinculo;

                            var updateRetorno = new bTipoVinculo(db).UpdateTipoVinculo(vinculo);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoVinculoController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });                        
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoVinculo/{id}")]
        public OutPutRemoveTipoVinculo Remove(int id)
        {
            var retorno = new OutPutRemoveTipoVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoVinculo(db).RemoveTipoVinculo(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoVinculoController-Remove");
                            
                            throw;
                        }
                    }
                });                        
            }

            return retorno;
        }
    }

    #region Retornos
    public class OutPutGetTipoVinculo
    {
        public int IdTipoVinculo { get; set; }
        public string DsTipoVinculo { get; set; }
    }

    public class OutputGetId
    {
        public int IdTipoVinculo { get; set; }
        public string DsTipoVinculo { get; set; }
    }

    public class InputAddTipoVinculo
    {
        public string DsTipoVinculo { get; set; }
    }

    public class InputUpDateTipoVinculo
    {
        public int IdTipoVinculo { get; set; }
        public string DsTipoVinculo { get; set; }

    }

    public class OutPutAddTipoVinculo
    {
        public bool Result { get; set; }
        public int IdTipoVinculo { get; set; }

    }

    public class OutPutUpDateTipoVinculo
    {
        public bool Result { get; set; }
        public int IdTipoVinculo { get; set; }

    }

    public class OutPutRemoveTipoVinculo
    {
        public bool Result { get; set; }
        public int TipoVinculo { get; set; }

    }

    #endregion
}