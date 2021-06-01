using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bUsuarioToken
    {
        public FIPEContratosContext db { get; set; }

        public bUsuarioToken(FIPEContratosContext db)
        {
            this.db = db;
        }       

        public bool ValidaUsuarioByToken(string token)
        {
            bool usuarioAutenticado = false;

            if (!string.IsNullOrEmpty(token))
            {
                var usuario = db.Usuario.Where(w => w.NrToken.ToString() == token).FirstOrDefault();

                if (usuario != null)
                {
                    usuarioAutenticado = true;
                }
            }

            return usuarioAutenticado;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Agosto/2020 
        *  Localiza o ID do usuario logado
        ===========================================================================================*/
        public int RetornaIDUsuarioByToken(string token)
        {
            int usuarioAutenticado = 0;

            if (!string.IsNullOrEmpty(token))
            {
                var usuario = db.Usuario.Where(w => w.NrToken.ToString() == token).FirstOrDefault();

                if (usuario != null)
                {
                    usuarioAutenticado = usuario.IdUsuario;
                }
            }
            return usuarioAutenticado;
        }

    }
}