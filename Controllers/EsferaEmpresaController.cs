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
    public class EsferaEmpresaController : ControllerBase
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
                    var esferas = new bEsfera(db).Get().OrderBy(o => o.DsEsferaEmpresa);
                    var listaEsferas = new List<OutputGet>();

                    foreach (var item in esferas)
                    {
                        var esfera = new OutputGet();

                        esfera.IdEsferaEmpresa = item.IdEsferaEmpresa;
                        esfera.DsEsferaEmpresa = item.DsEsferaEmpresa;
                        switch (item.IdClassificacaoEmpresa)
                        {
                            case 1:
                                esfera.DsClassificacaoEmpresa = "Privado";
                                break;
                            case 2:
                                esfera.DsClassificacaoEmpresa = "Público";
                                break;
                        }
                        listaEsferas.Add(esfera);
                    }

                    return listaEsferas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EsferaEmpresaController-Get");



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
                    var esferaEmpresa = new OutputGetId();
                    var esf = new bEsfera(db).GetById(id);
                    esferaEmpresa.IdEsferaEmpresa = esf.IdEsferaEmpresa;
                    esferaEmpresa.DsEsferaEmpresa = esf.DsEsferaEmpresa;
                    esferaEmpresa.IdClassificacaoEmpresa = esf.IdClassificacaoEmpresa.Value;

                    return esferaEmpresa;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EsferaEmpresaController-GetById");
		

                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddEsferaEmpresa")]
        public OutPutAddEsferaEmpresa Add([FromBody] InputAddEsferaEmpresa item)
        {
            var retorno = new OutPutAddEsferaEmpresa();

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
                            

                            var esferaEmpresa = new EsferaEmpresa();
                            esferaEmpresa.DsEsferaEmpresa = item.DsEsferaEmpresa;
                            esferaEmpresa.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;
                            var addRetorno = new bEsfera(db).AddEsfera(esferaEmpresa);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EsferaEmpresaController-Add");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateEsferaEmpresa")]
        public OutPutUpDateEsferaEmpresa Update([FromBody] InputUpDateEsferaEmpresa item)
        {
            var retorno = new OutPutUpDateEsferaEmpresa();

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
                            

                            var esferaEmpresa = new EsferaEmpresa();
                            esferaEmpresa.IdEsferaEmpresa = item.IdEsferaEmpresa;
                            esferaEmpresa.DsEsferaEmpresa = item.DsEsferaEmpresa;
                            esferaEmpresa.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;

                            var updateRetorno = new bEsfera(db).UpdateEsferaEmpresa(esferaEmpresa);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EsferaEmpresaController-Update");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveEsferaEmpresa/{id}")]
        public OutPutRemoveEsferaEmpresa Remove(int id)
        {
            var retorno = new OutPutRemoveEsferaEmpresa();

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
                            

                            retorno.Result = new bEsfera(db).RemoveEsferaEmpresaId(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EsferaEmpresaController-Remove");
                            
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
            public int IdEsferaEmpresa { get; set; }
            public string DsEsferaEmpresa { get; set; }
            public string DsClassificacaoEmpresa { get; set; }
        }

        public class OutputGetId
        {
            public int IdEsferaEmpresa { get; set; }
            public string DsEsferaEmpresa { get; set; }
            public int IdClassificacaoEmpresa { get; set; }
        }

        public class InputAddEsferaEmpresa
        {
            public string DsEsferaEmpresa { get; set; }
            public int IdClassificacaoEmpresa { get; set; }
        }

        public class InputUpDateEsferaEmpresa
        {
            public int IdEsferaEmpresa { get; set; }
            public string DsEsferaEmpresa { get; set; }
            public int IdClassificacaoEmpresa { get; set; }

        }

        public class OutPutAddEsferaEmpresa
        {
            public bool Result { get; set; }
            public int IdEsferaEmpresa { get; set; }
        }

        public class OutPutUpDateEsferaEmpresa
        {
            public bool Result { get; set; }
            public int IdEsferaEmpresa { get; set; }
        }

        public class OutPutRemoveEsferaEmpresa
        {
            public bool Result { get; set; }
            public int IdEsferaEmpresa { get; set; }
        }

        #endregion
    }
}