using System;

namespace ApiFipe.DTOs
{
    public class InputEntregavel
    {
        public int Quantidade { get; set; }
        public DateTime DataPrevista { get; set; }
        public string Nome { get; set; }
        public int IdContrato { get; set; }
    }

    public class EntregavelDTO
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public DateTime? DataPrevista { get; set; }
        //public int Frente { get; set; }
        public FrenteDTO Frente { get; set; }
        public SituacaoDTO Situacao { get; set; }
        //public string NomeFrente { get; set; }
        public ClienteDTO Cliente { get; set; }
        public bool? IcAtraso { get; set; }
    }

    public class EntregavelResumoDTO
    {
        public int Id { get; set; }
        public int Numero { get; set; }
    }
}
