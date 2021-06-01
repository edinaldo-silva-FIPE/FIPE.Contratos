using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string NmCliente { get; set; }
        public int? CodigoCliente { get; set; }
    }
}
