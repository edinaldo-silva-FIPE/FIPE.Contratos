using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ContratoEquipeTecnicaController : ControllerBase
    {
        [HttpPost]
        [Route("Add")]
        public OutPutAddContratoEquipeTecnica Add([FromBody] InputAddContratoEquipeTecnica item)
        {
            var retorno = new OutPutAddContratoEquipeTecnica();

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
                            

                            var addRetorno = new bContratoEquipeTecnica(db).Add(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-Add");


                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("Update")]
        public OutPutUpdateContratoEquipeTecnica Update([FromBody] InputUpdateContratoEquipeTecnica item)
        {
            var retorno = new OutPutUpdateContratoEquipeTecnica();

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
                            

                            var updateRetorno = new bContratoEquipeTecnica(db).Update(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-Update");                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaContratoEquipeTecnicaIdContrato/{id}")]
        public OutPutGridGetContratoEquipeTecnica ListaContratoEquipeTecnicaIdContrato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno                    = new OutPutGridGetContratoEquipeTecnica();
                    var listaContratoEquipeTecnica = new List<OutPutGridGetContratoEquipeTecnicaDados>();
                    var contratosEquipeTecnica     = new bContratoEquipeTecnica(db).BuscaContratoEquipeTecnicaIdContrato(id);
                    retorno.ValorSomas             = new OutPutGridGetContratoEquipeTecnicaSomas();
                    retorno.ListEquipeTecnica      = new List<OutPutGridGetContratoEquipeTecnicaDados>();

                    if (contratosEquipeTecnica.Count > 0)
                    {
                        retorno.ValorSomas.ValorTaxaSoma       = 0;
                        retorno.ValorSomas.VlCustoProjetoSoma  = 0;
                        retorno.ValorSomas.VlTotalAReceberSoma = 0;
                        foreach (var contratoEquipeTecnica in contratosEquipeTecnica)
                        {
                            var contratoEquipeTecnicaGrid = new OutPutGridGetContratoEquipeTecnicaDados();
                            var pessoa = new bPessoaFisica(db).GetById(contratoEquipeTecnica.IdPessoaFisica);
                            var formacaoProfissional = db.FormacaoProfissional.Where(w => w.IdFormacaoProfissional == contratoEquipeTecnica.IdFormacaoProfissional).FirstOrDefault();
                            var taxaUsp = db.TaxaInstitucional.Where(w => w.IdTaxaInstitucional == contratoEquipeTecnica.IdTaxaInstitucional).FirstOrDefault();
                            contratoEquipeTecnicaGrid.IdContratoEquipeTecnica = contratoEquipeTecnica.IdContratoEquipeTecnica;
                            contratoEquipeTecnicaGrid.CdEmail = pessoa.CdEmail;
                            contratoEquipeTecnicaGrid.NmPessoa = pessoa.NmPessoa;
                            //EGS 30.08.2020 Estava dando erro, dai verifica se a vriavel existe antes de usar..
                            if (formacaoProfissional != null) { 
                                contratoEquipeTecnicaGrid.DsFormacaoProfissional = formacaoProfissional.DsFormacaoProfissional;
                            }
                            contratoEquipeTecnicaGrid.DsAtividadeDesempenhada = contratoEquipeTecnica.DsAtividadeDesempenhada;
                            if (contratoEquipeTecnica.IdPessoaJuridica != null)
                            {
                                var pessoaJuridica = new bPessoaJuridica(db).BuscarPessoaJuridicaId(contratoEquipeTecnica.IdPessoaJuridica.Value);
                                contratoEquipeTecnicaGrid.DsRazaoSocial = pessoaJuridica.RazaoSocial;
                            }
                            contratoEquipeTecnicaGrid.VlTotalAReceber = contratoEquipeTecnica.VlTotalAreceber;
                            contratoEquipeTecnicaGrid.DsTaxaInstitucional = taxaUsp.DsTaxaInstitucional;
                            var valorTotal = contratoEquipeTecnica.VlTotalAreceber;
                            var percentual = taxaUsp.PcTaxaInstitucional / 100;
                            var valorTaxaInstitucional = percentual * valorTotal;
                            contratoEquipeTecnicaGrid.ValorTaxa = valorTaxaInstitucional;
                            contratoEquipeTecnicaGrid.VlCustoProjeto = valorTaxaInstitucional + valorTotal;

                            // Somas do Grid                            
                            retorno.ValorSomas.ValorTaxaSoma       += valorTaxaInstitucional;
                            retorno.ValorSomas.VlTotalAReceberSoma += contratoEquipeTecnica.VlTotalAreceber;
                            retorno.ValorSomas.VlCustoProjetoSoma  += contratoEquipeTecnicaGrid.VlCustoProjeto;

                            listaContratoEquipeTecnica.Add(contratoEquipeTecnicaGrid);
                        }
                        retorno.ListEquipeTecnica = listaContratoEquipeTecnica;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-ListaContratoEquipeTecnicaIdContrato");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaContratoEquipeTecnicaId/{id}")]
        public OutPutGetContratoEquipeTecnica BuscaContratoEquipeTecnicaId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoContratoEquipeTecnica = new OutPutGetContratoEquipeTecnica();
                    var contratoEquipeTecnica = new bContratoEquipeTecnica(db).BuscaContratoEquipeTecnicaId(id);

                    if (contratoEquipeTecnica != null)
                    {
                        //retornoContratoEquipeTecnica. = frente.CdFrente;
                    }

                    return retornoContratoEquipeTecnica;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-BuscaContratoEquipeTecnicaId");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaTotaisEquipeTecnica/{id}")]
        public InputTotaisEquipeTecnica BuscaTotaisEquipeTecnica(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retorno = new InputTotaisEquipeTecnica();
                    var contrato = db.Contrato.Where(w => w.IdContrato == id).FirstOrDefault();

                    if (contrato != null)
                    {
                        retorno.VlCustoProjeto = contrato.VlCustoProjeto;
                        retorno.VlDiferenca    = contrato.VlDiferenca;
                        retorno.VlOverHead     = contrato.VlOverhead;
                        retorno.VlProjeto      = contrato.VlContrato;
                        retorno.VlOutrosCustos = contrato.VlOutrosCustos;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-BuscaTotaisEquipeTecnica");
                    throw;
                }
            }
        }

        [HttpGet]
        [Route("BuscaOverhead")]
        public decimal BuscaOverhead()
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    decimal retorno = 0;
                    var parametro = db.Parametro.Where(w => w.IdParametro == 1).FirstOrDefault();

                    if (parametro != null)
                    {
                        retorno = parametro.NuPercentualOverhead.Value;
                    }

                    return retorno;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-BuscaOverhead");
                    throw;
                }
            }
        }

        [HttpDelete]
        [Route("RemoveContratoEquipeTecnicaId/{id}")]
        public OutPutRemoveContratoEquipeTecnica RemoveContratoEquipeTecnicaId(int id)
        {
            var retorno = new OutPutRemoveContratoEquipeTecnica();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            retorno = new bContratoEquipeTecnica(db).RemoveContratoEquipeTecnicaId(id);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-RemoveContratoEquipeTecnicaId");                                                                                                          
                            throw;
                        }
                    }

                    return retorno;
                });
                return retorno;
            }
        }

        [HttpPost]
        [Route("SalvarTotaisEquipeTecnica")]
        public bool SalvarTotaisEquipeTecnica([FromBody] InputTotaisEquipeTecnica item)
        {
            var retorno = false;

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
                            var contrato = db.Contrato.Where(w => w.IdContrato == item.IdContrato).FirstOrDefault();
                            contrato.VlCustoProjeto = item.VlCustoProjeto;
                            contrato.VlOutrosCustos = item.VlOutrosCustos;
                            contrato.VlOverhead = item.VlOverHead;
                            contrato.VlDiferenca = item.VlDiferenca;

                            db.SaveChanges();
                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = true;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "ContratoEquipeTecnicaController-SalvarTotaisEquipeTecnica");
                            retorno = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }        
    }

    #region Retornos
    public class InputTotaisEquipeTecnica
    {
        public int IdContrato { get; set; }
        public decimal? VlOutrosCustos { get; set; }
        public decimal? VlOverHead { get; set; }
        public decimal? VlCustoProjeto { get; set; }
        public decimal? VlDiferenca { get; set; }
        public decimal? VlProjeto { get; set; }
    }

    public class InputAddContratoEquipeTecnica
    {
        public int IdContrato { get; set; }
        public int IdPessoaFisica { get; set; }
        public int? IdPessoaJuridica { get; set; }
        public int? IdFormacaoProfissional { get; set; }
        public int IdTaxaInstitucional { get; set; }
        public int? IdTipoContratacao { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public decimal? VlTotalAReceber { get; set; }
        public decimal? VlCustoProjeto { get; set; }
    }

    public class InputUpdateContratoEquipeTecnica
    {
        public int IdContratoEquipeTecnica { get; set; }
        public int IdContrato { get; set; }
        public int IdPessoaFisica { get; set; }
        public int? IdPessoaJuridica { get; set; }
        public int? IdFormacaoProfissional { get; set; }
        public int IdTaxaInstitucional { get; set; }
        public int? IdTipoContratacao { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public decimal? VlTotalAReceber { get; set; }
        public decimal? VlCustoProjeto { get; set; }
    }
    public class OutPutAddContratoEquipeTecnica
    {
        public bool Result { get; set; }
    }

    public class OutPutUpdateContratoEquipeTecnica
    {
        public bool Result { get; set; }
    }

    public class OutPutRemoveContratoEquipeTecnica
    {
        public bool Result { get; set; }
    }

    public class OutPutGetContratoEquipeTecnica
    {
        public int IdContratoEquipeTecnica { get; set; }
        public int IdContrato { get; set; }
        public int IdPessoaFisica { get; set; }
        public int? IdPessoaJuridica { get; set; }
        public int? IdFormacaoProfissional { get; set; }
        public int IdTaxaInstitucional { get; set; }
        public int? IdTipoContratacao { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public decimal? VlTotalAReceber { get; set; }
        public decimal? VlCustoProjeto { get; set; }
    }

    public class OutPutGridGetContratoEquipeTecnica
    {
        public List<OutPutGridGetContratoEquipeTecnicaDados> ListEquipeTecnica { get; set; }
        public OutPutGridGetContratoEquipeTecnicaSomas ValorSomas { get; set; }
    }


    public class OutPutGridGetContratoEquipeTecnicaDados
    {
        public int IdContratoEquipeTecnica { get; set; }
        public string NmPessoa { get; set; }
        public string CdEmail { get; set; }
        public string DsFormacaoProfissional { get; set; }
        public string DsAtividadeDesempenhada { get; set; }
        public string DsRazaoSocial { get; set; }
        public decimal? VlTotalAReceber { get; set; }
        public string DsTaxaInstitucional { get; set; }
        public decimal? ValorTaxa { get; set; }
        public decimal? VlCustoProjeto { get; set; }
    }

    public class OutPutGridGetContratoEquipeTecnicaSomas
    {
        public decimal? VlTotalAReceberSoma { get; set; }
        public decimal? ValorTaxaSoma { get; set; }
        public decimal? VlCustoProjetoSoma { get; set; }
    }
    #endregion

}
