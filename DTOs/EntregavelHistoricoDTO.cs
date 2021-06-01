using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.DTOs
{
    public class EntregavelHistoricoDTO
    {
        public long Id { get; set; }
        public int IdAditivo { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public DateTime? DataPrevista { get; set; }
        public FrenteDTO Frente { get; set; }
        public SituacaoDTO Situacao { get; set; }
        public ClienteDTO Cliente { get; set; }
    }
}
