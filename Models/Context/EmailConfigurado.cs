using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class EmailConfigurado
    {
        public int IdEmail { get; set; }
        public string DsTitulo { get; set; }
        public string DsTexto { get; set; }
        public string DsPerfil { get; set; }
    }
}
