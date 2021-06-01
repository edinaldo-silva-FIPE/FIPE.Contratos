using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.PropostaController;
using ApiFipe.Utilitario;


namespace ApiFipe.Models
{
    public class bProposta
    {
        GravaLog _GLog = new GravaLog();
        public FIPEContratosContext db { get; set; }

        public bProposta(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutputAdd Add(InputAddProposta item)
        {
            var retorno = new OutputAdd();
            var proposta = new Proposta();
            var oportunidade = new bOportunidade(db).BuscarOportunidadeId(item.IdOportunidade);
            oportunidade.IdSituacao = 3;

            var lastProposta = db.Proposta.OrderByDescending(w => w.IdProposta).FirstOrDefault();
            if (lastProposta != null)
            {
                proposta.IdProposta = lastProposta.IdProposta + 1;
            }
            else
            {
                proposta.IdProposta = 1;
            }
            proposta.IdOportunidade           = item.IdOportunidade;
            proposta.IdSituacao               = 4;
            proposta.IdUsuarioCriacao         = AppSettings.constGlobalUserID;
            proposta.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
            proposta.DsAssunto                = oportunidade.DsAssunto;
            proposta.DtCriacao                = DateTime.Now;
            proposta.DtUltimaAlteracao        = DateTime.Now;
            proposta.DtLimiteEntregaProposta  = oportunidade.DtLimiteEntregaProposta;

            db.Proposta.Add(proposta);

            var propostaHist        = new PropostaHistorico();
            propostaHist.IdSituacao = 4;
            propostaHist.DtInicio   = DateTime.Now;
            propostaHist.IdUsuario  = AppSettings.constGlobalUserID;
            propostaHist.IdProposta = proposta.IdProposta;
            db.PropostaHistorico.Add(propostaHist);
            _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico da Proposta [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usuario Info [" + item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

            db.SaveChanges();

            // Copia registro na tabela Proposta Contato com base na tabela Oportunidade Contato
            CopiaPropostaContato(proposta);

            // Copia registro na tabela Proposta Cliente com base na tabela Oportunidade Cliente
            CopiaPropostaCliente(proposta);

            // Copia registro na tabela Proposta Coordenador com base na tabela Oportunidade Responsavel
            CopiaPropostaCoordenador(proposta);

            // Copia registro na tabela Proposta Docs com base na tabela Oportunidade Docs
            CopiaPropostaDocs(proposta);

            retorno.Result = true;
            retorno.IdProposta = proposta.IdProposta.ToString();

            return retorno;
        }



        public OutputAddAditivo AddPropostaAditivo(InputAddPropostaAditivo item)
        {
            var retorno = new OutputAddAditivo();
            var proposta = new Proposta();
            List<PropostaContato> propostaAntigaContatos = new List<PropostaContato>();
            List<PropostaDocs> propostaAntigaDocumentos = new List<PropostaDocs>();
            List<PropostaDocsPrincipais> propostaAntigaDocumentosPrincipais = new List<PropostaDocsPrincipais>();
            List<PropostaCliente> propostaAntigaClientes = new List<PropostaCliente>();
            List<PropostaCoordenador> propostaAntigaCoordenadores = new List<PropostaCoordenador>();
            var contrato = db.Contrato.Where(w => w.NuContratoEdit == item.NuContratoEdit).FirstOrDefault();
            if (contrato != null)
            {
                var propostaOriginal = db.Proposta.Where(w => w.IdProposta == contrato.IdProposta).FirstOrDefault();
                if (propostaOriginal != null)
                {
                    var lastProposta = db.Proposta.OrderByDescending(w => w.IdProposta).FirstOrDefault();
                    if (lastProposta != null)
                    {
                        proposta.IdProposta = lastProposta.IdProposta + 1;
                    }
                    else
                    {
                        proposta.IdProposta = 1;
                    }                    
                    propostaAntigaContatos             = db.PropostaContato.Where(w => w.IdProposta == propostaOriginal.IdProposta).ToList();
                    propostaAntigaDocumentos           = db.PropostaDocs.Where(w => w.IdProposta == propostaOriginal.IdProposta).ToList();
                    propostaAntigaDocumentosPrincipais = db.PropostaDocsPrincipais.Where(w => w.IdProposta == propostaOriginal.IdProposta).ToList();
                    propostaAntigaClientes             = db.PropostaCliente.Where(w => w.IdProposta == propostaOriginal.IdProposta).ToList();
                    propostaAntigaCoordenadores        = db.PropostaCoordenador.Where(w => w.IdProposta == propostaOriginal.IdProposta).ToList();
                    if (propostaOriginal.IdFundamento != null)
                    {
                        proposta.IdFundamento = contrato.IdFundamento;
                    }
                    if (propostaOriginal.IdTipoReajuste != null)
                    {
                        proposta.IdTipoReajuste = contrato.IdIndiceReajuste;
                    }
                    proposta.OrdemInicio                = contrato.IcOrdemInicio;
                    proposta.RenovacaoAutomatica        = contrato.IcRenovacaoAutomatica;
                    proposta.Reajustes                  = contrato.IcReajuste;                    
                    proposta.VlContrato                 = contrato.VlContrato;
                    proposta.DtAssinaturaContrato       = contrato.DtAssinatura;
                    proposta.DsObjeto                   = contrato.DsObjeto;
                    proposta.NuPrazoExecucaoJuridico    = propostaOriginal.NuPrazoExecucaoJuridico;
                    proposta.IdUnidadeTempoJuridico     = propostaOriginal.IdUnidadeTempoJuridico;
                    proposta.NuPrazoEstimadoMesJuridico = propostaOriginal.NuPrazoEstimadoMesJuridico;
                    proposta.NuContratoCliente          = contrato.NuContratoCliente;
                    proposta.IcContratantesValidos      = propostaOriginal.IcContratantesValidos;
                    proposta.IdTema                     = contrato.IdTema;
                    proposta.DsApelidoProposta          = contrato.DsApelido;                    
                    proposta.NuPrazoExecucao            = propostaOriginal.NuPrazoExecucao;
                    proposta.DtValidadeProposta         = propostaOriginal.DtValidadeProposta;
                    proposta.VlProposta                 = propostaOriginal.VlProposta;
                    proposta.DsObservacao               = propostaOriginal.DsObservacao;
                    proposta.IdTipoOportunidade         = 7;                    
                    proposta.IcRitoSumario              = true;
                    proposta.IdContrato                 = contrato.IdContrato;
                    proposta.IdUnidadeTempo             = propostaOriginal.IdUnidadeTempo;
                    proposta.NuPrazoEstimadoMes         = propostaOriginal.NuPrazoEstimadoMes;
                    proposta.IdSituacao                 = 99;
                    proposta.IdUsuarioCriacao           = AppSettings.constGlobalUserID;
                    proposta.IdUsuarioUltimaAlteracao   = AppSettings.constGlobalUserID;
                    proposta.DsAssunto                  = propostaOriginal.DsAssunto;
                    proposta.DtCriacao                  = DateTime.Now;
                    proposta.DtUltimaAlteracao          = DateTime.Now;
                    proposta.DtLimiteEntregaProposta    = propostaOriginal.DtLimiteEntregaProposta;
                    db.Proposta.Add(proposta);

                    //EGS 30.03.2021 Valida Historico da Proposta
                    new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                    var propostaHist        = new PropostaHistorico();
                    propostaHist.IdSituacao = 99;
                    propostaHist.DtInicio   = DateTime.Now;
                    propostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                    propostaHist.IdProposta = proposta.IdProposta;
                    db.PropostaHistorico.Add(propostaHist);
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico da Proposta [" + proposta.IdProposta + "] criada com situacao [" + proposta.IdSituacao + "] Usuario Info [" + item.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                    db.SaveChanges();

                    if (propostaAntigaContatos.Count > 0)
                    {
                        foreach (var contato in propostaAntigaContatos)
                        {
                            var propostaContato            = new PropostaContato();
                            propostaContato.IdProposta     = proposta.IdProposta;
                            propostaContato.IdTipoContato  = contato.IdTipoContato;
                            propostaContato.NmContato      = contato.NmContato;
                            propostaContato.NmDepartamento = contato.NmDepartamento;
                            propostaContato.NuCelular      = contato.NuCelular;
                            propostaContato.NuTelefone     = contato.NuTelefone;
                            db.PropostaContato.Add(propostaContato);
                        }
                    }
                    if (propostaAntigaDocumentosPrincipais.Count > 0)
                    {
                        foreach (var docPrincipal in propostaAntigaDocumentosPrincipais)
                        {
                            var propostaDocPrincipal         = new PropostaDocsPrincipais();
                            propostaDocPrincipal.DocFisico   = docPrincipal.DocFisico;
                            propostaDocPrincipal.DocFisicoId = Guid.NewGuid();
                            propostaDocPrincipal.DsDoc       = docPrincipal.DsDoc;
                            propostaDocPrincipal.DtUpLoad    = docPrincipal.DtUpLoad;
                            propostaDocPrincipal.IdTipoDoc   = docPrincipal.IdTipoDoc;
                            propostaDocPrincipal.NmDocumento = docPrincipal.NmDocumento;
                            propostaDocPrincipal.IdProposta  = proposta.IdProposta;
                            propostaDocPrincipal.NmCriador   = docPrincipal.NmCriador;
                            db.PropostaDocsPrincipais.Add(propostaDocPrincipal);
                        }
                    }
                    if (propostaAntigaDocumentos.Count > 0)
                    {
                        foreach (var documento in propostaAntigaDocumentos)
                        {
                            var propostaDoc = new PropostaDocs();
                            propostaDoc.DocFisico = documento.DocFisico;
                            propostaDoc.DocFisicoId = Guid.NewGuid();
                            propostaDoc.DsDoc = documento.DsDoc;
                            propostaDoc.DtUpLoad = documento.DtUpLoad;
                            propostaDoc.IdProposta = proposta.IdProposta;
                            propostaDoc.IdTipoDoc = documento.IdTipoDoc;
                            propostaDoc.NmCriador = documento.NmCriador;
                            propostaDoc.NmDocumento = documento.NmDocumento;

                            db.PropostaDocs.Add(propostaDoc);
                        }
                    }
                    if (propostaAntigaClientes.Count > 0)
                    {
                        foreach (var cliente in propostaAntigaClientes)
                        {
                            var propostaCliente = new PropostaCliente();
                            propostaCliente.IdCliente = cliente.IdCliente;
                            propostaCliente.IdProposta = proposta.IdProposta;
                            propostaCliente.NmFantasia = cliente.NmFantasia;
                            propostaCliente.RazaoSocial = cliente.RazaoSocial;

                            db.PropostaCliente.Add(propostaCliente);
                        }
                    }
                    if (propostaAntigaCoordenadores.Count > 0)
                    {
                        foreach (var propostaCoord in propostaAntigaCoordenadores)
                        {
                            var propostaCoordenador = new PropostaCoordenador();
                            propostaCoordenador.IdProposta = proposta.IdProposta;
                            propostaCoordenador.IdPessoa = propostaCoord.IdPessoa;
                            propostaCoordenador.IcAprovado = propostaCoordenador.IcAprovado;
                            propostaCoordenador.IcAnaliseLiberada = propostaCoordenador.IcAnaliseLiberada;
                            propostaCoordenador.GuidPropostaCoordenador = Guid.NewGuid();

                            db.PropostaCoordenador.Add(propostaCoordenador);
                        }
                    }
                }

                db.SaveChanges();

                retorno.Result = true;
                retorno.IdProposta = proposta.IdProposta;
            }
            return retorno;
        }

        public void UpdateSituacao(int idProposta, int situacao)
        {
            var proposta = db.Proposta.Where(w => w.IdProposta == idProposta).FirstOrDefault();
            proposta.IdSituacao = situacao;

            _GLog._GravaLog(AppSettings.constGlobalUserID, "UpdateSituacao Proposta [" + idProposta + "] com Old.situacao [" + proposta.IdSituacao + "] para NEW.Situacao [" + situacao + "]");

            db.SaveChanges();
        }

        public Proposta BuscarPropostaId(int id)
        {
            var item = db.Proposta
                .Include(i => i.PropostaCliente)
                .Include(i => i.PropostaCoordenador)
                .Include(i => i.PropostaDocs)
                .Where(w => w.IdProposta == id).FirstOrDefault();

            return item;
        }







        public int Update(InputUpdate item)
        {
            var proposta = new Proposta();
            List<PropostaContato> propostaAntigaContatos = new List<PropostaContato>();
            List<PropostaDocs> propostaAntigaDocumentos = new List<PropostaDocs>();
            List<PropostaDocsPrincipais> propostaAntigaDocumentosPrincipais = new List<PropostaDocsPrincipais>();
            bool situacaoNova = false;

            try
            {
                if (item.IdProposta != 0)
                {
                    proposta = new bProposta(db).GetById(item.IdProposta);
                    if ((proposta.IdSituacao != item.IdSituacao) && (item.IdSituacao != 5))
                    {
                        //Update Proposta[1798] com Old.situacao[9] para NEW.Situacao[10]
                        if ((proposta.IdSituacao == 9) && (item.IdSituacao == 10))
                        {
                            item.IdSituacao = 9;   //EGS 10.03.2021 Qdo Luciene atualiza a Proposta, ela não tem permissão para movimentar para Situacao 10
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + proposta.IdProposta + "] com Old.situacao [" + proposta.IdSituacao + "] para NEW.Situacao [" + item.IdSituacao + "] cancelado.. Voltando a Situacao 9");
                        } else {
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + proposta.IdProposta + "] com Old.situacao [" + proposta.IdSituacao + "] para NEW.Situacao [" + item.IdSituacao + "]");
                            situacaoNova = true;
                        }
                    }
                }
                else
                {
                    proposta                  = new Proposta();
                    proposta.IdUsuarioCriacao = AppSettings.constGlobalUserID;
                    situacaoNova              = true;
                    if (item.IdContrato != null)
                    {
                        var contrato                       = new bContrato(db).GetContratoById(item.IdContrato.Value);
                        var propostaAntiga                 = new bProposta(db).GetById(contrato.IdProposta);
                        propostaAntigaContatos             = db.PropostaContato.Where(w => w.IdProposta == propostaAntiga.IdProposta).ToList();
                        propostaAntigaDocumentos           = db.PropostaDocs.Where(w => w.IdProposta == propostaAntiga.IdProposta).ToList();
                        propostaAntigaDocumentosPrincipais = db.PropostaDocsPrincipais.Where(w => w.IdProposta == propostaAntiga.IdProposta).ToList();
                        if (propostaAntiga.IdFundamento != null)
                        {
                            proposta.IdFundamento = (short)propostaAntiga.IdFundamento;
                        }
                        if (propostaAntiga.IdTipoReajuste != null)
                        {
                            proposta.IdTipoReajuste = (short)propostaAntiga.IdTipoReajuste;
                        }
                        proposta.OrdemInicio             = propostaAntiga.OrdemInicio;
                        proposta.RenovacaoAutomatica     = propostaAntiga.RenovacaoAutomatica;
                        proposta.Reajustes               = propostaAntiga.Reajustes;
                        proposta.IdTipoReajuste          = propostaAntiga.IdTipoReajuste;
                        proposta.VlContrato              = propostaAntiga.VlContrato;
                        proposta.DtAssinaturaContrato    = propostaAntiga.DtAssinaturaContrato;
                        proposta.DsObjeto                = propostaAntiga.DsObjeto;
                        proposta.NuPrazoExecucaoJuridico = propostaAntiga.NuPrazoExecucaoJuridico;
                        proposta.IdUnidadeTempoJuridico  = propostaAntiga.IdUnidadeTempoJuridico;
                        proposta.NuPrazoEstimadoMesJuridico = propostaAntiga.NuPrazoEstimadoMesJuridico;
                        proposta.NuContratoCliente       = propostaAntiga.NuContratoCliente;
                        proposta.IcContratantesValidos   = propostaAntiga.IcContratantesValidos;
                    }
                }

                if (proposta.IdSituacao == 9 && item.IdSituacao == 99)
                {
                    proposta.IcAditivoAnalisado = true;
                }
                proposta.IdSituacao               = item.IdSituacao;
                proposta.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
                proposta.IdTema                   = item.IdTema;
                proposta.DsApelidoProposta        = item.DsApelidoProposta;
                proposta.DsAssunto                = item.DsAssunto;
                proposta.DsObjeto                 = item.DsObjeto;
                proposta.NuPrazoExecucao          = item.NuPrazoExecucao;
                proposta.DtUltimaAlteracao        = DateTime.Now;
                proposta.DtValidadeProposta       = item.DtValidadeProposta;
                proposta.VlProposta               = item.VlProposta;
                proposta.DtLimiteEntregaProposta  = item.DtLimiteEntregaProposta;
                proposta.DtUltimaAlteracao        = DateTime.Now;
                proposta.DsObservacao             = item.DsObservacao;
                proposta.IdTipoOportunidade       = item.IdTipoOportunidade;
                proposta.DsAditivo                = item.DsAditivo;
                proposta.IcRitoSumario            = item.IcRitoSumario;
                proposta.IdContrato               = item.IdContrato;
                proposta.IdUnidadeTempo           = item.IdUnidadeTempo;
                proposta.NuPrazoEstimadoMes       = item.NuPrazoEstimadoMes;

                // Cria uma nova Proposta , caso a Proposta não tenha Oportunidade
                if (proposta.IdProposta == 0)
                {
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + proposta.IdProposta + "] Cria uma nova Proposta , caso a Proposta não tenha Oportunidade");

                    var lastProposta = db.Proposta.OrderByDescending(w => w.IdProposta).FirstOrDefault();
                    if (lastProposta != null)
                    {
                        proposta.IdProposta = lastProposta.IdProposta + 1;
                    }
                    if (propostaAntigaContatos.Count > 0)
                    {
                        foreach (var contato in propostaAntigaContatos)
                        {
                            var propostaContato            = new PropostaContato();
                            propostaContato.IdProposta     = proposta.IdProposta;
                            propostaContato.IdTipoContato  = contato.IdTipoContato;
                            propostaContato.NmContato      = contato.NmContato;
                            propostaContato.NmDepartamento = contato.NmDepartamento;
                            propostaContato.NuCelular      = contato.NuCelular;
                            propostaContato.NuTelefone     = contato.NuTelefone;
                            db.PropostaContato.Add(propostaContato);
                        }
                    }
                    if (propostaAntigaDocumentosPrincipais.Count > 0)
                    {
                        foreach (var docPrincipal in propostaAntigaDocumentosPrincipais)
                        {
                            var propostaDocPrincipal         = new PropostaDocsPrincipais();
                            propostaDocPrincipal.DocFisico   = docPrincipal.DocFisico;
                            propostaDocPrincipal.DocFisicoId = Guid.NewGuid();
                            propostaDocPrincipal.DsDoc       = docPrincipal.DsDoc;
                            propostaDocPrincipal.DtUpLoad    = docPrincipal.DtUpLoad;
                            propostaDocPrincipal.IdTipoDoc   = docPrincipal.IdTipoDoc;
                            propostaDocPrincipal.NmDocumento = docPrincipal.NmDocumento;
                            propostaDocPrincipal.IdProposta  = proposta.IdProposta;
                            propostaDocPrincipal.NmCriador   = docPrincipal.NmCriador;
                            db.PropostaDocsPrincipais.Add(propostaDocPrincipal);
                        }
                    }
                    if (propostaAntigaDocumentos.Count > 0)
                    {
                        foreach (var documento in propostaAntigaDocumentos)
                        {
                            var propostaDoc         = new PropostaDocs();
                            propostaDoc.DocFisico   = documento.DocFisico;
                            propostaDoc.DocFisicoId = Guid.NewGuid();
                            propostaDoc.DsDoc       = documento.DsDoc;
                            propostaDoc.DtUpLoad    = documento.DtUpLoad;
                            propostaDoc.IdProposta  = proposta.IdProposta;
                            propostaDoc.IdTipoDoc   = documento.IdTipoDoc;
                            propostaDoc.NmCriador   = documento.NmCriador;
                            propostaDoc.NmDocumento = documento.NmDocumento;
                            db.PropostaDocs.Add(propostaDoc);
                        }
                    }
                    proposta.DtCriacao = DateTime.Now;
                    db.Proposta.Add(proposta);
                    db.SaveChanges();
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + proposta.IdProposta + "] inserida com sucesso");
                    item.IdProposta = proposta.IdProposta;
                }

                // Salva e atualiza os registros na tabela Proposta Cliente
                SalvaPropostaCliente(item);
                // Salva os registros na tabela Proposta Coordenador
                SalvaPropostaCoordenador(item);

                db.SaveChanges();

                // Salvar os Aprovadores da Proposta
                foreach (var coordenadorSelecionado in item.CoordenadoresSelecionados)
                {
                    var coordenador = new bPropostaCoordenador(db).GetByPropostaPessoa(proposta.IdProposta, coordenadorSelecionado);
                    coordenador.IcAprovado = true;
                }

                if (situacaoNova)
                {
                    if ((AppSettings.constGlobalUserID == 0) && ((Int32)proposta.IdUsuarioUltimaAlteracao != 0)) { AppSettings.constGlobalUserID = (Int32)proposta.IdUsuarioUltimaAlteracao; }
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico da Proposta [" + proposta.IdProposta + "] com situacao [" + proposta.IdSituacao + "] Usuario info [" + (Int32)proposta.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "] alterado");

                    //EGS 30.03.2021 Valida Historico da Proposta
                    new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                    var newPropostaHist        = new PropostaHistorico();
                    newPropostaHist.IdProposta = proposta.IdProposta;
                    newPropostaHist.DtInicio   = DateTime.Now;
                    newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                    newPropostaHist.IdSituacao = proposta.IdSituacao;
                    db.PropostaHistorico.Add(newPropostaHist);
                }

                db.SaveChanges();
              //_GLog._GravaLog(AppSettings.constGlobalUserID, "Update Proposta [" + proposta.IdProposta + "] SaveChanges Final concluido");
            }
            catch (Exception ex)
            {
                new bEmail(db).EnviarEmailTratamentoErro(ex, "bProposta-Update [" + proposta.IdProposta + "]");
            }

            return proposta.IdProposta;
        }





