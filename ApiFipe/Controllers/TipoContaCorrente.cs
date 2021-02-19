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
    public class TipoContaCorrenteController : ControllerBase
    {
        [HttpGet]
        [Route("ListaContasCorrente")]
        public List<OutputGetContaCorrente> ListaContasCorrente(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstContasCorrente = new List<OutputGetContaCorrente>();
                    var lstContas = new bContaCorrente(db).ListaContasCorrente();

                    foreach (var con in lstContas)
                    {
                        var conta = new OutputGetContaCorrente();
                        conta.IdContaCorrente = con.IdContaCorrente;
                        conta.NuContaEditado = con.NuContaEditado;

                        lstContasCorrente.Add(conta);
                    }

                    return lstContasCorrente;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoContaCorrenteController-ListaContasCorrente");
                    throw;
                }
            }
        }
    }

    #region Retornos
    public class OutputGetContaCorrente
    {
        public int IdContaCorrente { get; set; }
        public string NuContaEditado { get; set; }
    }

    #endregion
}