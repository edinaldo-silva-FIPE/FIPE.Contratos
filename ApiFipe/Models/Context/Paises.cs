using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Paises
    {
        public string Iso { get; set; }
        public string Iso3 { get; set; }
        public int? IdPais { get; set; }
        public string Nome { get; set; }
    }
}