        public void UpdateJuridico(InputUpdateJuridico item)
        {
            var proposta = new bProposta(db).GetById(item.IdProposta);
            var contrato = new Contrato();
            if (proposta.IdTipoOportunidade == 7)
            {
                contrato = new bContrato(db).BuscarContratoId(proposta.IdContrato.Value);
                var contratoAditivo = db.ContratoAditivo.Where(w => w.IdContrato == contrato.IdContrato && w.IdProposta == proposta.IdProposta).FirstOrDefault();
                if (contratoAditivo != null)
                {
                    contratoAditivo.DtFimAditivada = item.DtNovoFimVigencia;
                }
            }
            else
            {
                contrato = db.Contrato.Where(w => w.IdProposta == proposta.IdProposta).FirstOrDefault();
            }
            var ordemInicio = db.PropostaDocsPrincipais.Where(w => w.IdProposta == proposta.IdProposta && w.IdTipoDoc == 16).FirstOrDefault();

            bool situacaoNova = false;

            if (proposta.IdSituacao == 37 && contrato != null && item.DtAssinaturaContrato != null
                && ordemInicio != null && item.OrdemInicio ||
                proposta.IdSituacao == 37 && contrato != null && item.DtAssinaturaContrato != null
                && !item.OrdemInicio)
            {
                if (item.IcInformacoesIncompletas != null)
                {
                    if (!item.IcInformacoesIncompletas.Value)
                    {
                        contrato.IcInformacoesIncompletas = false;
                        proposta.IcInformacoesIncompletas = false;
                    }
                }
            }

            if (proposta.IdSituacao != item.IdSituacao)
            {
                situacaoNova = true;
                var propostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);
                propostaHist.DtFim = DateTime.Now;
            }

