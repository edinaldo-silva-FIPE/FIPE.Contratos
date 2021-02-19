using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContratoReajuste
    {
        #region | Propriedades

        public FIPEContratosContext db { get; set; }

        #endregion

        #region | Construtores

        public bContratoReajuste(FIPEContratosContext db)
        {
            this.db = db;
        }

        #endregion

        #region | Métodos

        public OutPutGetContratoReajuste Consultar(int idContratoReajuste_)
        {
            var retorno = new OutPutGetContratoReajuste();
            var contratoReajusteEncontrado = db.ContratoReajuste.FirstOrDefault(_ => _.IdContratoReajuste == idContratoReajuste_);
            var lstContratoCronogramaFinanceiroPendentesTemporaria = new List<ContratoCronogramaFinanceiroTemporaria>();
            retorno.VlTotalParcelasPendentes = 0;

            if (contratoReajusteEncontrado != null)
            {
                var contrato = db.Contrato.Where(w => w.IdContrato == contratoReajusteEncontrado.IdContrato).FirstOrDefault();
                if (string.IsNullOrEmpty(contrato.DtProxReajuste.ToString()))
                {
                    lstContratoCronogramaFinanceiroPendentesTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato && w.IdSituacao != 94 && w.IdSituacao != 96 && w.DtFaturamento != null).ToList();
                }
                else
                {
                    lstContratoCronogramaFinanceiroPendentesTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato && w.IdSituacao != 94 && w.IdSituacao != 96 && w.DtFaturamento != null && w.DtFaturamento.Value.Date >= contrato.DtProxReajuste.Value.Date).ToList();
                }
               
                foreach (var parcela in lstContratoCronogramaFinanceiroPendentesTemporaria)
                {
                    retorno.VlTotalParcelasPendentes += parcela.VlParcela;
                }
                var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == contrato.IdIndiceReajuste).FirstOrDefault();
                var situacao = db.Situacao.Where(w => w.IdSituacao == contratoReajusteEncontrado.IdSituacao).FirstOrDefault();
                var usuario = db.Usuario.Where(w => w.IdUsuario == contrato.IdUsuarioCriacao)
                    .Include(w => w.IdPessoaNavigation)
                    .FirstOrDefault();

                retorno.DataMinima = contrato.DtFim;
                retorno.IdProposta = contrato.IdProposta;
                retorno.IdContrato = contrato.IdContrato;
                retorno.NuContratoEdit = contrato.NuContratoEdit;
                retorno.DataCriacao = contrato.DtCriacao;
                retorno.CriadoPor = usuario.IdPessoaNavigation.NmPessoa;
                retorno.DsIndiceReajuste = indiceReajuste != null ? indiceReajuste.DsIndiceReajuste : string.Empty;
                retorno.DsSituacao = situacao != null ? situacao.DsSituacao : string.Empty;
                retorno.IcReajusteConcluido = retorno.DsSituacao == "Aplicado" ? true : false;
                retorno.DtInicio = contrato.DtInicio.Value;
                retorno.DtFim = contrato.DtFim.Value;
                retorno.DtReajuste = contratoReajusteEncontrado.DtReajuste;
                retorno.PcReajuste = contratoReajusteEncontrado.PcReajuste;
                retorno.IcReajusteConcluido = contratoReajusteEncontrado.IcReajusteConcluido;
                retorno.VlContratoAntesReajuste = contratoReajusteEncontrado.VlContratoAntesReajuste;
                retorno.VlContratoReajustado = contratoReajusteEncontrado.VlContratoReajustado;
                retorno.DtProxReajuste = contratoReajusteEncontrado.DtProxReajuste;
                retorno.VlReajuste = contratoReajusteEncontrado.VlReajuste;
                retorno.VlReajusteAcumulado = contratoReajusteEncontrado.VlReajusteAcumulado;
                retorno.NuReajuste = contratoReajusteEncontrado.NuReajuste.Value;
            }

            return retorno;
        }

        public List<OutPutGetContratoReajuste> GetAllByIdContrato(int idContrato_)
        {
            var lstRetorno = new List<OutPutGetContratoReajuste>();
            var lstContratoReajuste = db.ContratoReajuste.Where(_ => _.IdContrato == idContrato_).OrderBy(w => w.IdContratoReajuste).ToList();
            if (lstContratoReajuste.Count > 0)
            {
                foreach (var contratoReajusteEncontrado in lstContratoReajuste)
                {
                    var contrato = db.Contrato.Where(w => w.IdContrato == contratoReajusteEncontrado.IdContrato).FirstOrDefault();
                    var indiceReajuste = db.IndiceReajuste.Where(w => w.IdIndiceReajuste == contrato.IdIndiceReajuste).FirstOrDefault();
                    var situacao = db.Situacao.Where(w => w.IdSituacao == contratoReajusteEncontrado.IdSituacao).FirstOrDefault();
                    var usuario = db.Usuario.Where(w => w.IdUsuario == contrato.IdUsuarioCriacao)
                        .Include(w => w.IdPessoaNavigation)
                        .FirstOrDefault();
                    var retorno = new OutPutGetContratoReajuste();

                    retorno.IdContratoReajuste = contratoReajusteEncontrado.IdContratoReajuste;
                    retorno.DataMinima = contrato.DtFim;
                    retorno.IdProposta = contrato.IdProposta;
                    retorno.IdContrato = contrato.IdContrato;
                    retorno.NuContratoEdit = contrato.NuContratoEdit;
                    retorno.DataCriacao = contrato.DtCriacao;
                    retorno.CriadoPor = usuario.IdPessoaNavigation.NmPessoa;
                    retorno.DsIndiceReajuste = indiceReajuste != null ? indiceReajuste.DsIndiceReajuste : string.Empty;
                    retorno.DsSituacao = situacao != null ? situacao.DsSituacao : string.Empty;
                    retorno.IcReajusteConcluido = retorno.DsSituacao == "Aplicado" ? true : false;
                    retorno.DtInicio = contrato.DtInicio.Value;
                    retorno.DtFim = contrato.DtFim.Value;
                    retorno.DtReajuste = contratoReajusteEncontrado.DtReajuste;
                    retorno.PcReajuste = contratoReajusteEncontrado.PcReajuste;
                    retorno.IcReajusteConcluido = contratoReajusteEncontrado.IcReajusteConcluido;
                    retorno.VlContratoAntesReajuste = contratoReajusteEncontrado.VlContratoAntesReajuste;
                    retorno.VlContratoReajustado = contratoReajusteEncontrado.VlContratoReajustado;
                    retorno.DtProxReajuste = contratoReajusteEncontrado.DtProxReajuste;
                    retorno.VlReajuste = contratoReajusteEncontrado.VlReajuste;
                    retorno.VlReajusteAcumulado = contratoReajusteEncontrado.VlReajusteAcumulado;
                    retorno.NuReajuste = contratoReajusteEncontrado.NuReajuste.Value;

                    lstRetorno.Add(retorno);
                }
            }

            return lstRetorno;
        }

        public void Atualizar(InputUpdateContratoReajuste contratoReajuste_)
        {
            var reajuste = db.ContratoReajuste.FirstOrDefault(_ => _.IdContratoReajuste == contratoReajuste_.IdContratoReajuste);
            reajuste.DtReajuste = contratoReajuste_.DtReajuste;
            reajuste.PcReajuste = contratoReajuste_.PcReajuste;
            reajuste.VlContratoReajustado = contratoReajuste_.VlContratoReajustado;
            reajuste.DtProxReajuste = contratoReajuste_.DtProxReajuste;

            db.SaveChanges();
        }

        public decimal? CalcularValorContratoReajustado(int idContrato, decimal? pcReajuste)
        {
            var contrato = db.Contrato.Where(w => w.IdContrato == idContrato).FirstOrDefault();
            var lstContratoCronogramaFinanceiroPendentesTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == idContrato && w.IdSituacao != 94 && w.IdSituacao != 96 && w.DtFaturamento != null && w.DtFaturamento.Value.Date >= contrato.DtProxReajuste.Value.Date).ToList();
            decimal? valorContratoReajustado = 0;
            if (lstContratoCronogramaFinanceiroPendentesTemporaria.Count > 0)
            {
                decimal? valorTotalParcelas = lstContratoCronogramaFinanceiroPendentesTemporaria.Sum(w => w.VlParcela);
                var valorTotalParcelasReajustadas = valorTotalParcelas * (1 + pcReajuste / 100);
                var valorPorParcela = valorTotalParcelasReajustadas / lstContratoCronogramaFinanceiroPendentesTemporaria.Count;
                var valorPorParcelaString = valorPorParcela.ToString().Split(',');
                var valorPorParcelaTexto = valorPorParcelaString[0] + "," + valorPorParcelaString[1].Substring(0, 2);
                valorPorParcela = Convert.ToDecimal(valorPorParcelaTexto);

                foreach (var parcela in lstContratoCronogramaFinanceiroPendentesTemporaria)
                {
                    parcela.VlParcela = valorPorParcela.Value;
                }
                valorContratoReajustado = (contrato.VlContrato - valorTotalParcelas) + lstContratoCronogramaFinanceiroPendentesTemporaria.Sum(w => w.VlParcela);
            }
            else
            {
                valorContratoReajustado = contrato.VlContrato;
            }

            return valorContratoReajustado;
        }

        public void AplicarContratoReajuste(int idContratoReajuste_)
        {
            var reajuste = db.ContratoReajuste.FirstOrDefault(_ => _.IdContratoReajuste == idContratoReajuste_);
            var lstContratoCronogramaFinanceiroPendentesTemporaria = new List<ContratoCronogramaFinanceiroTemporaria>();
            reajuste.IdSituacao = 101;
            reajuste.DtAplicacao = DateTime.Now;

            db.SaveChanges();

            var contrato = db.Contrato.FirstOrDefault(_ => _.IdContrato == reajuste.IdContrato);
            if (string.IsNullOrEmpty(contrato.DtProxReajuste.Value.ToString()))
            {
                lstContratoCronogramaFinanceiroPendentesTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato && w.IdSituacao != 94 && w.IdSituacao != 96 && w.DtFaturamento != null).ToList();
            }
            else
            {
                lstContratoCronogramaFinanceiroPendentesTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato && w.IdSituacao != 94 && w.IdSituacao != 96 && w.DtFaturamento != null && w.DtFaturamento.Value.Date >= contrato.DtProxReajuste.Value.Date).ToList();
            }
            
            var lstContratoCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == contrato.IdContrato).ToList();
            var diferencaReajuste = reajuste.VlContratoReajustado - reajuste.VlContratoAntesReajuste;
            var diferencaReajusteString = diferencaReajuste.ToString().Split(',');
            var diferencaReajusteTexto = diferencaReajusteString[0] + "," + diferencaReajusteString[1].Substring(0, 2);
            diferencaReajuste = Convert.ToDecimal(diferencaReajusteTexto);
            var somaParcela = diferencaReajuste / lstContratoCronogramaFinanceiroPendentesTemporaria.Count;
            var somaParcelaString = somaParcela.ToString().Split(',');
            var somaParcelaTexto = somaParcelaString[0] + "," + somaParcelaString[1].Substring(0, 2);
            somaParcela = Convert.ToDecimal(somaParcelaTexto);
            //decimal? valorTotalParcelas = somaParcela * lstContratoCronogramaFinanceiroPendentesTemporaria.Count;
            //decimal? valorDiferencaUltimaParcela;
            //if (diferencaReajuste < 0)
            //{
            //    valorDiferencaUltimaParcela = diferencaReajuste + Math.Abs(valorTotalParcelas.Value);
            //}
            //else
            //{
            //    valorDiferencaUltimaParcela = diferencaReajuste - valorTotalParcelas;
            //}

            foreach (var parcela in lstContratoCronogramaFinanceiro)
            {
                var lstContratoParcelaEntregavel = db.ContratoParcelaEntregavel.Where(w => w.IdParcela == parcela.IdContratoCronFinanceiro).ToList();

                var newHistoricoCronograma = new ContratoCronogramaFinanceiroHistorico();
                if (lstContratoParcelaEntregavel.Count > 0)
                {
                    foreach (var parcEntregavel in lstContratoParcelaEntregavel)
                    {
                        var entregavel = db.ContratoEntregavel.Where(w => w.IdContratoEntregavel == parcEntregavel.IdEntregavel).FirstOrDefault();
                        if (entregavel != null)
                        {
                            newHistoricoCronograma.NuEntregaveis += entregavel.VlOrdem + ", ";
                        }
                    }

                    newHistoricoCronograma.NuEntregaveis = newHistoricoCronograma.NuEntregaveis.Substring(0, newHistoricoCronograma.NuEntregaveis.Length - 2);
                }

                newHistoricoCronograma.IdContratoReajuste = reajuste.IdContratoReajuste;
                newHistoricoCronograma.CdIss = parcela.CdIss;
                newHistoricoCronograma.CdParcela = parcela.CdParcela;
                newHistoricoCronograma.DsTextoCorpoNf = parcela.DsTextoCorpoNf;
                newHistoricoCronograma.DtFaturamento = parcela.DtFaturamento;
                newHistoricoCronograma.DtNotaFiscal = parcela.DtNotaFiscal;
                newHistoricoCronograma.IdContratoCliente = parcela.IdContratoCliente;
                newHistoricoCronograma.IdSituacao = parcela.IdSituacao;
                newHistoricoCronograma.NuNotaFiscal = parcela.NuNotaFiscal;
                newHistoricoCronograma.NuParcela = parcela.NuParcela;
                newHistoricoCronograma.VlParcela = parcela.VlParcela;
                newHistoricoCronograma.IdFrente = parcela.IdFrente;
                newHistoricoCronograma.DsObservacao = parcela.DsObservacao;

                db.ContratoCronogramaFinanceiroHistorico.Add(newHistoricoCronograma);

                db.SaveChanges();
            }

            //var i = 0;
            foreach (var parcelaTemp in lstContratoCronogramaFinanceiroPendentesTemporaria)
            {
                //i++;
                parcelaTemp.VlParcela += somaParcela.Value;
                //if (i == lstContratoCronogramaFinanceiroPendentesTemporaria.Count)
                //{
                //    parcelaTemp.VlParcela += valorDiferencaUltimaParcela.Value;
                //}
                // Retorna somente os dois ultimos caracteres do Código da Parcela
                string codigoReajusteParcela = parcelaTemp.CdParcela.Substring(parcelaTemp.CdParcela.Length - 2, 2);
                // Remove os dois ultimos caracteres do Código da Parcela
                parcelaTemp.CdParcela = parcelaTemp.CdParcela.Remove(parcelaTemp.CdParcela.Length - 2, 2);
                // Soma 1 nos dois ultimos caracteres do Código da Parcela
                int novoCdReajusteParcela = Convert.ToInt32(codigoReajusteParcela) + 1;
                codigoReajusteParcela = novoCdReajusteParcela.ToString().Length > 1 ? novoCdReajusteParcela.ToString() : "0" + novoCdReajusteParcela.ToString();
                parcelaTemp.CdParcela += codigoReajusteParcela;

                db.SaveChanges();
            }
        }

        public void ConcluirContratoReajuste(InputConcluirReajuste concluirReajuste_)
        {
            var contratoReajuste = db.ContratoReajuste.Where(w => w.IdContratoReajuste == concluirReajuste_.IdContratoReajuste).FirstOrDefault();
            var contrato = db.Contrato.Where(w => w.IdContrato == contratoReajuste.IdContrato).FirstOrDefault();
            var lstContratoReajustes = db.ContratoReajuste.Where(w => w.IdContrato == contratoReajuste.IdContrato && w.IdContratoReajuste != contratoReajuste.IdContratoReajuste).ToList();
            decimal? valorReajusteAcumulado = 0;
            if (lstContratoReajustes.Count > 0)
            {
                foreach (var reajuste in lstContratoReajustes)
                {
                    if (reajuste.VlReajuste != null)
                    {
                        valorReajusteAcumulado += reajuste.VlReajuste.Value;
                    }
                }
            }
            contratoReajuste.VlReajuste = contratoReajuste.VlContratoReajustado - contratoReajuste.VlContratoAntesReajuste;
            if (valorReajusteAcumulado != 0)
            {
                contratoReajuste.VlReajusteAcumulado = valorReajusteAcumulado + contratoReajuste.VlReajuste;
            }
            else
            {
                contratoReajuste.VlReajusteAcumulado = contratoReajuste.VlReajuste;
            }
            contrato.VlContrato = concluirReajuste_.VlTotalParcelas.Value;
            contrato.DtProxReajuste = concluirReajuste_.DtProxReajuste;
            contratoReajuste.DtProxReajuste = concluirReajuste_.DtProxReajuste;
            contratoReajuste.IcReajusteConcluido = true;
            contratoReajuste.VlContratoReajustado = concluirReajuste_.VlTotalParcelas.Value;

            db.SaveChanges();

            var lstContratoCronogramaFinanceiroTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato).ToList();

            foreach (var parcelaTemp in lstContratoCronogramaFinanceiroTemporaria)
            {
                if (parcelaTemp.IdParcela != null)
                {
                    var contratoCronogramaFinanceiro = db.ContratoCronogramaFinanceiro.Where(w => w.IdContratoCronFinanceiro == parcelaTemp.IdParcela).FirstOrDefault();
                    contratoCronogramaFinanceiro.NuParcela = parcelaTemp.NuParcela;
                    contratoCronogramaFinanceiro.VlParcela = parcelaTemp.VlParcela;
                    contratoCronogramaFinanceiro.CdParcela = parcelaTemp.CdParcela;
                }
                else
                {
                    var contratoCronogramaFinanceiro = new ContratoCronogramaFinanceiro();
                    contratoCronogramaFinanceiro.CdIss = parcelaTemp.CdIss;
                    contratoCronogramaFinanceiro.CdParcela = parcelaTemp.CdParcela;
                    contratoCronogramaFinanceiro.DsTextoCorpoNf = parcelaTemp.DsTextoCorpoNf;
                    contratoCronogramaFinanceiro.DtFaturamento = parcelaTemp.DtFaturamento;
                    contratoCronogramaFinanceiro.DtNotaFiscal = parcelaTemp.DtNotaFiscal;
                    contratoCronogramaFinanceiro.IdContratoCliente = parcelaTemp.IdContratoCliente;
                    contratoCronogramaFinanceiro.IdSituacao = parcelaTemp.IdSituacao;
                    contratoCronogramaFinanceiro.NuNotaFiscal = parcelaTemp.NuNotaFiscal;
                    contratoCronogramaFinanceiro.NuParcela = parcelaTemp.NuParcela;
                    contratoCronogramaFinanceiro.VlParcela = parcelaTemp.VlParcela;
                    contratoCronogramaFinanceiro.IcAtraso = parcelaTemp.IcAtraso;
                    contratoCronogramaFinanceiro.IdContrato = parcelaTemp.IdContrato;
                    contratoCronogramaFinanceiro.IdFrente = parcelaTemp.IdFrente;

                    db.ContratoCronogramaFinanceiro.Add(contratoCronogramaFinanceiro);
                }

                db.SaveChanges();

                // Após copiar a Parcela , exclui da tabela Temporaria
                db.ContratoCronogramaFinanceiroTemporaria.Remove(parcelaTemp);
                db.SaveChanges();
            }

            db.SaveChanges();

        }

        #region || Métodos Privados

        private Contrato ObterContrato(int idContrato_)
        {
            return db.Contrato.FirstOrDefault(_ => _.IdContrato == idContrato_);
        }

        private string ObterNomeUsuario(int idUsuario_)
        {
            string nomeUsuario = string.Empty;
            var usuario = db.Usuario.Where(w => w.IdUsuario == idUsuario_).FirstOrDefault();
            nomeUsuario = db.PessoaFisica.Where(w => w.IdPessoaFisica == usuario.IdPessoa).FirstOrDefault().NmPessoa;

            return nomeUsuario;
        }

        private string ObterSituacao(int idSituacao_)
        {
            return db.Situacao.FirstOrDefault(_ => _.IdSituacao == idSituacao_)?.DsSituacao;
        }

        #endregion


        #endregion

        #region Retornos
        public class OutPutGetContratoReajuste
        {
            public int IdProposta { get; set; }
            public int IdContratoReajuste { get; set; }
            public int IdContrato { get; set; }
            public string NuContratoEdit { get; set; }
            public DateTime? DataCriacao { get; set; }
            public string CriadoPor { get; set; }
            public DateTime DtInicio { get; set; }
            public DateTime DtFim { get; set; }
            public decimal? VlContratoAntesReajuste { get; set; }
            //public bool IcReajuste { get; set; }
            public string DsIndiceReajuste { get; set; }
            public DateTime? DtReajuste { get; set; }
            public decimal? PcReajuste { get; set; }
            public decimal? VlContratoReajustado { get; set; }
            public DateTime? DtProxReajuste { get; set; }
            public string DsSituacao { get; set; }
            public bool? IcReajusteConcluido { get; set; }
            public DateTime? DataMinima { get; set; }
            public decimal? VlReajuste { get; set; }
            public decimal? VlReajusteAcumulado { get; set; }
            public short NuReajuste { get; set; }
            public decimal? VlTotalParcelasPendentes { get; set; }
        }

        public class InputUpdateContratoReajuste
        {
            public int IdContratoReajuste { get; set; }
            public DateTime DtReajuste { get; set; }
            public decimal PcReajuste { get; set; }
            public decimal VlContratoReajustado { get; set; }
            public DateTime? DtProxReajuste { get; set; }
        }

        public class InputConcluirReajuste
        {
            public int IdContratoReajuste { get; set; }
            public decimal? VlTotalParcelas { get; set; }
            public DateTime? DtProxReajuste { get; set; }
        }
        #endregion
    }
}
