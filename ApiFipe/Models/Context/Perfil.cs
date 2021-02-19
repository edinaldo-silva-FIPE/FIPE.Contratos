using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Perfil
    {
        public Perfil()
        {
            PerfilUsuario = new HashSet<PerfilUsuario>();
        }

        public int IdPerfil { get; set; }
        public string DsPerfil { get; set; }

        public virtual ICollection<PerfilUsuario> PerfilUsuario { get; set; }
    }
}
