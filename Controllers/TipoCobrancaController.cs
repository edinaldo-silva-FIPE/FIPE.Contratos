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
    public class TipoCobrancaController : ControllerBase
    {
        [HttpGet]
        [Route("ListaTipoCobrancas")]
        public List<OutPutGetTipoCobranca> ListaTipoCobrancas(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTipoCobranca = new List<OutPutGetTipoCobranca>();
                    var lstTipoCobranca = new bTipoCobranca(db).Get();

                    foreach (var tv in lstTipoCobranca)
                    {
                        var tipoCobranca = new OutPutGetTipoCobranca();
                        tipoCobranca.IdTipoCobranca = tv.IdTipoCobranca;
                        tipoCobranca.DsTipoCobranca = tv.DsTipoCobranca;

                        lstRetornoTipoCobranca.Add(tipoCobranca);
                    }

                    return lstRetornoTipoCobranca;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoCobrancaController-ListaTipoCobrancas");




                    throw;
                }
            }
        }       
    }

    #region Retornos
    public class OutPutGetTipoCobranca
    {
        public int IdTipoCobranca { get; set; }
        public string DsTipoCobranca { get; set; }
    }   

    #endregion
}