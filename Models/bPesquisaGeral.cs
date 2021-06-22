using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using static ApiFipe.Controllers.PesquisaGeralController;
using ApiFipe.Controllers;
using static ApiFipe.Controllers.PropostaController;
using static ApiFipe.Controllers.EntidadeController;
using static ApiFipe.Controllers.OportunidadeController;
using OutputGet = ApiFipe.Controllers.OportunidadeController.OutputGet;

namespace ApiFipe.Models
{
    public class bPesquisaGeral
    {
        public FIPEContratosContext db { get; set; }

        public bPesquisaGeral(FIPEContratosContext db)
        {
            this.db = db;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os registros de Oportunidade com os filtros informados
        ===========================================================================================*/
        public List<OutPutPesqGeralListaOportunidades> GetOportunidades(InputPesqGeral pFiltro)
        {
            var allOportunidades = new List<OutPutPesqGeralListaOportunidades>();
            try
            {
                string sSQL = " SELECT DISTINCT OPORT.IdOportunidade, OPORT.IdSituacao, Substring(SITUA.DsSituacao,0,80) DsSituacao, " +
                              "        OPORT.IdTipoOportunidade, Substring(OTIPO.DsTipoOportunidade,0,50) DsTipoOportunidade, " +
                              "        Substring(OPORT.DsAssunto,1,80) DsAssunto, Substring(OPORT.DsObservacao,0,80) DsObservacao, " +
                              "        OPORT.DtLimiteEntregaProposta, " +
                              "        OPORT.DtCriacao, OPORT.IdUsuarioCriacao, OPORT.IdUsuarioUltimaAlteracao " +
                              " FROM   Oportunidade OPORT " +
                              "        LEFT JOIN OportunidadeCliente     OCLIE ON (OCLIE.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeContato     OCONT ON (OCONT.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeDocs        ODOCS ON (ODOCS.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeResponsavel ORESP ON (ORESP.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN PessoaFisica            PESSO ON (PESSO.IdPessoaFisica     = ORESP.IdPessoaFisica    ) " +
                              "        LEFT JOIN TipoOportunidade        OTIPO ON (OTIPO.IdTipoOportunidade = OPORT.IdTipoOportunidade) " +
                              "        LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = OPORT.IdSituacao        ) " +
                              " WHERE (OPORT.IdOportunidade <> 0) ";

                if (!string.IsNullOrEmpty(pFiltro.edtNumOportunidade))
                {
                    sSQL = sSQL + " AND (OPORT.IdOportunidade  LIKE '%" + pFiltro.edtNumOportunidade.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                else
                {
                    if ((!string.IsNullOrEmpty(pFiltro.edtNumProposta)) || (!string.IsNullOrEmpty(pFiltro.edtNumContrato)) || (!string.IsNullOrEmpty(pFiltro.edtNumContraClie)) || (pFiltro.idSituacaoProposta.ToString() != ""))
                    {
                        sSQL = sSQL + " AND (OPORT.IdOportunidade = 0) ";
                    }
                }
                if (!string.IsNullOrEmpty(pFiltro.dsContratante))
                {
                    sSQL = sSQL + " AND (Upper(OCLIE.NmFantasia)  LIKE '%" + pFiltro.dsContratante.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsRespCoordCliente))
                {
                    sSQL = sSQL + " AND (Upper(PESSO.NmPessoa  )  LIKE '%" + pFiltro.dsRespCoordCliente.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsTitulo))
                {
                    sSQL = sSQL + " AND (Upper(OPORT.DsAssunto)  LIKE '%" + pFiltro.dsTitulo.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsNomeDocto))
                {
                    sSQL = sSQL + " AND (Upper(ODOCS.NmDocumento)  LIKE '%" + pFiltro.dsNomeDocto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (pFiltro.situacao.ToString() != "")
                {
                    sSQL = sSQL + " AND (OPORT.IdSituacao = " + pFiltro.situacao.ToString() + ") ";
                }
                if (pFiltro.idTipoOportunidade.ToString() != "")
                {
                    sSQL = sSQL + " AND (OPORT.IdTipoOportunidade = " + pFiltro.idTipoOportunidade.ToString() + ") ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsObjeto))
                {
                    sSQL = sSQL + " AND (Upper(OPORT.DsObservacao)  LIKE '%" + pFiltro.dsObjeto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if ((!string.IsNullOrEmpty(pFiltro.dsCentroCusto)) || (!string.IsNullOrEmpty(pFiltro.dsNrProcesso)) || (!string.IsNullOrEmpty(pFiltro.dsValorMin)) || (!string.IsNullOrEmpty(pFiltro.dsValorMax)) || (pFiltro.idTema > 0) || (pFiltro.idEsfera > 0))
                {
                    sSQL = sSQL + " AND (OPORT.IdOportunidade = 9999999) ";
                }


                //====================================================================== FINAL dos FILTROS ========================================================
                //====================================================================== FINAL dos FILTROS ========================================================
                sSQL = sSQL + " ORDER BY OPORT.IdOportunidade DESC ";

                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutPutPesqGeralListaOportunidades itemOportunidade = new OutPutPesqGeralListaOportunidades();
                            itemOportunidade.idOportunidade = Convert.ToInt32(reader["idOportunidade"]);
                            itemOportunidade.idProposta = _GetOportunidadeNumProposta(itemOportunidade.idOportunidade);
                            itemOportunidade.nmContratantes = _GetOportunidadeNomeContratantes(itemOportunidade.idOportunidade);
                            itemOportunidade.nmResponsaveis = _GetOportunidadeNomeResponsaveis(itemOportunidade.idOportunidade);
                            itemOportunidade.dtCriacao = Convert.ToString(reader["DtCriacao"]);
                            itemOportunidade.dsTitulo = Convert.ToString(reader["DsAssunto"]);
                            itemOportunidade.dsTipoOportunidade = Convert.ToString(reader["DsTipoOportunidade"]);
                            itemOportunidade.dsSituacao = Convert.ToString(reader["DsSituacao"]);
                            if (itemOportunidade.dtCriacao.Length > 10) { itemOportunidade.dtCriacao = itemOportunidade.dtCriacao.Substring(0, 10); }
                            //--------------------------------------------------------------------------------------------------
                            allOportunidades.Add(itemOportunidade);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetOportunidades");
                throw;
            }
            return allOportunidades;
        }


        public List<OutputGet> GetOportunidades()
        {
            var allOportunidades = new List<OutputGet>();
            try
            {
                string sSQL = " SELECT DISTINCT OPORT.IdOportunidade, OPORT.IdSituacao, Substring(SITUA.DsSituacao,0,80) DsSituacao," +
                              "        OPORT.IdTipoOportunidade, Substring(OTIPO.DsTipoOportunidade,0,50) DsTipoOportunidade, " +
                              "        OPORT.DsAssunto DsAssunto, Substring(OPORT.DsObservacao,0,80) DsObservacao, " +
                              "        OPORT.DtLimiteEntregaProposta, " +
                              "        OPORT.DtCriacao, OPORT.IdUsuarioCriacao, OPORT.IdUsuarioUltimaAlteracao, " +
                              " (select top 1 NmFantasia from OportunidadeCliente where IdOportunidade = OPORT.IdOportunidade) NmFantasia, " +
                              "(select top 1  NmPessoa from OportunidadeResponsavel orr  join PessoaFisica pf on(pf.IdPessoaFisica = orr.IdPessoaFisica) where IdOportunidade = OPORT.IdOportunidade) as contratante, " +
                              "(select top 1 IdProposta from proposta where IdOportunidade = OPORT.IdOportunidade) as IdProposta," +
                              "(SELECT NmFantasia + '+'  AS[text()] FROM OportunidadeCliente ST1 WHERE ST1.IdOportunidade = OCLIE.IdOportunidade ORDER BY ST1.NmFantasia FOR XML PATH(''))  AS PessoaJuridica," +
                              "(SELECT  ST1.NmPessoa + '+' AS[text()] FROM(SELECT distinct  PROPO1.IdOportunidade, PESSO1.NmPessoa   from Oportunidade PROPO1  JOIN OportunidadeCliente PCLIE1  ON(PCLIE1.IdOportunidade = PROPO1.IdOportunidade) LEFT JOIN OportunidadeResponsavel PCOOR1    ON(PCOOR1.IdOportunidade = PROPO1.IdOportunidade) LEFT JOIN PessoaFisica PESSO1 ON(PESSO1.IdPessoaFisica = PCOOR1.IdPessoaFisica)) ST1 where st1.IdOportunidade = OPORT.IdOportunidade   ORDER BY ST1.NmPessoa  FOR XML PATH(''))  AS PessoaFisica," +
                              "(SELECT  top 1 ppjd.UF + '  '  AS[text()] FROM OportunidadeCliente ST1 join Cliente ORESP1 on(ORESP1.IdCliente = st1.IdCliente) join Pessoa o on(o.IdPessoa = ORESP1.IdPessoa) join PessoaJuridica ppjd on(ppjd.IdPessoaJuridica = o.IdPessoaJuridica) WHERE ST1.IdOportunidade = OCLIE.IdOportunidade ORDER BY ST1.RazaoSocial FOR XML PATH(''))  AS uf," +
                              "(SELECT top 1 ccd.NmCidade + '  '  AS[text()] FROM OportunidadeCliente ST1 join Cliente ORESP1 on(ORESP1.IdCliente = st1.IdCliente) join Pessoa o on(o.IdPessoa = ORESP1.IdPessoa) join PessoaJuridica ppjd on(ppjd.IdPessoaJuridica = o.IdPessoaJuridica) join Cidade ccd on(ccd.IdCidade = ppjd.IdCidade) WHERE ST1.IdOportunidade = OCLIE.IdOportunidade ORDER BY ST1.RazaoSocial FOR XML PATH(''))  AS cidade," +
                              "(SELECT st1.NmPessoa + '  ' AS[text()] FROM(SELECT distinct  PROPO1.IdOportunidade, ppf.NmPessoa   from Oportunidade PROPO1  JOIN OportunidadeCliente PCLIE1  ON(PCLIE1.IdOportunidade = PROPO1.IdOportunidade) LEFT JOIN Cliente cc1 on cc1.IdCliente =PCLIE1.IdCliente join pessoa ppp on (ppp.IdPessoa = cc1.IdPessoa) join PessoaFisica ppf on (ppf.IdPessoaFisica = ppp.IdPessoaFisica)) ST1 where st1.IdOportunidade = OPORT.IdOportunidade   ORDER BY ST1.NmPessoa  FOR XML PATH(''))  AS PessoaFisicaContratante" +
                              " FROM   Oportunidade OPORT " +
                              "        LEFT JOIN OportunidadeCliente     OCLIE ON (OCLIE.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeContato     OCONT ON (OCONT.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeDocs        ODOCS ON (ODOCS.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN OportunidadeResponsavel ORESP ON (ORESP.IdOportunidade     = OPORT.IdOportunidade    ) " +
                              "        LEFT JOIN PessoaFisica            PESSO ON (PESSO.IdPessoaFisica     = ORESP.IdPessoaFisica    ) " +
                              "        LEFT JOIN TipoOportunidade        OTIPO ON (OTIPO.IdTipoOportunidade = OPORT.IdTipoOportunidade) " +
                              "        LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = OPORT.IdSituacao        ) " +
                              " WHERE (OPORT.IdOportunidade <> 0) ";
                sSQL = sSQL + " ORDER BY OPORT.IdOportunidade DESC ";

                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutputGet itemOportunidade = new OutputGet();
                            itemOportunidade.IdOportunidade = Convert.ToInt32(reader["idOportunidade"]);
                            if (!String.IsNullOrEmpty(reader["IdProposta"].ToString()))
                                itemOportunidade.IdProposta = Convert.ToInt32(reader["IdProposta"]);

                            if (!string.IsNullOrWhiteSpace(Convert.ToString(reader["NmFantasia"])))
                            {
                                itemOportunidade.DsClienteTexto = Convert.ToString(reader["NmFantasia"]);
                                itemOportunidade.DsCliente = new List<string>();
                                itemOportunidade.DsCliente.Add(" ");
                                itemOportunidade.DsCliente.AddRange(Convert.ToString(reader["PessoaJuridica"]).Split('+').ToList());
                            }
                            else
                            {
                                itemOportunidade.DsClienteTexto = Convert.ToString(reader["PessoaFisicaContratante"]);
                                itemOportunidade.DsCliente = new List<string>();
                                itemOportunidade.DsCliente.Add(Convert.ToString(reader["PessoaFisicaContratante"]));
                            }

                            itemOportunidade.ResponsavelTexto = reader["contratante"].ToString();
                            itemOportunidade.Responsavel = new List<string>();
                            itemOportunidade.Responsavel.Add(" ");
                            itemOportunidade.Responsavel.AddRange(Convert.ToString(reader["PessoaFisica"]).Split('+').ToList());
                            itemOportunidade.DtCriacao        = Convert.ToDateTime(reader["DtCriacao"]);
                            itemOportunidade.DsAssunto        = Convert.ToString(reader["DsAssunto"]);
                            itemOportunidade.TipoOportunidade = Convert.ToString(reader["DsTipoOportunidade"]);
                            itemOportunidade.Status           = Convert.ToString(reader["DsSituacao"]);
                            itemOportunidade.Uf               = Convert.ToString(reader["uf"]).Trim().TrimEnd();
                            itemOportunidade.NmCidade         = Convert.ToString(reader["cidade"]);
                            if (!String.IsNullOrEmpty(reader["DtLimiteEntregaProposta"].ToString()))
                                itemOportunidade.DtLimiteEntregaProposta = Convert.ToDateTime(reader["DtLimiteEntregaProposta"]);



                            //--------------------------------------------------------------------------------------------------
                            allOportunidades.Add(itemOportunidade);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetOportunidades");
                throw;
            }
            return allOportunidades;
        }




        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os registros de Propostas com os filtros informados
        ===========================================================================================*/
        public List<OutPutPesqGeralListaPropostas> GetPropostas(InputPesqGeral pFiltro)
        {
            var allPropostas = new List<OutPutPesqGeralListaPropostas>();
            try
            {
                string sSQL = " SELECT DISTINCT PROPO.IdProposta, PROPO.IdOportunidade, PROPO.IdSituacao, SITUA.DsSituacao, " +
                              "        PROPO.IdTema, TEMAS.DsTema, Substring(PROPO.DsApelidoProposta, 0, 80) DsApelidoProposta, " +
                              "        Substring(PROPO.DsAssunto, 0, 80) DsAssunto, Substring(PROPO.DsObjeto, 0, 80) DsObjeto, " +
                              " 	   PROPO.IdTipoOportunidade, Substring(OTIPO.DsTipoOportunidade,0,80) DsTipoOportunidade, PROPO.NuContratoCliente, " +
                              "        PROPO.DtProposta, PROPO.DtValidadeProposta, PROPO.DtLimiteEnvioProposta, PROPO.DtLimiteEntregaProposta,  " +
                              "        ISNULL(PROPO.VlProposta,0) VlProposta, PROPO.DtCriacao, Substring(PROPO.DsObservacao, 0, 80) DsObservacao " +
                              " FROM   PROPOSTA PROPO " +
                              "        LEFT JOIN PropostaCliente         PCLIE ON (PCLIE.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN PropostaCoordenador     PCOOR ON (PCOOR.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN PropostaDocs            ODOCS ON (ODOCS.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN Pessoa                  PESSO ON (PESSO.IdPessoa           = PCOOR.IdPessoa          ) " +
                              "        LEFT JOIN PessoaFisica            FISIC ON (FISIC.IdPessoaFisica     = PCOOR.IdPessoa           ) " +
                              "        LEFT JOIN PessoaJuridica          JURID ON (JURID.IdPessoaJuridica   = PESSO.IdPessoaJuridica  ) " +
                              "        LEFT JOIN TipoOportunidade        OTIPO ON (OTIPO.IdTipoOportunidade = PROPO.IdTipoOportunidade) " +
                              "        LEFT JOIN TipoProposta            PTIPO ON (PTIPO.IdTipoProposta     = PROPO.IdTipoProposta    ) " +
                              "        LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = PROPO.IdSituacao        ) " +
                              "        LEFT JOIN Tema                    TEMAS ON (TEMAS.IdTema             = PROPO.IdTema            ) " +
                              "        LEFT JOIN EsferaEmpresa           ESFER ON (ESFER.IdEsferaEmpresa    = JURID.IdEsferaEmpresa   ) " +
                              " WHERE (PROPO.IdProposta <> 0) ";

                if (!string.IsNullOrEmpty(pFiltro.edtNumProposta))
                {
                    sSQL = sSQL + " AND (PROPO.IdProposta LIKE '%" + pFiltro.edtNumProposta.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                else
                {
                    if ((!string.IsNullOrEmpty(pFiltro.edtNumOportunidade)) || (!string.IsNullOrEmpty(pFiltro.edtNumContrato)))
                    {
                        sSQL = sSQL + " AND (PROPO.IdProposta = 1) ";
                    }
                }
                if (!string.IsNullOrEmpty(pFiltro.dsContratante))
                {
                    sSQL = sSQL + " AND (Upper(PCLIE.NmFantasia)  LIKE '%" + pFiltro.dsContratante.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsRespCoordCliente))
                {
                    sSQL = sSQL + " AND (Upper(FISIC.NmPessoa  )  LIKE '%" + pFiltro.dsRespCoordCliente.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsTitulo))
                {
                    sSQL = sSQL + " AND (Upper(PROPO.DsAssunto)  LIKE '%" + pFiltro.dsTitulo.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsNomeDocto))
                {
                    sSQL = sSQL + " AND (Upper(ODOCS.NmDocumento)  LIKE '%" + pFiltro.dsNomeDocto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (pFiltro.situacao.ToString() != "")
                {
                    sSQL = sSQL + " AND (PROPO.IdSituacao = " + pFiltro.situacao.ToString() + ") ";
                }
                if (pFiltro.idTipoOportunidade.ToString() != "")
                {
                    sSQL = sSQL + " AND (PROPO.IdTipoOportunidade = " + pFiltro.idTipoOportunidade.ToString() + ") ";
                }
                //EGS 30.03.2021 Filtro por Situacao da proposta
                if (pFiltro.idSituacaoProposta.ToString() != "")
                {
                    sSQL = sSQL + " AND (PROPO.IdSituacao = " + pFiltro.idSituacaoProposta.ToString() + ") ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsObjeto))
                {
                    sSQL = sSQL + " AND (Upper(PROPO.DsObjeto)  LIKE '%" + pFiltro.dsObjeto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if ((!string.IsNullOrEmpty(pFiltro.dsCentroCusto)) || (!string.IsNullOrEmpty(pFiltro.dsNrProcesso)))
                {
                    sSQL = sSQL + " AND (PROPO.IdProposta = 9999999) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsValorMin))
                {
                    sSQL = sSQL + " AND (PROPO.VlProposta >= " + pFiltro.dsValorMin + ")";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsValorMax))
                {
                    sSQL = sSQL + " AND (PROPO.VlProposta <= " + pFiltro.dsValorMax + ")";
                }
                if (pFiltro.idEsfera.ToString() != "")
                {
                    sSQL = sSQL + " AND (JURID.IdEsferaEmpresa = " + pFiltro.idEsfera.ToString() + ") ";
                }
                if (pFiltro.idTema.ToString() != "")
                {
                    sSQL = sSQL + " AND (PROPO.IdTema = " + pFiltro.idTema.ToString() + ") ";
                }
                if (pFiltro.edtNumContraClie.ToString() != "")
                {
                    sSQL = sSQL + " AND (UPPER(PROPO.NuContratoCliente) LIKE '%" + pFiltro.edtNumContraClie.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }



                if (pFiltro.idComprovacaoValor.ToString() != "")
                {
                    sSQL = sSQL + " AND (PROPO.IdComprovacaoValor = " + pFiltro.idComprovacaoValor.ToString() + ") ";
                }




                //====================================================================== FINAL dos FILTROS ========================================================
                //====================================================================== FINAL dos FILTROS ========================================================
                sSQL = sSQL + " ORDER BY PROPO.IdProposta DESC ";

                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int nNumOport = 0;
                            if (!string.IsNullOrEmpty(reader["idOportunidade"].ToString())) { nNumOport = Convert.ToInt32(reader["idOportunidade"]); }
                            OutPutPesqGeralListaPropostas itemProposta = new OutPutPesqGeralListaPropostas();
                            itemProposta.idProposta = Convert.ToInt32(reader["idProposta"]);
                            itemProposta.idOportunidade = nNumOport;
                            itemProposta.nmContratantes = _GetPropostaNomeContratantes(itemProposta.idProposta);
                            itemProposta.nmCoordenadores = _GetPropostaNomeCoordenadores(itemProposta.idProposta);
                            itemProposta.dsTitulo = Convert.ToString(reader["DsAssunto"]);
                            itemProposta.dsObjeto = Convert.ToString(reader["DsObjeto"]);
                            itemProposta.dsApelido = Convert.ToString(reader["DsApelidoProposta"]);
                            itemProposta.dsSituacao = Convert.ToString(reader["DsSituacao"]);
                            itemProposta.dsTema = Convert.ToString(reader["DsTema"]);
                            itemProposta.dtCriacao = Convert.ToString(reader["DtCriacao"]);
                            itemProposta.dtValidade = Convert.ToString(reader["DtValidadeProposta"]);
                            itemProposta.nValor = Convert.ToDecimal(reader["VlProposta"]);
                            itemProposta.dsValor = String.Format("{0:C2}", itemProposta.nValor);
                            if (itemProposta.dtCriacao.Length > 10) { itemProposta.dtCriacao = itemProposta.dtCriacao.Substring(0, 10); }
                            if (itemProposta.dtValidade.Length > 10) { itemProposta.dtValidade = itemProposta.dtValidade.Substring(0, 10); }
                            //--------------------------------------------------------------------------------------------------
                            allPropostas.Add(itemProposta);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetPropostas");
                throw;
            }
            return allPropostas;
        }


        /* ===========================================================================================
        *  Alessandro Pereira FIPE
        *  Fev/2021 
        *  Retorna todos os registros de Propostas com os filtros informados mais rapido
        ===========================================================================================*/
        public List<OutPutGetPropostas> GetPropostas()
        {
            var allPropostas = new List<OutPutGetPropostas>();
            try
            {
                string sSQL = " SELECT DISTINCT PROPO.IdProposta, PROPO.IdOportunidade, PROPO.IdSituacao, SITUA.DsSituacao, " +
                              "        PROPO.IdTema, TEMAS.DsTema, Substring(PROPO.DsApelidoProposta, 0, 80) DsApelidoProposta," +
                              "        PROPO.DsAssunto DsAssunto, Substring(PROPO.DsObjeto, 0, 80) DsObjeto, PROPO.IdSituacao, " +
                              " 	   PROPO.IdTipoOportunidade, Substring(OTIPO.DsTipoOportunidade,0,80) DsTipoOportunidade, PROPO.NuContratoCliente, " +
                              "        PROPO.DtProposta, PROPO.DtValidadeProposta, PROPO.DtLimiteEnvioProposta, PROPO.DtLimiteEntregaProposta as DtLimiteEntregaProposta ,  " +
                              "        PROPO.VlProposta, PROPO.DtCriacao, Substring(PROPO.DsObservacao, 0, 80) DsObservacao,PROPO.DtUltimaAlteracao as DtUltimaAlteracao, " +
                              "        (SELECT  NmFantasia  + '+'  AS[text()] FROM propostacliente ST1 WHERE ST1.IdProposta = PCLIE.IdProposta ORDER BY ST1.NmFantasia FOR XML PATH(''))  AS PessoaJuridica, " +
                              "        (SELECT  ST1.NmPessoa  + '+' AS[text()] FROM(SELECT distinct  PROPO1.IdProposta, FISIC1.NmPessoa   from proposta PROPO1  JOIN propostacliente PCLIE1  ON(PCLIE1.idproposta = PROPO1.idproposta) LEFT JOIN propostacoordenador PCOOR1    ON(PCOOR1.idproposta = PROPO1.idproposta) LEFT JOIN propostadocs ODOCS1 ON(ODOCS1.idproposta = PROPO1.idproposta) LEFT JOIN pessoa PESSO1 ON(PESSO1.idpessoa = PCOOR1.idpessoa) LEFT JOIN pessoafisica FISIC1  ON(FISIC1.idpessoafisica = PCOOR1.idpessoa)) ST1 where st1.IdProposta = PROPO.IdProposta   ORDER BY ST1.NmPessoa  FOR XML PATH(''))  AS PessoaFisica," +
                              "        (SELECT top 1 ppjd.UF   AS[text()] FROM PropostaCliente ST1 join Cliente ORESP1 on(ORESP1.IdCliente = st1.IdCliente) join Pessoa o on(o.IdPessoa = ORESP1.IdPessoa) join PessoaJuridica ppjd on(ppjd.IdPessoaJuridica = o.IdPessoaJuridica) WHERE ST1.IdProposta = PCLIE.IdProposta ORDER BY ST1.RazaoSocial FOR XML PATH(''))  AS uf," +
                              "        (SELECT top 1 ccd.NmCidade   AS[text()] FROM PropostaCliente ST1 join Cliente ORESP1 on(ORESP1.IdCliente = st1.IdCliente) join Pessoa o on(o.IdPessoa = ORESP1.IdPessoa) join PessoaJuridica ppjd on(ppjd.IdPessoaJuridica = o.IdPessoaJuridica) join Cidade ccd on(ccd.IdCidade = ppjd.IdCidade) WHERE ST1.IdProposta = PCLIE.IdProposta ORDER BY ST1.RazaoSocial FOR XML PATH(''))  AS cidade," +
                              "        (select top 1  eef.DsEsferaEmpresa from PropostaCliente ppc join Cliente ccl on ppc.IdCliente = ccl.IdCliente join Pessoa pp on pp.IdPessoa = ccl.IdPessoa join PessoaJuridica ppj on pp.IdPessoaJuridica = ppj.IdPessoaJuridica join EsferaEmpresa eef on eef.IdEsferaEmpresa = ppj.IdEsferaEmpresa where IdProposta = PROPO.idproposta) esfera," +
                              "        (select top 1  ppj.NmPessoa from PropostaCliente ppc join Cliente ccl on ppc.IdCliente = ccl.IdCliente join Pessoa pp on pp.IdPessoa  = ccl.IdPessoa join PessoaFisica ppj on pp.IdPessoaFisica = ppj.IdPessoaFisica  where IdProposta=PROPO.idproposta) as PessoaFisicaContratante," +
                              "        (SELECT TOP 1 eef.DsClassificacaoEmpresa FROM propostacliente ppc JOIN cliente ccl ON ppc.idcliente = ccl.idcliente JOIN pessoa pp  ON pp.idpessoa = ccl.idpessoa   JOIN pessoajuridica ppj ON pp.idpessoajuridica = ppj.idpessoajuridica JOIN ClassificacaoEmpresa eef  ON eef.IdClassificacaoEmpresa = ppj.IdClassificacaoEmpresa  WHERE idproposta = PROPO.idproposta)     classificacao, (select count(1) from ContratoAditivo where IdProposta= PROPO.IdProposta) as HasAditivo " +
                              " FROM   PROPOSTA PROPO " +
                              "        LEFT JOIN PropostaCliente         PCLIE ON (PCLIE.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN PropostaCoordenador     PCOOR ON (PCOOR.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN PropostaDocs            ODOCS ON (ODOCS.IdProposta         = PROPO.IdProposta        ) " +
                              "        LEFT JOIN Pessoa                  PESSO ON (PESSO.IdPessoa           = PCOOR.IdPessoa          ) " +
                              "        LEFT JOIN PessoaFisica            FISIC ON (FISIC.IdPessoaFisica     = PCOOR.IdPessoa           ) " +
                              "        LEFT JOIN PessoaJuridica          JURID ON (JURID.IdPessoaJuridica   = PESSO.IdPessoaJuridica  ) " +
                              "        LEFT JOIN TipoOportunidade        OTIPO ON (OTIPO.IdTipoOportunidade = PROPO.IdTipoOportunidade) " +
                              "        LEFT JOIN TipoProposta            PTIPO ON (PTIPO.IdTipoProposta     = PROPO.IdTipoProposta    ) " +
                              "        LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = PROPO.IdSituacao        ) " +
                              "        LEFT JOIN Tema                    TEMAS ON (TEMAS.IdTema             = PROPO.IdTema            ) " +
                              "        LEFT JOIN EsferaEmpresa           ESFER ON (ESFER.IdEsferaEmpresa    = JURID.IdEsferaEmpresa   ) " +
                              "        LEFT JOIN Cidade                  CIDPJ   ON (CIDPJ.IdCidade         = JURID.IdCidade          ) " +
                              "        LEFT JOIN Cidade                  CIDPF   ON (CIDPF.IdCidade         = FISIC.IdCidade          ) " +
                              " WHERE (PROPO.IdProposta <> 0) ORDER BY PROPO.IdProposta DESC  ";   

                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutPutGetPropostas itemProposta = new OutPutGetPropostas();
                            itemProposta.IdProposta         = Convert.ToInt32 (reader["idProposta"]);
                            itemProposta.DsAssunto          = Convert.ToString(reader["DsAssunto"]);
                            itemProposta.DsApelidoProposta  = Convert.ToString(reader["DsApelidoProposta"]);
                            itemProposta.DsSituacao         = Convert.ToString(reader["DsSituacao"]);
                            itemProposta.IdSituacao         = Convert.ToInt32 (reader["IdSituacao"]);
                            if (!string.IsNullOrEmpty(Convert.ToString(reader["VlProposta"])))
                            {
                                itemProposta.VlProposta     = Convert.ToDecimal(reader["VlProposta"]);
                            }
                            if (!string.IsNullOrEmpty(Convert.ToString(reader["DtLimiteEntregaProposta"])))
                                itemProposta.DtLimiteEntregaProposta = Convert.ToDateTime(reader["DtLimiteEntregaProposta"]);
                            itemProposta.clientes = new List<string>();
                            itemProposta.coordenadores = new List<string>();

                            if (String.IsNullOrEmpty(Convert.ToString(reader["PessoaJuridica"])))
                            {
                                itemProposta.clientesTexto = Convert.ToString(reader["PessoaFisicaContratante"]);
                                itemProposta.clientes.Add(" ");
                                itemProposta.clientes.Add(Convert.ToString(reader["PessoaFisicaContratante"]));
                            }
                            else
                            {
                                itemProposta.clientes.Add(" ");
                                itemProposta.clientesTexto = Convert.ToString(reader["PessoaJuridica"]);
                                itemProposta.clientes.AddRange(Convert.ToString(reader["PessoaJuridica"]).Split('+').ToList());
                            }
                            itemProposta.NmCidade           = Convert.ToString(reader["cidade"]);
                            itemProposta.UF                 = Convert.ToString(reader["UF"]);
                            itemProposta.HasAditivo         = Convert.ToBoolean(reader["HasAditivo"]);
                            itemProposta.coordenadoresTexto = Convert.ToString(reader["PessoaFisica"]);
                            itemProposta.coordenadores.Add(" ");
                            itemProposta.coordenadores.AddRange(Convert.ToString(reader["PessoaFisica"]).Split('+').ToList());
                            itemProposta.DtUltimaAlteracao = Convert.ToDateTime(reader["DtUltimaAlteracao"]);
                            if (!string.IsNullOrEmpty(Convert.ToString(reader["esfera"])))
                                itemProposta.DsEsfera = Convert.ToString(reader["esfera"]);
                            else
                                itemProposta.DsEsfera = Convert.ToString(reader["classificacao"]);

                            //EGS 30.06.2021 se for aditivo, mostra no final do assunto
                            if (itemProposta.HasAditivo == true)
                            {
                                itemProposta.DsApelidoProposta = itemProposta.DsApelidoProposta + " (aditivo)";
                            }
                            allPropostas.Add(itemProposta);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetPropostas");
                throw;
            }
            return allPropostas;
        }



        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os registros de Propostas com os filtros informados
        ===========================================================================================*/
        public List<OutPutPesqGeralListaContratos> GetContratos(InputPesqGeral pFiltro)
        {
            var allContratos = new List<OutPutPesqGeralListaContratos>();
            try
            {
                string sSQL = " SELECT DISTINCT CONTR.IdContrato, CONTR.IdProposta, CONTR.IdSituacao, SITUA.DsSituacao, CONTR.IdFundamento, FUNDA.DsFundamento, " +
                              "        CONTR.NuContratoEdit, CONTR.DsPrazoExecucao, " +
                              "        CONTR.NuContratoCliente, CONTR.NuProcessoCliente, Substring(CONTR.DsApelido, 0, 80) DsApelido, " +
                              "        Substring(CONTR.DsAssunto, 0, 80) DsAssunto, Substring(CONTR.DsObjeto, 0, 80) DsObjeto, " +
                              "        Substring(CONTR.DsObservacao, 0, 80) DsObservacao, ISNULL(CONTR.VlContrato,0) VlContrato, CONTR.DtAssinatura, " +
                              "        CONTR.NuCentroCusto, CONTR.CdISS, CONTR.IdTema, TEMAS.DsTema," +
                              "        CONTR.DtInicio, CONTR.DtFim, CONTR.IdUsuarioCriacao, CONTR.IdUsuarioUltimaAlteracao " +
                              " FROM   CONTRATO CONTR" +
                              "         LEFT JOIN ContratoCliente         CCLIE ON (CCLIE.IdContrato         = CONTR.IdContrato        ) " +
                              "         LEFT JOIN ContratoDoc             CDOCS ON (CDOCS.IdContrato         = CONTR.IdContrato        ) " +
                              "         LEFT JOIN ContratoCoordenador     CCOOR ON (CCOOR.IdContrato         = CCLIE.IdContrato        ) " +
                              "         LEFT JOIN Pessoa                  PESSO ON (PESSO.IdPessoa           = CCOOR.IdPessoa          ) " +
                              "         LEFT JOIN PessoaFisica            FISIC ON (FISIC.IdPessoaFisica     = CCOOR.IdPessoa          ) " +
                              "         LEFT JOIN PessoaJuridica          JURID ON (JURID.IdPessoaJuridica   = PESSO.IdPessoaJuridica  ) " +
                              "         LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = CONTR.IdSituacao        ) " +
                              "         LEFT JOIN Tema                    TEMAS ON (TEMAS.IdTema             = CONTR.IdTema            ) " +
                              "         LEFT JOIN FundamentoContratacao   FUNDA ON (FUNDA.IdFundamento       = CONTR.IdFundamento      ) " +
                              "         LEFT JOIN EsferaEmpresa           ESFER ON (ESFER.IdEsferaEmpresa    = JURID.IdEsferaEmpresa   ) " +
                              " WHERE (CONTR.IdContrato <> 0)";

                if (!string.IsNullOrEmpty(pFiltro.edtNumContrato))
                {
                    sSQL = sSQL + " AND (CONTR.NuContratoEdit LIKE '%" + pFiltro.edtNumContrato.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                else
                {
                    if ((!string.IsNullOrEmpty(pFiltro.edtNumOportunidade)) || (!string.IsNullOrEmpty(pFiltro.edtNumProposta)) || (pFiltro.idSituacaoProposta.ToString() != ""))
                    {
                        sSQL = sSQL + " AND (CONTR.IdContrato = 0) ";
                    }
                }
                if (!string.IsNullOrEmpty(pFiltro.dsContratante))
                {
                    sSQL = sSQL + " AND (Upper(CCLIE.NmFantasia)  LIKE '%" + pFiltro.dsContratante.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsRespCoordCliente))
                {
                    sSQL = sSQL + " AND (Upper(FISIC.NmPessoa  )  LIKE '%" + pFiltro.dsRespCoordCliente.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsTitulo))
                {
                    sSQL = sSQL + " AND (Upper(CONTR.DsAssunto)  LIKE '%" + pFiltro.dsTitulo.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsNomeDocto))
                {
                    sSQL = sSQL + " AND (Upper(CDOCS.DsDoc)      LIKE '%" + pFiltro.dsNomeDocto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (pFiltro.situacao.ToString() != "")
                {
                    sSQL = sSQL + " AND (CONTR.IdSituacao = " + pFiltro.situacao.ToString() + ") ";
                }
                if (pFiltro.idTipoOportunidade.ToString() != "")
                {
                    sSQL = sSQL + " AND (CONTR.IdContrato = 9999999999" + pFiltro.idTipoOportunidade.ToString() + ") ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsObjeto))
                {
                    sSQL = sSQL + " AND (Upper(CONTR.DsObjeto)  LIKE '%" + pFiltro.dsObjeto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsCentroCusto))
                {
                    sSQL = sSQL + " AND (Upper(CONTR.NuCentroCusto)  LIKE '%" + pFiltro.dsCentroCusto.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsNrProcesso))
                {
                    sSQL = sSQL + " AND (Upper(CONTR.NuProcessoCliente)  LIKE '%" + pFiltro.dsNrProcesso.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsValorMin))
                {
                    sSQL = sSQL + " AND (CONTR.VlContrato >= " + pFiltro.dsValorMin + ")";
                }
                if (!string.IsNullOrEmpty(pFiltro.dsValorMax))
                {
                    sSQL = sSQL + " AND (CONTR.VlContrato <= " + pFiltro.dsValorMax + ")";
                }
                if (pFiltro.idEsfera.ToString() != "")
                {
                    sSQL = sSQL + " AND (JURID.IdEsferaEmpresa = " + pFiltro.idEsfera.ToString() + ") ";
                }
                if (pFiltro.idTema.ToString() != "")
                {
                    sSQL = sSQL + " AND (CONTR.IdTema = " + pFiltro.idTema.ToString() + ") ";
                }
                if (pFiltro.edtNumContraClie.ToString() != "")
                {
                    sSQL = sSQL + " AND (UPPER(CONTR.NuContratoCliente) LIKE '%" + pFiltro.edtNumContraClie.ToUpper() + "%'  COLLATE SQL_Latin1_General_CP1253_CI_AI) ";
                }




                //====================================================================== FINAL dos FILTROS ========================================================
                //====================================================================== FINAL dos FILTROS ========================================================
                sSQL = sSQL + " ORDER BY CONTR.IdContrato DESC ";

                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutPutPesqGeralListaContratos itemContrato = new OutPutPesqGeralListaContratos();
                            itemContrato.idContrato = Convert.ToInt32(reader["idContrato"]);
                            itemContrato.idProposta = Convert.ToInt32(reader["idProposta"]);
                            itemContrato.nmContratantes = _GetContratoNomeContratantes(itemContrato.idContrato);
                            itemContrato.nmClientes = _GetContratoNomeCoordenadores(itemContrato.idContrato);
                            itemContrato.dsNumEdit = Convert.ToString(reader["NuContratoEdit"]);
                            itemContrato.dsProcessoCliente = Convert.ToString(reader["NuProcessoCliente"]);
                            itemContrato.dsTitulo = Convert.ToString(reader["DsAssunto"]);
                            itemContrato.dsObjeto = Convert.ToString(reader["DsObjeto"]);
                            itemContrato.dtAssinatura = Convert.ToString(reader["DtAssinatura"]);
                            itemContrato.dsPrazoExecucao = Convert.ToString(reader["DsPrazoExecucao"]);
                            itemContrato.dsCentroCusto = Convert.ToString(reader["NuCentroCusto"]);
                            itemContrato.nValor = Convert.ToDecimal(reader["VlContrato"]);
                            itemContrato.dsSituacao = Convert.ToString(reader["DsSituacao"]);
                            itemContrato.dsValor = String.Format("{0:C2}", itemContrato.nValor);
                            if (itemContrato.dtAssinatura.Length > 10) { itemContrato.dtAssinatura = itemContrato.dtAssinatura.Substring(0, 10); }
                            //--------------------------------------------------------------------------------------------------
                            allContratos.Add(itemContrato);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetContratos");
                throw;
            }
            return allContratos;
        }




        
        public List<OutPutGetContratos> GetContratos()
        {
            var allContratos = new List<OutPutGetContratos>();
            try
            {
                string sSQL = " SELECT DISTINCT CONTR.IdContrato, CONTR.IdProposta, CONTR.IdSituacao, SITUA.DsSituacao, CONTR.IdFundamento, FUNDA.DsFundamento, CONTR.IcOrdemInicio,CONTR.IcRenovacaoAutomatica, " +
                              "        CONTR.NuContratoEdit, CONTR.DsPrazoExecucao, " +
                              "        CONTR.NuContratoCliente, CONTR.NuProcessoCliente, Substring(CONTR.DsApelido, 0, 80) DsApelido, " +
                              "        CONTR.DsAssunto DsAssunto, Substring(CONTR.DsObjeto, 0, 80) DsObjeto, " +
                              "        Substring(CONTR.DsObservacao, 0, 80) DsObservacao, ISNULL(CONTR.VlContrato,0) VlContrato, CONTR.DtAssinatura, " +
                              "        CONTR.NuCentroCusto, CONTR.CdISS, CONTR.IdTema, TEMAS.DsTema," +
                              "        CONTR.DtInicio, CONTR.DtFim, CONTR.IdUsuarioCriacao, CONTR.IdUsuarioUltimaAlteracao, " +
                              "        (SELECT NmFantasia   + '+'  AS [text()] FROM ContratoCliente ST1  WHERE ST1.IdContrato = CONTR.IdContrato  ORDER BY ST1.NmFantasia  FOR XML PATH (''))  AS PessoaJuridica," +
                              "        (SELECT ST1.NmPessoa + '+' AS [text()] FROM (SELECT distinct  PROPO1.IdContrato, FISIC1.NmPessoa   from  contrato PROPO1  JOIN contratocliente PCLIE1  ON ( PCLIE1.IdContrato = PROPO1.IdContrato ) LEFT JOIN contratocoordenador PCOOR1    ON ( PCOOR1.IdContrato = PROPO1.IdContrato ) LEFT JOIN contratodoc ODOCS1 ON ( ODOCS1.IdContrato = PROPO1.IdContrato ) LEFT JOIN pessoa PESSO1 ON ( PESSO1.idpessoa = PCOOR1.idpessoa ) LEFT JOIN pessoafisica FISIC1  ON (FISIC1.idpessoafisica = PCOOR1.idpessoa)) ST1 where st1.IdContrato = CONTR.IdContrato   ORDER BY ST1.NmPessoa  FOR XML PATH (''))  AS PessoaFisica" +
                              " FROM   CONTRATO CONTR" +
                              "         LEFT JOIN ContratoCliente         CCLIE ON (CCLIE.IdContrato         = CONTR.IdContrato        ) " +
                              "         LEFT JOIN ContratoDoc             CDOCS ON (CDOCS.IdContrato         = CONTR.IdContrato        ) " +
                              "         LEFT JOIN ContratoCoordenador     CCOOR ON (CCOOR.IdContrato         = CCLIE.IdContrato        ) " +
                              "         LEFT JOIN Pessoa                  PESSO ON (PESSO.IdPessoa           = CCOOR.IdPessoa          ) " +
                              "         LEFT JOIN PessoaFisica            FISIC ON (FISIC.IdPessoaFisica     = CCOOR.IdPessoa          ) " +
                              "         LEFT JOIN PessoaJuridica          JURID ON (JURID.IdPessoaJuridica   = PESSO.IdPessoaJuridica  ) " +
                              "         LEFT JOIN Situacao                SITUA ON (SITUA.IdSituacao         = CONTR.IdSituacao        ) " +
                              "         LEFT JOIN Tema                    TEMAS ON (TEMAS.IdTema             = CONTR.IdTema            ) " +
                              "         LEFT JOIN FundamentoContratacao   FUNDA ON (FUNDA.IdFundamento       = CONTR.IdFundamento      ) " +
                              "         LEFT JOIN EsferaEmpresa           ESFER ON (ESFER.IdEsferaEmpresa    = JURID.IdEsferaEmpresa   ) " +
                              " WHERE (CONTR.IdContrato <> 0) ORDER BY CONTR.NuContratoEdit DESC";


                string stringConexaoPortal = FIPEContratosContext.ConnectionString;

                using (SqlConnection connection = new SqlConnection(stringConexaoPortal))
                {
                    SqlCommand command = new SqlCommand(sSQL, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OutPutGetContratos itemContrato = new OutPutGetContratos();
                            try
                            {
                                itemContrato.IdContrato = Convert.ToInt32(reader["idContrato"]);
                                itemContrato.IdProposta = Convert.ToInt32(reader["idProposta"]);
                                itemContrato.NuContratoEdit = Convert.ToString(reader["NuContratoEdit"]);
                                itemContrato.DsAssunto = Convert.ToString(reader["DsAssunto"]);
                                itemContrato.DtAssinatura = Convert.ToDateTime(reader["DtAssinatura"]);
                                itemContrato.DsPrazoExecucao = Convert.ToString(reader["DsPrazoExecucao"]);
                                itemContrato.DsSituacao = Convert.ToString(reader["DsSituacao"]);
                                itemContrato.DsCentroCusto = Convert.ToString(reader["NucentroCusto"]);
                                if (Convert.ToDecimal(reader["VlContrato"]) > 0) itemContrato.VlContrato = Convert.ToDecimal(reader["VlContrato"]);
                                itemContrato.clientes = new List<string>();
                                itemContrato.coordenadores = new List<string>();
                                itemContrato.clientesTexto = Convert.ToString(reader["PessoaJuridica"]);

                                //EGS 30.05.2021 - Alessandro colocou um jeito de listar varias pessoas ou PJ, mas esta vindo com campo em branco, tem que ser removido
                                var lstPessoaJuridica = Convert.ToString(reader["PessoaJuridica"]).Split('+').ToList();
                                foreach (var lstItem in lstPessoaJuridica)
                                {
                                    if (lstItem.ToString() != "")
                                    {
                                        itemContrato.clientes.Add(lstItem.ToString());
                                    }
                                }

                                //EGS 30.05.2021 - Alessandro colocou um jeito de listar varias pessoas ou PJ, mas esta vindo com campo em branco, tem que ser removido
                                var lstPessoaFisica = Convert.ToString(reader["PessoaFisica"]).Split('+').ToList();
                                foreach (var lstItem in lstPessoaFisica)
                                {
                                    if (lstItem.ToString() != "")
                                    {
                                        itemContrato.coordenadores.Add(lstItem.ToString());
                                    }
                                }

                                if (Convert.ToBoolean(reader["IcOrdemInicio"]))
                                    itemContrato.IcOrdemInicio = "Sim";
                                else
                                    itemContrato.IcOrdemInicio = "Não";

                                if (Convert.ToBoolean(reader["IcRenovacaoAutomatica"]))
                                    itemContrato.IcRenovacaoAutomatica = "Sim";
                                else
                                    itemContrato.IcRenovacaoAutomatica = "Não";
                            }
                            catch (Exception Exx)
                            {
                                string sErro = Exx.Message + " " + Exx.InnerException;
                            }
                            finally
                            {
                                allContratos.Add(itemContrato);
                            }
                            ////--------------------------------------------------------------------------------------------------
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-GetContratos");
                throw;
            }
            return allContratos;
        }






















        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Contratantes da Oportunidade com os filtros informados
        ===========================================================================================*/
        private string _GetOportunidadeNumProposta(int pIDOortunidade)
        {
            string iNumProposta = "";
            try
            {
                var dbProposta = db.Proposta.Where(x => x.IdOportunidade == pIDOortunidade).FirstOrDefault();
                if (dbProposta != null)
                {
                    iNumProposta = dbProposta.IdProposta.ToString();
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetOportunidadeNumProposta");
                throw;
            }
            return iNumProposta;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 http://localhost:4200/login
        *  Retorna todos os Contratantes da Oportunidade com os filtros informados
        ===========================================================================================*/
        private string _GetOportunidadeNomeContratantes(int pIDOortunidade)
        {
            string sNmContratante = "";
            try
            {
                var dbOportunidadeCliente = db.OportunidadeCliente.Where(x => x.IdOportunidade == pIDOortunidade).ToList();
                if (dbOportunidadeCliente != null)
                {
                    foreach (var dbItem in dbOportunidadeCliente)
                    {
                        if (!string.IsNullOrEmpty(dbItem.NmFantasia))
                        {
                            if (dbItem.NmFantasia.Length >= 80)
                            {
                                sNmContratante = sNmContratante + dbItem.NmFantasia.Substring(0, 75) + " ";
                            }
                            else
                            {
                                sNmContratante = sNmContratante + dbItem.NmFantasia + " ";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetOportunidadeNomeContratantes");
                throw;
            }
            return sNmContratante;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Contratantes da Proposta com os filtros informados
        ===========================================================================================*/
        private string _GetPropostaNomeContratantes(int pIDProposta)
        {
            string sNmContratante = "";
            try
            {
                var dbPropostaCliente = db.PropostaCliente.Where(x => x.IdProposta == pIDProposta).ToList();
                foreach (var dbItem in dbPropostaCliente)
                {
                    if (!string.IsNullOrEmpty(dbItem.NmFantasia))
                    {
                        if (dbItem.NmFantasia.Length >= 80)
                        {
                            sNmContratante = sNmContratante + dbItem.NmFantasia.Substring(0, 79) + " ";
                        }
                        else
                        {
                            sNmContratante = sNmContratante + dbItem.NmFantasia + " ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetPropostaNomeContratantes");
                throw;
            }
            return sNmContratante;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Coordenadores da Oportunidade com os filtros informados
        ===========================================================================================*/
        private string _GetOportunidadeNomeResponsaveis(int pIDOortunidade)
        {
            string sNmResponsaveis = "";
            try
            {
                var dbOportunidadeResponsavel = db.OportunidadeResponsavel.Where(x => x.IdOportunidade == pIDOortunidade).ToList();
                foreach (var dbItem in dbOportunidadeResponsavel)
                {
                    var dbPessoaFisica = db.PessoaFisica.Where(x => x.IdPessoaFisica == dbItem.IdPessoaFisica).ToList();
                    foreach (var dbItemPessoa in dbPessoaFisica)
                    {
                        if (dbItemPessoa.NmPessoa.Length >= 80)
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa.Substring(0, 79) + " ";
                        }
                        else
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa + " ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetOportunidadeNomeResponsaveis");
                throw;
            }
            return sNmResponsaveis;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Coordenadores da Proposta com os filtros informados
        ===========================================================================================*/
        private string _GetPropostaNomeCoordenadores(int pIDProposta)
        {
            string sNmResponsaveis = "";
            try
            {
                var dbPropostaCoordenador = db.PropostaCoordenador.Where(x => x.IdProposta == pIDProposta).ToList();
                foreach (var dbItem in dbPropostaCoordenador)
                {
                    var dbPessoaFisica = db.PessoaFisica.Where(x => x.IdPessoaFisica == dbItem.IdPessoa).ToList();
                    foreach (var dbItemPessoa in dbPessoaFisica)
                    {
                        if (dbItemPessoa.NmPessoa.Length >= 80)
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa.Substring(0, 79) + " ";
                        }
                        else
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa + " ";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetPropostaNomeCoordenadores");
                throw;
            }
            return sNmResponsaveis;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Contratantes do Contrato com os filtros informados
        ===========================================================================================*/
        private string _GetContratoNomeContratantes(int pIDContrato)
        {
            string sNmContratante = "";
            try
            {
                var dbContratoCliente = db.ContratoCliente.Where(x => x.IdContrato == pIDContrato).ToList();
                foreach (var dbItem in dbContratoCliente)
                {
                    if (dbItem.NmFantasia.Length >= 80)
                    {
                        sNmContratante = sNmContratante + dbItem.NmFantasia.Substring(0, 79) + " ";
                    }
                    else
                    {
                        sNmContratante = sNmContratante + dbItem.NmFantasia + " ";
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetContratoNomeContratantes");
                throw;
            }
            return sNmContratante;
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Jul/2020 
        *  Retorna todos os Coordenadores da Proposta com os filtros informados
        ===========================================================================================*/
        private string _GetContratoNomeCoordenadores(int pIDContrato)
        {
            string sNmResponsaveis = "";
            try
            {
                var dbContratoCoordenador = db.ContratoCoordenador.Where(x => x.IdContrato == pIDContrato).ToList();
                foreach (var dbItem in dbContratoCoordenador)
                {
                    var dbPessoaFisica = db.PessoaFisica.Where(x => x.IdPessoaFisica == dbItem.IdPessoa).ToList();
                    foreach (var dbItemPessoa in dbPessoaFisica)
                    {
                        if (dbItemPessoa.NmPessoa.Length >= 80)
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa.Substring(0, 79) + " ";
                        }
                        else
                        {
                            sNmResponsaveis = sNmResponsaveis + dbItemPessoa.NmPessoa + " ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var db = new FIPEContratosContext();
                new bEmail(db).EnviarEmailTratamentoErro(ex, "PesquisaGeralController-_GetPropostaNomeCoordenadores");
                throw;
            }
            return sNmResponsaveis;
        }



    }
}