using ApiFipe.DTOs;
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
    public class ParcelaController : ControllerBase
    {
        #region | Métodos

        [HttpPost]
        [Route("GerarCronogramaFinanceiro")]
        public void GerarCronogramaFinanceiro([FromBody] int idContrato)
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
                            new bParcela(db).GerarCronogramaFinanceiro(idContrato);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-GerarCronogramaFinanceiro");
                            throw ex;
                        }
                    }
                });
                       
            }
        }

        [HttpGet]
        [Route("CarregarCronogramaFinanceiro/{idContrato}")]
        public List<ParcelaDTO> CarregarCronogramaFinanceiro(int idContrato)
        {
            var parcelas = new List<ParcelaDTO>();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            parcelas = new bParcela(db).ObterParcelas(idContrato);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-CarregarCronogramaFinanceiro");
                            throw ex;
                        }
                    }
                });

                return parcelas;
            }
        }

        [HttpGet]
        [Route("CarregarCronogramaFinanceiroTemporaria/{idContrato}")]
        public List<ParcelaDTO> CarregarCronogramaFinanceiroTemporaria(int idContrato)
        {
            var parcelas = new List<ParcelaDTO>();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            parcelas = new bParcela(db).ObterParcelasTemporaria(idContrato);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-CarregarCronogramaFinanceiroTemporaria");
                            throw ex;
                        }
                    }
                });

                return parcelas;
            }
        }

        [HttpGet]
        [Route("ConsultarParcela/{idParcela}")]
        public ParcelaDTO ConsultarParcela(int idParcela)
        {
            var parcela = new ParcelaDTO();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    parcela = new bParcela(db).ConsultarParcela(idParcela);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ConsultarParcela");
		
                   
                    throw ex;
                }

                return parcela;
            }
        }

        [HttpGet]
        [Route("ConsultarParcelaTemporaria/{idParcela}")]
        public ParcelaDTO ConsultarParcelaTemporaria(int idParcela)
        {
            var parcela = new ParcelaDTO();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    parcela = new bParcela(db).ConsultarParcelaTemporaria(idParcela);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ConsultarParcelaTemporaria");


                    throw ex;
                }

                return parcela;
            }
        }

        [HttpPost]
        [Route("CriarNovaParcela")]
        public void CriarNovaParcela([FromBody] InputParcela parcela)
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
                            new bParcela(db).CriarNovaParcela(parcela);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-CriarNovaParcela");

                            throw ex;
                        }
                    }
                });                        
            }
        }

        [HttpPost]
        [Route("CriarNovaParcelaTemporaria")]
        public void CriarNovaParcelaTemporaria([FromBody] InputParcela parcela)
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
                            new bParcela(db).CriarNovaParcelaTemporaria(parcela);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-CriarNovaParcelaTemporaria");

                            throw ex;
                        }
                    }
                });
            }
        }

        [HttpPost]
        [Route("AtualizarParcela")]
        public void AtualizarParcela([FromBody] InputParcela parcela)
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
                            new bParcela(db).AtualizarParcela(parcela);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-AtualizarParcela");

                            throw ex;
                        }
                    }
                });                        
            }
        }

        [HttpPost]
        [Route("AtualizarParcelaTemporaria")]
        public void AtualizarParcelaTemporaria([FromBody] InputParcela parcela)
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
                            new bParcela(db).AtualizarParcelaTemporaria(parcela);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-AtualizarParcelaTemporaria");

                            throw ex;
                        }
                    }
                });
            }
        }

        [HttpPost]
        [Route("SalvarParcela")]
        public void SalvarParcela([FromBody] InputParcela parcela)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var strategy = db.Database.CreateExecutionStrategy();

                    strategy.Execute(() =>
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            var bParcela = new bParcela(db);

                            if (parcela.Id == 0)
                                bParcela.CriarNovaParcela(parcela);
                            else
                                bParcela.AtualizarParcela(parcela);

                            db.Database.CommitTransaction();
                        }
                    });                        
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-SalvarParcela");
		

                    throw ex;
                }
            }
        }

        [HttpPost]
        [Route("SalvarParcelaTemporaria")]
        public void SalvarParcelaTemporaria([FromBody] InputParcela parcela)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var strategy = db.Database.CreateExecutionStrategy();

                    strategy.Execute(() =>
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            var bParcela = new bParcela(db);

                            if (parcela.Id == 0)
                                bParcela.CriarNovaParcelaTemporaria(parcela);
                            else
                                bParcela.AtualizarParcelaTemporaria(parcela);

                            db.Database.CommitTransaction();
                        }
                    });
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-SalvarParcelaTemporaria");


                    throw ex;
                }
            }
        }

        [HttpPost]
        [Route("ExcluirParcela")]
        public bool ExcluirParcela([FromBody] int idParcela)
        {
            bool parcelaExcluida = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {                            
                            new bParcela(db).ExcluirParcela(idParcela);
                            db.Database.CommitTransaction();
                            parcelaExcluida = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ExcluirParcela");

                            db.Database.RollbackTransaction();
                        }
                    }
                });
                        
            }

            return parcelaExcluida;
        }

        [HttpPost]
        [Route("ExcluirParcelaTemporaria")]
        public bool ExcluirParcelaTemporaria([FromBody] int idParcela)
        {
            bool parcelaExcluida = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bParcela(db).ExcluirParcelaTemporaria(idParcela);
                            db.Database.CommitTransaction();
                            parcelaExcluida = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ExcluirParcelaTemporaria");

                            db.Database.RollbackTransaction();
                        }
                    }
                });

            }

            return parcelaExcluida;
        }

        [HttpPost]
        [Route("SalvarAlteracoesGrid")]
        public bool SalvarAlteracoesGrid([FromBody] ParcelaDTO[] parcelasAlteradas)
        {
            bool operacaoBemSucedida = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var parcelas = parcelasAlteradas.GroupBy(_ => _.Id).Select(_ => _.Last()).ToList();
                            var bParcela = new bParcela(db);

                            for (int i = 0; i < parcelas.Count; i++)
                            {                                
                                bParcela.AtualizarParcela(parcelas[i]);
                            }

                            operacaoBemSucedida = true;

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-SalvarAlteracoesGrid");

                            throw ex;
                        }
                    }
                   
                });                        

                return operacaoBemSucedida;
            }
        }

        [HttpPost]
        [Route("SalvarAlteracoesGridTemporaria")]
        public bool SalvarAlteracoesGridTemporaria([FromBody] ParcelaDTO[] parcelasAlteradas)
        {
            bool operacaoBemSucedida = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var parcelas = parcelasAlteradas.GroupBy(_ => _.Id).Select(_ => _.Last()).ToList();
                            var bParcela = new bParcela(db);

                            for (int i = 0; i < parcelas.Count; i++)
                            {
                                if (parcelas[i].Situacao.Id == 98)
                                    parcelas[i].DtFaturamento = null;

                                bParcela.AtualizarParcelaTemporaria(parcelas[i]);
                            }

                            operacaoBemSucedida = true;

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-SalvarAlteracoesGridTemporaria");

                            throw ex;
                        }
                    }

                });

                return operacaoBemSucedida;
            }
        }
        
        [HttpGet]
        [Route("ObterValorTotalContrato/{idContrato}")]
        public decimal ObterValorTotalContrato(int idContrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    return new bParcela(db).ObterValorContrato(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ObterValorTotalContrato");
                    throw ex;
                }
            }
        }

        [HttpPost]
        [Route("ValidarValorParcela")]
        public bool ValidarValorParcela([FromBody] InputParcela parcela)
        {
            bool parcelaValida = false;

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bParcela = new bParcela(db);
                    var parcelas = bParcela.ObterParcelas(parcela.IdContrato);
                    decimal valorContrato = bParcela.ObterValorContrato(parcela.IdContrato);

                    if (parcelas == null)
                    {
                        parcelaValida = parcela.Valor <= valorContrato;
                    }
                    else
                    {
                        decimal? valorParcelas = parcelas.Where(_ => parcela.IdContrato == 0 || _.Id != parcela.Id)?.Sum(_ => _.Valor);
                        parcelaValida = (valorParcelas == null ? parcela.Valor : (valorParcelas.Value + parcela.Valor)) <= valorContrato;
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ValidarValorParcela");
		

                    throw ex;
                }

                return parcelaValida;
            }
        }

        [HttpPost]
        [Route("ValidarValorParcelaTemporaria")]
        public bool ValidarValorParcelaTemporaria([FromBody] InputParcela parcela)
        {
            bool parcelaValida = false;

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bParcela = new bParcela(db);
                    var parcelas = bParcela.ObterParcelasTemporaria(parcela.IdContrato);
                    decimal valorContrato = bParcela.ObterValorContrato(parcela.IdContrato);

                    if (parcelas == null)
                    {
                        parcelaValida = parcela.Valor <= valorContrato;
                    }
                    else
                    {
                        decimal? valorParcelas = parcelas.Where(_ => parcela.IdContrato == 0 || _.Id != parcela.Id)?.Sum(_ => _.Valor);
                        parcelaValida = (valorParcelas == null ? parcela.Valor : (valorParcelas.Value + parcela.Valor)) <= valorContrato;
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ValidarValorParcelaTemporaria");


                    throw ex;
                }

                return parcelaValida;
            }
        }

        [HttpGet]
        [Route("HabilitarCronogramaFinanceiro/{idContrato}")]
        public bool HabilitarCronogramaFinanceiro(int idContrato)
        {
            bool habilitado = false;

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bParcela = new bParcela(db);
                    var clientes = bParcela.ObterClientes(idContrato);
                    habilitado = clientes != null && clientes.Count > 0;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-HabilitarCronogramaFinanceiro");
		
 
                    throw ex;
                }

                return habilitado;
            }
        }

        [HttpGet]
        [Route("ValidarCriacaoCronogramaFinanceiro/{idContrato}")]
        public bool ValidarCriacaoCronogramaFinanceiro(int idContrato)
        {
            bool contratoValido = false;

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bParcela = new bParcela(db);
                    contratoValido = bParcela.ContratoPossuiFrente(idContrato) && bParcela.ContratoPossuiPagador(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-ValidarCriacaoCronogramaFinanceiro");
		

                    throw ex;
                }

                return contratoValido;
            }
        }

        [HttpGet]
        [Route("CarregarHistorico/{idAditivo}")]
        public List<ParcelaHistoricoDTO> CarregarHistorico(int idAditivo)
        {
            var parcelas = new List<ParcelaHistoricoDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    parcelas = new bParcela(db).ObterHistorico(idAditivo);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ParcelaController-CarregarHistorico");
		

                    throw ex;
                }

                return parcelas;
            }
        }

        #endregion
    }
}