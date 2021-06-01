using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PerfilUsuario
    {
        public int IdPerfilUsuario { get; set; }
        public int IdPerfil { get; set; }
        public int IdUsuario { get; set; }
        public bool EnviaEmail { get; set; }                //EGS 30.06.2020 Se envia email das propostas

        public virtual Perfil IdPerfilNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
