using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Controllers.PropostaController;
using static ApiFipe.Models.bContrato;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class ContratoController : ControllerBase
    {

        private readonly IHostingEnvironment _hostingEnvironment;

        public ContratoController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("Add")]
        public OutputAdd Add([FromBody] InputAddContrato item)
        {
            var retorno = new OutputAdd();

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
                            // Grava registro                    
                            var addRetorno = new bContrato(db).Add(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-Add");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPost]
        [Route("EnviaEmailGestorContratos")]
        public OutputEnviaEmailGestorContratos EnviaEmailGestorContratos([FromBody] InputEnviaEmailGestorContratos item)
        {
            var retorno = new OutputEnviaEmailGestorContratos();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    retorno.Result = new bContrato(db).EnviaEmailGestorContratos(item.IdProposta, item.IdContrato, item.Url, item.bCompletoSucesso);

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-EnviaEmailGestorContratos Proposta/Contrato [" + item.IdProposta + "/" + item.IdContrato + "]");

                    retorno.Result = false;

                    return retorno;
                }
            }
        }

        [HttpPut]
        [Route("Update")]
        public OutPutUpdateContrato Update([FromBody] InputUpdateContrato item)
        {
            var retorno = new OutPutUpdateContrato();

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
                            // Grava registro                    
                            new bContrato(db).Update(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-Update Contrato [" + item.IdProposta + "]");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }


        [HttpGet]
        [Route("ListaContratosHome/{idUsuario}")]
        public OutPutListaContratos ListaContratosHome(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    var usuario = new bUsuario(db).GetById(idUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    var contratos = new List<Contrato>();
                    if (perfilUsuario.IdPerfil == 7)
                    {
                        contratos = new bContrato(db).GetContratoFinanceiro();
                    }
                    else
                    {
                        contratos = new bContrato(db).ListaContratos();

                    }
                    foreach (var item in contratos)
                    {
                        if (item.IdSituacao == 19 && item.IcInformacoesIncompletas != true || item.IdSituacao == 111 || item.IdSituacao == 112)
                        {
                            retorno.TamanhoContratosAtivos += 1;
                        }
                        else if (item.IdSituacao == 110)
                        {
                            retorno.tamanhoContratosCanEncSus += 1;
                        }
                        else if (item.IdSituacao == 18 && item.IcInformacoesIncompletas != true)
                        {
                            retorno.TamanhoContrato += 1;
                        }

                        if (item.IcInformacoesIncompletas.Value == true)
                        {
                            retorno.TamanhoContratoInfoIncompleta += 1;
                        }                 
                    }
                                       
                   
                    retorno.TamanhoFaturamentoAtraso = new bContrato(db).ListaFaturamentosEmAtraso().Count;
                    retorno.TamanhoFaturamentosAteData = new bContrato(db).ListaFaturamentosAteData(DateTime.Now).Count;
                    retorno.TamanhoEntregaveisAtraso = new bContrato(db).ListaEntregaveisEmAtraso().Count;
                    retorno.TamanhoEntregaveisAteData = new bContrato(db).ListaEntregaveisAteData(DateTime.Now).Count;
                    retorno.TamanhoAditivo = new bContrato(db).ListaAditivos(0).Count();
                    if (retorno.TamanhoAditivo == 1)
                    {
                        retorno.IdContratoAditivo = new bContrato(db).ListaAditivos(0).FirstOrDefault().IdContratoAditivo;
                    }
                    retorno.TamanhoReajustes = new bContrato(db).ListaContratoReajuste(DateTime.Now).Count();

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosHome IdUsuario [" + idUsuario + "]");
                    throw;
                }
            }
        }





        [HttpGet]
        [Route("ListaContratos/{idUsuario}")]
        public OutPutListaContratos ListaContratos(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    
                    listaContratos = new bPesquisaGeral(db).GetContratos();
                    retorno.lstOutPutGetContratos = listaContratos;
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratos IdUsuario [" + idUsuario + "]");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosFipeNaoAssinado/{idUsuario}")]
        public OutPutListaContratos ListaContratosFipeNaoAssinado(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    var usuario = new bUsuario(db).GetById(idUsuario);
                    var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                    var contratos = new List<Contrato>();
                    if (perfilUsuario.IdPerfil == 7)
                    {
                        contratos = new bContrato(db).GetContratoFinanceiro();
                    }
                    else
                    {
                        contratos = new bContrato(db).ListaContratos().Where(w => w.IcViaFipeNaoAssinada == true).ToList();
                    }
                    foreach (var item in contratos)
                    {
                        if (item.IdSituacao == 19 && item.IcInformacoesIncompletas != true)
                        {
                            retorno.TamanhoContratosAtivos += 1;
                        }
                        else if (item.IdSituacao == 18 && item.IcInformacoesIncompletas != true)
                        {
                            retorno.TamanhoContrato += 1;
                        }

                        if (item.IcInformacoesIncompletas.Value != null && item.IcInformacoesIncompletas.Value == true)
                        {
                            retorno.TamanhoContratoInfoIncompleta += 1;
                        }
                        if (item.IcViaFipeNaoAssinada.Value)
                        {
                            retorno.TamanhoContratosFipeNaoAssinada += 1;
                        }
                        var itemContrato = new OutPutGetContratos();
                        itemContrato.clientes = new List<string>();
                        itemContrato.coordenadores = new List<string>();
                        var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                        foreach (var contratoCli in contratoClientes)
                        {
                            var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemContrato.clientes.Add(pessoaFisica.NmPessoa);
                                itemContrato.clientesTexto = itemContrato.clientesTexto + " " + pessoaFisica.NmPessoa;
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemContrato.clientes.Add(contratoCli.NmFantasia);
                                itemContrato.clientesTexto = itemContrato.clientesTexto + " " + contratoCli.NmFantasia;
                            }
                        }
                        var contratoCoordenadores = new bContratoCoordenador(db).ListaCoordenadorGrid(item.IdContrato);
                        foreach (var contratoCoord in contratoCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(contratoCoord.IdPessoa));
                            itemContrato.coordenadores.Add(coordenador.NmPessoa);
                            itemContrato.coordenadoresTexto = itemContrato.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }
                        var situcao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();

                        itemContrato.IdContrato = item.IdContrato;
                        itemContrato.NuContratoEdit = item.NuContratoEdit;
                        itemContrato.IdProposta = item.IdProposta;
                        itemContrato.VlContrato = item.VlContrato;
                        itemContrato.DsPrazoExecucao = item.DsPrazoExecucao;
                        itemContrato.DsCentroCusto = item.NuCentroCusto;    //EGS 30.05.2020 - Nova coluna solicitada por Nelson

                        if (itemContrato.DtAssinatura != null)
                        {
                            itemContrato.DtAssinatura = item.DtAssinatura.Value;
                        }
                        itemContrato.IcOrdemInicio = "Não";
                        itemContrato.IcRenovacaoAutomatica = "Não";
                        if (item.IcOrdemInicio == true)
                        {
                            itemContrato.IcOrdemInicio = "Sim";
                        }

                        if (item.IcRenovacaoAutomatica == true)
                        {
                            itemContrato.IcRenovacaoAutomatica = "Sim";
                        }

                        itemContrato.DsSituacao = situcao.DsSituacao;
                        listaContratos.Add(itemContrato);
                    }

                    retorno.lstOutPutGetContratos = listaContratos;
                    retorno.TamanhoFaturamentoAtraso = new bContrato(db).ListaFaturamentosEmAtraso().Count;
                    retorno.TamanhoFaturamentosAteData = new bContrato(db).ListaFaturamentosAteData(DateTime.Now).Count;
                    retorno.TamanhoEntregaveisAtraso = new bContrato(db).ListaEntregaveisEmAtraso().Count;
                    retorno.TamanhoEntregaveisAteData = new bContrato(db).ListaEntregaveisAteData(DateTime.Now).Count;
                    retorno.TamanhoAditivo = new bContrato(db).ListaAditivos(0).Count();
                    if (retorno.TamanhoAditivo == 1)
                    {
                        retorno.IdContratoAditivo = new bContrato(db).ListaAditivos(0).FirstOrDefault().IdContratoAditivo;
                    }
                    retorno.TamanhoReajustes = new bContrato(db).ListaContratoReajuste(DateTime.Now).Count();

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosFipeNaoAssinado");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosReajuste")]
        public List<OutPutListaContratosReajuste> ListaContratosReajuste()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {

                    var retorno = new List<OutPutListaContratosReajuste>();
                    var parametro = db.Parametro.FirstOrDefault();
                    var contratoReajuste = new bContrato(db).ListaContratos();
                    var dataReajuste = DateTime.Now.AddDays(parametro.NuDiasReajuste.Value);

                    foreach (var item in contratoReajuste)
                    {
                        if (item.DtProxReajuste != null)
                        {
                            if (item.DtProxReajuste.Value.Date <= dataReajuste.Date && item.IdSituacao == 19)
                            {
                                var itemContrato = new OutPutListaContratosReajuste();

                                itemContrato.Clientes = new List<string>();
                                var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                                foreach (var contratoCli in contratoClientes)
                                {
                                    var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                    var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                    if (pessoa.IdPessoaFisica != null)
                                    {
                                        var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                        itemContrato.Clientes.Add(pessoaFisica.NmPessoa);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + pessoaFisica.NmPessoa;
                                    }
                                    else if (pessoa.IdPessoaJuridica != null)
                                    {
                                        var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                        itemContrato.Clientes.Add(contratoCli.NmFantasia);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + contratoCli.NmFantasia;
                                    }
                                }
                                itemContrato.VlContrato = item.VlContrato;
                                itemContrato.IdContrato = item.IdContrato;
                                itemContrato.NuContratoEdit = item.NuContratoEdit;
                                if (item.DtInicio != null)
                                {
                                    itemContrato.DtInicio = item.DtInicio.Value;
                                }
                                if (item.DtFim != null)
                                {
                                    itemContrato.DtFim = item.DtFim.Value;
                                }
                                itemContrato.DsApelido = item.DsApelido;
                                if (item.IdIndiceReajuste != null)
                                {
                                    var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == item.IdIndiceReajuste).FirstOrDefault();
                                    itemContrato.IndiceReajuste = indiceReajuste != null ? indiceReajuste.DsIndiceReajuste : string.Empty;
                                }
                                itemContrato.DsSituacao = "Pendente";

                                retorno.Add(itemContrato);
                            }
                        }
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosReajuste");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("VerificaContratoReajuste/{idContrato}")]
        public OutPutGetVerificaContratoReajuste VerificaContratoReajuste(int idContrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutGetVerificaContratoReajuste();
                    var contrato = new bContrato(db).BuscarContratoId(idContrato);
                    var contratoReajuste = db.ContratoReajuste.Where(w => w.IdContrato == idContrato && w.DtReajuste == contrato.DtProxReajuste).FirstOrDefault();
                    var lstContratoReajuste = db.ContratoReajuste.Where(w => w.IdContrato == idContrato).ToList();
                    if (contratoReajuste != null)
                    {
                        retorno.IdContratoReajuste = contratoReajuste.IdContratoReajuste;
                    }
                    else
                    {
                        var newContratoReajuste = new ContratoReajuste();
                        newContratoReajuste.IdContrato = idContrato;
                        var nuReajuste = lstContratoReajuste.Count + 1;
                        newContratoReajuste.NuReajuste = Convert.ToInt16(nuReajuste);
                        newContratoReajuste.IdSituacao = 102;
                        newContratoReajuste.IcHistoricoCopiado = false;
                        newContratoReajuste.VlContratoAntesReajuste = contrato.VlContrato;
                        newContratoReajuste.IdIndiceReajuste = contrato.IdIndiceReajuste;
                        newContratoReajuste.IcReajuste = contrato.IcReajuste;
                        newContratoReajuste.DtReajuste = contrato.DtProxReajuste.Value;

                        db.ContratoReajuste.Add(newContratoReajuste);

                        db.SaveChanges();

                        var lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == contrato.IdContrato).OrderBy(w => w.IdContratoCronFinanceiro).ToList();

                        foreach (var cronogramaFinanceiro in lstCronogramaFinanceiro)
                        {
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
                        }

                        retorno.IdContratoReajuste = newContratoReajuste.IdContratoReajuste;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaContratoReajuste");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosInfoIncompleta")]
        public OutPutListaContratos ListaContratosInfoIncompleta()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    var contratos = new List<Contrato>();
                    contratos = new bContrato(db).ListaContratos();

                    foreach (var item in contratos)
                    {
                        if (item.IcInformacoesIncompletas.Value)
                        {
                            var itemContrato = new OutPutGetContratos();
                            itemContrato.clientes = new List<string>();
                            itemContrato.coordenadores = new List<string>();
                            var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                            foreach (var contratoCli in contratoClientes)
                            {
                                var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemContrato.clientes.Add(pessoaFisica.NmPessoa);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + pessoaFisica.NmPessoa;
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemContrato.clientes.Add(contratoCli.NmFantasia);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + contratoCli.NmFantasia;
                                }
                            }
                            var contratoCoordenadores = new bContratoCoordenador(db).ListaCoordenadorGrid(item.IdContrato);
                            foreach (var contratoCoord in contratoCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(contratoCoord.IdPessoa));
                                itemContrato.coordenadores.Add(coordenador.NmPessoa);
                                itemContrato.coordenadoresTexto = itemContrato.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }
                            var situcao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();

                            itemContrato.IdContrato = item.IdContrato;
                            itemContrato.NuContratoEdit = item.NuContratoEdit;
                            itemContrato.IdProposta = item.IdProposta;
                            itemContrato.VlContrato = item.VlContrato;
                            itemContrato.DsPrazoExecucao = item.DsPrazoExecucao;
                            itemContrato.DsCentroCusto = item.NuCentroCusto;    //EGS 30.05.2020 - Nova coluna solicitada por Nelson

                            if (itemContrato.DtAssinatura != null)
                            {
                                itemContrato.DtAssinatura = item.DtAssinatura.Value;
                            }
                            itemContrato.IcOrdemInicio = "Não";
                            itemContrato.IcRenovacaoAutomatica = "Não";
                            if (item.IcOrdemInicio == true)
                            {
                                itemContrato.IcOrdemInicio = "Sim";
                            }

                            if (item.IcRenovacaoAutomatica == true)
                            {
                                itemContrato.IcRenovacaoAutomatica = "Sim";
                            }

                            itemContrato.DsSituacao = situcao.DsSituacao;
                            listaContratos.Add(itemContrato);
                        }
                    }

                    retorno.lstOutPutGetContratos = listaContratos;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosInfoIncompleta");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosEncerramentoProximo")]
        public List<OutPutListaContratosEncerramentoProximo> ListaContratosEncerramentoProximo()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new List<OutPutListaContratosEncerramentoProximo>();
                    var listaContratos = new List<OutPutListaContratosEncerramentoProximo>();
                    var contratos = new List<Contrato>();
                    contratos = new bContrato(db).ListaContratosAtivos();
                    var parametro = db.Parametro.FirstOrDefault();

                    foreach (var item in contratos)
                    {
                        if (item.DtFim != null)
                        {
                            if (item.DtFim.Value.Date <= DateTime.Now.Date.AddDays(parametro.NuDiasEncerramentoContrato.Value) && item.IcRenovacaoAutomatica == false)
                            {
                                var itemContrato = new OutPutListaContratosEncerramentoProximo();
                                itemContrato.Clientes = new List<string>();
                                var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                                foreach (var contratoCli in contratoClientes)
                                {
                                    var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                    var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                    if (pessoa.IdPessoaFisica != null)
                                    {
                                        var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                        itemContrato.Clientes.Add(pessoaFisica.NmPessoa);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + pessoaFisica.NmPessoa;
                                    }
                                    else if (pessoa.IdPessoaJuridica != null)
                                    {
                                        var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                        itemContrato.Clientes.Add(contratoCli.NmFantasia);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + contratoCli.NmFantasia;
                                    }
                                }

                                itemContrato.IdContrato = item.IdContrato;
                                itemContrato.NuContratoEdit = item.NuContratoEdit;
                                if (item.DtInicio != null)
                                {
                                    itemContrato.DtInicio = item.DtInicio.Value;
                                }
                                if (item.DtFim != null)
                                {
                                    itemContrato.DtFim = item.DtFim.Value;
                                }
                                itemContrato.IcRenovacaoAutomatica = item.IcRenovacaoAutomatica;
                                itemContrato.IcReajuste = item.IcReajuste;
                                itemContrato.DsApelido = item.DsApelido;
                                itemContrato.NuCentroCusto = item.NuCentroCusto;
                                if (item.IdIndiceReajuste != null)
                                {
                                    var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == item.IdIndiceReajuste).FirstOrDefault();
                                    itemContrato.IndiceReajuste = indiceReajuste != null ? indiceReajuste.DsIndiceReajuste : string.Empty;
                                }

                                listaContratos.Add(itemContrato);
                            }
                        }

                        retorno = listaContratos;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosEncerramentoProximo");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosRenovacaoAutomatica")]
        public List<OutPutListaContratosRenovacaoAutomatica> ListaContratosRenovacaoAutomatica()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new List<OutPutListaContratosRenovacaoAutomatica>();
                    var listaContratos = new List<OutPutListaContratosRenovacaoAutomatica>();
                    var contratos = new List<Contrato>();
                    contratos = new bContrato(db).ListaContratosAtivos();
                    var parametro = db.Parametro.FirstOrDefault();

                    foreach (var item in contratos)
                    {
                        if (item.DtFim != null)
                        {
                            if (item.DtFim.Value.Date <= DateTime.Now.Date.AddDays(parametro.NuDiasRenovacao.Value) && item.IcRenovacaoAutomatica == true)
                            {
                                var itemContrato = new OutPutListaContratosRenovacaoAutomatica();
                                itemContrato.Clientes = new List<string>();
                                var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                                foreach (var contratoCli in contratoClientes)
                                {
                                    var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                    var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                    if (pessoa.IdPessoaFisica != null)
                                    {
                                        var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                        itemContrato.Clientes.Add(pessoaFisica.NmPessoa);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + pessoaFisica.NmPessoa;
                                    }
                                    else if (pessoa.IdPessoaJuridica != null)
                                    {
                                        var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                        itemContrato.Clientes.Add(contratoCli.NmFantasia);
                                        itemContrato.ClientesTexto = itemContrato.ClientesTexto + " " + contratoCli.NmFantasia;
                                    }
                                }

                                itemContrato.IdContrato = item.IdContrato;
                                itemContrato.NuContratoEdit = item.NuContratoEdit;
                                itemContrato.VlContrato = item.VlContrato;
                                if (item.DtInicio != null)
                                {
                                    itemContrato.DtInicio = item.DtInicio.Value;
                                }
                                if (item.DtFim != null)
                                {
                                    itemContrato.DtFim = item.DtFim.Value;
                                }
                                itemContrato.IcRenovacaoAutomatica = item.IcRenovacaoAutomatica;
                                itemContrato.IcReajuste = item.IcReajuste;
                                itemContrato.DsApelido = item.DsApelido;
                                itemContrato.NuCentroCusto = item.NuCentroCusto;
                                if (item.IdIndiceReajuste != null)
                                {
                                    var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == item.IdIndiceReajuste).FirstOrDefault();
                                    itemContrato.IndiceReajuste = indiceReajuste != null ? indiceReajuste.DsIndiceReajuste : string.Empty;
                                }

                                listaContratos.Add(itemContrato);
                            }
                        }

                        retorno = listaContratos;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosRenovacaoAutomatica");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosParaDefinicaoEquipeTec")]
        public OutPutListaContratos ListaContratosParaDefinicaoEquipeTec()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    var contratos = new List<Contrato>();
                    contratos = new bContrato(db).ListaContratos();

                    foreach (var item in contratos)
                    {
                        var equipeTecnica = db.ContratoEquipeTecnica.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();
                        if (equipeTecnica == null)
                        {
                            var itemContrato = new OutPutGetContratos();
                            itemContrato.clientes = new List<string>();
                            itemContrato.coordenadores = new List<string>();
                            var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                            foreach (var contratoCli in contratoClientes)
                            {
                                var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemContrato.clientes.Add(pessoaFisica.NmPessoa);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + pessoaFisica.NmPessoa;
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemContrato.clientes.Add(contratoCli.NmFantasia);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + contratoCli.NmFantasia;
                                }
                            }
                            var contratoCoordenadores = new bContratoCoordenador(db).ListaCoordenadorGrid(item.IdContrato);
                            foreach (var contratoCoord in contratoCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(contratoCoord.IdPessoa));
                                itemContrato.coordenadores.Add(coordenador.NmPessoa);
                                itemContrato.coordenadoresTexto = itemContrato.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }
                            var situcao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();

                            itemContrato.IdContrato = item.IdContrato;
                            itemContrato.NuContratoEdit = item.NuContratoEdit;
                            itemContrato.IdProposta = item.IdProposta;
                            itemContrato.VlContrato = item.VlContrato;
                            itemContrato.DsPrazoExecucao = item.DsPrazoExecucao;
                            itemContrato.DsCentroCusto = item.NuCentroCusto;    //EGS 30.05.2020 - Nova coluna solicitada por Nelson

                            if (itemContrato.DtAssinatura != null)
                            {
                                itemContrato.DtAssinatura = item.DtAssinatura.Value;
                            }
                            itemContrato.IcOrdemInicio = "Não";
                            itemContrato.IcRenovacaoAutomatica = "Não";
                            if (item.IcOrdemInicio == true)
                            {
                                itemContrato.IcOrdemInicio = "Sim";
                            }

                            if (item.IcRenovacaoAutomatica == true)
                            {
                                itemContrato.IcRenovacaoAutomatica = "Sim";
                            }

                            itemContrato.DsSituacao = situcao.DsSituacao;
                            listaContratos.Add(itemContrato);
                        }
                    }

                    retorno.lstOutPutGetContratos = listaContratos;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosParaDefinicaoEquipeTec");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosPorSituacao/{idSituacao}")]
        public List<OutPutGetContratos> ListaContratosPorSituacao(int idSituacao)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaContratos = new List<OutPutGetContratos>();
                    var contratos = new bContrato(db).ListaContratosPorSituacao(idSituacao);
                    foreach (var item in contratos)
                    {
                        var itemContrato = new OutPutGetContratos();
                        itemContrato.clientes = new List<string>();
                        itemContrato.coordenadores = new List<string>();
                        var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                        foreach (var contratoCli in contratoClientes)
                        {
                            var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                itemContrato.clientes.Add(pessoaFisica.NmPessoa);
                                itemContrato.clientesTexto = itemContrato.clientesTexto + " " + pessoaFisica.NmPessoa;
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                itemContrato.clientes.Add(contratoCli.NmFantasia);
                                itemContrato.clientesTexto = itemContrato.clientesTexto + " " + contratoCli.NmFantasia;
                            }
                        }
                        var contratoCoordenadores = new bContratoCoordenador(db).ListaCoordenadorGrid(item.IdContrato);
                        foreach (var contratoCoord in contratoCoordenadores)
                        {
                            var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(contratoCoord.IdPessoa));
                            itemContrato.coordenadores.Add(coordenador.NmPessoa);
                            itemContrato.coordenadoresTexto = itemContrato.coordenadoresTexto + " " + coordenador.NmPessoa;
                        }
                        var situcao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();

                        itemContrato.IdContrato = item.IdContrato;
                        itemContrato.NuContratoEdit = item.NuContratoEdit;
                        itemContrato.IdProposta = item.IdProposta;
                        itemContrato.VlContrato = item.VlContrato;
                        itemContrato.DsPrazoExecucao = item.DsPrazoExecucao;
                        itemContrato.DsCentroCusto = item.NuCentroCusto;    //EGS 30.05.2020 - Nova coluna solicitada por Nelson

                        if (item.DtAssinatura != null)
                        {
                            itemContrato.DtAssinatura = item.DtAssinatura.Value;
                        }
                        itemContrato.IcOrdemInicio = "Não";
                        itemContrato.IcRenovacaoAutomatica = "Não";
                        if (item.IcOrdemInicio == true)
                        {
                            itemContrato.IcOrdemInicio = "Sim";
                        }

                        if (item.IcRenovacaoAutomatica == true)
                        {
                            itemContrato.IcRenovacaoAutomatica = "Sim";
                        }

                        itemContrato.DsSituacao = situcao.DsSituacao;
                        listaContratos.Add(itemContrato);

                    }

                    return listaContratos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosPorSituacao");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratosPorData/{data}")]
        public OutPutListaContratosPorData ListaContratosPorData(string data)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratosPorData();

                    retorno.TamanhoFaturamentosAteData = new bContrato(db).ListaFaturamentosAteData(Convert.ToDateTime(data)).Count;
                    retorno.TamanhoEntregaveisAteData = new bContrato(db).ListaEntregaveisAteData(Convert.ToDateTime(data)).Count;

                    retorno.TamanhoReajustes = new bContrato(db).ListaContratoReajuste(Convert.ToDateTime(data)).Count;

                    if (retorno.TamanhoReajustes == 1)
                    {
                        retorno.IdContrato = new bContrato(db).ListaContratoReajuste(Convert.ToDateTime(data)).FirstOrDefault().IdContrato;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratosPorData");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("VerificaContratoExiste/{id}")]
        public OutPutVerificaContratoExiste VerificaContratoExiste(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaContratoExiste();

                    var existe = new bContrato(db).VerificaContratoExiste(id);

                    retorno.Result = existe;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaContratoExiste Contrato [" + id + "]");
                    throw;
                }
            }
        }


        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Maio/2020 
         *  Pesquisa se contrato nu Contrato Edit existe, e não pelo ID
         ===========================================================================================*/
        [HttpGet]
        [Route("VerificaNuContratoEditExiste/{id}")]
        public OutPutVerificaContratoExiste VerificaNuContratoEditExiste(string id)
        {

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaContratoExiste();
                    retorno.Result = false;

                    var existe = new bContrato(db).VerificaNuContratoEditExiste(id);
                    if (existe != 0)
                    {
                        retorno.Result = true;
                    }
                    retorno.idContratoEdit = existe;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaNuContratoEditExiste");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("VerificaNuContratoExiste/{idContrato}/{nuContratoEdit}")]
        public OutPutVerificaContratoExiste VerificaNuContratoExiste(int idContrato, string nuContratoEdit)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaContratoExiste();
                    if (!nuContratoEdit.Contains("CT")) { nuContratoEdit = "CT" + nuContratoEdit; }

                    var existe = new bContrato(db).VerificaNuContratoExiste(idContrato, nuContratoEdit);

                    retorno.Result = existe;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaNuContratoExiste");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("VerificarEntregaveisCronogramaAtraso")]
        public void VerificarEntregaveisCronogramaAtraso()
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
                            var listaEntregaveis = new bEntregavel(db).ObterTodosEntregaiveis();
                            var listaParcelas = new bParcela(db).ObterTodasParcelas();

                            foreach (var entregavel in listaEntregaveis)
                            {
                                if (DateTime.Now.Date > entregavel.DtProduto)
                                {
                                    if (entregavel.IdSituacao != 89 && entregavel.IdSituacao != 90 && entregavel.IdSituacao != 91)
                                    {
                                        entregavel.IcAtraso = true;
                                        if (entregavel.IdSituacao == 56)
                                        {
                                            entregavel.IdSituacao = 68;
                                        }
                                    }
                                }
                            }

                            foreach (var parcela in listaParcelas)
                            {
                                if (DateTime.Now.Date > parcela.DtFaturamento && parcela.IdSituacao == 92)
                                {
                                    parcela.IdSituacao = 93;
                                    parcela.IcAtraso = true;
                                }
                            }

                            db.SaveChanges();
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificarEntregaveisCronogramaAtraso");
                            throw;
                        }

                    }
                });
            }
        }

        [HttpGet]
        [Route("BuscaContratoId/{id}")]
        public OutPutGetContratoId BuscaContratoId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new bContrato(db).GetContratoById(id);
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaContratoId [" + id + "]");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("VerificaContratoGerado/{id}")]
        public OutPutVerificaContratoGerado VerificaContratoGerado(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaContratoGerado();
                    retorno = new bContrato(db).VerificaContratoGerado(id);

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaContratoGerado [" + id + "]");
                    throw;
                }
            }
        }

        [HttpDelete]
        [Route("ExcluirContrato/{id}")]
        public OutPutDeleteContrato ExcluirContrato(int id)
        {
            var retorno = new OutPutDeleteContrato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contrato = new bContrato(db);

                            // Inicia transação
                            contrato.RemoveContrato(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                            retorno.IdContrato = id;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ExcluirContrato [" + id + "]");
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaSituacao")]
        public List<OutputSituacao> ListaSituacao()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaSituacoes = new List<OutputSituacao>();

                    var situacoes = new bSituacao(db).BuscaSituacoesContrato();

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaSituacao");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaFormacaoProfissional")]
        public List<OutPutGetFormacaoProfissional> ListaFormacaoProfissional()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaFormacaoProfissional = new List<OutPutGetFormacaoProfissional>();

                    var formacoesProfissionais = db.FormacaoProfissional.ToList();

                    foreach (var formacaoProf in formacoesProfissionais)
                    {
                        var item = new OutPutGetFormacaoProfissional();
                        item.IdFormacaoProfissional = formacaoProf.IdFormacaoProfissional;
                        item.DsFormacaoProfissional = formacaoProf.DsFormacaoProfissional;

                        listaFormacaoProfissional.Add(item);
                    }
                    return listaFormacaoProfissional;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaFormacaoProfissional");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaTipoContratacao")]
        public List<OutPutGetTipoContratacao> ListaTipoContratacao()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaTipoContratacao = new List<OutPutGetTipoContratacao>();

                    var tiposContratacao = db.TipoContratacaoEquipeTecnica.ToList();

                    foreach (var tipoContratacacao in tiposContratacao)
                    {
                        var item = new OutPutGetTipoContratacao();
                        item.IdTipoContratacao = tipoContratacacao.IdTipoContratacao;
                        item.DsTipoContratacao = tipoContratacacao.DsTipoContratacao;

                        listaTipoContratacao.Add(item);
                    }
                    return listaTipoContratacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaTipoContratacao");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaTaxaInstitucional")]
        public List<OutPutGetTaxaInstitucional> ListaTaxaInstitucional()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaTaxaInstitucional = new List<OutPutGetTaxaInstitucional>();

                    var taxasInstitucionais = db.TaxaInstitucional.ToList();

                    foreach (var taxaInstitucional in taxasInstitucionais)
                    {
                        var item = new OutPutGetTaxaInstitucional();
                        item.IdTaxaInstitucional = taxaInstitucional.IdTaxaInstitucional;
                        item.DsTaxaInstitucional = taxaInstitucional.DsTaxaInstitucional;
                        item.PcTaxaInstitucional = taxaInstitucional.PcTaxaInstitucional;

                        listaTaxaInstitucional.Add(item);
                    }
                    return listaTaxaInstitucional;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaTaxaInstitucional");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratoCoordenadores/{id}")]
        public List<OutPutGetCoordenador> ListaContratoCoordenadores(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacoordenador = new List<OutPutGetCoordenador>();
                    var pessoas = new bContratoCoordenador(db).BuscarCoordenador(id);

                    foreach (var item in pessoas)
                    {

                        var pessoa = new OutPutGetCoordenador();
                        pessoa.IdPessoa = item.IdPessoa;
                        pessoa.IdContrato = item.IdContrato;
                        pessoa.NmPessoa = item.IdPessoaNavigation.NmPessoa;

                        listacoordenador.Add(pessoa);
                    }
                    return listacoordenador;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratoCoordenadores [" + id + "]");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaRecursoEquipeTecnicaId/{id}")]
        public OutPutGetEquipeTecnica BuscaRecursoEquipeTecnicaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutGetEquipeTecnica();
                    var equipeTec = db.ContratoEquipeTecnica.Where(w => w.IdContratoEquipeTecnica == id).FirstOrDefault();
                    var pessoaFisica = new bPessoaFisica(db).GetById(equipeTec.IdPessoaFisica);
                    retorno.CdEmail = pessoaFisica.CdEmail;
                    retorno.DsAtividadeDesempenhada = equipeTec.DsAtividadeDesempenhada;
                    retorno.IdFormacaoProfissional = equipeTec.IdFormacaoProfissional;
                    retorno.IdPessoaFisica = equipeTec.IdPessoaFisica;
                    retorno.IdPessoaJuridica = equipeTec.IdPessoaJuridica;
                    retorno.IdTaxaInstitucional = equipeTec.IdTaxaInstitucional;
                    retorno.IdTipoContratacao = equipeTec.IdTipoContratacao;
                    retorno.IdTipoVinculo = pessoaFisica.IdTipoVinculo;
                    retorno.VlCustoProjeto = equipeTec.VlCustoProjeto;
                    retorno.VlTotalAReceber = equipeTec.VlTotalAreceber;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaRecursoEquipeTecnicaId [" + id + "]");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContratoClientes/{id}")]
        public List<OutPutGetCliente> ListaContratoClientes(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacliente = new List<OutPutGetCliente>();
                    var contratoClientes = new bContratoCliente(db).BuscarClienteFisico(id);
                    foreach (var item in contratoClientes)
                    {
                        var cliente = new OutPutGetCliente();
                        if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisica != null)
                        {
                            cliente.NmCliente = item.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.NmPessoa;
                        }
                        if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridica != null)
                        {

                            if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdEsferaEmpresaNavigation != null)
                            {
                                cliente.DsEsferaEmpresa = item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdEsferaEmpresaNavigation.DsEsferaEmpresa;
                            }
                            cliente.NmCliente = item.NmFantasia;
                            var cli = new bPessoaJuridica(db).BuscarPessoaJuridicaId(item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridica.Value);
                            cliente.Cnpj = Convert.ToUInt64(cli.Cnpj).ToString(@"00\.000\.000\/0000\-00");
                        }
                        cliente.IdCliente = item.IdContratoCliente;
                        cliente.IdPessoa = item.IdClienteNavigation.IdPessoa;
                        cliente.NuContratante = item.NuContratante != null ? item.NuContratante.Value : 0;
                        cliente.IcPagador = item.IcPagador;
                        cliente.IcSomentePagador = item.IcSomentePagador;

                        listacliente.Add(cliente);
                    }
                    return listacliente;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratoClientes [" + id + "]");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaClientes")]
        public List<OutPutGetCliente> ListaClientes()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaClientes = new List<OutPutGetCliente>();

                    var pessoasFisicas = new bPessoaFisica(db).BuscarPessoa();
                    var pessoasJuridicas = new bPessoaJuridica(db).BuscarPessoaJuridica();

                    foreach (var pJuridica in pessoasJuridicas)
                    {
                        var itemPesJuridica = new OutPutGetCliente();
                        var pessoa = db.Pessoa.Where(p => p.IdPessoaJuridica == pJuridica.IdPessoaJuridica).FirstOrDefault();
                        if (pessoa != null)
                        {
                            itemPesJuridica.IdPessoa = pessoa.IdPessoa;
                            itemPesJuridica.NmCliente = pJuridica.NmFantasia;
                            if (pJuridica.IdEsferaEmpresa != null)
                            {
                                var esferaEmpresa = db.EsferaEmpresa.Where(w => w.IdEsferaEmpresa == pJuridica.IdEsferaEmpresa).FirstOrDefault();
                                itemPesJuridica.DsEsferaEmpresa = esferaEmpresa.DsEsferaEmpresa;
                            }
                            listaClientes.Add(itemPesJuridica);
                        }
                    }

                    foreach (var pFisica in pessoasFisicas)
                    {
                        var itemPesFisica = new OutPutGetCliente();
                        var pessoa = db.Pessoa.Where(p => p.IdPessoaFisica == pFisica.IdPessoaFisica).FirstOrDefault();
                        if (pessoa != null)
                        {
                            itemPesFisica.IdPessoa = pessoa.IdPessoa;
                            itemPesFisica.NmCliente = pFisica.NmPessoa;
                            listaClientes.Add(itemPesFisica);
                        }
                    }

                    return listaClientes.OrderBy(w => w.NmCliente).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaClientes");
                    throw;
                }
            }
        }

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaFundamentos");
                    throw;
                }
            }
        }

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaTiposReajuste");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaCoordenadoresDropdown")]
        public List<bContrato.OutputResponsavel> ListaCoordenadoresDropdown()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaResponsaveis = new List<bContrato.OutputResponsavel>();
                    var lista = db.ContratoCoordenador.GroupBy(w => w.IdPessoa).Select(s => s.First()).ToList();

                    foreach (var item in lista)
                    {
                        var itemResponsavel = new bContrato.OutputResponsavel();

                        var pessoa = db.Pessoa.Where(w => w.IdPessoaFisica == item.IdPessoa).FirstOrDefault();
                        itemResponsavel.NmPessoa = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica).FirstOrDefault().NmPessoa;

                        listaResponsaveis.Add(itemResponsavel);
                    }

                    return listaResponsaveis;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaCoordenadoresDropdown");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaCoordenadores")]
        public List<bContrato.OutputResponsavel> ListaCoordenadores()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaCoordenadores = new List<bContrato.OutputResponsavel>();

                    var coordenadores = new bContrato(db).BuscarCoordenadores();

                    foreach (var itemCoord in coordenadores)
                    {

                        var itemCoordenador = new bContrato.OutputResponsavel();

                        itemCoordenador.IdPessoaFisica = itemCoord.IdPessoaFisica;
                        itemCoordenador.NmPessoa = itemCoord.NmPessoa;

                        listaCoordenadores.Add(itemCoordenador);
                    }

                    return listaCoordenadores;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaCoordenadores");

                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddContato")]
        public OutPutContatoContrato AddContato([FromBody]InputAddContatoContrato item)
        {
            var retorno = new OutPutContatoContrato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var addRetorno = new bContrato(db).NovoContato(item);
                            db.Database.CommitTransaction();

                            retorno = addRetorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-AddContato Contrato [" + item.IdContrato + "]");
                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaContato/{id}")]
        public List<InputAddContatoContrato> ListaContato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacontato = new List<InputAddContatoContrato>();

                    var contatos = new bContrato(db).BuscarContatos(id);

                    foreach (var item in contatos)
                    {
                        var tipoContato = db.TipoContato.Where(w => w.IdTipoContato == item.IdTipoContato).FirstOrDefault();
                        var contato = new InputAddContatoContrato();
                        if (item.IdContratoCliente != null)
                        {
                            var contratoCliente = new bContratoCliente(db).GetById(Convert.ToInt32(item.IdContratoCliente));
                            if (contratoCliente.NmFantasia == null)
                            {
                                var idPessoa = db.Cliente.Where(b => b.IdCliente == contratoCliente.IdCliente).FirstOrDefault().IdPessoa;
                                var idPessoaFisica = db.Pessoa.Where(a => a.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                                var nmCliente = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                                contato.NmContrante = nmCliente;
                            }
                            else
                            {
                                contato.NmContrante = contratoCliente.NmFantasia;
                            }
                        }
                        contato.IdContratoContato = item.IdContratoContato;
                        contato.NmContato = item.NmContato;
                        contato.CdEmail = item.CdEmail;
                        contato.NuTelefone = item.NuTelefone;
                        contato.NuCelular = item.NuCelular;
                        contato.IdContrato = item.IdContrato;
                        contato.IdTipoContato = item.IdTipoContato;

                        if (contato.IdTipoContato != null)
                        {
                            contato.DsTipoContato = tipoContato.DsTipoContato;
                        }
                        contato.DsEndereco = item.DsEndereco;
                        contato.NmDepartamento = item.NmDepartamento;
                        if (item.IcApareceFichaResumo != null)
                        {
                            contato.IcApareceFichaResumo = item.IcApareceFichaResumo.Value;
                        }
                        listacontato.Add(contato);
                    }

                    return listacontato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContato [" + id + "]");

                    throw;
                }

            }
        }

        [HttpGet]
        [Route("VerificarAtivarContrato/{id}")]
        public OutPutAtivarContrato VerificarAtivarContrato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutAtivarContrato();

                    var permiteAtivar = new bContrato(db).VerificaAtivarContrato(id);

                    retorno.Result = permiteAtivar;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificarAtivarContrato [" + id + "]");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaContatoId/{id}")]
        public InputAddContatoContrato BuscaContatoId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemContato = new InputAddContatoContrato();

                try
                {
                    var retornoContato = new bContrato(db).BuscarContatoId(id);

                    itemContato.IdContratoContato = retornoContato.IdContratoContato;
                    itemContato.IdContrato = retornoContato.IdContrato;
                    itemContato.NmContato = retornoContato.NmContato;
                    itemContato.CdEmail = retornoContato.CdEmail;
                    itemContato.NuCelular = retornoContato.NuCelular;
                    itemContato.NuTelefone = retornoContato.NuTelefone;
                    itemContato.IdTipoContato = retornoContato.IdTipoContato != null ? retornoContato.IdTipoContato.Value : 0;
                    itemContato.NmDepartamento = retornoContato.NmDepartamento;
                    itemContato.DsEndereco = retornoContato.DsEndereco;
                    itemContato.IdContratoCliente = retornoContato.IdContratoCliente != null ? retornoContato.IdContratoCliente.Value : 0;
                    itemContato.IcApareceFichaResumo = retornoContato.IcApareceFichaResumo;

                    return itemContato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaContatoId [" + id + "]");

                    throw;
                }
            }
        }

        [HttpPut]
        [Route("AtualizarContato")]
        public OutPutContatoContrato AtualizarContato([FromBody]InputAddContatoContrato item)
        {
            var retorno = new OutPutContatoContrato();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var addRetorno = new bContrato(db).UpdateContato(item);
                            db.Database.CommitTransaction();

                            retorno = addRetorno;

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-AtualizarContato Contrato [" + item.IdContrato + "]");
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

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
                            new bContrato(db).RemoverContato(id);
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-RemoverContato [" + id + "]");

                            contatoRemovido = false;
                        }

                        return contatoRemovido;
                    }
                });
                return contatoRemovido;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("PesquisaContrato")]
        public OutPutListaContratos PesquisaContrato([FromBody] InputPesquisaContrato pesquisa)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutListaContratos();
                    var listaContratos = new List<OutPutGetContratos>();
                    var contratos = new List<Contrato>();
                    retorno.lstOutPutGetContratos = new List<OutPutGetContratos>();

                    var sqlParameter = CriarParametroTexto("@Palavra", pesquisa.Palavra);
                    var dsResultado = ExecutarProcedureComRetorno("PR_FiltraGridContratoByPalavra", pesquisa.Url, new List<System.Data.SqlClient.SqlParameter>() { sqlParameter });

                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        var item = new bContrato(db).BuscarContratoId(Convert.ToInt32(row["IdContrato"]));
                        if (listaContratos.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault() == null)
                        {
                            var itemContrato = new OutPutGetContratos();
                            itemContrato.clientes = new List<string>();
                            itemContrato.coordenadores = new List<string>();
                            var contratoClientes = new bContratoCliente(db).GetByContrato(item.IdContrato);
                            foreach (var contratoCli in contratoClientes)
                            {
                                var cliente = new bCliente(db).BuscarClienteId(contratoCli.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                                if (pessoa.IdPessoaFisica != null)
                                {
                                    var pessoaFisica = new bPessoaFisica(db).GetById((Int32)pessoa.IdPessoaFisica);
                                    itemContrato.clientes.Add(pessoaFisica.NmPessoa);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + pessoaFisica.NmPessoa;
                                }
                                else if (pessoa.IdPessoaJuridica != null)
                                {
                                    var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)pessoa.IdPessoaJuridica);
                                    itemContrato.clientes.Add(contratoCli.NmFantasia);
                                    itemContrato.clientesTexto = itemContrato.clientesTexto + " " + contratoCli.NmFantasia;
                                }
                            }
                            var contratoCoordenadores = new bContratoCoordenador(db).ListaCoordenadorGrid(item.IdContrato);
                            foreach (var contratoCoord in contratoCoordenadores)
                            {
                                var coordenador = new bResponsavel(db).BuscarPessoaId(Convert.ToInt32(contratoCoord.IdPessoa));
                                itemContrato.coordenadores.Add(coordenador.NmPessoa);
                                itemContrato.coordenadoresTexto = itemContrato.coordenadoresTexto + " " + coordenador.NmPessoa;
                            }
                            var situcao = db.Situacao.Where(s => s.IdSituacao == item.IdSituacao).Single();

                            itemContrato.IdContrato = item.IdContrato;
                            itemContrato.IdProposta = item.IdProposta;
                            itemContrato.VlContrato = item.VlContrato;
                            itemContrato.DsPrazoExecucao = item.DsPrazoExecucao;
                            if (itemContrato.DtAssinatura != null)
                            {
                                itemContrato.DtAssinatura = item.DtAssinatura.Value;
                            }
                            itemContrato.IcOrdemInicio = "Não";
                            itemContrato.IcRenovacaoAutomatica = "Não";
                            if (item.IcOrdemInicio == true)
                            {
                                itemContrato.IcOrdemInicio = "Sim";
                            }

                            if (item.IcRenovacaoAutomatica == true)
                            {
                                itemContrato.IcRenovacaoAutomatica = "Sim";
                            }

                            itemContrato.DsSituacao = situcao.DsSituacao;
                            listaContratos.Add(itemContrato);
                        }
                    }

                    retorno.lstOutPutGetContratos = listaContratos;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-PesquisaContrato");

                    throw;
                }
            }
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
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ExecutarProcedureComRetorno");
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

        // Gerar Planilha Excel
        [HttpGet]
        [Route("GetExcel/{id}")]
        public async Task<IActionResult> ExportToExcel(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var stream = new MemoryStream();

                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        int contCell = 0;
                        int contCell2 = 0;
                        int contQuebraPag = 0;

                        var contrato = new bContrato(db).GetPlanilhaExcel(id);

                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Contrato");
                        worksheet.PrinterSettings.LeftMargin = 0.2M;
                        worksheet.PrinterSettings.RightMargin = 0.1M;

                        string webRootPath = _hostingEnvironment.WebRootPath;
                        string contentRootPath = _hostingEnvironment.ContentRootPath + "\\Imagens\\fipe.png";

                        var imagem = Image.FromFile(contentRootPath);

                        contCell++;
                        contCell2++;

                        var Logo = worksheet.Drawings.AddPicture("Logo", imagem);
                        Logo.SetPosition(1, 380);
                        Logo.SetSize(55, 51);

                        worksheet.Cells[contCell, 1].Value = "";
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Height = 50;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "Nº DO CONTRATO:";
                        worksheet.Cells[contCell, 1].Style.Font.Bold = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        worksheet.Cells[contCell, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 2].Value = contrato.NuContratoEdit;
                        worksheet.Cells[contCell, 2].Style.Font.Bold = true;
                        worksheet.Cells[contCell, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 2, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 2, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 10;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "CENTRO DE CUSTO: ";
                        worksheet.Cells[contCell, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 2].Value = contrato.NuCentroCusto;
                        worksheet.Cells[contCell, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 2, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 2, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "CÓDIGO DE ISS: ";
                        worksheet.Cells[contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 2].Value = contrato.CdIss;
                        worksheet.Cells[contCell, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[contCell, 2, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 2, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        if (String.IsNullOrEmpty(contrato.IdArea.ToString()))
                        {
                            worksheet.Cells[contCell, 1].Value = "ÁREA: ";
                        }
                        else
                        {
                            worksheet.Cells[contCell, 1].Value = "ÁREA: " + new bArea(db).BuscaAreaId(contrato.IdArea.Value).DsArea;
                        }
                        worksheet.Cells[contCell, 1, contCell, 3].Merge = true;
                        if (String.IsNullOrEmpty(contrato.IdTema.ToString()))
                        {
                            worksheet.Cells[contCell, 4].Value = "TEMA: ";
                        }
                        else
                        {
                            worksheet.Cells[contCell, 4].Value = "TEMA: " + new bTema(db).BuscaTemaId(contrato.IdTema.Value).DsTema;
                        }
                        worksheet.Cells[contCell, 4, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        //dados do cliente
                        worksheet.Cells[contCell, 1].Value = "DADOS DO CLIENTE";
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        foreach (var item in contrato.ContratoCliente)
                        {
                            contCell++;
                            contCell2++;

                            var pessoaFisica = new PessoaFisica();
                            var pessoaJuridica = new PessoaJuridica();
                            var cliente = new bCliente(db).BuscarClienteId(item.IdCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                pessoaFisica = new bPessoaFisica(db).GetById(pessoa.IdPessoaFisica.Value);

                                worksheet.Cells[contCell, 1].Value = "CONTRATANTE: " + pessoaFisica.NmPessoa;
                                worksheet.Cells[contCell, 1].Style.Font.Bold = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                contCell++;
                                contCell2++;

                                worksheet.Cells[contCell, 1].Value = "CPF: " + FormatCPF(pessoaFisica.NuCpf);
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                contCell++;
                                contCell2++;

                                var cidade = pessoaFisica.IdCidade != null ? db.Cidade.Where(w => w.IdCidade == pessoaFisica.IdCidade).FirstOrDefault().NmCidade : "";
                                worksheet.Cells[contCell, 1].Value = "ENDEREÇO: " + pessoaFisica.DsEndereco + " Nº: " + pessoaFisica.NuEndereco + " BAIRRO: " + pessoaFisica.NmBairro + " CIDADE: " + cidade + " UF: " + pessoaFisica.SgUf + " CEP: " + pessoaFisica.NuCep;
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                if (item.ContratoContatos.Count > 0)
                                {
                                    contCell++;
                                    contCell2++;

                                    worksheet.Cells[contCell, 1].Value = "CONTATOS";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                    worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                    worksheet.Row(contCell).Style.Font.Size = 9;
                                    worksheet.Row(contCell).Style.Font.Bold = true;
                                    worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    contCell++;
                                    contCell2++;

                                    worksheet.Cells[contCell, 1].Value = "NOME";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 2].Merge = true;
                                    worksheet.Cells[contCell, 3].Value = "E-MAIL";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 3, contCell, 4].Merge = true;
                                    worksheet.Cells[contCell, 5].Value = "TELEFONE";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 5, contCell, 7].Merge = true;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                                    worksheet.Row(contCell).Style.Font.Size = 9;
                                    worksheet.Row(contCell).Style.Font.Bold = true;

                                    foreach (var itemContato in item.ContratoContatos.Where(w => w.IcApareceFichaResumo == true))
                                    {
                                        contCell++;
                                        contCell2++;

                                        worksheet.Cells[contCell, 1].Value = itemContato.NmContato;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 2].Merge = true;
                                        worksheet.Cells[contCell, 3].Value = itemContato.CdEmail;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 3, contCell, 4].Merge = true;
                                        worksheet.Cells[contCell, 5].Value = itemContato.NuTelefone;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 5, contCell, 7].Merge = true;
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                                        worksheet.Row(contCell).Style.Font.Size = 9;
                                        worksheet.Row(contCell).Style.Font.Bold = true;
                                    }
                                }
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(pessoa.IdPessoaJuridica.Value);

                                worksheet.Cells[contCell, 1].Value = "CONTRATANTE: " + pessoaJuridica.RazaoSocial;
                                worksheet.Cells[contCell, 1].Style.Font.Bold = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                contCell++;
                                contCell2++;

                                worksheet.Cells[contCell, 1].Value = "CNPJ: " + FormatCNPJ(pessoaJuridica.Cnpj);
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                contCell++;
                                contCell2++;

                                if (pessoaJuridica.IdClassificacaoEmpresa != null)
                                {
                                    worksheet.Cells[contCell, 1].Value = "CLASSIFICAÇÃO: " + db.ClassificacaoEmpresa.Where(w => w.IdClassificacaoEmpresa == pessoaJuridica.IdClassificacaoEmpresa).FirstOrDefault().DsClassificacaoEmpresa;
                                }
                                else
                                {
                                    worksheet.Cells[contCell, 1].Value = "CLASSIFICAÇÃO: ";
                                }

                                if (!String.IsNullOrEmpty(pessoaJuridica.IdEsferaEmpresa.ToString()))
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 2 || pessoaJuridica.IdClassificacaoEmpresa == 3)
                                    {
                                        worksheet.Cells[contCell, 3].Value = "ESFERA: " + "N/A";
                                    }
                                    else
                                    {
                                        worksheet.Cells[contCell, 3].Value = "ESFERA: " + db.EsferaEmpresa.Where(w => w.IdEsferaEmpresa == pessoaJuridica.IdEsferaEmpresa).FirstOrDefault().DsEsferaEmpresa;
                                    }

                                }
                                else
                                {
                                    worksheet.Cells[contCell, 3].Value = "ESFERA: " + "N/A";
                                }

                                if (!String.IsNullOrEmpty(pessoaJuridica.IdEntidade.ToString()))
                                {
                                    if (pessoaJuridica.IdClassificacaoEmpresa == 3)
                                    {
                                        worksheet.Cells[contCell, 3].Value = "ENTIDADE: " + "N/A";
                                    }
                                    else
                                    {
                                        worksheet.Cells[contCell, 5].Value = "ENTIDADE: " + db.Entidade.Where(w => w.IdEntidade == pessoaJuridica.IdEntidade).FirstOrDefault().DsEntidade;
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[contCell, 5].Value = "ENTIDADE: " + "N/A";
                                }

                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 2].Merge = true;
                                worksheet.Cells[contCell, 3, contCell, 4].Merge = true;
                                worksheet.Cells[contCell, 5, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                contCell++;
                                contCell2++;

                                var cidade = pessoaJuridica.IdCidade != null ? db.Cidade.Where(w => w.IdCidade == pessoaJuridica.IdCidade).FirstOrDefault().NmCidade : "";
                                worksheet.Cells[contCell, 1].Value = "ENDEREÇO: " + pessoaJuridica.Endereco + " Nº: " + pessoaJuridica.NuEndereco + " BAIRRO: "
                                + pessoaJuridica.NmBairro + " CIDADE: " + cidade + " UF: " + pessoaJuridica.Uf + " CEP: " + pessoaJuridica.Cep;
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                                if (item.ContratoContatos.Count > 0)
                                {
                                    contCell++;
                                    contCell2++;

                                    worksheet.Cells[contCell, 1].Value = "CONTATOS";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                                    worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                                    worksheet.Row(contCell).Style.Font.Size = 9;
                                    worksheet.Row(contCell).Style.Font.Bold = true;
                                    worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                                    worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    contCell++;
                                    contCell2++;

                                    worksheet.Cells[contCell, 1].Value = "NOME";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 2].Merge = true;
                                    worksheet.Cells[contCell, 3].Value = "E-MAIL";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 3, contCell, 4].Merge = true;
                                    worksheet.Cells[contCell, 5].Value = "TELEFONE";
                                    worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 5, contCell, 7].Merge = true;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                    worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                                    worksheet.Row(contCell).Style.Font.Size = 9;
                                    worksheet.Row(contCell).Style.Font.Bold = true;

                                    foreach (var itemContato in item.ContratoContatos.Where(w => w.IcApareceFichaResumo == true))
                                    {
                                        contCell++;
                                        contCell2++;

                                        worksheet.Cells[contCell, 1].Value = itemContato.NmContato;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 2].Merge = true;
                                        worksheet.Cells[contCell, 3].Value = itemContato.CdEmail;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 3, contCell, 4].Merge = true;
                                        worksheet.Cells[contCell, 5].Value = itemContato.NuTelefone;
                                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 5, contCell, 7].Merge = true;
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                                        worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                                        worksheet.Row(contCell).Style.Font.Size = 9;
                                        worksheet.Row(contCell).Style.Font.Bold = true;
                                    }
                                }
                            }
                        }

                        contCell++;
                        contCell2++;

                        //Dados Contrato
                        worksheet.Cells[contCell, 1].Value = "DADOS DO CONTRATO";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "PROPOSTA: " + contrato.IdProposta;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 3].Merge = true;
                        worksheet.Cells[contCell, 4].Value = "CONTRATO: " + contrato.IdContrato;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 4, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "TÍTULO: " + contrato.DsAssunto;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        if (quebraLinhaExcel(contrato.DsAssunto) <= 2)
                        {
                            worksheet.Row(contCell).Height = 13 * quebraLinhaExcel(contrato.DsAssunto); ;
                        }
                        else
                        {
                            worksheet.Row(contCell).Height = 12 * quebraLinhaExcel(contrato.DsAssunto); ;
                        }

                        if (quebraLinhaExcel(contrato.DsAssunto) > 1)
                        {
                            contQuebraPag += quebraLinhaExcel(contrato.DsAssunto);
                        }
                        worksheet.Row(contCell).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        contCell++;
                        contCell2++;

                        var objeto = contrato.DsObjeto.Replace("\n", " ");
                        worksheet.Cells[contCell, 1].Value = "OBJETO: " + objeto;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                        if (quebraLinhaExcel(contrato.DsObjeto) <= 2)
                        {
                            worksheet.Row(contCell).Height = 13 * quebraLinhaExcel(contrato.DsObjeto);
                        }
                        else
                        {
                            worksheet.Row(contCell).Height = 12 * quebraLinhaExcel(contrato.DsObjeto);
                        }

                        if (quebraLinhaExcel(contrato.DsObjeto) > 1)
                        {
                            contQuebraPag += quebraLinhaExcel(contrato.DsObjeto);
                        }
                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "VALOR: " + contrato.VlContrato.ToString("0,0.00");
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        var ordemInicio = contrato.IcOrdemInicio.Value ? "SIM" : "NÃO";
                        if (contrato.DtAssinatura != null)
                        {
                            worksheet.Cells[contCell, 1].Value = "ASSINATURA DO CONTRATO: " + contrato.DtAssinatura.Value.ToShortDateString();
                        }
                        else
                        {
                            worksheet.Cells[contCell, 1].Value = "ASSINATURA DO CONTRATO: ";
                        }
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 3].Merge = true;
                        worksheet.Cells[contCell, 4].Value = "ORDEM DE INÍCIO: " + ordemInicio;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 4, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        if (contrato.DtFim != null)
                        {
                            worksheet.Cells[contCell, 1].Value = "PERÍODO DE VIGÊNCIA: " + contrato.DtInicio.Value.ToShortDateString() + " á " + contrato.DtFim.Value.ToShortDateString();
                        }
                        else
                        {
                            worksheet.Cells[contCell, 1].Value = "PERÍODO DE VIGÊNCIA: ";
                        }

                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        contCell++;
                        contCell2++;

                        if (contrato.DtInicioExecucao != null)
                        {
                            worksheet.Cells[contCell, 1].Value = "PERÍODO DE EXECUÇÃO: " + contrato.DtInicioExecucao.Value.ToShortDateString() + " á " + contrato.DtFimExecucao.Value.ToShortDateString();
                        }
                        else
                        {
                            worksheet.Cells[contCell, 1].Value = "PERÍODO DE EXECUÇÃO: ";
                        }

                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        contCell++;
                        contCell2++;

                        var icReajuste = contrato.IcReajuste.Value ? "SIM" : "NÃO";
                        if (icReajuste == "SIM")
                        {
                            worksheet.Cells[contCell, 1].Value = "REAJUSTE: " + contrato.IdIndiceReajusteNavigation.DsIndiceReajuste;
                        }
                        else
                        {
                            worksheet.Cells[contCell, 1].Value = "REAJUSTE: ";
                        }
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        //Dados Equipe Técnica
                        worksheet.Cells[contCell, 1].Value = "EQUIPE TÉCNICA";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        contCell++;
                        contCell2++;

                        var coordenadores = db.ContratoCoordenador.Where(w => w.IdContrato == contrato.IdContrato).ToList();
                        string contCoordenadores = "";
                        string coo = "";
                        foreach (var item in coordenadores)
                        {
                            contCoordenadores += db.PessoaFisica.Where(w => w.IdPessoaFisica == item.IdPessoa).FirstOrDefault().NmPessoa + ", ";
                        }
                        if (!String.IsNullOrEmpty(contCoordenadores))
                        {
                            coo = contCoordenadores.Substring(0, contCoordenadores.Length - 2);
                        }
                        else
                        {
                            coo = "";
                        }
                        worksheet.Cells[contCell, 1].Value = "COORDENADOR: " + coo;
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        contCell++;
                        contCell2++;

                        //Dados Entregáveis
                        worksheet.Cells[contCell, 1].Value = "ENTREGÁVEIS";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "Nº";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 2].Value = "CONTRATANTE";
                        worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 3].Value = "FRENTE";
                        worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 4].Value = "ENTREGÁVEL";
                        worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 5].Value = "DATA";
                        worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 6].Value = "SITUAÇÃO";
                        worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 6, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 6, contCell, 7].Merge = true;
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        //ExcelAddress address = new ExcelAddress(contCell.ToString() + ":" + contCell.ToString());
                        //worksheet.PrinterSettings.RepeatRows = address;

                        foreach (var item in contrato.ContratoEntregavel)
                        {
                            if (contCell2 == (55 - contQuebraPag))
                            {
                                contCell2 = 0;
                                contQuebraPag = 0;

                                worksheet.Row(contCell).PageBreak = true;

                                contCell++;
                                contCell2++;

                                worksheet.Cells[contCell, 1].Value = "Nº";
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 2].Value = "CONTRATANTE";
                                worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 3].Value = "FRENTE";
                                worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 4].Value = "ENTREGÁVEL";
                                worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 5].Value = "DATA";
                                worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 6].Value = "SITUAÇÃO";
                                worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 6, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 6, contCell, 7].Merge = true;
                                //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;
                            }

                            contCell++;
                            contCell2++;

                            worksheet.Cells[contCell, 1].Value = item.VlOrdem;
                            worksheet.Cells[contCell, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            var idCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == item.IdContratoCliente).FirstOrDefault().IdCliente;
                            var cliente = new bCliente(db).BuscarClienteId(idCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                worksheet.Cells[contCell, 2].Value = pessoa.IdPessoaFisicaNavigation.NmPessoa;
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                worksheet.Cells[contCell, 2].Value = pessoa.IdPessoaJuridicaNavigation.NmFantasia;
                            }
                            worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 3].Value = item.IdFrenteNavigation.NmFrente;
                            worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 4].Value = item.DsProduto;
                            worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            if (item.DtProduto != null)
                            {
                                worksheet.Cells[contCell, 5].Value = item.DtProduto.Value.ToShortDateString();
                            }
                            else
                            {
                                worksheet.Cells[contCell, 5].Value = "";
                            }
                            worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 6].Value = db.Situacao.Where(w => w.IdSituacao == item.IdSituacao).FirstOrDefault().DsSituacao;
                            worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 6, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 6, contCell, 7].Merge = true;
                            worksheet.Row(contCell).Style.Font.Size = 9;
                            worksheet.Row(contCell).Style.Font.Bold = true;
                        }

                        contCell++;
                        contCell2++;

                        //Dados Cronograma Financeiro
                        worksheet.Cells[contCell, 1].Value = "CRONOGRAMA FINANCEIRO";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                        worksheet.Cells[contCell, 1].Style.Font.Size = 18;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Fill.BackgroundColor.SetColor(Color.Lavender);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;
                        worksheet.Row(contCell).Style.Font.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        contCell++;
                        contCell2++;

                        worksheet.Cells[contCell, 1].Value = "Nº";
                        worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 2].Value = "CONTRATANTE";
                        worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 3].Value = "ISS";
                        worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 4].Value = "VALOR";
                        worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 5].Value = "DATA";
                        worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 6].Value = "SITUAÇÃO";
                        worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        worksheet.Cells[contCell, 7].Value = "ENTREGÁVEIS";
                        worksheet.Cells[contCell, 7, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Color.SetColor(Color.Black);
                        worksheet.Row(contCell).Style.Font.Size = 9;
                        worksheet.Row(contCell).Style.Font.Bold = true;

                        //ExcelAddress address = new ExcelAddress(contCell.ToString() + ":" + contCell.ToString());
                        //worksheet.PrinterSettings.RepeatRows = address;

                        foreach (var item in contrato.ContratoCronogramaFinanceiro)
                        {
                            //Quebra pagina coloca
                            if (contCell2 == (55 - contQuebraPag))
                            {
                                contCell2 = 0;
                                contQuebraPag = 0;

                                worksheet.Row(contCell).PageBreak = true;

                                contCell++;
                                contCell2++;

                                worksheet.Cells[contCell, 1].Value = "Nº";
                                worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 2].Value = "CONTRATANTE";
                                worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 3].Value = "ISS";
                                worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 4].Value = "VALOR";
                                worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 5].Value = "DATA";
                                worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 6].Value = "SITUAÇÃO";
                                worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                worksheet.Cells[contCell, 7].Value = "ENTREGÁGEIS";
                                worksheet.Cells[contCell, 7, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                                //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                //worksheet.Cells[contCell, 1, contCell, 8].Style.Border.Left.Color.SetColor(Color.Black);
                                worksheet.Row(contCell).Style.Font.Size = 9;
                                worksheet.Row(contCell).Style.Font.Bold = true;

                            }

                            contCell++;
                            contCell2++;

                            worksheet.Cells[contCell, 1].Value = item.NuParcela;
                            worksheet.Cells[contCell, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            var idCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == item.IdContratoCliente).FirstOrDefault().IdCliente;
                            var cliente = new bCliente(db).BuscarClienteId(idCliente);
                            var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                            if (pessoa.IdPessoaFisica != null)
                            {
                                worksheet.Cells[contCell, 2].Value = pessoa.IdPessoaFisicaNavigation.NmPessoa;
                            }
                            else if (pessoa.IdPessoaJuridica != null)
                            {
                                worksheet.Cells[contCell, 2].Value = pessoa.IdPessoaJuridicaNavigation.NmFantasia;
                            }
                            worksheet.Cells[contCell, 2, contCell, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 3].Value = item.CdIss;
                            worksheet.Cells[contCell, 3, contCell, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 4].Value = item.VlParcela.ToString("0,0.00");
                            worksheet.Cells[contCell, 4, contCell, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            if (item.DtFaturamento != null)
                            {
                                worksheet.Cells[contCell, 5].Value = item.DtFaturamento.Value.ToShortDateString();
                            }
                            else
                            {
                                worksheet.Cells[contCell, 5].Value = "";
                            }
                            worksheet.Cells[contCell, 5, contCell, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 6].Value = db.Situacao.Where(w => w.IdSituacao == item.IdSituacao).FirstOrDefault().DsSituacao;
                            worksheet.Cells[contCell, 6, contCell, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            var bParcela = new bParcela(db);
                            var entregaveis = bParcela.ObterNumerosEntregaveisParcela(bParcela.ConsultarParcela(item.IdContratoCronFinanceiro).Entregaveis);
                            string nuEntregaveis = "";
                            string nuEnt = "";

                            var itensEntregaveis = entregaveis.Split(",");
                            foreach (var itemEntregaveis in itensEntregaveis)
                            {
                                nuEntregaveis += itemEntregaveis + ", ";
                            }
                            if (!String.IsNullOrEmpty(nuEntregaveis))
                            {
                                nuEnt = nuEntregaveis.Substring(0, nuEntregaveis.Length - 2);
                            }
                            else
                            {
                                nuEnt = "";
                            }
                            worksheet.Cells[contCell, 7].Value = nuEnt;
                            worksheet.Cells[contCell, 7, contCell, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Row(contCell).Style.Font.Size = 9;
                            worksheet.Row(contCell).Style.Font.Bold = true;
                        }

                        worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:5"];

                        if (!String.IsNullOrEmpty(contrato.DsObservacao))
                        {
                            contCell++;
                            contCell2++;

                            var observacao = contrato.DsObservacao.Replace("\n", " ");
                            worksheet.Cells[contCell, 1].Value = "OBSERVAÇÃO: " + observacao;
                            worksheet.Cells[contCell, 1, contCell, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            worksheet.Cells[contCell, 1, contCell, 7].Merge = true;
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Right.Color.SetColor(Color.Black);
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[contCell, 1, contCell, 7].Style.Border.Top.Color.SetColor(Color.Black);
                            worksheet.Cells[contCell, 1, contCell, 7].Style.WrapText = true;
                            worksheet.Row(contCell).Style.Font.Size = 9;
                            worksheet.Row(contCell).Style.Font.Bold = true;
                            worksheet.Row(contCell).Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            if (quebraLinhaExcel(contrato.DsObservacao) <= 2)
                            {
                                worksheet.Row(contCell).Height = 13 * quebraLinhaExcel(contrato.DsObservacao);
                            }
                            else
                            {
                                worksheet.Row(contCell).Height = 12 * quebraLinhaExcel(contrato.DsObservacao);
                            }

                            if (quebraLinhaExcel(contrato.DsObservacao) > 1)
                            {
                                contQuebraPag += quebraLinhaExcel(contrato.DsObservacao);
                            }
                        }

                        worksheet.Column(1).Width = 15;
                        worksheet.Column(2).Width = 35;
                        worksheet.Column(3).Width = 10;
                        worksheet.Column(4).Width = 11;
                        worksheet.Column(5).Width = 9;
                        worksheet.Column(6).Width = 8;
                        worksheet.Column(7).Width = 11;

                        package.Save();
                    }

                    string fileName = @"Contrato.xlsx";
                    string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    stream.Position = 0;
                    return File(stream, fileType, fileName);

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ExportToExcel");

                    throw;
                }
            }
        }

        // metodo para quebrar linhas excel
        public int quebraLinhaExcel(string texto)
        {
            int contadorLinhasObjeto = 1;
            int tamanhoCaracteres = 0;
            string[] objeto = texto.Split(' ');
            for (int i = 0; i < objeto.Length; i++)
            {
                tamanhoCaracteres += objeto[i].Length + 1;
                if (tamanhoCaracteres >= 106)
                {
                    contadorLinhasObjeto++;
                    tamanhoCaracteres = 0;
                }
            }

            return contadorLinhasObjeto;
        }

        // metodo para quebrar linhas excel
        public int quebraLinhaExcelCellMenor(string texto)
        {
            int contadorLinhasObjeto = 1;
            int tamanhoCaracteres = 0;
            string[] objeto = texto.Split(' ');
            for (int i = 0; i < objeto.Length; i++)
            {
                tamanhoCaracteres += objeto[i].Length + 1;
                if (tamanhoCaracteres >= 40)
                {
                    contadorLinhasObjeto++;
                    tamanhoCaracteres = 0;
                }
            }

            return contadorLinhasObjeto;
        }

        #region Contrato Comentario 

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioGestorContrato/{id}/{contrato}")]
        public List<InputAddComentario> ListaComentarioGestorContrato(int id, int contrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bContrato(db).ListaComentarioGestorContrato(id, contrato);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdContratoComentario = item.IdContratoComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaComentarioGestorContrato Contrato [" + contrato + "]");
                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioDiretoria/{id}/{contrato}")]
        public List<InputAddComentario> ListaComentarioDiretoria(int id, int contrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bContrato(db).ComentarioDiretoria(id, contrato);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdContratoComentario = item.IdContratoComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaComentarioDiretoria Contrato [" + contrato + "]");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaComentarioJuridico/{id}/{contrato}")]
        public List<InputAddComentario> ListaComentarioJuridico(int id, int contrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaComentario = new List<InputAddComentario>();

                    var comentarios = new bContrato(db).ComentarioDiretoria(id, contrato);

                    foreach (var item in comentarios)
                    {

                        var itemComentario = new InputAddComentario();

                        itemComentario.IdContratoComentario = item.IdContratoComentario;

                        listaComentario.Add(itemComentario);
                    }

                    return listaComentario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaComentarioJuridico Contrato [" + contrato + "]");

                    throw;
                }
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

                    var com = new bContrato(db).GetComentarioById(id);

                    comentario.IdContratoComentario = com.IdContratoComentario;
                    comentario.DsComentario = com.DsComentario;
                    comentario.IdContrato = com.IdContrato;
                    comentario.IdUsuario = com.IdUsuario;
                    comentario.DtComentario = com.DtComentario;

                    return comentario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-GetComentarioById [" + id + "]");
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
                            // Inicia transação
                            var comentario = new ContratoComentario();
                            comentario.IdContrato = item.IdContrato;
                            comentario.IdContratoComentario = item.IdContratoComentario;
                            comentario.IdUsuario = AppSettings.constGlobalUserID;
                            comentario.DsComentario = item.DsComentario;
                            comentario.DtComentario = item.DtComentario;

                            var updateRetorno = new bContrato(db).UpdateComentario(comentario);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateComentario Contrato [" + item.IdContrato + "]");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }


        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Janeiro/2021 
         *  Salva informacao se o Contrato é Abono ou não
         ===========================================================================================*/
        [HttpGet]
        [Route("SalvarContratoAbono/{idContrato}/{bContratoAbono}")]
        public void SalvarContratoAbono(int idContrato, bool bContratoAbono)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    new bContrato(db).UpdateContratoAbono(idContrato, bContratoAbono);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-SalvarContratoAbono");
                    throw ex;
                }
            }
        }



        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [Route("AddComentario")]
        public OutPutAddComentario AddComentario([FromBody]InputAddComentario item)
        {
            var retorno = new OutPutAddComentario();
            var comentario = new ContratoComentario();

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
                            var objComentario = new bContrato(db);

                            comentario.DsComentario = item.DsComentario;
                            comentario.DtComentario = DateTime.Now;
                            comentario.IdUsuario = AppSettings.constGlobalUserID;
                            comentario.IdContrato = item.IdContrato;

                            objComentario.AddComentario(comentario);

                            var contratoComentarioLido = new ContratoComentarioLido();
                            contratoComentarioLido.IdContratoComentario = comentario.IdContratoComentario;
                            contratoComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                            db.ContratoComentarioLido.Add(contratoComentarioLido);

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-AddComentario Contrato [" + item.IdContrato + "]");

                            retorno.Result = false;
                        }

                        return retorno;
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

                    var comentarios = new bContrato(db).BuscarComentario(id);

                    foreach (var item in comentarios)
                    {
                        var comentario = new OutPutComentario();
                        comentario.IdContratoComentario = item.IdContratoComentario;
                        comentario.DtComentario = item.DtComentario.ToString();
                        comentario.DsComentario = item.DsComentario;

                        var pComentarioLido = db.ContratoComentarioLido
                            .Where(c => c.IdContratoComentario == item.IdContratoComentario && c.IdUsuario == idUsuario)
                            .FirstOrDefault();
                        if (pComentarioLido == null)
                        {
                            comentario.ComentarioLido = false;
                        }
                        else
                        {
                            comentario.ComentarioLido = true;
                        }
                        comentario.IdUsuario = item.IdUsuario;
                        var usuario = new bUsuario(db).GetById(item.IdUsuario);
                        if (usuario != null)
                        {
                            var pessoaFisica = new bPessoaFisica(db).GetById(usuario.IdPessoa);
                            comentario.NmUsuario = pessoaFisica.NmPessoa;
                        }
                        listacomentario.Add(comentario);
                    }

                    return listacomentario.OrderByDescending(o => o.DtComentario).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaComentarios IdUsuario [" + idUsuario + "]");
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
                var retorno = new OutPutSalvaComentarios();
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.ComentariosLidos = new List<int>();

                            var rangeFim = item.Pagina * 50;
                            var rangeInicio = rangeFim - 50;
                            var listaContratoComentarios = db.ContratoComentario
                                .Where(p => p.IdContrato == item.IdContrato)
                                .OrderByDescending(o => o.DtComentario)
                                .ToList();

                            if (listaContratoComentarios.Count < rangeFim)
                            {
                                var diferenca = rangeFim - listaContratoComentarios.Count;
                                rangeFim = 50 - diferenca;
                                retorno.TotalComentariosLidos = rangeFim;

                                var pComentarios = listaContratoComentarios.GetRange(rangeInicio, rangeFim);

                                foreach (var pComentario in pComentarios)
                                {
                                    bool comentarioExiste = VerificaComentarioLidoExiste(item, pComentario);

                                    if (!comentarioExiste)
                                    {
                                        var pComentarioLido = new ContratoComentarioLido();
                                        pComentarioLido.IdContratoComentario = pComentario.IdContratoComentario;
                                        pComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                                        db.ContratoComentarioLido.Add(pComentarioLido);

                                        retorno.ComentariosLidos.Add(pComentario.IdContratoComentario);
                                    }
                                }
                            }
                            else
                            {
                                retorno.TotalComentariosLidos = 50;

                                var pComentarios = listaContratoComentarios.GetRange(rangeInicio, 50);

                                foreach (var pComentario in pComentarios)
                                {
                                    bool comentarioexiste = VerificaComentarioLidoExiste(item, pComentario);

                                    if (!comentarioexiste)
                                    {
                                        var pComentarioLido = new ContratoComentarioLido();
                                        pComentarioLido.IdContratoComentario = pComentario.IdContratoComentario;
                                        pComentarioLido.IdUsuario = AppSettings.constGlobalUserID;

                                        db.ContratoComentarioLido.Add(pComentarioLido);

                                        retorno.ComentariosLidos.Add(pComentario.IdContratoComentario);
                                    }
                                }
                            }
                            retorno.Result = true;

                            db.SaveChanges();

                            db.Database.CommitTransaction();

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-SalvaComentariosLidos Contrato [" + item.IdContrato + "]");

                            throw;
                        }
                    }
                });
                return retorno;
            }
        }

        private static bool VerificaComentarioLidoExiste(InputSalvaComentarios item, ContratoComentario pComentario)
        {
            using (var db = new FIPEContratosContext())
            {
                bool comentarioExiste = false;

                var pComentarioLidoExistente = db.ContratoComentarioLido
                                                .Where(p => p.IdContratoComentario == pComentario.IdContratoComentario && p.IdUsuario == item.IdUsuario)
                                                .FirstOrDefault();
                if (pComentarioLidoExistente != null)
                {
                    comentarioExiste = true;
                }

                return comentarioExiste;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("VerificaQtdComentariosNaoLidos/{idContrato}/{idUsuario}")]
        public OutPutVerificaQtdComentariosNaoLidos VerificaQtdComentariosNaoLidos(int idContrato, int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutVerificaQtdComentariosNaoLidos();
                    retorno.ComentariosNaoLidos = new List<int>();

                    // Busca todos os Comentários da Contrato
                    var comentarios = new bContrato(db).BuscarComentario(idContrato);

                    // Varre a lista de Comentários e verifica se o Comentário já existe na tabela Contrato Comentários Lidos
                    // Caso não exista soma a váriavel TotalComentariosNaoLidos e o Id do Contrato Comentário na lista de ComentariosNao Lidos
                    foreach (var item in comentarios)
                    {
                        var pComentarioLido = db.ContratoComentarioLido
                            .Where(c => c.IdContratoComentario == item.IdContratoComentario && c.IdUsuario == idUsuario)
                            .FirstOrDefault();
                        if (pComentarioLido == null)
                        {
                            retorno.TotalComentariosNaoLidos++;
                            retorno.ComentariosNaoLidos.Add(item.IdContratoComentario);
                        }
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-VerificaQtdComentariosNaoLidos Contrato [" + idContrato + "]");

                    throw;
                }

            }
        }

        #endregion

        #region Documentos Principais    
        [HttpPost]
        [Route("UpLoadDocPrincipal/{IdTipoDocumento}/{IdContrato}/{NmCriador}")]
        public async Task<IActionResult> UpLoadDocPrincipal(short IdTipoDocumento, int IdContrato, string NmCriador)
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
                            // Inicia transação

                            var files = Request.Form.Files;

                            var itemDoc = new ContratoDocPrincipal();

                            var objDocumento = new bContrato(db);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    itemDoc.IdTipoDoc = IdTipoDocumento;
                                    itemDoc.IdContrato = IdContrato;
                                    itemDoc.NmCriador = NmCriador;
                                    itemDoc.DtUpLoad = DateTime.Now;
                                    itemDoc.DocFisico = fileBytes;
                                    itemDoc.NmDocumento = files[0].Name;
                                    itemDoc.DsDoc = files[0].Name;
                                }
                            }

                            objDocumento.AddDocumentoPrincipal(itemDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpLoadDocPrincipal Contrato [" + IdContrato + "] Criador [" + NmCriador + "]");

                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [HttpGet]
        [Route("ListaDocumentosPrincipais/{id}")]
        public List<OutPutDocumentoPrincipal> ListaDocumentosPrincipais(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaDocs = new List<OutPutDocumentoPrincipal>();

                    var documentos = new bContrato(db).BuscarDocumentosPrincipais(id);

                    foreach (var itemDoc in documentos)
                    {

                        var itemDocumento = new OutPutDocumentoPrincipal();
                        var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDoc).Single();

                        itemDocumento.IdContratoDocPrincipal = itemDoc.IdContratoDocPrincipal;
                        itemDocumento.IdContrato = itemDoc.IdContrato;
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

                        listaDocs.Add(itemDocumento);
                    }

                    return listaDocs;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaDocumentosPrincipais Contrato [" + id + "]");
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
                    var contrato = new bContrato(db).BuscarContratoId(id);
                    var item = new bContratoDoc(db).BuscaDocumentoAditivo(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaDocumentoAditivo [" + id + "]");

                    throw ex;
                }
            }
        }

        [HttpGet]
        [Route("BuscaTermoDocs/{id}")]
        public OutPutBuscaNomeDocs BuscaTermoDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var item = new bContratoDoc(db).BuscarTermoId(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaTermoDocs [" + id + "]");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaFinalDocs/{id}")]
        public OutPutBuscaNomeDocs BuscaFinalDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var item = new bContratoDoc(db).BuscarFinalId(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaFinalDocs [" + id + "]");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaContratoAssinado/{id}")]
        public OutPutBuscaNomeDocs BuscaContratoAssinado(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var item = new bContratoDoc(db).BuscaContratoAssinado(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaContratoAssinado [" + id + "]");

                    throw;
                }
            }
        }



        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Setembro/2020 
         *  Pesquisa os nomes dos arquivos de ADITIVO do Contrato pelo ID
         ===========================================================================================*/
        [HttpGet]
        [Route("BuscaDadosUltimoAditivo/{id}")]
        public OutPutDadosUltimoAditivo BuscaDadosUltimoAditivo(int id)
        {
            var retorno = new OutPutDadosUltimoAditivo();
            retorno.bPossuiAditivo = false;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    retorno = new bContrato(db).BuscaDadosUltimoAditivo(id);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaDadosUltimoAditivo [" + id + "]");
                    throw;
                }
            }
            return retorno;
        }




        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Setembro/2020 
         *  Pesquisa os nomes dos arquivos de ADITIVO do Contrato pelo ID
         ===========================================================================================*/
        [HttpGet]
        [Route("BuscaArquivosAditivos/{id}")]
        public OutPutDoctoAditivoPrincipal BuscaArquivosAditivos(int id)
        {
            var retorno = new OutPutDoctoAditivoPrincipal();
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var item = new bContratoDoc(db).BuscaArquivosAditivos(id);
                    if (item != null)
                    {
                        if (item.IdContrato != 0)
                        {
                            retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                            retorno.IdContrato             = item.IdContrato;
                            retorno.NmDocumento            = item.NmDocumento;
                            retorno.DsTipoDocumento        = item.DsTipoDocumento;
                            retorno.TextDown               = item.TextDown;
                            retorno.DtUpLoad               = item.DtUpLoad;
                            retorno.NmCriador              = item.NmCriador;
                            retorno.DocFisico              = item.DocFisico;
                        }
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaArquivosAditivos [" + id + "]");
                    throw;
                }
            }
            return retorno;
        }




        [HttpGet]
        [Route("BuscaOrdemInicio/{id}")]
        public OutPutBuscaNomeDocs BuscaOrdemInicio(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeDocs();
                    var item = new bContratoDoc(db).BuscaOrdemInicio(id);
                    if (item != null)
                    {
                        retorno.DsDoc = item.DsDoc;
                        retorno.IdContratoDocPrincipal = item.IdContratoDocPrincipal;
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaOrdemInicio [" + id + "]");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("DownloadDocPrincipal/{id}")]
        public async Task<IActionResult> DownloadDocPrincipal(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoDocumento = new bContrato(db).BuscarDocumentoPrincipalId(id);

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-DownloadDocPrincipal [" + id + "]");

                    return NotFound();
                }

                return NotFound();
            }
        }

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
                            var objDoc = new bContrato(db);

                            // Inicia transação


                            objDoc.RemoveDocumentoPrincipal(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ExcluirDocumentoPrincipal [" + id + "]");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }
        #endregion


        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        [Route("UpLoadDoc/{IdTipoDocumento}/{IdContrato}/{NmCriador}/{DtDoc}")]
        public async Task<IActionResult> UpLoadDoc(short IdTipoDocumento, int IdContrato, string NmCriador, string DtDoc)
        {
            using (var db = new FIPEContratosContext())
            {

                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Database.SetCommandTimeout(800);
                        try
                        {
                            //EGS 30.11.2020 Campo não é mais obrigatorio, solicitado por Nelson
                            DateTime sDtCalc = Convert.ToDateTime(DtDoc);
                            if (sDtCalc > DateTime.Now.Date) { DtDoc = ""; }

                            // Inicia transação
                            var files = Request.Form.Files;

                            var itemDoc = new ContratoDoc();

                            var objDocumento = new bContrato(db);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    itemDoc.IdTipoDoc = IdTipoDocumento;
                                    itemDoc.IdContrato = IdContrato;
                                    itemDoc.NmCriador = NmCriador;
                                    itemDoc.DtUpload = DateTime.Now;
                                    itemDoc.DocFisico = fileBytes;
                                    itemDoc.DsDoc = files[0].Name;
                                    if (DtDoc != "")
                                    {
                                        itemDoc.DtDoc = Convert.ToDateTime(DtDoc); //EGS 30.11.2020 Campo não é mais obrigatorio, solicitado por Nelson
                                    }
                                }
                            }

                            objDocumento.AddDocumento(itemDoc);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpLoadDoc Contrato [" + IdContrato + "] Criador [" + NmCriador + "]");
                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaDocumentos/{id}/{docContratual}")]
        public List<OutPutContratoDocumento> ListaDocumentos(int id, bool? docContratual)
        {
            using (var db = new FIPEContratosContext())
            {
                db.Database.SetCommandTimeout(800);
                try
                {
                    var listaDocs = new List<OutPutContratoDocumento>();

                    if (docContratual == true && docContratual != null)
                    {
                        var documentos = new bContrato(db).BuscarDocumentosContratuais(id);

                        foreach (var itemDoc in documentos)
                        {
                            var itemDocumento = new OutPutContratoDocumento();
                            var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDoc).Single();

                            itemDocumento.IdContratoDoc = itemDoc.IdContratoDocPrincipal;
                            itemDocumento.IdContrato = itemDoc.IdContrato;
                            itemDocumento.DsTipoDocumento = tipoDocumento.DsTipoDoc;
                            itemDocumento.DtUpLoad = itemDoc.DtUpLoad;
                            itemDocumento.NmCriador = itemDoc.NmCriador;
                            itemDocumento.NmDocumento = itemDoc.DsDoc;
                            itemDocumento.DsDoc = itemDoc.DsDoc;
                            itemDocumento.TextDown = "Download";

                            listaDocs.Add(itemDocumento);
                        }
                    }
                    else
                    {
                        var documentos = new bContrato(db).BuscarDocumentos(id, docContratual);

                        foreach (var itemDoc in documentos)
                        {

                            var itemDocumento = new OutPutContratoDocumento();
                            var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDoc).Single();

                            itemDocumento.IdContratoDoc = itemDoc.IdContratoDoc;
                            itemDocumento.IdContrato = itemDoc.IdContrato;
                            itemDocumento.DsTipoDocumento = tipoDocumento.DsTipoDoc;
                            itemDocumento.DtUpLoad = itemDoc.DtUpload;
                            itemDocumento.NmCriador = itemDoc.NmCriador;
                            itemDocumento.NmDocumento = itemDoc.DsDoc;
                            itemDocumento.DsDoc = itemDoc.DsDoc;
                            itemDocumento.TextDown = "Download";
                            itemDocumento.DtDoc = itemDoc.DtDoc;
                            if (itemDocumento.DsTipoDocumento == "Proposta Minuta")
                            {
                                itemDocumento.Minuta = true;
                            }

                            listaDocs.Add(itemDocumento);
                        }
                    }

                    return listaDocs;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaDocumentos [" + id + "]");

                    throw;
                }
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        [RequestSizeLimit(int.MaxValue)]
        [Route("UpdateDocumento")]
        public OutputUpdateDocumento UpdateDocumento([FromBody] InputUpdateDocumento item)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
                var retorno = new OutputUpdateDocumento();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.Database.SetCommandTimeout(800);
                        try
                        {
                            var contratoDoc = new bContratoDoc(db).GetById(item.IdContratoDoc);

                            contratoDoc.IdTipoDoc = item.IdTipoDoc;
                            contratoDoc.DtDoc = item.DtDoc;

                            retorno.Result = true;

                            db.SaveChanges();
                            db.Database.CommitTransaction();

                            return retorno;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateDocumento ContratoDoc [" + item.IdContratoDoc + "]");

                            throw;
                        }
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaDocumentoById/{id}")]
        public OutputTipoDocumentoId BuscaDocumentoById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                db.Database.SetCommandTimeout(800);
                try
                {
                    var documento = new bContrato(db).BuscarDocumentoId(id);

                    var itemDocumento = new OutputTipoDocumentoId();
                    var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == documento.IdTipoDoc).Single();

                    itemDocumento.IdTipoDocumento = tipoDocumento.IdTipoDoc;
                    itemDocumento.result = true;
                    itemDocumento.DsDoc = documento.DsDoc;
                    itemDocumento.DtDoc = documento.DtDoc;

                    return itemDocumento;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-BuscaDocumentoById [" + id + "]");

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
                db.Database.SetCommandTimeout(800);
                try
                {
                    var retornoDocumento = new bContrato(db).BuscarDocumentoId(id);

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-DownloadDoc [" + id + "]");

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
                        db.Database.SetCommandTimeout(800);
                        try
                        {
                            var objDoc = new bContrato(db);

                            // Inicia transação


                            objDoc.RemoveDocumento(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ExcluirDocumento [" + id + "]");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("ListaTipoDocs/{id}/{docContratual}")]
        public List<OutPutTipoDocumento> ListaTipoDocs(int id, bool? docContratual)
        {
            using (var db = new FIPEContratosContext())
            {
                db.Database.SetCommandTimeout(800);
                try
                {
                    var listatipoDocs = new List<OutPutTipoDocumento>();

                    var tipoDocumentos = new bContrato(db).BuscarTipoDocumentos(id, docContratual);

                    foreach (var itemDoc in tipoDocumentos)
                    {

                        var itemTipoDocumento = new OutPutTipoDocumento();

                        itemTipoDocumento.IdTipoDocumento = itemDoc.IdTipoDoc;
                        itemTipoDocumento.DsTipoDocumento = itemDoc.DsTipoDoc;
                        itemTipoDocumento.TipoDocumento = itemDoc.DsTipoDoc;

                        listatipoDocs.Add(itemTipoDocumento);
                    }

                    return listatipoDocs;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaTipoDocs [" + id + "]");

                    throw;
                }
            }
        }


        #region Contrato Dados de Cobrança

        [HttpPost]
        [Route("UpdateDadosCobranca")]
        public OutPutUpdateDadosCobranca UpdateDadosCobranca([FromBody] InputUpdateContratoDadosCobranca item)
        {
            var retorno = new OutPutUpdateDadosCobranca();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            var contratoDadosCobranca = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();
                            var contratoDocAcompanhaNfs = new List<ContratoDocsAcompanhaNf>();

                            contratoDadosCobranca.IdContrato = item.IdContrato;
                            if (item.IdTipoEntregaDocumento == 0)
                            {
                                contratoDadosCobranca.IdTipoEntregaDocumento = null;
                            }
                            else
                            {
                                contratoDadosCobranca.IdTipoEntregaDocumento = Convert.ToInt16(item.IdTipoEntregaDocumento);
                            }
                            if (item.IdFormaPagamento == 0)
                            {
                                contratoDadosCobranca.IdFormaPagamento = null;
                            }
                            else
                            {
                                contratoDadosCobranca.IdFormaPagamento = item.IdFormaPagamento;
                            }
                            contratoDadosCobranca.IcFatAprovEntregavel = item.IcFatAprovEntregavel;
                            contratoDadosCobranca.IcFatPedidoEmpenho = item.IcFatPedidoEmpenho;
                            contratoDadosCobranca.IdTipoCobranca = Convert.ToInt16(item.IdTipoCobranca);
                            contratoDadosCobranca.IdContaCorrente = item.IdContaCorrente;
                            contratoDadosCobranca.DsPrazoPagamento = item.DsPrazoPagamento;
                            contratoDadosCobranca.NuBanco = item.NuBanco;
                            contratoDadosCobranca.NuAgencia = item.NuAgencia;
                            contratoDadosCobranca.NuConta = item.NuConta;
                            contratoDadosCobranca.DsTextoCorpoNf = item.DsTextoCorpoNF;
                            contratoDadosCobranca.DsObservacao = item.DsTextoCorpoNF;
                            contratoDadosCobranca.DsPrazoPagamento = item.DsPrazoPagamento;

                            foreach (var itemDocs in item.DocAcompanhaNFs)
                            {
                                var itemDocAcompanhaNF = new ContratoDocsAcompanhaNf();

                                itemDocAcompanhaNF.IdContrato = item.IdContrato;
                                itemDocAcompanhaNF.IdTipoDocsAcompanhaNf = itemDocs.IdTipoDocsAcompanhaNF;
                                contratoDocAcompanhaNfs.Add(itemDocAcompanhaNF);
                            }

                            // Inicia transação

                            var removeDadosDocAcompanhaNF = db.ContratoDocsAcompanhaNf.Where(w => w.IdContrato == item.IdContrato).ToList();
                            if (removeDadosDocAcompanhaNF.Count > 0)
                            {
                                db.ContratoDocsAcompanhaNf.RemoveRange(removeDadosDocAcompanhaNF);
                            }

                            db.ContratoDocsAcompanhaNf.AddRange(contratoDocAcompanhaNfs);
                            db.SaveChanges();

                            // Grava registro                    
                            new bContrato(db).UpdateDadosCobranca(contratoDadosCobranca);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateDadosCobranca Contrato [" + item.IdContrato + "]");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        #endregion

        #region Grid Histórico do Contrato

        [HttpGet]
        [Route("ListaContratoHistorico/{idContrato}")]
        public List<OutPutGetContratoHistorico> ListaContratoHistorico(int idContrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaHistorico = new List<OutPutGetContratoHistorico>();

                    var historicos = new bContrato(db).ListaHistoricoContrato(idContrato);

                    foreach (var item in historicos)
                    {
                        var historico = new OutPutGetContratoHistorico();

                        var usuario = db.Usuario.Where(w => w.IdUsuario == item.IdUsuario).FirstOrDefault().IdPessoa;

                        historico.IdContratoHistorico = item.IdContratoHistorico;
                        historico.IdContrato = db.Contrato.Where(w => w.IdContrato == idContrato).FirstOrDefault().NuContratoEdit;
                        historico.DtInicio = item.DtInicio.ToString();
                        historico.DtFim = item.DtFim.ToString();
                        historico.DataInicial = item.DtInicio;
                        historico.dsSituacao = item.IdSituacaoNavigation.DsSituacao;
                        historico.nmUsuario = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario).FirstOrDefault().NmPessoa;
                        historico.dsEmailObserva = item.EmailObserva;
                        listaHistorico.Add(historico);
                    }

                    return listaHistorico.OrderByDescending(o => o.DataInicial).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaContratoHistorico Contrato [" + idContrato + "]");

                    throw;
                }
            }
        }

        #endregion

        #region Grid Entregaveis
        [HttpGet]
        [Route("ListaEntregaveis/{data}")]
        public List<OutPutGetEntregaveis> ListaEntregaveis(string data)
        {
            var entregaveis = new List<OutPutGetEntregaveis>();
            var dataEntregaveis = Convert.ToDateTime(data);

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaEntregaveis = new bContrato(db).GetEntregaveisByData(dataEntregaveis).Where(w => w.IdSituacaoNavigation.IcEntregue != true && w.IdSituacaoNavigation.IcEntidade == "E" && w.IdContratoNavigation.IdSituacao == 19).OrderBy(o => o.DtProduto);

                    foreach (var item in listaEntregaveis)
                    {
                        var entregavel = new OutPutGetEntregaveis();
                        var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();

                        entregavel.IdContrato = item.IdContrato;
                        entregavel.NuContratoEdit = contrato.NuContratoEdit;
                        entregavel.IdContratoEntregavel = item.IdContratoEntregavel;
                        entregavel.NmFrente = item.IdFrenteNavigation.NmFrente;
                        entregavel.DsProduto = item.DsProduto;
                        entregavel.DtProduto = item.DtProduto.HasValue ? item.DtProduto.Value.ToShortDateString() : "À definir";
                        entregavel.DsApelidoContrato = item.IdContratoNavigation.DsApelido;
                        entregavel.DsSituacao = item.IdSituacaoNavigation.DsSituacao;

                        entregaveis.Add(entregavel);
                    }
                    return entregaveis;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaEntregaveis");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaEntregaveisEmAtraso/{data}")]
        public List<OutPutGetEntregaveis> ListaEntregaveisEmAtraso(string data)
        {
            var entregaveis = new List<OutPutGetEntregaveis>();
            var dataEntregaveis = Convert.ToDateTime(data);

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaEntregaveis = new bContrato(db).GetEntregaveisEmAtraso(dataEntregaveis).Where(w => w.IdSituacaoNavigation.IcEntregue != true && w.IdSituacaoNavigation.IcEntidade == "E" && w.IdContratoNavigation.IdSituacao == 19).OrderBy(o => o.DtProduto);

                    foreach (var item in listaEntregaveis)
                    {
                        var entregavel = new OutPutGetEntregaveis();
                        var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();

                        entregavel.IdContrato = item.IdContrato;
                        entregavel.NuContratoEdit = contrato.NuContratoEdit;
                        entregavel.IdContratoEntregavel = item.IdContratoEntregavel;
                        entregavel.NmFrente = item.IdFrenteNavigation.NmFrente;
                        entregavel.DsProduto = item.DsProduto;
                        entregavel.DtProduto = item.DtProduto.HasValue ? item.DtProduto.Value.ToShortDateString() : "A definir";
                        entregavel.DsApelidoContrato = item.IdContratoNavigation.DsApelido;
                        entregavel.DsSituacao = item.IdSituacaoNavigation.DsSituacao;

                        entregaveis.Add(entregavel);
                    }
                    return entregaveis;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaEntregaveisEmAtraso");

                    throw;
                }
            }
        }
        #endregion

        #region Grid Faturamentos
        [HttpGet]
        [Route("ListaFaturamentos/{data}")]
        public List<OutPutGetFaturamentos> ListaFaturamentos(string data)
        {
            var faturamentos = new List<OutPutGetFaturamentos>();
            var dataFaturamentos = Convert.ToDateTime(data);

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaFaturamentos = new bContrato(db).GetFaturamentosByData(dataFaturamentos).Where(w => w.IdSituacaoNavigation.IcNfemitida != true && w.IdSituacaoNavigation.IcEntidade == "F" && w.IdContratoNavigation.IdSituacao == 19).OrderBy(o => o.DtFaturamento);

                    foreach (var item in listaFaturamentos)
                    {
                        var faturamento = new OutPutGetFaturamentos();

                        var contratoCli = db.ContratoCliente.Where(w => w.IdContratoCliente == item.IdContratoCliente).FirstOrDefault();
                        var idPessoa = db.Cliente.Where(w => w.IdCliente == contratoCli.IdCliente).FirstOrDefault().IdPessoa;
                        var idPessoaFisica = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                        if (idPessoaFisica != null)
                        {
                            faturamento.DsCliente = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                        }
                        var idPessoaJur = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaJuridica;
                        if (idPessoaJur != null)
                        {
                            faturamento.DsCliente = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().NmFantasia;
                        }
                        var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();

                        faturamento.IdContrato = item.IdContrato;
                        faturamento.NuContratoEdit = contrato.NuContratoEdit;
                        faturamento.IdContratoCronFinanceiro = item.IdContratoCronFinanceiro;
                        faturamento.DsApelidoContrato = item.IdContratoNavigation.DsApelido;
                        faturamento.DtFaturamento = item.DtFaturamento.HasValue ? item.DtFaturamento.Value.ToShortDateString() : "A definir";
                        faturamento.NuParcela = item.NuParcela;
                        faturamento.VlParcela = item.VlParcela;
                        faturamento.DsSituacao = item.IdSituacaoNavigation.DsSituacao;

                        faturamentos.Add(faturamento);
                    }
                    return faturamentos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaFaturamentos");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaFaturamentosEmAtraso/{data}")]
        public List<OutPutGetFaturamentos> ListaFaturamentosEmAtraso(string data)
        {
            var faturamentos = new List<OutPutGetFaturamentos>();
            var dataFaturamentos = Convert.ToDateTime(data);

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaFaturamentos = new bContrato(db).GetFaturamentosEmAtraso(dataFaturamentos).Where(w => w.IdSituacaoNavigation.IcNfemitida != true && w.IdSituacaoNavigation.IcEntidade == "F" && w.IdContratoNavigation.IdSituacao == 19).OrderBy(o => o.DtFaturamento);

                    foreach (var item in listaFaturamentos)
                    {
                        var faturamento = new OutPutGetFaturamentos();

                        var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == item.IdContratoCliente).FirstOrDefault().IdCliente;
                        var idPessoa = db.Cliente.Where(w => w.IdCliente == contratoCliente).FirstOrDefault().IdPessoa;
                        var idPessoaFisica = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                        if (idPessoaFisica != null)
                        {
                            faturamento.DsCliente = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                        }
                        var idPessoaJuridica = db.Cliente.Where(w => w.IdCliente == contratoCliente).FirstOrDefault().IdPessoa;
                        var idPessoaJur = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaJuridica;
                        if (idPessoaJur != null)
                        {
                            faturamento.DsCliente = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().NmFantasia;
                        }
                        var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();

                        faturamento.IdContrato = item.IdContrato;
                        faturamento.NuContratoEdit = contrato.NuContratoEdit;
                        faturamento.IdContratoCronFinanceiro = item.IdContratoCronFinanceiro;
                        faturamento.DsApelidoContrato = item.IdContratoNavigation.DsApelido;
                        faturamento.DtFaturamento = item.DtFaturamento.HasValue ? item.DtFaturamento.Value.ToShortDateString() : "A definir";
                        faturamento.NuParcela = item.NuParcela;
                        faturamento.VlParcela = item.VlParcela;
                        faturamento.DsSituacao = item.IdSituacaoNavigation.DsSituacao;

                        faturamentos.Add(faturamento);
                    }
                    return faturamentos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaFaturamentosEmAtraso");


                    throw;
                }
            }
        }
        #endregion

        #region Grid Aditivos

        [HttpGet]
        [Route("ListaAditivos/{idContrato}")]
        public List<OutPutGetAditivos> ListaAditivos(int idContrato)
        {
            var aditivos = new List<OutPutGetAditivos>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaAditivos = new bContrato(db).ListaAditivos(idContrato);

                    foreach (var item in listaAditivos)
                    {
                        var aditivo = new OutPutGetAditivos();
                        var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();

                        aditivo.IdContrato = item.IdContrato;
                        aditivo.NuContratoEdit = contrato.NuContratoEdit;
                        aditivo.IdContratoAditivo = item.IdContratoAditivo;
                        aditivo.DsApelidoContrato = item.IdContratoNavigation.DsApelido;
                        aditivo.DsSituacao = item.IdSituacaoNavigation.DsSituacao;
                        string tiposAditivo = string.Empty;
                        if (item.IcAditivoData != null && item.IcAditivoData.Value && item.IcAditivoEscopo != null && item.IcAditivoEscopo.Value
                            && item.IcAditivoValor != null && item.IcAditivoValor.Value)
                        {
                            tiposAditivo = "Data , Escopo e Valor";
                        }
                        else if (item.IcAditivoData != null && item.IcAditivoData.Value && item.IcAditivoEscopo != null && item.IcAditivoEscopo.Value)
                        {
                            tiposAditivo = "Data e Escopo";
                        }
                        else if (item.IcAditivoData != null && item.IcAditivoData.Value && item.IcAditivoValor != null && item.IcAditivoValor.Value)
                        {
                            tiposAditivo = "Data e Valor";
                        }
                        else if (item.IcAditivoEscopo != null && item.IcAditivoEscopo.Value && item.IcAditivoValor != null && item.IcAditivoValor.Value)
                        {
                            tiposAditivo = "Escopo e Valor";
                        }
                        else if (item.IcAditivoData != null && item.IcAditivoData.Value)
                        {
                            tiposAditivo = "Data";
                        }
                        else if (item.IcAditivoEscopo != null && item.IcAditivoEscopo.Value)
                        {
                            tiposAditivo = "Escopo";
                        }
                        else if (item.IcAditivoValor != null && item.IcAditivoValor.Value)
                        {
                            tiposAditivo = "Valor";
                        }
                        aditivo.DsTipoAditivo = tiposAditivo;
                        aditivo.NuAditivo = item.NuAditivo;
                        aditivo.NuAditivoCliente = item.NuAditivoCliente;
                        aditivo.DtCriacao = item.DtCriacao;
                        aditivo.DtAplicacao = item.DtAplicacao;
                        if (item.IdUsuarioAplicacao != null)
                        {
                            var pessoaCriadora = db.Usuario.Where(w => w.IdUsuario == item.IdUsuarioCriacao).FirstOrDefault().IdPessoa;
                            aditivo.DsUsuarioAplicacao = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoaCriadora).FirstOrDefault().NmPessoa;
                        }
                        aditivos.Add(aditivo);
                    }
                    return aditivos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaAditivos Contrato [" + idContrato + "]");

                    throw;
                }
            }
        }

        #endregion

        #region Contrato Tomador de Serviço NF

        [HttpGet]
        [Route("GetByIdCronogramaFinanceiro/{id}")]
        public OutputGetContratoTomadorServico GetByIdCronogramaFinanceiro(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var Faturamento = new bContrato(db).GetByIdCronogramaFinanceiro(id);
                    var faturamentoNF = new OutputGetContratoTomadorServico();

                    var contratoCli = db.ContratoCliente.Where(w => w.IdContratoCliente == Faturamento.IdContratoCliente).FirstOrDefault();
                    var idPessoa = db.Cliente.Where(w => w.IdCliente == contratoCli.IdCliente).FirstOrDefault().IdPessoa;
                    var idPessoaFisica = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                    if (idPessoaFisica != null)
                    {
                        faturamentoNF.DsNome = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                        faturamentoNF.DsCpf = FormatCPF(db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NuCpf);
                    }
                    var idPessoaJur = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaJuridica;
                    if (idPessoaJur != null)
                    {
                        faturamentoNF.DsRazaoSocial = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().NmFantasia;
                        faturamentoNF.DsCnpj = FormatCNPJ(db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().Cnpj);
                    }
                    faturamentoNF.IdContrato = Faturamento.IdContrato;
                    faturamentoNF.NuAgencia = Faturamento.IdContratoNavigation.NuAgencia;
                    faturamentoNF.NuBanco = Faturamento.IdContratoNavigation.NuBanco;
                    faturamentoNF.NuConta = Faturamento.IdContratoNavigation.NuConta;
                    faturamentoNF.NuParcela = Faturamento.NuParcela;
                    faturamentoNF.NuCentroCusto = Faturamento.IdContratoNavigation.NuCentroCusto;
                    faturamentoNF.NuNotaFiscal = Faturamento.NuNotaFiscal;
                    faturamentoNF.DtNotaFiscal = Faturamento.DtNotaFiscal;
                    faturamentoNF.IdSituacao = Faturamento.IdSituacao;
                    faturamentoNF.DsAssunto = Faturamento.IdContratoNavigation.DsAssunto;
                    faturamentoNF.VlParcela = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0,0.00}", Faturamento.VlParcela);
                    faturamentoNF.DsTextoCorpoNF = db.Contrato.Where(w => w.IdContrato == Faturamento.IdContrato).FirstOrDefault().DsObservacao;

                    //var textoNfs = Faturamento.IdContratoNavigation.DsTextoCorpoNf.Split("?");

                    //for (int i = 0; i < textoNfs.Length; i++)
                    //{
                    //    switch (i)
                    //    {
                    //        case 0:
                    //            textoNfs[i] += faturamentoNF.IdContrato;
                    //            break;
                    //        case 1:
                    //            textoNfs[i] += faturamentoNF.NuParcela;
                    //            break;
                    //        case 2:
                    //            textoNfs[i] += faturamentoNF.NuCentroCusto;
                    //            break;
                    //        case 3:
                    //            textoNfs[i] += faturamentoNF.DsAssunto;
                    //            break;
                    //        case 4:
                    //            textoNfs[i] += faturamentoNF.NuBanco;
                    //            break;
                    //        case 5:
                    //            textoNfs[i] += faturamentoNF.NuAgencia;
                    //            break;
                    //        case 6:
                    //            textoNfs[i] += faturamentoNF.NuConta;
                    //            break;
                    //    }                        
                    //}
                    return faturamentoNF;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-GetByIdCronogramaFinanceiro Contrato [" + id + "]");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetByIdCronogramaFinanceiroTemporaria/{id}")]
        public OutputGetContratoTomadorServico GetByIdCronogramaFinanceiroTemporaria(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var Faturamento = new bContrato(db).GetByIdCronogramaFinanceiroTemporaria(id);
                    var faturamentoNF = new OutputGetContratoTomadorServico();

                    var contratoCli = db.ContratoCliente.Where(w => w.IdContratoCliente == Faturamento.IdContratoCliente).FirstOrDefault();
                    var idPessoa = db.Cliente.Where(w => w.IdCliente == contratoCli.IdCliente).FirstOrDefault().IdPessoa;
                    var idPessoaFisica = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                    if (idPessoaFisica != null)
                    {
                        faturamentoNF.DsNome = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                        faturamentoNF.DsCpf = FormatCPF(db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NuCpf);
                    }
                    var idPessoaJur = db.Pessoa.Where(W => W.IdPessoa == idPessoa).FirstOrDefault().IdPessoaJuridica;
                    if (idPessoaJur != null)
                    {
                        faturamentoNF.DsRazaoSocial = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().NmFantasia;
                        faturamentoNF.DsCnpj = FormatCNPJ(db.PessoaJuridica.Where(w => w.IdPessoaJuridica == idPessoaJur).FirstOrDefault().Cnpj);
                    }
                    faturamentoNF.IdContrato = Faturamento.IdContrato;
                    faturamentoNF.NuAgencia = Faturamento.IdContratoNavigation.NuAgencia;
                    faturamentoNF.NuBanco = Faturamento.IdContratoNavigation.NuBanco;
                    faturamentoNF.NuConta = Faturamento.IdContratoNavigation.NuConta;
                    faturamentoNF.NuParcela = Faturamento.NuParcela;
                    faturamentoNF.NuCentroCusto = Faturamento.IdContratoNavigation.NuCentroCusto;
                    faturamentoNF.NuNotaFiscal = Faturamento.NuNotaFiscal;
                    faturamentoNF.DtNotaFiscal = Faturamento.DtNotaFiscal;
                    faturamentoNF.IdSituacao = Faturamento.IdSituacao;
                    faturamentoNF.DsAssunto = Faturamento.IdContratoNavigation.DsAssunto;
                    faturamentoNF.VlParcela = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0,0.00}", Faturamento.VlParcela);
                    faturamentoNF.DsTextoCorpoNF = Faturamento.DsObservacao;

                    return faturamentoNF;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-GetByIdCronogramaFinanceiroTemporaria Contrato [" + id + "]");

                    throw;
                }
            }
        }

        [HttpPut]
        [Route("UpdateNfTomadorServico")]
        public OutPutNfTomadorServico UpdateNfTomadorServico([FromBody]InputUpdateNfTomadorServico item)
        {
            var retorno = new OutPutNfTomadorServico();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            var inputItem = db.ContratoCronogramaFinanceiro.Where(w => w.IdContratoCronFinanceiro == item.IdContratoCronFinanceiro).FirstOrDefault();


                            inputItem.IdContratoCronFinanceiro = item.IdContratoCronFinanceiro;
                            inputItem.IdSituacao = item.IdSituacao;
                            inputItem.NuNotaFiscal = item.NuNotaFiscal;
                            inputItem.DsTextoCorpoNf = item.DsTextoCorpoNF;
                            inputItem.DtNotaFiscal = item.DtNotaFiscal;

                            var addRetorno = new bContrato(db).UpdateNfTomadorServico(inputItem);

                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateNfTomadorServico");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        #endregion

        #region Formatar CNPJ
        public static string FormatCNPJ(string CNPJ)
        {
            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }
        #endregion

        #region Formatar CPF
        public static string FormatCPF(string CPF)
        {
            return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
        }
        #endregion

        #region Renovação Automática
        //Renovação Automática
        [HttpGet]
        [Route("ConsultarRenovacao/{idContrato}/{idContratoRenovacao}")]
        public OutPutGetRenovacao ConsultarRenovacao(int idContrato, int idContratoRenovacao)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var itemProrrogacao = new OutPutGetRenovacao();
                    var prorrogacao = new ContratoProrrogacao();

                    if (idContratoRenovacao != 0)
                    {
                        prorrogacao = db.ContratoProrrogacao.Where(w => w.IdContratoRenovacao == idContratoRenovacao).LastOrDefault();
                    }
                    else
                    {
                        prorrogacao = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato && w.IdSituacao != 101).LastOrDefault();
                    }

                    var contrato = db.Contrato.Where(w => w.IdContrato == idContrato).FirstOrDefault();
                    var pessoaCriadora = db.Usuario.Where(w => w.IdUsuario == contrato.IdUsuarioCriacao).FirstOrDefault().IdPessoa;

                    var ContratoProrrogacao = new ContratoProrrogacao();

                    itemProrrogacao.DtCriacao = contrato.DtCriacao.ToShortDateString();
                    itemProrrogacao.NmUsuario = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoaCriadora).FirstOrDefault().NmPessoa;
                    itemProrrogacao.NuCentroCusto = contrato.NuContratoEdit;
                    itemProrrogacao.idProposta = contrato.IdProposta;
                    if (prorrogacao != null)
                    {
                        itemProrrogacao.IdContratoRenovacao = prorrogacao.IdContratoRenovacao;
                        itemProrrogacao.NuPrazoMesesContrato = contrato.DsPrazoExecucao;
                        itemProrrogacao.NuPrazoRenovacaoMeses = prorrogacao.NuPrazoRenovacaoMeses.ToString();
                        itemProrrogacao.DtInicioVigenciaContrato = contrato.DtInicio.Value;
                        itemProrrogacao.dtFimVigenciaAtualContrato = contrato.DtFim.Value;
                        itemProrrogacao.DtFimVigenciaProrrogada = prorrogacao.DtFimVigenciaRenovacao;
                        itemProrrogacao.DtInicioExecucaoContrato = contrato.DtInicioExecucao;
                        itemProrrogacao.DtFimExecucaoContrato = contrato.DtFimExecucao;
                        itemProrrogacao.DtFimExecucaoRenovacao = prorrogacao.DtFimExecucaoRenovacao;
                        itemProrrogacao.VlContratoAntesRenovacao = prorrogacao.VlContratoAntesRenovacao;
                        itemProrrogacao.VlContratoRenovado = prorrogacao.VlContratoRenovado;
                        itemProrrogacao.FatorReajuste = prorrogacao.PcReajuste;
                        itemProrrogacao.IdUsuarioAplicacao = prorrogacao.IdUsuarioAplicacao;
                        itemProrrogacao.IcEntregaveisCopiado = prorrogacao.IcEntregaveisCopiado;
                        itemProrrogacao.IcCronogramaFinanceiroCopiado = prorrogacao.IcCronogramaFinanceiroCopiado;
                    }
                    else
                    {
                        var contContratoRenovacao = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato).Count() != 0 ? db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato).OrderBy(o => o.NuRenovacao).LastOrDefault().NuRenovacao + 1 : 1;


                        // item para salvar na tabela contratoProrrogacao                        
                        ContratoProrrogacao.IdContrato = contrato.IdContrato;
                        ContratoProrrogacao.IdSituacao = 102;
                        ContratoProrrogacao.NuRenovacao = contContratoRenovacao;
                        ContratoProrrogacao.DtInicioVigencia = contrato.DtInicio.Value;
                        ContratoProrrogacao.DtFimVigenciaAtual = contrato.DtFim.Value;
                        ContratoProrrogacao.VlContratoAntesRenovacao = ContratoProrrogacao.VlContratoAntesRenovacao == null ? contrato.VlContrato : ContratoProrrogacao.VlContratoAntesRenovacao;
                        ContratoProrrogacao.DsPrazoExecucao = contrato.DsPrazoExecucao;

                        var retorno = new bContrato(db).AddProrrogacao(ContratoProrrogacao);

                        if (retorno != null)
                        {
                            itemProrrogacao.IdContratoRenovacao = retorno;
                            itemProrrogacao.NuPrazoMesesContrato = contrato.DsPrazoExecucao;
                            //itemProrrogacao.NuPrazoRenovacaoMeses = prorrogacao.NuPrazoRenovacaoMeses;                        
                            itemProrrogacao.DtInicioVigenciaContrato = contrato.DtInicio;
                            itemProrrogacao.dtFimVigenciaAtualContrato = contrato.DtFim;
                            //itemProrrogacao.DtFimVigenciaProrrogada = prorrogacao.DtFimVigenciaRenovacao;
                            itemProrrogacao.DtInicioExecucaoContrato = contrato.DtInicioExecucao;
                            itemProrrogacao.DtFimExecucaoContrato = contrato.DtFimExecucao;
                            //itemProrrogacao.DtFimExecucaoRenovacao = prorrogacao.DtFimExecucaoRenovacao;
                            itemProrrogacao.VlContratoAntesRenovacao = ContratoProrrogacao.VlContratoAntesRenovacao;
                            //itemProrrogacao.VlContratoRenovado = prorrogacao.VlContratoRenovado;
                        }
                    }

                    return itemProrrogacao;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ConsultarRenovacao Contrato [" + idContrato + "]");

                    throw;
                }
            }
        }

        [HttpPut]
        [Route("UpdateRenovacao")]
        public bool UpdateRenovacao([FromBody] InputUpdateRenovacao item)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
                bool retorno = false;

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var itemRenovacao = db.ContratoProrrogacao.Where(w => w.IdContratoRenovacao == item.IdContratoRenovacao).FirstOrDefault();

                            itemRenovacao.IdContratoRenovacao = item.IdContratoRenovacao;
                            itemRenovacao.NuPrazoRenovacaoMeses = Int32.Parse(item.NuPrazoRenovacaoMeses);
                            itemRenovacao.DtInicioVigencia = item.DtInicioVigenciaContrato.Value;
                            itemRenovacao.DtFimVigenciaAtual = item.dtFimVigenciaAtualContrato.Value;
                            itemRenovacao.DtFimVigenciaRenovacao = item.DtFimVigenciaProrrogada.Value;
                            itemRenovacao.DtFimExecucaoRenovacao = item.DtFimExecucaoRenovacao;
                            itemRenovacao.VlContratoAntesRenovacao = item.VlContratoAntesRenovacao;
                            itemRenovacao.VlContratoRenovado = item.VlContratoRenovado;
                            if (!string.IsNullOrEmpty(item.FatorReajuste.ToString()))
                            {
                                itemRenovacao.PcReajuste = Math.Round(item.FatorReajuste.Value, 4);
                            }

                            retorno = new bContrato(db).UpDateRenovacao(itemRenovacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateRenovacao ContratoRenov [" + item.IdContratoRenovacao + "]");

                            retorno = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateRenovacaoValor")]
        public bool UpdateRenovacaoValor([FromBody] InputUpdateRenovacao item)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
                bool retorno = false;

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var itemRenovacao = db.ContratoProrrogacao.Where(w => w.IdContratoRenovacao == item.IdContratoRenovacao).FirstOrDefault();

                            if (item.VlContratoRenovado != itemRenovacao.VlContratoRenovado)
                            {
                                itemRenovacao.PcReajuste = item.FatorReajuste;
                            }
                            itemRenovacao.VlContratoRenovado = item.VlContratoRenovado;

                            retorno = new bContrato(db).UpDateRenovacao(itemRenovacao);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-UpdateRenovacaoValor");

                            retorno = false;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("CopiarEntregaveisCronogramaFinanceiro/{idContrato}/{idContratoRenovacao}")]
        public bool CopiarEntregaveisCronogramaFinanceiro(int idContrato, int idContratoRenovacao)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
                bool retorno = false;

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contrato = db.Contrato.Where(w => w.IdContrato == idContrato).FirstOrDefault();
                            var renovacao = db.ContratoProrrogacao.Where(w => w.IdContratoRenovacao == idContratoRenovacao).FirstOrDefault();
                            var contRenovacoes = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato && w.IdSituacao == 101).ToList();
                            var lstCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();
                            var lstContratoEntregavel = new List<ContratoEntregavel>();

                            decimal novoValorContrato = 0;

                            var lstEntregavelTemporaria = new List<ContratoEntregavelTemporaria>();
                            if (contRenovacoes.Count > 0)
                            {
                                var dataInicialRenovacao = contRenovacoes.OrderBy(o => o.NuRenovacao).LastOrDefault().DtFimVigenciaAtual;

                                lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == idContrato && w.DtFaturamento >= dataInicialRenovacao && w.DtFaturamento <= contrato.DtFim).OrderBy(w => w.IdContratoCronFinanceiro).ToList();
                                lstContratoEntregavel = db.ContratoEntregavel.Where(w => w.IdContrato == idContrato && w.DtProduto >= dataInicialRenovacao).OrderBy(w => w.IdContratoEntregavel).ToList();
                            }
                            else
                            {
                                lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == idContrato && w.DtFaturamento >= contrato.DtInicio && w.DtFaturamento <= contrato.DtFim).OrderBy(w => w.IdContratoCronFinanceiro).ToList();
                                lstContratoEntregavel = db.ContratoEntregavel.Where(w => w.IdContrato == idContrato).OrderBy(w => w.IdContratoEntregavel).ToList();
                            }

                            var contCronFinan = lstCronogramaFinanceiro != null ? lstCronogramaFinanceiro.OrderBy(o => o.NuParcela).LastOrDefault().NuParcela : 0;
                            var contEntregaveis = lstContratoEntregavel != null ? lstContratoEntregavel.OrderBy(o => o.VlOrdem).LastOrDefault().VlOrdem : 0;
                            //var dataRenovacao = renovacao.DtFimVigenciaRenovacao.Value - contrato.DtFim.Value;

                            foreach (var contratoEntregavel in lstContratoEntregavel)
                            {
                                contEntregaveis++;

                                var contratoEntregavelTemporaria = new ContratoEntregavelTemporaria();
                                contratoEntregavelTemporaria.IdEntregavel = contratoEntregavel.IdContratoEntregavel;
                                contratoEntregavelTemporaria.IdContrato = contratoEntregavel.IdContrato;
                                contratoEntregavelTemporaria.DsProduto = contratoEntregavel.DsProduto;
                                contratoEntregavelTemporaria.DtProduto = contratoEntregavel.DtProduto.Value.AddMonths(renovacao.NuPrazoRenovacaoMeses.Value);
                                contratoEntregavelTemporaria.IdContratoCliente = contratoEntregavel.IdContratoCliente.Value;
                                contratoEntregavelTemporaria.IdFrente = contratoEntregavel.IdFrente;
                                if (contratoEntregavelTemporaria.DtProduto <= DateTime.Now)
                                {
                                    contratoEntregavelTemporaria.IdSituacao = 68;
                                    contratoEntregavelTemporaria.IcAtraso = true;
                                }
                                else
                                {
                                    contratoEntregavelTemporaria.IdSituacao = 56;
                                    contratoEntregavelTemporaria.IcAtraso = false;
                                }
                                contratoEntregavelTemporaria.VlOrdem = contEntregaveis;

                                db.ContratoEntregavelTemporaria.Add(contratoEntregavelTemporaria);
                                //db.SaveChanges();

                                lstEntregavelTemporaria.Add(contratoEntregavelTemporaria);
                            }

                            foreach (var cronogramaFinanceiro in lstCronogramaFinanceiro)
                            {
                                contCronFinan++;

                                // Retorna somente os dois ultimos caracteres do Código da Parcela
                                string codigoReajusteParcela = cronogramaFinanceiro.CdParcela.Substring(cronogramaFinanceiro.CdParcela.Length - 2, 2);
                                string cdCodPar = "";
                                // Remove os dois ultimos caracteres do Código da Parcela
                                //cronogramaFinanceiro.CdParcela = cronogramaFinanceiro.CdParcela.Remove(cronogramaFinanceiro.CdParcela.Length - 2, 2);
                                cdCodPar = cronogramaFinanceiro.CdParcela.Remove(cronogramaFinanceiro.CdParcela.Length - 2, 2);
                                // Soma 1 nos dois ultimos caracteres do Código da Parcela
                                int novoCdReajusteParcela = Convert.ToInt32(codigoReajusteParcela) + 1;
                                codigoReajusteParcela = novoCdReajusteParcela.ToString().Length > 1 ? novoCdReajusteParcela.ToString() : "0" + novoCdReajusteParcela.ToString();
                                cdCodPar += codigoReajusteParcela;

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

                                var contratoCronogramaFinanceiroTemporaria = new ContratoCronogramaFinanceiroTemporaria();
                                contratoCronogramaFinanceiroTemporaria.IdParcela = cronogramaFinanceiro.IdContratoCronFinanceiro;
                                contratoCronogramaFinanceiroTemporaria.CdIss = cronogramaFinanceiro.CdIss;
                                //contratoCronogramaFinanceiroTemporaria.CdParcela = cronogramaFinanceiro.CdParcela;
                                contratoCronogramaFinanceiroTemporaria.CdParcela = cdCodPar;
                                contratoCronogramaFinanceiroTemporaria.DsTextoCorpoNf = cronogramaFinanceiro.DsTextoCorpoNf;
                                contratoCronogramaFinanceiroTemporaria.DtFaturamento = cronogramaFinanceiro.DtFaturamento.Value.AddMonths(renovacao.NuPrazoRenovacaoMeses.Value);
                                contratoCronogramaFinanceiroTemporaria.DtNotaFiscal = cronogramaFinanceiro.DtNotaFiscal;
                                contratoCronogramaFinanceiroTemporaria.IdContratoCliente = cronogramaFinanceiro.IdContratoCliente;
                                if (contratoCronogramaFinanceiroTemporaria.DtFaturamento <= DateTime.Now)
                                {
                                    contratoCronogramaFinanceiroTemporaria.IdSituacao = 93;
                                    contratoCronogramaFinanceiroTemporaria.IcAtraso = true;
                                }
                                else
                                {
                                    contratoCronogramaFinanceiroTemporaria.IdSituacao = 92;
                                    contratoCronogramaFinanceiroTemporaria.IcAtraso = false;
                                }
                                contratoCronogramaFinanceiroTemporaria.NuNotaFiscal = cronogramaFinanceiro.NuNotaFiscal;
                                contratoCronogramaFinanceiroTemporaria.NuParcela = Convert.ToInt16(contCronFinan);
                                if (renovacao.PcReajuste != null)
                                {
                                    contratoCronogramaFinanceiroTemporaria.VlParcela = cronogramaFinanceiro.VlParcela * (1 + (renovacao.PcReajuste.Value / 100));
                                }
                                else
                                {
                                    contratoCronogramaFinanceiroTemporaria.VlParcela = cronogramaFinanceiro.VlParcela;
                                }
                                contratoCronogramaFinanceiroTemporaria.IdContrato = cronogramaFinanceiro.IdContrato;
                                contratoCronogramaFinanceiroTemporaria.IdFrente = cronogramaFinanceiro.IdFrente;

                                novoValorContrato = novoValorContrato + contratoCronogramaFinanceiroTemporaria.VlParcela;

                                db.ContratoCronogramaFinanceiroTemporaria.Add(contratoCronogramaFinanceiroTemporaria);
                                //db.SaveChanges();

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

                            //Atualiza a tabela ContratoProrrogacao
                            renovacao.IcCronogramaFinanceiroCopiado = true;
                            renovacao.IcEntregaveisCopiado = true;
                            // soma do valor do contrato antes da renovação + valor total das parcelas renovadas
                            renovacao.VlContratoRenovado = renovacao.VlContratoAntesRenovacao + novoValorContrato;
                            lstCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();
                            lstContratoEntregavel = new List<ContratoEntregavel>();

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-CopiarEntregaveisCronogramaFinanceiro Contrato [" + idContrato + "]");

                            retorno = false;
                        }
                    }
                });

                return retorno;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("AplicarRenovacao/{idContrato}/{idContratoRenovacao}/{idUsuario}")]
        public bool AplicarRenovacao(int idContrato, int idContratoRenovacao, int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();
                bool retorno = false;

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var contrato = db.Contrato.Where(w => w.IdContrato == idContrato).FirstOrDefault();
                            var contratoUpdate = new InputUpdateContrato();
                            var renovacao = db.ContratoProrrogacao.Where(w => w.IdContratoRenovacao == idContratoRenovacao).FirstOrDefault();
                            var ContratoRenovacoes = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato).ToList();
                            var lstContratoCronogramaFinanceiroTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato).ToList();
                            var lstEntregaveisTemporaria = db.ContratoEntregavelTemporaria.Where(w => w.IdContrato == contrato.IdContrato).ToList();
                            decimal valorParcelasAtuais = 0;
                            decimal valorParcelasAntigas = 0;

                            var contRenovacoes = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato).ToList();
                            var lstCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();

                            var contRenovacoesAplicadas = contRenovacoes.Where(w => w.IdSituacao == 101).ToList();

                            if (contRenovacoesAplicadas.Count > 0)
                            {
                                var dataInicialRenovacao = contRenovacoesAplicadas.OrderBy(o => o.NuRenovacao).LastOrDefault().DtFimVigenciaAtual;
                                lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == idContrato && w.DtFaturamento >= dataInicialRenovacao && w.DtFaturamento <= contrato.DtFim).OrderBy(w => w.IdContratoCronFinanceiro).ToList();
                            }

                            if (contRenovacoes.Count == 1)
                            {
                                lstCronogramaFinanceiro = new List<ContratoCronogramaFinanceiro>();
                                lstCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == idContrato && w.DtFaturamento >= contrato.DtInicio && w.DtFaturamento <= contrato.DtFim).OrderBy(w => w.IdContratoCronFinanceiro).ToList();
                            }


                            foreach (var item in lstCronogramaFinanceiro)
                            {
                                valorParcelasAntigas = valorParcelasAntigas + item.VlParcela;
                            }

                            foreach (var item in lstContratoCronogramaFinanceiroTemporaria)
                            {
                                valorParcelasAtuais = valorParcelasAtuais + item.VlParcela;
                            }

                            decimal valorReajusteAcumulado = 0;

                            //Atualiza a tabela ContratoProrrogacao
                            renovacao.IdSituacao = 101;
                            renovacao.DtAplicacao = DateTime.Now;
                            renovacao.IdUsuarioAplicacao = idUsuario;
                            renovacao.VlReajustePeriodo = Math.Round(valorParcelasAtuais) - Math.Round(valorParcelasAntigas);
                            renovacao.IcHistoricoCopiado = true;
                            foreach (var item in ContratoRenovacoes)
                            {
                                valorReajusteAcumulado = valorReajusteAcumulado + item.VlReajustePeriodo.Value;
                            }
                            renovacao.VlReajusteAcumulado = valorReajusteAcumulado;

                            //Atualiza tabela contrato                            
                            contratoUpdate.IdSituacao = 112;
                            contratoUpdate.IdContrato = idContrato;
                            contratoUpdate.IdUsuarioUltimaAlteracao = idUsuario;

                            new bContrato(db).UpdateRenovacaoHistorico(contratoUpdate);

                            contrato.VlContrato = renovacao.VlContratoRenovado.Value;
                            contrato.DtFim = renovacao.DtFimVigenciaRenovacao.Value;
                            contrato.DtFimExecucao = renovacao.DtFimExecucaoRenovacao.Value;
                            contrato.IdSituacao = 19;

                            // Atualiza parcelas e entregáveis no Contrato
                            var lstContratoCronogramaFinanceiroAntigos = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == contrato.IdContrato).ToList();
                            var lstContratoEntregaveisAntigos = db.ContratoEntregavel.Where(w => w.IdContrato == contrato.IdContrato).ToList();


                            foreach (var entregavelTemp in lstEntregaveisTemporaria)
                            {
                                var contratoEntregavel = new ContratoEntregavel();
                                contratoEntregavel.IdContrato = entregavelTemp.IdContrato;
                                contratoEntregavel.DsProduto = entregavelTemp.DsProduto;
                                contratoEntregavel.DtProduto = entregavelTemp.DtProduto;
                                contratoEntregavel.IdContratoCliente = entregavelTemp.IdContratoCliente.Value;
                                contratoEntregavel.IdFrente = entregavelTemp.IdFrente;
                                contratoEntregavel.IdSituacao = entregavelTemp.IdSituacao;
                                contratoEntregavel.VlOrdem = entregavelTemp.VlOrdem;

                                db.ContratoEntregavel.Add(contratoEntregavel);
                                db.SaveChanges();

                                entregavelTemp.IdEntregavel = contratoEntregavel.IdContratoEntregavel;
                            }

                            foreach (var parcelaTemp in lstContratoCronogramaFinanceiroTemporaria)
                            {
                                var contratoCronogramaFinanceiro = new ContratoCronogramaFinanceiro();
                                contratoCronogramaFinanceiro.CdIss = parcelaTemp.CdIss;
                                // Retorna somente os dois ultimos caracteres do Código da Parcela
                                //string codigoReajusteParcela = parcelaTemp.CdParcela.Substring(parcelaTemp.CdParcela.Length - 2, 2);
                                //// Remove os dois ultimos caracteres do Código da Parcela
                                //parcelaTemp.CdParcela = parcelaTemp.CdParcela.Remove(parcelaTemp.CdParcela.Length - 2, 2);
                                //// Soma 1 nos dois ultimos caracteres do Código da Parcela
                                //int novoCdReajusteParcela = Convert.ToInt32(codigoReajusteParcela) + 1;
                                //codigoReajusteParcela = novoCdReajusteParcela.ToString().Length > 1 ? novoCdReajusteParcela.ToString() : "0" + novoCdReajusteParcela.ToString();
                                //parcelaTemp.CdParcela += codigoReajusteParcela;
                                contratoCronogramaFinanceiro.CdParcela = parcelaTemp.CdParcela;
                                contratoCronogramaFinanceiro.DsTextoCorpoNf = parcelaTemp.DsTextoCorpoNf;
                                contratoCronogramaFinanceiro.DtFaturamento = parcelaTemp.DtFaturamento;
                                contratoCronogramaFinanceiro.DtNotaFiscal = parcelaTemp.DtNotaFiscal;
                                contratoCronogramaFinanceiro.IdContratoCliente = parcelaTemp.IdContratoCliente;
                                contratoCronogramaFinanceiro.IdSituacao = parcelaTemp.IdSituacao;
                                contratoCronogramaFinanceiro.NuNotaFiscal = parcelaTemp.NuNotaFiscal;
                                contratoCronogramaFinanceiro.NuParcela = parcelaTemp.NuParcela;
                                contratoCronogramaFinanceiro.VlParcela = parcelaTemp.VlParcela;
                                contratoCronogramaFinanceiro.IcAtraso = parcelaTemp.IcAtraso;
                                contratoCronogramaFinanceiro.IdContrato = parcelaTemp.IdContrato;
                                contratoCronogramaFinanceiro.IdFrente = parcelaTemp.IdFrente;

                                db.ContratoCronogramaFinanceiro.Add(contratoCronogramaFinanceiro);
                                db.SaveChanges();

                                // Busca os Registros na Tabela ContratoParcelaEntregavelTemporaria para verificar se a Parcela estava ligada com Entregaveis
                                var lstContratoParcelaEntregavelTemp = db.ContratoParcelaEntregavelTemporaria.Where(w => w.IdParcela == parcelaTemp.IdContratoCronFinanceiro).ToList();
                                if (lstContratoParcelaEntregavelTemp.Count > 0)
                                {
                                    foreach (var contratoParcEntreTemp in lstContratoParcelaEntregavelTemp)
                                    {
                                        // Busca o registro na tabela ContratoEntregavelTemporaria
                                        var contratoEntregavelTemp = lstEntregaveisTemporaria.Where(w => w.IdContratoEntregavel == contratoParcEntreTemp.IdEntregavel).FirstOrDefault();
                                        if (contratoEntregavelTemp != null)
                                        {
                                            // Com o ContratoEntregavel e a Parcela , cria um novo registro na tabela ContratoEntregavelParcela
                                            var contratoParcelaEntregavel = new ContratoParcelaEntregavel();
                                            contratoParcelaEntregavel.IdEntregavel = contratoEntregavelTemp.IdEntregavel.Value;
                                            contratoParcelaEntregavel.IdParcela = contratoCronogramaFinanceiro.IdContratoCronFinanceiro;

                                            db.ContratoParcelaEntregavel.Add(contratoParcelaEntregavel);
                                            db.SaveChanges();
                                        }

                                        // Após copiar o relacionamento entre Parcela e Entregavel remove o registro
                                        db.ContratoParcelaEntregavelTemporaria.Remove(contratoParcEntreTemp);
                                        db.SaveChanges();
                                    }
                                }

                                // Após copiar a Parcela , exclui da tabela Temporaria
                                db.ContratoCronogramaFinanceiroTemporaria.Remove(parcelaTemp);
                                db.SaveChanges();
                            }

                            // Remove todos os registros de EntregaveisTemporaria do devido Contrato            
                            db.ContratoEntregavelTemporaria.RemoveRange(lstEntregaveisTemporaria);

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-AplicarRenovacao Contrato [" + idContrato + "]");

                            retorno = false;
                        }
                    }
                });

                return retorno;
            }
        }

        //Retorna todas a renovações do contrato
        [HttpGet]
        [Route("ListaRenovacoes/{idContrato}")]
        public List<OutPutGetListaRenovacao> ListaRenovacoes(int idContrato)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaRenovacoes = new List<OutPutGetListaRenovacao>();

                    var renovacoes = db.ContratoProrrogacao.Where(w => w.IdContrato == idContrato).ToList();

                    foreach (var item in renovacoes)
                    {
                        var itemProrrogacao = new OutPutGetListaRenovacao();

                        itemProrrogacao.IdContratoRenovacao = item.IdContratoRenovacao;
                        itemProrrogacao.DtInicioVigenciaContrato = item.DtInicioVigencia;
                        itemProrrogacao.DtFimVigenciaProrrogada = item.DtFimVigenciaRenovacao;
                        itemProrrogacao.VlContratoAntesRenovacao = item.VlContratoAntesRenovacao;
                        itemProrrogacao.VlContratoRenovado = item.VlContratoRenovado;
                        itemProrrogacao.VlReajusteAcumulado = item.VlReajusteAcumulado;
                        itemProrrogacao.VlReajustePeriodo = item.VlReajustePeriodo;
                        itemProrrogacao.NuRenovacao = item.NuRenovacao;
                        itemProrrogacao.DsSituacao = db.Situacao.Where(w => w.IdSituacao == item.IdSituacao).FirstOrDefault().DsSituacao;

                        listaRenovacoes.Add(itemProrrogacao);
                    }

                    return listaRenovacoes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoController-ListaRenovacoes Contrato [" + idContrato + "]");

                    throw;
                }
            }
        }

        #endregion
    }

    #region Retornos

    #region Documentos Principais
    public class OutPutBuscaNomeDocs
    {
        public string DsDoc { get; set; }
        public int IdContratoDocPrincipal { get; set; }
    }

    public class OutPutDocumentoPrincipal
    {
        public int IdContratoDocPrincipal { get; set; }
        public int IdContrato { get; set; }
        public string DsTipoDocumento { get; set; }
        public string NmDocumento { get; set; }
        public byte[] DocFisico { get; set; }
        public string DtUpLoad { get; set; }
        public string NmCriador { get; set; }
        public string TextDown { get; set; }
        public bool Termo { get; set; }
        public bool PropostaFinal { get; set; }
    }

    //EGS 30.03.2021 Documento de Aditivo
    public class OutPutDadosUltimoAditivo
    {
        public int       advIdContrato               { get; set; }
        public int       advIdContratoAditivo        { get; set; }
        public int       advIdNumeroAditivo          { get; set; }
        public int       advIdNumeroAditivoCliente   { get; set; }
        public string    advDsAditivo                { get; set; }
        public string    advDtCriacaoAditivo         { get; set; }
        public string    advDtInicioAditivo          { get; set; }
        public string    advDtFinalAditivo           { get; set; }
        public string    advDtInicioVigênciaAditivo  { get; set; }
        public string    advDtFinalVigênciaAditivo   { get; set; }
        public string    advValorContratoOriginal    { get; set; }
        public string    advValorContratoAditivo     { get; set; }
        public string    advValorAditivo             { get; set; }
        public bool?     IcTipoAditivoValor          { get; set; }
        public bool?     IcTipoAditivoData           { get; set; }
        public bool?     IcTipoAditivoEscopo         { get; set; }
        public bool?     IcTipoAditivoOutro          { get; set; }
        public bool      bPossuiAditivo              { get; set; }
    }


    //EGS 30.03.2021 Documento de Aditivo
    public class OutPutDoctoAditivoPrincipal
    {
        public int       IdContratoDocPrincipal      { get; set; }
        public int       IdContrato                  { get; set; }
        public string    DsTipoDocumento             { get; set; }
        public string    NmDocumento                 { get; set; }
        public byte[]    DocFisico                   { get; set; }
        public string    DtUpLoad                    { get; set; }
        public string    NmCriador                   { get; set; }
        public string    TextDown                    { get; set; }
        public bool      Termo                       { get; set; }
        public bool      PropostaFinal               { get; set; }
    }
    #endregion

    #region Contrato 
    public class InputAddContrato
    {
        public int IdProposta { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public bool? IcInformacoesIncompletas { get; set; }
        public string Url { get; set; }
    }

    public class OutPutUpdateContrato
    {
        public bool Result { get; set; }
    }

    public class OutPutAddContratoDoc
    {
        public bool Result { get; set; }
    }

    public class OutPutAtivarContrato
    {
        public bool Result { get; set; }
    }

    public class OutPutGetFormacaoProfissional
    {
        public int IdFormacaoProfissional { get; set; }
        public string DsFormacaoProfissional { get; set; }
    }

    public class OutPutGetTipoContratacao
    {
        public int IdTipoContratacao { get; set; }
        public string DsTipoContratacao { get; set; }
    }

    public class OutPutGetTaxaInstitucional
    {
        public int IdTaxaInstitucional { get; set; }
        public string DsTaxaInstitucional { get; set; }
        public decimal PcTaxaInstitucional { get; set; }
    }

    public class InputPesquisaContrato
    {
        public int IdUsuario { get; set; }
        public string Palavra { get; set; }
        public string Url { get; set; }
    }
    public class OutPutGetContratos
    {
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public int IdProposta { get; set; }
        public DateTime? DtAssinatura { get; set; }
        public List<string> coordenadores { get; set; }
        public string coordenadoresTexto { get; set; }
        public string DsPrazoExecucao { get; set; }
        public string DsPrazoExecFim { get; set; }                //EGS 30.06.2020 - Nova coluna solicitado por Nelson
        public string IcOrdemInicio { get; set; }
        public string IcRenovacaoAutomatica { get; set; }
        public string DsSituacao { get; set; }
        public string DsAssunto { get; set; }
        public string DsNumProjeto { get; set; }                   //EGS 30.05.2020 - Nova coluna solicitado por Nelson
        public string DsCentroCusto { get; set; }                  //EGS 30.05.2020 - Nova coluna solicitado por Nelson
        public List<string> clientes { get; set; }
        public string clientesTexto { get; set; }
        public decimal? VlContrato { get; set; }
    }

    public class OutPutListaContratos
    {
        public List<OutPutGetContratos> lstOutPutGetContratos { get; set; }
        public int TamanhoEntregaveisAtraso { get; set; }
        public int TamanhoFaturamentoAtraso { get; set; }
        public int TamanhoContratosAtivos { get; set; }
        public int tamanhoContratosCanEncSus { get; set; }
        public int TamanhoEntregaveisAteData { get; set; }
        public int TamanhoFaturamentosAteData { get; set; }
        public int TamanhoEncerramentoProximo { get; set; }
        public int TamanhoReajustes { get; set; }
        public int TamanhoContrato { get; set; }
        public int TamanhoContratoInfoIncompleta { get; set; }
        public int TamanhoContratosFipeNaoAssinada { get; set; }
        public int TamanhoAditivo { get; set; }
        public int IdContratoAditivo { get; set; }
    }

    public class OutPutListaContratosEncerramentoProximo
    {
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public string DsApelido { get; set; }
        public string NuCentroCusto { get; set; }
        public bool? IcRenovacaoAutomatica { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public List<string> Clientes { get; set; }
        public string ClientesTexto { get; set; }
        public bool? IcReajuste { get; set; }
        public string IndiceReajuste { get; set; }
    }

    public class OutPutListaContratosRenovacaoAutomatica
    {
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public string DsApelido { get; set; }
        public string NuCentroCusto { get; set; }
        public bool? IcRenovacaoAutomatica { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public List<string> Clientes { get; set; }
        public string ClientesTexto { get; set; }
        public bool? IcReajuste { get; set; }
        public string IndiceReajuste { get; set; }
        public decimal VlContrato { get; set; }

    }

    public class OutPutListaContratosPorData
    {
        public int TamanhoEntregaveisAteData { get; set; }
        public int TamanhoFaturamentosAteData { get; set; }
        public int TamanhoReajustes { get; set; }
        public int IdContrato { get; set; }
    }

    public class OutPutCliente
    {
        public int IdCliente { get; set; }
        public bool? IcPagador { get; set; }
        public bool? IcSomentePagador { get; set; }
    }

    public class OutPutDeleteContrato
    {
        public bool Result { get; set; }
        public int IdContrato { get; set; }
    }

    public class InputEnviaEmailGestorContratos
    {
        public int    IdProposta       { get; set; }
        public int    IdContrato       { get; set; }
        public string Url              { get; set; }
        public bool   bCompletoSucesso { get; set; }
    }

    public class OutputEnviaEmailGestorContratos
    {
        public bool Result { get; set; }
    }

    public class OutPutGetEquipeTecnica
    {
        public int IdPessoaFisica { get; set; }
        public string CdEmail { get; set; }
        public int? IdFormacaoProfissional { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public int? IdPessoaJuridica { get; set; }
        public int? IdTipoContratacao { get; set; }
        public int? IdTipoVinculo { get; set; }
        public int IdTaxaInstitucional { get; set; }
        public decimal? VlTotalAReceber { get; set; }
        public decimal? VlTaxaInstitucional { get; set; }
        public decimal? VlCustoProjeto { get; set; }
    }
    #endregion

    #region Contrato Coordenador
    public class OutPutGetCoordenador
    {
        public int IdPessoa { get; set; }
        public int IdContrato { get; set; }
        public int IdContratoCoordenador { get; set; }
        public string NmPessoa { get; set; }
    }
    #endregion

    #region Contrato Cliente
    public class OutPutGetCliente
    {
        public int? IdPessoa { get; set; }
        public string Cnpj { get; set; }
        public string NmCliente { get; set; }
        public string DsEsferaEmpresa { get; set; }
        public int IdCliente { get; set; }
        public bool? IcPagador { get; set; }
        public bool? IcSomentePagador { get; set; }
        public List<int> ClientesPagadores { get; set; }
        public int IdContratoCliente { get; set; }
        public int NuContratante { get; set; }
    }
    #endregion

    #region Contrato Contatos
    #endregion

    #region Contrato Documentos
    public class InputAddContratoDoc
    {
        public int IdTipoDoc { get; set; }
        public int IdContrato { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime DtDoc { get; set; }
    }

    public class InputUpdateDocumento
    {
        public int IdContratoDoc { get; set; }
        public short IdTipoDoc { get; set; }
        public DateTime DtDoc { get; set; }
    }

    public class OutputUpdateDocumento
    {
        public bool Result { get; set; }
    }

    public class OutputTipoDocumentoId
    {
        public bool result { get; set; }
        public int IdTipoDocumento { get; set; }
        public DateTime DtDoc { get; set; }
        public string DsDoc { get; set; }
    }
    public class OutPutContratoDocumento
    {

        public int IdContratoDoc { get; set; }
        public int IdContrato { get; set; }
        public string DsTipoDocumento { get; set; }
        public string NmDocumento { get; set; }
        public byte[] DocFisico { get; set; }
        public DateTime DtUpLoad { get; set; }
        public string NmCriador { get; set; }
        public string TextDown { get; set; }
        public bool Minuta { get; set; }
        public DateTime DtDoc { get; set; }
        public string DsDoc { get; set; }
    }

    public class OutPutReturnDoc
    {

        public bool Result { get; set; }

    }

    public class OutPutTipoDocumento
    {
        public short IdTipoDocumento { get; set; }
        public string DsTipoDocumento { get; set; }
        public string TipoDocumento { get; set; }

    }
    #endregion

    #region Contrato Dados Cobrança

    public class InputUpdateContratoDadosCobranca
    {
        public int IdContrato { get; set; }
        public int IdTipoEntregaDocumento { get; set; }
        public int IdTipoCobranca { get; set; }
        public int IdFormaPagamento { get; set; }
        public string IcFatAprovEntregavel { get; set; }
        public string IcFatPedidoEmpenho { get; set; }
        public int IdContaCorrente { get; set; }
        public string DsPrazoPagamento { get; set; }
        public string DsTextoCorpoNF { get; set; }
        public List<InputUpdateContratoDocsAcompanhaNF> DocAcompanhaNFs { get; set; }
        public string NuBanco { get; set; }
        public string NuAgencia { get; set; }
        public string NuConta { get; set; }

    }

    public class InputUpdateContratoDocsAcompanhaNF
    {
        public int IdContrato { get; set; }
        public int IdTipoDocsAcompanhaNF { get; set; }
        public string DsTipoDocsAcompanhaNF { get; set; }
    }
    public class OutPutUpdateDadosCobranca
    {
        public bool Result { get; set; }
        public int IdContrato { get; set; }
    }
    #endregion

    #region Lista Entregáveis

    public class OutPutGetEntregaveis
    {
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public int IdContratoEntregavel { get; set; }
        public string DsApelidoContrato { get; set; }
        public string DsProduto { get; set; }
        public string DsSituacao { get; set; }
        public string NmFrente { get; set; }
        public string DtProduto { get; set; }

    }

    #endregion

    #region Lista Faturamentos

    public class OutPutGetFaturamentos
    {
        public int IdContrato { get; set; }
        public string NuContratoEdit { get; set; }
        public int IdContratoCronFinanceiro { get; set; }
        public string DsApelidoContrato { get; set; }
        public string DsCliente { get; set; }
        public int NuParcela { get; set; }
        public string DtFaturamento { get; set; }
        public decimal VlParcela { get; set; }
        public string DsSituacao { get; set; }
    }

    #endregion

    #region Lista Aditivos

    public class OutPutGetAditivos
    {
        public int IdContrato { get; set; }
        public int IdContratoAditivo { get; set; }
        public string DsApelidoContrato { get; set; }
        public string DsSituacao { get; set; }
        public string DsTipoAditivo { get; set; }
        public DateTime DtCriacao { get; set; }
        public int NuAditivo { get; set; }
        public int NuAditivoCliente { get; set; }
        public string NuContratoEdit { get; set; }
        public string DsUsuarioAplicacao { get; set; }
        public DateTime? DtAplicacao { get; set; }
    }

    #endregion

    #region Contrato Comentário

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
        public int IdContratoComentario { get; set; }
        public int IdContrato { get; set; }
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
        public int IdContrato { get; set; }
    }
    public class OutPutAddComentario
    {
        public bool Result { get; set; }
    }
    public class OutPutComentario
    {
        public string DsComentario { get; set; }
        public string DtComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdContratoComentario { get; set; }
        public int IdContrato { get; set; }
        public string NmUsuario { get; set; }
        public bool ComentarioLido { get; set; }
    }

    public class OutPutUpDateComentario
    {
        public string DsComentario { get; set; }
        public DateTime DtComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdContratoComentario { get; set; }
        public int IdContrato { get; set; }
        public string NmUsuario { get; set; }
    }

    public class InputUpDateComentario
    {
        public string DsComentario { get; set; }
        public DateTime DtComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdContratoComentario { get; set; }
        public int IdContrato { get; set; }
    }

    #endregion

    #region Contrato Tomador de Serviço NF

    public class InputUpdateNfTomadorServico
    {
        public int IdContratoCronFinanceiro { get; set; }
        public int IdSituacao { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public string DsTextoCorpoNF { get; set; }
        public string NuNotaFiscal { get; set; }
    }

    public class OutputGetContratoTomadorServico
    {
        public int IdContrato { get; set; }
        public string DsTextoCorpoNF { get; set; }
        public string NuBanco { get; set; }
        public string NuAgencia { get; set; }
        public string NuConta { get; set; }
        public int NuParcela { get; set; }
        public string DsCnpj { get; set; }
        public string DsRazaoSocial { get; set; }
        public string DsCpf { get; set; }
        public string DsNome { get; set; }
        public string DsAssunto { get; set; }
        public string NuCentroCusto { get; set; }
        public string VlParcela { get; set; }
        public int IdSituacao { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public string NuNotaFiscal { get; set; }
    }

    public class OutPutNfTomadorServico
    {
        public bool Result { get; set; }
        public int IdContratoCronFinanceiro { get; set; }
    }
    #endregion

    #region lista Histórico
    public class OutPutGetContratoHistorico
    {
        public int IdContratoHistorico { get; set; }
        public string IdContrato { get; set; }
        public string DtInicio { get; set; }
        public string DtFim { get; set; }
        public DateTime DataInicial { get; set; }
        public string dsSituacao { get; set; }
        public string nmUsuario { get; set; }
        public string dsEmailObserva { get; set; }     //EGS 30.08.2020 Email ou Observacao da Proposta ou Contrato
    }
    #endregion

    #region Renovação Automática
    public class OutPutGetRenovacao
    {
        public int IdContratoRenovacao { get; set; }
        public int? IdUsuarioAplicacao { get; set; }
        public string NuPrazoMesesContrato { get; set; }
        public DateTime? DtInicioVigenciaContrato { get; set; }
        public DateTime? dtFimVigenciaAtualContrato { get; set; }
        public DateTime? DtFimVigenciaProrrogada { get; set; }
        public string NuPrazoRenovacaoMeses { get; set; }
        public DateTime? DtInicioExecucaoContrato { get; set; }
        public DateTime? DtFimExecucaoContrato { get; set; }
        public DateTime? DtFimExecucaoRenovacao { get; set; }
        public decimal? VlContratoAntesRenovacao { get; set; }
        public decimal? FatorReajuste { get; set; }
        public decimal? VlContratoRenovado { get; set; }
        public bool? IcEntregaveisCopiado { get; set; }
        public bool? IcCronogramaFinanceiroCopiado { get; set; }
        //informações de criação
        public string DtCriacao { get; set; }
        public string NmUsuario { get; set; }
        public int idProposta { get; set; }
        public string NuCentroCusto { get; set; }
    }

    public class InputUpdateRenovacao
    {
        public int IdContratoRenovacao { get; set; }
        public string NuPrazoMesesContrato { get; set; }
        public DateTime? DtInicioVigenciaContrato { get; set; }
        public DateTime? dtFimVigenciaAtualContrato { get; set; }
        public DateTime? DtFimVigenciaProrrogada { get; set; }
        public string NuPrazoRenovacaoMeses { get; set; }
        public DateTime? DtInicioExecucaoContrato { get; set; }
        public DateTime? DtFimExecucaoContrato { get; set; }
        public DateTime? DtFimExecucaoRenovacao { get; set; }
        public decimal? VlContratoAntesRenovacao { get; set; }
        public decimal? FatorReajuste { get; set; }
        public decimal? VlContratoRenovado { get; set; }
    }
    #endregion

    #region Reajuste
    public class OutPutListaContratosReajuste
    {
        public int IdContrato { get; set; }
        public int IdContratoReajuste { get; set; }
        public string NuContratoEdit { get; set; }
        public string DsApelido { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public List<string> Clientes { get; set; }
        public string ClientesTexto { get; set; }
        public decimal? VlContrato { get; set; }
        public string IndiceReajuste { get; set; }
        public string DsSituacao { get; set; }
    }

    public class OutPutGetListaRenovacao
    {
        public int IdContratoRenovacao { get; set; }
        public int NuRenovacao { get; set; }
        public string DsSituacao { get; set; }
        public DateTime? DtInicioVigenciaContrato { get; set; }
        public DateTime? DtFimVigenciaProrrogada { get; set; }
        public decimal? VlContratoAntesRenovacao { get; set; }
        public decimal? VlContratoRenovado { get; set; }
        public decimal? VlReajusteAcumulado { get; set; }
        public decimal? VlReajustePeriodo { get; set; }

    }

    public class OutPutGetVerificaContratoReajuste
    {
        public int IdContratoReajuste { get; set; }
    }
    #endregion

    #endregion

}
