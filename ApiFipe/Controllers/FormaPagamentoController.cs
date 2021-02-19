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
    public class FormaPagamentoController : ControllerBase
    {
        [HttpGet]
        [Route("ListaFormaPagamentos")]
        public List<OutPutGetFormaPagamentos> ListaFormaPagamentos(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoFormaPagamentos = new List<OutPutGetFormaPagamentos>();
                    var lstFormaPagamentos = new bFormaPagamento(db).Get();

                    foreach (var tv in lstFormaPagamentos)
                    {
                        var formaPagamentos = new OutPutGetFormaPagamentos();
                        formaPagamentos.IdFormaPagamento = tv.IdFormaPagamento;
                        formaPagamentos.DsFormaPagamento = tv.DsFormaPagamento;

                        lstRetornoFormaPagamentos.Add(formaPagamentos);
                    }

                    return lstRetornoFormaPagamentos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FormaPagamentoController-ListaFormaPagamentos");
	
                    throw;
                }
            }
        }
       
    }

    #region Retornos
    public class OutPutGetFormaPagamentos
    {
        public int IdFormaPagamento { get; set; }
        public string DsFormaPagamento { get; set; }
    }   

    #endregion
}