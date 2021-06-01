using ApiFipe.DTOs;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using static ApiFipe.Models.bContratoReajuste;

namespace ApiFipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoReajusteController : ControllerBase
    {
        #region | Métodos

        [HttpGet]
        [Route("ConsultarContratoReajuste/{idContratoReajuste}")]
        public OutPutGetContratoReajuste ConsultarContratoReajuste(int idContratoReajuste)
        {
            OutPutGetContratoReajuste contratoReajuste;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    contratoReajuste = new bContratoReajuste(db).Consultar(idContratoReajuste);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-ConsultarContratoReajuste");
                    throw ex;
                }

                return contratoReajuste;
            }
        }

        [HttpGet]
        [Route("GetAllByIdContrato/{idContrato}")]
        public List<OutPutGetContratoReajuste> GetAllByIdContrato(int idContrato)
        {
            List<OutPutGetContratoReajuste> lstContratoReajuste;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    lstContratoReajuste = new bContratoReajuste(db).GetAllByIdContrato(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-GetAllByIdContrato");

                    throw ex;
                }

                return lstContratoReajuste;
            }
        }

        [HttpGet]
        [Route("CalcularValorContratoReajustado/{idContrato}/{pcReajuste}")]
        public decimal? CalcularValorContratoReajustado(int idContrato, decimal? pcReajuste)
        {
            decimal? valorcontratoReajustado;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    valorcontratoReajustado = new bContratoReajuste(db).CalcularValorContratoReajustado(idContrato, pcReajuste);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-CalcularValorContratoReajustado");

                    throw ex;
                }

                return valorcontratoReajustado;
            }
        }

        [HttpPost]
        [Route("SalvarReajuste")]
        public bool SalvarReajuste([FromBody] InputUpdateContratoReajuste reajuste)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bContratoReajuste(db).Atualizar(reajuste);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-SalvarReajuste");

                            return false;
                        }
                    }
                });
                return true;
            }
        }

        [HttpPost]
        [Route("AplicarReajuste")]
        public bool AplicarReajuste([FromBody] int idContratoReajuste)
        {            
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bContratoReajuste(db).AplicarContratoReajuste(idContratoReajuste);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-AplicarReajuste");

                            return false;
                        }
                    }
                });
                return true;
            }
        }

        [HttpPost]
        [Route("ConcluirReajuste")]
        public bool ConcluirReajuste([FromBody] InputConcluirReajuste concluirReajuste)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bContratoReajuste(db).ConcluirContratoReajuste(concluirReajuste);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-ConcluirReajuste");

                            return false;
                        }
                    }
                });
                return true;
            }
        }

        [HttpGet]
        [Route("CarregarHistorico/{idContratoReajuste}")]
        public List<ParcelaHistoricoDTO> CarregarHistorico(int idContratoReajuste)
        {
            var parcelas = new List<ParcelaHistoricoDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    parcelas = new bParcela(db).ObterHistoricoContratoReajuste(idContratoReajuste);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoReajusteController-CarregarHistorico");
                    throw ex;
                }

                return parcelas;
            }
        }

        #endregion
    }
}