            proposta.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
            proposta.DtUltimaAlteracao = DateTime.Now;
            proposta.IdFundamento = (short)item.IdFundamento;
            if (item.IdTipoReajuste != null)
            {
                proposta.IdTipoReajuste = (short)item.IdTipoReajuste;
            }
            proposta.OrdemInicio = item.OrdemInicio;
            proposta.RenovacaoAutomatica = item.RenovacaoAutomatica;
            proposta.Reajustes = item.Reajustes;
            proposta.VlContrato = item.VlContrato;
            proposta.DtAssinaturaContrato = item.DtAssinaturaContrato;
            if (contrato != null)
            {
                contrato.DtAssinatura = proposta.DtAssinaturaContrato;
            }
            proposta.IdSituacao = item.IdSituacao;
            proposta.DsObjeto = item.DsObjeto;
            proposta.IcContratantesValidos = item.IcContratantesValidos;
            proposta.IdUnidadeTempoJuridico = item.IdUnidadeTempoJuridico;
            proposta.NuPrazoExecucaoJuridico = item.NuPrazoExecucaoJuridico;
            proposta.NuPrazoEstimadoMesJuridico = item.NuPrazoEstimadoMesJuridico;
            proposta.NuContratoCliente = item.NuContratoCliente;
            proposta.IdTipoAditivo = item.IdTipoAditivo;
            proposta.DtNovoFimVigencia = item.DtNovoFimVigencia;
            proposta.VlContratoComAditivo = item.VlContratoComAditivo;
            proposta.IcAditivoData = item.IcAditivoData;
            proposta.IcAditivoEscopo = item.IcAditivoEscopo;
            proposta.IcAditivoValor = item.IcAditivoValor;
            proposta.IcAditivoRetRat = item.IcAditivoRetRat;
            proposta.IcRitoSumario = item.IcRitoSumario;
            proposta.VlAditivo = item.VlAditivo;
            proposta.DsAditivo = item.DsAditivo;

