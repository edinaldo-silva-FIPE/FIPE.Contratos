using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class VinculoPessoaFisicaController : ControllerBase
    {
        [HttpPost]
        [Route("AddVinculo")]
        public OutPutAddVinculo AddVinculo([FromBody] InputAddVinculo item)
        {
            var retorno = new OutPutAddVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var addRetorno = new bVinculoPessoaFisica(db).AddVinculo(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "VinculoPessoaFisicaController-AddVinculo");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });

                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateVinculo")]
        public OutPutUpdateVinculo UpdateVinculo([FromBody] InputUpdateVinculo item)
        {
            var retorno = new OutPutUpdateVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var updateRetorno = new bVinculoPessoaFisica(db).UpdateVinculo(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = updateRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "VinculoPessoaFisicaController-UpdateVinculo");
                            
                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });                        

                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaVinculoPessoaFisicaId/{id}")]
        public List<OutPutGetVinculo> BuscaVinculoPessoaFisicaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var lstRetornoVinculos = new List<OutPutGetVinculo>();

                try
                {
                    var lstVinculos = new bVinculoPessoaFisica(db).BuscaVinculoPessoaFisicaId(id);
                    if (lstVinculos.Count > 0)
                    {
                        foreach (var vinculo in lstVinculos)
                        {
                            var retornoVinculo = new OutPutGetVinculo();
                            var tipoVinculo = new bTipoVinculo(db).BuscaTipoVinculoId(vinculo.IdTipoVinculo);
                            retornoVinculo.IdVinculoPessoaFisica = vinculo.IdVinculoPessoa;
                            retornoVinculo.IdTipoVinculo = vinculo.IdTipoVinculo;
                            retornoVinculo.DtInicioVinculo = vinculo.DtInicioVinculo;
                            retornoVinculo.DtFimVinculo = vinculo.DtFimVinculo;
                            retornoVinculo.DsTipoVinculo = tipoVinculo.DsTipoVinculo;
                            lstRetornoVinculos.Add(retornoVinculo);
                        }
                    }
                    return lstRetornoVinculos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "VinculoPessoaFisicaController-BuscaVinculoPessoaFisicaId");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaVinculoId/{id}")]
        public OutPutGetVinculo BuscaVinculoId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutPutGetVinculo();

                try
                {
                    var vinculo = new bVinculoPessoaFisica(db).BuscaVinculoId(id);
                    if (vinculo != null)
                    {
                        retorno.IdVinculoPessoaFisica = vinculo.IdVinculoPessoa;
                        retorno.IdTipoVinculo = vinculo.IdTipoVinculo;
                        retorno.DtInicioVinculo = vinculo.DtInicioVinculo;
                        retorno.DtFimVinculo = vinculo.DtFimVinculo;
                        var tipoVinculo = new bTipoVinculo(db).BuscaTipoVinculoId(vinculo.IdTipoVinculo);
                        retorno.DsTipoVinculo = tipoVinculo.DsTipoVinculo;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "VinculoPessoaFisicaController-BuscaVinculoId");



                    throw;
                }

            }
        }

        [HttpDelete]
        [Route("RemoveVinculoId/{id}")]
        public OutPutDeleteVinculo RemoveVinculoId(int id)
        {
            var retorno = new OutPutDeleteVinculo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno = new bVinculoPessoaFisica(db).RemoveVinculoId(id);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "VinculoPessoaFisicaController-RemoveVinculoId");
                            
                            throw;
                        }
                    }
                });                        
            }

            return retorno;
        }
    }

    #region Retornos
    public class InputAddVinculo
    {
        public int IdPessoaFisica { get; set; }
        public int IdTipoVinculo { get; set; }
        public DateTime? DtInicioVinculo { get; set; }
        public DateTime? DtFimVinculo { get; set; }
    }

    public class InputUpdateVinculo
    {
        public int IdVinculoPessoaFisica { get; set; }
        public int IdPessoaFisica { get; set; }
        public int IdTipoVinculo { get; set; }
        public DateTime? DtInicioVinculo { get; set; }
        public DateTime? DtFimVinculo { get; set; }
    }

    public class OutPutAddVinculo
    {
        public bool Result { get; set; }
    }

    public class OutPutUpdateVinculo
    {
        public bool Result { get; set; }
    }

    public class OutPutGetVinculo
    {
        public int IdVinculoPessoaFisica { get; set; }
        public DateTime? DtInicioVinculo { get; set; }
        public DateTime? DtFimVinculo { get; set; }
        public int IdTipoVinculo { get; set; }
        public string DsTipoVinculo { get; set; }
    }

    public class OutPutDeleteVinculo
    {
        public bool Result { get; set; }
    }
    #endregion
}