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
    public class UnidadeTempoController : ControllerBase
    {
        [HttpGet]
        [Route("ListaUnidadeTempo")]
        public List<OutPutListaUnidadeTempo> ListaUnidadeTempo()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoUnidadeTempo = new List<OutPutListaUnidadeTempo>();

                    var lstUnidadesTempo = new bUnidadeTempo(db).Get();

                    foreach (var ut in lstUnidadesTempo)
                    {
                        var unidadeTempo = new OutPutListaUnidadeTempo();

                        unidadeTempo.IdUnidadeTempo = ut.IdUnidadeTempo;
                        unidadeTempo.DsUnidadeTempo = ut.DsUnidadeTempo;

                        lstRetornoUnidadeTempo.Add(unidadeTempo);
                    }

                    return lstRetornoUnidadeTempo;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UnidadeTempoController-ListaUnidadeTempo");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("ListaUnidadeTempoJuridico")]
        public List<OutPutListaUnidadeTempoJuridico> ListaUnidadeTempoJuridico()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoUnidadeTempo = new List<OutPutListaUnidadeTempoJuridico>();

                    var lstUnidadesTempo = new bUnidadeTempo(db).Get();

                    foreach (var ut in lstUnidadesTempo)
                    {
                        var unidadeTempo = new OutPutListaUnidadeTempoJuridico();

                        unidadeTempo.IdUnidadeTempoJuridico = ut.IdUnidadeTempo;
                        unidadeTempo.DsUnidadeTempoJuridico = ut.DsUnidadeTempo;

                        lstRetornoUnidadeTempo.Add(unidadeTempo);
                    }

                    return lstRetornoUnidadeTempo;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "UnidadeTempoController-ListaUnidadeTempoJuridico");



                    throw;
                }
            }
        }

    }

    #region Retornos
    public class OutPutListaUnidadeTempo
    {
        public int IdUnidadeTempo { get; set; }
        public string DsUnidadeTempo { get; set; }        
    }

    public class OutPutListaUnidadeTempoJuridico
    {
        public int IdUnidadeTempoJuridico { get; set; }
        public string DsUnidadeTempoJuridico { get; set; }
    }

    #endregion
}