            if (situacaoNova)
            {
                //EGS 30.03.2021 Valida Historico da Proposta
                new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                var newPropostaHist        = new PropostaHistorico();
                newPropostaHist.IdProposta = proposta.IdProposta;
                newPropostaHist.DtInicio   = DateTime.Now;
                newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                newPropostaHist.IdSituacao = proposta.IdSituacao;
                db.PropostaHistorico.Add(newPropostaHist);
            }

            _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico Proposta [" + proposta.IdProposta + "] Usuario Info [" + (Int32)proposta.IdUsuarioUltimaAlteracao + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");


            db.SaveChanges();

        }

        public PropostaHistorico GetPropostaHistBySituacao(int idProposta)
        {
            var propostaHist = db.PropostaHistorico
                .Where(w => w.IdProposta == idProposta)
                .OrderByDescending(w => w.IdPropostaHistorico).
                FirstOrDefault();

            return propostaHist;
        }

        public List<PropostaHistorico> GetPropostaHist(int idProposta)
        {
            var propostaHist = db.PropostaHistorico
                .Where(w => w.IdProposta == idProposta).OrderByDescending(w => w.IdPropostaHistorico).OrderByDescending(w => w.IdPropostaHistorico).ToList();

            return propostaHist;
        }

        public PropostaComentario GetComentarioById(int id)
        {
            var item = db.PropostaComentario.Where(w => w.IdPropostaComentario == id).FirstOrDefault();
            return item;
        }

