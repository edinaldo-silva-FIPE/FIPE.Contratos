using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bProposta;
using ApiFipe.Utilitario;


namespace ApiFipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropostaController : ControllerBase
    {
        GravaLog _GLog = new GravaLog();

        #region Metódos
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("Add")]
        public OutputAdd Add([FromBody] InputAddProposta item)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutputAdd();
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    try
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            // Grava registro                    
                            var addRetorno = new bProposta(db).Add(item);

                            // Confirma operações
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Proposta [IDOportunidade: " + item.IdOportunidade + " - IDProposta: " + addRetorno.IdProposta + "] criada com sucesso");

                            retorno = addRetorno;
                        }
                    }
                    catch (Exception ex)
                    {
                        new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-Add Oport [" + item.IdOportunidade + "] usuario [" + item.IdUsuarioCriacao + "]");
                        retorno.Result = false;
                    }
                });

                return retorno;
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(AutenticacaoActionFilter))]  //EGS 30.09.2020
        [Route("AddPropostaAditivo")]
        public OutputAddAditivo AddPropostaAditivo([FromBody] InputAddPropostaAditivo item)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutputAddAditivo();
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    try
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            //EGS 30.09.2020 Verifica se existe o contrato antes de criar ADITIVO
                            var bRetContrato = db.Contrato.Where(w => w.NuContratoEdit == item.NuContratoEdit).FirstOrDefault();
                            if (bRetContrato == null)
                            {
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "AddPropostaAditivo [ContratoEdit: " + item.NuContratoEdit + "] não encontrado para criar o aditivo");
                                retorno.Result = false;
                            }
                            else { 
                                // Grava registro                    
                                var addRetorno = new bProposta(db).AddPropostaAditivo(item);

                                // Confirma operações
                                db.Database.CommitTransaction();

                                retorno = addRetorno;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AddPropostaAditivo ContratoEdit [" + item.NuContratoEdit + "] usuario [" + item.IdUsuarioCriacao + "]");

                        retorno.Result = false;
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("EncaminharPropostaDiretoria")]
        public void EncaminharPropostaDiretoria([FromBody] InputEncaminharPropostaDiretoria item)
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
                            var proposta        = new bProposta(db).GetById(item.IdProposta);
                            var propostaHist    = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim  = DateTime.Now;
                            proposta.IdSituacao = 7;

                            var email = new bEmail(db).GetEmailById(11);
                            var linkVisualizar = item.Url + "/proposta/" + proposta.IdProposta;
                            var usuariosDiretores = db.PerfilUsuario.Where(p => p.IdPerfil == 6 && p.EnviaEmail == true).ToList();     //EGS 30.06.2020 So envia email se esta marcado
                            var assunto = String.Format(email.DsTitulo, proposta.IdProposta);
                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);
                            foreach (var usuarioDiretor in usuariosDiretores)
                            {
                                var usuario = db.Usuario.Where(w => w.IdUsuario == usuarioDiretor.IdUsuario).FirstOrDefault();
                                var pessoa  = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario.IdPessoa).FirstOrDefault();
                                var corpo   = String.Format(email.DsTexto, pessoa.NmPessoa, proposta.IdProposta, linkVisualizar);
                                                                
                                new bEmail(db).EnviarEmail(pessoa.CdEmail, pessoa.NmPessoa, assunto, corpo, null, string.Empty);
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta [" + proposta.IdProposta + "] para Diretoria enviado para [" + pessoa.CdEmail + "]");

                            }
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Proposta [" + proposta.IdProposta + "] para Diretoria gravado");

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-EncaminharPropostaDiretoria Proposta [" + item.IdProposta + "]");
                            throw ex;
                        }
                    }
                });
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("EnviaEmailsContatos")]
        public OutPutEnviaEmailsContatos EnviaEmailsContatos([FromBody] InputEnviaEmailsContatos item)
        {
            var retorno = new OutPutEnviaEmailsContatos();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var proposta = new bProposta(db).GetById(item.IdProposta);
                            var propostaDoc = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);

                            var propostaHist     = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim   = DateTime.Now;
                            proposta.IdSituacao  = 9;

                            if (item.EmailCliente)
                            {
                                if ((AppSettings.constGlobalUserID == 0) && ((Int32)proposta.IdUsuarioUltimaAlteracao != 0)) { AppSettings.constGlobalUserID = (Int32)proposta.IdUsuarioUltimaAlteracao; }
                                var contatoProposta = db.PropostaContato.Where(p => p.IdProposta == proposta.IdProposta && p.IdTipoContato == 2).ToList();

                                var email = new bEmail(db).GetEmailById(24);

                                new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);
                                foreach (var contato in contatoProposta)
                                {
                                    var corpo = String.Format(email.DsTexto, contato.NmContato, proposta.IdProposta);
                                    var assunto = String.Format(email.DsTitulo, proposta.IdProposta);

                                    new bEmail(db).EnviarEmail(contato.CdEmail, contato.NmContato, assunto, corpo, propostaDoc.DocFisico, propostaDoc.DsDoc);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email da Proposta [" + proposta.IdProposta + "] para Contato enviado para [" + contato.CdEmail + "]");
                                }
                            }
                            var newPropostaHist        = new PropostaHistorico();
                            newPropostaHist.IdProposta = proposta.IdProposta;
                            newPropostaHist.DtInicio   = DateTime.Now;
                            newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "EnviaEmailsContatos Historico [" + proposta.IdProposta + "] Usu Info [" + (Int32)proposta.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-EnviaEmailsContatos [" + item.IdProposta+ "]");
                            retorno.Result = false;
                        }
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("EnviaEmailsResponsaveis")]
        public OutputEnviaEmailsResponsaveis EnviaEmailsResponsaveis([FromBody] InputEnviaEmailsResponsaveis item)
        {
            var retorno = new OutputEnviaEmailsResponsaveis();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
               // _GLog._GravaLog(AppSettings.constGlobalUserID, "Inicio EnviaEmailsResponsaveis da Proposta [" + item.IdProposta + "]");

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var proposta = new bProposta(db).GetById(item.IdProposta);
                            var propostaDoc = new PropostaDocsPrincipais();
                            if (proposta.IdTipoOportunidade == 7)
                            {
                                propostaDoc = new bPropostaDocs(db).BuscaPropostaAditivoId(proposta.IdProposta);
                            }
                            else
                            {
                                propostaDoc = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);
                            }
                            propostaDoc.ParaAjustes = false;

                            // Ao solicitar ajustes , marca o flag na tabela de Proposta Docs Principais
                            var propostaDocPrincipal = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);
                            propostaDocPrincipal.ParaAjustes = false;

                            if (item.Coordenadores.Count <= 0)
                            {
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta [" + proposta.IdProposta + "] não enviado porque não tinha Coordenador selecionado");
                            } else {
                                string bEmailDestino = "";
                                new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);
                                foreach (var coordenador in item.Coordenadores)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById(coordenador);

                                    /*EGS 30.09.2020 Verifica se pode ou não mandar email*/
                                    #region Corpo do E-mail  
                                    if (new bProposta(db)._PodeEnviarEmail(item.IdProposta, coordenador, pessoaFisica.CdEmail))
                                    {
                                        var propostaCoordenador = new bPropostaCoordenador(db).GetByPropostaPessoa(proposta.IdProposta, pessoaFisica.IdPessoaFisica);
                                        var itemURL             = "https://sgpc.fipe.org.br";

                                        //EGS 30.07.2020 Se ambiente HML mesmo assim aponta para PRD
                                        if (!FIPEContratosContext.EnvironmentIsProduction)
                                        {
                                            itemURL = "https://sgpc.cit-homolog.com.br";
                                        }
                                        var linkAprovar         = itemURL + "/proposta/aprovarProposta/"         + propostaCoordenador.GuidPropostaCoordenador.ToString();
                                        var linkSolicitarAjuste = itemURL + "/proposta/solicitarAjusteProposta/" + propostaCoordenador.GuidPropostaCoordenador.ToString();
                                        var email               = new bEmail(db).GetEmailById(1);
                                        bEmailDestino           = pessoaFisica.CdEmail;
                                        propostaCoordenador.IcPropostaAprovada = null;
                                        propostaCoordenador.IcAnaliseLiberada  = true;

                                        var corpo   = String.Format(email.DsTexto, pessoaFisica.NmPessoa, proposta.IdProposta, linkAprovar, linkSolicitarAjuste);
                                        var assunto = String.Format(email.DsTitulo, proposta.IdProposta);
                                        //remover =============================================================================================================================
                                        //remover =============================================================================================================================
                                        //if (item.Url.Contains("sgpc.fipe.org.br"))
                                        //{
                                        new bEmail(db).EnviarEmail("marcio.rodrigues@fipe.org.br", pessoaFisica.NmPessoa, assunto, corpo, propostaDoc.DocFisico, propostaDoc.DsDoc);
                                        _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta [" + proposta.IdProposta + "] para Responsavel enviado para [" + "marcio.rodrigues@fipe.org.br" + "]");


                                        //}
                                        //else
                                        //{
                                        //    new bEmail(db).EnviarEmail(pessoaFisica.CdEmail, pessoaFisica.NmPessoa, assunto, corpo, propostaDoc.DocFisico, propostaDoc.DsDoc);
                                        //    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta [" + proposta.IdProposta + "] para Responsavel enviado para [" + pessoaFisica.CdEmail + "]");
                                        //}
                                    }
                                    //remover =============================================================================================================================
                                    //remover =============================================================================================================================
                                }
                                //-------------------------------------------------- Grava Historico de "Analise para Coordenador ---------------------------------- 27.07.2020 --------
                                var newPropostaHist          = new PropostaHistorico();
                                newPropostaHist.IdProposta   = proposta.IdProposta;
                                newPropostaHist.DtInicio     = DateTime.Now;
                                newPropostaHist.DtFim        = DateTime.Now;
                                newPropostaHist.IdUsuario    = AppSettings.constGlobalUserID;
                                newPropostaHist.IdSituacao   = 5;
                                db.PropostaHistorico.Add(newPropostaHist);
                                db.SaveChanges();
                                //-------------------------------------------------- Grava Historico de "Analise para Coordenador ---------------------------------- 27.07.2020 --------

                            }
                            #endregion
                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            retorno.Result = true;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-EnviaEmailsResponsaveis");

                            retorno.Result = false;
                        }

                    }
                });
                return retorno;
            }
        }




























        [HttpGet]
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [Route("VerificaAnaliseLiberada/{guidPropostaCoordenador}")]
        public int VerificaAnaliseLiberada(string guidPropostaCoordenador)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno             = 0;
                    var propostaCoordenador = db.PropostaCoordenador.Where(w => w.GuidPropostaCoordenador.ToString() == guidPropostaCoordenador).FirstOrDefault();

                    if (propostaCoordenador == null)
                    {
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "Sem Coordenador na guidPropostaCoord [" + guidPropostaCoordenador + "]");
                    }
                    else { 
                        if (propostaCoordenador.IcAnaliseLiberada != null && (bool)!propostaCoordenador.IcAnaliseLiberada)
                        {
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Tem Coordenador na guidPropostaCoord [" + guidPropostaCoordenador + "] com [IcAnaliseLiberada: " + propostaCoordenador.IcAnaliseLiberada.ToString() + "] NÃO sera enviado email");
                          //retorno = false;   //EGS 30.06.2020 Originalmente verificava se Analise Liberada era verdadeiro...
                        }
                        else {
                            retorno = propostaCoordenador.IdProposta;
                        }
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-VerificaAnaliseLiberada");
                    throw;
                }

            }
        }



        [HttpPost]
        [ServiceFilter(typeof(AutenticacaoActionFilter))]  //EGS 30.09.2020
        [Route("AprovaPropostaCoordenador")]
        public OutputAprovaPropostaCoordenador AprovaPropostaCoordenador([FromBody] InputAprovaPropostaCoordenador item)
        {
            var retorno = new OutputAprovaPropostaCoordenador();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                //30.09.2020
                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var propostaCoordenador    = new bPropostaCoordenador(db).GetByGuid(item.GuidPropostaCoordenador);
                            var proposta               = new bProposta(db).GetById(propostaCoordenador.IdProposta);
                            var coordenador            = new bResponsavel(db).BuscarPessoaId((Int32)propostaCoordenador.IdPessoa);
                            var usuariosGestorProposta = new bUsuario(db).GetGestoresProposta();
                            var emailAprovacao         = new bEmail(db).GetEmailById(3);
                            propostaCoordenador.IcPropostaAprovada = true;
                            propostaCoordenador.IcAnaliseLiberada  = false;    //EGS 30.06.2020 Comentado para sempre enviar email
                            db.SaveChanges();

                            var ultimoCoord = new bPropostaCoordenador(db).VerificaUltimoCoordenador(proposta.IdProposta);

                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);
                            foreach (var gestorProposta in usuariosGestorProposta)
                            {

                                #region Envia E-mail para o Gestor da Proposta informando a aprovação da Proposta pelo Coordenador
                                if (!ultimoCoord)
                                {
                                    //Corpo do E-mail Aprovado pelo Coordenador
                                    var corpo   = String.Format(emailAprovacao.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, coordenador.NmPessoa);
                                    var assunto = String.Format(emailAprovacao.DsTitulo, proposta.IdProposta);
                                    new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto, corpo, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta Aprovada Coordenador [" + proposta.IdProposta + "] foi enviado para [" + gestorProposta.CdEmail + "]");
                                }
                                #endregion

                                #region Envia E-mail para o Gestor da Proposta informando o término das análises Tem dos Coordenadores
                                if (ultimoCoord)
                                {
                                    var emailTermino         = new bEmail(db).GetEmailById(5);
                                    var emailTerminoAprovada = new bEmail(db).GetEmailById(10);
                                    var ajusteSolicitado     = new bPropostaCoordenador(db).VerificaSolicitacaoAjustes(proposta.IdProposta);
                                    var linkVisualizar       = item.Url + "/proposta/" + proposta.IdProposta;
                                    var corpo1               = string.Empty;
                                    var assunto1             = string.Empty;

                                    var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                                    propostaHist.DtFim = DateTime.Now;

                                    if (ajusteSolicitado)
                                    {
                                        assunto1 = String.Format(emailTermino.DsTitulo, proposta.IdProposta);
                                        #region Corpo do E-mail após o término da análise de todos os Coordenadores
                                        corpo1 = String.Format(emailTermino.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkVisualizar);
                                        #endregion
                                        proposta.IdSituacao = 26;
                                    }
                                    else
                                    {
                                        assunto1 = String.Format(emailTerminoAprovada.DsTitulo, proposta.IdProposta);
                                        #region Corpo do E-mail após o término da análise e aprovação de todos os Coordenadores
                                        corpo1 = String.Format(emailTerminoAprovada.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkVisualizar);
                                        #endregion
                                        proposta.IdSituacao = 24;
                                    }
                                    new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto1, corpo1, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta Término da Analise do Coordenador [" + proposta.IdProposta + "] foi enviado para [" + gestorProposta.CdEmail + "]");
                                    db.SaveChanges();
                                }
                                #endregion
                            }

                            var newPropostaHist         = new PropostaHistorico();
                            newPropostaHist.IdProposta  = proposta.IdProposta;
                            newPropostaHist.DtInicio    = DateTime.Now;
                            newPropostaHist.IdUsuario   = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao  = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "AprovaPropostaCoordenador HIstorico [" + proposta.IdProposta + "] Sit [" + proposta.IdSituacao + "] Historico inserido Usu Info [" + proposta.IdUsuarioCriacao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                            db.SaveChanges();

                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "AprovaPropostaCoordenador Proposta [" + proposta.IdProposta + "] feito Commit");

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AprovaPropostaCoordenador");


                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }




        [HttpPost]
        [ServiceFilter(typeof(AutenticacaoActionFilter))]  //EGS 30.09.2020
        [Route("SolicitaAjustePropostaCoordenador")]
        public OutputSolicitaAjustePropostaCoordenador SolicitaAjustePropostaCoordenador([FromBody] InputSolicitaAjustePropostaCoordenador item)
        {
            var retorno = new OutputSolicitaAjustePropostaCoordenador();
            _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitaAjustePropostaCoordenador GuidPropostaCoordenador [" + item.GuidPropostaCoordenador + "] entrando na rotina");

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var propostaCoordenador    = new bPropostaCoordenador(db).GetByGuid(item.GuidPropostaCoordenador);
                            propostaCoordenador.IcPropostaAprovada = false;
                          //propostaCoordenador.IcAnaliseLiberada = false;   //EGS 30.06.2020 Comentado para sempre enviar email
                            var proposta               = new bProposta(db).GetById(propostaCoordenador.IdProposta);
                            var coordenador            = new bResponsavel(db).BuscarPessoaId((Int32)propostaCoordenador.IdPessoa);
                            var usuariosGestorProposta = new bUsuario(db).GetGestoresProposta();
                            var emailAjuste            = new bEmail(db).GetEmailById(6);
                            proposta.IdSituacao        = 26;

                            // Ao solicitar ajustes , marca o flag na tabela de Proposta Docs Principais
                            var propostaDocPrincipal = new PropostaDocsPrincipais();
                            if (proposta.IdTipoOportunidade == 7)
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscaPropostaAditivoId(proposta.IdProposta);
                                // Remove Documento da Proposta Aditivo Final e salva na aba de Proposta Documentos   
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 28 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Solicita Ajuste Proposta Coordenador [" + proposta.IdProposta + "] oldDoc SaveChange proposta.IdTipoOportunidade == 7");
                                }
                            }
                            else
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);
                                // Remove Documento da Proposta Final e salva na aba de Proposta Documentos                            
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 4 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Solicita Ajuste Proposta Coordenador [" + proposta.IdProposta + "] oldDoc SaveChange");
                                }
                            }
                            propostaDocPrincipal.ParaAjustes = true;
                            var propostaHist                 = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim               = DateTime.Now;

                            var ultimoCoord = new bPropostaCoordenador(db).VerificaUltimoCoordenador(proposta.IdProposta);
                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                            foreach (var gestorProposta in usuariosGestorProposta)
                            {
                                #region Envia E-mail para o Gestor da Proposta informando a solicitação de ajustes
                                if (!ultimoCoord)
                                {
                                    #region Corpo do E-mail
                                    var corpo = String.Format(emailAjuste.DsTexto, gestorProposta.NmPessoa, coordenador.NmPessoa, proposta.IdProposta);
                                    #endregion

                                    var assunto = String.Format(emailAjuste.DsTitulo, proposta.IdProposta);
                                    
                                     new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto, corpo, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Solicita Ajuste Proposta Coordenador na Proposta [" + proposta.IdProposta + "] enviado para [" + gestorProposta.CdEmail + "]");
                                }
                                #endregion

                                #region Envia E-mail para o Gestor informando o término das análises dos Coordenadores                    
                                if (ultimoCoord)
                                {
                                    var emailTermino = new bEmail(db).GetEmailById(5);
                                    var linkVisualizar = item.Url + "/proposta/" + proposta.IdProposta;

                                    #region Corpo do E-mail após o término da análise de todos os Coordenadores
                                    var corpo1 = String.Format(emailTermino.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkVisualizar);
                                    #endregion

                                    var assunto1 = String.Format(emailTermino.DsTitulo, proposta.IdProposta);
                                    
                                    new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto1, corpo1, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Solicitar Ajuste na Proposta [" + proposta.IdProposta + "] enviado para [" + gestorProposta.CdEmail + "]");

                                    db.SaveChanges();

                                    retorno.Result = true;
                                }
                                #endregion
                            }
                            var newPropostaHist         = new PropostaHistorico();
                            newPropostaHist.IdProposta  = proposta.IdProposta;
                            newPropostaHist.DtInicio    = DateTime.Now;
                            newPropostaHist.IdUsuario   = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao  = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico da Proposta [" + proposta.IdProposta + "] criado com situaçao [" + proposta.IdSituacao + "] Usu Info [" + proposta.IdUsuarioCriacao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-SolicitaAjustePropostaCoordenador");


                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("AprovarPropostaDiretoria")]
        public OutputAprovarPropostaDiretoria AprovarPropostaDiretoria([FromBody] InputAprovarPropostaDiretoria item)
        {
            var retorno = new OutputAprovarPropostaDiretoria();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                         {
                            var proposta                      = new bProposta(db).BuscarPropostaId(item.IdProposta);
                            var usuariosGestorProposta        = new bUsuario(db).GetGestoresProposta();
                            var emailAprovacao                = new bEmail(db).GetEmailById(14);
                            var linkAprovar                   = item.Url + "/proposta/" + item.IdProposta;
                            proposta.IdSituacao               = 10;
                            proposta.IdTema                   = item.Idtema;
                            proposta.IdUsuarioUltimaAlteracao = item.IdUsuarioUltimaAlteracao;

                            var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim = DateTime.Now;
                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                            foreach (var gestorProposta in usuariosGestorProposta)
                            {
                                var corpo = String.Format(emailAprovacao.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkAprovar);
                                var assunto = String.Format(emailAprovacao.DsTitulo, proposta.IdProposta);

                                #region Envio de e-mail informando a Aprovação da Proposta pela Diretoria                               
                                new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto, corpo, null, string.Empty);
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Aprovado pela Diretoria na Proposta [" + proposta.IdProposta + "] enviado para [" + gestorProposta.CdEmail + "]");
                                #endregion
                            }
                            var newPropostaHist        = new PropostaHistorico();
                            newPropostaHist.IdProposta = proposta.IdProposta;
                            newPropostaHist.DtInicio   = DateTime.Now;
                            newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "AprovarPropostaDiretoria [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usu Info [" + (Int32)item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AprovarPropostaDiretoria");


                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("SolicitarAjustesPropostaDiretoria")]
        public OutputSolicitarAjustePropostaDiretoria SolicitarAjustesDiretoria([FromBody] InputSolicitarAjustePropostaDiretoria item)
        {
            var retorno = new OutputSolicitarAjustePropostaDiretoria();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var proposta                      = new bProposta(db).BuscarPropostaId(item.IdProposta);
                            var usuariosGestorProposta        = new bUsuario(db).GetGestoresProposta();
                            var emailAprovacao                = new bEmail(db).GetEmailById(17);
                            var linkAprovar                   = item.Url + "/proposta/" + item.IdProposta;
                            proposta.IdSituacao               = 26;
                            proposta.IdUsuarioUltimaAlteracao = item.IdUsuarioUltimaAlteracao;

                            // Ao solicitar ajustes , marca o flag na tabela de Proposta Docs Principais
                            var propostaDocPrincipal = new PropostaDocsPrincipais();
                            if (proposta.IdTipoOportunidade == 7)
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscaPropostaAditivoId(proposta.IdProposta);
                                // Remove Documento da Proposta Aditivo Final e salva na aba de Proposta Documentos   
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 28 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesDiretoria na Proposta [" + proposta.IdProposta + "] SaveChange 1");
                                }
                            }
                            else
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);
                                // Remove Documento da Proposta Final e salva na aba de Proposta Documentos                            
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 4 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesDiretoria na Proposta [" + proposta.IdProposta + "] SaveChange 2");
                                }
                            }
                            propostaDocPrincipal.ParaAjustes = true;

                            var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim = DateTime.Now;
                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                            foreach (var gestorProposta in usuariosGestorProposta)
                            {
                                var corpo = String.Format(emailAprovacao.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkAprovar);
                                var assunto = String.Format(emailAprovacao.DsTitulo, proposta.IdProposta);

                                    #region Envio de e-mail informando a Aprovação da Proposta pela Diretoria                                                
                                    new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto, corpo, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Solicitar Ajustes Diretoria na Proposta [" + proposta.IdProposta + "] enviado para [" + gestorProposta.CdEmail + "]");
                                #endregion

                            }
                            var newPropostaHist        = new PropostaHistorico();
                            newPropostaHist.IdProposta = proposta.IdProposta;
                            newPropostaHist.DtInicio   = DateTime.Now;
                            newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesDiretoria Historico [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usu Info [" + (Int32)item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-SolicitarAjustesDiretoria ID [" + item.IdProposta + "]");
                            retorno.Result = false;
                        }
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("SolicitarAjustesCliente/{idProposta}")]
        public bool SolicitarAjustesCliente(int idProposta)
        {
            var retorno = false;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var proposta = new bProposta(db).BuscarPropostaId(idProposta);
                            proposta.IdSituacao = 26;

                            // Ao solicitar ajustes , marca o flag na tabela de Proposta Docs Principais                            
                            var propostaDocPrincipal = new PropostaDocsPrincipais();
                            if (proposta.IdTipoOportunidade == 7)
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscaPropostaAditivoId(proposta.IdProposta);
                                // Remove Documento da Proposta Aditivo Final e salva na aba de Proposta Documentos   
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 28 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                  //_GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesCliente na Proposta [" + proposta.IdProposta + "] proposta.IdTipoOportunidade == 7 SaveChange");
                                }
                            }
                            else
                            {
                                propostaDocPrincipal = new bPropostaDocs(db).BuscarMinutaId(proposta.IdProposta);
                                // Remove Documento da Proposta Final e salva na aba de Proposta Documentos   
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == 4 && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                                if (oldDoc != null)
                                {
                                    db.PropostaDocsPrincipais.Remove(oldDoc);
                                    db.SaveChanges();
                                }
                              //_GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesCliente na Proposta [" + proposta.IdProposta + "] SaveChange 2");
                            }
                            propostaDocPrincipal.ParaAjustes = true;

                            var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim = DateTime.Now;

                            var newPropostaHist        = new PropostaHistorico();
                            newPropostaHist.IdProposta = proposta.IdProposta;
                            newPropostaHist.DtInicio   = DateTime.Now;
                            newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao = proposta.IdSituacao;

                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesCliente Historico [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usu Info [" + (Int32)proposta.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-SolicitarAjustesCliente Proposta [" + idProposta + "]");

                            retorno = false;
                        }
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("SolicitarAjustesPropostaJuridico")]
        public OutputSolicitarAjustePropostaDiretoria SolicitarAjustesPropostaJuridico([FromBody] InputSolicitarAjustePropostaDiretoria item)
        {
            var retorno = new OutputSolicitarAjustePropostaDiretoria();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var proposta                      = new bProposta(db).BuscarPropostaId(item.IdProposta);
                            var usuariosGestorProposta        = new bUsuario(db).GetGestoresProposta();
                            var emailAprovacao                = new bEmail(db).GetEmailById(26);
                            var linkProposta                  = item.Url + "/proposta/" + item.IdProposta;
                            proposta.IdSituacao               = 26;
                            proposta.IdUsuarioUltimaAlteracao = item.IdUsuarioUltimaAlteracao;

                            var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim = DateTime.Now;
                            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                            foreach (var gestorProposta in usuariosGestorProposta)
                            {
                                var corpo = String.Format(emailAprovacao.DsTexto, gestorProposta.NmPessoa, proposta.IdProposta, linkProposta);
                                var assunto = String.Format(emailAprovacao.DsTitulo, proposta.IdProposta);
                                
                                    #region Envio de e-mail informando a solicitação de ajustes pelo Juridico
                                    new bEmail(db).EnviarEmail(gestorProposta.CdEmail, gestorProposta.NmPessoa, assunto, corpo, null, string.Empty);
                                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Solicitar Ajustes Proposta Juridico na Proposta [" + proposta.IdProposta + "] enviado para [" + gestorProposta.CdEmail + "]");
                                #endregion
                            }
                            var newPropostaHist         = new PropostaHistorico();
                            newPropostaHist.IdProposta  = proposta.IdProposta;
                            newPropostaHist.DtInicio    = DateTime.Now;
                            newPropostaHist.IdUsuario   = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao  = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "SolicitarAjustesPropostaJuridico Historico [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usu Info [" + (Int32)item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-SolicitarAjustesPropostaJuridico ID [" + item.IdProposta + "]");
                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaId/{id}")]
        public InputUpdateProposta BuscaPropostaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemProposta = new InputUpdateProposta();
                itemProposta.Clientes = new List<OutputCliente>();
                itemProposta.Coordenadores = new List<OutputCoordenador>();
                itemProposta.CoordenadoresSelecionados = new List<int>();

                try
                {
                    var retornoProposta = new bProposta(db).BuscarPropostaId(id);
                    var historicoSituacao = new bProposta(db).GetPropostaHist(id);
                    if (historicoSituacao.Count > 1)
                    {
                        var lastHistSituacao = historicoSituacao[1];
                        var situacao = new bSituacao(db).GetById(lastHistSituacao.IdSituacao);
                        itemProposta.PerfilSolicitanteAjuste = situacao.DsArea;
                    }

                    foreach (var itemCoordCli in retornoProposta.PropostaCliente)
                    {
                        var itemCli = new OutputCliente();

                        var cliente = db.Cliente.Where(w => w.IdCliente == itemCoordCli.IdCliente).FirstOrDefault();
                        var pessoa = db.Pessoa.Where(p => p.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                        itemCli.IdCliente = cliente.IdCliente;
                        if (pessoa.IdPessoaFisica != null)
                        {
                            var pFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                            itemCli.IdPessoa = pessoa.IdPessoa;
                            itemCli.NmCliente = pFisica.NmPessoa;
                        }
                        else if (pessoa.IdPessoaJuridica != null)
                        {
                            var pJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                            itemCli.IdPessoa = pessoa.IdPessoa;
                            itemCli.NmCliente = itemCoordCli.NmFantasia;
                        }
                        itemProposta.Clientes.Add(itemCli);
                    }

                    foreach (var itemCoordResp in retornoProposta.PropostaCoordenador)
                    {
                        var itemCoord = new OutputCoordenador();

                        var coordenador = db.PessoaFisica.Where(w => w.IdPessoaFisica == itemCoordResp.IdPessoa).FirstOrDefault();

                        itemCoord.IdPessoaFisica = coordenador.IdPessoaFisica;
                        itemCoord.NmPessoa = coordenador.NmPessoa;
                        itemCoord.IcPropostaAprovada = itemCoordResp.IcPropostaAprovada;
                        if (itemCoordResp.IcAprovado != null)
                        {
                            itemCoord.IcAprovado = (bool)itemCoordResp.IcAprovado;
                            if (itemCoord.IcAprovado)
                            {
                                itemProposta.CoordenadoresSelecionados.Add(Convert.ToInt32(itemCoordResp.IdPessoa));
                            }
                        }
                        itemProposta.Coordenadores.Add(itemCoord);
                    }

                    if (retornoProposta.IdOportunidade != null)
                    {
                        itemProposta.IdOportunidade = (Int32)retornoProposta.IdOportunidade;
                    }

                    //EGS 30.09.2020
                    if ((AppSettings.constGlobalUserID == 0) && (retornoProposta.IdUsuarioCriacao != 0)) { AppSettings.constGlobalUserID = retornoProposta.IdUsuarioCriacao; }
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "BuscaPropostaId [" + id + "] Usu.Cria [" + retornoProposta.IdUsuarioCriacao + "] e GlobalUserID [" + AppSettings.constGlobalUserID + "] e sit [" + retornoProposta.IdSituacao + "]");

                    itemProposta.IdSituacao               = retornoProposta.IdSituacao;
                    itemProposta.IdTema                   = retornoProposta.IdTema;
                    itemProposta.IdTipoProposta           = retornoProposta.IdTipoProposta;
                    itemProposta.IdUsuarioCriacao         = retornoProposta.IdUsuarioCriacao;
                    var NmUsuario                         = new bUsuario(db).GetById(retornoProposta.IdUsuarioCriacao);
                    var pessoaFisica                      = new bPessoaFisica(db).GetById(NmUsuario.IdPessoa);
                    itemProposta.NmUsuario                = pessoaFisica.NmPessoa;

                    itemProposta.IdUsuarioUltimaAlteracao = retornoProposta.IdUsuarioUltimaAlteracao;
                    itemProposta.NuPrazoEstimadoMes       = retornoProposta.NuPrazoEstimadoMes;
                    itemProposta.DsApelidoProposta        = retornoProposta.DsApelidoProposta;
                    itemProposta.DsAssunto                = retornoProposta.DsAssunto;
                    itemProposta.DsObjeto                 = retornoProposta.DsObjeto;
                    itemProposta.NuPrazoExecucao          = retornoProposta.NuPrazoExecucao;
                    itemProposta.DtAssinaturaContrato     = retornoProposta.DtAssinaturaContrato;
                    itemProposta.DtAutorizacaoInicio      = retornoProposta.DtAutorizacaoInicio;
                    itemProposta.DtCriacao                = retornoProposta.DtCriacao;
                    itemProposta.DtLimiteEnvioProposta    = retornoProposta.DtLimiteEnvioProposta;
                    itemProposta.DtProposta               = retornoProposta.DtCriacao;
                    itemProposta.DtUltimaAlteracao        = retornoProposta.DtUltimaAlteracao;
                    itemProposta.DtValidadeProposta       = retornoProposta.DtValidadeProposta;
                    itemProposta.DtLimiteEntregaProposta  = retornoProposta.DtLimiteEntregaProposta;
                    itemProposta.VlProposta               = retornoProposta.VlProposta;
                    itemProposta.DtAssinaturaContrato     = retornoProposta.DtAssinaturaContrato;
                    itemProposta.Reajustes                = retornoProposta.Reajustes;
                    itemProposta.OrdemInicio              = retornoProposta.OrdemInicio;     //EGS 30.09.2020 Estava pegando campo errado
                    itemProposta.VlContrato               = retornoProposta.VlContrato;
                    itemProposta.IdTipoReajuste           = retornoProposta.IdTipoReajuste;
                    itemProposta.RenovacaoAutomatica      = retornoProposta.RenovacaoAutomatica;
                    itemProposta.IdFundamento             = retornoProposta.IdFundamento;
                    itemProposta.IdContrato               = retornoProposta.IdContrato;
                    itemProposta.IcContratantesValidos    = retornoProposta.IcContratantesValidos;
                    itemProposta.IcAditivoAnalisado       = retornoProposta.IcAditivoAnalisado;
                    if (retornoProposta.IdContrato != null)
                    {
                        var contrato = db.Contrato.Where(w => w.IdContrato == retornoProposta.IdContrato).FirstOrDefault();
                        if (contrato != null)
                        {
                            itemProposta.IdContrato = contrato.IdContrato;
                            itemProposta.NuContratoEdit = contrato.NuContratoEdit;
                            itemProposta.DtVigenciaAtual = contrato.DtFim;
                            itemProposta.DtVigenciaInicial = contrato.DtInicio;
                        }
                    }
                    else
                    {
                        var contrato = db.Contrato.Where(w => w.IdProposta == retornoProposta.IdProposta).FirstOrDefault();
                        if (contrato != null)
                        {
                            itemProposta.IdContrato = contrato.IdContrato;
                            itemProposta.NuContratoEdit = contrato.NuContratoEdit;
                            itemProposta.DtVigenciaAtual = contrato.DtFim;
                            itemProposta.DtVigenciaInicial = contrato.DtInicio;
                        }
                    }

                    itemProposta.DsObservacao = retornoProposta.DsObservacao;
                    if (retornoProposta.IdTipoOportunidade != null)
                    {
                        itemProposta.IdTipoOportunidade = retornoProposta.IdTipoOportunidade.Value;
                    }
                    itemProposta.IcRitoSumario = retornoProposta.IcRitoSumario;
                    itemProposta.DsAditivo = retornoProposta.DsAditivo;
                    if (itemProposta.IdSituacao == 100)
                    {
                        var contratoAditivo = db.ContratoAditivo.Where(w => w.IdProposta == id).FirstOrDefault();
                        itemProposta.IdTipoAditivo = contratoAditivo.IdTipoAditivo;
                        itemProposta.DtNovoFimVigencia = contratoAditivo.DtFimAditivada;
                        itemProposta.VlContratoComAditivo = contratoAditivo.VlContratoAditivado;
                        itemProposta.DsAditivo = contratoAditivo.DsAditivo;
                    }
                    itemProposta.IdUnidadeTempo = retornoProposta.IdUnidadeTempo;
                    itemProposta.IdUnidadeTempoJuridico = retornoProposta.IdUnidadeTempoJuridico;
                    itemProposta.NuPrazoExecucaoJuridico = retornoProposta.NuPrazoExecucaoJuridico;
                    itemProposta.NuPrazoEstimadoMesJuridico = retornoProposta.NuPrazoEstimadoMesJuridico;
                    itemProposta.NuPrazoEstimadoMes = retornoProposta.NuPrazoEstimadoMes;
                    itemProposta.NuContratoCliente = retornoProposta.NuContratoCliente;
                    itemProposta.IcInformacoesIncompletas = retornoProposta.IcInformacoesIncompletas;
                    itemProposta.IdTipoAditivo = retornoProposta.IdTipoAditivo;
                    itemProposta.DtNovoFimVigencia = retornoProposta.DtNovoFimVigencia;
                    itemProposta.VlContratoComAditivo = retornoProposta.VlContratoComAditivo;
                    itemProposta.IcAditivoData = retornoProposta.IcAditivoData;
                    itemProposta.IcAditivoEscopo = retornoProposta.IcAditivoEscopo;
                    itemProposta.IcAditivoValor = retornoProposta.IcAditivoValor;
                    itemProposta.VlAditivo = retornoProposta.VlAditivo;

                    return itemProposta;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaId ID [" + id + "]");
                    throw;
                }

            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaIdContrato/{id}")]
        public OutPutGetPropostaByContrato BuscaPropostaIdContrato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutPutGetPropostaByContrato();
                var itemProposta = new InputUpdateProposta();
                itemProposta.Clientes = new List<OutputCliente>();
                itemProposta.Coordenadores = new List<OutputCoordenador>();
                itemProposta.CoordenadoresSelecionados = new List<int>();

                try
                {
                    var contrato = db.Contrato.Where(w => w.IdContrato == id).FirstOrDefault();
                    if (contrato != null)
                    {
                        if (contrato.IdSituacao == 19)
                        {
                            var retornoProposta = new bProposta(db).BuscarPropostaId(contrato.IdProposta);

                            if (retornoProposta != null)
                            {
                                foreach (var itemCoordCli in retornoProposta.PropostaCliente)
                                {
                                    var itemCli = new OutputCliente();

                                    var cliente = db.Cliente.Where(w => w.IdCliente == itemCoordCli.IdCliente).FirstOrDefault();
                                    var pessoa = db.Pessoa.Where(p => p.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                    itemCli.IdCliente = cliente.IdCliente;
                                    if (pessoa.IdPessoaFisica != null)
                                    {
                                        var pFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                        itemCli.IdPessoa = pessoa.IdPessoa;
                                        itemCli.NmCliente = pFisica.NmPessoa;
                                    }
                                    else if (pessoa.IdPessoaJuridica != null)
                                    {
                                        var pJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                        itemCli.IdPessoa = pessoa.IdPessoa;
                                        itemCli.NmCliente = itemCoordCli.NmFantasia;
                                    }
                                    itemProposta.Clientes.Add(itemCli);
                                }

                                foreach (var itemCoordResp in retornoProposta.PropostaCoordenador)
                                {
                                    var itemCoord = new OutputCoordenador();

                                    var coordenador = db.PessoaFisica.Where(w => w.IdPessoaFisica == itemCoordResp.IdPessoa).FirstOrDefault();

                                    itemCoord.IdPessoaFisica = coordenador.IdPessoaFisica;
                                    itemCoord.NmPessoa = coordenador.NmPessoa;
                                    itemCoord.IcPropostaAprovada = itemCoordResp.IcPropostaAprovada;
                                    if (itemCoordResp.IcAprovado != null)
                                    {
                                        itemCoord.IcAprovado = (bool)itemCoordResp.IcAprovado;
                                        if (itemCoord.IcAprovado)
                                        {
                                            itemProposta.CoordenadoresSelecionados.Add(Convert.ToInt32(itemCoordResp.IdPessoa));
                                        }
                                    }
                                    itemProposta.Coordenadores.Add(itemCoord);
                                }

                                itemProposta.IdOportunidade       = (Int32)retornoProposta.IdOportunidade;
                                itemProposta.IdSituacao           = 4;
                                itemProposta.IdTema               = retornoProposta.IdTema;
                                itemProposta.IdTipoProposta       = retornoProposta.IdTipoProposta;
                                itemProposta.IdUsuarioCriacao     = retornoProposta.IdUsuarioCriacao;

                                var NmUsuario                     = new bUsuario(db).GetById(retornoProposta.IdUsuarioCriacao);

                                var pessoaFisica                  = new bPessoaFisica(db).GetById(NmUsuario.IdPessoa);
                                itemProposta.NmUsuario            = pessoaFisica.NmPessoa;

                                itemProposta.IdUsuarioUltimaAlteracao = retornoProposta.IdUsuarioUltimaAlteracao;
                                itemProposta.NuPrazoEstimadoMes = retornoProposta.NuPrazoEstimadoMes;
                                itemProposta.IdUnidadeTempo = retornoProposta.IdUnidadeTempo;
                                itemProposta.DsApelidoProposta = retornoProposta.DsApelidoProposta;
                                itemProposta.DsAssunto = retornoProposta.DsAssunto;
                                itemProposta.DsObjeto = retornoProposta.DsObjeto;
                                itemProposta.NuPrazoExecucao = retornoProposta.NuPrazoExecucao;
                                itemProposta.DtAssinaturaContrato = retornoProposta.DtAssinaturaContrato;
                                itemProposta.DtAutorizacaoInicio = retornoProposta.DtAutorizacaoInicio;
                                itemProposta.DtCriacao = retornoProposta.DtCriacao;
                                itemProposta.DtLimiteEnvioProposta = retornoProposta.DtLimiteEnvioProposta;
                                itemProposta.DtProposta = retornoProposta.DtCriacao;
                                itemProposta.DtUltimaAlteracao = retornoProposta.DtUltimaAlteracao;
                                itemProposta.DtValidadeProposta = retornoProposta.DtValidadeProposta;
                                itemProposta.DtLimiteEntregaProposta = retornoProposta.DtLimiteEntregaProposta;
                                itemProposta.VlProposta = retornoProposta.VlProposta;
                                itemProposta.DtAssinaturaContrato = retornoProposta.DtAssinaturaContrato;
                                itemProposta.Reajustes = retornoProposta.Reajustes;
                                itemProposta.OrdemInicio = retornoProposta.Reajustes;
                                itemProposta.VlContrato = retornoProposta.VlContrato;
                                itemProposta.IdTipoReajuste = retornoProposta.IdTipoReajuste;
                                itemProposta.RenovacaoAutomatica = retornoProposta.RenovacaoAutomatica;
                                itemProposta.IdFundamento = retornoProposta.IdFundamento;
                                if (contrato != null)
                                {
                                    itemProposta.IdContrato = contrato.IdContrato;
                                }
                                itemProposta.IcRitoSumario = retornoProposta.IcRitoSumario;

                                retorno.proposta = new InputUpdateProposta();
                                retorno.proposta = itemProposta;

                            }
                        }
                        else
                        {
                            retorno.ContratoNaoAtivo = true;
                        }
                    }
                    else
                    {
                        retorno.ContratoNaoCadastrado = true;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaIdContrato ID [" + id + "]");
                    throw ex;
                }

            }
        }




        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaNuContratoEdit/{nuContratoEdit}")]
        public OutPutGetPropostaByContrato BuscaPropostaNuContratoEdit(string nuContratoEdit)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutPutGetPropostaByContrato();
                var itemProposta = new InputUpdateProposta();
                itemProposta.Clientes = new List<OutputCliente>();
                itemProposta.Coordenadores = new List<OutputCoordenador>();
                itemProposta.CoordenadoresSelecionados = new List<int>();

                try
                {
                    var contrato = db.Contrato.Where(w => w.NuContratoEdit == nuContratoEdit).FirstOrDefault();
                    if (contrato != null)
                    {
                        if (contrato.IdSituacao == 19 || contrato.IdSituacao == 111)
                        {
                            var retornoProposta = new bProposta(db).BuscarPropostaId(contrato.IdProposta);

                            if (retornoProposta != null)
                            {
                                foreach (var itemCoordCli in retornoProposta.PropostaCliente)
                                {
                                    var itemCli = new OutputCliente();

                                    var cliente = db.Cliente.Where(w => w.IdCliente == itemCoordCli.IdCliente).FirstOrDefault();
                                    var pessoa = db.Pessoa.Where(p => p.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                    itemCli.IdCliente = cliente.IdCliente;
                                    if (pessoa.IdPessoaFisica != null)
                                    {
                                        var pFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                        itemCli.IdPessoa = pessoa.IdPessoa;
                                        itemCli.NmCliente = pFisica.NmPessoa;
                                    }
                                    else if (pessoa.IdPessoaJuridica != null)
                                    {
                                        var pJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                        itemCli.IdPessoa = pessoa.IdPessoa;
                                        itemCli.NmCliente = itemCoordCli.NmFantasia;
                                    }
                                    itemProposta.Clientes.Add(itemCli);
                                }

                                foreach (var itemCoordResp in retornoProposta.PropostaCoordenador)
                                {
                                    var itemCoord = new OutputCoordenador();

                                    var coordenador = db.PessoaFisica.Where(w => w.IdPessoaFisica == itemCoordResp.IdPessoa).FirstOrDefault();

                                    itemCoord.IdPessoaFisica = coordenador.IdPessoaFisica;
                                    itemCoord.NmPessoa = coordenador.NmPessoa;
                                    itemCoord.IcPropostaAprovada = itemCoordResp.IcPropostaAprovada;
                                    if (itemCoordResp.IcAprovado != null)
                                    {
                                        itemCoord.IcAprovado = (bool)itemCoordResp.IcAprovado;
                                        if (itemCoord.IcAprovado)
                                        {
                                            itemProposta.CoordenadoresSelecionados.Add(Convert.ToInt32(itemCoordResp.IdPessoa));
                                        }
                                    }
                                    itemProposta.Coordenadores.Add(itemCoord);
                                }

                                itemProposta.IdOportunidade = (Int32)retornoProposta.IdOportunidade;
                                itemProposta.IdSituacao = 4;
                                itemProposta.IdTema = retornoProposta.IdTema;
                                itemProposta.IdTipoProposta = retornoProposta.IdTipoProposta;
                                itemProposta.IdUsuarioCriacao = retornoProposta.IdUsuarioCriacao;

                                var NmUsuario = new bUsuario(db).GetById(retornoProposta.IdUsuarioCriacao);

                                var pessoaFisica = new bPessoaFisica(db).GetById(NmUsuario.IdPessoa);
                                itemProposta.NmUsuario = pessoaFisica.NmPessoa;

                                itemProposta.IdUsuarioUltimaAlteracao = retornoProposta.IdUsuarioUltimaAlteracao;
                                itemProposta.NuPrazoEstimadoMes = retornoProposta.NuPrazoEstimadoMes;
                                itemProposta.IdUnidadeTempo = retornoProposta.IdUnidadeTempo;
                                itemProposta.DsApelidoProposta = retornoProposta.DsApelidoProposta;
                                itemProposta.DsAssunto = retornoProposta.DsAssunto;
                                itemProposta.DsObjeto = retornoProposta.DsObjeto;
                                itemProposta.NuPrazoExecucao = retornoProposta.NuPrazoExecucao;
                                itemProposta.DtAssinaturaContrato = retornoProposta.DtAssinaturaContrato;
                                itemProposta.DtAutorizacaoInicio = retornoProposta.DtAutorizacaoInicio;
                                itemProposta.DtCriacao = retornoProposta.DtCriacao;
                                itemProposta.DtLimiteEnvioProposta = retornoProposta.DtLimiteEnvioProposta;
                                itemProposta.DtProposta = retornoProposta.DtCriacao;
                                itemProposta.DtUltimaAlteracao = retornoProposta.DtUltimaAlteracao;
                                itemProposta.DtValidadeProposta = retornoProposta.DtValidadeProposta;
                                itemProposta.DtLimiteEntregaProposta = retornoProposta.DtLimiteEntregaProposta;
                                itemProposta.VlProposta = retornoProposta.VlProposta;
                                itemProposta.DtAssinaturaContrato = retornoProposta.DtAssinaturaContrato;
                                itemProposta.Reajustes = retornoProposta.Reajustes;
                                itemProposta.OrdemInicio = retornoProposta.Reajustes;
                                itemProposta.VlContrato = retornoProposta.VlContrato;
                                itemProposta.IdTipoReajuste = retornoProposta.IdTipoReajuste;
                                itemProposta.RenovacaoAutomatica = retornoProposta.RenovacaoAutomatica;
                                itemProposta.IdFundamento = retornoProposta.IdFundamento;
                                if (contrato != null)
                                {
                                    itemProposta.IdContrato = contrato.IdContrato;
                                }
                                itemProposta.IcRitoSumario = retornoProposta.IcRitoSumario;

                                retorno.proposta = new InputUpdateProposta();
                                retorno.proposta = itemProposta;
                            }
                        }
                        else
                        {
                            retorno.ContratoNaoAtivo = true;
                        }
                    }
                    else
                    {
                        retorno.ContratoNaoCadastrado = true;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaNuContratoEdit [" + nuContratoEdit + "]");
                    throw ex;
                }

            }
        }







        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Setembro/2020 
        *  Verifica se ja existe um contrato de aditivo pendente
        ==============================================================================================*/
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaNuContratoAditivoAtivo/{nuContratoEdit}")]
        public bool BuscaNuContratoAditivoAtivo(string nuContratoEdit)
        {
            bool bRetorno = false;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var contrato = db.Contrato.Where(w => w.NuContratoEdit == nuContratoEdit).FirstOrDefault();
                    if (contrato != null)
                    {
                        //Acha o IDSituacao de Aditivo Pendente
                        var sitAditivoAtivo = db.Situacao.Where(s => s.DsSituacao == "Pendente" && s.IcEntidade=="A").FirstOrDefault().IdSituacao;
                        if (sitAditivoAtivo != 0)
                        {
                            var conAditivo = db.ContratoAditivo.Where(w => w.IdContrato == contrato.IdContrato && w.IdSituacao == sitAditivoAtivo).FirstOrDefault();
                            if (conAditivo != null)
                            {
                                bRetorno = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaNuContratoAditivoAtivo [" + nuContratoEdit + "]");
                    throw ex;
                }
            }
            return bRetorno;
        }



        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaMunicipio")]
        public List<OutPutMunicipio> ListaMunicipio()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaMunicipios = new List<OutPutMunicipio>();
                    var pCliente = new bPropostaCliente(db).GetAll();
                    var i = 0;

                    foreach (var item in pCliente)
                    {
                        var itemCidade = new OutPutMunicipio();
                        if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisica != null)
                        {
                            var cidadeFis = pCliente[i].IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.IdCidadeNavigation.NmCidade;
                            itemCidade.NmCidade = cidadeFis;
                            listaMunicipios.Add(itemCidade);
                        }
                        if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridica != null)
                        {
                            var cidadeJur = pCliente[i].IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdCidadeNavigation.NmCidade;
                            itemCidade.NmCidade = cidadeJur;
                            listaMunicipios.Add(itemCidade);
                        }
                        i++;
                    }
                    return listaMunicipios.GroupBy(w => w.NmCidade).Select(s => s.First()).ToList();

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaMunicipio");



                    throw;
                }
            }
        }





        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasHome/{idUsuario}")]
        public OutPutListaPropostas ListaPropostasHome(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var usuario = new bUsuario(db).GetById(idUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    List<Proposta> propostas = new List<Proposta>();
                    if (perfilUsuario.IdPerfil == 3)
                    {
                        propostas = new bProposta(db).GetPropostasJuridicoSemContrato();
                    }
            
                    else
                    {
                        propostas = new bProposta(db).Get();
                    }

                    foreach (var itemProp in propostas)
                    {
                        if (itemProp.IdTipoOportunidade == 7)
                        {
                            retornoPropostas.TamanhoPropostasAditivos += 1;
                        }

                        if (itemProp.IdSituacao == 5)
                        {
                            retornoPropostas.TamanhoCoordenador += 1;
                        }
                        else if (itemProp.IdSituacao == 7)
                        {
                            retornoPropostas.TamanhoDiretoria += 1;
                        }
                        else if (itemProp.IdSituacao == 9)
                        {
                            retornoPropostas.TamanhoCliente += 1;
                        }
                        else if (itemProp.IdSituacao == 11 || itemProp.IdSituacao == 31 | itemProp.IdSituacao == 32 || itemProp.IdSituacao == 37 || itemProp.IdSituacao == 107 || itemProp.IdSituacao == 108)
                        {
                            var contrato = db.Contrato.Where(w => w.IdProposta == itemProp.IdProposta).FirstOrDefault();
                            //EGS 30.12.2020 Nao considerar aditivo em laboracao
                            //if (contrato == null || itemProp.IdSituacao == 99)
                            if (contrato == null)
                            {
                                retornoPropostas.TamanhoJuridico += 1;
                            }

                            if (contrato != null)
                            {
                                var equipeTec = db.ContratoEquipeTecnica.Where(w => w.IdContrato == contrato.IdContrato).FirstOrDefault();
                                if (equipeTec == null)
                                {
                                    retornoPropostas.TamanhoContratosDefinicaoEquipeTec += 1;
                                }
                            }

                        }
                        else if (itemProp.IdSituacao == 17)
                        {
                            retornoPropostas.TamanhoContratoFechado += 1;
                        }
                        else if (itemProp.IdSituacao == 4)
                        {
                            retornoPropostas.TamanhoEmElaboracao += 1;
                        }
                        else if (itemProp.IdSituacao == 26)
                        {
                            retornoPropostas.TamanhoAjustes += 1;
                        }
                        else if (itemProp.IdSituacao == 24)
                        {
                            retornoPropostas.TamanhoAprovadasCoord += 1;
                        }
                        else if (itemProp.IdSituacao == 10)
                        {
                            retornoPropostas.TamanhoAprovadasDiretoria += 1;
                        }
                    }
                    retornoPropostas.lstPropostas = listaPropostas;
                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasHome");



                    throw;
                }
            }
        }




        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostas/{idUsuario}")]
        public OutPutListaPropostas ListaPropostas(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>(); 
                    var Tamanho = listaPropostas.Count;
                    listaPropostas = new bPesquisaGeral(db).GetPropostas();
                    retornoPropostas.lstPropostas = listaPropostas;
                    return retornoPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostas");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasAditivo")]
        public OutPutListaPropostas ListaPropostasAditivo()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).Get();

                    foreach (var itemProp in propostas)
                    {
                        if (itemProp.IdTipoOportunidade == 7)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            listaPropostas.Add(itemProposta);
                        }

                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasAditivo");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasAditivoJuridico")]
        public OutPutListaPropostas ListaPropostasAditivoJuridico()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).Get();

                    foreach (var itemProp in propostas)
                    {
                        //EGS 30.12.2020 Nao deve constar "99-Elaborar Aditivo"
                      //if (itemProp.IdTipoOportunidade == 7 && itemProp.IdSituacao == 99)
                        if (itemProp.IdTipoOportunidade == 7)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            listaPropostas.Add(itemProposta);
                        }

                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasAditivoJuridico");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasComContrato")]
        public OutPutListaPropostas ListaPropostasComContrato()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).GetPropostasJuridico();

                    foreach (var itemProp in propostas)
                    {
                        var contrato = db.Contrato.Where(w => w.IdProposta == itemProp.IdProposta).FirstOrDefault();
                        if (contrato != null)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            listaPropostas.Add(itemProposta);
                        }

                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasComContrato");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasSemContrato")]
        public OutPutListaPropostas ListaPropostasSemContrato()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).GetPropostasJuridico();

                    foreach (var itemProp in propostas)
                    {
                        var contrato = db.Contrato.Where(w => w.IdProposta == itemProp.IdProposta).FirstOrDefault();
                        //EGS 30.12.2020 Nao deve constar "99-Elaborar Aditivo"
                      //if (contrato == null || itemProp.IdSituacao == 99)
                        if (contrato == null)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            listaPropostas.Add(itemProposta);
                        }

                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasSemContrato");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasNoJuridico")]
        public OutPutListaPropostas ListaPropostasNoJuridico()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).GetPropostasJuridico();

                    foreach (var itemProp in propostas)
                    {
                        var contrato = db.Contrato.Where(w => w.IdProposta == itemProp.IdProposta).FirstOrDefault();
                        if ((contrato == null && itemProp.IdSituacao != 100 || itemProp.IdSituacao != 37 && itemProp.IdSituacao != 100) || (itemProp.IdSituacao == 37 && itemProp.IcInformacoesIncompletas == true))
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            listaPropostas.Add(itemProposta);
                        }

                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasNoJuridico");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasInfoIncompleta")]
        public OutPutListaPropostas ListaPropostasInfoIncompleta()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).GetPropostasJuridicoInfoIncompleta();

                    foreach (var itemProp in propostas)
                    {
                        var itemProposta = new OutPutGetPropostas();
                        itemProposta.clientes = new List<string>();
                        itemProposta.coordenadores = new List<string>();
                        var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                        foreach (var propostaCli in propostaClientes)
                        {
                            esfera = new EsferaEmpresa();
                            var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                if (pessoaFisica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemProposta.clientes.Add(propostaCli.NmFantasia);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                if (pessoaJuridica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                                if (pessoaJuridica.IdEsferaEmpresa != null)
                                {
                                    esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                    itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                }
                                else
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                    {
                                        itemProposta.DsEsfera = "Privado";
                                    }
                                }
                            }
                        }

                        var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                        foreach (var propostaCoord in propostaCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                            itemProposta.coordenadores.Add(coordenador.NmPessoa);
                            itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }

                        var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                        itemProposta.IdProposta = itemProp.IdProposta;
                        itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                        itemProposta.DsAssunto = itemProp.DsAssunto;
                        itemProposta.DtProposta = itemProp.DtCriacao;
                        if (itemProp.DtLimiteEntregaProposta != null)
                        {
                            itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                        }
                        itemProposta.DsSituacao = situcao.DsSituacao;
                        itemProposta.VlProposta = itemProp.VlProposta;
                        listaPropostas.Add(itemProposta);
                    }

                    var Tamanho = listaPropostas.Count;
                    retornoPropostas.lstPropostas = listaPropostas;

                    return retornoPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasInfoIncompleta");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasPorSituacao/{idSituacao}/{idUsuario}")]
        public List<OutPutGetPropostas> ListaPropostasPorSituacao(int idSituacao, int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    List<Proposta> propostas = new List<Proposta>();
                    var usuario = new bUsuario(db).GetById(idUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    if (perfilUsuario.IdPerfil == 3)
                    {
                        propostas = new bProposta(db).GetByIdSituacaoJuridico(idSituacao);
                    }
                    else
                    {
                        propostas = new bProposta(db).GetByIdSituacao(idSituacao);
                    }

                    foreach (var itemProp in propostas)
                    {
                        var itemProposta = new OutPutGetPropostas();
                        itemProposta.clientes = new List<string>();
                        itemProposta.coordenadores = new List<string>();
                        var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                        foreach (var propostaCli in propostaClientes)
                        {
                            esfera = new EsferaEmpresa();
                            var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                if (pessoaFisica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemProposta.clientes.Add(propostaCli.NmFantasia);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                                if (pessoaJuridica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                                if (pessoaJuridica.IdEsferaEmpresa != null)
                                {
                                    esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                    itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                }
                                else
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                    {
                                        itemProposta.DsEsfera = "Privado";
                                    }
                                }
                            }
                        }

                        var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                        foreach (var propostaCoord in propostaCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                            itemProposta.coordenadores.Add(coordenador.NmPessoa);
                            itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }

                        var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                        itemProposta.IdProposta = itemProp.IdProposta;
                        itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                        itemProposta.DsAssunto = itemProp.DsAssunto;
                        itemProposta.DtProposta = itemProp.DtCriacao;
                        itemProposta.DtUltimaAlteracao = itemProp.DtUltimaAlteracao;
                        if (itemProp.DtLimiteEntregaProposta != null)
                        {
                            itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                        }
                        itemProposta.DsSituacao = situcao.DsSituacao;
                        itemProposta.VlProposta = itemProp.VlProposta;
                        listaPropostas.Add(itemProposta);

                    }

                    var Tamanho = listaPropostas.Count;

                    return listaPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasPorSituacao");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasJuridico")]
        public OutPutListaPropostasJuridico ListaPropostasJuridico()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaPropostasJuridico();
                    var lstPropostasJuridico = new bProposta(db).GetPropostasJuridico();

                    foreach (var prop in lstPropostasJuridico)
                    {
                        var retornoContrato = db.Contrato.Where(w => w.IdProposta == prop.IdProposta).FirstOrDefault();
                        if (retornoContrato != null)
                        {
                            if (retornoContrato.IcViaFipeNaoAssinada != null)
                            {
                                if (retornoContrato.IcViaFipeNaoAssinada.Value)
                                {
                                    retorno.tamanhoContratosFipeNaoAssinada += 1;
                                }
                            }
                        }
                        else if (retornoContrato == null || prop.IdSituacao == 99)
                        {
                            retorno.tamanhoJuridico += 1;
                        }

                        if (prop.IcInformacoesIncompletas != null)
                        {
                            if (prop.IcInformacoesIncompletas.Value)
                            {
                                retorno.tamanhoJuridicoInformacoesIncompletas += 1;
                            }
                        }

                        if (prop.IdSituacao == 31)
                        {
                            retorno.tamanhoJuridicoAssinadaFIPE += 1;
                        }
                        else if (prop.IdSituacao == 32)
                        {
                            retorno.tamanhoJuridicoAssinadaContratante += 1;
                        }
                        else if (prop.IdSituacao == 37)
                        {
                            retorno.tamanhoJuridicoAssinadaFipeContratante += 1;
                        }
                        else if (prop.IdSituacao == 99)
                        {
                            retorno.tamanhoJuridicoAditivo += 1;
                        }
                        else if (prop.IdSituacao == 107)
                        {
                            retorno.tamanhoJuridicoEnviadoContratante += 1;
                        }
                        else if (prop.IdSituacao == 11)
                        {
                            retorno.tamanhoJuridicoMinutaContrato += 1;
                        }
                        else if (prop.IdSituacao == 108)
                        {
                            retorno.tamanhoJuridicoAprovadaContratacao += 1;
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasJuridico");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasDiretoria")]
        public List<OutPutGetPropostas> ListaPropostasDiretoria()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    List<Proposta> propostas = new List<Proposta>();
                    propostas = new bProposta(db).GetPropostasDiretoria();

                    foreach (var itemProp in propostas)
                    {
                        var itemProposta = new OutPutGetPropostas();
                        itemProposta.clientes = new List<string>();
                        itemProposta.coordenadores = new List<string>();
                        var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                        foreach (var propostaCli in propostaClientes)
                        {
                            esfera = new EsferaEmpresa();
                            var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                if (pessoaFisica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemProposta.clientes.Add(pessoaJuridica.NmFantasia);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaJuridica.NmFantasia;
                                if (pessoaJuridica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                                if (pessoaJuridica.IdEsferaEmpresa != null)
                                {
                                    esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                    itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                }
                                else
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                    {
                                        itemProposta.DsEsfera = "Privado";
                                    }
                                }
                            }
                        }

                        var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                        foreach (var propostaCoord in propostaCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                            itemProposta.coordenadores.Add(coordenador.NmPessoa);
                            itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }

                        var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                        itemProposta.IdProposta = itemProp.IdProposta;
                        itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                        itemProposta.DsAssunto = itemProp.DsAssunto;
                        itemProposta.DtProposta = itemProp.DtCriacao;
                        if (itemProp.DtLimiteEntregaProposta != null)
                        {
                            itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                        }
                        itemProposta.DsSituacao = situcao.DsSituacao;
                        itemProposta.VlProposta = itemProp.VlProposta;
                        itemProposta.IdTipoOportunidade = itemProp.IdTipoOportunidade;
                        listaPropostas.Add(itemProposta);
                    }
                    var Tamanho = listaPropostas.Count;

                    return listaPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasDiretoria");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasAditivoDiretoria")]
        public List<OutPutGetPropostas> ListaPropostasAditivoDiretoria()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    List<Proposta> propostas = new List<Proposta>();
                    propostas = new bProposta(db).GetPropostasDiretoria();

                    foreach (var itemProp in propostas)
                    {
                        if (itemProp.IdTipoOportunidade == 7)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                            foreach (var propostaCli in propostaClientes)
                            {
                                esfera = new EsferaEmpresa();
                                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                    if (pessoaFisica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemProposta.clientes.Add(pessoaJuridica.NmFantasia);
                                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaJuridica.NmFantasia;
                                    if (pessoaJuridica.IdCidade != null)
                                    {
                                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                        itemProposta.NmCidade = cidade.NmCidade;
                                        itemProposta.UF = cidade.Uf;
                                    }
                                    if (pessoaJuridica.IdEsferaEmpresa != null)
                                    {
                                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                    }
                                    else
                                    {
                                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                        {
                                            itemProposta.DsEsfera = "Privado";
                                        }
                                    }
                                }
                            }

                            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                            foreach (var propostaCoord in propostaCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }

                            var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                            itemProposta.IdProposta = itemProp.IdProposta;
                            itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                            itemProposta.DsAssunto = itemProp.DsAssunto;
                            itemProposta.DtProposta = itemProp.DtCriacao;
                            if (itemProp.DtLimiteEntregaProposta != null)
                            {
                                itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                            }
                            itemProposta.DsSituacao = situcao.DsSituacao;
                            itemProposta.VlProposta = itemProp.VlProposta;
                            itemProposta.IdTipoOportunidade = itemProp.IdTipoOportunidade;
                            listaPropostas.Add(itemProposta);
                        }
                    }

                    var Tamanho = listaPropostas.Count;

                    return listaPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasAditivoDiretoria");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasPorDataLimite/{dtFiltro}")]
        public List<OutPutGetPropostas> ListaPropostasPorDataLimite(string dtFiltro)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();

                    var propostas = new bProposta(db).GetPropostasPorDataLimite(Convert.ToDateTime(dtFiltro));

                    foreach (var itemProp in propostas)
                    {
                        var itemProposta = new OutPutGetPropostas();
                        itemProposta.clientes = new List<string>();
                        itemProposta.coordenadores = new List<string>();
                        var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                        foreach (var propostaCli in propostaClientes)
                        {
                            esfera = new EsferaEmpresa();
                            var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                if (pessoaFisica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemProposta.clientes.Add(pessoaJuridica.RazaoSocial);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaJuridica.RazaoSocial;
                                if (pessoaJuridica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                                if (pessoaJuridica.IdEsferaEmpresa != null)
                                {
                                    esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                    itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                }
                                else
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                    {
                                        itemProposta.DsEsfera = "Privado";
                                    }
                                }
                            }
                        }

                        var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                        foreach (var propostaCoord in propostaCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                            itemProposta.coordenadores.Add(coordenador.NmPessoa);
                            itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }

                        var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                        itemProposta.IdProposta = itemProp.IdProposta;
                        itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                        itemProposta.DsAssunto = itemProp.DsAssunto;
                        itemProposta.DtProposta = itemProp.DtCriacao;
                        if (itemProp.DtLimiteEntregaProposta != null)
                        {
                            itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                        }
                        itemProposta.DsSituacao = situcao.DsSituacao;
                        itemProposta.VlProposta = itemProp.VlProposta;
                        listaPropostas.Add(itemProposta);

                    }

                    var Tamanho = listaPropostas.Count;

                    return listaPropostas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasPorDataLimite");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasPorDataValidade/{dtFiltro}")]
        public List<OutPutGetPropostas> ListaPropostasPorDataValidade(string dtFiltro)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var propostas = new bProposta(db).GetPropostasPorDataValidade(Convert.ToDateTime(dtFiltro));

                    foreach (var itemProp in propostas)
                    {
                        var itemProposta = new OutPutGetPropostas();
                        itemProposta.clientes = new List<string>();
                        itemProposta.coordenadores = new List<string>();
                        var propostaClientes = new bPropostaCliente(db).GetByProposta(itemProp.IdProposta);

                        foreach (var propostaCli in propostaClientes)
                        {
                            esfera = new EsferaEmpresa();
                            var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                                if (pessoaFisica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemProposta.clientes.Add(pessoaJuridica.RazaoSocial);
                                itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaJuridica.RazaoSocial;
                                if (pessoaJuridica.IdCidade != null)
                                {
                                    cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                                    itemProposta.NmCidade = cidade.NmCidade;
                                    itemProposta.UF = cidade.Uf;
                                }
                                if (pessoaJuridica.IdEsferaEmpresa != null)
                                {
                                    esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                                    itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                                }
                                else
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                                    {
                                        itemProposta.DsEsfera = "Privado";
                                    }
                                }
                            }
                        }

                        var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(itemProp.IdProposta);
                        foreach (var propostaCoord in propostaCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                            itemProposta.coordenadores.Add(coordenador.NmPessoa);
                            itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }

                        var situcao = db.Situacao.Where(s => s.IdSituacao == itemProp.IdSituacao).Single();

                        itemProposta.IdProposta = itemProp.IdProposta;
                        itemProposta.DsApelidoProposta = itemProp.DsApelidoProposta;
                        itemProposta.DsAssunto = itemProp.DsAssunto;
                        itemProposta.DtProposta = itemProp.DtCriacao;
                        if (itemProp.DtLimiteEntregaProposta != null)
                        {
                            itemProposta.DtLimiteEntregaProposta = itemProp.DtLimiteEntregaProposta.Value;
                        }
                        itemProposta.DsSituacao = situcao.DsSituacao;
                        itemProposta.VlProposta = itemProp.VlProposta;
                        listaPropostas.Add(itemProposta);

                    }
                    var Tamanho = listaPropostas.Count;

                    return listaPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasPorDataValidade");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasPorDatas/{dtFiltro}")]
        public OutPutGetPropostasPorDatas ListaPropostasPorDatas(string dtFiltro)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutGetPropostasPorDatas();

                    #region Por Data Limite de entrega
                    var propostasDataLimite = new bProposta(db).GetPropostasPorDataLimite(Convert.ToDateTime(dtFiltro));
                    var tamanhoDataLimite = propostasDataLimite.Count;
                    if (tamanhoDataLimite == 1)
                    {
                        retorno.IdPropostaDataLimite = propostasDataLimite[0].IdProposta;
                    }
                    #endregion

                    #region Por Data de Validade
                    var propostasDataValidade = new bProposta(db).GetPropostasPorDataValidade(Convert.ToDateTime(dtFiltro));
                    var tamanhoDataValidade = propostasDataValidade.Count;
                    if (tamanhoDataValidade == 1)
                    {
                        retorno.IdPropostaDataValidade = propostasDataValidade[0].IdProposta;
                    }
                    #endregion

                    retorno.TamanhoPropostaDataLimite = tamanhoDataLimite;
                    retorno.TamanhoPropostaDataValidade = tamanhoDataValidade;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasPorDatas");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaPropostasPorPeriodo/{idUsuario}")]
        public OutPutGetPropostasPorPeriodo ListaPropostasPorPeriodo(int idUsuario, string periodo)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutGetPropostasPorPeriodo();

                    var usuario = new bUsuario(db).GetById(idUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    List<Proposta> propostas = new List<Proposta>();
                    if (perfilUsuario.IdPerfil == 6)
                    {
                        propostas = new bProposta(db).GetPropostasDiretoria();
                    }
                    else if (perfilUsuario.IdPerfil == 3)
                    {
                        propostas = new bProposta(db).GetPropostasJuridico();
                    }
                    else if (perfilUsuario.IdPerfil == 4)
                    {
                        propostas = new bProposta(db).GetPropostasGestorContrato();
                    }
                    else
                    {
                        propostas = new bProposta(db).Get();
                    }

                    retorno.Tamanho = propostas.Count;

                    return retorno;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaPropostasPorPeriodo");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaResponsaveisGrid")]
        public List<OutputResponsavel> ListaResponsaveisGrid()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaResponsaveis = new List<OutputResponsavel>();
                    var lista = db.PropostaCoordenador.GroupBy(w => w.IdPessoa).Select(s => s.First()).ToList();

                    foreach (var item in lista)
                    {
                        var itemResponsavel = new OutputResponsavel();

                        var pessoa = db.Pessoa.Where(w => w.IdPessoaFisica == item.IdPessoa).FirstOrDefault();
                        itemResponsavel.NmPessoa = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica).FirstOrDefault().NmPessoa;

                        listaResponsaveis.Add(itemResponsavel);
                    }

                    return listaResponsaveis;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaResponsaveisGrid");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaFundamentos")]
        public List<OutPutFundamento> ListaFundamentos()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new List<OutPutFundamento>();
                    var listaFundamentos = db.FundamentoContratacao.OrderBy(w => w.DsFundamento).ToList();

                    foreach (var item in listaFundamentos)
                    {
                        var itemFundamento = new OutPutFundamento();
                        itemFundamento.IdFundamento = item.IdFundamento;
                        itemFundamento.DsFundamento = item.DsFundamento;

                        retorno.Add(itemFundamento);
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaFundamentos");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaTiposReajuste")]
        public List<OutPutTipoReajuste> ListaTiposReajuste()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new List<OutPutTipoReajuste>();
                    var listaTipoReajuste = db.IndiceReajuste.OrderBy(w => w.DsIndiceReajuste).ToList();

                    foreach (var item in listaTipoReajuste)
                    {
                        var itemTipoReajuste = new OutPutTipoReajuste();
                        itemTipoReajuste.IdIndiceReajuste = item.IdIndiceReajuste;
                        itemTipoReajuste.DsIndiceReajuste = item.DsIndiceReajuste;

                        retorno.Add(itemTipoReajuste);
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaTiposReajuste");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPut]
        [Route("Update")]
        public OutputUpdate Update([FromBody] InputUpdate item)
        {
            var retorno = new OutputUpdate();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Grava registro                    
                            int idProposta = new bProposta(db).Update(item);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + item.IdProposta + "] Usu.Cria [" + item.IdUsuarioCriacao + "] Últ.Usua [" + item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "] e sit [" + item.IdSituacao + "]");

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                            retorno.IdProposta = idProposta;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-Update [ID: "+item.IdProposta+"]");
                            retorno.Result = false;
                        }
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPut]
        [Route("UpdateJuridico")]
        public OutputUpdate UpdateJuridico([FromBody] InputUpdateJuridico item)
        {
            var retorno = new OutputUpdate();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Grava registro                    
                            new bProposta(db).UpdateJuridico(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "UpdateJuridico Proposta [" + item.IdProposta + "] finalizado  Últ.Usua [" + item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-UpdateJuridico [ID: " + item.IdProposta + "]");

                            retorno.Result = false;
                        }
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaCliente")]
        public List<OutputCliente> ListaCliente()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaClientes = new List<OutputCliente>();

                    var pessoasFisicas = new bPessoaFisica(db).BuscarPessoa();
                    var pessoasJuridicas = new bPessoaJuridica(db).BuscarPessoaJuridica();

                    foreach (var pJuridica in pessoasJuridicas)
                    {
                        var itemPesJuridica = new OutputCliente();
                        var pessoa = db.Pessoa.Where(p => p.IdPessoaJuridica == pJuridica.IdPessoaJuridica).FirstOrDefault();

                        itemPesJuridica.IdPessoa = pessoa.IdPessoa;
                        itemPesJuridica.NmCliente = pJuridica.NmFantasia;

                        listaClientes.Add(itemPesJuridica);
                    }

                    foreach (var pFisica in pessoasFisicas)
                    {
                        var itemPesFisica = new OutputCliente();
                        var pessoa = db.Pessoa.Where(p => p.IdPessoaFisica == pFisica.IdPessoaFisica).FirstOrDefault();
                        if (pessoa != null)
                        {
                            itemPesFisica.IdPessoa  = pessoa.IdPessoa;
                            itemPesFisica.NmCliente = pFisica.NmPessoa;
                            listaClientes.Add(itemPesFisica);
                        }
                    }

                    return listaClientes.OrderBy(w => w.NmCliente).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaCliente");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioGestorProposta/{id}/{proposta}")]
        public List<InputAddComentario> ListaComentarioGestorProposta(int id, int proposta)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bProposta(db).ListaComentarioGestorProposta(id, proposta);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdPropostaComentario = item.IdPropostaComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaComentarioGestorProposta");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioDiretoria/{id}/{proposta}")]
        public List<InputAddComentario> ListaComentarioDiretoria(int id, int proposta)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bProposta(db).ComentarioDiretoria(id, proposta);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdPropostaComentario = item.IdPropostaComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaComentarioDiretoria");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioJuridico/{id}/{proposta}")]
        public List<InputAddComentario> ListaComentarioJuridico(int id, int proposta)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bProposta(db).ComentarioJuridico(id, proposta);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdPropostaComentario = item.IdPropostaComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaComentarioJuridico");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaCoordenadores")]
        public List<OutputCoordenador> ListaCoordenadores()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaCoordenadores = new List<OutputCoordenador>();

                    var coordenadores = new bProposta(db).BuscarCoordenadores();

                    foreach (var itemCoord in coordenadores)
                    {

                        var itemCoordenador = new OutputCoordenador();

                        itemCoordenador.IdPessoaFisica = itemCoord.IdPessoaFisica;
                        itemCoordenador.NmPessoa = itemCoord.NmPessoa;

                        listaCoordenadores.Add(itemCoordenador);
                    }

                    return listaCoordenadores;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaCoordenadores");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaSituacaoArea/{area}")]
        public List<OutputSituacao> ListaSituacaoArea(string area)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaSituacoes = new List<OutputSituacao>();

                    var situacoes = new bProposta(db).BuscarSituacaoArea(area);

                    foreach (var itemSit in situacoes)
                    {

                        var itemSituacao = new OutputSituacao();

                        itemSituacao.IdSituacao = itemSit.IdSituacao;
                        itemSituacao.DsSituacao = itemSit.DsSituacao;

                        listaSituacoes.Add(itemSituacao);
                    }

                    return listaSituacoes;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaSituacaoArea");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaSituacao")]
        public List<OutputSituacao> ListaSituacao()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaSituacoes = new List<OutputSituacao>();

                    var situacoes = new bProposta(db).BuscarSituacao();

                    foreach (var itemSit in situacoes)
                    {

                        var itemSituacao = new OutputSituacao();

                        itemSituacao.IdSituacao = itemSit.IdSituacao;
                        itemSituacao.DsSituacao = itemSit.DsSituacao;
                        itemSituacao.Status = itemSit.DsSituacao;

                        listaSituacoes.Add(itemSituacao);
                    }

                    return listaSituacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaSituacao");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaSituacaoAditivo")]
        public List<OutputSituacao> ListaSituacaoAditivo()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaSituacoes = new List<OutputSituacao>();

                    var situacoes = new bProposta(db).BuscarSituacaoAditivo();

                    foreach (var itemSit in situacoes)
                    {

                        var itemSituacao = new OutputSituacao();

                        itemSituacao.IdSituacao = itemSit.IdSituacao;
                        itemSituacao.DsSituacao = itemSit.DsSituacao;
                        itemSituacao.Status = itemSit.DsSituacao;

                        listaSituacoes.Add(itemSituacao);
                    }

                    return listaSituacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaSituacaoAditivo");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaTiposAditivo")]
        public List<OutPutGetTiposAditivo> ListaTiposAditivo()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaTiposAditivo = new List<OutPutGetTiposAditivo>();

                    var tiposAditivo = new bProposta(db).BuscarTiposAditivo();

                    foreach (var tipoAditivo in tiposAditivo)
                    {

                        var itemTipoAditivo = new OutPutGetTiposAditivo();

                        itemTipoAditivo.IdTipoAditivo = tipoAditivo.IdTipoAditivo;
                        itemTipoAditivo.DsTipoAditivo = tipoAditivo.DsTipoAditivo;

                        listaTiposAditivo.Add(itemTipoAditivo);
                    }

                    return listaTiposAditivo;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaTiposAditivo");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("EncaminhaAditivoContrato")]
        public OutPutEncaminhaAditivoContrato EncaminhaAditivoContrato([FromBody]InputEncaminhaAditivoContrato item)
        {
            var retorno = new OutPutEncaminhaAditivoContrato();
            var contratoAditivo = new ContratoAditivo();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contratosAditivos = db.ContratoAditivo.Where(w => w.IdContrato == item.IdContrato).ToList();
                            var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();
                            var lstEntregavelTemporaria = new List<ContratoEntregavelTemporaria>();

                            var propostaOriginal = new bProposta(db).GetById(contrato.IdProposta);
                            if (item.VlContratoComAditivo != null)
                            {
                                propostaOriginal.VlContrato = item.VlContratoComAditivo;
                            }

                            var proposta                      = new bProposta(db).GetById(item.IdProposta);
                            proposta.IdSituacao               = 100;
                            proposta.DsAditivo                = item.DsAditivo;
                            proposta.DtNovoFimVigencia        = item.DtNovoFimVigencia;
                            proposta.IdTipoAditivo            = item.IdTipoAditivo;
                            proposta.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
                            proposta.VlContratoComAditivo     = item.VlContratoComAditivo;
                            proposta.VlAditivo                = item.VlAditivo;
                            proposta.IcAditivoData            = item.IcAditivoData;
                            proposta.IcAditivoEscopo          = item.IcAditivoEscopo;
                            proposta.IcAditivoValor           = item.IcAditivoValor;
                            var propostaHist                  = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                            propostaHist.DtFim                = DateTime.Now;

                            var newPropostaHist        = new PropostaHistorico();
                            newPropostaHist.IdProposta = proposta.IdProposta;
                            newPropostaHist.DtInicio   = DateTime.Now;
                            newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                            newPropostaHist.IdSituacao = proposta.IdSituacao;
                            db.PropostaHistorico.Add(newPropostaHist);
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "EncaminhaAditivoContrato Historico [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usu Info [" + (Int32)item.IdUsuarioCriacao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            contratoAditivo.IdTipoAditivo = item.IdTipoAditivo;
                            contratoAditivo.IdUsuarioCriacao = AppSettings.constGlobalUserID;
                            contratoAditivo.IdContrato = item.IdContrato;
                            contratoAditivo.VlContrato = contrato.VlContrato;
                            contratoAditivo.VlContratoAditivado = item.VlContratoComAditivo != null ? item.VlContratoComAditivo : contrato.VlContrato;
                            contratoAditivo.IdSituacao = 102;
                            contratoAditivo.DsAditivo = item.DsAditivo;
                            contratoAditivo.DtCriacao = DateTime.Now;
                            contratoAditivo.DtFimAditivada = item.DtNovoFimVigencia;
                            contratoAditivo.DtInicio = contrato.DtInicio.Value;
                            contratoAditivo.DtFim = contrato.DtFim.Value;
                            var nuAditivo = contratosAditivos.Count + 1;
                            contratoAditivo.NuAditivo = (short)nuAditivo;
                            contratoAditivo.IdProposta = proposta.IdProposta;
                            contratoAditivo.IcAditivoData = item.IcAditivoData;
                            contratoAditivo.IcAditivoEscopo = item.IcAditivoEscopo;
                            contratoAditivo.IcAditivoValor = item.IcAditivoValor;
                            contratoAditivo.VlAditivo = item.VlAditivo;

                            db.ContratoAditivo.Add(contratoAditivo);

                            var contratoDocPrincipal = new ContratoDocPrincipal();
                            var propostaDocAditivo = db.PropostaDocsPrincipais.Where(w => w.IdProposta == proposta.IdProposta && w.IdTipoDoc == 23).FirstOrDefault();

                            contratoDocPrincipal.IdTipoDoc = 23;
                            contratoDocPrincipal.IdContrato = item.IdContrato;
                            contratoDocPrincipal.NmCriador = propostaDocAditivo.NmCriador;
                            contratoDocPrincipal.DtUpLoad = DateTime.Now;
                            contratoDocPrincipal.DocFisico = propostaDocAditivo.DocFisico;
                            contratoDocPrincipal.NmDocumento = propostaDocAditivo.NmDocumento;
                            contratoDocPrincipal.DsDoc = propostaDocAditivo.DsDoc;
                            contratoDocPrincipal.DocFisicoId = Guid.NewGuid();

                            db.ContratoDocPrincipal.Add(contratoDocPrincipal);
                            db.SaveChanges();

                            var lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == item.IdContrato).OrderBy(w => w.IdContratoCronFinanceiro).ToList();
                            var lstContratoEntregavel = db.ContratoEntregavel.Where(w => w.IdContrato == item.IdContrato).OrderBy(w => w.IdContratoEntregavel).ToList();

                            foreach (var contratoEntregavel in lstContratoEntregavel)
                            {
                                var newHistoricoContratoEntregavel = new ContratoEntregavelHistórico();
                                newHistoricoContratoEntregavel.IdContratoAditivo = contratoAditivo.IdContratoAditivo;
                                newHistoricoContratoEntregavel.DsProduto = contratoEntregavel.DsProduto;
                                newHistoricoContratoEntregavel.DtProduto = contratoEntregavel.DtProduto;
                                newHistoricoContratoEntregavel.IdContratoCliente = contratoEntregavel.IdContratoCliente.Value;
                                newHistoricoContratoEntregavel.IdFrente = contratoEntregavel.IdFrente;
                                newHistoricoContratoEntregavel.IdSituacao = contratoEntregavel.IdSituacao;
                                //newHistoricoContratoEntregavel.NuEntregavel = contratoEntregavel.NuEntregavel;
                                newHistoricoContratoEntregavel.VlOrdem = contratoEntregavel.VlOrdem;

                                db.ContratoEntregavelHistórico.Add(newHistoricoContratoEntregavel);

                                var contratoEntregavelTemporaria = new ContratoEntregavelTemporaria();
                                contratoEntregavelTemporaria.IdContrato = contratoEntregavel.IdContrato;
                                contratoEntregavelTemporaria.DsProduto = contratoEntregavel.DsProduto;
                                contratoEntregavelTemporaria.DtProduto = contratoEntregavel.DtProduto;
                                contratoEntregavelTemporaria.IdContratoCliente = contratoEntregavel.IdContratoCliente.Value;
                                contratoEntregavelTemporaria.IdFrente = contratoEntregavel.IdFrente;
                                contratoEntregavelTemporaria.IdSituacao = contratoEntregavel.IdSituacao;
                                contratoEntregavelTemporaria.VlOrdem = contratoEntregavel.VlOrdem;
                                contratoEntregavelTemporaria.IcAtraso = contratoEntregavel.IcAtraso;
                                contratoEntregavelTemporaria.IdEntregavel = contratoEntregavel.IdContratoEntregavel;

                                db.ContratoEntregavelTemporaria.Add(contratoEntregavelTemporaria);
                                db.SaveChanges();

                                lstEntregavelTemporaria.Add(contratoEntregavelTemporaria);
                            }

                            foreach (var cronogramaFinanceiro in lstCronogramaFinanceiro)
                            {
                                var lstContratoParcelaEntregavel = db.ContratoParcelaEntregavel.Where(w => w.IdParcela == cronogramaFinanceiro.IdContratoCronFinanceiro).ToList();

                                var newHistoricoCronograma = new ContratoCronogramaFinanceiroHistorico();
                                if (lstContratoParcelaEntregavel.Count > 0)
                                {
                                    foreach (var parcEntregavel in lstContratoParcelaEntregavel)
                                    {
                                        var entregavel = db.ContratoEntregavel.Where(w => w.IdContratoEntregavel == parcEntregavel.IdEntregavel).FirstOrDefault();
                                        if (entregavel != null)
                                        {
                                            newHistoricoCronograma.NuEntregaveis += entregavel.VlOrdem + ", ";
                                        }
                                    }

                                    newHistoricoCronograma.NuEntregaveis = newHistoricoCronograma.NuEntregaveis.Substring(0, newHistoricoCronograma.NuEntregaveis.Length - 2);
                                }

                                newHistoricoCronograma.IdContratoAditivo = contratoAditivo.IdContratoAditivo;
                                newHistoricoCronograma.CdIss = cronogramaFinanceiro.CdIss;
                                newHistoricoCronograma.CdParcela = cronogramaFinanceiro.CdParcela;
                                newHistoricoCronograma.DsTextoCorpoNf = cronogramaFinanceiro.DsTextoCorpoNf;
                                newHistoricoCronograma.DtFaturamento = cronogramaFinanceiro.DtFaturamento;
                                newHistoricoCronograma.DtNotaFiscal = cronogramaFinanceiro.DtNotaFiscal;
                                newHistoricoCronograma.IdContratoCliente = cronogramaFinanceiro.IdContratoCliente;
                                newHistoricoCronograma.IdSituacao = cronogramaFinanceiro.IdSituacao;
                                newHistoricoCronograma.NuNotaFiscal = cronogramaFinanceiro.NuNotaFiscal;
                                newHistoricoCronograma.NuParcela = cronogramaFinanceiro.NuParcela;
                                newHistoricoCronograma.VlParcela = cronogramaFinanceiro.VlParcela;

                                db.ContratoCronogramaFinanceiroHistorico.Add(newHistoricoCronograma);

                                var contratoCronogramaFinanceiroTemporaria = new ContratoCronogramaFinanceiroTemporaria();
                                contratoCronogramaFinanceiroTemporaria.CdIss = cronogramaFinanceiro.CdIss;
                                contratoCronogramaFinanceiroTemporaria.CdParcela = cronogramaFinanceiro.CdParcela;
                                contratoCronogramaFinanceiroTemporaria.DsTextoCorpoNf = cronogramaFinanceiro.DsTextoCorpoNf;
                                contratoCronogramaFinanceiroTemporaria.DtFaturamento = cronogramaFinanceiro.DtFaturamento;
                                contratoCronogramaFinanceiroTemporaria.DtNotaFiscal = cronogramaFinanceiro.DtNotaFiscal;
                                contratoCronogramaFinanceiroTemporaria.IdContratoCliente = cronogramaFinanceiro.IdContratoCliente;
                                contratoCronogramaFinanceiroTemporaria.IdSituacao = cronogramaFinanceiro.IdSituacao;
                                contratoCronogramaFinanceiroTemporaria.NuNotaFiscal = cronogramaFinanceiro.NuNotaFiscal;
                                contratoCronogramaFinanceiroTemporaria.NuParcela = cronogramaFinanceiro.NuParcela;
                                contratoCronogramaFinanceiroTemporaria.VlParcela = cronogramaFinanceiro.VlParcela;
                                contratoCronogramaFinanceiroTemporaria.IcAtraso = cronogramaFinanceiro.IcAtraso;
                                contratoCronogramaFinanceiroTemporaria.IdContrato = cronogramaFinanceiro.IdContrato;
                                contratoCronogramaFinanceiroTemporaria.IdFrente = cronogramaFinanceiro.IdFrente;
                                contratoCronogramaFinanceiroTemporaria.IdParcela = cronogramaFinanceiro.IdContratoCronFinanceiro;

                                db.ContratoCronogramaFinanceiroTemporaria.Add(contratoCronogramaFinanceiroTemporaria);
                                db.SaveChanges();

                                var lstContratoParcEntre = db.ContratoParcelaEntregavel.Where(w => w.IdParcela == cronogramaFinanceiro.IdContratoCronFinanceiro).ToList();
                                foreach (var contratoParcelaEntregavel in lstContratoParcEntre)
                                {
                                    var contratoParcelaEntregavelTemporaria = new ContratoParcelaEntregavelTemporaria();

                                    // Copia o relacionamento de Entregavel e Parcela
                                    var entregavel = db.ContratoEntregavel.Where(w => w.IdContratoEntregavel == contratoParcelaEntregavel.IdEntregavel).FirstOrDefault();
                                    if (entregavel != null)
                                    {
                                        var entregavelTemporaria = lstEntregavelTemporaria.Where(w => w.IdEntregavel == entregavel.IdContratoEntregavel).FirstOrDefault();
                                        if (entregavelTemporaria != null)
                                        {
                                            contratoParcelaEntregavelTemporaria.IdEntregavel = entregavelTemporaria.IdContratoEntregavel;
                                            contratoParcelaEntregavelTemporaria.IdParcela = contratoCronogramaFinanceiroTemporaria.IdContratoCronFinanceiro;

                                            db.ContratoParcelaEntregavelTemporaria.Add(contratoParcelaEntregavelTemporaria);
                                        }
                                    }
                                }
                            }

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-EncaminhaAditivoContrato [" + item.IdContrato + "]");
                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("UploadDocAditivo/{IdContrato}/{NmCriador}")]
        public async Task<IActionResult> UploadDocAditivo(int IdContrato, string NmCriador)
        {
            using (var db = new FIPEContratosContext())
            {
                db.Database.SetCommandTimeout(800);
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var files = Request.Form.Files;

                            var itemDoc = new ContratoDocPrincipal();

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    itemDoc.IdTipoDoc = 23;
                                    itemDoc.IdContrato = IdContrato;
                                    itemDoc.NmCriador = NmCriador;
                                    itemDoc.DtUpLoad = DateTime.Now;
                                    itemDoc.DocFisico = fileBytes;
                                    itemDoc.NmDocumento = files[0].Name;
                                    itemDoc.DsDoc = files[0].Name;
                                }
                            }

                            db.ContratoDocPrincipal.Add(itemDoc);
                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-UploadDocAditivo");

                            throw ex;
                        }
                    }
                });
                return Ok();
            }
        }

        [HttpGet]
        [Route("GetComentarioById/{id}")]
        public OutPutUpDateComentario GetComentarioById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var comentario = new OutPutUpDateComentario();

                    var com = new bProposta(db).GetComentarioById(id);

                    comentario.IdPropostaComentario = com.IdPropostaComentario;
                    comentario.DsComentario         = com.DsComentario;
                    comentario.IdProposta           = com.IdProposta;
                    comentario.IdUsuario            = com.IdUsuario;
                    comentario.DtComentario         = com.DtComentario;

                    return comentario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-GetComentarioById");



                    throw;
                }
            }
        }

        [HttpPut]
        [Route("UpdateComentario")]
        public OutPutAddComentario UpdateComentario([FromBody] InputUpDateComentario item)
        {
            var retorno = new OutPutAddComentario();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var comentario                  = new PropostaComentario();
                            comentario.IdProposta           = item.IdProposta;
                            comentario.IdPropostaComentario = item.IdPropostaComentario;
                            comentario.IdUsuario            = item.IdUsuario;
                            comentario.DsComentario         = item.DsComentario;
                            comentario.DtComentario         = item.DtComentario;
                            var updateRetorno               = new bProposta(db).UpdateComentario(comentario);
                         // _GLog._GravaLog(AppSettings.constGlobalUserID, "UpdateComentario Historico [" + item.IdProposta + "] criada com comentario [" + item.DsComentario + "] Usu Info [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-UpdateComentario");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("AddComentario")]
        public OutPutAddComentario AddComentario([FromBody]InputAddComentario item)
        {
            var retorno = new OutPutAddComentario();
            var comentario = new PropostaComentario();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objComentario         = new bProposta(db);
                            comentario.DsComentario   = item.DsComentario;
                            comentario.DtComentario   = DateTime.Now;
                            comentario.IdUsuario      = item.IdUsuario;
                            comentario.IdProposta     = item.IdProposta;

                            objComentario.AddComentario(comentario);

                            var propostaComentarioLido = new PropostaComentarioLido();
                            propostaComentarioLido.IdPropostaComentario = comentario.IdPropostaComentario;
                            propostaComentarioLido.IdUsuario  = item.IdUsuario;

                            db.PropostaComentarioLido.Add(propostaComentarioLido);

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "AddComentario Proposta [" + item.IdProposta + "] Comentario [" + item.IdPropostaComentario + "] finalizado Usu Info [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AddComentario");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarios/{id}/{idUsuario}")]
        public List<OutPutComentario> ListaComentarios(int id, int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacomentario = new List<OutPutComentario>();

                    var comentarios = new bProposta(db).BuscarComentario(id);

                    foreach (var item in comentarios)
                    {
                        var listaComentarios = db.PropostaComentario.Where(e => e.IdPropostaComentario == item.IdPropostaComentario)
                            .ToList();

                        foreach (var itemCom in listaComentarios)
                        {
                            var comentario = new OutPutComentario();
                            comentario.IdPropostaComentario = itemCom.IdPropostaComentario;
                            comentario.DtComentario = itemCom.DtComentario.ToString();
                            comentario.DsComentario = itemCom.DsComentario;

                            var pComentarioLido = db.PropostaComentarioLido
                                .Where(c => c.IdPropostaComentario == itemCom.IdPropostaComentario && c.IdUsuario == idUsuario)
                                .FirstOrDefault();
                            if (pComentarioLido == null)
                            {
                                comentario.ComentarioLido = false;
                            }
                            else
                            {
                                comentario.ComentarioLido = true;
                            }
                            comentario.IdUsuario = itemCom.IdUsuario;
                            var usuario = new bUsuario(db).GetById(itemCom.IdUsuario);
                            if (usuario != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById(usuario.IdPessoa);
                                comentario.NmUsuario = pessoaFisica.NmPessoa;
                            }

                            listacomentario.Add(comentario);
                        }
                    }

                    return listacomentario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaComentarios");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("SalvaComentariosLidos")]
        public OutPutSalvaComentarios SalvaComentariosLidos([FromBody] InputSalvaComentarios item)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutSalvaComentarios();
                    retorno.ComentariosLidos = new List<int>();

                    var strategy = db.Database.CreateExecutionStrategy();
                    strategy.Execute(() =>
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var rangeFim = item.Pagina * 10;
                                var rangeInicio = rangeFim - 10;
                                var listaPropostaComentarios = db.PropostaComentario
                                    .Where(p => p.IdProposta == item.IdProposta)
                                    .OrderByDescending(o => o.DtComentario)
                                    .ToList();

                                if (listaPropostaComentarios.Count < rangeFim)
                                {
                                    var diferenca = rangeFim - listaPropostaComentarios.Count;
                                    rangeFim = 10 - diferenca;
                                    retorno.TotalComentariosLidos = rangeFim;

                                    var pComentarios = listaPropostaComentarios.GetRange(rangeInicio, rangeFim);

                                    foreach (var pComentario in pComentarios)
                                    {
                                        bool comentarioExiste = VerificaComentarioLidoExiste(item, pComentario);

                                        if (!comentarioExiste)
                                        {
                                            var pComentarioLido = new PropostaComentarioLido();
                                            pComentarioLido.IdPropostaComentario = pComentario.IdPropostaComentario;
                                            pComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                                            db.PropostaComentarioLido.Add(pComentarioLido);

                                            retorno.ComentariosLidos.Add(pComentario.IdPropostaComentario);
                                        }
                                    }
                                }
                                else
                                {
                                    retorno.TotalComentariosLidos = 10;

                                    var pComentarios = listaPropostaComentarios.GetRange(rangeInicio, 10);

                                    foreach (var pComentario in pComentarios)
                                    {
                                        bool comentarioexiste = VerificaComentarioLidoExiste(item, pComentario);

                                        if (!comentarioexiste)
                                        {
                                            var pComentarioLido = new PropostaComentarioLido();
                                            pComentarioLido.IdPropostaComentario = pComentario.IdPropostaComentario;
                                            pComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                                            db.PropostaComentarioLido.Add(pComentarioLido);

                                            retorno.ComentariosLidos.Add(pComentario.IdPropostaComentario);
                                        }
                                    }
                                }
                                retorno.Result = true;

                                db.SaveChanges();

                                db.Database.CommitTransaction();
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "Comentario da Proposta [" + item.IdProposta+ "] Pag [" + item.Pagina + "] Lido Usu.Cria [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
                            }
                            catch (Exception exx)
                            {
                                _GLog._GravaLog(AppSettings.constGlobalUserID, "Comentario da Proposta [" + item.IdProposta + "] Pag [" + item.Pagina + "] deu erro " + exx.Message + " " + exx.InnerException);
                                retorno.Result = false;
                                throw;
                            }
                        }
                    });
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-SalvaComentariosLidos");

                    throw;
                }
            }
        }

        private static bool VerificaComentarioLidoExiste(InputSalvaComentarios item, PropostaComentario pComentario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    bool comentarioExiste = false;

                    var pComentarioLidoExistente = db.PropostaComentarioLido
                                                    .Where(p => p.IdPropostaComentario == pComentario.IdPropostaComentario && p.IdUsuario == item.IdUsuario)
                                                    .FirstOrDefault();
                    if (pComentarioLidoExistente != null)
                    {
                        comentarioExiste = true;
                    }

                    return comentarioExiste;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-VerificaComentarioLidoExiste");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("VerificaQtdComentariosNaoLidos/{idProposta}/{idUsuario}")]
        public OutPutVerificaQtdComentariosNaoLidos VerificaQtdComentariosNaoLidos(int idProposta, int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaQtdComentariosNaoLidos();
                    retorno.ComentariosNaoLidos = new List<int>();

                    // Busca todos os Comentários da Proposta
                    var comentarios = new bProposta(db).BuscarComentario(idProposta);

                    // Varre a lista de Comentários e verifica se o Comentário já existe na tabela Proposta Comentários Lidos
                    // Caso não exista soma a váriavel TotalComentariosNaoLidos e o Id da Proposta Comentário na lista de ComentariosNao Lidos
                    foreach (var item in comentarios)
                    {
                        var pComentarioLido = db.PropostaComentarioLido
                            .Where(c => c.IdPropostaComentario == item.IdPropostaComentario && c.IdUsuario == idUsuario)
                            .FirstOrDefault();
                        if (pComentarioLido == null)
                        {
                            retorno.TotalComentariosNaoLidos++;
                            retorno.ComentariosNaoLidos.Add(item.IdPropostaComentario);
                        }
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-VerificaQtdComentariosNaoLidos");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaHistoricoSituacao/{id}")]
        public List<OutPutGetHstSituacao> ListaHistoricoSituacao(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listasituacao = new List<OutPutGetHstSituacao>();
                    var situacoes = new bProposta(db).BuscarSituacao(id);
                    int IdUsua = 0;
                    foreach (var item in situacoes)
                    {
                        var histSituacao = new OutPutGetHstSituacao();
                        var situacao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();
                        if (situacao != null)
                        {
                            histSituacao.Situacao   = situacao.DsSituacao;
                        }
                        histSituacao.DtInicio       = item.DtInicio.ToString();
                        histSituacao.dsEmailObserva = item.EmailObserva;

                        if (item.DtFim != null)
                        {
                            histSituacao.DtFim = item.DtFim.Value.ToString();
                        }
                        else
                        {
                            histSituacao.DtFim = string.Empty;
                        }

                        //EGS 30.09.2020 Quando nao encontrar usuario, pega o ultimo
                        if ((item.IdUsuario == 0) && (IdUsua != 0)) { item.IdUsuario = IdUsua; }

                        if (item.IdUsuario != 0) 
                        {
                            IdUsua      = item.IdUsuario;
                            var usuario = new bUsuario(db).GetById(item.IdUsuario);
                            if (usuario != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById(usuario.IdPessoa);
                                histSituacao.NmUsuario = pessoaFisica.NmPessoa;
                            }
                        }
                        listasituacao.Add(histSituacao);
                    }

                    return listasituacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaHistoricoSituacao ID [" + id + "]");
                    throw;
                }
            }

        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("ValidaClientes")]
        public OutPutValidaCliente ValidaClientes([FromBody] InputValidaCliente inputValidaCliente)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    bool clientesValidos  = true;
                    var retorno           = new OutPutValidaCliente();
                    var retornoClientes   = new List<OutPutClientesJuridico>();
                    var pessoasExistentes = new List<OutPutClientesJuridico>();
                    var propostaClientes  = new bPropostaCliente(db).GetByProposta(inputValidaCliente.IdProposta);

                    // Verifica se os Clientes já cadastrados na base foram informados pelo Juridíco no Contrato
                    foreach (var pCliente in propostaClientes)
                    {
                        var cliente     = new bCliente(db).GetById(pCliente.IdCliente);
                        var pessoa      = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                        var novoCliente = new OutPutClientesJuridico();

                        if (pessoa.IdPessoaFisica != null)
                        {
                            var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(pessoa.IdPessoaFisica.Value);
                            var clienteExiste = inputValidaCliente.Clientes.Where(w => w.CpfCnpj == pessoaFisica.NuCpf).FirstOrDefault();
                            if (clienteExiste == null)
                            {
                                clientesValidos = false;
                                novoCliente.CpfCnpj = pessoaFisica.NuCpf;
                                novoCliente.DsSituacao = "Consta na Proposta e não consta no Contrato";
                                retornoClientes.Add(novoCliente);
                            }
                            else
                            {
                                clienteExiste.DsSituacao = "Consta na Proposta e consta no Contrato";
                                retornoClientes.Add(clienteExiste);
                            }
                        }
                        else if (pessoa.IdPessoaJuridica != null)
                        {
                            var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);
                            var clienteExiste = inputValidaCliente.Clientes.Where(w => w.CpfCnpj == pessoaJuridica.Cnpj).FirstOrDefault();
                            if (clienteExiste == null)
                            {
                                //EGS 30.09.2020 Valida se é IDENTIF INTER
                                var clienteExisteInter = inputValidaCliente.Clientes.Where(w => w.CpfCnpj == pessoaJuridica.DsInternacional).FirstOrDefault();
                                if (clienteExisteInter == null)
                                {
                                    clientesValidos             = false;
                                    novoCliente.CpfCnpj         = pessoaJuridica.Cnpj;
                                    novoCliente.IdentificaInter = pessoaJuridica.DsInternacional;
                                    novoCliente.DsSituacao      = "Consta na Proposta e não consta no Contrato";
                                    retornoClientes.Add(novoCliente);
                                } else {
                                    clienteExisteInter.IdentificaInter = clienteExisteInter.CpfCnpj;
                                    clienteExisteInter.DsSituacao      = "Consta na Proposta e consta no Contrato";
                                    retornoClientes.Add(clienteExisteInter);
                                }
                            }
                            else
                                {
                                    clienteExiste.DsSituacao = "Consta na Proposta e consta no Contrato";
                                    retornoClientes.Add(clienteExiste);
                            }
                        }
                    }

                    // Verifica se os Clientes informados pelo Juridíco estão na Proposta
                    foreach (var cliente in inputValidaCliente.Clientes)
                    {
                        var clienteRetorno = retornoClientes.Where(w => w.CpfCnpj == cliente.CpfCnpj).FirstOrDefault();
                        if (clienteRetorno == null)
                        {
                            clientesValidos = false;
                            var novoCliente = new OutPutClientesJuridico();

                            novoCliente.CpfCnpj = cliente.CpfCnpj;
                            novoCliente.DsSituacao = "Não consta na Proposta e consta no Contrato";
                            retornoClientes.Add(novoCliente);
                        }
                    }

                    retorno.clientes = retornoClientes;
                    retorno.Result = clientesValidos;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ValidaClientes");



                    throw ex;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("PesquisaProposta")]
        public OutPutListaPropostas PesquisaProposta([FromBody] InputPesquisaProposta pesquisa)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoPropostas = new OutPutListaPropostas();
                    retornoPropostas.lstPropostas = new List<OutPutGetPropostas>();
                    var cidade = new Cidade();
                    var esfera = new EsferaEmpresa();
                    var usuario = new bUsuario(db).GetById(pesquisa.IdUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();

                    var sqlParameter = CriarParametroTexto("@Palavra", pesquisa.Palavra);
                    var dsResultado = ExecutarProcedureComRetorno("PR_FiltraGridPropostaByPalavra", pesquisa.Url, new List<System.Data.SqlClient.SqlParameter>() { sqlParameter });

                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        int idProposta = Convert.ToInt32(row["IdProposta"]);
                        if (retornoPropostas.lstPropostas.Where(w => w.IdProposta == idProposta).FirstOrDefault() == null)
                        {
                            var itemProposta = new OutPutGetPropostas();
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();
                            var propostaClientes = new bPropostaCliente(db).GetByProposta(Convert.ToInt32(row["IdProposta"]));
                            var proposta = new bProposta(db).GetById(Convert.ToInt32(row["IdProposta"]));
                            var situacao = db.Situacao.Where(s => s.IdSituacao == proposta.IdSituacao).Single();

                            if (perfilUsuario.IdPerfil == 3)
                            {
                                if (situacao.IdSituacao == 11 || situacao.IdSituacao == 31 || situacao.IdSituacao == 32)
                                {
                                    AddListProposta(db, proposta, situacao, retornoPropostas, ref cidade, ref esfera, row, itemProposta, propostaClientes);
                                }
                            }
                            else if (perfilUsuario.IdPerfil == 4)
                            {
                                if (situacao.IdSituacao == 16)
                                {
                                    AddListProposta(db, proposta, situacao, retornoPropostas, ref cidade, ref esfera, row, itemProposta, propostaClientes);
                                }
                            }
                            else
                            {
                                AddListProposta(db, proposta, situacao, retornoPropostas, ref cidade, ref esfera, row, itemProposta, propostaClientes);
                            }
                        }
                    }

                    retornoPropostas.lstPropostas.OrderBy(w => w.DtLimiteEntregaProposta).ThenBy(w => w.IdProposta);
                    return retornoPropostas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-PesquisaProposta");



                    throw;
                }
            }
        }

        private static void AddListProposta(FIPEContratosContext db, Proposta proposta, Situacao situacao, OutPutListaPropostas retornoPropostas, ref Cidade cidade, ref EsferaEmpresa esfera, DataRow row, OutPutGetPropostas itemProposta, List<PropostaCliente> propostaClientes)
        {
            foreach (var propostaCli in propostaClientes)
            {
                esfera = new EsferaEmpresa();
                var cliente = new bCliente(db).BuscarClienteId(propostaCli.IdCliente);
                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                if (pessoa.IdPessoaFisica != null)
                {
                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                    itemProposta.clientes.Add(pessoaFisica.NmPessoa);
                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + pessoaFisica.NmPessoa;
                    if (pessoaFisica.IdCidade != null)
                    {
                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).Single();
                        itemProposta.NmCidade = cidade.NmCidade;
                        itemProposta.UF = cidade.Uf;
                    }
                }
                else if (pessoa.IdPessoaJuridica != null)
                {
                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                    itemProposta.clientes.Add(propostaCli.NmFantasia);
                    itemProposta.clientesTexto = itemProposta.clientesTexto + " " + propostaCli.NmFantasia;
                    if (pessoaJuridica.IdCidade != null)
                    {
                        cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).Single();
                        itemProposta.NmCidade = cidade.NmCidade;
                        itemProposta.UF = cidade.Uf;
                    }
                    if (pessoaJuridica.IdEsferaEmpresa != null)
                    {
                        esfera = db.EsferaEmpresa.Where(e => e.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).Single();
                        itemProposta.DsEsfera = esfera.DsEsferaEmpresa;
                    }
                    else
                    {
                        if (pessoaJuridica.IdClassificacaoEmpresa == 2)
                        {
                            itemProposta.DsEsfera = "Privado";
                        }
                    }
                }
            }

            var propostaCoordenadores = new bPropostaCoordenador(db).GetByProposta(Convert.ToInt32(row["IdProposta"]));
            foreach (var propostaCoord in propostaCoordenadores)
            {
                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(propostaCoord.IdPessoa));
                itemProposta.coordenadores.Add(coordenador.NmPessoa);
                itemProposta.coordenadoresTexto = itemProposta.coordenadoresTexto + " " + coordenador.NmPessoa;
            }

            itemProposta.IdProposta = Convert.ToInt32(row["IdProposta"]);
            itemProposta.DsApelidoProposta = proposta.DsApelidoProposta;
            itemProposta.DsAssunto = proposta.DsAssunto;
            itemProposta.DtProposta = proposta.DtCriacao;
            if (proposta.DtLimiteEntregaProposta != null)
            {
                itemProposta.DtLimiteEntregaProposta = proposta.DtLimiteEntregaProposta.Value;
            }
            itemProposta.DsSituacao = situacao.DsSituacao;
            itemProposta.VlProposta = proposta.VlProposta;

            retornoPropostas.lstPropostas.Add(itemProposta);
        }

        public static DataSet ExecutarProcedureComRetorno(string nomeProcedure_, string url, List<SqlParameter> parametrosSQL_ = null)
        {
            try
            {
                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection conexao = new SqlConnection(stringConexaoPortal))
                {
                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandText = nomeProcedure_;

                        if (parametrosSQL_ != null)
                            comando.Parameters.AddRange(parametrosSQL_.ToArray());

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            sqlDataAdapter.SelectCommand = comando;

                            conexao.Open();
                            DataSet dsResultado = new DataSet();
                            sqlDataAdapter.Fill(dsResultado);
                            comando.Parameters.Clear();
                            conexao.Close();

                            return dsResultado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //_GLog(ex, "PropostaController-ExecutarProcedureComRetorno");
                throw;
            }
        }

        public static SqlParameter CriarParametroTexto(string nomeParametro, string texto_)
        {
            SqlParameter parametro = null;

            if (string.IsNullOrEmpty(texto_))
                parametro = new SqlParameter(nomeParametro, DBNull.Value);
            else
                parametro = new SqlParameter(nomeParametro, texto_);

            return parametro;
        }

        #region Métodos Documentos
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("UpLoadDoc/{IdTipoDocumento}/{IdProposta}/{NmCriador}")]
        public async Task<IActionResult> UpLoadDoc(short IdTipoDocumento, int IdProposta, string NmCriador)
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
                            var files = Request.Form.Files;

                            var itemDoc = new PropostaDocs();

                            var objDocumento = new bProposta(db);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    itemDoc.IdTipoDoc = IdTipoDocumento;
                                    itemDoc.IdProposta = IdProposta;
                                    itemDoc.NmCriador = NmCriador;
                                    itemDoc.DtUpLoad = DateTime.Now;
                                    itemDoc.DocFisico = fileBytes;
                                    itemDoc.NmDocumento = files[0].Name;
                                    itemDoc.DsDoc = files[0].Name;
                                }
                            }

                            objDocumento.AddDocumento(itemDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "UpLoadDoc Doc adicionado na Proposta [" + IdProposta + "] criador [" + NmCriador + "]");

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-UpLoadDoc ["+ IdProposta + "]");
                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaDocumentos/{id}")]
        public List<OutPutDocumento> ListaDocumentos(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaDocs = new List<OutPutDocumento>();

                    var documentos = new bProposta(db).BuscarDocumentos(id);

                    foreach (var itemDoc in documentos)
                    {
                        var itemDocumento = new OutPutDocumento();
                        var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDoc).Single();

                        itemDocumento.IdPropostaDocs = itemDoc.IdPropostaDocs;
                        itemDocumento.IdProposta = itemDoc.IdProposta;
                        itemDocumento.DsTipoDocumento = tipoDocumento.DsTipoDoc;
                        itemDocumento.DtUpLoad = itemDoc.DtUpLoad;
                        itemDocumento.NmCriador = itemDoc.NmCriador;
                        itemDocumento.NmDocumento = itemDoc.NmDocumento;
                        itemDocumento.DsDoc = itemDoc.DsDoc;
                        itemDocumento.TextDown = "Download";
                        if (itemDocumento.DsTipoDocumento == "Proposta Minuta")
                        {
                            itemDocumento.Minuta = true;
                        }

                        listaDocs.Add(itemDocumento);
                    }

                    return listaDocs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaDocumentos");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("DownloadDoc/{id}")]
        public async Task<IActionResult> DownloadDoc(int id)
        {
            using (var db = new FIPEContratosContext())
            {

                try
                {
                    var retornoDocumento = new bProposta(db).BuscarDocumentoId(id);

                    if (retornoDocumento != null && retornoDocumento.DocFisico != null)
                    {
                        var stream = new MemoryStream(retornoDocumento.DocFisico);

                        if (stream == null)
                            return NotFound();

                        return File(stream, "application/octet-stream", retornoDocumento.DsDoc);
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-DownloadDoc");
                    return NotFound();
                }

                return NotFound();
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpDelete]
        [Route("ExcluirDocumento/{id}")]
        public OutPutReturnDoc ExcluirDocumento(int id)
        {
            var retorno = new OutPutReturnDoc();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objDoc = new bProposta(db);

                            objDoc.RemoveDocumento(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ExcluirDocumento");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaTipoDocs/{id}")]
        public List<OutPutTipoDocumento> ListaTipoDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listatipoDocs = new List<OutPutTipoDocumento>();

                    var tipoDocumentos = new bProposta(db).BuscarTipoDocumentos(id);

                    foreach (var itemDoc in tipoDocumentos)
                    {
                        var itemTipoDocumento = new OutPutTipoDocumento();

                        itemTipoDocumento.IdTipoDoc = itemDoc.IdTipoDoc;
                        itemTipoDocumento.DsTipoDocumento = itemDoc.DsTipoDoc;
                        itemTipoDocumento.TipoDocumento = itemDoc.DsTipoDoc;

                        listatipoDocs.Add(itemTipoDocumento);
                    }

                    return listatipoDocs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaTipoDocs");
                    throw;
                }
            }
        }

        //Documentos principais
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("UpLoadDocPrincipal/{IdTipoDocumento}/{IdProposta}/{NmCriador}")]
        public async Task<IActionResult> UpLoadDocPrincipal(short IdTipoDocumento, int IdProposta, string NmCriador)
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
                            var files = Request.Form.Files;

                            var itemDoc = new PropostaDocsPrincipais();

                            var objDocumento = new bProposta(db);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    itemDoc.IdTipoDoc = IdTipoDocumento;
                                    itemDoc.IdProposta = IdProposta;
                                    itemDoc.NmCriador = NmCriador;
                                    itemDoc.DtUpLoad = DateTime.Now;
                                    itemDoc.DocFisico = fileBytes;
                                    itemDoc.NmDocumento = files[0].Name;
                                    itemDoc.DsDoc = files[0].Name;
                                    itemDoc.ParaAjustes = false;
                                }
                            }
                            if (IdTipoDocumento == 1)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);
                                    db.SaveChanges();
                                }
                            }
                            if (IdTipoDocumento == 3)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);
                                    db.SaveChanges();
                                }
                            }
                            if (IdTipoDocumento == 4)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);

                                    db.SaveChanges();
                                }
                            }
                            if (IdTipoDocumento == 7 || IdTipoDocumento == 16)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);
                                    db.SaveChanges();
                                }

                                var contrato = db.Contrato.Where(w => w.IdProposta == IdProposta).FirstOrDefault();
                                if (contrato != null)
                                {
                                    var newContratoDoc = new ContratoDocPrincipal();
                                    newContratoDoc.IdContrato = contrato.IdContrato;
                                    newContratoDoc.NmCriador = itemDoc.NmCriador;
                                    newContratoDoc.DtUpLoad = itemDoc.DtUpLoad;
                                    newContratoDoc.DocFisico = itemDoc.DocFisico;
                                    newContratoDoc.DsDoc = itemDoc.DsDoc;
                                    newContratoDoc.NmDocumento = itemDoc.DsDoc;
                                    newContratoDoc.IdTipoDoc = itemDoc.IdTipoDoc;

                                    db.ContratoDocPrincipal.Add(newContratoDoc);

                                    var oldContratoDoc = db.ContratoDocPrincipal.Where(w => w.IdTipoDoc == IdTipoDocumento && w.IdContrato == contrato.IdContrato).ToList();
                                    if (oldContratoDoc.Count > 0)
                                    {
                                        db.ContratoDocPrincipal.RemoveRange(oldContratoDoc);
                                    }

                                    db.SaveChanges();
                                }
                            }
                            if (IdTipoDocumento == 23)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);

                                    db.SaveChanges();
                                }
                            }
                            if (IdTipoDocumento == 27 || IdTipoDocumento == 28)
                            {
                                var oldDoc = db.PropostaDocsPrincipais.Where(w => w.IdTipoDoc == itemDoc.IdTipoDoc && w.IdProposta == IdProposta).ToList();
                                if (oldDoc.Count > 0)
                                {
                                    db.PropostaDocsPrincipais.RemoveRange(oldDoc);

                                    db.SaveChanges();
                                }
                            }

                            objDocumento.AddDocumentoPrincipal(itemDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-UpLoadDocPrincipal");
                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaDocumentosPrincipais/{id}")]
        public List<OutPutDocumentoPrincipal> ListaDocumentosPrincipais(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaDocs = new List<OutPutDocumentoPrincipal>();

                    var documentos = new bProposta(db).BuscarDocumentosPrincipais(id);

                    foreach (var itemDoc in documentos)
                    {
                        var itemDocumento = new OutPutDocumentoPrincipal();
                        var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDoc).Single();

                        itemDocumento.IdPropostaDocsPrincipais = itemDoc.IdPropostaDocsPrincipais;
                        itemDocumento.IdProposta = itemDoc.IdProposta;
                        itemDocumento.DsTipoDocumento = tipoDocumento.DsTipoDoc;
                        itemDocumento.DtUpLoad = itemDoc.DtUpLoad.ToShortDateString();
                        itemDocumento.NmCriador = itemDoc.NmCriador;
                        itemDocumento.NmDocumento = itemDoc.NmDocumento;
                        itemDocumento.TextDown = "Download";
                        if (itemDocumento.DsTipoDocumento == "Termo de Referência")
                        {
                            itemDocumento.Termo = true;
                        }
                        if (itemDocumento.DsTipoDocumento == "Proposta Final")
                        {
                            itemDocumento.PropostaFinal = true;
                        }
                        if (itemDocumento.DsTipoDocumento == "Proposta Minuta")
                        {
                            itemDocumento.PropostaMinuta = true;
                        }

                        listaDocs.Add(itemDocumento);
                    }

                    return listaDocs;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaDocumentosPrincipais");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaTermoDocs/{id}")]
        public OutPutBuscaNomeDocs BuscaTermoDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscarTermoId(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaTermoDocs");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaFinalDocs/{id}")]
        public OutPutBuscaNomeDocs BuscaFinalDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscarFinalId(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaFinalDocs");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaOrdemInicio/{id}")]
        public OutPutBuscaNomeDocs BuscaOrdemInicio(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscaOrdemInicio(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaOrdemInicio");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaAditivo/{id}")]
        public OutPutBuscaNomeDocs BuscaPropostaAditivo(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var histProp = new bProposta(db).GetPropostaHist(id);
                    if (histProp.Count >= 2)
                    {
                        if (histProp[1].IdSituacao == 5)
                        {
                            retorno.PerfilSolicitanteAjustes = "Coordenador";
                        }
                        else if (histProp[1].IdSituacao == 7)
                        {
                            retorno.PerfilSolicitanteAjustes = "Diretoria";
                        }
                        else if (histProp[1].IdSituacao == 9)
                        {
                            retorno.PerfilSolicitanteAjustes = "Cliente";
                        }
                    }
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscaPropostaAditivo(id);
                    if (item != null)
                    {
                        retorno.ParaAjustes = item.ParaAjustes;
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaAditivo");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaAditivoFinal/{id}")]
        public OutPutBuscaNomeDocs BuscaPropostaAditivoFinal(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscaPropostaAditivoFinal(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaAditivoFinal");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaContratoAssinado/{id}")]
        public OutPutBuscaNomeDocs BuscaContratoAssinado(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscaContratoAssinado(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaContratoAssinado");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaPropostaMinuta/{id}")]
        public OutPutBuscaNomeDocs BuscaPropostaMinuta(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var histProp = new bProposta(db).GetPropostaHist(id);
                    if (histProp.Count >= 2)
                    {
                        if (histProp[1].IdSituacao == 5)
                        {
                            retorno.PerfilSolicitanteAjustes = "Coordenador";
                        }
                        else if (histProp[1].IdSituacao == 7)
                        {
                            retorno.PerfilSolicitanteAjustes = "Diretoria";
                        }
                        else if (histProp[1].IdSituacao == 9)
                        {
                            retorno.PerfilSolicitanteAjustes = "Cliente";
                        }
                    }

                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscarMinutaId(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                        retorno.ParaAjustes = item.ParaAjustes;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaPropostaMinuta");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaDocumentoAditivo/{id}")]
        public OutPutBuscaNomeDocs BuscaDocumentoAditivo(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var proposta = new bProposta(db).GetById(id);
                    var item = new bPropostaDocs(db).BuscaDocumentoAditivo(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdPropostaDocsPrincipais = item.IdPropostaDocsPrincipais;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaDocumentoAditivo");



                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("DownloadDocPrincipal/{id}")]
        public async Task<IActionResult> DownloadDocPrincipal(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoDocumento = new bProposta(db).BuscarDocumentoPrincipalId(id);

                    if (retornoDocumento != null && retornoDocumento.DocFisico != null)
                    {
                        var stream = new MemoryStream(retornoDocumento.DocFisico);

                        if (stream == null)
                            return NotFound();

                        return File(stream, "application/octet-stream", retornoDocumento.DsDoc);
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-DownloadDocPrincipal");



                    return NotFound();
                }

                return NotFound();
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpDelete]
        [Route("ExcluirDocumentoPrincipal/{id}")]
        public OutPutReturnDoc ExcluirDocumentoPrincipal(int id)
        {
            var retorno = new OutPutReturnDoc();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objDoc = new bProposta(db);

                            objDoc.RemoveDocumentoPrincipal(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ExcluirDocumentoPrincipal");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [HttpDelete]
        [Route("CancelarProposta/{id}/{idUsuario}")]
        public OutPutCancelarProposta CancelarProposta(int id, int idUsuario)
        {
            var retorno = new OutPutCancelarProposta();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bProposta(db).CancelarProposta(id, idUsuario);
                         // _GLog._GravaLog(AppSettings.constGlobalUserID, "CancelarProposta [" + id + "] Usu.Cria [" + idUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-CancelarProposta");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }
        #endregion

        #region Métodos Contato
        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("AddContato")]
        public OutPutContato AddContato([FromBody]InputAddContato item)
        {
            var retorno = new OutPutContato();
            var contato = new PropostaContato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objContato = new bProposta(db);

                            contato.NmContato = item.NmContato;
                            contato.CdEmail = item.CdEmail;
                            contato.NuTelefone = item.NuTelefone;
                            contato.NuCelular = item.NuCelular;
                            contato.NmDepartamento = item.NmDepartamento;
                            contato.IdProposta = item.IdProposta;
                            contato.IdTipoContato = item.IdTipoContato;

                            objContato.AddContato(contato);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.IdPropostaContato = contato.IdPropostaContato;

                            retorno.Result = true;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AddContato");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPut]
        [Route("AtualizarContato")]
        public OutPutContato AtualizarContato([FromBody]InputAddContato item)
        {
            var retorno = new OutPutContato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objContato = new bProposta(db);
                            var itemContato = new PropostaContato();

                            itemContato.IdPropostaContato = item.IdPropostaContato.Value;
                            itemContato.NmContato = item.NmContato;
                            itemContato.NuCelular = item.NuCelular;
                            itemContato.NuTelefone = item.NuTelefone;
                            itemContato.NmDepartamento = item.NmDepartamento;
                            itemContato.CdEmail = item.CdEmail;
                            itemContato.IdTipoContato = item.IdTipoContato;

                            objContato.UpdateContato(itemContato);

                            db.Database.CommitTransaction();

                            retorno.Result = true;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-AtualizarContato");

                            retorno.Result = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaContato/{id}")]
        public List<InputAddContato> ListaContato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacontato = new List<InputAddContato>();

                    var contatos = new bProposta(db).BuscarContato(id);

                    foreach (var item in contatos)
                    {
                        var tipoContato = db.TipoContato.Where(w => w.IdTipoContato == item.IdTipoContato).FirstOrDefault();
                        var contato = new InputAddContato();
                        contato.IdPropostaContato = item.IdPropostaContato;
                        contato.NmContato = item.NmContato;
                        contato.CdEmail = item.CdEmail;
                        contato.NuTelefone = item.NuTelefone;
                        contato.NuCelular = item.NuCelular;
                        contato.IdProposta = item.IdProposta;
                        contato.NmDepartamento = item.NmDepartamento;
                        if (tipoContato != null)
                        {
                            contato.DsTipoContato = tipoContato.DsTipoContato;
                        }
                        listacontato.Add(contato);
                    }

                    return listacontato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-ListaContato");



                    throw;
                }

            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("RemoverContato/{id}")]
        public bool RemoverContato(int id)
        {
            bool contatoRemovido = true;

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            new bProposta(db).RemoverContato(id);
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-RemoverContato");

                            contatoRemovido = false;
                        }
                    }
                });

                return contatoRemovido;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("BuscaContatoId/{id}")]
        public InputAddContato BuscaContatoId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemContato = new InputAddContato();

                try
                {
                    var retornoContato = new bProposta(db).BuscarContatoId(id);

                    itemContato.IdPropostaContato = retornoContato.IdPropostaContato;
                    itemContato.NmContato = retornoContato.NmContato;
                    itemContato.CdEmail = retornoContato.CdEmail;
                    itemContato.NuCelular = retornoContato.NuCelular;
                    itemContato.NuTelefone = retornoContato.NuTelefone;
                    itemContato.NmDepartamento = retornoContato.NmDepartamento;
                    itemContato.IdTipoContato = retornoContato.IdTipoContato;

                    return itemContato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PropostaController-BuscaContatoId");



                    throw;
                }
            }
        }
        #endregion




        #endregion

            #region Retornos
        public class InputAddProposta
        {
            public int IdOportunidade { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
        }

        public class InputAddPropostaAditivo
        {
            public string NuContratoEdit { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
        }

        public class InputUpdateProposta
        {
            public int IdProposta { get; set; }
            public int? IdContrato { get; set; }
            public string NuContratoEdit { get; set; }
            public int IdSituacao { get; set; }
            public int? IdTipoProposta { get; set; }
            public int IdOportunidade { get; set; }
            public int? IdTema { get; set; }
            public string DsApelidoProposta { get; set; }
            public DateTime? DtProposta { get; set; }
            public DateTime? DtValidadeProposta { get; set; }
            public DateTime? DtLimiteEnvioProposta { get; set; }
            public DateTime? DtLimiteEntregaProposta { get; set; }
            public string DsAssunto { get; set; }
            public string DsObjeto { get; set; }
            public short? NuPrazoExecucao { get; set; }
            public decimal? VlProposta { get; set; }
            public decimal? NuPrazoEstimadoMes { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public DateTime DtCriacao { get; set; }
            public int? IdUsuarioUltimaAlteracao { get; set; }
            public DateTime? DtUltimaAlteracao { get; set; }
            public DateTime? DtAssinaturaContrato { get; set; }
            public DateTime? DtAutorizacaoInicio { get; set; }
            public List<OutputCliente> Clientes { get; set; }
            public List<OutputCoordenador> Coordenadores { get; set; }
            public List<int> CoordenadoresSelecionados { get; set; }
            public string NmUsuario { get; set; }
            public bool? OrdemInicio { get; set; }
            public bool? Reajustes { get; set; }
            public int? IdTipoReajuste { get; set; }
            public bool? RenovacaoAutomatica { get; set; }
            public decimal? VlContrato { get; set; }
            public int? IdFundamento { get; set; }
            public string DsObservacao { get; set; }
            public int IdTipoOportunidade { get; set; }
            public bool? IcRitoSumario { get; set; }
            public string DsAditivo { get; set; }
            public int? IdTipoAditivo { get; set; }
            public DateTime? DtNovoFimVigencia           { get; set; }
            public decimal?  VlContratoComAditivo        { get; set; }
            public bool?     IcContratantesValidos       { get; set; }
            public short?    IdUnidadeTempo              { get; set; }
            public short?    IdUnidadeTempoJuridico      { get; set; }
            public int?      NuPrazoExecucaoJuridico     { get; set; }
            public decimal?  NuPrazoEstimadoMesJuridico  { get; set; }
            public string    NuContratoCliente           { get; set; }
            public bool?     IcInformacoesIncompletas    { get; set; }
            public bool?     IcAditivoData               { get; set; }
            public bool?     IcAditivoEscopo             { get; set; }
            public bool?     IcAditivoValor              { get; set; }
            public bool?     IcAditivoRetRat             { get; set; }
            public decimal?  VlAditivo                   { get; set; }
            public string    PerfilSolicitanteAjuste     { get; set; }
            public DateTime? DtVigenciaAtual             { get; set; }
            public DateTime? DtVigenciaInicial           { get; set; }
            public bool?     IcAditivoAnalisado          { get; set; }
        }

        public class InputEncaminhaAditivoContrato
        {
            public short? IdTipoAditivo { get; set; }
            public DateTime? DtNovoFimVigencia { get; set; }
            public string DsAditivo { get; set; }
            public decimal? VlContratoComAditivo { get; set; }
            public int IdContrato { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public int IdProposta { get; set; }
            public bool? IcAditivoData { get; set; }
            public bool? IcAditivoEscopo { get; set; }
            public bool? IcAditivoValor { get; set; }
            public bool? IcAditivoRetRat { get; set; }
            public decimal? VlAditivo { get; set; }
        }

        public class OutPutEncaminhaAditivoContrato
        {
            public bool Result { get; set; }
        }

        public class OutPutGetPropostaByContrato
        {
            public InputUpdateProposta proposta { get; set; }
            public bool? ContratoNaoCadastrado { get; set; }
            public bool? ContratoNaoAtivo { get; set; }
            public bool? ContratoJaTemAditivoAtivo { get; set; }
        }

        //EGS 30.09.2020 Verifica se ja existe aditivo ativo
        public class OutPutGetContratoAditivoAtivo
        {
            public int    IdContrato                { get; set; }
            public string NuContratoAditivoAtivo    { get; set; }
            public string NuAditivoAtivo            { get; set; }
            public bool?  ContratoJaTemAditivoAtivo { get; set; }
        }

        public class OutPutGetTiposAditivo
        {
            public int IdTipoAditivo { get; set; }
            public string DsTipoAditivo { get; set; }
        }

        public class InputUpdateJuridico
        {
            public int IdProposta { get; set; }
            public int IdFundamento { get; set; }
            public decimal VlContrato { get; set; }
            public bool RenovacaoAutomatica { get; set; }
            public bool OrdemInicio { get; set; }
            public bool Reajustes { get; set; }
            public int? IdTipoReajuste { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
            public DateTime? DtAssinaturaContrato { get; set; }
            public int IdSituacao { get; set; }
            public string DsObjeto { get; set; }
            public bool? IcContratantesValidos { get; set; }
            public short? IdUnidadeTempoJuridico { get; set; }
            public short? NuPrazoExecucaoJuridico { get; set; }
            public decimal? NuPrazoEstimadoMesJuridico { get; set; }
            public string NuContratoCliente { get; set; }
            public bool? IcInformacoesIncompletas { get; set; }
            public short? IdTipoAditivo { get; set; }
            public DateTime? DtNovoFimVigencia { get; set; }
            public decimal? VlContratoComAditivo { get; set; }
            public bool? IcAditivoData { get; set; }
            public bool? IcAditivoEscopo { get; set; }
            public bool? IcAditivoValor { get; set; }
            public bool? IcAditivoRetRat { get; set; }
            public decimal? VlAditivo { get; set; }
            public string DsAditivo { get; set; }
            public bool? IcRitoSumario { get; set; }
        }

        public class InputEnviaEmailsResponsaveis
        {
            public int IdProposta { get; set; }
            public List<int> Coordenadores { get; set; }
            public string Url { get; set; }
        }

        public class OutPutCancelarProposta
        {
            public bool Result { get; set; }
        }
        public class OutputEnviaEmailsResponsaveis
        {
            public bool Result { get; set; }
        }

        public class OutPutEnviaEmailsContatos
        {
            public bool Result { get; set; }
        }

        public class OutPutValidaCliente
        {
            public List<OutPutClientesJuridico> clientes { get; set; }
            public bool Result { get; set; }
        }

        public class OutPutGetPropostasPorDatas
        {
            public int? IdPropostaDataLimite { get; set; }
            public int? IdPropostaDataValidade { get; set; }
            public int TamanhoPropostaDataLimite { get; set; }
            public int TamanhoPropostaDataValidade { get; set; }
        }

        public class InputValidaCliente
        {
            public List<OutPutClientesJuridico> Clientes { get; set; }
            public int IdProposta { get; set; }
        }

        public class OutPutClientesJuridico
        {
            public int    IdCliente        { get; set; }
            public string CpfCnpj          { get; set; }
            public string IdentificaInter  { get; set; }   //EGS 30.09.2020 Identif Intern
            public string DsSituacao       { get; set; }
        }

        public class InputEncaminharPropostaDiretoria
        {
            public int IdProposta { get; set; }
            public string Url { get; set; }
        }

        public class InputEnviaEmailsContatos
        {
            public int IdProposta { get; set; }
            public string Url { get; set; }
            public List<int> Contatos { get; set; }
            public bool EmailCliente { get; set; }
        }
        public class InputAprovaPropostaCoordenador
        {
            public string GuidPropostaCoordenador { get; set; }
            public string Url { get; set; }
        }

        public class OutputAprovaPropostaCoordenador
        {
            public bool Result { get; set; }
        }

        public class OutputResponsavel
        {
            public int IdPessoaFisica { get; set; }
            public string NmPessoa { get; set; }

        }
        public class InputSolicitaAjustePropostaCoordenador
        {
            public string GuidPropostaCoordenador { get; set; }
            public string Url { get; set; }
        }

        public class OutputSolicitaAjustePropostaCoordenador
        {
            public bool Result { get; set; }
        }

        public class InputAprovarPropostaDiretoria
        {
            public int IdProposta { get; set; }
            public string Url { get; set; }
            public int Idtema { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
        }

        public class OutputAprovarPropostaDiretoria
        {
            public bool Result { get; set; }
        }

        public class InputSolicitarAjustePropostaDiretoria
        {
            public int IdProposta { get; set; }
            public string Url { get; set; }
            public int IdTema { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
        }

        public class OutputSolicitarAjustePropostaDiretoria
        {
            public bool Result { get; set; }
        }
        public class OutputCliente
        {
            public int IdCliente { get; set; }
            public int IdPessoa { get; set; }
            public string NmCliente { get; set; }
        }

        public class OutPutBuscaNomeDocs
        {
            public string DsDoc { get; set; }
            public int IdPropostaDocsPrincipais { get; set; }
            public bool? ParaAjustes { get; set; }
            public string PerfilSolicitanteAjustes { get; set; }
        }

        public class OutputCoordenador
        {
            public int IdPessoaFisica { get; set; }
            public string NmPessoa { get; set; }
            public bool IcAprovado { get; set; }
            public bool? IcPropostaAprovada { get; set; }

        }

        public class OutputSituacao
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string Status { get; set; }

        }

        public class OutPutContato
        {
            public bool Result { get; set; }
            public int IdPropostaContato { get; set; }

        }

        public class OutPutListaPropostas
        {
            public List<OutPutGetPropostas> lstPropostas { get; set; }
            public int TamanhoCoordenador { get; set; }
            public int TamanhoDiretoria { get; set; }
            public int TamanhoCliente { get; set; }
            public int TamanhoJuridico { get; set; }
            public int TamanhoContratoFechado { get; set; }
            public int TamanhoEmElaboracao { get; set; }
            public int TamanhoAprovadasCoord { get; set; }
            public int TamanhoAprovadasDiretoria { get; set; }
            public int TamanhoAjustes { get; set; }
            public int TamanhoPropostasAditivos { get; set; }
            public int TamanhoContratosDefinicaoEquipeTec { get; set; }
        }
        public class OutPutGetPropostas
        {
            public int IdProposta { get; set; }
            public string DsSituacao { get; set; }
            public string DsTipoProposta { get; set; }
            public string DsApelidoProposta { get; set; }
            public DateTime DtProposta { get; set; }
            public DateTime? DtLimiteEntregaProposta { get; set; }
            public DateTime? DtUltimaAlteracao { get; set; }
            public string DsAssunto { get; set; }
            public List<string> clientes { get; set; }
            public string clientesTexto { get; set; }
            public List<string> coordenadores { get; set; }
            public string coordenadoresTexto { get; set; }
            public string UF { get; set; }
            public string DsEsfera { get; set; }
            public decimal? VlProposta { get; set; }
            public string NmCidade { get; set; }
            public int Tamanho { get; set; }
            public int? IdTipoOportunidade { get; set; }
        }

        public class InputPesquisaProposta
        {
            public int IdUsuario { get; set; }
            public string Palavra { get; set; }
            public string Url { get; set; }
        }

        public class OutPutGetPropostasPorPeriodo
        {
            public int Tamanho { get; set; }
        }
        public class InputAddContato
        {
            public string NmContato { get; set; }
            public string CdEmail { get; set; }
            public string NuTelefone { get; set; }
            public string NuCelular { get; set; }
            public string NmDepartamento { get; set; }
            public int? IdProposta { get; set; }
            public int? IdPropostaContato { get; set; }
            public int? IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }
        }

        // Documento

        public class OutPutDocumento
        {

            public int IdPropostaDocs { get; set; }
            public int IdProposta { get; set; }
            public string DsTipoDocumento { get; set; }
            public string NmDocumento { get; set; }
            public byte[] DocFisico { get; set; }
            public DateTime DtUpLoad { get; set; }
            public string NmCriador { get; set; }
            public string TextDown { get; set; }
            public bool Minuta { get; set; }
            public string DsDoc { get; set; }
        }

        public class OutPutDocumentoPrincipal
        {
            public int IdPropostaDocsPrincipais { get; set; }
            public int IdProposta { get; set; }
            public string DsTipoDocumento { get; set; }
            public string NmDocumento { get; set; }
            public byte[] DocFisico { get; set; }
            public string DtUpLoad { get; set; }
            public string NmCriador { get; set; }
            public string TextDown { get; set; }
            public bool Termo { get; set; }
            public bool PropostaFinal { get; set; }
            public bool PropostaMinuta { get; set; }
        }

        public class OutPutMunicipio
        {
            public int IdCidade { get; set; }
            public string NmCidade { get; set; }
        }

        public class OutPutTipoDocumento
        {

            public short IdTipoDoc { get; set; }
            public string DsTipoDocumento { get; set; }
            public string TipoDocumento { get; set; }

        }

        public class OutPutReturnDoc
        {

            public bool Result { get; set; }

        }

        public class OutPutSalvaComentarios
        {
            public bool Result { get; set; }
            public int TotalComentariosLidos { get; set; }
            public List<int> ComentariosLidos { get; set; }

        }

        public class InputAddComentario
        {
            public string DsComentario { get; set; }
            public DateTime DtComentario { get; set; }
            public int IdUsuario { get; set; }
            public int IdPropostaComentario { get; set; }
            public int IdProposta { get; set; }
            public string NmUsuario { get; set; }
        }

        public class OutPutVerificaQtdComentariosNaoLidos
        {
            public int TotalComentariosNaoLidos { get; set; }
            public List<int> ComentariosNaoLidos { get; set; }

        }

        public class InputSalvaComentarios
        {
            public int Pagina { get; set; }
            public int IdUsuario { get; set; }
            public int IdProposta { get; set; }
        }
        public class OutPutAddComentario
        {
            public bool Result { get; set; }
        }

        public class InputUpDateComentario
        {
            public string DsComentario { get; set; }
            public DateTime DtComentario { get; set; }
            public int IdUsuario { get; set; }
            public int IdPropostaComentario { get; set; }
            public int IdProposta { get; set; }
        }

        public class OutPutUpDateComentario
        {
            public string DsComentario { get; set; }
            public DateTime DtComentario { get; set; }
            public int IdUsuario { get; set; }
            public int IdPropostaComentario { get; set; }
            public int IdProposta { get; set; }
        }

        public class OutPutComentario
        {
            public string DsComentario { get; set; }
            public string DtComentario { get; set; }
            public int IdUsuario { get; set; }
            public int IdPropostaComentario { get; set; }
            public int IdProposta { get; set; }
            public string NmUsuario { get; set; }
            public bool ComentarioLido { get; set; }
        }

        public class OutPutGetHstSituacao
        {
            public string DtFim          { get; set; }
            public string DtInicio       { get; set; }
            public int    IdUsuario      { get; set; }
            public string Situacao       { get; set; }
            public int    IdProposta     { get; set; }
            public string NmUsuario      { get; set; }
            public string dsEmailObserva { get; set; }     //EGS 30.08.2020 Email ou Observacao da Proposta ou Contrato
        }

        public class OutPutFundamento
        {
            public int IdFundamento { get; set; }
            public string DsFundamento { get; set; }
        }

        public class OutPutTipoReajuste
        {
            public int IdIndiceReajuste { get; set; }
            public string DsIndiceReajuste { get; set; }
        }

        public class OutPutListaPropostasJuridico
        {
            public int tamanhoJuridico { get; set; }
            public int tamanhoJuridicoAssinadaFIPE { get; set; }
            public int tamanhoJuridicoAssinadaContratante { get; set; }
            public int tamanhoJuridicoAssinadaFipeContratante { get; set; }
            public int tamanhoJuridicoContratoGerado { get; set; }
            public int tamanhoJuridicoAditivo { get; set; }
            public int tamanhoContratosFipeNaoAssinada { get; set; }
            public int tamanhoJuridicoMinutaContrato { get; set; }
            public int tamanhoJuridicoAprovadaContratacao { get; set; }
            public int tamanhoJuridicoEnviadoContratante { get; set; }
            public int tamanhoJuridicoInformacoesIncompletas { get; set; }
        }
        #endregion
    }
}