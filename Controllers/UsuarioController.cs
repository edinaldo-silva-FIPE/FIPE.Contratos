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

    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        GravaLog _GLog = new GravaLog();

        #region Métodos
        [HttpGet]
        [Route("Get")]
        public List<OutputGet> Get()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var outPutGetUsuarios = new List<OutputGet>();
                    var usuarios = new bUsuario(db).Get();

                    foreach (var itemUsuario in usuarios)
                    {
                        var outPutGetUsuario = new OutputGet();
                        var perfis = db.PerfilUsuario.Where(w => w.IdUsuario == itemUsuario.IdUsuario).ToList();
                        if (perfis.Count > 0)
                        {
                            foreach (var item in perfis)
                            {
                                var perfisConcat = db.Perfil.Where(w => w.IdPerfil == item.IdPerfil).FirstOrDefault().DsPerfil + ", ";
                                outPutGetUsuario.DsPerfil += perfisConcat;
                            }
                            outPutGetUsuario.DsPerfil = outPutGetUsuario.DsPerfil.Substring(0, outPutGetUsuario.DsPerfil.Length - 2);
                        }
                        else
                        {
                            outPutGetUsuario.DsPerfil = "";
                        }                        

                        outPutGetUsuario.IdUsuario = itemUsuario.IdUsuario;
                        outPutGetUsuario.DsLogin = itemUsuario.DsLogin;
                        outPutGetUsuario.NmPessoa = itemUsuario.IdPessoaNavigation.NmPessoa;                        

                        outPutGetUsuarios.Add(outPutGetUsuario);
                    }

                    return outPutGetUsuarios;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-Get");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetPerfilUsuario")]
        public List<OutputGetPerfil> GetPerfilUsuario()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var outPutGetPerfil = new List<OutputGetPerfil>();
                    var perfis = new bUsuario(db).GetPerfilUsuario();

                    foreach (var itemPerfil in perfis)
                    {
                        var perfil = new OutputGetPerfil();

                        perfil.IdPerfil = itemPerfil.IdPerfil;
                        perfil.DsPerfil = itemPerfil.DsPerfil;

                        outPutGetPerfil.Add(perfil);
                    }

                    return outPutGetPerfil;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-GetPerfilUsuario");


                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutPutGetId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var usuario = new OutPutGetId();
                    var usu = new bUsuario(db).GetById(id);

                    usuario.IdUsuario = usu.IdUsuario;
                    usuario.IdPessoa = usu.IdPessoa;
                    usuario.DsLogin = usu.DsLogin;
                    usuario.Perfis = new List<OutputGetPerfil>();

                    foreach (var item in db.PerfilUsuario.Where(w => w.IdUsuario == usu.IdUsuario).ToList())
                    {
                        var perfil = new OutputGetPerfil();
                        perfil.IdPerfil = item.IdPerfil;
                        perfil.DsPerfil = db.Perfil.Where(w => w.IdPerfil == item.IdPerfil).FirstOrDefault().DsPerfil;
                        usuario.Perfis.Add(perfil);
                    }
                                         
                    return usuario;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-GetById");

                    throw;
                }
            }
        }



        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Abril/2021 
        *  Verifica se o usuario tem o perfil para aprovar proposta
        ==============================================================================================*/
        [HttpGet]
        [Route("GetPermissaoProposta")]
        public bool GetPermissaoProposta(int id, int pIDProposta, string pTipoPermissao)
        {
            bool bRetorno = false;
            pTipoPermissao = pTipoPermissao.ToUpper();
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var usu = new bUsuario(db).GetById(id);
                    foreach (var item in db.PerfilUsuario.Where(w => w.IdUsuario == usu.IdUsuario).ToList())
                    {
                        if ((pTipoPermissao == "APROVAR_PROPOSTA") && (item.IdPerfil == 6))   /* Diretoria pra Aprovar Proposta */
                        {
                            _GLog._GravaLog(AppSettings.constGlobalUserID, "GetPermissaoProposta Usuario [" + id + "] Proposta [" + pIDProposta + "] e " + pTipoPermissao + " concedido");
                            bRetorno = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-GetPermissaoProposta");
                    throw;
                }
            }
            if (!bRetorno)
            {
                _GLog._GravaLog(AppSettings.constGlobalUserID, "GetPermissaoProposta Usuario [" + id + "] Proposta [" + pIDProposta + "] e " + pTipoPermissao + " sem permissão");
            }
            return bRetorno;
        }





        [HttpPost]
        [Route("AddUsuario")]
        public OutPutAddUsuario AddUsuario([FromBody] InputAddUsuario item)
        {
            var retorno = new OutPutAddUsuario();

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
                            int usuarioDuplicado = 0;
                            usuarioDuplicado = db.Usuario.Where(w => w.DsLogin.ToUpper() == item.DsLogin.ToUpper()).Count();
                            if (usuarioDuplicado > 0)
                            {
                                retorno.Result = false;
                                retorno.LoginDuplicado = false;
                                return retorno;
                            }
                            else
                            {
                                var usuario = new Usuario();
                                var perfisUsuario = new List<OutputGetPerfil>();

                                usuario.IdPessoa = item.IdPessoa;
                                usuario.CdSenha = Codificar(item.CdSenha);
                                usuario.DsLogin = item.DsLogin;
                                usuario.NrToken = Guid.NewGuid();

                                foreach (var itemPerfil in item.Perfis)
                                {
                                    perfisUsuario.Add(itemPerfil);
                                }                                

                                var addRetorno = new bUsuario(db).AddUsuario(usuario, perfisUsuario);

                                // Confirma operações
                                db.Database.CommitTransaction();

                                retorno.Result = addRetorno;
                                retorno.LoginDuplicado = true;

                                // envio de email criação de login
                                byte[] anexo = null;
                                var nomePessoa = db.PessoaFisica.Where(w => w.IdPessoaFisica == item.IdPessoa).FirstOrDefault().NmPessoa;
                                var texto = db.EmailConfigurado.Where(w => w.DsTitulo == "Criação de Login").FirstOrDefault().DsTexto;
                                var corpoEmail = String.Format(texto, nomePessoa, item.UrlLogin, item.UrlLogin, item.DsLogin, "123");

                                new bEmail(db).EnviarEmail(item.DsLogin, nomePessoa, "Senha de primeiro acesso ao Sistema", corpoEmail, anexo, "");

                                return retorno;
                            }
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-AddUsuario");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpDateUsuario")]
        public OutPutUpDateUsuario UpDateUsuario([FromBody] InputUpDateUsuario item)
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
                            var usuario = new Usuario();
                            var perfisUsuario = new List<OutputGetPerfil>();

                            usuario.IdUsuario = item.IdUsuario;
                            usuario.IdPessoa = item.IdPessoa;
                            usuario.CdSenha = Codificar(item.CdSenha);
                            usuario.DsLogin = item.DsLogin;

                            foreach (var itemPerfil in item.Perfis)
                            {
                                perfisUsuario.Add(itemPerfil);
                            }
                            
                            var updateRetorno = new bUsuario(db).UpdateUsuario(usuario, perfisUsuario);

                            // Confirma operações
                            db.Database.CommitTransaction();
                            retorno.LoginDuplicado = true;
                            retorno.Result = true;
                            return retorno;
                        }

                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "UsuarioController-UpDateUsuario");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }
        #endregion

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

        #region Retornos
        public class OutputGet
        {
            public int IdUsuario { get; set; }
            public string DsLogin { get; set; }
            public string NmPessoa { get; set; }
            public string DsPerfil { get; set; }
        }

        public class OutputGetPerfil
        {
            public int IdPerfil { get; set; }
            public string DsPerfil { get; set; }
        }

        public class OutPutGetId
        {
            public int IdUsuario { get; set; }
            public string DsLogin { get; set; }
            public int IdPessoa { get; set; }
            public List<OutputGetPerfil> Perfis { get; set; }
        }

        public class InputAddUsuario
        {
            public string DsLogin { get; set; }
            public int IdPessoa { get; set; }
            public List<OutputGetPerfil> Perfis { get; set; }
            public string CdSenha { get; set; }
            public string UrlLogin { get; set; }
        }

        public class InputUpDateUsuario
        {
            public int IdUsuario { get; set; }
            public string DsLogin { get; set; }
            public int IdPessoa { get; set; }
            public List<OutputGetPerfil> Perfis { get; set; }
            public string CdSenha { get; set; }

        }

        public class OutPutAddUsuario
        {
            public bool Result { get; set; }
            public bool LoginDuplicado { get; set; }
            public int IdUsuario { get; set; }
        }

        public class OutPutUpDateUsuario
        {
            public bool Result { get; set; }
            public bool LoginDuplicado { get; set; }
            public int IdUsuario { get; set; }
        }
        public class OutGetUsuarioLogin
        {
            public int IdUsuario { get; set; }
        }

        #endregion
    }
}