using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.DTOs
{
    public class SituacaoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IcEntregue { get; set; }
        public bool IcNFEmitida { get; set; }
    }
}
