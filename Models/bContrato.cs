using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;


namespace ApiFipe.Models
{
    public class bContrato
    {
        public FIPEContratosContext db { get; set; }     

        public bContrato(FIPEContratosContext db)
        {
            this.db = db;
        }

        public OutputAdd Add(InputAddContrato item)
        {
            var retorno   = new OutputAdd();
            var contrato  = new Contrato();
            var proposta  = new bProposta(db).BuscarPropostaId(item.IdProposta);
            var parametro = db.Parametro.FirstOrDefault();            

            proposta.IcInformacoesIncompletas = item.IcInformacoesIncompletas;

            var lastContrato = db.Contrato.OrderByDescending(w => w.IdContrato).FirstOrDefault();
            if (lastContrato != null)
            {
                contrato.IdContrato = lastContrato.IdContrato + 1;
            }
            else
            {
                contrato.IdContrato = 1;
            }

            contrato.IdProposta = item.IdProposta;
            contrato.IdSituacao = 18;
            contrato.IdUsuarioCriacao = AppSettings.constGlobalUserID;
            contrato.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
            contrato.DsAssunto = proposta.DsAssunto;
            contrato.DsApelido = proposta.DsApelidoProposta;
            contrato.DsObjeto = proposta.DsObjeto;
            contrato.DtCriacao = DateTime.Now;
            contrato.DtUltimaAlteracao = DateTime.Now;
            contrato.DsPrazoExecucao = string.Format("{0:0.######}", proposta.NuPrazoEstimadoMesJuridico);
            contrato.IcOrdemInicio = proposta.OrdemInicio;
            contrato.IcRenovacaoAutomatica = proposta.RenovacaoAutomatica;
            contrato.IcReajuste = proposta.Reajustes;
            contrato.IdTema = proposta.IdTema;
            contrato.VlContrato = proposta.VlContrato.Value;
            contrato.IdIndiceReajuste = proposta.IdTipoReajuste;
            contrato.DtAssinatura = proposta.DtAssinaturaContrato;
            contrato.IdFundamento = proposta.IdFundamento.Value;
            contrato.IcInformacoesIncompletas = false;
            contrato.DsPrazoPagamento = parametro.DsPrazoPagto;
            contrato.NuBanco = parametro.NuBanco;
            contrato.NuAgencia = parametro.NuAgencia;
            contrato.NuConta = parametro.NuConta;
            contrato.DsTextoCorpoNf = parametro.DsTextoCorpoNf.Replace("<BR>", "\n");
            contrato.NuContratoCliente = proposta.NuContratoCliente;
            contrato.IcInformacoesIncompletas = item.IcInformacoesIncompletas;
            contrato.IcFrenteUnica = true;
            var nuContratoEdit = contrato.IdContrato.ToString();
            while (nuContratoEdit.Length < 4)
            {
                nuContratoEdit = "0" + nuContratoEdit;
            }
            contrato.NuContratoEdit = "CT" + nuContratoEdit;

            db.Contrato.Add(contrato);

            db.SaveChanges();

            // Grava histórico ao criar o contrato
            var h = new ContratoHistorico();

            h.IdContrato = contrato.IdContrato;
            h.IdSituacao = contrato.IdSituacao;
            h.IdUsuario = AppSettings.constGlobalUserID;
            h.DtInicio = DateTime.Now;

            db.ContratoHistorico.Add(h);
            db.SaveChanges();

            // Caso usuário gerou o Contrato com informações incompletas , busca ultimo comentário e copia para Contrato
            retorno.bCompletoSucesso = true;
            if (proposta.IcInformacoesIncompletas != null)
            {
                if (proposta.IcInformacoesIncompletas == true) { retorno.bCompletoSucesso = false; } 
                if (proposta.IcInformacoesIncompletas.Value)
                {
                    var propostaComentario          = db.PropostaComentario.Where(w => w.IdProposta == proposta.IdProposta).OrderByDescending(w => w.IdPropostaComentario).FirstOrDefault();
                    var contratoComentario          = new ContratoComentario();
                    contratoComentario.DsComentario = propostaComentario.DsComentario;
                    contratoComentario.DtComentario = DateTime.Now;
                    contratoComentario.IdUsuario    = AppSettings.constGlobalUserID;
                    contratoComentario.IdContrato   = contrato.IdContrato;
                    db.ContratoComentario.Add(contratoComentario);

                    db.SaveChanges();

                    var contratoComentarioLido      = new ContratoComentarioLido();
                    contratoComentarioLido.IdContratoComentario = contratoComentario.IdContratoComentario;
                    contratoComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                    db.ContratoComentarioLido.Add(contratoComentarioLido);

                    db.SaveChanges();
                }
            }

            // Copia registro na tabela Contrato Contato com base na tabela Proposta Contato
            CopiaContratoCliente(contrato);

            // Copia registro na tabela Contrato Contato com base na tabela Proposta Contato
            CopiaContratoContato(contrato);

            // Copia registro na tabela Contrato Coordenador com base na tabela Proposta Coordenador
            CopiaContratoCoordenador(contrato);

            // Copia registro na tabela Contrato Docs Principais com base na tabela Proposta Docs Principais
            CopiaContratoDocsPrincipais(proposta, contrato.IdContrato);

            // Copia registro na tabela Contrato Comentários com base na tabela Proposta Comentários
            //CopiaContratoComentarios(proposta);            

            retorno.Result     = true;
            retorno.IdContrato = contrato.IdContrato.ToString();

            return retorno;
        }

