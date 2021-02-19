using ApiFipe.DTOs;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        [Route("ObterClientes/{idContrato}")]
        public List<ClienteDTO> ObterClientes(int idContrato)
        {
            var clientes = new List<ClienteDTO>();
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    clientes = new bCliente(db).ObterClientes(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ClienteController-ObterClientes");                  
                    throw ex;
                }

                return clientes;
            }
        }

        [HttpGet]
        [Route("ListaContratoClientes/{idContrato}")]
        public List<OutPutGetCliente> ListaContratoClientes(int idContrato)
        {
            var clientes = new List<OutPutGetCliente>();

            using (var db = new FIPEContratosContext())
            {
                try
                {
                    clientes = new bCliente(db).ListaContratoClientes(idContrato);
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ClienteController-ListaContratoClientes");	
                    throw ex;
                }

                return clientes;
            }
        }
    }
}