        public bool UpdateComentario(PropostaComentario item)
        {
            try
            {
                item.IdUsuario = AppSettings.constGlobalUserID;
                db.PropostaComentario.Update(item);
                db.SaveChanges();
                _GLog._GravaLog(AppSettings.constGlobalUserID, "UpdateComentario [" + item.IdProposta + "] Usuario Info [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void AddComentario(PropostaComentario item)
        {

            item.IdUsuario = AppSettings.constGlobalUserID;
            db.PropostaComentario.Add(item);
            db.SaveChanges();
            _GLog._GravaLog(AppSettings.constGlobalUserID, "AddComentario [" + item.IdProposta + "] Usuario Info [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

        }

        public void AddComentarioHistorico(PropostaComentarioLido item)
        {
            item.IdUsuario = AppSettings.constGlobalUserID;
            db.PropostaComentarioLido.Add(item);
            db.SaveChanges();
            _GLog._GravaLog(AppSettings.constGlobalUserID, "AddComentarioHistorico [" + item.IdPropostaComentario + "] Usuario Info [" + item.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

        }

        public List<PropostaComentario> BuscarComentario(int id)
        {

            var itens = db.PropostaComentario.Where(w => w.IdProposta == id)
                .OrderByDescending(o => o.DtComentario)
                .ToList();

            return itens;
        }

        public List<PropostaHistorico> BuscarSituacao(int id)
        {

            var itens = db.PropostaHistorico.Where(w => w.IdProposta == id)
                .OrderByDescending(w => w.IdPropostaHistorico)
                .ToList();

            return itens;
        }

        public List<TipoAditivo> BuscarTiposAditivo()
        {
            var itens = db.TipoAditivo.ToList();

            return itens;
        }

        public List<PropostaComentario> ComentarioDiretoria(int id, int proposta)
        {
            var itens = db.PropostaComentario.Where(w => w.IdUsuario == id && w.IdProposta == proposta && w.DtComentario.Date == DateTime.Now.Date).ToList();

            return itens;
        }

        public List<PropostaComentario> ComentarioJuridico(int id, int proposta)
        {
            var itens = db.PropostaComentario.Where(w => w.IdUsuario == id && w.IdProposta == proposta && w.DtComentario.Date == DateTime.Now.Date).ToList();

            return itens;
        }

        public List<PropostaComentario> ListaComentarioGestorProposta(int id, int proposta)
        {
            var itens = db.PropostaComentario.Where(w => w.IdUsuario == id && w.IdProposta == proposta && w.DtComentario.Date == DateTime.Now.Date).ToList();

            return itens;
        }

        public List<Proposta> Get()
        {
            return db.Proposta.OrderByDescending(w => w.IdProposta).ToList();
        }

        public List<Proposta> GetByIdSituacao(int idSituacao)
        {
            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && p.IdSituacao == idSituacao)
                .OrderBy(w => w.DtLimiteEntregaProposta)
                .ToList();

            var propostasSemDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta == null && p.IdSituacao == idSituacao)
                .OrderBy(w => w.IdProposta)
                .ToList();

            propostasComDataLimite.AddRange(propostasSemDataLimite);

            return propostasComDataLimite;
        }
        public List<Proposta> GetByIdSituacaoJuridico(int idSituacao)
        {
            var propostas = db.Proposta
                .Where(p => p.IdSituacao == idSituacao)
                .OrderBy(w => w.DtUltimaAlteracao)
                .ToList();

            return propostas;
        }
        public List<Proposta> GetPropostasDiretoria()
        {
            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && p.IdSituacao == 7)
                .OrderBy(w => w.DtLimiteEntregaProposta)
                .ToList();

            var propostasSemDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta == null && p.IdSituacao == 7)
                .OrderBy(w => w.IdProposta)
                .ToList();

            propostasComDataLimite.AddRange(propostasSemDataLimite);

            return propostasComDataLimite;
        }

        public List<Proposta> GetPropostasJuridicoSemContrato()
        {
            var propostas = new List<Proposta>();

            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && (p.IdSituacao == 11 || p.IdSituacao == 31 || p.IdSituacao == 32 || p.IdSituacao == 37 || p.IdSituacao == 99))
                .OrderBy(w => w.DtUltimaAlteracao)
                .ToList();

            foreach (var proposta in propostasComDataLimite)
            {
                var contrato = db.Contrato.Where(w => w.IdProposta == proposta.IdProposta).FirstOrDefault();
                if (contrato == null)
                {
                    propostas.Add(proposta);
                }
            }

            return propostas;
        }

        public List<Proposta> GetPropostasJuridico()
        {
            var propostas = db.Proposta
                .Where(p => p.IdSituacao == 11 || p.IdSituacao == 31 || p.IdSituacao == 32 || p.IdSituacao == 37 || p.IdSituacao == 99 || p.IdSituacao == 107 || p.IdSituacao == 108)
                .OrderBy(w => w.DtUltimaAlteracao)
                .ToList();

            return propostas;
        }

        public List<Proposta> GetPropostasJuridicoInfoIncompleta()
        {
            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && p.IcInformacoesIncompletas != null && p.IcInformacoesIncompletas.Value)
                .OrderBy(w => w.DtLimiteEntregaProposta)
                .ToList();

