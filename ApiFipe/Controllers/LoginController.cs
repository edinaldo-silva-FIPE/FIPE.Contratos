using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApiFipe.Utilitario;


namespace ApiFipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        GravaLog _GLog = new GravaLog();

        [HttpPost]
        [Route("Auth")]
        public OutputAuth Auth([FromBody]InputAuth auth)
        {
            var retorno = new OutputAuth();
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    if (auth.ResetSenha == true)
                    {
                        var login = db.Usuario.Where(w => w.DsLogin == auth.DsLogin).FirstOrDefault();

                        if (login != null)
                        {
                            // envio de email recuperação de senha
                            var nomePessoa = db.PessoaFisica.Where(w => w.IdPessoaFisica == login.IdPessoa).FirstOrDefault().NmPessoa;
                            var texto = db.EmailConfigurado.Where(w => w.DsTitulo == "Recuperação de Senha").FirstOrDefault().DsTexto;
                            var corpoEmail = String.Format(texto, nomePessoa, auth.Url + login.NrToken);

                            byte[] anexo = null;
                            new bEmail(db).EnviarEmail(login.DsLogin, nomePessoa, "Recuperação de Senha", corpoEmail, anexo, "");

                            retorno.ResetSenha = true;
                        }
                        else
                        {
                            retorno.ResetSenha = false;
                        }
                        _GLog._GravaLog(login.IdUsuario, "Senha resetada de ["+ login.DsLogin + "]");
                        return retorno;
                    }
                    else
                    {
                        var usuario = new bUsuario(db).Auth(auth.DsLogin, Codificar(auth.CdSenha));

                        if (usuario != null)
                        {
                            var perfilUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == usuario.IdUsuario).FirstOrDefault();
                            var perfil        = db.Perfil.Where(w => w.IdPerfil == perfilUsuario.IdPerfil).OrderBy(x => x.IdPerfil).FirstOrDefault();
                            var nUsuario      = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario.IdPessoa).FirstOrDefault();

                            usuario.NrToken = Guid.NewGuid();

                            db.SaveChanges();

                            retorno.Result         = true;
                            retorno.PrimeiroAcesso = false;
                            retorno.IdUsuario      = usuario.IdUsuario;
                            retorno.IdPessoa       = usuario.IdPessoa;
                            retorno.DsLogin        = usuario.DsLogin;
                            retorno.PerfilUsuario  = "";
                            retorno.NomeUsuario    = nUsuario.NmPessoa;
                            retorno.NrToken        = usuario.NrToken.ToString();
                            _GLog._GravaLog(usuario.IdUsuario, "Usuario [" + usuario.IdUsuario + "-" + usuario.DsLogin + "] conectado no sistema");
                        }
                        else
                        {
                            retorno.Result = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "LoginController-Auth");


                    retorno.Result = false;
                }

                return retorno;
            }
        }


        [HttpPut]
        [Route("AtualizarSenha")]
        public OutPutUpDateUsuario AtualizarSenha([FromBody] InputUpDateUsuario item)
        {
            var retorno = new OutPutUpDateUsuario();
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
                            if (!String.IsNullOrEmpty(item.NrToken))
                            {
                                Guid codUsuario = Guid.Parse(item.NrToken);
                                var usuarios = db.Usuario.Where(w => w.NrToken == codUsuario).ToList();
                                if (usuarios.Count != 0)
                                {
                                    var usuario = new Usuario();

                                    usuario.CdSenha = Codificar(item.CdSenha);
                                    usuario.DsLogin = usuarios.FirstOrDefault().DsLogin;
                                    usuario.NrToken = Guid.NewGuid();

                                    var updateRetorno = new bUsuario(db).AtualizarSenha(usuario);

                                    // Confirma operações
                                    db.Database.CommitTransaction();

                                    retorno.Result = updateRetorno;

                                    return retorno;
                                }
                                else
                                {
                                    retorno.ErroAlterarSenha = true;

                                    return retorno;
                                }
                            }
                            else
                            {
                                var usuario = new Usuario();

                                usuario.CdSenha = Codificar(item.CdSenha);
                                usuario.DsLogin = item.DsLogin;

                                var updateRetorno = new bUsuario(db).AtualizarSenha(usuario);

                                // Confirma operações
                                db.Database.CommitTransaction();

                                retorno.Result = updateRetorno;

                                return retorno;
                            }
                        }

                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "LoginController-AtualizarSenha");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("VerificaAtualizacaoSenha/{guid}")]
        public bool VerificaAtualizacaoSenha(Guid guid)
        {
            var retorno = new OutPutUpDateUsuario();
            using (var db = new FIPEContratosContext())
            {

                try
                {
                    // Inicia transação                             
                    var usuarios = db.Usuario.Where(w => w.NrToken == guid).ToList();
                    if (usuarios.Count == 0)
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
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "LoginController-VerificaAtualizacaoSenha");

                    return false;
                }
            }
        }

        // Busca perfil de usuário por id usuário
        [HttpGet]
        [Route("GetPerfilByUsuario/{idUsuario}")]
        public List<OutputGetPerfil> GetPerfilByUsuario(int idUsuario)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var listaPerfis = new List<OutputGetPerfil>();
                    var perfis = new bUsuario(db).GetPerfilUsuario();

                    var perfisUsuario = db.PerfilUsuario.Where(w => w.IdUsuario == idUsuario).ToList();

                    foreach (var itemPerfil in perfisUsuario)
                    {
                        var perfil = new OutputGetPerfil();

                        perfil.IdPerfil = itemPerfil.IdPerfil;
                        perfil.DsPerfil = perfis.Where(w => w.IdPerfil == itemPerfil.IdPerfil).FirstOrDefault().DsPerfil;

                        listaPerfis.Add(perfil);
                    }

                    return listaPerfis.OrderBy(o=>o.DsPerfil).ToList();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "LoginController-GetPerfilByUsuario");


                    throw;
                }
            }
        }

        //EGS 30.05.2020
        [HttpGet]
        [Route("EnvironmentCheck")]
        public bool EnvironmentCheck()
        {
            return FIPEContratosContext.EnvironmentIsProduction;
        }


        //Criptografar Senha
        public static string Codificar(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }

    #region Retornos   

    public class InputAuth
    {
        public string DsLogin { get; set; }
        public string CdSenha { get; set; }
        public string Url { get; set; }
        public bool ResetSenha { get; set; }
    }

    public class OutputAuth
    {
        public bool Result { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool ResetSenha { get; set; }
        public int IdUsuario { get; set; }
        public int IdPessoa { get; set; }
        public string DsLogin { get; set; }
        public string PerfilUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public string NrToken { get; set; }

    }

    public class InputUpDateUsuario
    {
        public string DsLogin { get; set; }
        public string CdSenha { get; set; }
        public string NrToken { get; set; }
    }

    public class OutPutUpDateUsuario
    {
        public bool Result { get; set; }
        public bool ErroAlterarSenha { get; set; }

    }

    public class OutputGetPerfil
    {
        public int IdPerfil { get; set; }
        public string DsPerfil { get; set; }
    }

    #endregion

}
