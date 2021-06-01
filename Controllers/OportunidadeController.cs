using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ApiFipe.Utilitario;


namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class OportunidadeController : ControllerBase
    {
        #region | Métodos
        GravaLog _GLog = new GravaLog();

        [HttpPost]
        [Route("AddOportunidade")]
        public OutPutOportunidade AddOportunidade([FromBody]InputAddOportunidade item)
        {
            var retorno = new OutPutOportunidade();
            var oportunidade = new Oportunidade();

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


                            var objOportunidade = new bOportunidade(db);
                            var listOpCli = new List<OportunidadeCliente>();
                            var listOpResp = new List<OportunidadeResponsavel>();

                            var lastOportunidade = db.Oportunidade.OrderByDescending(w => w.IdOportunidade).FirstOrDefault();
                            if (lastOportunidade != null)
                            {
                                oportunidade.IdOportunidade = lastOportunidade.IdOportunidade + 1;
                            }
                            else
                            {
                                oportunidade.IdOportunidade = 1;
                            }
                            oportunidade.IdSituacao = 1;
                            oportunidade.IdTipoOportunidade = item.TipoOportunidade;
                            oportunidade.DsAssunto = item.DsAssunto;
                            oportunidade.DsObservacao = item.DsObservacao;
                            oportunidade.DtLimiteEntregaProposta = item.DtLimiteEntregaProposta;
                            oportunidade.DtCriacao = DateTime.Now;
                            oportunidade.IdProposta = item.IdProposta.Value;
                            oportunidade.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;
                            oportunidade.IdUsuarioCriacao         = AppSettings.constGlobalUserID;

                            objOportunidade.AddOportunidade(oportunidade);

                            retorno.IdOportunidade = oportunidade.IdOportunidade;

                            foreach (var itemCli in item.Clientes)
                            {
                                // Salva Cliente antes de salvar Oportunidade Cliente
                                var cliente = new Cliente();
                                cliente.IdPessoa = itemCli.IdPessoa;
                                var Pessoa = db.Pessoa.Where(w => w.IdPessoa == itemCli.IdPessoa).FirstOrDefault();

                                new bCliente(db).AddCliente(cliente);

                                var opCliente = new OportunidadeCliente();

                                opCliente.IdCliente = cliente.IdCliente;
                                opCliente.IdOportunidade = retorno.IdOportunidade;

                                if (Pessoa.IdPessoaJuridica != null)
                                {
                                    var PessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)Pessoa.IdPessoaJuridica);
                                    opCliente.NmFantasia = PessoaJuridica.NmFantasia;
                                    opCliente.RazaoSocial = PessoaJuridica.RazaoSocial;
                                }

                                listOpCli.Add(opCliente);
                            }
                            objOportunidade.AddOportunidadeCli(listOpCli);

                            if (item.Responsaveis.Count > 0)
                            {
                                foreach (var itemResp in item.Responsaveis)
                                {
                                    var opResponsavel = new OportunidadeResponsavel();

                                    opResponsavel.IdPessoaFisica = itemResp.IdPessoaFisica;
                                    opResponsavel.IdOportunidade = retorno.IdOportunidade;
                                    listOpResp.Add(opResponsavel);
                                }
                                objOportunidade.AddOportunidadeResp(listOpResp);
                            }

                            db.Database.CommitTransaction();

                            retorno.Result = true;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-AddOportunidade");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaOportunidadeId/{id}")]
        public InputAddOportunidade BuscaOportunidadeId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemOportunidade = new InputAddOportunidade();
                itemOportunidade.Clientes = new List<OutputCliente>();
                itemOportunidade.Responsaveis = new List<OutputResponsavel>();

                try
                {
                    var retornoOportunidade = new bOportunidade(db).BuscarOportunidadeId(id);

                    foreach (var itemOpCli in retornoOportunidade.OportunidadeCliente)
                    {
                        var itemCli = new OutputCliente();

                        var cliente = db.Cliente.Where(w => w.IdCliente == itemOpCli.IdCliente).FirstOrDefault();
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
                            itemCli.NmCliente = itemOpCli.NmFantasia;
                        }
                        itemOportunidade.Clientes.Add(itemCli);
                    }

                    foreach (var itemOpResp in retornoOportunidade.OportunidadeResponsavel)
                    {
                        var itemResp = new OutputResponsavel();
                        var responsavel = db.PessoaFisica.Where(w => w.IdPessoaFisica == itemOpResp.IdPessoaFisica).FirstOrDefault();

                        itemResp.IdPessoaFisica = responsavel.IdPessoaFisica;
                        itemResp.NmPessoa = responsavel.NmPessoa;
                        itemOportunidade.Responsaveis.Add(itemResp);

                    }

                    var NmUsuario = new bUsuario(db).GetById(retornoOportunidade.IdUsuarioCriacao);

                    var pessoaFisica = new bPessoaFisica(db).GetById(NmUsuario.IdPessoa);
                    itemOportunidade.NmUsuario = pessoaFisica.NmPessoa;

                    itemOportunidade.IdOportunidade = retornoOportunidade.IdOportunidade;
                    itemOportunidade.Status = retornoOportunidade.IdSituacao;
                    itemOportunidade.TipoOportunidade = retornoOportunidade.IdTipoOportunidade;
                    itemOportunidade.DsAssunto = retornoOportunidade.DsAssunto;
                    itemOportunidade.DsObservacao = retornoOportunidade.DsObservacao;
                    itemOportunidade.DtLimiteEntregaProposta = retornoOportunidade.DtLimiteEntregaProposta;
                    itemOportunidade.DtCriacao = retornoOportunidade.DtCriacao;
                    itemOportunidade.IdUsuarioUltimaAlteracao = retornoOportunidade.IdUsuarioUltimaAlteracao;
                    itemOportunidade.IdProposta = retornoOportunidade.IdProposta;

                    return itemOportunidade;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-BuscaOportunidadeId");

                    throw;
                }

            }
        }

        [HttpPut]
        [Route("AtualizarOportunidade")]
        public OutPutOportunidade AtualizarOportunidade([FromBody]InputAddOportunidade item)
        {
            var retorno = new OutPutOportunidade();
            var oportunidade = new Oportunidade();
            var opCliente = new OportunidadeCliente();
            var opResponsavel = new OportunidadeResponsavel();
            var listaOpCli = new List<OportunidadeCliente>();
            var listaOpResp = new List<OportunidadeResponsavel>();

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


                            var objOportunidade = new bOportunidade(db);

                            oportunidade.IdOportunidade = item.IdOportunidade;
                            oportunidade.IdSituacao = item.Status;
                            oportunidade.IdTipoOportunidade = item.TipoOportunidade;
                            oportunidade.DsAssunto = item.DsAssunto;
                            oportunidade.DsObservacao = item.DsObservacao;
                            oportunidade.DtLimiteEntregaProposta = item.DtLimiteEntregaProposta;
                            oportunidade.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;

                            //objOportunidade.ExcluirCliente(item.IdOportunidade);
                            //objOportunidade.ExcluirResponsavel(item.IdOportunidade);

                            foreach (var itemCli in item.Clientes)
                            {
                                if (itemCli.IdCliente == null)
                                {
                                    // Salvar Clientes antes de salvar Oportunidade Cliente
                                    var clienteExiste = db.Cliente.Where(w => w.IdPessoa == itemCli.IdPessoa).FirstOrDefault();
                                    if (clienteExiste != null)
                                    {
                                        var Pessoa = db.Pessoa.Where(w => w.IdPessoa == itemCli.IdPessoa).FirstOrDefault();

                                        var opCliExiste = db.OportunidadeCliente.Where(w => w.IdCliente == clienteExiste.IdCliente && w.IdOportunidade == item.IdOportunidade).FirstOrDefault();
                                        if (opCliExiste == null)
                                        {
                                            var opCli = new OportunidadeCliente();
                                            opCli.IdOportunidade = item.IdOportunidade;
                                            opCli.IdCliente = clienteExiste.IdCliente;

                                            if (Pessoa.IdPessoaJuridica != null)
                                            {
                                                var PessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)Pessoa.IdPessoaJuridica);
                                                opCli.NmFantasia = PessoaJuridica.NmFantasia;
                                                opCli.RazaoSocial = PessoaJuridica.RazaoSocial;
                                            }
                                            listaOpCli.Add(opCli);
                                        }
                                    }
                                    else
                                    {
                                        var cliente = new Cliente();
                                        cliente.IdPessoa = itemCli.IdPessoa;
                                        new bCliente(db).AddCliente(cliente);
                                        var Pessoa = db.Pessoa.Where(w => w.IdPessoa == itemCli.IdPessoa).FirstOrDefault();

                                        var opCli = new OportunidadeCliente();

                                        opCli.IdOportunidade = item.IdOportunidade;
                                        opCli.IdCliente = cliente.IdCliente;

                                        if (Pessoa.IdPessoaJuridica != null)
                                        {
                                            var PessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId((Int32)Pessoa.IdPessoaJuridica);
                                            opCli.NmFantasia = PessoaJuridica.NmFantasia;
                                            opCli.RazaoSocial = PessoaJuridica.RazaoSocial;
                                        }

                                        listaOpCli.Add(opCli);
                                    }
                                }
                            }
                            objOportunidade.AddOportunidadeCli(listaOpCli);

                            //  Exclui os Clientes que não fazem mais parte da Oportunidade
                            var allOpClientes = db.OportunidadeCliente
                                .Where(w => w.IdOportunidade == item.IdOportunidade && w.IdOportunidadeCliente != 0)
                                .ToList();
                            foreach (var oportunidadeCliente in allOpClientes)
                            {
                                var cliente = new bCliente(db).BuscarClienteId((Int32)oportunidadeCliente.IdCliente);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                                // Verifica se a Oportunidade Cliente existe nas Oportunidades que foram salvas
                                var clienteExisteItem = item.Clientes
                                    .Where(w => w.IdPessoa == pessoa.IdPessoa)
                                    .FirstOrDefault();

                                // Caso não exista , exclui a Oportunidade Cliente e Cliente
                                if (clienteExisteItem == null)
                                {
                                    db.OportunidadeCliente.Remove(oportunidadeCliente);

                                    var propCliente = db.PropostaCliente.Where(w => w.IdCliente == oportunidadeCliente.IdCliente).FirstOrDefault();
                                    if (propCliente == null)
                                    {
                                        var contratoCliente = db.ContratoCliente.Where(w => w.IdCliente == oportunidadeCliente.IdCliente).FirstOrDefault();
                                        if (contratoCliente == null)
                                        {
                                            var clienteExcluir = new bCliente(db).BuscarClienteId((Int32)oportunidadeCliente.IdCliente);
                                            
                                            db.Cliente.Remove(clienteExcluir);                                            
                                        }
                                    }                                    
                                }
                            }

                            if (item.Responsaveis.Count > 0)
                            {
                                foreach (var itemResp in item.Responsaveis)
                                {
                                    var opResponsavelExiste = db.OportunidadeResponsavel
                                        .Where(w => w.IdPessoaFisica == itemResp.IdPessoaFisica && w.IdOportunidade == item.IdOportunidade)
                                        .FirstOrDefault();

                                    if (opResponsavelExiste == null)
                                    {
                                        var opResp = new OportunidadeResponsavel();

                                        opResp.IdOportunidade = item.IdOportunidade;
                                        opResp.IdPessoaFisica = itemResp.IdPessoaFisica;

                                        listaOpResp.Add(opResp);
                                    }
                                }
                                objOportunidade.AddOportunidadeResp(listaOpResp);
                            }

                            //  Exclui os Responsáveis que não fazem mais parte da Oportunidade
                            var allOpResp = db.OportunidadeResponsavel
                                .Where(w => w.IdOportunidade == item.IdOportunidade && w.IdOportunidadeResponsavel != 0)
                                .ToList();
                            foreach (var oportunidadeResp in allOpResp)
                            {
                                var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId((Int32)oportunidadeResp.IdPessoaFisica);
                                var pessoa = db.Pessoa.Where(w => w.IdPessoaFisica == pessoaFisica.IdPessoaFisica).FirstOrDefault();
                                // Verifica se a Oportunidade Cliente existe nas Oportunidades que foram salvas
                                var responsavelExisteItem = item.Responsaveis
                                    .Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica)
                                    .FirstOrDefault();

                                // Caso não exista , exclui a Oportunidade Responsavel
                                if (responsavelExisteItem == null)
                                {
                                    db.OportunidadeResponsavel.Remove(oportunidadeResp);
                                }
                            }

                            objOportunidade.UpdateOportunidade(oportunidade);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                            retorno.IdOportunidade = oportunidade.IdOportunidade;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-AtualizarOportunidade");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("ExcluirOportunidade/{id}")]
        public OutPutOportunidade ExcluirOportunidade(int id)
        {
            var retorno = new OutPutOportunidade();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            var objOportunidade = new bOportunidade(db);

                            // Inicia transação


                            objOportunidade.RemoveOportunidade(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                            retorno.IdOportunidade = id;

                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ExcluirOportunidade");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaOportunidade")]
        public List<OutputGet> ListaOportunidade()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    
                    return new bPesquisaGeral(db).GetOportunidades();

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaOportunidade");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("PesquisaOportunidade")]
        public List<OutputGet> PesquisaOportunidade([FromBody] InputPesquisaOportunidade pesquisa)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaOportunidades = new List<OutputGet>();
                    var queryFiltrada = new List<OutputGet>();

                    var sqlParameter = CriarParametroTexto("@Palavra", pesquisa.Palavra);
                    var dsResultado = ExecutarProcedureComRetorno("PR_FiltraGridOportunidadeByPalavra", pesquisa.Url, new List<System.Data.SqlClient.SqlParameter>() { sqlParameter });

                    foreach (DataRow row in dsResultado.Tables[0].Rows)
                    {
                        int idOportunidade = Convert.ToInt32(row["IdOportunidade"]);
                        if (listaOportunidades.Where(w => w.IdOportunidade == idOportunidade).FirstOrDefault() == null)
                        {
                            var item = new bOportunidade(db).BuscarOportunidadeId(idOportunidade);

                            var oportunidade = ObterOutputGet(db, item);
                            listaOportunidades.Add(oportunidade);

                            queryFiltrada = listaOportunidades;
                        }
                    }

                    return queryFiltrada.ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-PesquisaOportunidade");


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
                new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ExecutarProcedureComRetorno");
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

        [HttpGet]
        [Route("ListaOportunidadesSemResp")]
        public OutputGetOportunidade ListaOportunidadesSemResp()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutputGetOportunidade();
                    var listaOportunidades = new List<OutputGet>();
                    var queryFiltrada = new List<OutputGet>();

                    var oportunidades = new bOportunidade(db).BuscarOportunidadesSemResp();

                    foreach (var item in oportunidades)
                    {
                        var oportunidade = ObterOutputGet(db, item);
                        listaOportunidades.Add(oportunidade);
                    }
                    retorno.Tamanho = oportunidades.Count;
                    retorno.oportunidades = listaOportunidades;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaOportunidadesSemResp");


                    throw;
                }
            }
        }

        //[HttpGet]
        //[Route("PesquisaOportunidade/{palavra}/{datInicial}/{datFinal}/{cadastro}/{declinado}/{criada}/{elaboracao}")]
        //public List<OutputGet> PesquisaOportunidade(string palavra, string datInicial, string datFinal, bool cadastro, bool declinado, bool criada, bool elaboracao)
        //{
        //    using (var db = new FIPEContratosContext())
        //    {
        //        try
        //        {
        //            var listaOportunidades = new List<OutputGet>();
        //            var oportunidades = new bOportunidade(db).BuscarOportunidade();
        //            var queryFiltrada = new List<OutputGet>();
        //            var queryFiltradaCheck = new List<OutputGet>();
        //            var query = new List<OutputGet>();
        //            bool filtroDatCheck = false;
        //            var pesqPalavra = palavra.Split(" ");

        //            string opoCad = null, opoDec = null, opoCri = null;

        //            if (cadastro == true) { opoCad = "Cadastrada"; }

        //            if (declinado == true) { opoDec = "Declinada"; }

        //            if (criada == true) { opoCri = "Proposta Criada"; }

        //            if (datInicial == "null") { datInicial = null; }

        //            if (datFinal == "null") { datFinal = null; }

        //            foreach (var item in oportunidades)
        //            {
        //                var listaCli = db.OportunidadeCliente.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();

        //                foreach (var itemCli in listaCli)
        //                {
        //                    var oportunidade = ObterOutputGet(db, item, itemCli);
        //                    var oportunidadeUnica = listaOportunidades.Where(w => w.IdOportunidade == oportunidade.IdOportunidade).FirstOrDefault();

        //                    if (oportunidadeUnica == null)
        //                        listaOportunidades.Add(oportunidade);
        //                }
        //            }

        //            foreach (var itemPalavra in pesqPalavra)
        //            {
        //                var queryPalavra = listaOportunidades.Where(w => (w.DsCliente != null && w.DsCliente.ToUpper().Contains(itemPalavra.ToUpper())) ||
        //                                                (w.DsAssunto != null && w.DsAssunto.ToUpper().Contains(itemPalavra.ToUpper())) ||
        //                                                (w.Responsavel != null && w.Responsavel.ToUpper().Contains(itemPalavra.ToUpper()))
        //                                                 ).ToList();

        //                foreach (var item in queryPalavra)
        //                {
        //                    var itemExistente = query.Where(w => w.IdOportunidade == item.IdOportunidade).FirstOrDefault();

        //                    if (itemExistente == null)
        //                        query.Add(item);
        //                }
        //            }

        //            if (opoCad != null)
        //            {
        //                var lista = query.Where(w => w.Status != null && w.Status.ToUpper().Contains(opoCad.ToUpper())).ToList();

        //                foreach (var item in lista)
        //                    queryFiltradaCheck.Add(item);
        //            }

        //            if (opoCri != null)
        //            {
        //                var lista = query.Where(w => w.Status != null && w.Status.ToUpper().Contains(opoCri.ToUpper())).ToList();

        //                foreach (var item in lista)
        //                    queryFiltradaCheck.Add(item);
        //            }

        //            if (opoDec != null)
        //            {
        //                var lista = query.Where(w => w.Status != null && w.Status.ToUpper().Contains(opoDec.ToUpper())).ToList();

        //                foreach (var item in lista)
        //                    queryFiltradaCheck.Add(item);
        //            }

        //            if (datInicial != null && datFinal != null)
        //            {
        //                var datIni = Convert.ToDateTime(datInicial);
        //                var datFin = Convert.ToDateTime(datFinal);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date >= datIni.Date && w.DtCriacao.Date <= datFin.Date).ToList();
        //            }

        //            if (datInicial != null && datFinal == null)
        //            {
        //                var datIni = Convert.ToDateTime(datInicial);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date >= datIni.Date).ToList();
        //            }

        //            if (datInicial == null && datFinal != null)
        //            {
        //                var datFin = Convert.ToDateTime(datFinal);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date <= datFin.Date).ToList();
        //            }

        //            if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count > 0 && filtroDatCheck == true)
        //                return queryFiltrada = new List<OutputGet>();
        //            else if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count > 0)
        //                return queryFiltradaCheck.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count > 0 && queryFiltradaCheck.Count > 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count == 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count > 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else
        //                return query.OrderBy(o => o.IdOportunidade).ToList();
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //[HttpGet]
        //[Route("PesquisarOportunidade/{palavra}/{datInicial}/{datFinal}/{status}")]
        //public List<OutputGet> PesquisarOportunidade(string palavra, string datInicial, string datFinal, string status)
        //{
        //    using (var db = new FIPEContratosContext())
        //    {
        //        try
        //        {
        //            var listaOportunidades = new List<OutputGet>();
        //            var oportunidades = new bOportunidade(db).BuscarOportunidade();
        //            var queryFiltrada = new List<OutputGet>();
        //            var queryFiltradaCheck = new List<OutputGet>();
        //            var query = new List<OutputGet>();
        //            bool filtroDatCheck = false;
        //            string[] pesqPalavra = palavra == null || palavra == "null" ? null : palavra.Split(" ");
        //            string[] arrStatus = string.IsNullOrEmpty(status) || status == "null" ? null : status.Split(';').Where(_ => !string.IsNullOrEmpty(_)).ToArray();

        //            if (datInicial == "null") { datInicial = null; }

        //            if (datFinal == "null") { datFinal = null; }

        //            foreach (var item in oportunidades)
        //            {
        //                var listaCli = db.OportunidadeCliente.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();

        //                foreach (var itemCli in listaCli)
        //                {
        //                    var oportunidade = ObterOutputGet(db, item, itemCli);
        //                    var oportunidadeUnica = listaOportunidades.Where(w => w.IdOportunidade == oportunidade.IdOportunidade).FirstOrDefault();

        //                    if (oportunidadeUnica == null)
        //                        listaOportunidades.Add(oportunidade);
        //                }
        //            }

        //            if (pesqPalavra == null)
        //            {
        //                query = listaOportunidades;
        //            }
        //            else
        //            {
        //                foreach (var itemPalavra in pesqPalavra)
        //                {
        //                    var queryPalavra = listaOportunidades.Where(w => (w.DsCliente != null && w.DsCliente.ToUpper().Contains(itemPalavra.ToUpper())) ||
        //                                                    (w.DsAssunto != null && w.DsAssunto.ToUpper().Contains(itemPalavra.ToUpper())) ||
        //                                                    (w.Responsavel != null && w.Responsavel.ToUpper().Contains(itemPalavra.ToUpper()))
        //                                                     ).ToList();

        //                    foreach (var item in queryPalavra)
        //                    {
        //                        var itemExistente = query.Where(w => w.IdOportunidade == item.IdOportunidade).FirstOrDefault();

        //                        if (itemExistente == null)
        //                            query.Add(item);
        //                    }
        //                }
        //            }

        //            if (arrStatus == null)
        //            {
        //                queryFiltradaCheck = query;
        //            }
        //            else
        //            {
        //                foreach (string s in arrStatus)
        //                {
        //                    var lista = query.Where(w => w.Status != null && w.Status.ToUpper().Equals(s.ToUpper())).ToList();

        //                    foreach (var item in lista)
        //                        queryFiltradaCheck.Add(item);
        //                }
        //            }

        //            if (datInicial != null && datFinal != null)
        //            {
        //                var datIni = Convert.ToDateTime(datInicial);
        //                var datFin = Convert.ToDateTime(datFinal);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date >= datIni.Date && w.DtCriacao.Date <= datFin.Date).ToList();
        //            }

        //            if (datInicial != null && datFinal == null)
        //            {
        //                var datIni = Convert.ToDateTime(datInicial);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date >= datIni.Date).ToList();
        //            }

        //            if (datInicial == null && datFinal != null)
        //            {
        //                var datFin = Convert.ToDateTime(datFinal);

        //                filtroDatCheck = true;
        //                queryFiltrada = queryFiltradaCheck.Where(w => w.DtCriacao.Date <= datFin.Date).ToList();
        //            }

        //            if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count > 0 && filtroDatCheck == true)
        //                return queryFiltrada = new List<OutputGet>();
        //            else if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count > 0)
        //                return queryFiltradaCheck.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count > 0 && queryFiltradaCheck.Count > 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count == 0 && queryFiltradaCheck.Count == 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else if (queryFiltrada.Count > 0)
        //                return queryFiltrada.OrderBy(o => o.IdOportunidade).ToList();
        //            else
        //                return query.OrderBy(o => o.IdOportunidade).ToList();
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}

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
                        if (pessoa != null)
                        {
                            itemPesJuridica.IdPessoa  = pessoa.IdPessoa;
                            itemPesJuridica.NmCliente = pJuridica.NmFantasia;
                            listaClientes.Add(itemPesJuridica);
                        }
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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaCliente");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaResponsaveis")]
        public List<OutputResponsavel> ListaResponsaveis()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaResponsaveis = new List<OutputResponsavel>();

                    var responsaveis = new bOportunidade(db).BuscarResponsaveis();

                    if (responsaveis.Count > 0)
                    {
                        foreach (var itemResp in responsaveis)
                        {

                            var itemResponsavel = new OutputResponsavel();

                            itemResponsavel.IdPessoaFisica = itemResp.IdPessoaFisica;
                            itemResponsavel.NmPessoa = itemResp.NmPessoa;
                            itemResponsavel.Responsavel = itemResp.NmPessoa;
                            itemResponsavel.NmCoord = itemResp.NmPessoa;

                            listaResponsaveis.Add(itemResponsavel);
                        }
                    }

                    return listaResponsaveis;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaResponsaveis");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaResponsaveisGrid")]
        public List<OutputResponsavel> ListaResponsaveisGrid()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaResponsaveis = new List<OutputResponsavel>();
                    var lista = db.OportunidadeResponsavel.GroupBy(w => w.IdPessoaFisica).Select(s => s.First()).ToList();

                    foreach (var item in lista)
                    {
                        var itemResponsavel = new OutputResponsavel();

                        var pessoa = db.Pessoa.Where(w => w.IdPessoaFisica == item.IdPessoaFisica).FirstOrDefault();
                        itemResponsavel.Responsavel = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica).FirstOrDefault().NmPessoa;

                        listaResponsaveis.Add(itemResponsavel);
                    }

                    return listaResponsaveis;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaResponsaveisGrid");

                    throw;
                }
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

                    var situacoes = new bOportunidade(db).BuscarSituacao();

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaSituacao");

                    throw;
                }
            }
        }


        [HttpGet]
        [Route("ListaEstado")]
        public List<OutputEstado> ListaEstado()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaEstados = new List<OutputEstado>();

                    var estados = new bOportunidade(db).BuscarEstados();

                    foreach (var item in estados)
                    {

                        var itemEstado = new OutputEstado();

                        itemEstado.IdEstado = item.IdEstado;
                        itemEstado.NmEstado = item.NmEstado;
                        itemEstado.DsEstado = item.Uf;
                        itemEstado.UF       = item.Uf;

                        listaEstados.Add(itemEstado);
                    }

                    return listaEstados;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaEstado");

                    throw;
                }
            }
        }


        [HttpGet]
        [Route("ListaMunicipio")]
        public List<OutPutMunicipio> ListaMunicipio()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaMunicipios = new List<OutPutMunicipio>();
                    var pCliente = new bOportunidade(db).GetAll();
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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaMunicipio");

                    throw;
                }
            }
        }



        [HttpGet]
        [Route("ListaTipoOportunidade")]
        public List<OutputTipoOportunidade> ListaTipoOportunidade()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaTipoOportunidades = new List<OutputTipoOportunidade>();

                    var tipoOportunidades = new bOportunidade(db).BuscarTipoOportunidade();

                    foreach (var itemTipo in tipoOportunidades)
                    {
                        var itemTipoOportunidade = new OutputTipoOportunidade();

                        itemTipoOportunidade.IdTipoOportunidade = itemTipo.IdTipoOportunidade;
                        itemTipoOportunidade.DsTipoOportunidade = itemTipo.DsTipoOportunidade;
                        itemTipoOportunidade.TipoOportunidade = itemTipo.DsTipoOportunidade;

                        listaTipoOportunidades.Add(itemTipoOportunidade);
                    }

                    return listaTipoOportunidades;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaTipoOportunidade");

                    throw;
                }
            }
        }




        #region || Métodos Documentos

        [HttpPost]
        [Route("UpLoadDoc/{IdTipoDocumento}/{IdOportunidade}/{NmCriador}")]
        public async Task<IActionResult> UpLoadDoc(short IdTipoDocumento, int IdOportunidade, string NmCriador)
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
                            var files        = Request.Form.Files;
                            var itemDoc      = new OportunidadeDocs();
                            var objDocumento = new bOportunidade(db);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes           = ms.ToArray();
                                    itemDoc.IdTipoDocumento = IdTipoDocumento;
                                    itemDoc.IdOportunidade  = IdOportunidade;
                                    itemDoc.NmCriador       = NmCriador;
                                    itemDoc.DtUpLoad        = DateTime.Now;
                                    itemDoc.DocFisico       = fileBytes;
                                    itemDoc.NmDocumento     = files[0].Name;
                                }
                            }

                            objDocumento.AddDocumento(itemDoc);
                            
                            // Confirma operações
                            db.Database.CommitTransaction();
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "UpLoadDoc adicionado na Oport [" + IdOportunidade + "] criador [" + NmCriador + "]");

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-UpLoadDoc");

                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [HttpGet]
        [Route("ListaDocumentos/{id}")]
        public List<OutPutDocumento> ListaDocumentos(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaDocs = new List<OutPutDocumento>();

                    var documentos = new bOportunidade(db).BuscarDocumentos(id);

                    foreach (var itemDoc in documentos)
                    {
                        var tipoDocumento = db.TipoDocumento.Where(t => t.IdTipoDoc == itemDoc.IdTipoDocumento).Single();
                        var itemDocumento = new OutPutDocumento();

                        itemDocumento.IdOportunidadeDocs = itemDoc.IdOportunidadeDocs;
                        itemDocumento.IdOportunidade = itemDoc.IdOportunidade;
                        itemDocumento.IdTipoDocumento = (short)itemDoc.IdTipoDocumento;
                        itemDocumento.DsTipoDocumento = tipoDocumento.DsTipoDoc;
                        itemDocumento.NmDocumento = itemDoc.NmDocumento;
                        itemDocumento.DtUpLoad = itemDoc.DtUpLoad;
                        itemDocumento.NmCriador = itemDoc.NmCriador;
                        itemDocumento.TextDown = "Download";

                        listaDocs.Add(itemDocumento);
                    }

                    return listaDocs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaDocumentos");

                    throw;
                }
            }
        }

        [HttpGet]
        [Route("DownloadDoc/{id}")]
        public async Task<IActionResult> DownloadDoc(int id)
        {
            using (var db = new FIPEContratosContext())
            {

                try
                {
                    var retornoDocumento = new bOportunidade(db).BuscarDocumentoId(id);

                    if (retornoDocumento != null && retornoDocumento.DocFisico != null)
                    {
                        var stream = new MemoryStream(retornoDocumento.DocFisico);

                        if (stream == null)
                            return NotFound();

                        return File(stream, "application/octet-stream", retornoDocumento.NmDocumento);
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-DownloadDoc");


                    return NotFound();
                }

                return NotFound();
            }
        }

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
                            var objDoc = new bOportunidade(db);

                            // Inicia transação


                            objDoc.RemoveDocumento(id);

                            db.Database.CommitTransaction();

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ExcluirDocumento");


                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaTipoDocs")]
        public List<OutPutTipoDocumento> ListaTipoDocs()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listatipoDocs = new List<OutPutTipoDocumento>();

                    var tipoDocumentos = new bOportunidade(db).BuscarTipoDocumentos();

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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaTipoDocs");


                    throw;
                }
            }
        }

        #endregion

        #region || Métodos Contato

        [HttpPost]
        [Route("AddContato")]
        public OutPutContato AddContato([FromBody]InputAddContato item)
        {
            var retorno = new OutPutContato();
            var contato = new OportunidadeContato();

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


                            var objContato = new bOportunidade(db);

                            contato.NmContato = item.NmContato;
                            contato.CdEmail = item.CdEmail;
                            contato.NuTelefone = item.NuTelefone;
                            contato.NuCelular = item.NuCelular;
                            contato.NmDepartamento = item.NmDepartamento;
                            contato.IdOportunidade = item.IdOportunidade;
                            contato.IdTipoContato = item.IdTipoContato;

                            objContato.AddContato(contato);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.IdOportunidadeContato = contato.IdOportunidadeContato;

                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-AddContato");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

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
                            // Inicia transação

                            var objContato = new bOportunidade(db);
                            var itemContato = new OportunidadeContato();

                            itemContato.IdOportunidadeContato = item.IdOportunidadeContato.Value;
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
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-AtualizarContato");


                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaTipoContato")]
        public List<OutPutGetContato> ListaTipoContato()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaContato = new List<OutPutGetContato>();

                    var contatos = new bOportunidade(db).BuscarTipoContato();

                    foreach (var itemContato in contatos)
                    {

                        var item = new OutPutGetContato();

                        item.IdTipoContato = itemContato.IdTipoContato;
                        item.DsTipoContato = itemContato.DsTipoContato;

                        listaContato.Add(item);
                    }

                    return listaContato;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaTipoContato");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaContato/{id}")]
        public List<InputAddContato> ListaContato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listacontato = new List<InputAddContato>();

                    var contatos = new bOportunidade(db).BuscarContato(id);

                    foreach (var item in contatos)
                    {
                        var listaCont = db.OportunidadeContato.Where(e => e.IdOportunidadeContato == item.IdOportunidadeContato).ToList();

                        foreach (var itemCont in listaCont)
                        {
                            var contato = new InputAddContato();
                            contato.IdOportunidadeContato = item.IdOportunidadeContato;
                            contato.NmContato = item.NmContato;
                            contato.CdEmail = item.CdEmail;
                            contato.NuTelefone = Regex.Replace(item.NuTelefone, "[^0-9a-zA-Z]+", "");
                            contato.NuCelular = item.NuCelular;
                            contato.IdOportunidade = item.IdOportunidade;
                            contato.IdOportunidadeContato = item.IdOportunidadeContato;
                            contato.NmDepartamento = item.NmDepartamento;
                            contato.IdTipoContato = item.IdTipoContato;

                            listacontato.Add(contato);
                        }
                    }
                    return listacontato;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-ListaContato");


                    throw;
                }
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

                            new bOportunidade(db).RemoverContato(id);
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-RemoverContato");

                            contatoRemovido = false;
                        }

                        return contatoRemovido;
                    }
                });
                return contatoRemovido;
            }
        }

        [HttpGet]
        [Route("BuscaContatoId/{id}")]
        public InputAddContato BuscaClienteId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemContato = new InputAddContato();

                try
                {
                    var retornoContato = new bOportunidade(db).BuscarContatoId(id);

                    itemContato.IdOportunidadeContato = retornoContato.IdOportunidadeContato;
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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "OportunidadeController-BuscaClienteId");


                    throw;
                }
            }
        }




        #endregion

        #endregion

        #region | Retornos   

        public class InputAddOportunidade
        {
            public int IdOportunidade { get; set; }
            public int Status { get; set; }
            public int TipoOportunidade { get; set; }
            public string DsAssunto { get; set; }
            public string DsObservacao { get; set; }
            public DateTime? DtLimiteEntregaProposta { get; set; }
            public List<OutputCliente> Clientes { get; set; }
            public DateTime DtCriacao { get; set; }
            public List<OutputResponsavel> Responsaveis { get; set; }
            public int? IdProposta { get; set; }
            public int? IdUsuarioUltimaAlteracao { get; set; }
            public int? IdUsuarioCriacao { get; set; }
            public string NmUsuario { get; set; }

        }

        public class OutputGet
        {
            public int IdOportunidade { get; set; }
            public List<string> DsCliente { get; set; }
            public string DsClienteTexto { get; set; }
            public string DsAssunto { get; set; }
            public DateTime? DtLimiteEntregaProposta { get; set; }
            public DateTime DtCriacao { get; set; }
            public List<string> Responsavel { get; set; }
            public string ResponsavelTexto { get; set; }
            public string TipoOportunidade { get; set; }
            public string Status { get; set; }
            public string NmCidade { get; set; }
            public string Uf { get; set; }
            public int? IdProposta { get; set; }
        }

        public class OutputGetOportunidade
        {
            public List<OutputGet> oportunidades { get; set; }
            public int Tamanho { get; set; }
        }

        public class OutPutTipoContato
        {
            public int IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }
        }

        public class OutputCliente
        {
            public int? IdCliente { get; set; }
            public int IdPessoa { get; set; }
            public string NmCliente { get; set; }
        }

        public class OutputResponsavel
        {
            public int IdPessoaFisica { get; set; }
            public string Responsavel { get; set; }
            public string NmCoord { get; set; }
            public string NmPessoa { get; set; }

        }

        public class OutputSituacao
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string Status { get; set; }

        }

        public class OutputEstado
        {
            public int IdEstado { get; set; }
            public string NmEstado { get; set; }
            public string DsEstado { get; set; }
            public string UF { get; set; }

        }

        public class OutPutMunicipio
        {
            public int IdCidade { get; set; }
            public string NmCidade { get; set; }
        }

        public class OutputTipoOportunidade
        {
            public int IdTipoOportunidade { get; set; }
            public string DsTipoOportunidade { get; set; }
            public string TipoOportunidade { get; set; }

        }

        public class OutPutListaSituacao
        {
            public int IdSituacao { get; set; }
            public string DsSituacao { get; set; }
            public string IcEntidade { get; set; }
        }

        public class OutPutGetContato
        {
            public int IdTipoContato { get; set; }
            public string DsTipoContato { get; set; }

        }

        public class OutPutOportunidade
        {
            public bool Result { get; set; }
            public int IdOportunidade { get; set; }
        }

        public class OutPutContato
        {
            public bool Result { get; set; }
            public int IdOportunidadeContato { get; set; }

        }

        public class InputPesquisaOportunidade
        {
            public string Palavra { get; set; }
            public string Url { get; set; }
        }

        public class InputAddContato
        {
            public string NmContato { get; set; }
            public string CdEmail { get; set; }
            public string NuTelefone { get; set; }
            public string NuCelular { get; set; }
            public string NmDepartamento { get; set; }
            public int? IdTipoContato { get; set; }
            public int? IdOportunidade { get; set; }
            public int? IdOportunidadeContato { get; set; }
        }

        // Documento

        public class OutPutDocumento
        {

            public int IdOportunidadeDocs { get; set; }
            public int IdOportunidade { get; set; }
            public short IdTipoDocumento { get; set; }
            public string NmDocumento { get; set; }
            public byte[] DocFisico { get; set; }
            public DateTime DtUpLoad { get; set; }
            public string NmCriador { get; set; }
            public string TextDown { get; set; }
            public string DsTipoDocumento { get; set; }

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

        #endregion

        #region | Métodos Auxiliares
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        private OutputGet ObterOutputGet(FIPEContratosContext db, Oportunidade item)
        {
            var lstResponsaveis = db.OportunidadeResponsavel.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();
            var lstOpClientes = db.OportunidadeCliente.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();
            var dsCliente = string.Empty;
            var nmCidade = string.Empty;
            var uf = string.Empty;
            int? idProposta = null;
            string responsaveisTexto = string.Empty;
            string clientesTexto = string.Empty;
            List<string> responsaveis = new List<string>();
            List<string> clientes = new List<string>();
            if (lstResponsaveis.Count > 0)
            {
                foreach (var resp in lstResponsaveis)
                {
                    var responsavel = db.PessoaFisica.Where(w => w.IdPessoaFisica == resp.IdPessoaFisica).FirstOrDefault();

                    responsaveis.Add(responsavel.NmPessoa);
                    responsaveisTexto = responsaveisTexto + " " + responsavel.NmPessoa;
                }
            }
            else
            {
                responsaveis.Add("A definir");
                responsaveisTexto = "A definir";
            }
            if (lstOpClientes.Count > 0)
            {
                foreach (var opCliente in lstOpClientes)
                {
                    var cliente = db.Cliente.Where(w => w.IdCliente == opCliente.IdCliente).FirstOrDefault();
                    var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                    if (pessoa.IdPessoaFisica != null)
                    {
                        var pessoaFisica = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica).FirstOrDefault();
                        if (pessoaFisica.IdCidade != null)
                        {
                            var cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).FirstOrDefault();
                            nmCidade = cidade.NmCidade;
                            uf = cidade.Uf;
                        }
                        dsCliente = pessoaFisica.NmPessoa;

                        clientes.Add(pessoaFisica.NmPessoa);
                        clientesTexto = clientesTexto + " " + pessoaFisica.NmPessoa;
                    }
                    else if (pessoa.IdPessoaJuridica != null)
                    {
                        var pessoaJuridica = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == pessoa.IdPessoaJuridica).FirstOrDefault();
                        if (pessoaJuridica.IdCidade != null)
                        {
                            var cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).FirstOrDefault();
                            nmCidade = cidade.NmCidade;
                            uf = cidade.Uf;
                        }
                        dsCliente = opCliente.NmFantasia;

                        clientes.Add(dsCliente);
                        clientesTexto = clientesTexto + " " + dsCliente;
                    }
                }
            }
            if (item.IdSituacao == 3)
            {
                var proposta = db.Proposta.Where(w => w.IdOportunidade == item.IdOportunidade).FirstOrDefault();
                if (proposta != null)
                {
                    idProposta = proposta.IdProposta;
                }
            }


            return new OutputGet()
            {
                IdOportunidade = item.IdOportunidade,
                DsCliente = clientes,
                DsClienteTexto = clientesTexto,
                DsAssunto = item.DsAssunto,
                DtLimiteEntregaProposta = item.DtLimiteEntregaProposta,
                DtCriacao = item.DtCriacao,
                Responsavel = responsaveis,
                ResponsavelTexto = responsaveisTexto,
                TipoOportunidade = db.TipoOportunidade.Where(w => w.IdTipoOportunidade == item.IdTipoOportunidade).FirstOrDefault().DsTipoOportunidade,
                Status = db.Situacao.Where(w => w.IdSituacao == item.IdSituacao).FirstOrDefault().DsSituacao,
                NmCidade = nmCidade,
                Uf = uf,
                IdProposta = idProposta
            };
        }

        private OutputGet ObterOutputGetOportunidade(FIPEContratosContext db, Oportunidade item)
        {
            var lstResponsaveis = db.OportunidadeResponsavel.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();
            var lstOpClientes = db.OportunidadeCliente.Where(e => e.IdOportunidade == item.IdOportunidade).ToList();
            var dsCliente = string.Empty;
            var nmCidade = string.Empty;
            var uf = string.Empty;
            int? idProposta = null;
            string responsaveisTexto = string.Empty;
            string clientesTexto = string.Empty;
            List<string> responsaveis = new List<string>();
            List<string> clientes = new List<string>();
            if (lstResponsaveis.Count > 0)
            {
                foreach (var resp in lstResponsaveis)
                {
                    var responsavel = db.PessoaFisica.Where(w => w.IdPessoaFisica == resp.IdPessoaFisica).FirstOrDefault();

                    responsaveis.Add(responsavel.NmPessoa);
                    responsaveisTexto = responsaveisTexto + " " + responsavel.NmPessoa;
                }
            }
            else
            {
                responsaveis.Add("A definir");
                responsaveisTexto = "A definir";
            }
            if (lstOpClientes.Count > 0)
            {
                foreach (var opCliente in lstOpClientes)
                {
                    var cliente = db.Cliente.Where(w => w.IdCliente == opCliente.IdCliente).FirstOrDefault();
                    var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();

                    if (pessoa.IdPessoaFisica != null)
                    {
                        var pessoaFisica = db.PessoaFisica.Where(w => w.IdPessoaFisica == pessoa.IdPessoaFisica).FirstOrDefault();
                        var cidade = db.Cidade.Where(c => c.IdCidade == pessoaFisica.IdCidade).FirstOrDefault();
                        dsCliente = pessoaFisica.NmPessoa;
                        nmCidade = cidade.NmCidade;
                        uf = cidade.Uf;

                        clientes.Add(pessoaFisica.NmPessoa);
                        clientesTexto = clientesTexto + " " + pessoaFisica.NmPessoa;
                    }
                    else if (pessoa.IdPessoaJuridica != null)
                    {
                        var pessoaJuridica = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == pessoa.IdPessoaJuridica).FirstOrDefault();
                        var cidade = db.Cidade.Where(c => c.IdCidade == pessoaJuridica.IdCidade).FirstOrDefault();
                        dsCliente = pessoaJuridica.NmFantasia;
                        nmCidade = cidade.NmCidade;
                        uf = cidade.Uf;

                        clientes.Add(pessoaJuridica.RazaoSocial);
                        clientesTexto = clientesTexto + " " + pessoaJuridica.RazaoSocial;
                    }
                }
            }
            if (item.IdSituacao == 3)
            {
                var proposta = db.Proposta.Where(w => w.IdOportunidade == item.IdOportunidade).FirstOrDefault();
                if (proposta != null)
                {
                    idProposta = proposta.IdProposta;
                }
            }

            return new OutputGet()
            {
                IdOportunidade = item.IdOportunidade,
                DsCliente = clientes,
                DsClienteTexto = clientesTexto,
                DsAssunto = item.DsAssunto,
                DtLimiteEntregaProposta = item.DtLimiteEntregaProposta,
                DtCriacao = item.DtCriacao,
                Responsavel = responsaveis,
                ResponsavelTexto = responsaveisTexto,
                TipoOportunidade = db.TipoOportunidade.Where(w => w.IdTipoOportunidade == item.IdTipoOportunidade).FirstOrDefault().DsTipoOportunidade,
                Status = db.Situacao.Where(w => w.IdSituacao == item.IdSituacao).FirstOrDefault().DsSituacao,
                NmCidade = nmCidade,
                Uf = uf,
                IdProposta = idProposta
            };
        }

        #endregion
    }
}