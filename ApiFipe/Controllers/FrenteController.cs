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
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class FrenteController : ControllerBase
    {
        [HttpPost]
        [Route("AddFrente")]
        public OutPutAddFrente AddFrente([FromBody] InputAddFrente item)
        {
            var retorno = new OutPutAddFrente();
            var pessoaFisica = new PessoaFisica();

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
                            

                            var addRetorno = new bFrente(db).AddFrente(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = addRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FrenteController-AddFrente");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpPut]
        [Route("UpdateFrente")]
        public OutPutUpdateFrente UpdateFrente([FromBody] InputUpdateFrente item)
        {
            var retorno = new OutPutUpdateFrente();
            var pessoaFisica = new PessoaFisica();

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
                            

                            var updateRetorno = new bFrente(db).UpdateFrente(item);

                            // Confirma operações
                            db.Database.CommitTransaction();

                            retorno = updateRetorno;

                            return retorno;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FrenteController-UpdateFrente");
                            
                            retorno.Result = false;
                        }

                        return retorno;
                    }
                });
                return retorno;
            }
        }

        [HttpGet]
        [Route("ListaFrenteIdContrato/{id}")]
        public List<OutPutGridGetFrente> ListaFrenteIdContrato(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                var listaFrentes = new List<OutPutGridGetFrente>();
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {

                        try
                        {
                            
                            var frentes = new bFrente(db).BuscaFrenteIdContrato(id);

                            if (frentes.Count > 0)
                            {
                                foreach (var f in frentes)
                                {
                                    var frente = new OutPutGridGetFrente();

                                    frente.IdFrente = f.IdFrente;
                                    frente.NmFrente = f.NmFrente;
                                    frente.CdFrenteTexto = f.CdFrenteTexto;
                                    frente.CdFrente = f.CdFrente;

                                    var lstFrenteCoord = new bFrenteCoordenador(db).BuscaFrenteCoordenadorIdFrente(f.IdFrente);
                                    if (lstFrenteCoord.Count > 0)
                                    {
                                        frente.coordenadores = new List<OutPutGetFrentePessoaFisica>();
                                        foreach (var frenteCoordenador in lstFrenteCoord)
                                        {
                                            var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(frenteCoordenador.IdPessoaFisica);
                                            var pessoaFisicaRetorno = new OutPutGetFrentePessoaFisica();
                                            pessoaFisicaRetorno.IdPessoa = pessoaFisica.IdPessoaFisica;
                                            pessoaFisicaRetorno.NmPessoa = pessoaFisica.NmPessoa;

                                            frente.coordenadoresTexto += " " + pessoaFisica.NmPessoa;
                                            frente.coordenadores.Add(pessoaFisicaRetorno);
                                        }
                                    }

                                    listaFrentes.Add(frente);
                                }
                            }
                            else
                            {
                                var contrato = new bContrato(db).GetContratoById(id);

                                if (contrato.IcFrenteUnica != null)
                                {
                                    if (contrato.IcFrenteUnica.Value)
                                    {

                                        var frente = new OutPutGridGetFrente();

                                        var f = new Frente();

                                        f.NmFrente = "Frente 01";
                                        f.IdContrato = id;
                                        f.CdFrente = 1;
                                        f.CdFrenteTexto = "01";

                                        db.Frente.Add(f);
                                        db.SaveChanges();

                                        var coordenadoresContrato = new bContratoCoordenador(db).BuscarCoordenador(id);
                                        foreach (var coordContrato in coordenadoresContrato)
                                        {
                                            var inputAddFrenteCoordenador = new InputAddFrenteCoordenador();
                                            inputAddFrenteCoordenador.IdFrente = f.IdFrente;
                                            inputAddFrenteCoordenador.IdPessoa = coordContrato.IdPessoa;

                                            new bFrenteCoordenador(db).AddFrenteCoordenador(inputAddFrenteCoordenador);
                                        }
                                        db.Database.CommitTransaction();

                                        frente.IdFrente = f.IdFrente;
                                        frente.NmFrente = f.NmFrente;
                                        frente.CdFrenteTexto = f.CdFrenteTexto;
                                        frente.CdFrente = f.CdFrente;

                                        var lstFrenteCoord = new bFrenteCoordenador(db).BuscaFrenteCoordenadorIdFrente(f.IdFrente);
                                        if (lstFrenteCoord.Count > 0)
                                        {
                                            frente.coordenadores = new List<OutPutGetFrentePessoaFisica>();
                                            foreach (var frenteCoordenador in lstFrenteCoord)
                                            {
                                                var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(frenteCoordenador.IdPessoaFisica);
                                                var pessoaFisicaRetorno = new OutPutGetFrentePessoaFisica();
                                                pessoaFisicaRetorno.IdPessoa = pessoaFisica.IdPessoaFisica;
                                                pessoaFisicaRetorno.NmPessoa = pessoaFisica.NmPessoa;

                                                frente.coordenadoresTexto += " " + pessoaFisica.NmPessoa;
                                                frente.coordenadores.Add(pessoaFisicaRetorno);
                                            }
                                        }

                                        listaFrentes.Add(frente);
                                    }
                                }
                            }

                            return listaFrentes;
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FrenteController-ListaFrenteIdContrato");

                            throw;
                        }
                    }
                });
                return listaFrentes;
            }
        }

        [HttpGet]
        [Route("BuscaFrenteId/{id}")]
        public OutPutGetFrente BuscaFrenteId(int id)
        {
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    var retornoFrente = new OutPutGetFrente();
                    var frente = new bFrente(db).BuscaFrenteId(id);

                    if (frente != null)
                    {
                        retornoFrente.CdFrente = frente.CdFrente;
                        retornoFrente.CdFrenteTexto = frente.CdFrenteTexto;
                        retornoFrente.NmFrente = frente.NmFrente;
                        retornoFrente.coordenadores = new List<OutPutGetFrentePessoaFisica>();

                        List<FrenteCoordenador> lstFrenteCoordenador = new bFrenteCoordenador(db).BuscaFrenteCoordenadorIdFrente(frente.IdFrente);
                        foreach (var frenteCoord in lstFrenteCoordenador)
                        {
                            var retornoCoord = new OutPutGetFrentePessoaFisica();
                            retornoCoord.IdPessoa = frenteCoord.IdPessoaFisica;

                            var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(retornoCoord.IdPessoa);
                            retornoCoord.NmPessoa = pessoaFisica.NmPessoa;

                            retornoFrente.coordenadores.Add(retornoCoord);
                        }
                    }

                    return retornoFrente;
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "FrenteController-BuscaFrenteId");
		
                    throw;
                }
            }
        }

        [HttpDelete]
        [Route("RemoveFrenteId/{id}")]
        public OutPutRemoveFrente RemoveFrenteId(int id)
        {
            var retorno = new OutPutRemoveFrente();

            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            retorno = new bFrente(db).RemoveFrenteId(id);

                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            new bEmail(db).EnviarEmailTratamentoErro(ex, "FrenteController-RemoveFrenteId");


                            throw;
                        }
                    }

                    return retorno;
                });
                return retorno;
            }
        }
    }

    #region Retornos
    public class InputAddFrente
    {
        public string NmFrente { get; set; }
        public int IdContrato { get; set; }
        public string CdFrenteTexto { get; set; }
        public List<InputAddFrentePessoaFisica> Coordenadores { get; set; }
    }
    public class InputAddFrentePessoaFisica
    {
        public int IdPessoa { get; set; }
        public string NmPessoa { get; set; }
    }

    public class InputUpdateFrente
    {
        public int IdFrente { get; set; }
        public string NmFrente { get; set; }
        public string CdFrenteTexto { get; set; }
        public List<InputAddFrentePessoaFisica> Coordenadores { get; set; }
    }
    public class InputUpdateFrentePessoaFisica
    {
        public int IdPessoa { get; set; }
        public string NmPessoa { get; set; }
    }
    public class OutPutAddFrente
    {
        public bool Result { get; set; }
    }

    public class OutPutUpdateFrente
    {
        public bool Result { get; set; }
    }

    public class OutPutRemoveFrente
    {
        public bool Result { get; set; }
    }
}
public class OutPutGetFrente
{
    public string NmFrente { get; set; }
    public int CdFrente { get; set; }
    public string CdFrenteTexto { get; set; }
    public List<OutPutGetFrentePessoaFisica> coordenadores { get; set; }
}

public class OutPutGridGetFrente
{
    public int IdFrente { get; set; }
    public string NmFrente { get; set; }
    public int CdFrente { get; set; }
    public string CdFrenteTexto { get; set; }
    public List<OutPutGetFrentePessoaFisica> coordenadores { get; set; }
    public string coordenadoresTexto { get; set; }
}

public class OutPutGetFrentePessoaFisica
{
    public int IdPessoa { get; set; }
    public string NmPessoa { get; set; }
}
#endregion
