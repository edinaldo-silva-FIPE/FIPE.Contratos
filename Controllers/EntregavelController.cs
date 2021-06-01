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
    public class EntregavelController : ControllerBase
    {
        #region | Métodos

        [HttpPost]
        [Route("CriarEntregaveisEmLote")]
        public void CriarEntregaveisEmLote([FromBody] InputEntregavel entregavel)
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
                            var bEntregavel = new bEntregavel(db);
                            bEntregavel.CriarEntregaveisEmLote(entregavel);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-CriarEntregaveisEmLote");

                            throw ex;
                        }
                    }
                });
            }
        }

        [HttpGet]
        [Route("CarregarEntregaveis/{idContrato}")]
        public List<EntregavelDTO> CarregarEntregaveis(int idContrato)
        {
            var entregaveis = new List<EntregavelDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);
                    entregaveis = bEntregavel.ObterEntregaveis(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-CarregarEntregaveis");


                    throw ex;
                }
                return entregaveis;
            }
        }

        [HttpGet]
        [Route("CarregarEntregaveisTemporaria/{idContrato}")]
        public List<EntregavelDTO> CarregarEntregaveisTemporaria(int idContrato)
        {
            var entregaveis = new List<EntregavelDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);
                    entregaveis = bEntregavel.ObterEntregaveisTemporaria(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-CarregarEntregaveisTemporaria");


                    throw ex;
                }
                return entregaveis;
            }
        }

        [HttpPost]
        [Route("SalvarEntregavel")]
        public void SalvarEntregavel([FromBody] EntregavelDTO novoEntregavel)
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
                            var bEntregavel = new bEntregavel(db);

                            if (novoEntregavel.Id == 0)
                                bEntregavel.CriarNovoEntregavel(novoEntregavel.Nome, novoEntregavel.DataPrevista, novoEntregavel.IdContrato, novoEntregavel.Frente.Id, novoEntregavel.Numero, novoEntregavel.Cliente.Id, novoEntregavel.Situacao.Id);
                            else
                                bEntregavel.AtualizarEntregavel(novoEntregavel);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-SalvarEntregavel");

                            throw ex;
                        }
                    }
                });
            }
        }
        [HttpPost]
        [Route("SalvarEntregavelTemporaria")]
        public void SalvarEntregavelTemporaria([FromBody] EntregavelDTO novoEntregavel)
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
                            var bEntregavel = new bEntregavel(db);

                            if (novoEntregavel.Id == 0)
                                bEntregavel.CriarNovoEntregavelTemporaria(novoEntregavel.Nome, novoEntregavel.DataPrevista, novoEntregavel.IdContrato, novoEntregavel.Frente.Id, novoEntregavel.Numero, novoEntregavel.Cliente.Id, novoEntregavel.Situacao.Id);
                            else
                                bEntregavel.AtualizarEntregavelTemporaria(novoEntregavel);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-SalvarEntregavelTemporaria");

                            throw ex;
                        }
                    }
                });
            }
        }

        [HttpGet]
        [Route("ConsultarEntregavel/{idEntregavel}")]
        public EntregavelDTO ConsultarEntregavel(int idEntregavel)
        {
            var entregavel = new EntregavelDTO();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);
                    entregavel = bEntregavel.ConsultarEntregavel(idEntregavel);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ConsultarEntregavel");


                    throw ex;
                }

                return entregavel;
            }
        }

        [HttpGet]
        [Route("ConsultarEntregavelTemporaria/{idEntregavel}")]
        public EntregavelDTO ConsultarEntregavelTemporaria(int idEntregavel)
        {
            var entregavel = new EntregavelDTO();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);
                    entregavel = bEntregavel.ConsultarEntregavelTemporaria(idEntregavel);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ConsultarEntregavelTemporaria");


                    throw ex;
                }

                return entregavel;
            }
        }

        [HttpPost]
        [Route("ExcluirEntregavel")]
        public bool ExcluirEntregavel([FromBody] int idEntregavel)
        {
            bool entregavelExcluido = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            new bEntregavel(db).ExcluirEntregavel(idEntregavel);
                            db.Database.CommitTransaction();
                            entregavelExcluido = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ExcluirEntregavel");


                            db.Database.RollbackTransaction();
                        }
                    }

                    return entregavelExcluido;
                });
                return entregavelExcluido;
            }
        }

        [HttpPost]
        [Route("ExcluirEntregavelTemporaria")]
        public bool ExcluirEntregavelTemporaria([FromBody] int idEntregavel)
        {
            bool entregavelExcluido = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            new bEntregavel(db).ExcluirEntregavelTemporaria(idEntregavel);
                            db.Database.CommitTransaction();
                            entregavelExcluido = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ExcluirEntregavelTemporaria");


                            db.Database.RollbackTransaction();
                        }
                    }

                    return entregavelExcluido;
                });
                return entregavelExcluido;
            }
        }

        [HttpPost]
        [Route("SalvarAlteracoesGrid")]
        public bool SalvarAlteracoesGrid([FromBody] EntregavelDTO[] entregaveisAlterados)
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
                            var entregaveis = entregaveisAlterados.GroupBy(_ => _.Id).Select(_ => _.Last()).ToList();
                            var bEntregavel = new bEntregavel(db);

                            for (int i = 0; i < entregaveis.Count; i++)
                            {                                                            
                                bEntregavel.AtualizarEntregavel(entregaveis[i]);
                            }

                            operacaoBemSucedida = true;

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-SalvarAlteracoesGrid");

                            throw ex;
                        }
                        
                        return operacaoBemSucedida;
                    }
                });
                return operacaoBemSucedida;
            }
        }




        [HttpPost]
        [Route("SalvarAlteracoesGridTemporaria")]
        public bool SalvarAlteracoesGridTemporaria([FromBody] EntregavelDTO[] entregaveisAlterados)
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
                            var entregaveis = entregaveisAlterados.GroupBy(_ => _.Id).Select(_ => _.Last()).ToList();
                            var bEntregavel = new bEntregavel(db);

                            for (int i = 0; i < entregaveis.Count; i++)
                            {
                                bEntregavel.AtualizarEntregavelTemporaria(entregaveis[i]);
                            }

                            operacaoBemSucedida = true;

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-SalvarAlteracoesGridTemporaria");

                            throw ex;
                        }

                        return operacaoBemSucedida;
                    }
                });
                return operacaoBemSucedida;
            }
        }

        [HttpGet]
        [Route("CarregarHistorico/{idAditivo}")]
        public List<EntregavelHistoricoDTO> CarregarHistorico(int idAditivo)
        {
            var entregaveis = new List<EntregavelHistoricoDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);
                    entregaveis = bEntregavel.ObterHistorico(idAditivo);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-CarregarHistorico");


                    throw ex;
                }
                return entregaveis;
            }
        }

        [HttpPost]
        [Route("ObterEntregaveisPorId")]
        public List<EntregavelDTO> ObterEntregaveisPorId(int[] idsEntregaveis)
        {
            List<EntregavelDTO> entregaveis = new List<EntregavelDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);

                    foreach (int id in idsEntregaveis)
                        entregaveis.Add(bEntregavel.ConsultarEntregavel(id));
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ObterEntregaveisPorId");
                    throw ex;
                }
            }

            return entregaveis;
        }

        [HttpPost]
        [Route("ObterEntregaveisPorIdTemporaria")]
        public List<EntregavelDTO> ObterEntregaveisPorIdTemporaria(int[] idsEntregaveis)
        {
            List<EntregavelDTO> entregaveis = new List<EntregavelDTO>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var bEntregavel = new bEntregavel(db);

                    foreach (int id in idsEntregaveis)
                        entregaveis.Add(bEntregavel.ConsultarEntregavelTemporaria(id));
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "EntregavelController-ObterEntregaveisPorIdTemporaria");
                    throw ex;
                }
            }

            return entregaveis;
        }

        #endregion
    }
}