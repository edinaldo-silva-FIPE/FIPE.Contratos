using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoComprovacaoValor
    {
        public TipoComprovacaoValor()
        {
            //lstComprovacaoValor = new HashSet<TipoComprovacaoValor>();
        }

        public short  IdComprovacaoValor { get; set; }
        public string DsComprovacaoValor { get; set; }
        public bool   Obrigatorio        { get; set; }
        public bool   IcAtivo            { get; set; }



        //public virtual ICollection<TipoComprovacaoValor> lstComprovacaoValor { get; set; }
    }
}
