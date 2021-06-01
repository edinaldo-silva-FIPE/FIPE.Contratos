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
    public class TipoDocumentoController : ControllerBase
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
                    var tipoDocs = new bTipoDocumento(db).Get();
                    var listaTipoDocs = new List<OutputGet>();

                    foreach (var item in tipoDocs)
                    {
                        var tipoDoc = new OutputGet();

                        tipoDoc.IdTipoDocumento = item.IdTipoDoc;
                        tipoDoc.DsTipoDocumento = item.DsTipoDoc;
                        tipoDoc.IcDocContratual = item.IcDocContratual == true ? "Sim" : "Não";

                        switch (item.IdEntidade)
                        {
                            case 1:
                                tipoDoc.DsTipoEntidade = "Oportunidade";
                                break;
                            case 2:
                                tipoDoc.DsTipoEntidade = "Proposta";
                                break;
                            case 3:
                                tipoDoc.DsTipoEntidade = "Contrato";
                                break;
                        }
                        listaTipoDocs.Add(tipoDoc);
                    }

                    return listaTipoDocs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocumentoController-Get");



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
                    var tipoDoc = new OutputGetId();
                    var doc = new bTipoDocumento(db).GetById(id);

                    tipoDoc.IdTipoDocumento = doc.IdTipoDoc;
                    tipoDoc.DsTipoDocumento = doc.DsTipoDoc;
                    tipoDoc.IdTipoEntidade = doc.IdEntidade.Value;
                    tipoDoc.IcDocContratual = doc.IcDocContratual.Value;

                    return tipoDoc;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocumentoController-GetById");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoDocumento")]
        public OutPutAddTipoDocumento Add([FromBody] InputAddTipoDocumento item)
        {
            var retorno = new OutPutAddTipoDocumento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var tipoDoc = new TipoDocumento();
                            tipoDoc.DsTipoDoc = item.DsTipoDocumento;
                            tipoDoc.IdEntidade = item.IdTipoEntidade;
                            tipoDoc.IcDocContratual = item.IcDocContratual;
                            tipoDoc.IdPrincipais = 2;

                            var addRetorno = new bTipoDocumento(db).AddTipoDocumento(tipoDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocumentoController-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateTipoDocumento")]
        public OutPutUpDateTipoDocumento Update([FromBody] InputUpDateTipoDocumento item)
        {
            var retorno = new OutPutUpDateTipoDocumento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var tipoDoc = new TipoDocumento();
                            tipoDoc.IdTipoDoc = Convert.ToInt16(item.IdTipoDocumento);
                            tipoDoc.DsTipoDoc = item.DsTipoDocumento;
                            tipoDoc.IdEntidade = item.IdTipoEntidade;
                            tipoDoc.IcDocContratual = item.IcDocContratual;
                            tipoDoc.IdPrincipais = 1;

                            var updateRetorno = new bTipoDocumento(db).UpdateTipoDocumento(tipoDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocumentoController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoDocumento/{id}")]
        public OutPutRemoveTipoDocumento Remove(int id)
        {
            var retorno = new OutPutRemoveTipoDocumento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoDocumento(db).RemoveTipoDocumento(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocumentoController-Remove");

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
            public int IdTipoDocumento { get; set; }
            public string DsTipoDocumento { get; set; }
            public string DsTipoEntidade { get; set; }
            public string IcDocContratual { get; set; }
        }

        public class OutputGetId
        {
            public int IdTipoDocumento { get; set; }
            public string DsTipoDocumento { get; set; }
            public int IdTipoEntidade { get; set; }
            public bool IcDocContratual { get; set; }            
        }

        public class InputAddTipoDocumento
        {
            public string DsTipoDocumento { get; set; }
            public int IdTipoEntidade { get; set; }
            public bool IcDocContratual { get; set; }

        }

        public class InputUpDateTipoDocumento
        {
            public int IdTipoDocumento { get; set; }
            public string DsTipoDocumento { get; set; }
            public int IdTipoEntidade { get; set; }
            public bool IcDocContratual { get; set; }

        }

        public class OutPutAddTipoDocumento
        {
            public bool Result { get; set; }
            public int IdTipoDocumento { get; set; }

        }

        public class OutPutUpDateTipoDocumento
        {
            public bool Result { get; set; }
            public int IdTipoDocumento { get; set; }

        }

        public class OutPutRemoveTipoDocumento
        {
            public bool Result { get; set; }
            public int IdTipoDocumento { get; set; }

        }
        #endregion
    }
}