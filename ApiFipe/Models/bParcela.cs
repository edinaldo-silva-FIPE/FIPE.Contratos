using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using ApiFipe.Utilitario.Extensoes;
using System;
using System.Collections.Generic;
using System.Linq;
using ApiFipe.Utilitario;


namespace ApiFipe.Models
{
    public class bParcela
    {
        #region | Constantes

        private const string CODIGO_PARCELA = "CT{0}-{1}{2}{3}";
        GravaLog _GLog = new GravaLog();

        #endregion

        #region | Propriedades

        public FIPEContratosContext db { get; set; }

        #endregion

        #region | Construtores

        public bParcela(FIPEContratosContext db)
        {
            this.db = db;
        }

        #endregion

        #region | Métodos

        #region || Métodos Públicos

        public void GerarCronogramaFinanceiro(int idContrato_)
        {
            List<EntregavelDTO> entregaveis = new bEntregavel(db).ObterEntregaveis(idContrato_);

            if (entregaveis.ColecaoVazia())
            {
                _GLog._GravaLog(AppSettings.constGlobalUserID, "GerarCronogramaFinanceiro Contrato [" + idContrato_ + "] vazio");
            }
            else { 
                string iss                 = ObterCodigoISS(idContrato_);
                var clientes               = ObterClientesPagador(idContrato_);
                var parcelas               = new List<ParcelaDTO>();
                var numParcelas            = entregaveis.Select(w => w.Numero).Distinct().ToList().Count * clientes.Count;
                decimal valorTotalContrato = ObterValorContrato(idContrato_);
                decimal valorParcelas      = valorTotalContrato / numParcelas;                               
                var valorPorParcelaString  = valorParcelas.ToString().Split(',');
                var valorPorParcelaTexto   = valorPorParcelaString[0] + "," + valorPorParcelaString[1].Substring(0, 2);
                valorParcelas              = Convert.ToDecimal(valorPorParcelaTexto);
                
                //valorParcelas = Math.Round(valorParcelas, 2);
                decimal valorTotalParcelas = valorParcelas * numParcelas;
                decimal valorDiferencaUltimaParcela = 0;
                if (valorTotalParcelas < valorTotalContrato)
                {
                    valorDiferencaUltimaParcela = valorTotalContrato - valorTotalParcelas;
                }
                else if (valorTotalParcelas > valorTotalContrato)
                {
                    valorDiferencaUltimaParcela = valorTotalParcelas - valorTotalContrato;
                }
                var idContratoTexto = idContrato_.ToString();
                while (idContratoTexto.Length < 4)
                {
                    idContratoTexto = "0" + idContratoTexto;
                }
                foreach (var cliente in clientes)
                {
                    var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == cliente.Id && w.IdContrato == idContrato_).FirstOrDefault();
                    var nuContratante = contratoCliente.NuContratante.Value.ToString();
                    while (nuContratante.Length < 2)
                    {
                        nuContratante = "0" + nuContratante;
                    }
                    var entregaveisCliente = entregaveis.Where(w => w.Cliente.Id == cliente.Id).ToList();
                    if (entregaveisCliente.Count() == 0)
                    {
                        _GLog._GravaLog(AppSettings.constGlobalUserID, "Cronograma Financeiro do Contrato [" + idContrato_ + "] nao encontrado entregaveisCliente [" + cliente.Id + "]");
                    }
                    else {
                        foreach (var entregavel in entregaveisCliente)
                        {
                            var frente = new bFrente(db).BuscaFrenteId(entregavel.Frente.Id);
                            bool atraso = false;
                            SituacaoDTO situacao = new SituacaoDTO() { Id = 92, Nome = "No Prazo" };
                            if (entregavel.DataPrevista != null)
                            {
                                if (entregavel.DataPrevista.Value < DateTime.Now.Date)
                                {
                                    atraso = true;
                                }
                            }
                            else
                            {
                                situacao = new SituacaoDTO() { Id = 98, Nome = "A definir" };
                            }
                            parcelas.Add(new ParcelaDTO()
                            {
                                CdISS = iss,
                                Cliente = cliente,
                                DtFaturamento = entregavel.DataPrevista,
                                CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, frente.CdFrenteTexto, "00"),
                                Entregaveis = new List<int> { entregavel.Id },
                                IdContrato = idContrato_,
                                NuParcela = (Int16)entregavel.Numero,
                                Valor = valorParcelas * clientes.Count,
                                IcAtraso = atraso,
                                Situacao = situacao,
                                Frente = new bFrente(db).ConsultarFrente(entregavel.Frente.Id),
                            });
                        }
                    }
                }

                //EGS 30.06.2020 Em alguns casos estava vindo com parcelas zeradas, e dava erro...
                if (parcelas.Count >= 1)
                {
                    var nParc = parcelas.Count - 1;
                    if (parcelas[nParc].Valor < valorTotalContrato)
                    {
                        parcelas[nParc].Valor += valorDiferencaUltimaParcela;
                    }
                    else if (parcelas[nParc].Valor > valorTotalContrato)
                    {
                        parcelas[nParc].Valor -= valorDiferencaUltimaParcela;
                    }
                }
                parcelas.ForEach(_ => CriarNovaParcela(_));

