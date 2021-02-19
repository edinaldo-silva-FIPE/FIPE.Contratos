using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
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
    public class TipoAditivoController : ControllerBase
    {
        [HttpGet]
        [Route("ListaTipoAditivos")]
        public List<OutPutGetTipoAditivo> ListaTipoAditivos()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTipoAditivo = new List<OutPutGetTipoAditivo>();

                    var lstTipoAditivo = new bTipoAditivo(db).Get();

                    foreach (var tv in lstTipoAditivo)
                    {
                        var tipoAditivo = new OutPutGetTipoAditivo();

                        tipoAditivo.IdTipoAditivo = tv.IdTipoAditivo;
                        tipoAditivo.DsTipoAditivo = tv.DsTipoAditivo;
                        tipoAditivo.Value = tv.DsTipoAditivo;

                        lstRetornoTipoAditivo.Add(tipoAditivo);
                    }

                    return lstRetornoTipoAditivo;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoAditivoController-ListaTipoAditivos");




                    throw;
                }
            }
        }
       
    }

    #region Retornos
    public class OutPutGetTipoAditivo
    {
        public int IdTipoAditivo { get; set; }
        public string DsTipoAditivo { get; set; }
        public string Value { get; set; }
    }   

    #endregion
}