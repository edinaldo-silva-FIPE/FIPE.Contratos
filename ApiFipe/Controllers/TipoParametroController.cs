using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Controllers
{

    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class TipoParametroController : ControllerBase
    {
        #region Métodos
        [HttpGet]
        [Route("Get")]
        public List<OutputGet> Get()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var contatos = new bTipoParametro(db).Get().Select(s => new OutputGet()
                    {
                        IdParametro = s.IdParametro,
                        DsPrazoPagto = s.DsPrazoPagto,
                        NuBanco = s.NuBanco,
                        NuAgencia = s.NuAgencia,
                        NuConta = s.NuConta,
                        DsTextoCorpoNF = s.DsTextoCorpoNf,
                        NuDiasEntregaveis = s.NuDiasEntregaveis,
                        NuDiasFaturamento = s.NuDiasFaturamento,
                        NuDiasReajuste = s.NuDiasReajuste,
                        NuDiasEncerramentoContrato = s.NuDiasEncerramentoContrato,
                        NuDiasRenovacao = s.NuDiasRenovacao,
                        EmailsNotificacao = s.EmailsNotificacao.Replace(";", " ")

                    }).OrderBy(o => o.DsPrazoPagto).ToList();

                    return contatos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoParametroController-Get");



                    throw;
                }
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public OutputGetId GetById(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var parametro = new OutputGetId();
                    var par = new bTipoParametro(db).GetById(id);
                    parametro.IdParametro = par.IdParametro;
                    parametro.DsPrazoPagto = par.DsPrazoPagto;
                    parametro.NuBanco = par.NuBanco;
                    parametro.NuAgencia = par.NuAgencia;
                    parametro.NuConta = par.NuConta;
                    parametro.DsTextoCorpoNF = par.DsTextoCorpoNf;
                    parametro.NuDiasEntregaveis = par.NuDiasEntregaveis == null ? null : par.NuDiasEntregaveis;
                    parametro.NuDiasFaturamento = par.NuDiasFaturamento == null ? null : par.NuDiasFaturamento;
                    parametro.NuDiasReajuste = par.NuDiasReajuste == null ? null : par.NuDiasReajuste; 
                    parametro.NuDiasEncerramentoContrato = par.NuDiasEncerramentoContrato == null ? null : par.NuDiasEncerramentoContrato;
                    parametro.NuDiasRenovacao = par.NuDiasRenovacao;
                    parametro.EmailsNotificacao = par.EmailsNotificacao;
                  
                    return parametro;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoParametroController-GetById");

                    throw;
                }

            }
        }       

        [HttpPut]
        [Route("UpdateTipoParametro")]
        public OutPutUpDateTipoParametro Update([FromBody] InputUpDateTipoParametro item)
        {
            var retorno = new OutPutUpDateTipoParametro();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var parametro = db.Parametro.Where(w=>w.IdParametro == item.IdParametro).FirstOrDefault();
                            parametro.IdParametro = item.IdParametro;
                            parametro.DsPrazoPagto = item.DsPrazoPagto;
                            parametro.NuBanco = item.NuBanco;
                            parametro.NuAgencia = item.NuAgencia;
                            parametro.NuConta = item.NuConta;
                            parametro.DsTextoCorpoNf = item.DsTextoCorpoNF;
                            parametro.NuDiasEntregaveis = item.NuDiasEntregaveis;
                            parametro.NuDiasFaturamento = item.NuDiasFaturamento;
                            parametro.NuDiasReajuste = item.NuDiasReajuste;
                            parametro.NuDiasEncerramentoContrato = item.NuDiasEncerramentoContrato;
                            parametro.NuDiasRenovacao = item.NuDiasRenovacao;
                            string emailsSemEspaco = item.EmailsNotificacao.Replace(" ", "");
                            parametro.EmailsNotificacao = emailsSemEspaco;

                            var updateRetorno = new bTipoParametro(db).UpdateTipoParametro(parametro);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;                            
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "TipoParametroController-Update");

                            retorno.Result = false;
                        }
                        return retorno;
                    }
                });                        

                return retorno;
            }
        }      

        #endregion

        #region Retornos
        public class OutputGet
        {
            public int IdParametro { get; set; }
            public string DsPrazoPagto { get; set; }
            public string NuBanco { get; set; }
            public string NuAgencia { get; set; }
            public string NuConta { get; set; }
            public string DsTextoCorpoNF { get; set; }
            public short? NuDiasEntregaveis { get; set; }
            public short? NuDiasFaturamento { get; set; }
            public int? NuDiasReajuste { get; set; }
            public int? NuDiasEncerramentoContrato { get; set; }
            public int? NuDiasRenovacao { get; set; }
            public string EmailsNotificacao { get; set; }
        }

        public class OutputGetId
        {
            public int IdParametro { get; set; }
            public string DsPrazoPagto { get; set; }
            public string NuBanco { get; set; }
            public string NuAgencia { get; set; }
            public string NuConta { get; set; }
            public string DsTextoCorpoNF { get; set; }
            public short? NuDiasEntregaveis { get; set; }
            public short? NuDiasFaturamento { get; set; }
            public int? NuDiasReajuste { get; set; }
            public int? NuDiasEncerramentoContrato { get; set; }
            public int? NuDiasRenovacao { get; set; }
            public string EmailsNotificacao { get; set; }
        }

        public class InputUpDateTipoParametro
        {
            public int IdParametro { get; set; }
            public string DsPrazoPagto { get; set; }
            public string NuBanco { get; set; }
            public string NuAgencia { get; set; }
            public string NuConta { get; set; }
            public string DsTextoCorpoNF { get; set; }
            public short? NuDiasEntregaveis { get; set; }
            public short? NuDiasFaturamento { get; set; }
            public int? NuDiasReajuste { get; set; }
            public int? NuDiasEncerramentoContrato { get; set; }
            public int? NuDiasRenovacao { get; set; }
            public string EmailsNotificacao { get; set; }
        }       

        public class OutPutUpDateTipoParametro
        {
            public bool Result { get; set; }
            public int IdParametro { get; set; }
        }

        #endregion
    }
}