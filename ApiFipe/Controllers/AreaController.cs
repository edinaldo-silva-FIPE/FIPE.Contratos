using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        [HttpGet]
        [Route("ListaAreas")]
        public List<OutputGetArea> ListaAreas(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoAreas = new List<OutputGetArea>();
                    var lstAreas = new bArea(db).ListaAreas();

                    foreach (var a in lstAreas)
                    {
                        var area = new OutputGetArea();
                        area.IdArea = a.IdArea;
                        area.DsArea = a.DsArea;

                        lstRetornoAreas.Add(area);
                    }

                    return lstRetornoAreas;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ListaAreas");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutputGetAreaId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var area = new OutputGetAreaId();
                    var a = new bArea(db).BuscaAreaId(id);
                    area.IdArea = a.IdArea;
                    area.DsArea = a.DsArea;

                    return area;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "AreaController-GetById");

                    throw;
                }
            }
        }

    }

    #region Retornos
    public class OutputGetArea
    {
        public int IdArea { get; set; }
        public string DsArea { get; set; }
    }

    public class OutputGetAreaId
    {
        public int IdArea { get; set; }
        public string DsArea { get; set; }
    }


    #endregion
}