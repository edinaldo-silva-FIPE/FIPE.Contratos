using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ApiFipe.Controllers
{
    [ServiceFilter(typeof(AutenticacaoActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class PesquisaGeralController : ControllerBase
    {
        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os registros de Oportunidade, Proposta e Contrato com os filtros informados
        ===========================================================================================*/
        [HttpPost]
        [Route("PesquisaGeral")]
        public OutputGetPesquisaGeral PesquisaGeral(InputPesqGeral pFiltro)
        {
            using (var db = new FIPEContratosContext())
            {
                var retPesquisaGeral = new OutputGetPesquisaGeral();
                try
                {
                    var dbOportunidade                 = new bPesquisaGeral(db).GetOportunidades(pFiltro);
                    var dbProposta                     = new bPesquisaGeral(db).GetPropostas(pFiltro);
                    var dbContrato                     = new bPesquisaGeral(db).GetContratos(pFiltro);
                    retPesquisaGeral.lstOportunidades  = dbOportunidade;
                    retPesquisaGeral.lstPropostas      = dbProposta;
                    retPesquisaGeral.lstContratos      = dbContrato;
                    retPesquisaGeral.rVlTotProposta    = string.Format("{0:C2}", _PegaValorTotalPropostas(dbProposta));
                    retPesquisaGeral.rVlTotContrato    = string.Format("{0:C2}", _PegaValorTotalContratos(dbContrato));
                    retPesquisaGeral.result            = true;
                    retPesquisaGeral.rVlTotProposta    = retPesquisaGeral.rVlTotProposta.Replace("R$ ", "").Replace("R$", "");
                    retPesquisaGeral.rVlTotContrato    = retPesquisaGeral.rVlTotContrato.Replace("R$ ", "").Replace("R$", "");
                    return retPesquisaGeral;
                }
                catch (Exception ex)
                {
                    retPesquisaGeral.result = false;
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-PesquisaGeral");
                    throw;
                }
            }
        }





















        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna Valor Total das Propostas filtradas
        ===========================================================================================*/
        private decimal _PegaValorTotalPropostas(List<OutPutPesqGeralListaPropostas> pTabela)
        {
            decimal pRetorno = 0;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    foreach (var item in pTabela)
                    {
                        pRetorno = pRetorno + item.nValor;
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_PegaValorTotalPropostas");
                    throw;
                }
            }
            return pRetorno;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna Valor Total dos Contratos filtrados
        ===========================================================================================*/
        private decimal _PegaValorTotalContratos(List<OutPutPesqGeralListaContratos> pTabela)
        {
            decimal pRetorno = 0;
            using (var db = new FIPEContratosContext())
            {
                try
                {
                    foreach (var item in pTabela)
                    {
                        pRetorno = pRetorno + item.nValor;
                    }
                }
                catch (Exception ex)
                {
                    new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_PegaValorTotalContratos");
                    throw;
                }
            }
            return pRetorno;
        }

        //EGS 30.06.2020 - Lista de Envio dos filtros informado
        public class InputPesqGeral
        {
            public string    edtNumOportunidade      { get; set; }
            public string    edtNumProposta          { get; set; }
            public string    edtNumContrato          { get; set; }
            public string    edtNumContraClie        { get; set; }
            public string    dsContratante           { get; set; }
            public string    dsRespCoordCliente      { get; set; }
            public string    dsTitulo                { get; set; }
            public string    dsNomeDocto             { get; set; }
            public string    dsObjeto                { get; set; }
            public int?      idTema                  { get; set; }
            public int?      idEsfera                { get; set; }
            public int?      situacao                { get; set; }
            public int?      idSituacaoProposta      { get; set; }
            public int?      idTipoOportunidade      { get; set; }
            public DateTime? dtLimiteEntrega         { get; set; }
            public DateTime? dtVingencia             { get; set; }
            public DateTime? dtExecucao              { get; set; }
            public DateTime? dtValidade              { get; set; }
            public string    dsNrContrato            { get; set; }
            public string    dsNrProcesso            { get; set; }
            public string    dsCentroCusto           { get; set; }
            public string    dsCidade                { get; set; }
            public string    dsEstado                { get; set; }
            public string    dsValorMin              { get; set; }
            public string    dsValorMax              { get; set; }
        }
        public class OutputGetPesquisaGeral
        {
            public bool                                    result               { get; set; }
            public string                                  rVlTotProposta       { get; set; }
            public string                                  rVlTotContrato       { get; set; }
            public List<OutPutPesqGeralListaOportunidades> lstOportunidades     { get; set; }
            public List<OutPutPesqGeralListaPropostas>     lstPropostas         { get; set; }
            public List<OutPutPesqGeralListaContratos>     lstContratos         { get; set; }
    }
        //EGS 30.06.2020 - Lista de Retorno com as Oportunidades encontradas com o filtro informado
        public class OutPutPesqGeralListaOportunidades
        {
            public int    idOportunidade        { get; set; }
            public string idProposta            { get; set; }
            public string nmContratantes        { get; set; }
            public string dsTitulo              { get; set; }
            public string nmResponsaveis        { get; set; }
            public string dsTipoOportunidade    { get; set; }
            public string dsSituacao            { get; set; }
            public string dtCriacao             { get; set; }
        }
        //EGS 30.06.2020 - Lista de Retorno com as Propostas encontradas com o filtro informado
        public class OutPutPesqGeralListaPropostas
        {
            public int     idProposta           { get; set; }
            public int     idOportunidade       { get; set; }
            public string  nmContratantes       { get; set; }
            public string  nmCoordenadores      { get; set; }
            public string  dtProposta           { get; set; }
            public string  dsApelido            { get; set; }
            public string  dsTitulo             { get; set; }
            public string  dsObjeto             { get; set; }
            public string  dsObservacao         { get; set; }
            public string  dtCriacao            { get; set; }
            public string  dtValidade           { get; set; }
            public string  dsTema               { get; set; }
            public decimal nValor               { get; set; }
            public string  dsValor              { get; set; }
            public string  dsSituacao           { get; set; }
            public string  dsCidade             { get; set; }
            public string  dsEstado             { get; set; }
        }
        //EGS 30.06.2020 - Lista de Retorno com os Contratos encontrados com o filtro informado
        public class OutPutPesqGeralListaContratos
        {
            public int     idContrato           { get; set; }
            public int     idProposta           { get; set; }
            public string  dsNumEdit            { get; set; }
            public string  dsTitulo             { get; set; }
            public string  dsObjeto             { get; set; }
            public string  nmClientes           { get; set; }
            public string  nmContratantes       { get; set; }
            public string  dsPrazoExecucao      { get; set; }
            public string  dtAssinatura         { get; set; }
            public decimal nValor               { get; set; }
            public string  dsValor              { get; set; }
            public string  dsProcessoCliente    { get; set; }
            public string  dsCentroCusto        { get; set; }
            public string  dsSituacao           { get; set; }
        }

    }
}