                _GLog._GravaLog(AppSettings.constGlobalUserID, "Cronograma Financeiro do Contrato [" + idContrato_ + "] criado com sucesso");
            }
        }

        private void CriarNovaParcela(ParcelaDTO novaParcela_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == novaParcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            var idContratoTexto = novaParcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == novaParcela_.Cliente.Id && w.IdContrato == novaParcela_.IdContrato).FirstOrDefault();
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            int situacao = 0;
            bool atraso = false;

            if (novaParcela_.Situacao.Id != 94 && novaParcela_.Situacao.Id != 96)
            {
                if (novaParcela_.DtFaturamento != null)
                {
                    if (novaParcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                    {
                        atraso = true;
                        situacao = 93;
                    }
                    else
                    {
                        atraso = false;
                        situacao = novaParcela_.Situacao.Id;
                    }
                }
                else
                {
                    atraso = false;
                    situacao = novaParcela_.Situacao.Id;
                }
            }
            else
            {
                atraso = false;
                situacao = novaParcela_.Situacao.Id;
            }

            var novoRegistro = new ContratoCronogramaFinanceiro()
            {
                CdIss = novaParcela_.CdISS,
                CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00"),
                DtFaturamento = novaParcela_.DtFaturamento,
                IdContratoCliente = novaParcela_.Cliente.Id,
                IdContrato = novaParcela_.IdContrato,
                NuParcela = (short)novaParcela_.NuParcela,
                VlParcela = novaParcela_.Valor,
                IdSituacao = situacao,
                IdFrente = novaParcela_.Frente.Id,
                IcAtraso = atraso
            };

            db.ContratoCronogramaFinanceiro.Add(novoRegistro);
            db.SaveChanges();

            if (!novaParcela_.Entregaveis.ColecaoVazia())
                AtribuirEntregaveisParcela(novoRegistro.IdContratoCronFinanceiro, novaParcela_.Entregaveis.ToArray());
        }
        private void CriarNovaParcelaTemporaria(ParcelaDTO novaParcela_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == novaParcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            var idContratoTexto = novaParcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == novaParcela_.Cliente.Id && w.IdContrato == novaParcela_.IdContrato).FirstOrDefault();
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            int situacao = 0;
            bool atraso = false;

            if (novaParcela_.Situacao.Id != 94 && novaParcela_.Situacao.Id != 96)
            {
                if (novaParcela_.DtFaturamento != null)
                {
                    if (novaParcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                    {
                        atraso = true;
                        situacao = 93;
                    }
                    else
                    {
                        atraso = false;
                        situacao = novaParcela_.Situacao.Id;
                    }
                }
                else
                {
                    atraso = false;
                    situacao = novaParcela_.Situacao.Id;
                }
            }
            else
            {
                atraso = false;
                situacao = novaParcela_.Situacao.Id;
            }

            var novoRegistro = new ContratoCronogramaFinanceiroTemporaria()
            {
                CdIss = novaParcela_.CdISS,
                CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00"),
                DtFaturamento = novaParcela_.DtFaturamento,
                IdContratoCliente = novaParcela_.Cliente.Id,
                IdContrato = novaParcela_.IdContrato,
                NuParcela = (short)novaParcela_.NuParcela,
                VlParcela = novaParcela_.Valor,
                IdSituacao = situacao,
                IdFrente = novaParcela_.Frente.Id,
                IcAtraso = atraso
            };

            db.ContratoCronogramaFinanceiroTemporaria.Add(novoRegistro);
            db.SaveChanges();

            if (!novaParcela_.Entregaveis.ColecaoVazia())
                AtribuirEntregaveisParcelaTemporaria(novoRegistro.IdContratoCronFinanceiro, novaParcela_.Entregaveis.ToArray());
        }

        public void CriarNovaParcela(InputParcela novaParcela_)
        {
            ClienteDTO contratoCli = ObterCliente(novaParcela_.IdCliente, novaParcela_.IdContrato);
            var codigoFrente = db.Frente.Where(w => w.IdFrente == novaParcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            var idContratoTexto = novaParcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == novaParcela_.IdCliente && w.IdContrato == novaParcela_.IdContrato).FirstOrDefault();
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            int situacao = 0;
            bool atraso = false;
            if (novaParcela_.Situacao != 94 && novaParcela_.Situacao != 96)
            {
                if (novaParcela_.DtFaturamento != null)
                {
                    if (novaParcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                    {
                        atraso = true;
                        situacao = 93;
                    }
                    else
                    {
                        atraso = false;
                        situacao = novaParcela_.Situacao;
                    }
                }
                else
                {
                    atraso = false;
                    situacao = novaParcela_.Situacao;
                }
            }
            else
            {
                atraso = false;
                situacao = novaParcela_.Situacao;
            }

            var novoRegistro = new ContratoCronogramaFinanceiro()
            {
                CdIss = novaParcela_.CdISS,
                CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00"),
                DtFaturamento = novaParcela_.DtFaturamento,
                IdContratoCliente = novaParcela_.IdCliente,
                IdContrato = novaParcela_.IdContrato,
                NuParcela = (short)novaParcela_.NuParcela,
                VlParcela = novaParcela_.Valor,
                IdSituacao = situacao,
                IdFrente = novaParcela_.Frente.Id,
                IcAtraso = atraso,
                NuNotaFiscal = novaParcela_.NuNotaFiscal,
                DtNotaFiscal = novaParcela_.DtNotaFiscal,
                DsObservacao = novaParcela_.DsObservacao
            };

            db.ContratoCronogramaFinanceiro.Add(novoRegistro);
            db.SaveChanges();

            if (!novaParcela_.IdsEntregaveis.ColecaoVazia())
                AtribuirEntregaveisParcela(novoRegistro.IdContratoCronFinanceiro, novaParcela_.IdsEntregaveis);
        }

        public void CriarNovaParcelaTemporaria(InputParcela novaParcela_)
        {
            ClienteDTO contratoCli = ObterCliente(novaParcela_.IdCliente, novaParcela_.IdContrato);
            var codigoFrente = db.Frente.Where(w => w.IdFrente == novaParcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            var NuParcela = novaParcela_.NuParcela;
            var lstParcelasTemporarias = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == novaParcela_.IdContrato).OrderByDescending(w => w.IdContratoCronFinanceiro).ToList();
            string codParcelaNova = "00";

            if (lstParcelasTemporarias.Count > 0)
            {
                codParcelaNova = lstParcelasTemporarias.Where(w=>w.IdContratoCliente == novaParcela_.IdCliente).OrderBy(o => o.NuParcela).Last().CdParcela.Substring(11);
            }

            if (NuParcela == null)
            {
                if (novaParcela_.Situacao == 98)
                {
                    NuParcela = lstParcelasTemporarias[0].NuParcela + 1;
                }
                else
                {
                    var lstParcelasTemporariasDtInferiores = lstParcelasTemporarias.Where(w => w.DtFaturamento != null && w.DtFaturamento.Value.Date <= novaParcela_.DtFaturamento.Value.Date).ToList();
                    var lstParcelasTemporariasDtSuperiores = lstParcelasTemporarias.Where(w => w.DtFaturamento != null && w.DtFaturamento.Value.Date >= novaParcela_.DtFaturamento.Value.Date || w.IdSituacao == 98).ToList();

                    NuParcela = lstParcelasTemporariasDtInferiores[0].NuParcela + 1;
                    if (lstParcelasTemporariasDtSuperiores.Count > 0)
                    {
                        foreach (var parcelaSuperior in lstParcelasTemporariasDtSuperiores)
                        {
                            parcelaSuperior.NuParcela++;
                        }
                    }
                }

                db.SaveChanges();
            }
            var idContratoTexto = novaParcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == novaParcela_.IdCliente && w.IdContrato == novaParcela_.IdContrato).FirstOrDefault();
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            int situacao = 0;
            bool atraso = false;
            if (novaParcela_.Situacao != 94 && novaParcela_.Situacao != 96)
            {
                if (novaParcela_.DtFaturamento != null)
                {
                    if (novaParcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                    {
                        atraso = true;
                        situacao = 93;
                    }
                    else
                    {
                        atraso = false;
                        situacao = novaParcela_.Situacao;
                    }
                }
                else
                {
                    atraso = false;
                    situacao = novaParcela_.Situacao;
                }
            }
            else
            {
                atraso = false;
                situacao = novaParcela_.Situacao;
            }

            var novoRegistro = new ContratoCronogramaFinanceiroTemporaria()
            {
                CdIss = novaParcela_.CdISS,
                CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, codParcelaNova),
                DtFaturamento = novaParcela_.DtFaturamento,
                IdContratoCliente = novaParcela_.IdCliente,
                IdContrato = novaParcela_.IdContrato,
                NuParcela = (short)NuParcela,
                VlParcela = novaParcela_.Valor,
                IdSituacao = situacao,
                IdFrente = novaParcela_.Frente.Id,
                IcAtraso = atraso,
                NuNotaFiscal = novaParcela_.NuNotaFiscal,
                DtNotaFiscal = novaParcela_.DtNotaFiscal,
                DsObservacao = novaParcela_.DsObservacao
            };

            db.ContratoCronogramaFinanceiroTemporaria.Add(novoRegistro);
            db.SaveChanges();

            if (!novaParcela_.IdsEntregaveis.ColecaoVazia())
                AtribuirEntregaveisParcelaTemporaria(novoRegistro.IdContratoCronFinanceiro, novaParcela_.IdsEntregaveis);
        }

        public void AtualizarParcela(InputParcela parcelaAtualizada_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == parcelaAtualizada_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            ContratoCronogramaFinanceiro parcela = db.ContratoCronogramaFinanceiro.FirstOrDefault(_ => _.IdContratoCronFinanceiro == parcelaAtualizada_.Id);
            var idContratoTexto = parcelaAtualizada_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == parcelaAtualizada_.IdCliente && w.IdContrato == parcelaAtualizada_.IdContrato).FirstOrDefault();
            ClienteDTO contratoCli = ObterCliente(parcelaAtualizada_.IdCliente, parcelaAtualizada_.IdContrato);
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            if (parcela != null)
            {
                parcela.CdIss = parcelaAtualizada_.CdISS;
                parcela.CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00");
                parcela.DtFaturamento = parcelaAtualizada_.DtFaturamento;
                parcela.IdContratoCliente = parcelaAtualizada_.IdCliente;
                parcela.IdContrato = parcelaAtualizada_.IdContrato;
                parcela.NuParcela = (short)parcelaAtualizada_.NuParcela;
                parcela.VlParcela = parcelaAtualizada_.Valor;
                parcela.NuNotaFiscal = parcelaAtualizada_.NuNotaFiscal;
                parcela.DtNotaFiscal = parcelaAtualizada_.DtNotaFiscal;
                parcela.DsObservacao = parcelaAtualizada_.DsObservacao;
                if (parcelaAtualizada_.Situacao == 94 || parcelaAtualizada_.Situacao == 96)
                {
                    parcela.IcAtraso = false;
                    parcela.IdSituacao = parcelaAtualizada_.Situacao;
                }
                else
                {
                    if (parcelaAtualizada_.DtFaturamento != null)
                    {
                        if (parcelaAtualizada_.DtFaturamento.Value.Date < DateTime.Now.Date)
                        {
                            parcela.IcAtraso = true;
                            parcela.IdSituacao = 93;
                        }
                        else
                        {
                            parcela.IcAtraso = false;
                            parcela.IdSituacao = parcelaAtualizada_.Situacao != 93 ? parcelaAtualizada_.Situacao : 92;
                        }
                    }
                    else
                    {
                        parcela.IcAtraso = false;
                        parcela.IdSituacao = parcelaAtualizada_.Situacao;
                    }
                }
                parcela.IdFrente = parcelaAtualizada_.Frente.Id;

                db.SaveChanges();

                if (!parcelaAtualizada_.IdsEntregaveis.ColecaoVazia())
                    AtribuirEntregaveisParcela(parcela.IdContratoCronFinanceiro, parcelaAtualizada_.IdsEntregaveis);
                else
                    DeletarEntregaveisParcelas(parcela.IdContratoCronFinanceiro);
            }
        }

        public void AtualizarParcelaTemporaria(InputParcela parcelaAtualizada_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == parcelaAtualizada_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            ContratoCronogramaFinanceiroTemporaria parcela = db.ContratoCronogramaFinanceiroTemporaria.FirstOrDefault(_ => _.IdContratoCronFinanceiro == parcelaAtualizada_.Id);
            var idContratoTexto = parcelaAtualizada_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == parcelaAtualizada_.IdCliente && w.IdContrato == parcelaAtualizada_.IdContrato).FirstOrDefault();
            ClienteDTO contratoCli = ObterCliente(parcelaAtualizada_.IdCliente, parcelaAtualizada_.IdContrato);
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            if (parcela != null)
            {
                parcela.CdIss = parcelaAtualizada_.CdISS;
                parcela.CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00");
                parcela.DtFaturamento = parcelaAtualizada_.DtFaturamento;
                parcela.IdContratoCliente = parcelaAtualizada_.IdCliente;
                parcela.IdContrato = parcelaAtualizada_.IdContrato;
                parcela.NuParcela = (short)parcelaAtualizada_.NuParcela;
                parcela.VlParcela = parcelaAtualizada_.Valor;
                parcela.IdSituacao = parcelaAtualizada_.Situacao;
                parcela.NuNotaFiscal = parcelaAtualizada_.NuNotaFiscal;
                parcela.DtNotaFiscal = parcelaAtualizada_.DtNotaFiscal;
                parcela.DsObservacao = parcelaAtualizada_.DsObservacao;
                if (parcelaAtualizada_.Situacao == 94 || parcelaAtualizada_.Situacao == 96)
                {
                    parcela.IcAtraso = false;
                    parcela.IdSituacao = parcelaAtualizada_.Situacao;
                }
                else
                {
                    if (parcelaAtualizada_.DtFaturamento != null)
                    {
                        if (parcelaAtualizada_.DtFaturamento.Value.Date < DateTime.Now.Date)
                        {
                            parcela.IcAtraso = true;
                            parcela.IdSituacao = 93;
                        }
                        else
                        {
                            parcela.IcAtraso = false;
                            parcela.IdSituacao = parcelaAtualizada_.Situacao != 93 ? parcelaAtualizada_.Situacao : 92;
                        }
                    }
                    else
                    {
                        parcela.IcAtraso = false;
                        parcela.IdSituacao = parcelaAtualizada_.Situacao;
                    }
                }
                parcela.IdFrente = parcelaAtualizada_.Frente.Id;

                db.SaveChanges();

                if (!parcelaAtualizada_.IdsEntregaveis.ColecaoVazia())
                    AtribuirEntregaveisParcelaTemporaria(parcela.IdContratoCronFinanceiro, parcelaAtualizada_.IdsEntregaveis);
                else
                    DeletarEntregaveisParcelasTemporaria(parcela.IdContratoCronFinanceiro);
            }
        }

        public void AtualizarParcela(ParcelaDTO parcela_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == parcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            ContratoCronogramaFinanceiro parcela = db.ContratoCronogramaFinanceiro.FirstOrDefault(_ => _.IdContratoCronFinanceiro == parcela_.Id);
            var idContratoTexto = parcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == parcela_.Cliente.Id && w.IdContrato == parcela_.IdContrato).FirstOrDefault();
            ClienteDTO contratoCli = ObterCliente(parcela_.Cliente.Id, parcela_.IdContrato);
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            if (parcela != null)
            {
                parcela.CdIss = parcela_.CdISS;
                parcela.CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00");
                parcela.DtFaturamento = parcela_.DtFaturamento;
                parcela.IdContratoCliente = parcela_.Cliente.Id;
                parcela.IdContrato = parcela_.IdContrato;
                parcela.NuParcela = (short)parcela_.NuParcela;
                parcela.VlParcela = parcela_.Valor;
                parcela.NuNotaFiscal = parcela_.NuNotaFiscal;
                parcela.DtNotaFiscal = parcela_.DtNotaFiscal;
                parcela.DsObservacao = parcela_.DsObservacao;
                if (parcela_.Situacao.Id == 94 || parcela_.Situacao.Id == 96)
                {
                    parcela.IcAtraso = false;
                    parcela.IdSituacao = parcela_.Situacao.Id;
                }
                else
                {
                    if (parcela_.DtFaturamento != null)
                    {
                        if (parcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                        {
                            parcela.IdSituacao = 93;
                            parcela.IcAtraso = true;
                        }
                        else
                        {
                            if (parcela_.Situacao.Id == 93 || parcela_.Situacao.Id == 98)
                            {
                                parcela.IdSituacao = 92;
                                parcela.IcAtraso = false;
                            }
                            else
                            {
                                parcela.IdSituacao = parcela_.Situacao.Id;
                                parcela.IcAtraso = false;
                            }
                        }
                    }
                    else
                    {
                        parcela.IdSituacao = parcela_.Situacao.Id;
                        parcela.IcAtraso = false;
                    }
                }
                parcela.IdFrente = parcela_.Frente.Id;

                db.SaveChanges();

                //if (!parcela_.Entregaveis.ColecaoVazia())
                //    AtribuirEntregaveisParcela(parcela.IdContratoCronFinanceiro, parcela_.Entregaveis.ToArray());
                //else
                //    DeletarEntregaveisParcelas(parcela.IdContratoCronFinanceiro);
            }
        }

        public void AtualizarParcelaTemporaria(ParcelaDTO parcela_)
        {
            var codigoFrente = db.Frente.Where(w => w.IdFrente == parcela_.Frente.Id).FirstOrDefault().CdFrenteTexto;
            ContratoCronogramaFinanceiroTemporaria parcela = db.ContratoCronogramaFinanceiroTemporaria.FirstOrDefault(_ => _.IdContratoCronFinanceiro == parcela_.Id);
            var idContratoTexto = parcela_.IdContrato.ToString();
            while (idContratoTexto.Length < 4)
            {
                idContratoTexto = "0" + idContratoTexto;
            }
            var contratoCliente = db.ContratoCliente.Where(w => w.IdContratoCliente == parcela_.Cliente.Id && w.IdContrato == parcela_.IdContrato).FirstOrDefault();
            ClienteDTO contratoCli = ObterCliente(parcela_.Cliente.Id, parcela_.IdContrato);
            var nuContratante = contratoCliente.NuContratante.Value.ToString();
            while (nuContratante.Length < 2)
            {
                nuContratante = "0" + nuContratante;
            }

            if (parcela != null)
            {
                parcela.CdIss = parcela_.CdISS;
                parcela.CdParcela = string.Format(CODIGO_PARCELA, idContratoTexto, nuContratante, codigoFrente, "00");
                parcela.DtFaturamento = parcela_.DtFaturamento;
                parcela.IdContratoCliente = parcela_.Cliente.Id;
                parcela.IdContrato = parcela_.IdContrato;
                parcela.NuParcela = (short)parcela_.NuParcela;
                parcela.VlParcela = parcela_.Valor;
                parcela.IdSituacao = parcela_.Situacao.Id;
                parcela.NuNotaFiscal = parcela_.NuNotaFiscal;
                parcela.DtNotaFiscal = parcela_.DtNotaFiscal;
                parcela.DsObservacao = parcela_.DsObservacao;
                if (parcela_.Situacao.Id == 94 || parcela_.Situacao.Id == 96)
                {
                    parcela.IcAtraso = false;
                    parcela.IdSituacao = parcela_.Situacao.Id;
                }
                else
                {
                    if (parcela_.DtFaturamento != null)
                    {
                        if (parcela_.DtFaturamento.Value.Date < DateTime.Now.Date)
                        {
                            parcela.IdSituacao = 93;
                            parcela.IcAtraso = true;
                        }
                        else
                        {
                            if (parcela_.Situacao.Id == 93 || parcela_.Situacao.Id == 98)
                            {
                                parcela.IdSituacao = 92;
                                parcela.IcAtraso = false;
                            }
                            else
                            {
                                parcela.IdSituacao = parcela_.Situacao.Id;
                                parcela.IcAtraso = false;
                            }
                        }
                    }
                    else
                    {
                        parcela.IdSituacao = parcela_.Situacao.Id;
                        parcela.IcAtraso = false;
                    }
                }
                parcela.IdFrente = parcela_.Frente.Id;

                db.SaveChanges();

                //if (!parcela_.Entregaveis.ColecaoVazia())
                //    AtribuirEntregaveisParcelaTemporaria(parcela.IdContratoCronFinanceiro, parcela_.Entregaveis.ToArray());
                //else
                //    DeletarEntregaveisParcelasTemporaria(parcela.IdContratoCronFinanceiro);
            }
        }

        public List<ParcelaDTO> ObterParcelas(int idContrato_)
        {
            List<ContratoCronogramaFinanceiro> cronograma = db.ContratoCronogramaFinanceiro.Where(_ => _.IdContrato == idContrato_).ToList();
            List<ParcelaDTO> parcelas = null;


            //EGS 30.10.2020 Se não encontrou cronograma financeiro, tenta criar novamente
            if (cronograma.Count() == 0)
            {
                new bParcela(db).GerarCronogramaFinanceiro(idContrato_);
                cronograma = db.ContratoCronogramaFinanceiro.Where(_ => _.IdContrato == idContrato_).ToList();
            }

            if (cronograma != null)
            {
                parcelas = new List<ParcelaDTO>();

                for (int i = 0; i < cronograma.Count; i++)
                {
                    var entregaveis = ObterEntregaveisParcela(cronograma[i].IdContratoCronFinanceiro);

                    parcelas.Add(new ParcelaDTO()
                    {
                        CdISS              = cronograma[i].CdIss,
                        CdParcela          = cronograma[i].CdParcela,
                        Cliente            = ObterClientePagador(cronograma[i].IdContratoCliente, idContrato_),
                        DtFaturamento      = cronograma[i].DtFaturamento,
                        Entregaveis        = entregaveis.Select(_ => _.Id).ToList(),
                        NumerosEntregaveis = string.Join(',', entregaveis.Select(_ => _.Numero)),
                        Id                 = cronograma[i].IdContratoCronFinanceiro,
                        IdContrato         = cronograma[i].IdContrato,
                        NuParcela          = cronograma[i].NuParcela,
                        Valor              = cronograma[i].VlParcela,
                        NuNotaFiscal       = cronograma[i].NuNotaFiscal,
                        DtNotaFiscal       = cronograma[i].DtNotaFiscal,
                        DsObservacao       = cronograma[i].DsObservacao,
                        Situacao           = new bSituacao(db).ConsultarSituacao(cronograma[i].IdSituacao),
                        Frente             = new bFrente(db).ConsultarFrente(cronograma[i].IdFrente.ObterValorInteiro()),
                        IcAtraso           = cronograma[i].IcAtraso
                    });
                }
            }

            return parcelas?.OrderBy(_ => _.NuParcela).ToList();
        }

        public List<ParcelaDTO> ObterParcelasTemporaria(int idContrato_)
        {
            List<ContratoCronogramaFinanceiroTemporaria> cronograma = db.ContratoCronogramaFinanceiroTemporaria.Where(_ => _.IdContrato == idContrato_).ToList();
            List<ParcelaDTO> parcelas = null;

            if (cronograma != null)
            {
                parcelas = new List<ParcelaDTO>();

                for (int i = 0; i < cronograma.Count; i++)
                {
                    var entregaveis = ObterEntregaveisParcelaTemporaria(cronograma[i].IdContratoCronFinanceiro);

                    parcelas.Add(new ParcelaDTO()
                    {
                        CdISS = cronograma[i].CdIss,
                        CdParcela = cronograma[i].CdParcela,
                        Cliente = ObterCliente(cronograma[i].IdContratoCliente, idContrato_),
                        DtFaturamento = cronograma[i].DtFaturamento,
                        Entregaveis = entregaveis.Select(_ => _.Id).ToList(),
                        NumerosEntregaveis = string.Join(',', entregaveis.Select(_ => _.Numero)),
                        Id = cronograma[i].IdContratoCronFinanceiro,
                        IdContrato = cronograma[i].IdContrato,
                        NuParcela = cronograma[i].NuParcela,
                        Valor = cronograma[i].VlParcela,
                        NuNotaFiscal = cronograma[i].NuNotaFiscal,
                        DtNotaFiscal = cronograma[i].DtNotaFiscal,
                        DsObservacao = cronograma[i].DsObservacao,
                        Situacao = new bSituacao(db).ConsultarSituacao(cronograma[i].IdSituacao),
                        Frente = new bFrente(db).ConsultarFrente(cronograma[i].IdFrente.ObterValorInteiro()),
                        IcAtraso = cronograma[i].IcAtraso
                    });
                }
            }

            return parcelas?.OrderBy(_ => _.NuParcela).ToList();
        }

        public ParcelaDTO ConsultarParcela(int idParcela_)
        {
            var parcela = db.ContratoCronogramaFinanceiro.FirstOrDefault(_ => _.IdContratoCronFinanceiro == idParcela_);
            var entregaveis = ObterEntregaveisParcela(parcela.IdContratoCronFinanceiro);

            var parcelaDTO = new ParcelaDTO()
            {
                CdISS = parcela.CdIss,
                CdParcela = parcela.CdParcela,
                Cliente = parcela.IdContratoCliente != null ? ObterCliente(parcela.IdContratoCliente, parcela.IdContrato) : null,
                DtFaturamento = parcela.DtFaturamento,
                Entregaveis = entregaveis.Select(_ => _.Id).ToList(),
                NumerosEntregaveis = string.Join(',', entregaveis.Select(_ => _.Numero)),
                Id = parcela.IdContratoCronFinanceiro,
                IdContrato = parcela.IdContrato,
                NuParcela = parcela.NuParcela,
                NuNotaFiscal = parcela.NuNotaFiscal,
                DtNotaFiscal = parcela.DtNotaFiscal,
                DsObservacao = parcela.DsObservacao,
                Valor = parcela.VlParcela,
                Situacao = new bSituacao(db).ConsultarSituacao(parcela.IdSituacao),
                Frente = new bFrente(db).ConsultarFrente(parcela.IdFrente.ObterValorInteiro())
            };

            return parcelaDTO;
        }

        public ParcelaDTO ConsultarParcelaTemporaria(int idParcela_)
        {
            var parcela = db.ContratoCronogramaFinanceiroTemporaria.FirstOrDefault(_ => _.IdContratoCronFinanceiro == idParcela_);
            var entregaveis = ObterEntregaveisParcelaTemporaria(parcela.IdContratoCronFinanceiro);

            var parcelaDTO = new ParcelaDTO()
            {
                CdISS = parcela.CdIss,
                CdParcela = parcela.CdParcela,
                Cliente = ObterCliente(parcela.IdContratoCliente, parcela.IdContrato),
                DtFaturamento = parcela.DtFaturamento,
                Entregaveis = entregaveis.Select(_ => _.Id).ToList(),
                NumerosEntregaveis = string.Join(',', entregaveis.Select(_ => _.Numero)),
                Id = parcela.IdContratoCronFinanceiro,
                IdContrato = parcela.IdContrato,
                NuParcela = parcela.NuParcela,
                NuNotaFiscal = parcela.NuNotaFiscal,
                DtNotaFiscal = parcela.DtNotaFiscal,
                DsObservacao = parcela.DsObservacao,
                Valor = parcela.VlParcela,
                Situacao = new bSituacao(db).ConsultarSituacao(parcela.IdSituacao),
                Frente = new bFrente(db).ConsultarFrente(parcela.IdFrente.ObterValorInteiro())
            };

            return parcelaDTO;
        }

        public void ExcluirParcela(int idParcela_)
        {
            ContratoCronogramaFinanceiro parcela = db.ContratoCronogramaFinanceiro.Where(_ => _.IdContratoCronFinanceiro == idParcela_)?.FirstOrDefault();

            if (parcela != null)
            {
                var colParcelaEntregavel = db.ContratoParcelaEntregavel.Where(_ => _.IdParcela == parcela.IdContratoCronFinanceiro)?.ToList();

                if (colParcelaEntregavel != null)
                    colParcelaEntregavel.ForEach(_ => db.ContratoParcelaEntregavel.Remove(_));

                db.ContratoCronogramaFinanceiro.Remove(parcela);
                db.SaveChanges();
            }
        }
        public void ExcluirParcelaTemporaria(int idParcela_)
        {
            ContratoCronogramaFinanceiroTemporaria parcela = db.ContratoCronogramaFinanceiroTemporaria.Where(_ => _.IdContratoCronFinanceiro == idParcela_)?.FirstOrDefault();

            if (parcela != null)
            {
                var colParcelaEntregavel = db.ContratoParcelaEntregavelTemporaria.Where(_ => _.IdParcela == parcela.IdContratoCronFinanceiro)?.ToList();

                if (colParcelaEntregavel != null)
                    colParcelaEntregavel.ForEach(_ => db.ContratoParcelaEntregavelTemporaria.Remove(_));

                db.ContratoCronogramaFinanceiroTemporaria.Remove(parcela);
                db.SaveChanges();
            }
        }

        public bool ContratoPossuiFrente(int idContrato_)
        {
            try
            {
                var frentes = db.Frente.Where(_ => _.IdContrato == idContrato_);
                return frentes != null && frentes.Count() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ContratoPossuiPagador(int idContrato_)
        {
            try
            {
                var pagadores = ObterClientes(idContrato_);
                return pagadores.Count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ParcelaHistoricoDTO> ObterHistorico(int idAditivo_)
        {
            List<ContratoCronogramaFinanceiroHistorico> cronograma = db.ContratoCronogramaFinanceiroHistorico.Where(_ => _.IdContratoAditivo == idAditivo_).ToList();
            var idContrato = db.ContratoAditivo.Where(w => w.IdContratoAditivo == idAditivo_).FirstOrDefault().IdContrato;

            var listaHistoricoCronograma = new List<ParcelaHistoricoDTO>();

            foreach (var item in cronograma)
            {
                var historicoCronograma = new ParcelaHistoricoDTO();

                historicoCronograma.CdISS = item.CdIss;
                historicoCronograma.CdParcela = item.CdParcela;
                historicoCronograma.Cliente = ObterCliente(item.IdContratoCliente.GetValueOrDefault(), idContrato, true);
                historicoCronograma.DtFaturamento = item.DtFaturamento;
                historicoCronograma.Entregaveis = ObterEntregaveisParcelaHistorico(item.IdContratoCrongramaHistorico);
                historicoCronograma.NumerosEntregaveis = item.NuEntregaveis;
                historicoCronograma.Id = item.IdContratoCrongramaHistorico;
                historicoCronograma.IdAditivo = item.IdContratoAditivo.GetValueOrDefault();
                historicoCronograma.NuParcela = item.NuParcela;
                historicoCronograma.Valor = item.VlParcela;
                historicoCronograma.NuNotaFiscal = item.NuNotaFiscal;
                historicoCronograma.DtNotaFiscal = item.DtNotaFiscal;
                historicoCronograma.DsObservacao = item.DsObservacao;
                historicoCronograma.Situacao = new bSituacao(db).ConsultarSituacao(item.IdSituacao);

                listaHistoricoCronograma.Add(historicoCronograma);
            }

            return listaHistoricoCronograma.OrderBy(o => o.NuParcela).ToList();

            //return cronograma?.Select(_ => new ParcelaHistoricoDTO()
            //{
            //    CdISS = _.CdIss,
            //    CdParcela = _.CdParcela,
            //    Cliente = ObterCliente(_.IdContratoCliente.GetValueOrDefault(), idContrato, true),
            //    DtFaturamento = _.DtFaturamento,
            //    Entregaveis = ObterEntregaveisParcelaHistorico(_.IdContratoCrongramaHistorico),
            //    NumerosEntregaveis = string.Join(',', ObterEntregaveisParcelaHistorico(_.IdContratoCrongramaHistorico).Select(x => x.Numero)),
            //    Id = _.IdContratoCrongramaHistorico,
            //    IdAditivo = _.IdContratoAditivo.GetValueOrDefault(),
            //    NuParcela = _.NuParcela,
            //    Valor = _.VlParcela,
            //    Situacao = new bSituacao(db).ConsultarSituacao(_.IdSituacao),
            //}).OrderBy(_ => _.NuParcela).ToList();


        }

        public List<ParcelaHistoricoDTO> ObterHistoricoContratoReajuste(int idContratoReajuste_)
        {
            List<ContratoCronogramaFinanceiroHistorico> cronograma = db.ContratoCronogramaFinanceiroHistorico.Where(_ => _.IdContratoReajuste == idContratoReajuste_).ToList();
            var idContrato = db.ContratoReajuste.Where(w => w.IdContratoReajuste == idContratoReajuste_).FirstOrDefault().IdContrato;

            var listaHistoricoCronograma = new List<ParcelaHistoricoDTO>();

            foreach (var item in cronograma)
            {
                var historicoCronograma = new ParcelaHistoricoDTO();

                historicoCronograma.CdISS = item.CdIss;
                historicoCronograma.CdParcela = item.CdParcela;
                historicoCronograma.Cliente = ObterCliente(item.IdContratoCliente.GetValueOrDefault(), idContrato, true);
                historicoCronograma.DtFaturamento = item.DtFaturamento;
                historicoCronograma.Entregaveis = ObterEntregaveisParcelaHistorico(item.IdContratoCrongramaHistorico);
                historicoCronograma.NumerosEntregaveis = item.NuEntregaveis;
                historicoCronograma.Id = item.IdContratoCrongramaHistorico;
                historicoCronograma.IdAditivo = item.IdContratoAditivo.GetValueOrDefault();
                historicoCronograma.NuParcela = item.NuParcela;
                historicoCronograma.Valor = item.VlParcela;
                historicoCronograma.NuNotaFiscal = item.NuNotaFiscal;
                historicoCronograma.DtNotaFiscal = item.DtNotaFiscal;
                historicoCronograma.DsObservacao = item.DsObservacao;
                historicoCronograma.Situacao = new bSituacao(db).ConsultarSituacao(item.IdSituacao);

                listaHistoricoCronograma.Add(historicoCronograma);
            }

            return listaHistoricoCronograma.OrderBy(o => o.NuParcela).ToList();
        }

        public string ObterNumerosEntregaveisParcela(List<int> idEntregaveis_)
        {
            var numerosEntregaveis = db.ContratoEntregavel.Where(_ => idEntregaveis_.Contains(_.IdContratoEntregavel))?.Select(_ => _.VlOrdem).ToList();
            return numerosEntregaveis == null ? string.Empty : string.Join(',', numerosEntregaveis);
        }

        public List<ContratoCronogramaFinanceiro> ObterTodasParcelas()
        {
            var listaParcelas = db.ContratoCronogramaFinanceiro.ToList();

            return listaParcelas;
        }

        public List<ContratoCronogramaFinanceiroTemporaria> ObterTodasParcelasTemporaria()
        {
            var listaParcelas = db.ContratoCronogramaFinanceiroTemporaria.ToList();

            return listaParcelas;
        }

        #endregion

        #region || Métodos Privados

        private int ObterFrente(int idEntregavel_)
        {
            var frente = db.ContratoEntregavel.Where(_ => _.IdContratoEntregavel == idEntregavel_)?.FirstOrDefault();
            return frente == null ? 0 : frente.IdFrente;
        }

        public List<ClienteDTO> ObterClientes(int idContrato_)
        {
            var clientes    = new List<ClienteDTO>();
            var contrato    = new bContrato(db).GetContratoById(idContrato_);
            var cdsClientes = contrato.ClientesPagadores;
            cdsClientes.ForEach(_ => clientes.Add(ObterCliente(_, idContrato_)));

            return clientes;
        }


        public List<ClienteDTO> ObterClientesPagador(int idContrato_)
        {
            var clientes = new List<ClienteDTO>();
            var contrato = new bContrato(db).GetContratoById(idContrato_);
            var cdsClientes = contrato.ClientesPagadores;

            cdsClientes.ForEach(_ => clientes.Add(ObterClientePagador(_, idContrato_)));

            return clientes;
        }

        private string ObterCodigoISS(int idContrato_)
        {
            var contrato = new bContrato(db).GetContratoById(idContrato_);
            return contrato == null ? string.Empty : contrato.CdIss.ObterValorTexto();
        }

        public decimal ObterValorContrato(int idContrato_)
        {
            var contrato = new bContrato(db).GetContratoById(idContrato_);
            return contrato == null ? 0 : contrato.VlContrato;
        }

        private ClienteDTO ObterCliente(int idCliente_, int idContrato_, bool historicoAditivo_ = false)
        {
            var clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_ && _.IdContrato == idContrato_)?.FirstOrDefault();
            var nmCliente       = "";
            if (clienteContrato.NmFantasia == null)
            {
                var cliente      = new bCliente(db).BuscarClienteId(clienteContrato.IdCliente);
                var pessoa       = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(pessoa.IdPessoaFisica.Value);
                nmCliente        = pessoaFisica.NmPessoa;
            }
            else
            {
                nmCliente        = clienteContrato.NmFantasia;
            }

            if (clienteContrato == null)
                clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_ && _.IdContrato == idContrato_)?.FirstOrDefault();

            if (historicoAditivo_)
                clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_)?.FirstOrDefault();

            return new ClienteDTO()
            {
                Id            = clienteContrato.IdContratoCliente,
                NmCliente     = nmCliente,
                CodigoCliente = clienteContrato.NuContratante
            };
        }



        private ClienteDTO ObterClientePagador(int idCliente_, int idContrato_, bool historicoAditivo_ = false)
        {
            //EGS 30.10.2020 Verifica se o cliente esta marcado como PAGADOR
            var clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_ && _.IdContrato == idContrato_ && _.IcPagador==true)?.FirstOrDefault();
            var nmCliente       = "";
            if (clienteContrato.NmFantasia == null)
            {
                var cliente      = new bCliente(db).BuscarClienteId(clienteContrato.IdCliente);
                var pessoa       = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(pessoa.IdPessoaFisica.Value);
                nmCliente        = pessoaFisica.NmPessoa;
            } else {
                nmCliente        = clienteContrato.NmFantasia;
            }

            if (clienteContrato == null)
                clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_ && _.IdContrato == idContrato_ && _.IcPagador == true)?.FirstOrDefault();

            if (historicoAditivo_)
                clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_)?.FirstOrDefault();

            return new ClienteDTO()
            {
                Id            = clienteContrato.IdContratoCliente,
                NmCliente     = nmCliente,
                CodigoCliente = clienteContrato.NuContratante
            };
        }



        private List<EntregavelResumoDTO> ObterEntregaveisParcela(int idParcela_)
        {
            //var entregaveis = new List<EntregavelDTO>();
            //var parcelaEntregaveis = db.ContratoParcelaEntregavel.Where(_ => _.IdParcela == idParcela_)?.ToList();

            //if (parcelaEntregaveis != null)
            //{
            //    var bEntregavel = new bEntregavel(db);

            //    foreach (ContratoParcelaEntregavel item in parcelaEntregaveis)
            //    {
            //        entregaveis.Add(bEntregavel.ConsultarEntregavel(item.IdEntregavel));
            //    }
            //}

            //return entregaveis;

            var entregaveis = (from cpe in db.ContratoParcelaEntregavel
                               join ce in db.ContratoEntregavel on cpe.IdEntregavel equals ce.IdContratoEntregavel
                               where cpe.IdParcela == idParcela_
                               select new EntregavelResumoDTO()
                               {
                                   Id = ce.IdContratoEntregavel,
                                   Numero = ce.VlOrdem
                               })?.AsParallel().ToList();
            return entregaveis;
        }

        private List<EntregavelResumoDTO> ObterEntregaveisParcelaTemporaria(int idParcela_)
        {
            //var entregaveis = new List<EntregavelDTO>();
            //var parcelaEntregaveis = db.ContratoParcelaEntregavel.Where(_ => _.IdParcela == idParcela_)?.ToList();

            //if (parcelaEntregaveis != null)
            //{
            //    var bEntregavel = new bEntregavel(db);

            //    foreach (ContratoParcelaEntregavel item in parcelaEntregaveis)
            //    {
            //        entregaveis.Add(bEntregavel.ConsultarEntregavel(item.IdEntregavel));
            //    }
            //}

            //return entregaveis;

            var entregaveis = (from cpe in db.ContratoParcelaEntregavelTemporaria
                               join ce in db.ContratoEntregavelTemporaria on cpe.IdEntregavel equals ce.IdContratoEntregavel
                               where cpe.IdParcela == idParcela_
                               select new EntregavelResumoDTO()
                               {
                                   Id = ce.IdContratoEntregavel,
                                   Numero = ce.VlOrdem
                               })?.AsParallel().ToList();
            return entregaveis;
        }
        private List<EntregavelHistoricoDTO> ObterEntregaveisParcelaHistorico(long idParcela_)
        {
            //var entregaveis = new List<EntregavelHistoricoDTO>();
            //var parcelaEntregaveis = db.ContratoParcelaEntregavel.Where(_ => _.IdParcela == idParcela_)?.ToList();

            //if (parcelaEntregaveis != null)
            //{
            //    var bEntregavel = new bEntregavel(db);

            //    foreach (ContratoParcelaEntregavel item in parcelaEntregaveis)
            //    {
            //        entregaveis.Add(bEntregavel.ConsultarEntregavel(item.IdEntregavel));
            //    }
            //}

            var entregaveis = (from cpe in db.ContratoParcelaEntregavel
                               join ce in db.ContratoEntregavel on cpe.IdEntregavel equals ce.IdContratoEntregavel
                               where cpe.IdParcela == idParcela_
                               select new EntregavelHistoricoDTO()
                               {
                                   Id = ce.IdContratoEntregavel,
                                   Numero = ce.VlOrdem
                               })?.AsParallel().ToList();
            return entregaveis;

        }

        private void AtribuirEntregaveisParcela(int idParcela_, int[] idsEntregaveis_)
        {
            DeletarEntregaveisParcelas(idParcela_);

            foreach (int idEntregavel in idsEntregaveis_)
            {
                db.ContratoParcelaEntregavel.Add(new ContratoParcelaEntregavel()
                {
                    IdEntregavel = idEntregavel,
                    IdParcela = idParcela_
                });
            }

            db.SaveChanges();
        }

        private void AtribuirEntregaveisParcelaTemporaria(int idParcela_, int[] idsEntregaveis_)
        {
            DeletarEntregaveisParcelasTemporaria(idParcela_);

            foreach (int idEntregavel in idsEntregaveis_)
            {
                db.ContratoParcelaEntregavelTemporaria.Add(new ContratoParcelaEntregavelTemporaria()
                {
                    IdEntregavel = idEntregavel,
                    IdParcela = idParcela_
                });
            }

            db.SaveChanges();
        }

        private void DeletarEntregaveisParcelas(int idParcela_)
        {
            var entregaveisParcela = db.ContratoParcelaEntregavel.Where(_ => _.IdParcela == idParcela_)?.ToList();

            if (entregaveisParcela.Count > 0)
            {
                db.ContratoParcelaEntregavel.RemoveRange(entregaveisParcela);
                db.SaveChanges();
            }
        }

        private void DeletarEntregaveisParcelasTemporaria(int idParcela_)
        {
            var entregaveisParcela = db.ContratoParcelaEntregavelTemporaria.Where(_ => _.IdParcela == idParcela_)?.ToList();

            if (entregaveisParcela.Count > 0)
            {
                db.ContratoParcelaEntregavelTemporaria.RemoveRange(entregaveisParcela);
                db.SaveChanges();
            }
        }

        private string FormatarNumeroAditivo(int numeroAditivo_)
        {
            return numeroAditivo_ < 10 ? string.Format("0{0}", numeroAditivo_) : numeroAditivo_.ToString();
        }

        private string FormatarNumeroContratante(int? numero_)
        {
            return numero_.HasValue ? (numero_ < 10 ? string.Format("0{0}", numero_) : numero_.ToString()) : "00";
        }

        #endregion

        #endregion
    }
}
