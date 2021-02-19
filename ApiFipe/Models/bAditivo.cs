using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using ApiFipe.Utilitario.Extensoes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bAditivo
    {
        #region | Propriedades

        public FIPEContratosContext db { get; set; }

        #endregion

        #region | Construtores

        public bAditivo(FIPEContratosContext db)
        {
            this.db = db;
        }

        #endregion

        #region | Métodos

        public List<TipoAditivoDTO> ObterTipos()
        {
            return db.TipoAditivo.Select(_ => new TipoAditivoDTO()
            {
                Id = _.IdTipoAditivo,
                Nome = _.DsTipoAditivo
            }).ToList();
        }

        public AditivoDTO Consultar(int idAditivo_)
        {
            var aditivoEncontrado = db.ContratoAditivo.FirstOrDefault(_ => _.IdContratoAditivo == idAditivo_);
            var aditivoDTO = new AditivoDTO();

            if (aditivoEncontrado != null)
            {
                Contrato contrato = ObterContrato(aditivoEncontrado.IdContrato);       

                aditivoDTO = new AditivoDTO()
                {
                    CriadoPor = ObterNomeUsuario(aditivoEncontrado.IdUsuarioCriacao),
                    DataCriacao = aditivoEncontrado.DtCriacao,
                    DataExecucaoAditivo = aditivoEncontrado.DtIniExecucaoAditivo,
                    DataFim = aditivoEncontrado.DtFim,
                    DataFimAditada = aditivoEncontrado.DtFimAditivada,
                    DataInicio = aditivoEncontrado.DtInicio,
                    Id = aditivoEncontrado.IdContratoAditivo,
                    IdContrato = aditivoEncontrado.IdContrato,
                    IdProposta = contrato.IdProposta,
                    Numero = aditivoEncontrado.NuAditivo,
                    PercentualValorAditado = CalcularPercentualValorAditado(aditivoEncontrado.VlContrato, aditivoEncontrado.VlContratoAditivado),
                    Resumo = aditivoEncontrado.DsAditivo,
                    TipoAditivo = aditivoEncontrado.IdTipoAditivo,
                    ValorContrato = aditivoEncontrado.VlContrato,
                    ValorContratoAditado = aditivoEncontrado.VlContratoAditivado.GetValueOrDefault(),
                    Situacao = ObterSituacao(aditivoEncontrado.IdSituacao),
                    IcAditivoData = aditivoEncontrado.IcAditivoData,
                    IcAditivoEscopo = aditivoEncontrado.IcAditivoEscopo,
                    IcAditivoValor = aditivoEncontrado.IcAditivoValor,
                    NuContratoEdit = contrato.NuContratoEdit,
                };
            }

            return aditivoDTO;
        }

        public void Atualizar(AditivoDTO aditivoAtualizado_)
        {
            var aditivo = db.ContratoAditivo.FirstOrDefault(_ => _.IdContratoAditivo == aditivoAtualizado_.Id);
            //if (!aditivoAtualizado_.DataExecucaoAditivo.DataPadrao()) aditivo.DtIniExecucaoAditivo = aditivoAtualizado_.DataExecucaoAditivo;
            //aditivo.DtFim = aditivoAtualizado_.DataFim;
            if (!aditivoAtualizado_.DataFimAditada.DataPadrao()) aditivo.DtFimAditivada = aditivoAtualizado_.DataFimAditada;
            //aditivo.DtInicio = aditivoAtualizado_.DataInicio;
            aditivo.NuAditivo = aditivoAtualizado_.Numero;
            aditivo.DsAditivo = aditivoAtualizado_.Resumo;
            aditivo.IdTipoAditivo = aditivoAtualizado_.TipoAditivo;
            //aditivo.VlContrato = aditivoAtualizado_.ValorContrato;
            //aditivo.VlContratoAditivado = aditivoAtualizado_.ValorContratoAditado;
            db.SaveChanges();
        }

        public void AplicarAditivo(InputAplicarAditivo inputAplicarAditivo)
        {
            var aditivo = db.ContratoAditivo.FirstOrDefault(_ => _.IdContratoAditivo == inputAplicarAditivo.IdAditivo);
            var contrato = db.Contrato.FirstOrDefault(_ => _.IdContrato == aditivo.IdContrato);

            if (aditivo.IcAditivoData != null)
            {
                contrato.DtFim = aditivo.DtFimAditivada;
            }
            if (aditivo.IcAditivoValor != null)
            {
                contrato.VlContrato = aditivo.VlContratoAditivado.GetValueOrDefault();
            }

            aditivo.DtAplicacao = DateTime.Now;
            aditivo.IdUsuarioAplicacao = AppSettings.constGlobalUserID;
            aditivo.DtIniExecucaoAditivo = DateTime.Now;
            aditivo.IdSituacao = 101;

            var lstContratoCronogramaFinanceiroAntigos = db.ContratoCronogramaFinanceiro.Where(w => w.IdContrato == contrato.IdContrato).ToList();
            var lstContratoEntregaveisAntigos = db.ContratoEntregavel.Where(w => w.IdContrato == contrato.IdContrato).ToList();

            var lstContratoCronogramaFinanceiroTemporaria = db.ContratoCronogramaFinanceiroTemporaria.Where(w => w.IdContrato == contrato.IdContrato).ToList();
            var lstEntregaveisTemporaria = db.ContratoEntregavelTemporaria.Where(w => w.IdContrato == contrato.IdContrato).ToList();

            foreach (var entregavelTemp in lstEntregaveisTemporaria)
            {
                var contratoEntregavel = new ContratoEntregavel();
                contratoEntregavel.IdContrato = entregavelTemp.IdContrato;
                contratoEntregavel.DsProduto = entregavelTemp.DsProduto;
                contratoEntregavel.DtProduto = entregavelTemp.DtProduto;
                contratoEntregavel.IdContratoCliente = entregavelTemp.IdContratoCliente.Value;
                contratoEntregavel.IdFrente = entregavelTemp.IdFrente;
                contratoEntregavel.IdSituacao = entregavelTemp.IdSituacao;
                contratoEntregavel.VlOrdem = entregavelTemp.VlOrdem;

                db.ContratoEntregavel.Add(contratoEntregavel);
                db.SaveChanges();

                entregavelTemp.IdEntregavel = contratoEntregavel.IdContratoEntregavel;
            }

            foreach (var parcelaTemp in lstContratoCronogramaFinanceiroTemporaria)
            {
                var contratoCronogramaFinanceiro = new ContratoCronogramaFinanceiro();
                contratoCronogramaFinanceiro.CdIss = parcelaTemp.CdIss;
                // Retorna somente os dois ultimos caracteres do Código da Parcela
                string codigoReajusteParcela = parcelaTemp.CdParcela.Substring(parcelaTemp.CdParcela.Length - 2, 2);
                // Remove os dois ultimos caracteres do Código da Parcela
                parcelaTemp.CdParcela = parcelaTemp.CdParcela.Remove(parcelaTemp.CdParcela.Length - 2, 2);
                // Soma 1 nos dois ultimos caracteres do Código da Parcela
                int novoCdReajusteParcela = Convert.ToInt32(codigoReajusteParcela) + 1;
                codigoReajusteParcela = novoCdReajusteParcela.ToString().Length > 1 ? novoCdReajusteParcela.ToString() : "0" + novoCdReajusteParcela.ToString();
                parcelaTemp.CdParcela += codigoReajusteParcela;
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
                db.SaveChanges();

                // Busca os Registros na Tabela ContratoParcelaEntregavelTemporaria para verificar se a Parcela estava ligada com Entregaveis
                var lstContratoParcelaEntregavelTemp = db.ContratoParcelaEntregavelTemporaria.Where(w => w.IdParcela == parcelaTemp.IdContratoCronFinanceiro).ToList();
                if (lstContratoParcelaEntregavelTemp.Count > 0)
                {
                    foreach (var contratoParcEntreTemp in lstContratoParcelaEntregavelTemp)
                    {
                        // Busca o registro na tabela ContratoEntregavelTemporaria
                        var contratoEntregavelTemp = lstEntregaveisTemporaria.Where(w => w.IdContratoEntregavel == contratoParcEntreTemp.IdEntregavel).FirstOrDefault();
                        if (contratoEntregavelTemp != null)
                        {
                            // Com o ContratoEntregavel e a Parcela , cria um novo registro na tabela ContratoEntregavelParcela
                            var contratoParcelaEntregavel = new ContratoParcelaEntregavel();
                            contratoParcelaEntregavel.IdEntregavel = contratoEntregavelTemp.IdEntregavel.Value;
                            contratoParcelaEntregavel.IdParcela = contratoCronogramaFinanceiro.IdContratoCronFinanceiro;

                            db.ContratoParcelaEntregavel.Add(contratoParcelaEntregavel);
                            db.SaveChanges();
                        }

                        // Após copiar o relacionamento entre Parcela e Entregavel remove o registro
                        db.ContratoParcelaEntregavelTemporaria.Remove(contratoParcEntreTemp);
                        db.SaveChanges();
                    }
                }

                // Após copiar a Parcela , exclui da tabela Temporaria
                db.ContratoCronogramaFinanceiroTemporaria.Remove(parcelaTemp);
                db.SaveChanges();
            }

            // Remove todos os registros de EntregaveisTemporaria do devido Contrato            
            db.ContratoEntregavelTemporaria.RemoveRange(lstEntregaveisTemporaria);
            db.SaveChanges();

            // Busca todas as relações antigas entre Parcela e Entregavel e exclui
            foreach (var entregavelAntigo in lstContratoEntregaveisAntigos)
            {
                var lstParcelaEntregavelAntigo = db.ContratoParcelaEntregavel.Where(w => w.IdEntregavel == entregavelAntigo.IdContratoEntregavel).ToList();
                if (lstParcelaEntregavelAntigo.Count > 0)
                {
                    db.ContratoParcelaEntregavel.RemoveRange(lstParcelaEntregavelAntigo);
                    db.SaveChanges();
                }
            }

            // Remove todos Registros de Entregaveis e Parcelas antigos (salvos no histórico)
            db.ContratoEntregavel.RemoveRange(lstContratoEntregaveisAntigos);
            db.ContratoCronogramaFinanceiro.RemoveRange(lstContratoCronogramaFinanceiroAntigos);

            db.SaveChanges();
        }

        #region || Métodos Privados

        private Contrato ObterContrato(int idContrato_)
        {
            return db.Contrato.FirstOrDefault(_ => _.IdContrato == idContrato_);
        }

        private TipoAditivoDTO ObterTipoAditivo(short? idTipo_)
        {
            return db.TipoAditivo.Where(_ => _.IdTipoAditivo == idTipo_)?.Select(_ => new TipoAditivoDTO()
            {
                Id = _.IdTipoAditivo,
                Nome = _.DsTipoAditivo
            }).FirstOrDefault();
        }

        private decimal CalcularPercentualValorAditado(decimal valor_, decimal? valorAditado_)
        {
            decimal diferencaValor = valorAditado_.GetValueOrDefault() - valor_;
            decimal percentual = (diferencaValor / valor_) * 100;
            return Math.Round((diferencaValor / valor_) * 100, 2);
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

        #region | Métodos
        public class InputAplicarAditivo
        {
            public int IdAditivo { get; set; }
            public int IdUsuarioAplicacao { get; set; }
        }
        #endregion
    }
}
