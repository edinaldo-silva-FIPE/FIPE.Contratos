using ApiFipe.DTOs;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static ApiFipe.Models.bAditivo;

namespace ApiFipe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AditivoController : ControllerBase
    {
        #region | Métodos

        [HttpGet]
        [Route("ObterTiposAditivos")]
        public List<TipoAditivoDTO> ObterTiposAditivos()
        {
            List<TipoAditivoDTO> tipos;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    tipos = new bAditivo(db).ObterTipos();
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ObterTiposAditivos");

                    throw ex;
                }

                return tipos;
            }
        }

        [HttpGet]
        [Route("ConsultarAditivo/{idAditivo}")]
        public AditivoDTO ConsultarAditivo(int idAditivo)
        {
            AditivoDTO aditivo;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    aditivo = new bAditivo(db).Consultar(idAditivo);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ConsultarAditivo");

                    throw ex;
                }

                return aditivo;
            }
        }

        [HttpPost]
        [Route("SalvarAditivo")]
        public bool SalvarAditivo([FromBody] AditivoDTO aditivo)
        {
            bool operacaoBemSucedida = true;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    new bAditivo(db).Atualizar(aditivo);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "SalvarAditivo");

                    operacaoBemSucedida = false;
                }

                return operacaoBemSucedida;
            }
        }

        [HttpPost]
        [Route("AplicarAditivo")]
        public bool AplicarAditivo([FromBody] InputAplicarAditivo inputAplicarAditivo)
        {
            bool operacaoBemSucedida = true;

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    new bAditivo(db).AplicarAditivo(inputAplicarAditivo);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "AplicarAditivo");

                    operacaoBemSucedida = false;
                }

                return operacaoBemSucedida;
            }
        }

        [ServiceFilter(typeof(AutenticacaoActionFilter))]
        [HttpGet]
        [Route("DownloadTermoAditivo/{id}")]
        public async Task<IActionResult> DownloadTermoAditivo(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoDocumento = new bContrato(db).BuscarDocumentoPrincipalId(id);

                    if (retornoDocumento != null && retornoDocumento.DocFisico != null)
                    {
                        var stream = new MemoryStream(retornoDocumento.DocFisico);

                        if (stream == null)
                            return NotFound();

                        return File(stream, "application/octet-stream", retornoDocumento.DsDoc);
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "DownloadTermoAditivo");
                    return NotFound();
                }

                return NotFound();
            }
        }

        #endregion
    }
}