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
    public class ContratoClienteController : ControllerBase
    {
        [HttpGet]
        [Route("ListaContratoCliente/{id}")]
        public List<OutPutGetContratoCliente> ListaContratoCliente(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var lstRetornoContratoClientes = new List<OutPutGetContratoCliente>();
                    var lstContratoClientes = new bContratoCliente(db).GetByIdContrato(id);

                    foreach (var tv in lstContratoClientes)
                    {
                        var contratoCliente = new OutPutGetContratoCliente();
                        contratoCliente.IdContratoCliente = tv.IdContratoCliente;
                        contratoCliente.IdContrato = tv.IdContrato;
                        contratoCliente.IdCliente = tv.IdCliente;
                        if (String.IsNullOrEmpty(tv.NmFantasia))
                        {
                            var idPessoa = db.Cliente.Where(b => b.IdCliente == tv.IdCliente).FirstOrDefault().IdPessoa;
                            var idPessoaFisica = db.Pessoa.Where(a => a.IdPessoa == idPessoa).FirstOrDefault().IdPessoaFisica;
                            var nmCliente = db.PessoaFisica.Where(w => w.IdPessoaFisica == idPessoaFisica).FirstOrDefault().NmPessoa;
                            contratoCliente.NmFantasia = nmCliente;
                        }
                        else
                        {
                            contratoCliente.NmFantasia = tv.NmFantasia;
                        }
                        contratoCliente.IcPagador = tv.IcPagador;

                        lstRetornoContratoClientes.Add(contratoCliente);
                    }

                    return lstRetornoContratoClientes;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoClienteController-ListaContratoCliente");                
                    throw;
                }
            }
        }
    }

    #region Retornos
    public class OutPutGetContratoCliente
    {
        public int IdContratoCliente { get; set; }
        public int IdCliente { get; set; }
        public int IdContrato { get; set; }
        public string NmFantasia { get; set; }
        public bool? IcPagador { get; set; }
    }

    #endregion
}