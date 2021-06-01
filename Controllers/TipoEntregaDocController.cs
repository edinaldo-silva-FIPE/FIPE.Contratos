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
    public class TipoEntregaDocController : ControllerBase
    {
        [HttpGet]
        [Route("ListaTipoEntregaDocs")]
        public List<OutPutGetTipoEntregaDocs> ListaTipoEntregaDocs(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoTipoEntregaDocs = new List<OutPutGetTipoEntregaDocs>();
                    var lstTipoEntregaDocs = new bTipoEntregaDoc(db).Get();

                    foreach (var tv in lstTipoEntregaDocs)
                    {
                        var tipoEntregaDocs = new OutPutGetTipoEntregaDocs();
                        tipoEntregaDocs.IdTipoEntregaDocumento = tv.IdTipoEntregaDocumento;
                        tipoEntregaDocs.DsTipoEntregaDocumento = tv.DsTipoEntregaDocumento;

                        lstRetornoTipoEntregaDocs.Add(tipoEntregaDocs);
                    }

                    return lstRetornoTipoEntregaDocs;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoEntregaDocController-ListaTipoEntregaDocs");




                    throw;
                }
            }
        }
      
    }

    #region Retornos
    public class OutPutGetTipoEntregaDocs
    {
        public int IdTipoEntregaDocumento { get; set; }
        public string DsTipoEntregaDocumento { get; set; }
    }    

    #endregion
}