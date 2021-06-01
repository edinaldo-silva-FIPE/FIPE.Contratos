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
    public class FormacaoProfissionalController : ControllerBase
    {
        [HttpGet]
        [Route("ListaFormacaoProfissional")]
        public List<OutPutGetFormacaoProf> ListaFormacaoProfissional(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoFormacaoProfissional = new List<OutPutGetFormacaoProf>();
                    var lstFormacaoProfissional = new bFormacaoProfissional(db).ListaFormacaoProfissional();

                    foreach (var form in lstFormacaoProfissional)
                    {
                        var formacaoProfissional = new OutPutGetFormacaoProf();
                        formacaoProfissional.IdFormacaoProfissional = form.IdFormacaoProfissional;
                        formacaoProfissional.DsFormacaoProfissional = form.DsFormacaoProfissional;

                        lstRetornoFormacaoProfissional.Add(formacaoProfissional);
                    }

                    return lstRetornoFormacaoProfissional;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FormacaoProfissionalController-FormacaoProfissionalController");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutputGetFormacaoProfissionalId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var formacaoProfissional = new OutputGetFormacaoProfissionalId();
                    var form = new bFormacaoProfissional(db).BuscaFormacaoProfissionalId(id);
                    formacaoProfissional.IdFormacaoProfissional = form.IdFormacaoProfissional;
                    formacaoProfissional.DsFormacaoProfissional = form.DsFormacaoProfissional;

                    return formacaoProfissional;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FormacaoProfissionalController-GetById");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddFormacaoProfissional")]
        public OutPutAddFormacaoProfissional Add([FromBody] InputAddFormacaoProfissional item)
        {
            var retorno = new OutPutAddFormacaoProfissional();

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
                            

                            var formacaoProfissional = new FormacaoProfissional();
                            formacaoProfissional.DsFormacaoProfissional = item.DsFormacaoProfissional;

                            var addRetorno = new bFormacaoProfissional(db).AddFormacaoProfissional(formacaoProfissional);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FormacaoProfissionalController-Add");
                            
                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateFormacaoProfissional")]
        public OutPutUpDateFormacaoProfissional Update([FromBody] InputUpDateFormacaoProfissional item)
        {
            var retorno = new OutPutUpDateFormacaoProfissional();

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
                            

                            var formacaoProfissional = new FormacaoProfissional();
                            formacaoProfissional.IdFormacaoProfissional = item.IdFormacaoProfissional;
                            formacaoProfissional.DsFormacaoProfissional = item.DsFormacaoProfissional;

                            var updateRetorno = new bFormacaoProfissional(db).UpdateFormacaoProfissional(formacaoProfissional);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FormacaoProfissionalController-Update");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveFormacaoProfissional/{id}")]
        public OutPutRemoveFormacaoProfissional Remove(int id)
        {
            var retorno = new OutPutRemoveFormacaoProfissional();

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
                            

                            retorno.Result = new bFormacaoProfissional(db).RemoveFormacaoProfissional(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FormacaoProfissionalController-Remove");
                            
                            throw;
                        }
                    }

                    return retorno;
                });
                return retorno;
            }
        }
    }

    #region Retornos
    public class OutPutGetFormacaoProf
    {
        public int IdFormacaoProfissional { get; set; }
        public string DsFormacaoProfissional { get; set; }
    }

    public class OutputGetFormacaoProfissionalId
    {
        public int IdFormacaoProfissional { get; set; }
        public string DsFormacaoProfissional { get; set; }
    }

    public class InputAddFormacaoProfissional
    {
        public string DsFormacaoProfissional { get; set; }
    }

    public class InputUpDateFormacaoProfissional
    {
        public int IdFormacaoProfissional { get; set; }
        public string DsFormacaoProfissional { get; set; }

    }

    public class OutPutAddFormacaoProfissional
    {
        public bool Result { get; set; }
        public int IdFormacaoProfissional { get; set; }

    }

    public class OutPutUpDateFormacaoProfissional
    {
        public bool Result { get; set; }
        public int IdFormacaoProfissional { get; set; }

    }

    public class OutPutRemoveFormacaoProfissional
    {
        public bool Result { get; set; }
        public int FormacaoProfissional { get; set; }

    }

    #endregion
}