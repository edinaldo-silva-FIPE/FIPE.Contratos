using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFipe.Controllers
{

    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FundamentoController : ControllerBase
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
                    var fundamentos = new bFundamento(db).Get().Select(s => new OutputGet()
                    {
                        IdFundamento = s.IdFundamento,
                        DsFundamento = s.DsFundamento
                    }).OrderBy(o => o.DsFundamento).ToList();

                    return fundamentos;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FundamentoController-Get");

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
                    var fundamento = new OutputGetId();
                    var fund = new bFundamento(db).GetById(id);
                    fundamento.IdFundamento = fund.IdFundamento;
                    fundamento.DsFundamento = fund.DsFundamento;

                    return fundamento;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FundamentoController-GetById");

                    throw;
                }
            }
        }

        [HttpPost]
        [Route("AddFundamento")]
        public OutPutAddFundamento Add([FromBody] InputAddFundamento item)
        {
            var retorno = new OutPutAddFundamento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            // Inicia transação
                            

                            var fundamento = new FundamentoContratacao();
                            fundamento.DsFundamento = item.DsFundamento;

                            var addRetorno = new bFundamento(db).AddFundamento(fundamento);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FundamentoController-Add");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateFundamento")]
        public OutPutUpDateFundamento Update([FromBody] InputUpDateFundamento item)
        {
            var retorno = new OutPutUpDateFundamento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            // Inicia transação
                            

                            var fund = new FundamentoContratacao();
                            fund.IdFundamento = item.IdFundamento;
                            fund.DsFundamento = item.DsFundamento;

                            var updateRetorno = new bFundamento(db).UpdateFundamento(fund);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno.Result = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FundamentoController-Update");

                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpDelete]
        [Route("RemoveFundamento/{id}")]
        public OutPutRemoveFundamento Remove(int id)
        {
            var retorno = new OutPutRemoveFundamento();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            // Inicia transação
                            

                            retorno.Result = new bFundamento(db).RemoveFundamentoId(id);

                            // Confirma operações
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FundamentoController-Remove");
                            
                            throw;
                        }
                    }

                    return retorno;
                });
                return retorno;
            }
        }

        #endregion

        #region Retornos
        public class OutputGet
        {
            public int IdFundamento { get; set; }
            public string DsFundamento { get; set; }

        }

        public class OutputGetId
        {
            public int IdFundamento { get; set; }
            public string DsFundamento { get; set; }

        }

        public class InputAddFundamento
        {
            public string DsFundamento { get; set; }

        }

        public class InputUpDateFundamento
        {
            public short IdFundamento { get; set; }
            public string DsFundamento { get; set; }

        }

        public class OutPutAddFundamento
        {
            public bool Result { get; set; }
            public int IdFundamento { get; set; }
        }

        public class OutPutUpDateFundamento
        {
            public bool Result { get; set; }
            public int IdFundamento { get; set; }
        }

        public class OutPutRemoveFundamento
        {
            public bool Result { get; set; }
            public int IdFundamento { get; set; }
        }
        #endregion
    }
}