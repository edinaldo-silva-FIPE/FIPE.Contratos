using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaFisicaController : ControllerBase
    {
        [HttpPost]
        [Route("AddPessoaFisica")]
        public OutPutPessoaFisica AddPessoaFisica([FromBody] InputAddPessoaFisica item)
        {
            var retorno = new OutPutPessoaFisica();
            var pessoaFisica = new PessoaFisica();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PessoaFisica cpfCadastrado = null;
                            if (!String.IsNullOrEmpty(item.NuCpf))
                            {
                                cpfCadastrado = new PessoaFisica();
                                string cpf = Regex.Replace(item.NuCpf, "[^0-9a-zA-Z]+", "");
                                cpfCadastrado = new bPessoaFisica(db).VerificaCPFCadastrado(cpf, item.IdPessoaFisica);

                                AdicionarPessoaFisica(item, retorno, pessoaFisica, db, cpfCadastrado);
                            }
                            else
                            {
                                AdicionarPessoaFisica(item, retorno, pessoaFisica, db, cpfCadastrado);
                            }
                            // Confirma operações
                            db.Database.CommitTransaction();

                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException.Message.Contains("UQ_PessoaFisicaCPF"))
                            {
                                retorno.CpfCadastrado = true;
                            }
                            else
                            {
                                new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-AddPessoaFisica");
                            }
                        }
                    }
                });

            }

            return retorno;
        }

        private static void AdicionarPessoaFisica(InputAddPessoaFisica item, OutPutPessoaFisica retorno, PessoaFisica pessoaFisica, FIPEContratosContext db, PessoaFisica cpfCadastrado)
        {
            if (cpfCadastrado == null)
            {
                var objPessoaFisica = new bPessoaFisica(db);

                pessoaFisica.NmPessoa = item.NmPessoa;
                pessoaFisica.NuCpf = !String.IsNullOrEmpty(item.NuCpf) ? item.NuCpf : null;
                pessoaFisica.DtNascimento = item.DtNascimento;
                pessoaFisica.CdSexo = item.CdSexo;
                pessoaFisica.CdEmail = item.CdEmail;
                pessoaFisica.NuCep = item.NuCep;
                pessoaFisica.DsEndereco = item.DsEndereco;
                pessoaFisica.NuEndereco = item.NuEndereco;
                pessoaFisica.DsComplemento = item.DsComplemento;
                pessoaFisica.NmBairro = item.NmBairro;
                pessoaFisica.SgUf = item.SgUf;
                pessoaFisica.IdCidade = item.IdCidade;
                pessoaFisica.CdCvLattes = item.CdCvLattes;
                pessoaFisica.CdLinkedIn = item.CdLinkedIn;
                pessoaFisica.NuTelefoneComercial = item.NuTelefoneComercial;
                pessoaFisica.NuTelefoneFixo = item.NuTelefoneFixo;
                pessoaFisica.NuCelular = item.NuCelular;
                pessoaFisica.IdTipoVinculo = item.IdTipoVinculo;
                pessoaFisica.DtCriacao = DateTime.Now;
                pessoaFisica.IdUsuarioCriacao = AppSettings.constGlobalUserID;

                db.PessoaFisica.Add(pessoaFisica);
                db.SaveChanges();

                var pessoa = new Pessoa();
                pessoa.IdPessoaFisica = pessoaFisica.IdPessoaFisica;

                db.Pessoa.Add(pessoa);
                db.SaveChanges();

                // Confirma operações
                db.Database.CommitTransaction();

                retorno.IdPessoaFisica = pessoaFisica.IdPessoaFisica;
                retorno.Result = true;
            }
            else
            {
                retorno.Result = false;
                retorno.CpfCadastrado = true;
            }
        }

        [HttpGet]
        [Route("BuscaPessoaFisicaId/{id}")]
        public InputUpdatePessoaFisica BuscaPessoaFisicaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemPessoa = new InputUpdatePessoaFisica();

                try
                {
                    var item = new bPessoaFisica(db).BuscarPessoaId(id);

                    itemPessoa.IdPessoaFisica = item.IdPessoaFisica;
                    itemPessoa.NmPessoa = item.NmPessoa;
                    if (!String.IsNullOrEmpty(item.NuCpf))
                    {
                        itemPessoa.NuCpf = Regex.Replace(item.NuCpf, "[^0-9a-zA-Z]+", "");
                    }                    
                    itemPessoa.DtNascimento = item.DtNascimento;
                    itemPessoa.CdSexo = item.CdSexo;
                    itemPessoa.CdEmail = item.CdEmail;
                    if (item.NuCep != null)
                    {
                        itemPessoa.NuCep = Regex.Replace(item.NuCep, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuCep = item.NuCep;
                    }
                    itemPessoa.DsEndereco = item.DsEndereco;
                    itemPessoa.NuEndereco = item.NuEndereco;
                    itemPessoa.DsComplemento = item.DsComplemento;
                    itemPessoa.NmBairro = item.NmBairro;
                    itemPessoa.SgUf = item.SgUf;
                    if (item.IdCidade != null)
                    {
                        itemPessoa.IdCidade = item.IdCidade.Value;
                    }
                    itemPessoa.CdCvLattes = item.CdCvLattes;
                    itemPessoa.CdLinkedIn = item.CdLinkedIn;
                    if (itemPessoa.NuTelefoneComercial != null)
                    {
                        itemPessoa.NuTelefoneComercial = Regex.Replace(item.NuTelefoneComercial, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuTelefoneComercial = item.NuTelefoneComercial;
                    }
                    if (itemPessoa.NuCelular != null)
                    {
                        itemPessoa.NuCelular = Regex.Replace(item.NuCelular, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuCelular = item.NuCelular;
                    }
                    if (itemPessoa.NuTelefoneFixo != null)
                    {
                        itemPessoa.NuTelefoneFixo = Regex.Replace(item.NuTelefoneFixo, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuTelefoneFixo = item.NuTelefoneFixo;
                    }
                    itemPessoa.IdTipoVinculo = item.IdTipoVinculo;


                    return itemPessoa;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-BuscaPessoaFisicaId");


                    throw;
                }

            }
        }

        [HttpGet]
        [Route("BuscaHistId/{id}")]
        public InputAddPessoaFisica BuscaHistId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var itemPessoa = new InputAddPessoaFisica();

                try
                {
                    var item = new bPessoaFisica(db).BuscaHistId(id);

                    itemPessoa.IdHstPessoaFisica = item.IdHstPessoaFisica;
                    itemPessoa.IdPessoaFisica = item.IdPessoaFisica;
                    itemPessoa.NmPessoa = item.NmPessoa;
                    itemPessoa.NuCpf = Regex.Replace(item.NuCpf, "[^0-9a-zA-Z]+", "");
                    itemPessoa.DtNascimento = item.DtNascimento;
                    itemPessoa.CdSexo = item.CdSexo;
                    itemPessoa.CdEmail = item.CdEmail;
                    if (item.NuCep != null)
                    {
                        itemPessoa.NuCep = Regex.Replace(item.NuCep, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuCep = item.NuCep;
                    }
                    itemPessoa.DsEndereco = item.DsEndereco;
                    itemPessoa.NuEndereco = item.NuEndereco;
                    itemPessoa.DsComplemento = item.DsComplemento;
                    itemPessoa.NmBairro = item.NmBairro;
                    itemPessoa.SgUf = item.SgUf;
                    if (item.IdCidade != null)
                    {
                        itemPessoa.IdCidade = item.IdCidade.Value;
                    }
                    itemPessoa.CdCvLattes = item.CdCvLattes;
                    itemPessoa.CdLinkedIn = item.CdLinkedIn;
                    if (itemPessoa.NuTelefoneComercial != null)
                    {
                        itemPessoa.NuTelefoneComercial = Regex.Replace(item.NuTelefoneComercial, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuTelefoneComercial = item.NuTelefoneComercial;
                    }
                    if (itemPessoa.NuCelular != null)
                    {
                        itemPessoa.NuCelular = Regex.Replace(item.NuCelular, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuCelular = item.NuCelular;
                    }
                    if (itemPessoa.NuTelefoneFixo != null)
                    {
                        itemPessoa.NuTelefoneFixo = Regex.Replace(item.NuTelefoneFixo, "[^0-9a-zA-Z]+", "");
                    }
                    else
                    {
                        itemPessoa.NuTelefoneFixo = item.NuTelefoneFixo;
                    }
                    itemPessoa.IdTipoVinculo = item.IdTipoVinculo;

                    return itemPessoa;

                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-BuscaHistId");


                    throw;
                }

            }
        }

        [HttpPut]
        [Route("AtualizarPessoaFisica")]
        public OutPutPessoaFisica AtualizarPessoaFisica([FromBody]InputUpdatePessoaFisica item)
        {
            var retorno = new OutPutPessoaFisica();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PessoaFisica cpfCadastrado = null;
                            if (!String.IsNullOrEmpty(item.NuCpf))
                            {
                                cpfCadastrado   = new PessoaFisica();
                                string cpf      = Regex.Replace(item.NuCpf, "[^0-9a-zA-Z]+", "");
                                cpfCadastrado   = new bPessoaFisica(db).VerificaCPFCadastrado(cpf, item.IdPessoaFisica);
                                UpdatePessoaFisica(item, retorno, db, cpfCadastrado);
                            }
                            else
                            {
                                UpdatePessoaFisica(item, retorno, db, cpfCadastrado);
                            }
                            // Confirma operações
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {                      
                            if (ex.InnerException.Message.Contains("UQ_PessoaFisicaCPF"))
                            {
                                retorno.CpfCadastrado = true;
                            }
                            else
                            {
                                new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-AtualizarPessoaFisica");
                            }
                        }
                    }
                });
            }

            return retorno;

        }

        private static void UpdatePessoaFisica(InputUpdatePessoaFisica item, OutPutPessoaFisica retorno, FIPEContratosContext db, PessoaFisica cpfCadastrado)
        {
            if (cpfCadastrado == null)
            {
                var objPessoa           = new bPessoaFisica(db);
                var itemPessoaOld       = objPessoa.BuscarPessoaId(item.IdPessoaFisica);
                var itemPessoa          = itemPessoaOld;
                var itemPessoaHistorico = new HistoricoPessoaFisica();

                //Histórico
                itemPessoaHistorico.IdPessoaFisica = itemPessoaOld.IdPessoaFisica;
                itemPessoaHistorico.NmPessoa = itemPessoaOld.NmPessoa;
                itemPessoaHistorico.NuCpf = itemPessoaOld.NuCpf;
                itemPessoaHistorico.DtNascimento = itemPessoaOld.DtNascimento;
                itemPessoaHistorico.CdSexo = itemPessoaOld.CdSexo;
                itemPessoaHistorico.CdEmail = itemPessoaOld.CdEmail;
                itemPessoaHistorico.NuCep = itemPessoaOld.NuCep;
                itemPessoaHistorico.DsEndereco = itemPessoaOld.DsEndereco;
                itemPessoaHistorico.NuEndereco = itemPessoaOld.NuEndereco;
                itemPessoaHistorico.DsComplemento = itemPessoaOld.DsComplemento;
                itemPessoaHistorico.NmBairro = itemPessoaOld.NmBairro;
                itemPessoaHistorico.SgUf = itemPessoaOld.SgUf;
                itemPessoaHistorico.IdCidade = itemPessoaOld.IdCidade;
                itemPessoaHistorico.CdCvLattes = itemPessoaOld.CdCvLattes;
                itemPessoaHistorico.CdLinkedIn = itemPessoaOld.CdLinkedIn;
                itemPessoaHistorico.NuTelefoneComercial = itemPessoaOld.NuTelefoneComercial;
                itemPessoaHistorico.NuTelefoneFixo = itemPessoaOld.NuTelefoneFixo;
                itemPessoaHistorico.NuCelular = itemPessoaOld.NuCelular;
                itemPessoaHistorico.IdUsuarioAlteracao = AppSettings.constGlobalUserID;
                itemPessoaHistorico.DtAlteracao = DateTime.Now.Date;
                itemPessoaHistorico.IdTipoVinculo = itemPessoaOld.IdTipoVinculo;

                objPessoa.AddPessoaFisicaHistorico(itemPessoaHistorico);

                itemPessoa.NmPessoa = item.NmPessoa;
                if (item.NuCpf != null)
                {
                    string cpf = Regex.Replace(item.NuCpf, "[^0-9a-zA-Z]+", "");
                    itemPessoa.NuCpf = !String.IsNullOrEmpty(cpf) ? cpf : null;
                }                
                itemPessoa.DtNascimento = item.DtNascimento;
                itemPessoa.CdSexo = item.CdSexo;
                itemPessoa.CdEmail = item.CdEmail;
                if (item.NuCep != null)
                {
                    itemPessoa.NuCep = Regex.Replace(item.NuCep, "[^0-9a-zA-Z]+", "");
                }                
                itemPessoa.DsEndereco = item.DsEndereco;
                itemPessoa.NuEndereco = item.NuEndereco;
                itemPessoa.DsComplemento = item.DsComplemento;
                itemPessoa.NmBairro = item.NmBairro;
                itemPessoa.SgUf = item.SgUf;
                itemPessoa.IdCidade = item.IdCidade;
                itemPessoa.CdCvLattes = item.CdCvLattes;
                itemPessoa.CdLinkedIn = item.CdLinkedIn;
                if (item.NuTelefoneComercial != null)
                {
                    itemPessoa.NuTelefoneComercial = Regex.Replace(item.NuTelefoneComercial, "[^0-9a-zA-Z]+", "");
                }
                if (item.NuTelefoneFixo != null)
                {
                    itemPessoa.NuTelefoneFixo = Regex.Replace(item.NuTelefoneFixo, "[^0-9a-zA-Z]+", "");
                }
                if (item.NuCelular != null)
                {
                    itemPessoa.NuCelular = Regex.Replace(item.NuCelular, "[^0-9a-zA-Z]+", "");
                }
                itemPessoa.IdTipoVinculo = item.IdTipoVinculo;

                retorno.Result = true;
            }
            else
            {
                retorno.Result = false;
                retorno.CpfCadastrado = true;
            }
        }


        [HttpGet]
        [Route("ListaPessoaFisica")]
        public List<OutPutPessoaFisicaGrid> ListaPessoaFisica()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listapessoa = new List<OutPutPessoaFisicaGrid>();
                    var pessoas = new bPessoaFisica(db).BuscarPessoa();

                    foreach (var item in pessoas)
                    {
                        var listaPe = db.PessoaFisica.Where(e => e.IdPessoaFisica == item.IdPessoaFisica).ToList();

                        foreach (var itemPessoa in listaPe)
                        {
                            var pessoa = new OutPutPessoaFisicaGrid();
                            pessoa.IdPessoaFisica = item.IdPessoaFisica;
                            pessoa.NmPessoa = item.NmPessoa;
                            pessoa.NuCpf = item.NuCpf;
                            pessoa.NmCv = item.NmCv;
                            pessoa.CdCvLattes = item.CdCvLattes;

                            if (item.NuEndereco != "")
                            {
                                pessoa.DsEndereco = item.DsEndereco + ", " + item.NuEndereco + " " + item.DsComplemento;
                            }
                            else
                            {
                                pessoa.DsEndereco = item.DsEndereco;
                            }

                            if (itemPessoa.IdCidade != null)
                            {
                                pessoa.IdCidade = db.Cidade.Where(w => w.IdCidade == itemPessoa.IdCidade).FirstOrDefault().NmCidade;
                            }
                            listapessoa.Add(pessoa);
                        }
                    }

                    return listapessoa.OrderBy(o => o.NmPessoa).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-ListaPessoaFisica");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ConsultarCPF/{cpf}/{id}")]
        public bool ConsultarCPF(string cpf, int id)
        {
            bool cpfCadastrado = false;

            using (var db = new FIPEContratosContext())
                cpfCadastrado = new bPessoaFisica(db).VerificaCPFCadastrado(cpf, id) != null;

            return cpfCadastrado;
        }

        [HttpGet]
        [Route("ConsultarCPFAtualizacao/{cpf}/{id}")]
        public bool ConsultarCPFAtualizacao(string cpf, int id)
        {
            bool cpfCadastrado = false;

            using (var db = new FIPEContratosContext())
            {
                var pessoa = new bPessoaFisica(db).ObterPessoaPorCPF(cpf);
                cpfCadastrado = pessoa != null && pessoa.IdPessoaFisica != id;
            }

            return cpfCadastrado;
        }

        [HttpGet]
        [Route("ListaPessoaHistorico/{id}")]
        public List<InputAddPessoaFisica> ListaPessoaHistorico(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listapessoa = new List<InputAddPessoaFisica>();
                    var pessoas = new bPessoaFisica(db).BuscarPessoaHistorico(id);

                    foreach (var item in pessoas)
                    {
                        var listaPe = db.HistoricoPessoaFisica.Where(e => e.IdHstPessoaFisica == item.IdHstPessoaFisica).ToList();

                        foreach (var itemPessoa in listaPe)
                        {
                            var pessoa = new InputAddPessoaFisica();
                            pessoa.IdHstPessoaFisica = item.IdHstPessoaFisica;
                            pessoa.NmPessoa = item.NmPessoa;

                            var usuario = new bUsuario(db).GetById(item.IdUsuarioAlteracao);
                            if (usuario != null)
                            {
                                var pessoaFisica = new bPessoaFisica(db).GetById(usuario.IdPessoa);
                                pessoa.NmUsuarioAlteracao = pessoaFisica.NmPessoa;
                            }

                            pessoa.DtAlteracao = item.DtAlteracao.ToShortDateString();
                            pessoa.NuCpf = item.NuCpf;


                            listapessoa.Add(pessoa);

                        }
                    }

                    return listapessoa.OrderByDescending(o => o.IdHstPessoaFisica).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-ListaPessoaHistorico");


                    throw;
                }
            }
        }

        [HttpPost]
        [Route("UpLoadCurriculo/{id}")]
        public async Task<IActionResult> UpLoadCurriculo(int id)
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

                            var objPessoa = new bPessoaFisica(db).BuscarPessoaId(id);

                            if (files[0].Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    files[0].CopyTo(ms);

                                    var fileBytes = ms.ToArray();

                                    if (objPessoa != null)
                                    {
                                        objPessoa.NmCv = files[0].Name;
                                        objPessoa.DsCv = fileBytes;
                                    }
                                    else
                                    {
                                        var pessoaFisica = new PessoaFisica();

                                        pessoaFisica.NmCv = files[0].Name;
                                        pessoaFisica.DsCv = fileBytes;
                                    }
                                }
                            }

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-UpLoadCurriculo");
                            throw;
                        }
                    }
                });
                return Ok();
            }
        }

        [HttpGet]
        [Route("DownloadCurriculo/{id}")]
        public async Task<IActionResult> DownloadCurriculo(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var objPessoa = new bPessoaFisica(db).BuscarPessoaId(id);

                    if (objPessoa != null && objPessoa.DsCv != null)
                    {
                        var stream = new MemoryStream(objPessoa.DsCv);

                        if (stream == null)
                            return NotFound();

                        return File(stream, "application/octet-stream", objPessoa.NmCv);
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-DownloadCurriculo");


                    return NotFound();
                }

                return NotFound();
            }
        }

        [HttpDelete]
        [Route("ExcluiCurriculo/{id}")]
        public OutPutExcluiCurriculo ExcluiCurriculo(int id)
        {
            var retorno = new OutPutExcluiCurriculo();
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var objPessoa = new bPessoaFisica(db).BuscarPessoaId(id);

                            objPessoa.NmCv = null;
                            objPessoa.DsCv = null;

                            db.SaveChanges();

                            // Confirma operações
                            db.Database.CommitTransaction();
                            retorno.Result = true;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-ExcluiCurriculo");

                            throw;
                        }
                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("BuscaNomeArquivoCv/{id}")]
        public OutPutBuscaNomeArquivoCv BuscaNomeArquivoCv(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new OutPutBuscaNomeArquivoCv();
                    var item = new bPessoaFisica(db).BuscarPessoaId(id);
                    retorno.NmArquivo = item.NmCv;

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PessoaFisicaController-BuscaNomeArquivoCv");


                    throw;
                }

            }
        }
    }

    #region Retornos   

    public class InputUpdatePessoaFisica
    {
        public int IdPessoaFisica { get; set; }
        public string NmPessoa { get; set; }
        public string CdSexo { get; set; }
        public string CdEmail { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string NuCpf { get; set; }
        public string NuTelefoneFixo { get; set; }
        public string NuTelefoneComercial { get; set; }
        public string NuCelular { get; set; }
        public string NuCep { get; set; }
        public string DsEndereco { get; set; }
        public string NuEndereco { get; set; }
        public string DsComplemento { get; set; }
        public string NmBairro { get; set; }
        public int? IdCidade { get; set; }
        public string SgUf { get; set; }
        public string CdLinkedIn { get; set; }
        public string CdCvLattes { get; set; }
        public byte[] DsCv { get; set; }
        public int? IdTipoVinculo { get; set; }
        public int IdUsuarioAlteracao { get; set; }
    }

    public class InputAddPessoaFisica
    {
        public int IdPessoaFisica { get; set; }
        public string NmPessoa { get; set; }
        public string CdSexo { get; set; }
        public string CdEmail { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string NuCpf { get; set; }
        public string NuTelefoneFixo { get; set; }
        public string NuTelefoneComercial { get; set; }
        public string NuCelular { get; set; }
        public string NuCep { get; set; }
        public string DsEndereco { get; set; }
        public string NuEndereco { get; set; }
        public string DsComplemento { get; set; }
        public string NmBairro { get; set; }
        public int? IdCidade { get; set; }
        public string SgUf { get; set; }
        public string CdLinkedIn { get; set; }
        public string CdCvLattes { get; set; }
        //public string idTipoPessoa { get; set; }
        public int IdHstPessoaFisica { get; set; }
        public string NmUsuarioAlteracao { get; set; }
        public string DtAlteracao { get; set; }
        public int IdUsuario { get; set; }
        public string NmCv { get; set; }
        public int? IdTipoVinculo { get; set; }
        public DateTime? DtInicioVinculo { get; set; }
    }

    public class OutPutPessoaFisica
    {
        public bool Result { get; set; }
        public bool CpfCadastrado { get; set; }
        public int IdPessoaFisica { get; set; }
    }

    public class OutPutPessoaFisicaGrid
    {
        public int IdPessoaFisica { get; set; }
        public string NmPessoa { get; set; }
        public string IdCidade { get; set; }
        public string DsEndereco { get; set; }
        public string NuCpf { get; set; }
        public string NmCv { get; set; }
        public string CdCvLattes { get; set; }

    }

    public class OutPutBuscaNomeArquivoCv
    {
        public string NmArquivo { get; set; }
    }

    public class OutPutExcluiCurriculo
    {
        public bool Result { get; set; }
    }
    #endregion
}
