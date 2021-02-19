using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Controllers.UsuarioController;
using ApiFipe.Utilitario;



namespace ApiFipe.Models
{
    public class bUsuario
    {
        private FIPEContratosContext db { get; set; }
        GravaLog _GLog = new GravaLog();

        public bUsuario(FIPEContratosContext db)
        {
            this.db = db;
        }

        public Usuario Auth(string login, string senha)
        {
            var senhaMD5 = Util.GetMd5Hash(senha);

            var item = db.Usuario
                .Where(w => w.DsLogin == login && w.CdSenha == senha)
                .FirstOrDefault();

            return item;
        }         

        public Usuario GetById(int id)
        {
            var usuario = db.Usuario
                .Include(i => i.PerfilUsuario)
                .Where(w => w.IdUsuario == id)
                .Single();

            //EGS 30.09.2020 Nao achou constGlobalUserID, estava dando erro
            if (usuario == null)
            {
                usuario = db.Usuario.Where(w => w.IdUsuario == id).Single();
                _GLog._GravaLog(AppSettings.constGlobalUserID, "UsuarioGetById [ID: " + id + "] Normal Usu.Cria [" + id + "]  Usu.New.Achado [" + usuario.IdUsuario + "] e constGlobalUserID [" + AppSettings.constGlobalUserID + "]");
            }
            return usuario;
        }

        // metodos do menu configuraçoes usuarios
        public List<Usuario> Get()
        {
            var usuarios = db.Usuario
                .Include(i=>i.IdPessoaNavigation)
                .ToList();
            return usuarios;
        }

        public List<Perfil> GetPerfilUsuario()
        {
            var perfis = db.Perfil.ToList();

            return perfis;
        }

        public bool AddUsuario(Usuario item , List<OutputGetPerfil> perUsu)
        {
            try
            {               
                db.Usuario.Add(item);          
                                
                db.SaveChanges();
                var idCli = item.IdUsuario;

                foreach (var itemPerfil in perUsu)
                {
                    var perfilUsuario = new PerfilUsuario();
                    perfilUsuario.IdPerfil = itemPerfil.IdPerfil;
                    perfilUsuario.IdUsuario = idCli;

                    db.PerfilUsuario.Add(perfilUsuario);
                    db.SaveChanges();
                }                                

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateUsuario(Usuario item, List<OutputGetPerfil> perfis)
        {
            try
            {
                var perfisUsuario = db.PerfilUsuario.Where(w=>w.IdUsuario == item.IdUsuario).ToList();
                if (perfisUsuario.Count > 0)
                {
                    db.PerfilUsuario.RemoveRange(perfisUsuario);
                    db.SaveChanges();                    
                }
                foreach (var itemPerfis in perfis)
                {
                    var perfil = new PerfilUsuario();
                    perfil.IdPerfil = itemPerfis.IdPerfil;
                    perfil.IdUsuario = item.IdUsuario;

                    db.PerfilUsuario.Add(perfil);
                    db.SaveChanges();
                }
                var idCli = item.IdUsuario;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }        

        public List<PessoaFisica> GetGestoresProposta()
        {
            var lstGestores = new List<PessoaFisica>();
            var lstPerfilUsuario = db.PerfilUsuario.Where(w => w.IdPerfil == 2).ToList();

            foreach (var perfilUsuario in lstPerfilUsuario)
            {
                var usuario = db.Usuario.Where(w => w.IdUsuario == perfilUsuario.IdUsuario).FirstOrDefault();

                if (usuario != null)
                {
                    var pessoaFisica = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario.IdPessoa).FirstOrDefault();

                    if (pessoaFisica != null)
                    {
                        lstGestores.Add(pessoaFisica);
                    }
                }
            }
            return lstGestores;
        }

        public bool AtualizarSenha(Usuario item)
        {
            try
            {
                var usuario = db.Usuario.Where(w => w.DsLogin == item.DsLogin).FirstOrDefault();
                usuario.CdSenha = item.CdSenha;
                usuario.NrToken = item.NrToken;

                db.SaveChanges();

                var idCli = item.IdUsuario;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