        public bool EnviaEmailGestorContratos(int idProposta, int idContrato, string url, bool pSucesso)
        {
            try
            {
                string sIDEmailConteudo           = "Novo Contrato para Cadastramento";
                if (!pSucesso) { sIDEmailConteudo = "Novo Contrato para Cadastramento COM Pendências";  }
                #region Envia E-mail para o(s) Gestor(es) de Contratos informando encaminhamento de Contrato
                var email                    = new bEmail(db).GetEmailByTitulo(sIDEmailConteudo);
                var linkVisualizar           = url + "/contratos/" + idContrato;
                var usuariosGestoresContrato = db.PerfilUsuario.Where(p => p.IdPerfil == 4 && p.EnviaEmail == true).ToList();   //EGS 30.06.2020 So envia email se estiver marcado
                var assunto                  = email.DsTitulo;
                foreach (var gestorContrato in usuariosGestoresContrato)
                {
                    var usuario = db.Usuario.Where     (w => w.IdUsuario      == gestorContrato.IdUsuario).FirstOrDefault();
                    var pessoa  = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario.IdPessoa        ).FirstOrDefault();
                    var corpo   = String.Format(email.DsTexto, pessoa.NmPessoa, idContrato, idProposta, linkVisualizar);
                    new bEmail(db).EnviarEmail(pessoa.CdEmail, pessoa.NmPessoa, assunto, corpo, null, string.Empty);                
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Update(InputUpdateContrato item)
        {
            var contrato = new bContrato(db).BuscarContratoId(item.IdContrato);
            var histAnterior = db.ContratoHistorico.Where(w => w.IdContrato == item.IdContrato).LastOrDefault();

            if (histAnterior != null)
            {
                // Adiciona histórico se for alterado a situação
                if (item.IdSituacao != contrato.IdSituacao)
                {
                    if (histAnterior.DtFim == null)
                    {
                        histAnterior.DtFim = DateTime.Now;
                    }                  
                    db.SaveChanges();

                    var h = new ContratoHistorico();

                    h.IdContrato = item.IdContrato;
                    h.IdSituacao = item.IdSituacao;
                    h.IdUsuario = AppSettings.constGlobalUserID;
                    h.DtInicio = DateTime.Now;

                    db.ContratoHistorico.Add(h);
                    db.SaveChanges();
                }                
            }
            contrato.IdSituacao = item.IdSituacao;
            contrato.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
            contrato.DsAssunto = item.DsAssunto;
            contrato.DsObjeto = item.DsObjeto;
            contrato.DtUltimaAlteracao = DateTime.Now;
            contrato.DsPrazoExecucao = item.DsPrazoExecucao;
            contrato.IdTema = item.IdTema;
            contrato.VlContrato = item.VlContrato;
            contrato.IdIndiceReajuste = item.IdIndiceReajuste;
            contrato.DtAssinatura = item.DtAssinatura;
            contrato.IdFundamento = (short)item.IdFundamento;
            contrato.NuCentroCusto = item.NuCentroCusto;
            contrato.NuContratoCliente = item.NuContratoCliente;
            contrato.DtRenovacao = item.DtRenovacao;
            contrato.DtProxReajuste = item.DtProxReajuste;
            contrato.DsObservacao = item.DsObservacao;
            contrato.DsTextoCorpoNf = item.DsObservacao;
            contrato.DtInicio = item.DtInicio;
            contrato.DtFim = item.DtFim;
            contrato.CdIss = item.CdIss;
            contrato.DtInicioExecucao = item.DtInicioExecucao;
            contrato.DtFimExecucao = item.DtFimExecucao;
            contrato.IcFrenteUnica = item.IcFrenteUnica;
            contrato.IcContinuo = item.IcContinuo;
            contrato.IdArea = item.IdArea;
            contrato.NuProcessoCliente = item.NuProcessoCliente;
            contrato.IcInformacoesIncompletas = item.IcInformacoesIncompletas;
            contrato.IcViaFipeNaoAssinada = item.IcViaFipeNaoAssinada;
            contrato.NuContratoEdit = item.NuContratoEdit;

            var parcelas = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == contrato.IdContrato).ToList();

            foreach (var itemParcela in parcelas)
            {
                itemParcela.CdIss = contrato.CdIss;
            }

            //var clientesProposta = new bContratoCliente(db).GetByIdContrato(item.IdContrato);
            //foreach (var cli in clientesProposta)
            //{
            //    cli.IcPagador = false;
            //}

            //foreach (var clientesPagadores in item.ClientesPagadores)
            //{
            //    var cliente = new bContratoCliente(db).GetByContratoPessoa(item.IdContrato, clientesPagadores);
            //    cliente.IcPagador = true;
            //}

            // Salva os registros na tabela Contrato Cliente
            SalvaContratoCliente(item);

            // Salva os registros na tabela Contrato Coordenador
            SalvaContratoCoordenador(item);

            db.SaveChanges();
        }

        public void UpdateRenovacaoHistorico(InputUpdateContrato item)
        {
            var contrato = new bContrato(db).BuscarContratoId(item.IdContrato);
            var histAnterior = db.ContratoHistorico
                .Where(w => w.IdContrato == item.IdContrato).LastOrDefault();

            if (histAnterior != null)
            {
                // Adiciona histórico se for alterado a situação
                if (item.IdSituacao != contrato.IdSituacao)
                {
                    if (histAnterior.DtFim == null)
                    {
                        histAnterior.DtFim = DateTime.Now;
                    }
                    db.SaveChanges();

                    var h = new ContratoHistorico();

                    h.IdContrato = item.IdContrato;
                    h.IdSituacao = item.IdSituacao;
                    h.IdUsuario = AppSettings.constGlobalUserID;
                    h.DtInicio = DateTime.Now;

                    db.ContratoHistorico.Add(h);
                    db.SaveChanges();
                }
            }
        }


       /* ===========================================================================================
        *  Edinaldo FIPE
        *  Janeiro/2021 
        *  Salva informacao se o Contrato é Abono ou não
        ===========================================================================================*/
        public void UpdateContratoAbono(int pIDContrato, bool pContratoAbono)
        {
            var contrato = new bContrato(db).BuscarContratoId(pIDContrato);
            if (contrato != null)
            {
               //bool bAbono = false;
              //if (contrato.chkContratoAbono == false) { bAbono = true; } else { bAbono = false; }
                contrato.chkContratoAbono = pContratoAbono;
                db.Contrato.Update(contrato);
                db.SaveChanges();
            }
        }


        private void SalvaContratoCliente(InputUpdateContrato item)
        {
            if (item.clientes.Count > 0)
            {
                foreach (var cCliente in item.clientes)
                {
                    if (cCliente.IdCliente != 0)
                    {
                        var contratoCliente = new bContratoCliente(db).GetById(cCliente.IdCliente);
                        if (contratoCliente == null)
                        {
                            var newContratoCliente = new ContratoCliente();

                            newContratoCliente.IdCliente = (Int32)cCliente.IdCliente;
                            newContratoCliente.IdContrato = item.IdContrato;
                            newContratoCliente.IcPagador = cCliente.IcPagador;
                            newContratoCliente.IcSomentePagador = cCliente.IcSomentePagador;
                            newContratoCliente.NuContratante = cCliente.NuContratante;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cCliente.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newContratoCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newContratoCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            else
                            {
                                newContratoCliente.NmFantasia = cCliente.NmCliente;
                                newContratoCliente.RazaoSocial = cCliente.NmCliente;
                            }

                            db.ContratoCliente.Add(newContratoCliente);
                        }
                        else
                        {
                            contratoCliente.IcPagador = cCliente.IcPagador;
                            contratoCliente.IcSomentePagador = cCliente.IcSomentePagador;
                            contratoCliente.NuContratante = cCliente.NuContratante;

                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var clienteExiste = new bCliente(db).BuscarClienteIdPessoa(cCliente.IdPessoa.Value);
                        if (clienteExiste != null)
                        {
                            var newContratoCliente = new ContratoCliente();

                            newContratoCliente.IdCliente = clienteExiste.IdCliente;
                            newContratoCliente.IdContrato = item.IdContrato;
                            newContratoCliente.IcPagador = cCliente.IcPagador;
                            newContratoCliente.IcSomentePagador = cCliente.IcSomentePagador;
                            newContratoCliente.NuContratante = cCliente.NuContratante;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == clienteExiste.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newContratoCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newContratoCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            else
                            {
                                newContratoCliente.NmFantasia = cCliente.NmCliente;
                                newContratoCliente.RazaoSocial = cCliente.NmCliente;
                            }
                            db.ContratoCliente.Add(newContratoCliente);
                        }
                        else
                        {
                            var cliente = new Cliente();
                            cliente.IdPessoa = cCliente.IdPessoa.Value;
                            new bCliente(db).AddCliente(cliente);

                            var newContratoCliente = new ContratoCliente();

                            newContratoCliente.IdCliente = cliente.IdCliente;
                            newContratoCliente.IdContrato = item.IdContrato;
                            newContratoCliente.IcPagador = cCliente.IcPagador;
                            newContratoCliente.IcSomentePagador = cCliente.IcSomentePagador;
                            newContratoCliente.NuContratante = cCliente.NuContratante;

                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                            if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                newContratoCliente.NmFantasia = pessoaJuridica.NmFantasia;
                                newContratoCliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            else
                            {
                                newContratoCliente.NmFantasia = cCliente.NmCliente;
                                newContratoCliente.RazaoSocial = cCliente.NmCliente;
                            }
                            db.ContratoCliente.Add(newContratoCliente);
                        }
                    }
                }
            }

            //  Exclui os Clientes que não fazem mais parte do Contrato
            var allContratoClientes = db.ContratoCliente
                .Where(w => w.IdContrato == item.IdContrato)
                .ToList();
            foreach (var p in allContratoClientes)
            {
                var contratoCli = item.clientes
                    .Where(w => w.IdCliente == p.IdContratoCliente && w.IdCliente != 0)
                    .FirstOrDefault();

                if (contratoCli == null)
                {
                    db.ContratoCliente.Remove(p);

                    var opCliente = db.OportunidadeCliente.Where(w => w.IdCliente == p.IdCliente).FirstOrDefault();
                    if (opCliente == null)
                    {
                        var propCliente = db.PropostaCliente.Where(w => w.IdCliente == p.IdCliente).FirstOrDefault();
                        if (propCliente == null)
                        {
                            var contratoCliente = db.ContratoCliente.Where(w => w.IdCliente == p.IdCliente).FirstOrDefault();
                            if (contratoCliente == null)
                            {
                                var cliente = new bCliente(db).BuscarClienteId(p.IdCliente);
                                db.Cliente.Remove(cliente);
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }

        }

        // Dados de Cobrança
        public void UpdateDadosCobranca(Contrato item)
        {

            try
            {
                db.Contrato.Update(item);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        //Retorno planilha Excel Contrato
        public Contrato GetPlanilhaExcel(int id)
        {
            var contratoExcel = db.Contrato
                .Include(i => i.IdAreaNavigation)
                .Include(i => i.IdFormaPagamentoNavigation)
                .Include(i => i.IdFundamentoNavigation)
                .Include(i => i.IdIndiceReajusteNavigation)
                .Include(i => i.IdPropostaNavigation)
                .Include(i => i.IdSituacaoEquipeTecnicaNavigation)
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdTemaNavigation)
                .Include(i => i.IdTipoApresentacaoRelatorioNavigation)
                .Include(i => i.IdTipoCobrancaNavigation)
                .Include(i => i.IdTipoEntregaDocumentoNavigation)
                .Include(i => i.ContratoCliente)
                .Include(i => i.ContratoCronogramaFinanceiro)
                .Include(i => i.ContratoEntregavel)
                .Include(i => i.Frente)
                .Include(i => i.ContratoContatos)
                .Where(w => w.IdContrato == id).FirstOrDefault();

            return contratoExcel;
        }


        public OutPutGetContratoId GetContratoById(int id)
        {
            var itemContrato = new OutPutGetContratoId();

            try
            {
                var retornoContrato = new bContrato(db).BuscarContratoId(id);

                //EGS 30.05.2020 - Se não encontrar o contrato, nem continua...
                if (retornoContrato != null)
                {
                    var retornoClientes = new bContrato(db).BuscarClientes(retornoContrato.IdContrato);

                    itemContrato.ClientesPagadores = new List<int>();


                    itemContrato.IdProposta = (Int32)retornoContrato.IdProposta;
                    itemContrato.NuContratoEdit = retornoContrato.NuContratoEdit;
                    var proposta = new bProposta(db).GetById(itemContrato.IdProposta);

                    itemContrato.IdSituacao       = retornoContrato.IdSituacao;
                    itemContrato.IdTema           = retornoContrato.IdTema;
                    itemContrato.IdUsuarioCriacao = retornoContrato.IdUsuarioCriacao;

                    var NmUsuario                 = new bUsuario(db).GetById(retornoContrato.IdUsuarioCriacao);
                    var pessoaFisica              = new bPessoaFisica(db).GetById(NmUsuario.IdPessoa);
                    itemContrato.NmUsuario        = pessoaFisica.NmPessoa;

                    itemContrato.IdUsuarioUltimaAlteracao = (Int32)retornoContrato.IdUsuarioUltimaAlteracao;
                    itemContrato.DsApelidoProposta = retornoContrato.DsApelido;
                    itemContrato.DsAssunto = retornoContrato.DsAssunto;
                    itemContrato.DsObjeto = retornoContrato.DsObjeto;
                    itemContrato.DsPrazoExecucao = retornoContrato.DsPrazoExecucao;
                    itemContrato.DtAssinatura = retornoContrato.DtAssinatura;
                    itemContrato.DtCriacao = retornoContrato.DtCriacao;
                    itemContrato.DtUltimaAlteracao = retornoContrato.DtUltimaAlteracao;
                    itemContrato.IcReajuste = retornoContrato.IcReajuste;
                    itemContrato.IcOrdemInicio = retornoContrato.IcOrdemInicio;
                    itemContrato.VlContrato = retornoContrato.VlContrato;
                    if (retornoContrato.IdIndiceReajuste != null)
                    {
                        itemContrato.IdIndiceReajuste = retornoContrato.IdIndiceReajuste.Value;
                    }
                    if (retornoContrato.IcContinuo != null)
                    {
                        itemContrato.IcContinuo = retornoContrato.IcContinuo;
                    }
                    if (retornoContrato.IcFrenteUnica != null)
                    {
                        itemContrato.IcFrenteUnica = retornoContrato.IcContinuo;
                    }
                    itemContrato.IcPrazoIndeterminado = retornoContrato.IcRenovacaoAutomatica;
                    itemContrato.IcFrenteUnica = retornoContrato.IcFrenteUnica;
                    itemContrato.IdFundamento = retornoContrato.IdFundamento;
                    itemContrato.DtProxReajuste = retornoContrato.DtProxReajuste;
                    itemContrato.CdIss = retornoContrato.CdIss;
                    itemContrato.NuCentroCusto = retornoContrato.NuCentroCusto;
                    itemContrato.NuContratoCliente = retornoContrato.NuContratoCliente;
                    itemContrato.DtInicio = retornoContrato.DtInicio;
                    itemContrato.DtFim = retornoContrato.DtFim;
                    itemContrato.DtRenovacao = retornoContrato.DtRenovacao;
                    itemContrato.DsObservacao = retornoContrato.DsObservacao;
                    itemContrato.NuProcessoCliente = retornoContrato.NuProcessoCliente;
                    itemContrato.IdArea = retornoContrato.IdArea;
                    itemContrato.DtInicioExecucao = retornoContrato.DtInicioExecucao;
                    itemContrato.DtFimExecucao = retornoContrato.DtFimExecucao;
                    itemContrato.IcInformacoesIncompletas = retornoContrato.IcInformacoesIncompletas;
                    itemContrato.IcViaFipeNaoAssinada = retornoContrato.IcViaFipeNaoAssinada;

                    itemContrato.Coordenadores = new List<OutputResponsavel>();

                    foreach (var itemCoordResp in retornoContrato.ContratoCoordenador)
                    {
                        var itemCoord = new OutputResponsavel();

                        var coordenador = db.PessoaFisica.Where(w => w.IdPessoaFisica == itemCoordResp.IdPessoa).FirstOrDefault();

                        itemCoord.IdPessoaFisica = coordenador.IdPessoaFisica;
                        itemCoord.NmPessoa = coordenador.NmPessoa;
                        if (!string.IsNullOrEmpty(itemCoordResp.IdTipoCoordenacao.ToString()))
                        {
                            itemCoord.IdTipoCoordenacao = itemCoordResp.IdTipoCoordenacao.Value;
                        }

                        itemContrato.Coordenadores.Add(itemCoord);
                    }


                    foreach (var pagadores in retornoClientes)
                    {
                        var itemCli = new OutPutCliente();
                        var coordenador = db.PessoaFisica.Where(w => w.IdPessoaFisica == pagadores.IdCliente).FirstOrDefault();

                        if (pagadores.IcPagador != null)
                        {
                            itemCli.IcPagador = (bool)pagadores.IcPagador;
                            itemCli.IcSomentePagador = pagadores.IcSomentePagador;
                            if ((bool)pagadores.IcPagador)
                            {
                                itemContrato.ClientesPagadores.Add(Convert.ToInt32(pagadores.IdContratoCliente));
                            }
                        }
                    }

                    itemContrato.DsTextoCorpoNf = retornoContrato.DsObservacao;
                    itemContrato.IdContaCorrente = retornoContrato.IdContaCorrente;

                    if (retornoContrato.DsPrazoPagamento == null)
                    {
                        itemContrato.DsPrazoPagamento = retornoContrato.DsPrazoPagamento;
                    }
                    else
                    {
                        itemContrato.DsPrazoPagamento = retornoContrato.DsPrazoPagamento;
                    }
                    if (retornoContrato.NuAgencia == null)
                    {
                        itemContrato.NuAgencia = retornoContrato.NuAgencia;
                    }
                    else
                    {
                        itemContrato.NuAgencia = retornoContrato.NuAgencia;
                    }
                    if (retornoContrato.NuBanco == null)
                    {
                        itemContrato.NuBanco = retornoContrato.NuBanco;
                    }
                    else
                    {
                        itemContrato.NuBanco = retornoContrato.NuBanco;
                    }
                    if (retornoContrato.NuConta == null)
                    {
                        itemContrato.NuConta = retornoContrato.NuConta;
                    }
                    else
                    {
                        itemContrato.NuConta = retornoContrato.NuConta;
                    }
                    if (retornoContrato.IcFatAprovEntregavel == null)
                    {
                        itemContrato.IcFatAprovEntregavel = "nao";
                    }
                    else
                    {
                        itemContrato.IcFatAprovEntregavel = retornoContrato.IcFatAprovEntregavel;
                    }
                    if (retornoContrato.IcFatPedidoEmpenho == null)
                    {
                        itemContrato.IcFatPedidoEmpenho = "nao";
                    }
                    else
                    {
                        itemContrato.IcFatPedidoEmpenho = retornoContrato.IcFatPedidoEmpenho;
                    }
                    itemContrato.IdTipoEntregaDocumento = retornoContrato.IdTipoEntregaDocumento;
                    if (retornoContrato.IdTipoCobranca == null)
                    {
                        itemContrato.IdTipoCobranca = 1;
                    }
                    else
                    {
                        itemContrato.IdTipoCobranca = retornoContrato.IdTipoCobranca;
                    }
                    if (retornoContrato.IdFormaPagamento == null)
                    {
                        itemContrato.IdFormaPagamento = 0;
                    }
                    else
                    {
                        itemContrato.IdFormaPagamento = retornoContrato.IdFormaPagamento;
                    }
                    if (retornoContrato.IdTipoEntregaDocumento == null)
                    {
                        itemContrato.IdTipoEntregaDocumento = 0;
                    }
                    else
                    {
                        itemContrato.IdTipoEntregaDocumento = retornoContrato.IdTipoEntregaDocumento;
                    }
                    itemContrato.DocAcompanhaNFs = new List<InputUpdateContratoDocsAcompanhaNF>();
                    if (retornoContrato.ContratoDocsAcompanhaNf.Count == 0)
                    {

                        var listaTipoDocsAconpanhaNF = db.TipoDocsAcompanhaNf.Where(w => w.IcPadrao == true).ToList();

                        foreach (var item in listaTipoDocsAconpanhaNF)
                        {
                            var itemDocAcompanhaNF = new InputUpdateContratoDocsAcompanhaNF();

                            itemDocAcompanhaNF.IdTipoDocsAcompanhaNF = item.IdTipoDocsAcompanhaNf;
                            itemDocAcompanhaNF.DsTipoDocsAcompanhaNF = db.TipoDocsAcompanhaNf.Where(w => w.IdTipoDocsAcompanhaNf == item.IdTipoDocsAcompanhaNf).FirstOrDefault().DsTipoDocsAcompanhaNf;
                            itemContrato.DocAcompanhaNFs.Add(itemDocAcompanhaNF);
                        }
                    }
                    else
                    {
                        foreach (var item in retornoContrato.ContratoDocsAcompanhaNf)
                        {
                            var itemDocAcompanhaNF = new InputUpdateContratoDocsAcompanhaNF();

                            itemDocAcompanhaNF.IdContrato = retornoContrato.IdContrato;
                            itemDocAcompanhaNF.IdTipoDocsAcompanhaNF = item.IdTipoDocsAcompanhaNf;
                            itemDocAcompanhaNF.DsTipoDocsAcompanhaNF = db.TipoDocsAcompanhaNf.Where(w => w.IdTipoDocsAcompanhaNf == item.IdTipoDocsAcompanhaNf).FirstOrDefault().DsTipoDocsAcompanhaNf;

                            itemContrato.DocAcompanhaNFs.Add(itemDocAcompanhaNF);
                        }
                    }
                    //EGS 30.12.2020 Se contrato é de abono, dai nao obriga ter valor
                    itemContrato.chkContratoAbono = retornoContrato.chkContratoAbono;

                }
                return itemContrato;
            }
            catch (Exception exx)
            {
                new bEmail(db).EnviarEmailTratamentoErro(exx, "bContrato-GetContratoById [" + id + "]");
                throw;
            }
        }





        public OutPutVerificaContratoGerado VerificaContratoGerado(int id)
        {
            var retorno = new OutPutVerificaContratoGerado();
            retorno.Result = false;

            try
            {
                var retornoContrato = db.Contrato.Where(w => w.IdProposta == id).FirstOrDefault();
                if (retornoContrato != null)
                {
                    retorno.Result = true;
                }

                return retorno;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool VerificaContratoExiste(int id)
        {
            try
            {
                var retornoContrato = db.Contrato.Where(w => w.IdContrato == id).FirstOrDefault();
                if (retornoContrato != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int VerificaNuContratoEditExiste(string id)
        {
            try
            {
                //EGS 30.05.2020 - Estava pesquisando o numero inteiro do contrato, coloquei para pesquisar por qualquer parte
                //var retornoContrato = db.Contrato.Where(w => w.IdContrato == id).FirstOrDefault();
                var retornoContrato = db.Contrato.Where(w => w.NuContratoEdit.ToString().Contains(id.ToString())).FirstOrDefault();

                if (retornoContrato != null)
                {
                    return retornoContrato.IdContrato;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool VerificaNuContratoExiste(int idContrato, string nuContratoEdit)
        {
            try
            {
                var retornoContrato = db.Contrato.Where(w => w.NuContratoEdit == nuContratoEdit.ToString() && w.IdContrato != idContrato).FirstOrDefault();

                if (retornoContrato != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool VerificaAtivarContrato(int id)
        {
            try
            {
                var contrato = db.Contrato.Where(w => w.IdContrato == id).FirstOrDefault();
                decimal valorTotalParcelas = 0;
                var lstCronogramaFinan = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == id).ToList();
                foreach (var cronogramaFinan in lstCronogramaFinan)
                {
                    valorTotalParcelas += cronogramaFinan.VlParcela;
                }

                //EGS 30.12.2020 Se os valores baterem, ou for Abono, permite ativar o contrato
                if ((contrato.VlContrato == valorTotalParcelas) || (contrato.chkContratoAbono==true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Contrato> ListaContratosPorSituacao(int idSituacao)
        {
            var item = new List<Contrato>();

            if (idSituacao == 19 || idSituacao == 111 || idSituacao == 112)
            {
                item = db.Contrato.Where(w => w.IdSituacao == idSituacao && w.IcInformacoesIncompletas != true || w.IdSituacao == 111 || w.IdSituacao == 112).ToList();
            }
            else if (idSituacao == 18)
            {
                item = db.Contrato.Where(w => w.IdSituacao == idSituacao && w.IcInformacoesIncompletas != true).ToList();
            }
            else if (idSituacao == 110)
            {
                item = db.Contrato.Where(w => w.IdSituacao == idSituacao).ToList();
            }
            else
            {
                item = db.Contrato.Where(w => w.IdSituacao == idSituacao).ToList();
            }

            return item;
        }

        public List<Contrato> ListaContratos()
        {
          //var item = db.Contrato.ToList();                                            //EGS 12.06.2020 Ordenar por IDProposta recente
            var item = db.Contrato.OrderByDescending(w => w.IdContrato).ToList();

            return item;
        }
        public List<Contrato> ListaContratosAtivos()
        {
            var item = db.Contrato.Where(w => w.IdSituacao == 19).ToList();

            return item;
        }

        public List<ContratoEntregavel> ListaEntregaveisEmAtraso()
        {
            var contratos = db.Contrato.Where(w => w.IdSituacao == 19).ToList();
            var listContratoEntregavel = new List<ContratoEntregavel>();
            foreach (var item in contratos)
            {
                var listContratosEntregaveisAtrasado = db.ContratoEntregavel.Where(w => w.IdContrato == item.IdContrato && (w.DtProduto.HasValue && w.DtProduto.Value.Date < DateTime.Now.Date)).ToList();

                if (listContratosEntregaveisAtrasado.Count > 0)
                {
                    foreach (var itemEntregavel in listContratosEntregaveisAtrasado)
                    {
                        var situacao = new bSituacao(db).GetById(itemEntregavel.IdSituacao);
                        if (situacao.IcEntidade == "E" && situacao.IcEntregue == false)
                        {
                            listContratoEntregavel.Add(itemEntregavel);
                        }
                    }
                }
            }

            return listContratoEntregavel;
        }

        public List<ContratoCronogramaFinanceiro> ListaFaturamentosEmAtraso()
        {
            var contratos = db.Contrato.Where(w => w.IdSituacao == 19).ToList();
            var listCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();
            foreach (var item in contratos)
            {
                var listContratosFaturamentoAtrasado = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == item.IdContrato && w.DtFaturamento.HasValue && w.DtFaturamento.Value.Date < DateTime.Now.Date).ToList();
                if (listContratosFaturamentoAtrasado.Count > 0)
                {
                    foreach (var itemCronograma in listContratosFaturamentoAtrasado)
                    {
                        var situacao = new bSituacao(db).GetById(itemCronograma.IdSituacao);
                        if (situacao.IcEntidade == "F" && situacao.IcNfemitida == false)
                        {
                            listCronogramaFinanceiro.Add(itemCronograma);
                        }
                    }
                }
            }

            return listCronogramaFinanceiro;
        }

        public List<ContratoEntregavel> ListaEntregaveisAteData(DateTime data)
        {
            var contratos = db.Contrato.Where(w => w.IdSituacao == 19).ToList();
            var listContratoEntregavel = new List<ContratoEntregavel>();
            foreach (var item in contratos)
            {
                var listContratosEntregaveisAtrasado = db.ContratoEntregavel.Where(w => w.IdContrato == item.IdContrato && (w.DtProduto.HasValue && w.DtProduto.Value.Date <= data.Date && w.DtProduto.Value.Date >= DateTime.Now.Date)).ToList();
                if (listContratosEntregaveisAtrasado.Count > 0)
                {
                    foreach (var itemEntregavel in listContratosEntregaveisAtrasado)
                    {
                        var situacao = new bSituacao(db).GetById(itemEntregavel.IdSituacao);
                        if (situacao.IcEntidade == "E" && situacao.IcEntregue == false)
                        {
                            listContratoEntregavel.Add(itemEntregavel);
                        }
                    }
                }
            }

            return listContratoEntregavel;
        }

        public List<ContratoCronogramaFinanceiro> ListaFaturamentosAteData(DateTime data)
        {
            var contratos = db.Contrato.Where(w => w.IdSituacao == 19).ToList();
            var listCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();
            foreach (var item in contratos)
            {
                var listContratosFaturamentoAtrasado = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == item.IdContrato && w.DtFaturamento.HasValue && w.DtFaturamento.Value.Date <= data.Date && w.DtFaturamento.Value.Date >= DateTime.Now.Date).ToList();
                if (listContratosFaturamentoAtrasado.Count > 0)
                {
                    foreach (var itemCronograma in listContratosFaturamentoAtrasado)
                    {
                        var situacao = new bSituacao(db).GetById(itemCronograma.IdSituacao);
                        if (situacao.IcEntidade == "F" && situacao.IcNfemitida == false)
                        {
                            listCronogramaFinanceiro.Add(itemCronograma);
                        }
                    }
                }
            }

            return listCronogramaFinanceiro;
        }

        // lista contrato reajuste 
        public List<Contrato> ListaContratoReajuste(DateTime data)
        {
            var contratos = db.Contrato
                .Where(w => w.IcReajuste == true && w.DtProxReajuste <= data && w.IdSituacao == 19)
                .ToList();

            return contratos;
        }

        // Lista de Aditivos de Contratos
        public List<ContratoAditivo> ListaAditivos(int idContrato)
        {
            if (idContrato == 0)
            {
                var aditivos = db.ContratoAditivo.Where(w=>w.IdSituacao == 102)
                .Include(w => w.IdTipoAditivoNavigation)
                .Include(w => w.IdSituacaoNavigation)
                .Include(w => w.IdContratoNavigation)                
                .ToList();

                return aditivos;
            }
            else
            {
                var aditivos = db.ContratoAditivo
                .Include(w => w.IdTipoAditivoNavigation)
                .Include(w => w.IdSituacaoNavigation)
                .Include(w => w.IdContratoNavigation)
                .Where(w => w.IdContrato == idContrato)
                .ToList();

                return aditivos;
            }
        }



        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Setembro/2020 
         *  Pesquisa os nomes dos arquivos de ADITIVO do Contrato pelo ID
         ===========================================================================================*/
        public OutPutDadosUltimoAditivo BuscaDadosUltimoAditivo(int idContrato)
        {
            OutPutDadosUltimoAditivo lstUltimoAditivoAtivo = new OutPutDadosUltimoAditivo();

            //EGS 30.03.2021 Pega dados do aditivo
            var itemAditivo = db.ContratoAditivo.Where(w => w.IdContrato == idContrato && w.IdSituacao == 101 /*Ativo*/).OrderByDescending(x => x.IdContratoAditivo).FirstOrDefault();
            if (itemAditivo != null)
            {
                lstUltimoAditivoAtivo.advIdContrato              = itemAditivo.IdContrato;
                lstUltimoAditivoAtivo.advIdContratoAditivo       = itemAditivo.IdContratoAditivo;
                lstUltimoAditivoAtivo.advIdNumeroAditivo         = itemAditivo.NuAditivo;
                lstUltimoAditivoAtivo.advIdNumeroAditivoCliente  = itemAditivo.NuAditivoCliente;
                lstUltimoAditivoAtivo.advValorAditivo            = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0,0.00}", itemAditivo.VlAditivo);
                lstUltimoAditivoAtivo.advValorContratoOriginal   = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0,0.00}", itemAditivo.VlContrato);
                lstUltimoAditivoAtivo.advValorContratoAditivo    = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0,0.00}", itemAditivo.VlContratoAditivado);
                lstUltimoAditivoAtivo.advDtInicioVigênciaAditivo = itemAditivo.DtIniExecucaoAditivo.ToString().Substring(0, 10); 
                lstUltimoAditivoAtivo.advDtFinalVigênciaAditivo  = itemAditivo.DtFimAditivada.ToString().Substring(0, 10);
                lstUltimoAditivoAtivo.advDtCriacaoAditivo        = itemAditivo.DtCriacao.ToString();
                lstUltimoAditivoAtivo.advDtInicioAditivo         = itemAditivo.DtInicio.ToString().Substring(0,10);
                lstUltimoAditivoAtivo.advDtFinalAditivo          = itemAditivo.DtFim.ToString().Substring(0, 10);
                lstUltimoAditivoAtivo.advDsAditivo               = itemAditivo.DsAditivo;
                lstUltimoAditivoAtivo.IcAditivoValor             = itemAditivo.IcAditivoValor;
                lstUltimoAditivoAtivo.IcAditivoData              = itemAditivo.IcAditivoData;
                lstUltimoAditivoAtivo.IcAditivoEscopo            = itemAditivo.IcAditivoEscopo;
                lstUltimoAditivoAtivo.icAditivoOutro             = itemAditivo.IcAditivoRetRat;
                lstUltimoAditivoAtivo.bPossuiAditivo             = true;
            }
            return lstUltimoAditivoAtivo;
        }





        public List<Contrato> GetContratoFinanceiro()
        {
            var contrato = db.Contrato
                .Where(p => p.IdSituacao == 35)
                .ToList();

            return contrato;
        }

        public Contrato BuscarContratoId(int id)
        {
            var item = db.Contrato
                .Include(i => i.ContratoCoordenador)
                .Include(i => i.ContratoContatos)
                .Include(i => i.ContratoDocsAcompanhaNf)
                //.Include(i => i.ContratoDocPrincipal)
                .Where(w => w.IdContrato == id)
                .FirstOrDefault();

            return item;
        }


        public List<ContratoCliente> BuscarClientes(int id)
        {
            var item = db.ContratoCliente
                 .Include(w => w.IdContratoNavigation.ContratoCliente)
                .Where(w => w.IdContrato == id)
                .ToList();
            return item;
        }

        public void RemoveContrato(int id)
        {

            ExcluirCoordenador(id);

            var itemContrato = BuscarContratoId(id);
            var itemDocs = db.ContratoDocPrincipal.Where(w => w.IdContrato == id).ToList();
            var itemContatos = db.ContratoContatos.Where(w => w.IdContrato == id).ToList();
            var proposta = new bProposta(db).BuscarPropostaId(itemContrato.IdProposta);
            proposta.IdSituacao = 31;

            db.ContratoDocPrincipal.RemoveRange(itemDocs); //remove documentos principais
            db.ContratoContatos.RemoveRange(itemContatos); // remove contatos

            var oldPropostaHist = new bProposta(db).GetPropostaHistBySituacao(proposta.IdProposta);

            //EGS 30.03.2021 Valida Historico da Proposta
            new bProposta(db)._ValidaDataFinalHistorico(proposta.IdProposta);

            var propostaHist         = new PropostaHistorico();
            propostaHist.IdSituacao  = 31;
            propostaHist.DtInicio    = DateTime.Now;
            propostaHist.IdUsuario   = AppSettings.constGlobalUserID;
            propostaHist.IdProposta  = proposta.IdProposta;


            db.PropostaHistorico.Add(propostaHist);

            db.Contrato.Remove(itemContrato);

            db.SaveChanges();

        }

        // Carrega grid Entregaveis
        public List<ContratoEntregavel> GetEntregaveisByData(DateTime data)
        {
            var listaEntregaveis = db.ContratoEntregavel
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdFrenteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.DtProduto.Value.Date <= data.Date && w.DtProduto.Value.Date >= DateTime.Now.Date).ToList();

            return listaEntregaveis;
        }

        public List<ContratoEntregavel> GetEntregaveisEmAtraso(DateTime data)
        {
            var listaEntregaveis = db.ContratoEntregavel
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdFrenteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.DtProduto < data).ToList();

            return listaEntregaveis;
        }

        // Carrega grid Faturamentos
        public List<ContratoCronogramaFinanceiro> GetFaturamentosByData(DateTime data)
        {
            var listaEntregaveis = db.ContratoCronogramaFinanceiro
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdContratoClienteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.DtFaturamento.Value.Date <= data.Date && w.DtFaturamento.Value.Date >= DateTime.Now.Date).ToList();

            return listaEntregaveis;
        }

        public List<ContratoCronogramaFinanceiro> GetFaturamentosEmAtraso(DateTime data)
        {
            var listaEntregaveis = db.ContratoCronogramaFinanceiro
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdContratoClienteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.DtFaturamento <= data).ToList();

            return listaEntregaveis;
        }

        // tomador de serviço NF
        public ContratoCronogramaFinanceiro GetByIdCronogramaFinanceiro(int id)
        {
            var faturamentoNF = db.ContratoCronogramaFinanceiro
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdContratoClienteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.IdContratoCronFinanceiro == id).FirstOrDefault();

            return faturamentoNF;
        }

        public ContratoCronogramaFinanceiroTemporaria GetByIdCronogramaFinanceiroTemporaria(int id)
        {
            var faturamentoNF = db.ContratoCronogramaFinanceiroTemporaria
                .Include(i => i.IdSituacaoNavigation)
                .Include(i => i.IdContratoClienteNavigation)
                .Include(i => i.IdContratoNavigation)
                .Where(w => w.IdContratoCronFinanceiro == id).FirstOrDefault();

            return faturamentoNF;
        }
        public bool UpdateNfTomadorServico(ContratoCronogramaFinanceiro item)
        {
            try
            {
                db.ContratoCronogramaFinanceiro.Update(item);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region Renovação Automática

        public int AddProrrogacao(ContratoProrrogacao item)
        {
            int? idRenovacao = null;
            try
            {                
                db.ContratoProrrogacao.Add(item);
                db.SaveChanges();
                idRenovacao = item.IdContratoRenovacao;

                return idRenovacao.Value;
            }
            catch (Exception ex)
            {
                return idRenovacao.Value;
            }
        }

        public bool UpDateRenovacao(ContratoProrrogacao item)
        {            
            try
            {
                db.ContratoProrrogacao.Update(item);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion


        #region Métodos Comentarios

        public ContratoComentario GetComentarioById(int id)
        {
            var item = db.ContratoComentario.Where(w => w.IdContratoComentario == id).FirstOrDefault();

            return item;
        }

        public void AddComentario(ContratoComentario item)
        {

            db.ContratoComentario.Add(item);
            db.SaveChanges();

        }

        public bool UpdateComentario(ContratoComentario item)
        {

            try
            {
                db.ContratoComentario.Update(item);

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void AddComentarioHistorico(ContratoComentarioLido item)
        {

            db.ContratoComentarioLido.Add(item);
            db.SaveChanges();

        }

        public List<ContratoComentario> BuscarComentario(int id)
        {
            var itens = db.ContratoComentario.Where(w => w.IdContrato == id).ToList();

            return itens;
        }

        public List<ContratoComentario> ComentarioDiretoria(int id, int contrato)
        {
            var itens = db.ContratoComentario.Where(w => w.IdUsuario == id && w.IdContrato == contrato && w.DtComentario.Date == DateTime.Now.Date).ToList();

            return itens;
        }

        public List<ContratoComentario> ListaComentarioGestorContrato(int id, int contrato)
        {
            var itens = db.ContratoComentario.Where(w => w.IdUsuario == id && w.IdContrato == contrato && w.DtComentario.Date == DateTime.Now.Date).ToList();

            return itens;
        }

        #endregion

        #region Métodos Coordenadores
        public void AddContratoCoord(List<ContratoCoordenador> itensCoord)
        {
            db.ContratoCoordenador.AddRange(itensCoord);

            db.SaveChanges();
        }
        public List<PessoaFisica> BuscarCoordenadores()
        {
            var itens = db.PessoaFisica.ToList();

            return itens;
        }

        public void ExcluirCoordenador(int id)
        {

            var itens = db.ContratoCoordenador.Where(w => w.IdContrato == id).ToList();

            foreach (var item in itens)
            {
                db.ContratoCoordenador.Remove(item);
            }

            db.SaveChanges();

        }

        private void SalvaContratoCoordenador(InputUpdateContrato item)
        {
            if (item.Coordenadores.Count > 0)
            {
                foreach (var contratoCoord in item.Coordenadores)
                {
                    var contratoCoordenador = new bContratoCoordenador(db).GetByContratoPessoa(item.IdContrato, contratoCoord.IdPessoaFisica);

                    if (contratoCoordenador == null)
                    {
                        var newContratoCoordenador = new ContratoCoordenador();
                        newContratoCoordenador.IdContrato = item.IdContrato;
                        newContratoCoordenador.IdPessoa = contratoCoord.IdPessoaFisica;
                        newContratoCoordenador.IdTipoCoordenacao = contratoCoord.IdTipoCoordenacao;

                        db.ContratoCoordenador.Add(newContratoCoordenador);
                    }
                    else
                    {
                        contratoCoordenador.IdTipoCoordenacao = contratoCoord.IdTipoCoordenacao;

                        db.ContratoCoordenador.Update(contratoCoordenador);
                    }
                }
            }

            //  Exclui os Coordenadores que não fazem mais parte da Proposta
            var allContratoCoord = db.ContratoCoordenador
                .Where(w => w.IdContrato == item.IdContrato)
                .ToList();
            foreach (var c in allContratoCoord)
            {
                var contratoCoord = item.Coordenadores
                    .Where(w => w.IdPessoaFisica == c.IdPessoa)
                    .FirstOrDefault();

                if (contratoCoord == null)
                {
                    db.ContratoCoordenador.Remove(c);
                }
            }
        }
        #endregion

        #region Métodos Contatos
        public ContratoContatos BuscarContatoId(int id)
        {
            var item = db.ContratoContatos.Where(w => w.IdContratoContato == id).FirstOrDefault();

            return item;
        }

        public List<ContratoContatos> BuscarContatos(int id)
        {
            var itens = db.ContratoContatos.Where(w => w.IdContrato == id).ToList();

            return itens;
        }

        public OutPutContatoContrato NovoContato(InputAddContatoContrato item)
        {
            var retorno = new OutPutContatoContrato();
            var contato = new ContratoContatos();

            contato.NmContato = item.NmContato;
            contato.CdEmail = item.CdEmail;
            contato.NuTelefone = item.NuTelefone;
            contato.NuCelular = item.NuCelular;
            contato.NmDepartamento = item.NmDepartamento;
            contato.IdContrato = item.IdContrato;
            contato.IdTipoContato = item.IdTipoContato != 0 ? item.IdTipoContato : null;
            contato.DsEndereco = item.DsEndereco;
            contato.IdContratoCliente = item.IdContratoCliente != 0 ? item.IdContratoCliente : null;
            contato.IcApareceFichaResumo = item.IcApareceFichaResumo;

            db.ContratoContatos.Add(contato);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public OutPutContatoContrato UpdateContato(InputAddContatoContrato item)
        {
            var itemContato = BuscarContatoId((Int32)item.IdContratoContato);
            var retorno = new OutPutContatoContrato();
            itemContato.IdContratoContato = (Int32)item.IdContratoContato;
            itemContato.NmContato = item.NmContato;
            itemContato.NuCelular = item.NuCelular;
            itemContato.NuTelefone = item.NuTelefone;
            itemContato.NmDepartamento = item.NmDepartamento;
            itemContato.CdEmail = item.CdEmail;
            itemContato.IdTipoContato = item.IdTipoContato != 0 ? item.IdTipoContato : null;
            itemContato.DsEndereco = item.DsEndereco;
            itemContato.IdContratoCliente = item.IdContratoCliente != 0 ? item.IdContratoCliente : null;
            itemContato.IcApareceFichaResumo = item.IcApareceFichaResumo;

            db.SaveChanges();
            return retorno;
        }

        public void RemoverContato(int id)
        {
            var itemContato = db.ContratoContatos.FirstOrDefault(_ => _.IdContratoContato == id);

            if (itemContato != null)
            {
                db.ContratoContatos.Remove(itemContato);
                db.SaveChanges();
            }
        }

        public void AddContato(ContratoContatos item)
        {
            db.ContratoContatos.Add(item);
            db.SaveChanges();
            var idCli = item.IdContratoContato;
        }

        #endregion

        #region Métodos de Situação
        public List<Situacao> BuscarSituacao()
        {
            var itens = db.Situacao.Where(w => w.IcEntidade == "C").ToList();

            return itens;
        }
        #endregion

        #region Métodos de Documento

        public void AddDocumento(ContratoDoc item)
        {
            db.ContratoDoc.Add(item);
            db.SaveChanges();
        }

        public List<ContratoDoc> BuscarDocumentos(int id, bool? docContratual)
        {
            var itens = db.ContratoDoc.Where(w => w.IdContrato == id).ToList();
            return itens;
        }

        public List<ContratoDocPrincipal> BuscarDocumentosContratuais(int id)
        {
            var itens = db.ContratoDocPrincipal.Where(w => w.IdContrato == id).ToList();
            return itens;
        }

        public List<TipoDocumento> BuscarTipoDocumentos(int id, bool? docContratual)
        {
            if (docContratual == true && docContratual != null)
            {
                var itens = db.TipoDocumento.Where(w => w.IcDocContratual == true).OrderBy(w => w.DsTipoDoc).ToList();
                return itens;
            }
            else
            {
                var itens = db.TipoDocumento.Where(w => w.IdEntidade == 3 && w.IcDocContratual == false).OrderBy(w => w.DsTipoDoc).ToList();
                return itens;
            }

        }

        private void CopiaContratoDocsPrincipais(Proposta proposta, int idContrato)
        {
            var propostaDocsPrincipais = db.PropostaDocsPrincipais
                            .Where(w => w.IdProposta == proposta.IdProposta)
                            .ToList();

            foreach (var propostaDoc in propostaDocsPrincipais)
            {
                var contratoDocPrincipal = new ContratoDocPrincipal();
                contratoDocPrincipal.IdTipoDoc = propostaDoc.IdTipoDoc;
                contratoDocPrincipal.IdContrato = idContrato;
                contratoDocPrincipal.DocFisico = propostaDoc.DocFisico;
                contratoDocPrincipal.NmDocumento = propostaDoc.NmDocumento;
                contratoDocPrincipal.NmCriador = propostaDoc.NmCriador;
                contratoDocPrincipal.DsDoc = propostaDoc.NmDocumento;
                contratoDocPrincipal.DtUpLoad = propostaDoc.DtUpLoad;

                db.ContratoDocPrincipal.Add(contratoDocPrincipal);
            }

            db.SaveChanges();
        }

        public void RemoveDocumento(int id)
        {
            var itemDoc = db.ContratoDoc.Where(w => w.IdContratoDoc == id).FirstOrDefault();
            db.ContratoDoc.Remove(itemDoc);
            db.SaveChanges();
        }

        public ContratoDoc BuscarDocumentoId(int id)
        {
            var item = db.ContratoDoc
                .Where(w => w.IdContratoDoc == id).FirstOrDefault();
            return item;
        }


        public void AddDocumentoPrincipal(ContratoDocPrincipal item)
        {
            db.ContratoDocPrincipal.Add(item);
            db.SaveChanges();
        }

        public List<ContratoDocPrincipal> BuscarDocumentosPrincipais(int id)
        {
            var itens = db.ContratoDocPrincipal.Where(w => w.IdContrato == id).ToList();
            return itens;
        }

        public List<TipoDocumento> BuscarTipoDocumentosPrincipais()
        {
            var itens = db.TipoDocumento.ToList();
            return itens;
        }

        public ContratoDocPrincipal BuscarDocumentoPrincipalId(int id)
        {
            var item = db.ContratoDocPrincipal
                .Where(w => w.IdContratoDocPrincipal == id).FirstOrDefault();

            return item;
        }

        public void RemoveDocumentoPrincipal(int id)
        {

            var itemDoc = db.ContratoDocPrincipal.Where(w => w.IdContratoDocPrincipal == id).FirstOrDefault();

            db.ContratoDocPrincipal.Remove(itemDoc);

            db.SaveChanges();

        }
        #endregion

        #region Métodos auxiliares Proposta        
        //private void CopiaPropostaDocs(Proposta proposta)
        //{
        //    var oportunidadesDocs = db.OportunidadeDocs
        //                    .Where(w => w.IdOportunidade == proposta.IdOportunidade)
        //                    .ToList();

        //    foreach (var oportunidadeDoc in oportunidadesDocs)
        //    {
        //        var propostaDocs = new PropostaDocs();
        //        propostaDocs.IdTipoDoc = oportunidadeDoc.IdTipoDocumento;
        //        propostaDocs.IdProposta = proposta.IdProposta;
        //        propostaDocs.DocFisico = oportunidadeDoc.DocFisico;
        //        propostaDocs.NmDocumento = oportunidadeDoc.NmDocumento;
        //        propostaDocs.NmCriador = oportunidadeDoc.NmCriador;
        //        propostaDocs.DsDoc = oportunidadeDoc.NmDocumento;
        //        propostaDocs.DtUpLoad = oportunidadeDoc.DtUpLoad;

        //        db.PropostaDocs.Add(propostaDocs);
        //    }

        //    db.SaveChanges();
        //}

        private void CopiaContratoCliente(Contrato contrato)
        {
            var propostaClientes = db.PropostaCliente
                            .Where(w => w.IdProposta == contrato.IdProposta)
                            .OrderBy(w => String.IsNullOrEmpty(w.NmFantasia) ? w.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.NmPessoa : w.NmFantasia)
                            .ToList();
            int nuContratante = 0;

            foreach (var propostaCliente in propostaClientes)
            {
                nuContratante++;
                var contratoCliente = new ContratoCliente();
                contratoCliente.IdCliente = propostaCliente.IdCliente;
                contratoCliente.IdContrato = contrato.IdContrato;
                contratoCliente.NmFantasia = propostaCliente.NmFantasia;
                contratoCliente.RazaoSocial = propostaCliente.RazaoSocial;
                contratoCliente.IcPagador = false;
                contratoCliente.NuContratante = nuContratante;

                db.ContratoCliente.Add(contratoCliente);
            }
            db.SaveChanges();
        }
        private void CopiaContratoContato(Contrato contrato)
        {
            var propostaContatos = db.PropostaContato
                            .Where(w => w.IdProposta == contrato.IdProposta)
                            .ToList();

            foreach (var propostaContato in propostaContatos)
            {
                var contratoContato = new ContratoContatos();
                contratoContato.CdEmail = propostaContato.CdEmail;
                contratoContato.IdContrato = contrato.IdContrato;
                contratoContato.NmContato = propostaContato.NmContato;
                contratoContato.NuCelular = propostaContato.NuCelular;
                contratoContato.NuTelefone = propostaContato.NuTelefone;
                contratoContato.IdTipoContato = propostaContato.IdTipoContato;
                db.ContratoContatos.Add(contratoContato);
            }
            db.SaveChanges();
        }

        private void CopiaContratoCoordenador(Contrato contrato)
        {
            var propostasCoordenadores = db.PropostaCoordenador
                            .Where(w => w.IdProposta == contrato.IdProposta)
                            .ToList();

            foreach (var propostaCoordenador in propostasCoordenadores)
            {
                var contratoCoordenador = new ContratoCoordenador();
                contratoCoordenador.IdPessoa = (Int32)propostaCoordenador.IdPessoa;
                contratoCoordenador.IdContrato = contrato.IdContrato;

                db.ContratoCoordenador.Add(contratoCoordenador);
            }

            db.SaveChanges();
        }

        #endregion

        #region Método Histórico

        public List<ContratoHistorico> ListaHistoricoContrato(int idContrato)
        {
            var listaHistorico = db.ContratoHistorico
                .Include(i => i.IdSituacaoNavigation)
                .Where(w => w.IdContrato == idContrato).ToList();

            return listaHistorico;
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

        public class InputUpdateContrato
        {
            public int IdContrato { get; set; }
            public string NuContratoEdit { get; set; }
            public int IdProposta { get; set; }
            public int IdSituacao { get; set; }
            public short? IdIndiceReajuste { get; set; }
            public int IdFundamento { get; set; }
            public bool? IcOrdemInicio { get; set; }
            public bool? IcReajuste { get; set; }
            public bool? IcRenovacaoAutomatica { get; set; }
            public string NuContratoCliente { get; set; }
            public string DsAssunto { get; set; }
            public string DsObjeto { get; set; }
            public decimal VlContrato { get; set; }
            public string DsPrazoExecucao { get; set; }
            public DateTime? DtAssinatura { get; set; }
            public DateTime? DtInicio { get; set; }
            public DateTime? DtFim { get; set; }
            public DateTime? DtProxReajuste { get; set; }
            public string NuCentroCusto { get; set; }
            public string CdIss { get; set; }
            public int       IdUsuarioUltimaAlteracao { get; set; }
            public DateTime? DtUltimaAlteracao { get; set; }
            public int?      IdTema { get; set; }
            public DateTime? DtRenovacao { get; set; }
            public DateTime? DtAssinaturaContrato { get; set; }
            public string    DsObservacao { get; set; }
            public int[]     ClientesPagadores { get; set; }
            public List<OutputResponsavel> Coordenadores { get; set; }
            public DateTime? DtInicioExecucao            { get; set; }
            public DateTime? DtFimExecucao               { get; set; }
            public bool?     IcFrenteUnica               { get; set; }
            public bool?     IcContinuo                  { get; set; }
            public int?      IdArea                      { get; set; }
            public string    NuProcessoCliente           { get; set; }
            public bool?     IcInformacoesIncompletas    { get; set; }
            public bool?     IcViaFipeNaoAssinada        { get; set; }
            public List<OutPutGetCliente> clientes       { get; set; }
            public int       IdUsuarioConectado          { get; set; }     //EGS 30.08.2020 Usuario conectado no Angular
        }

        public class OutputResponsavel
        {
            public int IdPessoaFisica { get; set; }
            public string NmPessoa { get; set; }
            public int? IdTipoCoordenacao { get; set; }

        }

        public class OutPutGetContratoId
        {
            public int IdContrato { get; set; }
            public string NuContratoEdit { get; set; }
            public int IdProposta { get; set; }
            public int IdSituacao { get; set; }
            public short? IdTipoReajuste { get; set; }
            public int IdFundamento { get; set; }
            public int IdIndiceReajuste { get; set; }
            public bool? IcOrdemInicio { get; set; }
            public bool? IcReajuste { get; set; }
            public bool? IcPrazoIndeterminado { get; set; }
            public bool? IcFrenteUnica { get; set; }
            public bool? IcContinuo { get; set; }
            public string NuContratoCliente { get; set; }
            public string DsApelidoProposta { get; set; }
            public string DsAssunto { get; set; }
            public string DsObjeto { get; set; }
            public decimal VlContrato { get; set; }
            public string DsPrazoExecucao { get; set; }
            public DateTime? DtAssinatura { get; set; }
            public DateTime? DtInicio { get; set; }
            public DateTime? DtFim { get; set; }
            public DateTime? DtInicioExecucao { get; set; }
            public DateTime? DtFimExecucao { get; set; }
            public DateTime? DtProxReajuste { get; set; }
            public DateTime? DtCriacao { get; set; }
            public string NuCentroCusto { get; set; }
            public string CdIss { get; set; }
            public int IdUsuarioUltimaAlteracao { get; set; }
            public DateTime? DtUltimaAlteracao { get; set; }
            public int? IdTema { get; set; }
            public string NmUsuario { get; set; }
            public List<OutPutCliente> Clientes { get; set; }
            public DateTime? DtRenovacao { get; set; }
            public DateTime? DtAssinaturaContrato { get; set; }
            public string DsObservacao { get; set; }
            public int IdUsuarioCriacao { get; set; }
            public List<int> ClientesPagadores { get; set; }
            public List<OutputResponsavel> Coordenadores { get; set; }
            public string NuProcessoCliente { get; set; }
            public int? IdArea { get; set; }
            //Dados de Cobrança
            public string DsPrazoPagamento { get; set; }
            public string IcFatAprovEntregavel { get; set; }
            public string IcFatPedidoEmpenho { get; set; }
            public int? IdFormaPagamento { get; set; }
            public short? IdTipoEntregaDocumento { get; set; }
            public short? IdTipoCobranca { get; set; }
            public int? IdContaCorrente { get; set; }
            public string NuBanco { get; set; }
            public string NuAgencia { get; set; }
            public string NuConta                   { get; set; }
            public string DsTextoCorpoNf            { get; set; }
            public List<InputUpdateContratoDocsAcompanhaNF> DocAcompanhaNFs { get; set; }
            public bool? IcInformacoesIncompletas   { get; set; }
            public bool? IcViaFipeNaoAssinada       { get; set; }
            public bool? chkContratoAbono           { get; set; }   //EGS 30.12.2020 Se contrato é de abono, dai nao obriga ter valor
        }

        public class OutPutVerificaContratoGerado
        {
            public bool Result { get; set; }

        }

        public class OutPutVerificaContratoExiste
        {
            public bool Result { get; set; }
            public int idContratoEdit { get; set; }

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
            public bool Result             { get; set; }
            public string IdContrato       { get; set; }
            public bool   bCompletoSucesso { get; set; }
        }

        public class OutputUpdate
        {
            public bool Result { get; set; }
        }

        public class OutPutContatoContrato
        {
            public bool Result { get; set; }
            public int IdContratoContato { get; set; }

        }

        public class InputAddContatoContrato
        {
            public string NmContato { get; set; }
            public string CdEmail { get; set; }
            public string NuTelefone { get; set; }
            public string NuCelular { get; set; }
            public string NmDepartamento { get; set; }
            public int IdContrato { get; set; }
            public int? IdContratoContato { get; set; }
            public int? IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }
            public string DsEndereco { get; set; }
            public string NmContrante { get; set; }
            public int? IdContratoCliente { get; set; }
            public bool? IcApareceFichaResumo { get; set; }

        }

        #region Dados Cobrança
        public class InputUpdateContratoDadosCobranca
        {
            public int IdContrato { get; set; }
            public int IdTipoEntregaDocumento { get; set; }
            public int IdTipoCobranca { get; set; }
            public int IdFormaPagamento { get; set; }
            public int IcFatAprovEntregavel { get; set; }
            public int IcFatPedidoEmpenho { get; set; }
            public int DsPrazoPagamento { get; set; }
            public int DsTextoCorpoNF { get; set; }
            public List<InputUpdateContratoDocsAcompanhaNF> DocAcompanhaNFs { get; set; }
            public int NuBanco { get; set; }
            public int NuAgencia { get; set; }
            public int NuConta { get; set; }

        }
        public class InputUpdateContratoDocsAcompanhaNF
        {
            public int IdContrato { get; set; }
            public int IdTipoDocsAcompanhaNF { get; set; }
            public string DsTipoDocsAcompanhaNF { get; set; }
        }

        #endregion

        #endregion
    }
}