            var propostasSemDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta == null && p.IcInformacoesIncompletas != null && p.IcInformacoesIncompletas.Value)
                .OrderBy(w => w.IdProposta)
                .ToList();

            propostasComDataLimite.AddRange(propostasSemDataLimite);

            return propostasComDataLimite;
        }

        public List<Proposta> GetPropostasAditivoJuridico()
        {
            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && p.IdSituacao == 99)
                .OrderBy(w => w.DtLimiteEntregaProposta)
                .ToList();

            var propostasSemDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta == null && p.IdSituacao == 99)
                .OrderBy(w => w.IdProposta)
                .ToList();

            propostasComDataLimite.AddRange(propostasSemDataLimite);

            return propostasComDataLimite;
        }

        public List<Proposta> GetPropostasGestorContrato()
        {
            var propostasComDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta != null && p.IdSituacao == 31 || p.DtLimiteEntregaProposta != null && p.IdSituacao == 32 || p.DtLimiteEntregaProposta != null && p.IdSituacao == 37)
                .OrderBy(w => w.DtLimiteEntregaProposta)
                .ToList();

            var propostasSemDataLimite = db.Proposta
                .Where(p => p.DtLimiteEntregaProposta == null && p.IdSituacao == 31 || p.DtLimiteEntregaProposta == null && p.IdSituacao == 32 || p.DtLimiteEntregaProposta == null && p.IdSituacao == 37)
                .OrderBy(w => w.IdProposta)
                .ToList();

            propostasComDataLimite.AddRange(propostasSemDataLimite);

            return propostasComDataLimite;
        }

        public List<Proposta> GetPropostasPorDataLimite(DateTime filtro)
        {
            var propostasComDataLimite = new List<Proposta>();

            propostasComDataLimite = db.Proposta
            .Where(p => p.DtLimiteEntregaProposta != null && p.DtLimiteEntregaProposta.Value.Date <= filtro.Date && p.DtLimiteEntregaProposta.Value.Date >= DateTime.Now.Date)
            .OrderBy(w => w.DtLimiteEntregaProposta)
            .ToList();

            return propostasComDataLimite;
        }

        public List<Proposta> GetPropostasPorDataValidade(DateTime filtro)
        {
            var propostasComDataValidade = new List<Proposta>();

            propostasComDataValidade = db.Proposta
               .Where(p => p.DtValidadeProposta != null && p.DtValidadeProposta.Value.Date <= filtro.Date && p.DtValidadeProposta.Value.Date >= DateTime.Now.Date)
               .OrderBy(w => w.DtValidadeProposta)
               .ToList();

            return propostasComDataValidade;
        }
        public PropostaContato GetByIdContato(int id)
        {
            var contato = db.PropostaContato
                .Where(w => w.IdPropostaContato == id && w.IdTipoContato == 2)
                .Single();

            return contato;
        }

        public Proposta GetById(int id)
        {
            var proposta = db.Proposta
                .Where(w => w.IdProposta == id)
                .Single();

            return proposta;
        }

        public OutPutCancelarProposta CancelarProposta(int id, int idUsuario)
        {
            var retorno = new OutPutCancelarProposta();
            retorno.Result = false;

            var proposta = db.Proposta.Where(w => w.IdProposta == id).FirstOrDefault();

            if (proposta != null)
            {
                //EGS 30.03.2021 Valida Historico da Proposta
                new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

                proposta.IdSituacao        = 52;
                var newPropostaHist        = new PropostaHistorico();
                newPropostaHist.IdProposta = proposta.IdProposta;
                newPropostaHist.DtInicio   = DateTime.Now;
                newPropostaHist.IdUsuario  = AppSettings.constGlobalUserID;
                newPropostaHist.IdSituacao = proposta.IdSituacao;
                db.PropostaHistorico.Add(newPropostaHist);

                retorno.Result = true;
                _GLog._GravaLog(AppSettings.constGlobalUserID, "CancelarProposta [" + proposta.IdProposta + "] Usu Info [" + idUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");

                db.SaveChanges();
            }

            return retorno;
        }

        #region Métodos de Cliente
        public void AddPropostaCli(List<PropostaCliente> itensCli)
        {
            db.PropostaCliente.AddRange(itensCli);

            db.SaveChanges();
        }

        public List<Cliente> BuscarCliente()
        {

            var itens = db.Cliente.ToList();

            return itens;
        }

        public void ExcluirCliente(int id)
        {
            var itens = db.PropostaCliente.Where(w => w.IdProposta == id).ToList();

            foreach (var item in itens)
            {
                db.PropostaCliente.Remove(item);
            }

            db.SaveChanges();
        }
        #endregion

        #region Métodos de Coordenador
        public void AddPropostaCoord(List<PropostaCoordenador> itensCoord)
        {
            db.PropostaCoordenador.AddRange(itensCoord);

            db.SaveChanges();
        }

        public List<PessoaFisica> BuscarCoordenadores()
        {

            var itens = db.PessoaFisica.ToList();

            return itens;
        }

        public void ExcluirCoordenador(int id)
        {

            var itens = db.OportunidadeResponsavel.Where(w => w.IdOportunidade == id).ToList();

            foreach (var item in itens)
            {
                db.OportunidadeResponsavel.Remove(item);
            }

            db.SaveChanges();

        }
        #endregion

        #region Métodos de Situação
        public List<Situacao> BuscarSituacao()
        {

            var itens = db.Situacao.Where(w => w.IcEntidade == "P" && w.DsSubArea != "Aditivo").OrderBy(w => w.NuOrdem).ToList();

            return itens;
        }

        public List<Situacao> BuscarSituacaoAditivo()
        {

            var itens = db.Situacao.Where(w => w.IcEntidade == "P" && w.DsSubArea == "Aditivo").OrderBy(w => w.NuOrdem).ToList();

            return itens;
        }

        public List<Situacao> BuscarSituacaoArea(string area)
        {
            var itens = db.Situacao.Where(w => w.IcEntidade == "P" && w.DsArea == area && w.DsSubArea != "Aditivo").OrderBy(w => w.NuOrdem).ToList();

            return itens;
        }
        #endregion

        #region Métodos de Documento

        public void AddDocumento(PropostaDocs item)
        {

            db.PropostaDocs.Add(item);
            db.SaveChanges();


        }

        public List<PropostaDocs> BuscarDocumentos(int id)
        {

            var itens = db.PropostaDocs.Where(w => w.IdProposta == id).ToList();

            return itens;
        }

        public List<TipoDocumento> BuscarTipoDocumentos(int id)
        {

            var itens = db.TipoDocumento.Where(w => w.IdEntidade == id && w.IcDocContratual == false).ToList();

            return itens;
        }


        public void RemoveDocumento(int id)
        {

            var itemDoc = db.PropostaDocs.Where(w => w.IdPropostaDocs == id).FirstOrDefault();

            db.PropostaDocs.Remove(itemDoc);

            db.SaveChanges();

        }

        public PropostaDocs BuscarDocumentoId(int id)
        {
            var item = db.PropostaDocs
                .Where(w => w.IdPropostaDocs == id).FirstOrDefault();

            return item;
        }

        //Documentos Principais
        //NAO ESTOU USANDO, vou tentar colocar direto em PRD...
        public void AddDocumentoPrincipal(PropostaDocsPrincipais item)
        {
            //EGS 08.03.2021 Tenta salvar, se não conseguir, tenta de novo
            bool bVaiSalvar = false;
            int iValSalvar  = 1;
            while (!bVaiSalvar)
            {
                try
                {
                    db.PropostaDocsPrincipais.Add(item);
                    db.SaveChanges();
                    if (iValSalvar <= 1)
                    {
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "AddDocumentoPrincipal Prop [" + item.IdProposta + "] - Doc [" + item.NmDocumento + "] Gravada com sucesso");
                    } else {
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "AddDocumentoPrincipal Prop [" + item.IdProposta + "] - Doc [" + item.NmDocumento + "] Gravada depois de [" + iValSalvar + "]x");
                    }
                    bVaiSalvar = true;
                    break;
                }
                catch (Exception ex)
                {
                    string sLlog = ex.Message + " " + ex.InnerException;
                    if (iValSalvar <= 3)
                    {
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "AddDocumentoPrincipal Prop [" + item.IdProposta + "] - Erro ao gravar [" + iValSalvar + "] tentando novamente - " + sLlog);
                    } else { 
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "AddDocumentoPrincipal Prop [" + item.IdProposta + "] - Não foi possivel fazer Upload do documento, finalizando....");
                        break;
                    }
                }
                iValSalvar = iValSalvar + 1;
                System.Threading.Thread.Sleep(1000);
            }


        }

        public List<PropostaDocsPrincipais> BuscarDocumentosPrincipais(int id)
        {

            var itens = db.PropostaDocsPrincipais.Where(w => w.IdProposta == id).ToList();

            return itens;
        }

        public List<TipoDocumento> BuscarTipoDocumentosPrincipais()
        {

            var itens = db.TipoDocumento.ToList();

            return itens;
        }

        public void RemoveDocumentoPrincipal(int id)
        {

            var itemDoc = db.PropostaDocsPrincipais.Where(w => w.IdPropostaDocsPrincipais == id).FirstOrDefault();

            db.PropostaDocsPrincipais.Remove(itemDoc);

            db.SaveChanges();

        }

        public PropostaDocsPrincipais BuscarDocumentoPrincipalId(int id)
        {
            var item = db.PropostaDocsPrincipais
                .Where(w => w.IdPropostaDocsPrincipais == id).FirstOrDefault();

            return item;
        }

        #endregion

        #region Métodos de Contatos
        public void AddContato(PropostaContato item)
        {
            db.PropostaContato.Add(item);
            db.SaveChanges();
            var idCli = item.IdPropostaContato;
        }

        public List<PropostaContato> BuscarContato(int id)
        {
            var itens = db.PropostaContato.Where(w => w.IdProposta == id).ToList();

            return itens;
        }

        public PropostaContato BuscarContatoId(int id)
        {
            var item = db.PropostaContato.Where(w => w.IdPropostaContato == id).FirstOrDefault();

            return item;
        }

        public void RemoverContato(int id)
        {
            var itemContato = db.PropostaContato.FirstOrDefault(_ => _.IdPropostaContato == id);

            if (itemContato != null)
            {
                db.PropostaContato.Remove(itemContato);
                db.SaveChanges();
            }
        }

        public void UpdateContato(PropostaContato item)
        {
            var itemContato = BuscarContatoId(item.IdPropostaContato);

            itemContato.IdPropostaContato = item.IdPropostaContato;
            itemContato.NmContato = item.NmContato;
            itemContato.NuCelular = item.NuCelular;
            itemContato.NuTelefone = item.NuTelefone;
            itemContato.NmDepartamento = item.NmDepartamento;
            itemContato.CdEmail = item.CdEmail;
            itemContato.IdTipoContato = item.IdTipoContato;

            db.SaveChanges();
        }
        #endregion

        #region Métodos auxiliares Proposta        
        private void CopiaPropostaDocs(Proposta proposta)
        {
            var oportunidadesDocs = db.OportunidadeDocs
                            .Where(w => w.IdOportunidade == proposta.IdOportunidade)
                            .ToList();

            foreach (var oportunidadeDoc in oportunidadesDocs)
            {
                if (oportunidadeDoc.IdTipoDocumento == 1)
                {
                    var propostaDocPrincipal = new PropostaDocsPrincipais();
                    propostaDocPrincipal.IdTipoDoc = oportunidadeDoc.IdTipoDocumento;
                    propostaDocPrincipal.IdProposta = proposta.IdProposta;
                    propostaDocPrincipal.DocFisico = oportunidadeDoc.DocFisico;
                    propostaDocPrincipal.NmDocumento = oportunidadeDoc.NmDocumento;
                    propostaDocPrincipal.NmCriador = oportunidadeDoc.NmCriador;
                    propostaDocPrincipal.DsDoc = oportunidadeDoc.NmDocumento;
                    propostaDocPrincipal.DtUpLoad = oportunidadeDoc.DtUpLoad;

                    db.PropostaDocsPrincipais.Add(propostaDocPrincipal);
                }
                else
                {
                    var propostaDocs = new PropostaDocs();
                    propostaDocs.IdTipoDoc = oportunidadeDoc.IdTipoDocumento;
                    propostaDocs.IdProposta = proposta.IdProposta;
                    propostaDocs.DocFisico = oportunidadeDoc.DocFisico;
                    propostaDocs.NmDocumento = oportunidadeDoc.NmDocumento;
                    propostaDocs.NmCriador = oportunidadeDoc.NmCriador;
                    propostaDocs.DsDoc = oportunidadeDoc.NmDocumento;
                    propostaDocs.DtUpLoad = oportunidadeDoc.DtUpLoad;

                    db.PropostaDocs.Add(propostaDocs);
                }
            }

            db.SaveChanges();
        }

        private void CopiaPropostaContato(Proposta proposta)
        {
            var oportunidadesContatos = db.OportunidadeContato
                            .Where(w => w.IdOportunidade == proposta.IdOportunidade)
                            .ToList();

            foreach (var oportunidadeContato in oportunidadesContatos)
            {
                var propostaContato = new PropostaContato();
                propostaContato.CdEmail = oportunidadeContato.CdEmail;
                propostaContato.IdProposta = proposta.IdProposta;
                propostaContato.NmContato = oportunidadeContato.NmContato;
                propostaContato.NmDepartamento = oportunidadeContato.NmDepartamento;
                propostaContato.NuCelular = oportunidadeContato.NuCelular;
                propostaContato.NuTelefone = oportunidadeContato.NuTelefone;
                propostaContato.IdTipoContato = oportunidadeContato.IdTipoContato;

                db.PropostaContato.Add(propostaContato);
            }

            db.SaveChanges();
        }
        private void CopiaPropostaCoordenador(Proposta proposta)
        {
            var oportunidadesResponsaveis = db.OportunidadeResponsavel
                            .Where(w => w.IdOportunidade == proposta.IdOportunidade)
                            .ToList();

            foreach (var oportunidadeResponsavel in oportunidadesResponsaveis)
            {
                var propostaCoordenador = new PropostaCoordenador();
                propostaCoordenador.IdPessoa = oportunidadeResponsavel.IdPessoaFisica;
                propostaCoordenador.IdProposta = proposta.IdProposta;
                propostaCoordenador.GuidPropostaCoordenador = Guid.NewGuid();

                db.PropostaCoordenador.Add(propostaCoordenador);
            }

            db.SaveChanges();
        }

        private void CopiaPropostaCliente(Proposta proposta)
        {
            var oportunidadesClientes = db.OportunidadeCliente
                            .Where(w => w.IdOportunidade == proposta.IdOportunidade)
                            .ToList();

            foreach (var oportunidadeCliente in oportunidadesClientes)
            {
                var propostaCliente = new PropostaCliente();
                propostaCliente.IdCliente = (Int32)oportunidadeCliente.IdCliente;
                propostaCliente.IdProposta = proposta.IdProposta;
                propostaCliente.NmFantasia = oportunidadeCliente.NmFantasia;
                propostaCliente.RazaoSocial = oportunidadeCliente.RazaoSocial;

                db.PropostaCliente.Add(propostaCliente);
            }

            db.SaveChanges();
        }

        private void SalvaPropostaCoordenador(InputUpdate item)
        {
            if (item.Coordenadores.Count > 0)
            {
                foreach (var pCoordenador in item.Coordenadores)
                {
                    var propostaCoordenador = new bPropostaCoordenador(db).GetByPropostaPessoa(item.IdProposta, pCoordenador.IdPessoaFisica);

                    if (propostaCoordenador == null)
                    {
                        var newPropostaCoordenador = new PropostaCoordenador();
                        newPropostaCoordenador.IdProposta = item.IdProposta;
                        newPropostaCoordenador.IdPessoa = pCoordenador.IdPessoaFisica;
                        newPropostaCoordenador.IcAprovado = pCoordenador.IcAprovado;
                        newPropostaCoordenador.GuidPropostaCoordenador = Guid.NewGuid();

                        db.PropostaCoordenador.Add(newPropostaCoordenador);
                    }
                    else
                    {
                        propostaCoordenador.IcAprovado = false;
                    }
                }
            }

            //  Exclui os Coordenadores que não fazem mais parte da Proposta
            var allPropostasCoord = db.PropostaCoordenador
                .Where(w => w.IdProposta == item.IdProposta)
                .ToList();
            foreach (var p in allPropostasCoord)
            {
                var propostaCoord = item.Coordenadores
                    .Where(w => w.IdPessoaFisica == p.IdPessoa)
                    .FirstOrDefault();

                if (propostaCoord == null)
                {
                    db.PropostaCoordenador.Remove(p);
                }
            }
        }

        private void SalvaPropostaCliente(InputUpdate item)
        {
            if (item.Clientes.Count > 0)
            {
                foreach (var pCliente in item.Clientes)
                {
                    if (pCliente.IdCliente != 0)
                    {
                        var propostaCliente = new bPropostaCliente(db).GetByPropostaCliente(item.IdProposta, (Int32)pCliente.IdCliente);
                        if (propostaCliente == null)
                        {
                            var newPropostaCliente = new PropostaCliente();

                            newPropostaCliente.IdCliente = (Int32)pCliente.IdCliente;
                            newPropostaCliente.IdProposta = item.IdProposta;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == pCliente.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newPropostaCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newPropostaCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }

                            db.PropostaCliente.Add(newPropostaCliente);
                        }
                    }
                    else
                    {
                        var clienteExiste = new bCliente(db).BuscarClienteIdPessoa(pCliente.IdPessoa);
                        if (clienteExiste != null)
                        {
                            var newPropostaCliente = new PropostaCliente();

                            newPropostaCliente.IdCliente = clienteExiste.IdCliente;
                            newPropostaCliente.IdProposta = item.IdProposta;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == clienteExiste.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newPropostaCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newPropostaCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            db.PropostaCliente.Add(newPropostaCliente);
                        }
                        else
                        {
                            var cliente = new Cliente();
                            cliente.IdPessoa = pCliente.IdPessoa;
                            new bCliente(db).AddCliente(cliente);

                            var newPropostaCliente = new PropostaCliente();

                            newPropostaCliente.IdCliente = cliente.IdCliente;
                            newPropostaCliente.IdProposta = item.IdProposta;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newPropostaCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newPropostaCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            db.PropostaCliente.Add(newPropostaCliente);
                        }
                    }
                }
            }

            //  Exclui os Clientes que não fazem mais parte da Proposta
            var allPropostasClientes = db.PropostaCliente
                .Where(w => w.IdProposta == item.IdProposta)
                .ToList();
            foreach (var p in allPropostasClientes)
            {
                var propostaCli = item.Clientes
                    .Where(w => w.IdCliente == p.IdCliente && w.IdCliente != 0)
                    .FirstOrDefault();

                if (propostaCli == null)
                {
                    db.PropostaCliente.Remove(p);

                    var opCliente = db.OportunidadeCliente.Where(w => w.IdCliente == p.IdCliente).FirstOrDefault();
                    if (opCliente == null)
                    {
                        var contratoCliente = db.ContratoCliente.Where(w => w.IdCliente == p.IdCliente).FirstOrDefault();
                        if (contratoCliente == null)
                        {
                            var cliente = new bCliente(db).BuscarClienteId(p.IdCliente);
                            db.Cliente.Remove(cliente);
                        }
                    }
                    db.SaveChanges();
                }
            }

        }
        #endregion







        #region Funcoes e Rotinas
        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Setembro/2020 
        *  Verifica se pode enviar o email da proposta para o Usuario
        ==============================================================================================*/
        public bool _PodeEnviarEmail(int pIDProposta, int pCoordenador, string pEmail)
        {
            bool bEnviaEmail = true;
            using (var db = new FIPEContratosContext())
            {
                //EGS 30.06.2020 - Verifica se deve enviar email, pelo Perfil Usuario => EnviaEmail
                var dbPessoa = db.Pessoa.Where(w => w.IdPessoaFisica == pCoordenador).FirstOrDefault();
                if (dbPessoa != null)
                {
                    var dbUsuario = db.Usuario.Where(w => w.IdPessoa == dbPessoa.IdPessoa).FirstOrDefault();
                    if (dbUsuario != null)
                    {
                        var dbPerfilUsuarioEnviaEmail = db.PerfilUsuario.Where(w => w.IdUsuario == dbUsuario.IdUsuario).FirstOrDefault();
                        if (dbPerfilUsuarioEnviaEmail != null)
                        {
                            bEnviaEmail = dbPerfilUsuarioEnviaEmail.EnviaEmail;
                        }
                    }
                }
                if (!bEnviaEmail)
                {
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Email de Proposta [" + pIDProposta + "] do Perfil NAO ESTA CONFIGURADO para ser enviado para [" + pEmail + "]");
                }
            }
            return bEnviaEmail;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Setembro/2020 
        *  Verifica se tem registro no Historico sem Dt.Final preenchida, antes de inserir um novo
        ==============================================================================================*/
        public bool _ValidaDataFinalHistorico(int pIDProposta)
        {
            bool bRetorno = false;
            using (var db = new FIPEContratosContext())
            {
                //EGS 30.06.2020 - Verifica se deve enviar email, pelo Perfil Usuario => EnviaEmail
                int pIDPropostaHistorico = 0;
                var dbPropHistorico = db.PropostaHistorico.Where(w => w.IdProposta == pIDProposta).ToList();
                foreach (var item in dbPropHistorico)
                {
                    if (item.DtFim == null)
                    {
                        bRetorno             = true;
                        pIDPropostaHistorico = item.IdPropostaHistorico;
                        item.DtFim           = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                if (bRetorno)
                {
                    _GLog._GravaLog(AppSettings.constGlobalUserID, "Historico da Proposta [" + pIDProposta + "] do ID [" + pIDPropostaHistorico.ToString() + "] corrigido com DT.FINAL");
                }
            }
            return bRetorno;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Setembro/2020 
        *  Verifica se tem aditivo PENDENTE, antes de inserir um novo
        ==============================================================================================*/
        public bool _ValidaExisteAditivoPendente(int pIDAditivo)
        {
            bool bRetorno = false;
            using (var db = new FIPEContratosContext())
            {
                //EGS Localiza o numero do contrato pelo ID aditivo
                var IdconAditivo = db.ContratoAditivo.Where(w => w.IdContratoAditivo == pIDAditivo).FirstOrDefault().IdContrato;
                if (IdconAditivo != 0)
                {
                    var contrato = db.Contrato.Where(w => w.IdContrato == IdconAditivo).FirstOrDefault();
                    if (contrato != null)
                    {
                        //Acha o IDSituacao de Aditivo aplicado
                        var sitAditivoAtivo = db.Situacao.Where(s => s.DsSituacao == "Pendente" && s.IcEntidade == "A").FirstOrDefault().IdSituacao;
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
            }
            return bRetorno;
        }



        #endregion













        #region Retornos
        public class InputAdd
        {
            public int IdSituacao { get; set; }
            public int? IdTipoProposta { get; set; }
            public int IdOportunidade { get; set; }
            public int? IdTema { get; set; }
            public string DsApelidoProposta { get; set; }
            public DateTime? DtProposta { get; set; }
            public DateTime? DtValidadeProposta { get; set; }
            public DateTime? DtLimiteEnvioProposta { get; set; }
            public string DsAssunto { get; set; }
            public string DsObjeto { get; set; }
            public string DsPrazoExecucao { get; set; }
            public decimal? VlProposta { get; set; }
            public short? NuPrazoEstimadoMes { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public DateTime DtCriacao { get; set; }
            public int? IdUsuarioUltimaAlteracao { get; set; }
            public DateTime? DtUltimaAlteracao { get; set; }
            public DateTime? DtAssinaturaContrato { get; set; }
            public DateTime? DtAutorizacaoInicio { get; set; }
        }

        public class InputUpdate
        {
            public int IdProposta { get; set; }
            public int IdSituacao { get; set; }
            public int? IdTema { get; set; }
            public string DsApelidoProposta { get; set; }
            public DateTime? DtValidadeProposta { get; set; }
            public DateTime? DtLimiteEntregaProposta { get; set; }
            public string DsAssunto { get; set; }
            public string DsObjeto { get; set; }
            public short? NuPrazoExecucao { get; set; }
            public decimal? VlProposta { get; set; }
            public int? IdUsuarioUltimaAlteracao { get; set; }
            public List<InputUpdatePropostaCliente> Clientes { get; set; }
            public List<OutputCoordenador> Coordenadores { get; set; }
            public int[] CoordenadoresSelecionados { get; set; }
            public string DsObservacao { get; set; }
            public int IdTipoOportunidade { get; set; }
            public bool? IcRitoSumario { get; set; }
            public int? IdContrato { get; set; }
            public string DsAditivo { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public bool? IcContratantesValidos { get; set; }
            public short? IdUnidadeTempo { get; set; }
            public short? IdUnidadeTempoJuridico { get; set; }
            public int? NuPrazoExecucaoJuridico { get; set; }
            public decimal? NuPrazoEstimadoMes { get; set; }
            public decimal? NuPrazoEstimadoMesJuridico { get; set; }
        }

        public class InputUpdatePropostaCliente
        {
            public int? IdCliente { get; set; }
            public int IdPessoa { get; set; }
            public string NmCliente { get; set; }
        }

        public class InputUpdatePropostaCoordenador
        {
            public int IdPropostaCoordenador { get; set; }
            public int IdProposta { get; set; }
            public int IdPessoa { get; set; }
        }

        public class InputUpdatePropostaDocs
        {
            public int IdPropostaDoc { get; set; }
            public int IdProposta { get; set; }
            public short IdTipoDoc { get; set; }
            public string DsDoc { get; set; }
        }

        public class OutputAdd
        {
            public bool Result { get; set; }

            public string IdProposta { get; set; }
        }

        public class OutputAddAditivo
        {
            public bool Result { get; set; }

            public int IdProposta { get; set; }
        }

        public class OutputUpdate
        {
            public bool Result { get; set; }
            public int IdProposta { get; set; }
        }
        #endregion
    }
}