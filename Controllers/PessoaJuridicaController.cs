using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaJuridicaController : ControllerBase
    {

        [HttpGet]
        [Route("PesquisaPessoaJuridica/{palavra}")]
        public List<OutPutPessoaJuridicaGrid> PesquisaPessoaJuridica(string palavra)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPessoaJuridica = new List<OutPutPessoaJuridicaGrid>();
                    var bPessoaJuridica = new bPessoaJuridica(db);
                    var pessoasJuridicas = bPessoaJuridica.BuscarPessoaJuridica();
                    var query = new List<OutPutPessoaJuridicaGrid>();
                    var pesqPalavra = palavra.Split(" ");

                    foreach (var itemPalavra in pesqPalavra)
                    {
                        var queryPalavra = pessoasJuridicas.Where(w => (w.NmFantasia != null && w.NmFantasia.ToUpper().Contains(itemPalavra.ToUpper())) ||
                                                        (w.Endereco != null && w.Endereco.ToUpper().Contains(itemPalavra.ToUpper())) ||
                                                         (w.Cnpj != null && w.Cnpj.ToUpper().Contains(itemPalavra.ToUpper())) ||
                                                         (w.IdCidade > 0 && bPessoaJuridica.ObterNomeCidade((Int32)w.IdCidade).ToUpper().Contains(itemPalavra.ToUpper()))).ToList();

                        foreach (var item in queryPalavra)
                        {
                            var itemPessoaJuridica = new OutPutPessoaJuridicaGrid();

                            if (queryPalavra != null)
                            {
                                itemPessoaJuridica.IdPessoaJuridica = item.IdPessoaJuridica;
                                itemPessoaJuridica.NmFantasia = item.NmFantasia;
                                itemPessoaJuridica.Endereco = item.Endereco;
                                itemPessoaJuridica.IdCidade = db.Cidade.Where(w => w.IdCidade == item.IdCidade).FirstOrDefault().NmCidade;
                                itemPessoaJuridica.cnpj = item.Cnpj;

                                query.Add(itemPessoaJuridica);
                            }
                        }
                    }

                    return query.OrderBy(o => o.IdPessoaJuridica).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-PesquisaPessoaJuridica");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddPessoaJuridica")]
        public OutPutPessoaJuridica AddPessoaJuridica([FromBody]InputAddPessoaJuridica item)
        {
            var retorno = new OutPutPessoaJuridica();
            var pessoaJuridica = new PessoaJuridica();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno.CNPJExiste = false;
                            retorno.IdPessoaJuridica = 0;

                            var objPessoaJuridica = new bPessoaJuridica(db);
                            PessoaJuridica pessoaJuridicaExiste = null;
                            if (!String.IsNullOrEmpty(item.Cnpj))
                            {
                                pessoaJuridicaExiste = db.PessoaJuridica.Where(w => w.Cnpj == item.Cnpj).FirstOrDefault();
                            }
                            else
                            {
                                item.Cnpj = null;
                            }
                            if (pessoaJuridicaExiste == null)
                            {
                                pessoaJuridica.NmFantasia = item.NmFantasia;
                                pessoaJuridica.RazaoSocial = item.RazaoSocial;
                                pessoaJuridica.Cnpj = item.Cnpj;
                                pessoaJuridica.Cep = item.Cep;
                                pessoaJuridica.Uf = item.Uf;
                                pessoaJuridica.IdCidade = item.IdCidade;
                                pessoaJuridica.Endereco = item.Endereco;
                                pessoaJuridica.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;
                                pessoaJuridica.IdEsferaEmpresa = item.IdEsferaEmpresa;
                                pessoaJuridica.NmBairro = item.NmBairro;
                                pessoaJuridica.IdTipoAdministracao = item.IdTipoAdministracao;
                                pessoaJuridica.IdEntidade = item.IdEntidade;
                                pessoaJuridica.NuEndereco = item.NuEndereco;
                                pessoaJuridica.Complemento = item.Complemento;
                                pessoaJuridica.DtCriacao = DateTime.Now;
                                pessoaJuridica.IdUsuarioCriacao = AppSettings.constGlobalUserID;

                                if (item.IdClassificacaoEmpresa != 3)
                                {
                                    pessoaJuridica.IdPais = 76;
                                }
                                else
                                {
                                    pessoaJuridica.IdPais = item.IdPais;
                                    pessoaJuridica.DsInternacional = item.DsInternacional;
                                }

                                db.PessoaJuridica.Add(pessoaJuridica);
                                db.SaveChanges();

                                var pessoa = new Pessoa();
                                pessoa.IdPessoaJuridica = pessoaJuridica.IdPessoaJuridica;

                                retorno.IdPessoaJuridica = pessoaJuridica.IdPessoaJuridica;

                                db.Pessoa.Add(pessoa);
                                db.SaveChanges();

                                db.Database.CommitTransaction();

                                retorno.Result = true;
                            }
                            else
                            {
                                retorno.CNPJExiste = true;
                                retorno.Result = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException.Message.Contains("UQ_PessoaJuridicaCNPJ"))
                            {
                                retorno.Result = false;
                                retorno.CNPJExiste = true;
                            }
                            else
                            {
                                new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-AddPessoaJuridica");
                            }

                        }
                    }
                });

                return retorno;
            }
        }

        [HttpPut]
        [Route("AtualizarPessoaJuridica")]
        public OutPutPessoaJuridica AtualizarPessoaJuridica([FromBody]InputUpdatePessoaJuridica item)
        {
            using (var db = new FIPEContratosContext())
            {
                var retorno = new OutPutPessoaJuridica();
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objPessoaJuridica = new bPessoaJuridica(db);
                            var itemPessoaJuridica = new PessoaJuridica();
                            retorno.CNPJExiste = false;
                            retorno.IdPessoaJuridica = 0;
                            PessoaJuridica pessoaJuridicaExiste = null;
                            if (!String.IsNullOrEmpty(item.Cnpj))
                            {
                                pessoaJuridicaExiste = db.PessoaJuridica.Where(w => w.Cnpj == item.Cnpj && w.IdPessoaJuridica != item.IdPessoaJuridica).FirstOrDefault();
                            }
                            else
                            {
                                item.Cnpj = null;
                            }

                            if (pessoaJuridicaExiste == null)
                            {
                                var itemPessoaJuridicaOld = objPessoaJuridica.BuscarPessoaJuridicaId(item.IdPessoaJuridica);
                                var itemHistoricoPesJuridica = new HistoricoPessoaJuridica();

                                //Histórico Pessoa Juridica

                                itemHistoricoPesJuridica.IdPessoaJuridica = itemPessoaJuridicaOld.IdPessoaJuridica;
                                itemHistoricoPesJuridica.IdUsuarioAlteracao = AppSettings.constGlobalUserID;
                                itemHistoricoPesJuridica.DtAlteracao = DateTime.Now.Date;
                                itemHistoricoPesJuridica.NmFantasia = itemPessoaJuridicaOld.NmFantasia;
                                itemHistoricoPesJuridica.RazaoSocial = itemPessoaJuridicaOld.RazaoSocial;
                                if (itemPessoaJuridicaOld.Cnpj != null)
                                {
                                    itemHistoricoPesJuridica.Cnpj = itemPessoaJuridicaOld.Cnpj;
                                }
                                itemHistoricoPesJuridica.Cep = itemPessoaJuridicaOld.Cep;
                                itemHistoricoPesJuridica.Uf = !String.IsNullOrEmpty(itemPessoaJuridicaOld.Uf) ? itemPessoaJuridicaOld.Uf : null;
                                itemHistoricoPesJuridica.IdCidade = itemPessoaJuridicaOld.IdCidade;
                                itemHistoricoPesJuridica.Endereco = itemPessoaJuridicaOld.Endereco;
                                itemHistoricoPesJuridica.IdClassificacaoEmpresa = itemPessoaJuridicaOld.IdClassificacaoEmpresa;
                                itemHistoricoPesJuridica.NmBairro = itemPessoaJuridicaOld.NmBairro;
                                itemHistoricoPesJuridica.IdEsferaEmpresa = itemPessoaJuridicaOld.IdEsferaEmpresa;
                                itemHistoricoPesJuridica.IdTipoAdministracao = itemPessoaJuridicaOld.IdTipoAdministracao;
                                itemHistoricoPesJuridica.IdEntidade = itemPessoaJuridicaOld.IdEntidade;
                                itemHistoricoPesJuridica.NuEndereco = itemPessoaJuridicaOld.NuEndereco;
                                itemHistoricoPesJuridica.Complemento = itemPessoaJuridicaOld.Complemento;
                                if (itemPessoaJuridicaOld.IdClassificacaoEmpresa != 3)
                                {
                                    itemHistoricoPesJuridica.IdPais = 76;
                                }
                                else
                                {
                                    itemHistoricoPesJuridica.IdPais = itemPessoaJuridicaOld.IdPais;
                                    itemHistoricoPesJuridica.DsInternacional = itemPessoaJuridicaOld.DsInternacional;
                                }
                                objPessoaJuridica.AddHistoricoPessoaJuridica(itemHistoricoPesJuridica);

                                //EditarCliente

                                itemPessoaJuridica.IdPessoaJuridica = item.IdPessoaJuridica;
                                itemPessoaJuridica.NmFantasia = item.NmFantasia;
                                itemPessoaJuridica.RazaoSocial = item.RazaoSocial;
                                if (item.Cnpj != null)
                                {
                                    itemPessoaJuridica.Cnpj = item.Cnpj;
                                }
                                itemPessoaJuridica.Cep = item.Cep;
                                itemPessoaJuridica.Uf = !String.IsNullOrEmpty(item.Uf) ? item.Uf : null;
                                itemPessoaJuridica.IdCidade = item.IdCidade;
                                itemPessoaJuridica.Endereco = item.Endereco;
                                itemPessoaJuridica.NmBairro = item.NmBairro;
                                itemPessoaJuridica.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;
                                itemPessoaJuridica.IdEsferaEmpresa = item.IdEsferaEmpresa;
                                itemPessoaJuridica.IdTipoAdministracao = item.IdTipoAdministracao;
                                itemPessoaJuridica.IdEntidade = item.IdEntidade;
                                itemPessoaJuridica.NuEndereco = item.NuEndereco;
                                itemPessoaJuridica.Complemento = item.Complemento;
                                if (item.IdClassificacaoEmpresa != 3)
                                {
                                    itemPessoaJuridica.IdPais = 76;
                                }
                                else
                                {
                                    itemPessoaJuridica.IdPais = item.IdPais;
                                    itemPessoaJuridica.DsInternacional = item.DsInternacional;
                                }
                                objPessoaJuridica.UpdatePessoaJuridica(itemPessoaJuridica);

                                db.Database.CommitTransaction();

                                retorno.Result = true;
                            }
                            else
                            {
                                retorno.CNPJExiste = true;
                                retorno.Result = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException.Message.Contains("UQ_PessoaJuridicaCNPJ"))
                            {
                                retorno.Result = false;
                                retorno.CNPJExiste = true;
                            }
                            else
                            {
                                new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-AtualizarPessoaJuridica");
                            }
                        }
                    }
                });

                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaPessoaJuridica")]
        public List<OutPutPessoaJuridicaGrid> ListaPessoaJuridica()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPessoaJuridica = new List<OutPutPessoaJuridicaGrid>();

                    var pessoasJuridicas = new bPessoaJuridica(db).BuscarPessoaJuridica();

                    foreach (var item in pessoasJuridicas)
                    {
                        var pessoaJuridica = new OutPutPessoaJuridicaGrid();
                        pessoaJuridica.IdPessoaJuridica = item.IdPessoaJuridica;
                        pessoaJuridica.NmFantasia = item.NmFantasia;
                        if (item.IdCidade != null)
                        {
                            pessoaJuridica.IdCidade = db.Cidade.Where(w => w.IdCidade == item.IdCidade).FirstOrDefault().NmCidade;
                        }
                        pessoaJuridica.Endereco = item.Endereco;
                        if (item.Cnpj != null)
                        {
                            pessoaJuridica.cnpj = item.Cnpj;
                        }
                        pessoaJuridica.Uf = item.Uf;

                        listaPessoaJuridica.Add(pessoaJuridica);
                    }

                    return listaPessoaJuridica.OrderBy(o => o.NmFantasia).ToList();

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaPessoaJuridica");


                    throw;
                }

            }
        }

        [HttpGet]
        [Route("BuscaPessoaJuridicaId/{id}")]
        public InputUpdatePessoaJuridica BuscaPessoaJuridicaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemPessoaJuridica = new InputUpdatePessoaJuridica();

                try
                {
                    var retornoPessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(id);

                    itemPessoaJuridica.IdPessoaJuridica = retornoPessoaJuridica.IdPessoaJuridica;
                    itemPessoaJuridica.NmFantasia = retornoPessoaJuridica.NmFantasia;
                    itemPessoaJuridica.RazaoSocial = retornoPessoaJuridica.RazaoSocial;
                    itemPessoaJuridica.IdCidade = retornoPessoaJuridica.IdCidade;
                    if (retornoPessoaJuridica.Cnpj != null)
                    {
                        itemPessoaJuridica.Cnpj = Regex.Replace(retornoPessoaJuridica.Cnpj, "[^0-9a-zA-Z]+", "");
                    }
                    itemPessoaJuridica.Uf = retornoPessoaJuridica.Uf;
                    itemPessoaJuridica.Endereco = retornoPessoaJuridica.Endereco;
                    if (!String.IsNullOrEmpty(retornoPessoaJuridica.Cep))
                    {
                        itemPessoaJuridica.Cep = Regex.Replace(retornoPessoaJuridica.Cep, "[^0-9a-zA-Z]+", "");
                    }
                    itemPessoaJuridica.NmBairro = retornoPessoaJuridica.NmBairro;
                    itemPessoaJuridica.IdClassificacaoEmpresa = retornoPessoaJuridica.IdClassificacaoEmpresa;
                    itemPessoaJuridica.IdEsferaEmpresa = retornoPessoaJuridica.IdEsferaEmpresa;
                    itemPessoaJuridica.IdTipoAdministracao = retornoPessoaJuridica.IdTipoAdministracao;
                    itemPessoaJuridica.IdEntidade = retornoPessoaJuridica.IdEntidade;
                    itemPessoaJuridica.IdPais = retornoPessoaJuridica.IdPais;
                    itemPessoaJuridica.DsInternacional = retornoPessoaJuridica.DsInternacional;
                    itemPessoaJuridica.NuEndereco = retornoPessoaJuridica.NuEndereco;
                    itemPessoaJuridica.Complemento = retornoPessoaJuridica.Complemento;

                    return itemPessoaJuridica;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-BuscaPessoaJuridicaId");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ConsultarCNPJPessoaJuridica/{cnpj}")]
        public bool ConsultarCNPJPessoaJuridica(string cnpj)
        {
            bool cnpjCadastrado = false;

            if (!String.IsNullOrEmpty(cnpj))
            {

                using (var db = new FIPEContratosContext())
                    cnpjCadastrado = new bPessoaJuridica(db).ObterPessoaJuridicaPorCNPJ(cnpj) != null;
            }
            return cnpjCadastrado;
        }

        [HttpGet]
        [Route("ConsultarCNPJPessoaJuridicaAtualizacao/{cnpj}/{id}")]
        public bool ConsultarCNPJPessoaJuridicaAtualizacao(string cnpj, int id)
        {
            bool cnpjCadastrado = false;

            if (!String.IsNullOrEmpty(cnpj))
            {
                using (var db = new FIPEContratosContext())
                {
                    var pessoaJuridica = new bPessoaJuridica(db).ObterPessoaJuridicaPorCNPJ(cnpj);
                    cnpjCadastrado = pessoaJuridica != null && pessoaJuridica.IdPessoaJuridica != id;
                }
            }

            return cnpjCadastrado;
        }

        [HttpGet]
        [Route("ListaEsfera/{id}")]
        public List<OutputGetEsfera> ListaEsfera(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaEsferas = new List<OutputGetEsfera>();

                    var esferas = new bPessoaJuridica(db).BuscarEsfera(id);

                    foreach (var itemEsf in esferas)
                    {

                        var itemEsfera = new OutputGetEsfera();

                        itemEsfera.IdEsferaEmpresa = itemEsf.IdEsferaEmpresa;
                        itemEsfera.DsEsferaEmpresa = itemEsf.DsEsferaEmpresa;
                        itemEsfera.IdClassificacaoEmpresa = itemEsfera.IdClassificacaoEmpresa;

                        listaEsferas.Add(itemEsfera);
                    }

                    return listaEsferas;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaEsfera");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaAdm")]
        public List<OutputGetAdm> ListaAdm()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaAdministracao = new List<OutputGetAdm>();

                    var adms = new bPessoaJuridica(db).BuscarAdm();

                    foreach (var itemAdministracao in adms)
                    {

                        var itemAdm = new OutputGetAdm();

                        itemAdm.IdTipoAdministracao = itemAdministracao.IdTipoAdministracao;
                        itemAdm.DsTipoAdministracao = itemAdministracao.DsTipoAdministracao;

                        listaAdministracao.Add(itemAdm);
                    }

                    return listaAdministracao;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaAdm");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaEntidade/{tipoEntidade}")]
        public List<OutputGetEntidade> ListaEntidade(int tipoEntidade)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaEntidade = new List<OutputGetEntidade>();

                    var entidades = new bPessoaJuridica(db).BuscarEntidade(tipoEntidade);

                    foreach (var itemEntidades in entidades)
                    {
                        var itemEnts = new OutputGetEntidade();

                        itemEnts.IdEntidade = itemEntidades.IdEntidade;
                        itemEnts.DsEntidade = itemEntidades.DsEntidade;

                        listaEntidade.Add(itemEnts);
                    }

                    return listaEntidade;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaEntidade");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaPais")]
        public List<OutputGetPais> ListaPais()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPais = new List<OutputGetPais>();

                    var paises = new bPessoaJuridica(db).BuscarPaises();

                    foreach (var itemPaises in paises)
                    {

                        var itemP = new OutputGetPais();

                        itemP.IdPais = (Int32)itemPaises.IdPais;
                        itemP.Nome = itemPaises.Nome;

                        listaPais.Add(itemP);
                    }

                    return listaPais;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaPais");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaCidade/{Uf}")]
        public List<OutputGetCidade> ListaCidade(string Uf)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaCidades = new List<OutputGetCidade>();

                    var cidades = new bPessoaJuridica(db).BuscarCidade(Uf);

                    foreach (var itemCid in cidades)
                    {

                        var itemCidade      = new OutputGetCidade();
                        itemCidade.IdCidade = itemCid.IdCidade;
                        itemCidade.NmCidade = itemCid.NmCidade;
                        itemCidade.Uf = itemCid.Uf;

                        listaCidades.Add(itemCidade);
                    }

                    if (Uf == "SP")
                    {
                        var SP = listaCidades.Where(w => w.NmCidade == "São Paulo").First();

                        listaCidades.Remove(SP);
                        listaCidades.Insert(0, SP);
                    }
                    return listaCidades;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaCidade");


                    throw;
                }
            }
        }

        //Historico Pessoa Juridica

        [HttpGet]
        [Route("ListaHistPesJuridica/{id}")]
        public List<OutPutGetHistoricoPesJuridica> ListaHistPesJuridica(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaHistoricoPesJuridica = new List<OutPutGetHistoricoPesJuridica>();

                    var HistoricoPessoasJuridicas = new bPessoaJuridica(db).BuscarHistoricoPessoaJuridica(id);

                    foreach (var itemHisPesJuridica in HistoricoPessoasJuridicas)
                    {
                        var itemHistoricoPesJuridica = new OutPutGetHistoricoPesJuridica();

                        itemHistoricoPesJuridica.IdHistPessoaJuridica = itemHisPesJuridica.IdHistPessoaJuridica;
                        itemHistoricoPesJuridica.IdPessoaJuridica = itemHisPesJuridica.IdPessoaJuridica;
                        var usuario = new bUsuario(db).GetById(itemHisPesJuridica.IdUsuarioAlteracao);
                        if (usuario != null)
                        {
                            var pessoaFisica = new bPessoaFisica(db).GetById(usuario.IdPessoa);
                            itemHistoricoPesJuridica.NmUsuarioAlteracao = pessoaFisica.NmPessoa;
                        }
                        itemHistoricoPesJuridica.DtAlteracao = itemHisPesJuridica.DtAlteracao.ToShortDateString();
                        itemHistoricoPesJuridica.NmFantasia = itemHisPesJuridica.NmFantasia;
                        itemHistoricoPesJuridica.RazaoSocial = itemHisPesJuridica.RazaoSocial;
                        itemHistoricoPesJuridica.Cnpj = itemHisPesJuridica.Cnpj;
                        itemHistoricoPesJuridica.Cep = itemHisPesJuridica.Cep;
                        itemHistoricoPesJuridica.Uf = itemHisPesJuridica.Uf;
                        itemHistoricoPesJuridica.IdCidade = itemHisPesJuridica.IdCidade;
                        itemHistoricoPesJuridica.Endereco = itemHisPesJuridica.Endereco;
                        itemHistoricoPesJuridica.IdEsfera = itemHisPesJuridica.IdEsfera;
                        itemHistoricoPesJuridica.NmBairro = itemHisPesJuridica.NmBairro;
                        itemHistoricoPesJuridica.NuEndereco = itemHisPesJuridica.NuEndereco;
                        itemHistoricoPesJuridica.Complemento = itemHisPesJuridica.Complemento;

                        listaHistoricoPesJuridica.Add(itemHistoricoPesJuridica);
                    }

                    return listaHistoricoPesJuridica.OrderByDescending(o => o.IdHistPessoaJuridica).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-ListaHistPesJuridica");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaHistId/{id}")]
        public OutPutGetHistoricoPesJuridica BuscaHistId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemPessoaJuridica = new OutPutGetHistoricoPesJuridica();

                try
                {
                    var retornoPessoaJuridica = new bPessoaJuridica(db).BuscarHistId(id);

                    itemPessoaJuridica.IdHistPessoaJuridica = retornoPessoaJuridica.IdHistPessoaJuridica;
                    itemPessoaJuridica.IdPessoaJuridica = retornoPessoaJuridica.IdPessoaJuridica;
                    itemPessoaJuridica.NmFantasia = retornoPessoaJuridica.NmFantasia;
                    itemPessoaJuridica.RazaoSocial = retornoPessoaJuridica.RazaoSocial;
                    itemPessoaJuridica.IdCidade = retornoPessoaJuridica.IdCidade;
                    if (retornoPessoaJuridica.Cnpj != null)
                    {
                        itemPessoaJuridica.Cnpj = Regex.Replace(retornoPessoaJuridica.Cnpj, "[^0-9a-zA-Z]+", "");
                    }
                    itemPessoaJuridica.Uf = retornoPessoaJuridica.Uf;
                    itemPessoaJuridica.Endereco = retornoPessoaJuridica.Endereco;
                    itemPessoaJuridica.Cep = Regex.Replace(retornoPessoaJuridica.Cep, "[^0-9a-zA-Z]+", "");
                    itemPessoaJuridica.NmBairro = retornoPessoaJuridica.NmBairro;
                    itemPessoaJuridica.IdClassificacaoEmpresa = retornoPessoaJuridica.IdClassificacaoEmpresa;
                    itemPessoaJuridica.IdEsferaEmpresa = retornoPessoaJuridica.IdEsferaEmpresa;
                    itemPessoaJuridica.IdTipoAdministracao = retornoPessoaJuridica.IdTipoAdministracao;
                    itemPessoaJuridica.IdEntidade = retornoPessoaJuridica.IdEntidade;
                    itemPessoaJuridica.IdPais = retornoPessoaJuridica.IdPais;
                    itemPessoaJuridica.DsInternacional = retornoPessoaJuridica.DsInternacional;
                    itemPessoaJuridica.Complemento = retornoPessoaJuridica.Complemento;
                    itemPessoaJuridica.NuEndereco = retornoPessoaJuridica.NuEndereco;

                    return itemPessoaJuridica;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaJuridicaController-BuscaHistId");


                    throw;
                }

            }
        }
    }

    #region Retornos   

    public class InputAddPessoaJuridica
    {
        public string NmFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public short IdEsfera { get; set; }
        public int? IdCidade { get; set; }
        public string Cnpj { get; set; }
        public string Uf { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string NmBairro { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }
        public int? IdEsferaEmpresa { get; set; }
        public int? IdTipoAdministracao { get; set; }

        public int? IdEntidade { get; set; }
        public int? IdPais { get; set; }
        public string DsInternacional { get; set; }
        public int NumCode { get; set; }
        public string NuEndereco { get; set; }
        public string Complemento { get; set; }
        public int IdUsuario { get; set; }
    }

    public class InputUpdatePessoaJuridica
    {
        public int IdPessoaJuridica { get; set; }
        public string NmFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public int? IdCidade { get; set; }
        public string Cnpj { get; set; }
        public string Uf { get; set; }
        public string Endereco { get; set; }
        public string Cep { get; set; }
        public string NmBairro { get; set; }
        public int IdUsuarioAlteracao { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }
        public int? IdEsferaEmpresa { get; set; }
        public int? IdTipoAdministracao { get; set; }

        public int? IdEntidade { get; set; }

        public int? IdPais { get; set; }
        public string DsInternacional { get; set; }
        public int NumCode { get; set; }
        public string NuEndereco { get; set; }
        public string Complemento { get; set; }

    }

    public class OutPutPessoaJuridica
    {
        public bool Result { get; set; }
        public int IdPessoaJuridica { get; set; }
        public bool CNPJExiste { get; set; }

    }

    public class OutPutPessoaJuridicaGrid
    {
        public int IdPessoaJuridica { get; set; }
        public string NmFantasia { get; set; }
        public string Endereco { get; set; }
        public string IdCidade { get; set; }
        public string cnpj { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
    }

    public class OutputGetEsfera
    {
        public int IdEsferaEmpresa { get; set; }
        public string DsEsferaEmpresa { get; set; }
        public int IdClassificacaoEmpresa { get; set; }

    }

    public class OutputGetEntidade
    {
        public int IdEntidade { get; set; }
        public string DsEntidade { get; set; }

    }

    public class OutputGetAdm
    {
        public int IdTipoAdministracao { get; set; }
        public string DsTipoAdministracao { get; set; }

    }

    public class OutputGetPais
    {
        public int IdPais { get; set; }
        public string Nome { get; set; }

    }

    public class OutputGetCidade
    {
        public int IdCidade { get; set; }
        public string NmCidade { get; set; }
        public string Uf { get; set; }
    }

    public class OutPutGetHistoricoPesJuridica
    {
        public string NmUsuarioAlteracao { get; set; }
        public int IdHistPessoaJuridica { get; set; }
        public int IdPessoaJuridica { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string DtAlteracao { get; set; }
        public int? IdCidade { get; set; }
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public short? IdEsfera { get; set; }
        public string Uf { get; set; }
        public string Endereco { get; set; }
        public string NmBairro { get; set; }
        public string Cep { get; set; }
        public string NmFantasia { get; set; }
        public int? IdClassificacaoEmpresa { get; set; }

        public int? IdEsferaEmpresa { get; set; }

        public int? IdTipoAdministracao { get; set; }

        public int? IdEntidade { get; set; }
        public int? IdPais { get; set; }
        public string DsInternacional { get; set; }
        public int NumCode { get; set; }
        public string NuEndereco { get; set; }
        public string Complemento { get; set; }
    }

    #endregion

}