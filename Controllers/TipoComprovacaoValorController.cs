using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoComprovacaoValorController : ControllerBase
    {

        [HttpGet]
        [Route("ListaComprovacaoValor")]
        public List<OutPutGetTipoComprovacaoValor> ListaComprovacaoValor()
        {
            var lstRetornoTipoComprovacaoValor = new List<OutPutGetTipoComprovacaoValor>();
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var xlstRetornoTipoComprovacaoValor = new bTipoComprovacaoValor(db).Get().Where(x=> x.IcAtivo == true);

                    foreach (var item in xlstRetornoTipoComprovacaoValor)
                    {
                        var tipoComproValor = new OutPutGetTipoComprovacaoValor();
                        tipoComproValor.idComprovacaoValor = item.IdComprovacaoValor;
                        tipoComproValor.dsComprovacaoValor = item.DsComprovacaoValor;
                        tipoComproValor.obrigatorio        = item.Obrigatorio;
                        lstRetornoTipoComprovacaoValor.Add(tipoComproValor);
                    }
                }
                catch (Exception ex)
                {
                    string bErro = ex.Message + " " + ex.InnerException;                    
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoComprovacaoValorController-ListaComprovacaoValor");
                    throw;
                }
            }
            return lstRetornoTipoComprovacaoValor;

        }







        #region Retornos
        public class OutPutGetTipoComprovacaoValor
        {
            public short  idComprovacaoValor    { get; set; }
            public string dsComprovacaoValor    { get; set; }
            public bool   obrigatorio           { get; set; }
        }

    }

    #endregion
}