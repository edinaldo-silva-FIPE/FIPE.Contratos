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
    public class TipoDocAcompanhaNFController : ControllerBase
    {

        [HttpGet]
        [Route("ListaTipoDocAcompanhaNFsContrato")]
        public List<OutPutGetTipoDocAcompanhaNFs> ListaTipoDocAcompanhaNFsContratos(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTipoDocAcompanhaNFs = new List<OutPutGetTipoDocAcompanhaNFs>();
                    var lstTipoDocAcompanhaNFs = new bTipoDocAcompanhaNF(db).Get().Where(x=> x.IcPadrao != null);

                    foreach (var tv in lstTipoDocAcompanhaNFs)
                    {
                        var tipoDocAcompanhaNFs = new OutPutGetTipoDocAcompanhaNFs();
                        tipoDocAcompanhaNFs.IdTipoDocsAcompanhaNF = tv.IdTipoDocsAcompanhaNf;
                        tipoDocAcompanhaNFs.DsTipoDocsAcompanhaNF = tv.DsTipoDocsAcompanhaNf;

                        lstRetornoTipoDocAcompanhaNFs.Add(tipoDocAcompanhaNFs);
                    }

                    return lstRetornoTipoDocAcompanhaNFs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocAcompanhaNFController-ListaTipoDocAcompanhaNFs");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaTipoDocAcompanhaNFs")]
        public List<OutPutGetTipoDocAcompanhaNFs> ListaTipoDocAcompanhaNFs(int id)
        {
            using ( var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTipoDocAcompanhaNFs = new List<OutPutGetTipoDocAcompanhaNFs>();
                    var lstTipoDocAcompanhaNFs = new bTipoDocAcompanhaNF(db).Get();

                    foreach (var tv in lstTipoDocAcompanhaNFs)
                    {
                        var tipoDocAcompanhaNFs = new OutPutGetTipoDocAcompanhaNFs();
                        tipoDocAcompanhaNFs.IdTipoDocsAcompanhaNF = tv.IdTipoDocsAcompanhaNf;
                        tipoDocAcompanhaNFs.DsTipoDocsAcompanhaNF = tv.DsTipoDocsAcompanhaNf;

                        lstRetornoTipoDocAcompanhaNFs.Add(tipoDocAcompanhaNFs);
                    }

                    return lstRetornoTipoDocAcompanhaNFs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocAcompanhaNFController-ListaTipoDocAcompanhaNFs");



                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddTipoDocAcompanhaNFs")]
        public OutPutAddTipoDocAcompanhaNFs Add([FromBody] InputAddTipoDocAcompanhaNFs item)
        {
            var retorno = new OutPutAddTipoDocAcompanhaNFs();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var TipoDocAcompanhaNFs = new TipoDocsAcompanhaNf();
                            TipoDocAcompanhaNFs.DsTipoDocsAcompanhaNf = item.DsTipoDocsAcompanhaNF;
                            TipoDocAcompanhaNFs.IcPadrao = false;

                            var addRetorno = new bTipoDocAcompanhaNF(db).AddTipoDocsAcompanhaNf(TipoDocAcompanhaNFs);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocAcompanhaNFs-Add");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }

        }


        [HttpPut]
        [Route("UpdateTipoDocsAcompanhaNfs")]
        public OutputUpdateTipoDocAcompanhaNFs Update([FromBody] InputUpdateTipoDocAcompanhaNFs item)
        {
            var retorno = new OutputUpdateTipoDocAcompanhaNFs();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contato = new TipoDocsAcompanhaNf();
                            contato.IdTipoDocsAcompanhaNf = item.IdTipoDocsAcompanhaNf;
                            contato.DsTipoDocsAcompanhaNf = item.DsTipoDocsAcompanhaNF;
                            

                            var updateRetorno = new bTipoDocAcompanhaNF(db).UpdateTipoDocsAcompanhaNf(contato);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocAcompanhaNFs-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveTipoDocAcompanhaNFs/{id}")]
        public OutputRemoveTipoDocAcompanhaNFs Remove(int id)
        {
            var retorno = new OutputRemoveTipoDocAcompanhaNFs();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.Result = new bTipoDocAcompanhaNF(db).RemoveTipoDocsAcompanhaNf(id);
                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoDocAcompanhaNFs-Remove");


                            throw;
                        }
                    }
                });
            }

            return retorno;
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutPutGetTipoDocAcompanhaNFs GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var contato = new OutPutGetTipoDocAcompanhaNFs();
                    var cont = new bTipoDocAcompanhaNF(db).GetById(id);
                    contato.IdTipoDocsAcompanhaNF = cont.IdTipoDocsAcompanhaNf;
                    contato.DsTipoDocsAcompanhaNF = cont.DsTipoDocsAcompanhaNf;
                    return contato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContatoController-GetById");



                    throw;
                }

            }
        }







        #region Retornos
        public class OutPutGetTipoDocAcompanhaNFs
        {
            public int IdTipoDocsAcompanhaNF { get; set; }
            public string DsTipoDocsAcompanhaNF { get; set; }
        }

        public class OutPutAddTipoDocAcompanhaNFs
        {
            public bool Result { get; set; }
            public string DsTipoDocsAcompanhaNF { get; set; }
            public bool IcPadrao { get; set; }

        }

        public class InputAddTipoDocAcompanhaNFs
        {          
            public string DsTipoDocsAcompanhaNF { get; set; }
        }       

        public class OutputRemoveTipoDocAcompanhaNFs
        {
            public bool Result { get; set; }
            public int IdTipoDocsAcompanhaNf { get; set; }
        }

        public class OutputUpdateTipoDocAcompanhaNFs
        {
            public bool Result { get; set; }
            public string DsTipoDocsAcompanhaNF { get; set; }
        }



        public class InputUpdateTipoDocAcompanhaNFs
        {
            public int IdTipoDocsAcompanhaNf { get; set; }
            public string DsTipoDocsAcompanhaNF { get; set; }
        }
    }

    #endregion
}