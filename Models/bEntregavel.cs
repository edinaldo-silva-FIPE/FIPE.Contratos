using ApiFipe.Controllers;
using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using ApiFipe.Utilitario.Extensoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bEntregavel
    {
        #region | Propriedades

        public FIPEContratosContext db { get; set; }

        #endregion

        #region | Construtores

        public bEntregavel(FIPEContratosContext db)
        {
            this.db = db;
        }

        #endregion

        #region | Métodos

        #region || Métodos Públicos

        public void CriarEntregaveisEmLote(InputEntregavel parametroEntregavel_)
        {
            List<OutPutGetCliente> clienteContratante = new bCliente(db).ListaContratoClientes(parametroEntregavel_.IdContrato);

            int numero = 0;
            var entregaveis = db.ContratoEntregavel.Where(w => w.IdContrato == parametroEntregavel_.IdContrato).ToList();
            if (entregaveis.Count > 0)
            {
                numero = db.ContratoEntregavel.Where(w => w.IdContrato == parametroEntregavel_.IdContrato).Last().VlOrdem;
            }

            foreach (var cliente in clienteContratante)
            {              
                for (int i = 0; i < parametroEntregavel_.Quantidade; i++)
                {
                    numero++;
                    DateTime novaData = parametroEntregavel_.DataPrevista.AddMonths(i);
                    CriarNovoEntregavelLote(parametroEntregavel_.Nome, novaData, parametroEntregavel_.IdContrato, 0, numero, cliente.IdContratoCliente.ObterValorInteiro());
                }
            }
        }

        public void CriarNovoEntregavelLote(string nome_, DateTime? data_, int idContrato_, int frente_, int numero_, int idCliente_)
        {

            db.ContratoEntregavel.Add(new ContratoEntregavel()
            {
                DsProduto = nome_,
                DtProduto = data_,
                IdContrato = idContrato_,
                IdFrente = frente_ == 0 ? ObterFrentePadrao(idContrato_) : frente_,
                VlOrdem = numero_,
                //IdContratoCliente = idContratoCliente.ObterValorInteiro()
                IdContratoCliente = idCliente_,
                IcAtraso = data_.Value.Date < DateTime.Now.Date ? true : false,
                IdSituacao = data_.Value.Date < DateTime.Now.Date ? 68 : 56
            });

            db.SaveChanges();
        }

        public void CriarNovoEntregavel(string nome_, DateTime? data_, int idContrato_, int frente_, int numero_, int idCliente_, int idSituacao_)
        {
            //int? idContratoCliente = db.ContratoCliente.Where(_ => _.IdCliente == idCliente_)?.FirstOrDefault().IdContratoCliente;
            int situacao = 0;
            bool atraso = false;
            if (idSituacao_ == 89 || idSituacao_ == 90 || idSituacao_ == 91)
            {
                situacao = idSituacao_;
            }
            else
            {
                if (data_ != null)
                {
                    situacao = data_.Value.Date < DateTime.Now.Date ? 68 : idSituacao_;
                    atraso = data_.Value.Date < DateTime.Now.Date ? true : false;
                }
                else
                {
                    situacao = idSituacao_;
                    atraso = false;
                }
            }

            db.ContratoEntregavel.Add(new ContratoEntregavel()
            {
                DsProduto = nome_,
                DtProduto = data_,
                IdContrato = idContrato_,
                IdFrente = frente_ == 0 ? ObterFrentePadrao(idContrato_) : frente_,
                VlOrdem = numero_,
                //IdContratoCliente = idContratoCliente.ObterValorInteiro()
                IdContratoCliente = idCliente_,
                IcAtraso = atraso,
                IdSituacao = situacao
            });

            db.SaveChanges();
        }
        public void CriarNovoEntregavelTemporaria(string nome_, DateTime? data_, int idContrato_, int frente_, int numero_, int idCliente_, int idSituacao_)
        {
            //int? idContratoCliente = db.ContratoCliente.Where(_ => _.IdCliente == idCliente_)?.FirstOrDefault().IdContratoCliente;
            int situacao = 0;
            bool atraso = false;
            if (idSituacao_ == 89 || idSituacao_ == 90 || idSituacao_ == 91)
            {
                situacao = idSituacao_;
            }
            else
            {
                if (data_ != null)
                {
                    situacao = data_.Value.Date < DateTime.Now.Date ? 68 : idSituacao_;
                    atraso = data_.Value.Date < DateTime.Now.Date ? true : false;
                }
                else
                {
                    situacao = idSituacao_;
                    atraso = false;
                }
            }

            db.ContratoEntregavelTemporaria.Add(new ContratoEntregavelTemporaria()
            {
                DsProduto = nome_,
                DtProduto = data_,
                IdContrato = idContrato_,
                IdFrente = frente_ == 0 ? ObterFrentePadrao(idContrato_) : frente_,
                IdSituacao = situacao, // No Prazo
                VlOrdem = numero_,
                //IdContratoCliente = idContratoCliente.ObterValorInteiro()
                IdContratoCliente = idCliente_,
                IcAtraso = atraso
            });

            db.SaveChanges();
        }

        public void AtualizarEntregavel(EntregavelDTO entregavel_)
        {
            ContratoEntregavel entregavel = db.ContratoEntregavel.Where(_ => _.IdContratoEntregavel == entregavel_.Id)?.FirstOrDefault();

            if (entregavel != null)
            {
                entregavel.DsProduto = entregavel_.Nome;
                entregavel.DtProduto = entregavel_.DataPrevista;
                entregavel.IdFrente = entregavel_.Frente.Id;

                if (entregavel_.Situacao.Id == 89 || entregavel_.Situacao.Id == 90 || entregavel_.Situacao.Id == 91)
                {
                    entregavel.IdSituacao = entregavel_.Situacao.Id;
                    entregavel.IcAtraso = false;
                }
                else
                {
                    if (entregavel_.DataPrevista != null)
                    {
                        if (entregavel_.DataPrevista.Value.Date < DateTime.Now.Date)
                        {
                            entregavel.IdSituacao = 68;
                            entregavel.IcAtraso = true;
                        }
                        else
                        {
                            if (entregavel_.Situacao.Id == 68 || entregavel_.Situacao.Id == 97)
                            {
                                entregavel.IdSituacao = 56;
                                entregavel.IcAtraso = false;
                            }                            
                            else
                            {
                                entregavel.IdSituacao = entregavel_.Situacao.Id;
                                entregavel.IcAtraso = false;
                            }
                        }
                    }
                    else
                    {
                        entregavel.IdSituacao = entregavel_.Situacao.Id;
                        entregavel.IcAtraso = false;
                    }
                }
                entregavel.VlOrdem = entregavel_.Numero;
                entregavel.IdContratoCliente = entregavel_.Cliente.Id;

                db.SaveChanges();
            }
        }
        public void AtualizarEntregavelTemporaria(EntregavelDTO entregavel_)
        {
            ContratoEntregavelTemporaria entregavel = db.ContratoEntregavelTemporaria.Where(_ => _.IdContratoEntregavel == entregavel_.Id)?.FirstOrDefault();

            if (entregavel != null)
            {
                entregavel.DsProduto = entregavel_.Nome;
                entregavel.DtProduto = entregavel_.DataPrevista;
                entregavel.IdFrente = entregavel_.Frente.Id;
                entregavel.IdSituacao = entregavel_.Situacao.Id;
                if (entregavel_.Situacao.Id == 89 || entregavel_.Situacao.Id == 90 || entregavel_.Situacao.Id == 91)
                {
                    entregavel.IdSituacao = entregavel_.Situacao.Id;
                    entregavel.IcAtraso = false;
                }
                else
                {
                    if (entregavel_.DataPrevista != null)
                    {
                        if (entregavel_.DataPrevista.Value.Date < DateTime.Now.Date)
                        {
                            entregavel.IdSituacao = 68;
                            entregavel.IcAtraso = true;
                        }
                        else
                        {
                            if (entregavel_.Situacao.Id == 68 || entregavel_.Situacao.Id == 97)
                            {
                                entregavel.IdSituacao = 56;
                                entregavel.IcAtraso = false;
                            }
                            else
                            {
                                entregavel.IdSituacao = entregavel_.Situacao.Id;
                                entregavel.IcAtraso = false;
                            }
                        }
                    }
                    else
                    {
                        entregavel.IdSituacao = entregavel_.Situacao.Id;
                        entregavel.IcAtraso = false;
                    }
                }                
                entregavel.VlOrdem = entregavel_.Numero;
                entregavel.IdContratoCliente = entregavel_.Cliente.Id;

                db.SaveChanges();
            }
        }

        public List<EntregavelDTO> ObterEntregaveis(int idContrato_)
        {
            var retornoEntregaveis = new List<EntregavelDTO>();
            var entregaveis = db.ContratoEntregavel.Where(_ => _.IdContrato == idContrato_)?.ToList();

            if (entregaveis != null)
            {
                foreach (var e in entregaveis)
                {
                    retornoEntregaveis.Add(new EntregavelDTO()
                    {
                        DataPrevista   = e.DtProduto,
                        Frente         = new bFrente(db).ConsultarFrente(e.IdFrente),
                        Id             = e.IdContratoEntregavel,
                        IdContrato     = e.IdContrato,
                        Nome           = e.DsProduto,
                        Numero         = e.VlOrdem,
                        Situacao       = new bSituacao(db).ConsultarSituacao(e.IdSituacao),
                        Cliente        = new bCliente(db).ObterClienteContrato(e.IdContratoCliente.ObterValorInteiro()),
                        IcAtraso       = e.IcAtraso
                    });
                }
            }

            return retornoEntregaveis.OrderBy(_ => _.Numero).ThenBy(_ => _.Cliente.NmCliente).ToList();
        }
        public List<EntregavelDTO> ObterEntregaveisTemporaria(int idContrato_)
        {
            var retornoEntregaveis = new List<EntregavelDTO>();
            var entregaveis = db.ContratoEntregavelTemporaria.Where(_ => _.IdContrato == idContrato_)?.ToList();

            if (entregaveis != null)
            {
                foreach (var e in entregaveis)
                {
                    retornoEntregaveis.Add(new EntregavelDTO()
                    {
                        DataPrevista = e.DtProduto,
                        Frente = new bFrente(db).ConsultarFrente(e.IdFrente),
                        Id = e.IdContratoEntregavel,
                        IdContrato = e.IdContrato,
                        Nome = e.DsProduto,
                        Numero = e.VlOrdem,
                        Situacao = new bSituacao(db).ConsultarSituacao(e.IdSituacao),
                        Cliente = new bCliente(db).ObterClienteContrato(e.IdContratoCliente.ObterValorInteiro()),
                        IcAtraso = e.IcAtraso
                    });
                }
            }

            return retornoEntregaveis.OrderBy(_ => _.Numero).ThenBy(_ => _.Cliente.NmCliente).ToList();
        }

        public EntregavelDTO ConsultarEntregavel(int idEntregavel_)
        {
            var entregavel = db.ContratoEntregavel.FirstOrDefault(_ => _.IdContratoEntregavel == idEntregavel_);
            var contratoCliente = new bCliente(db).ListaContratoClientes(entregavel.IdContratoCliente.ObterValorInteiro())?.FirstOrDefault();

            return new EntregavelDTO()
            {
                DataPrevista = entregavel.DtProduto,
                Frente = new bFrente(db).ConsultarFrente(entregavel.IdFrente),
                Id = entregavel.IdContratoEntregavel,
                IdContrato = entregavel.IdContrato,
                Nome = entregavel.DsProduto,
                Numero = entregavel.VlOrdem,
                Situacao = new bSituacao(db).ConsultarSituacao(entregavel.IdSituacao),
                //Cliente = new ClienteDTO() { Id = contratoCliente.IdContratoCliente, NmCliente = contratoCliente.NmFantasia }
                Cliente = new bCliente(db).ObterClienteContrato(entregavel.IdContratoCliente.ObterValorInteiro())
            };
        }
        public EntregavelDTO ConsultarEntregavelTemporaria(int idEntregavel_)
        {
            var entregavel = db.ContratoEntregavelTemporaria.FirstOrDefault(_ => _.IdContratoEntregavel == idEntregavel_);
            var contratoCliente = new bCliente(db).ListaContratoClientes(entregavel.IdContratoCliente.ObterValorInteiro())?.FirstOrDefault();

            return new EntregavelDTO()
            {
                DataPrevista = entregavel.DtProduto,
                Frente = new bFrente(db).ConsultarFrente(entregavel.IdFrente),
                Id = entregavel.IdContratoEntregavel,
                IdContrato = entregavel.IdContrato,
                Nome = entregavel.DsProduto,
                Numero = entregavel.VlOrdem,
                Situacao = new bSituacao(db).ConsultarSituacao(entregavel.IdSituacao),
                //Cliente = new ClienteDTO() { Id = contratoCliente.IdContratoCliente, NmCliente = contratoCliente.NmFantasia }
                Cliente = new bCliente(db).ObterClienteContrato(entregavel.IdContratoCliente.ObterValorInteiro())
            };
        }

        public void ExcluirEntregavel(int idEntregavel_)
        {
            ContratoEntregavel entregavel = db.ContratoEntregavel.Where(_ => _.IdContratoEntregavel == idEntregavel_)?.FirstOrDefault();

            if (entregavel != null)
            {
                var colParcelaEntregavel = db.ContratoParcelaEntregavel.Where(_ => _.IdEntregavel == entregavel.IdContratoEntregavel)?.ToList();

                if (colParcelaEntregavel != null)
                    colParcelaEntregavel.ForEach(_ => db.ContratoParcelaEntregavel.Remove(_));

                db.ContratoEntregavel.Remove(entregavel);
                db.SaveChanges();
            }
        }
        public void ExcluirEntregavelTemporaria(int idEntregavel_)
        {
            ContratoEntregavelTemporaria entregavel = db.ContratoEntregavelTemporaria.Where(_ => _.IdContratoEntregavel == idEntregavel_)?.FirstOrDefault();

            if (entregavel != null)
            {
                var colParcelaEntregavel = db.ContratoParcelaEntregavelTemporaria.Where(_ => _.IdEntregavel == entregavel.IdContratoEntregavel)?.ToList();

                if (colParcelaEntregavel != null)
                    colParcelaEntregavel.ForEach(_ => db.ContratoParcelaEntregavelTemporaria.Remove(_));

                db.ContratoEntregavelTemporaria.Remove(entregavel);
                db.SaveChanges();
            }
        }

        public List<EntregavelHistoricoDTO> ObterHistorico(int idAditivo_)
        {
            var retornoEntregaveis = new List<EntregavelHistoricoDTO>();
            var entregaveis = db.ContratoEntregavelHistórico.Where(_ => _.IdContratoAditivo == idAditivo_)?.ToList();

            if (entregaveis != null)
            {
                foreach (var e in entregaveis)
                {
                    retornoEntregaveis.Add(new EntregavelHistoricoDTO()
                    {
                        DataPrevista = e.DtProduto,
                        Frente = new bFrente(db).ConsultarFrente(e.IdFrente),
                        Id = e.IdContratoEntregavelHistorico,
                        IdAditivo = e.IdContratoAditivo,
                        Nome = e.DsProduto,
                        Numero = e.VlOrdem,
                        Situacao = new bSituacao(db).ConsultarSituacao(e.IdSituacao),
                        Cliente = new bCliente(db).ObterClienteContrato(e.IdContratoCliente.ObterValorInteiro())
                    });
                }
            }

            return retornoEntregaveis.OrderBy(_ => _.Numero).ThenBy(_ => _.Cliente.NmCliente).ToList();
        }

        public List<ContratoEntregavel> ObterTodosEntregaiveis()
        {
            var listaEntregaveis = db.ContratoEntregavel.ToList();

            return listaEntregaveis;
        }
        public List<ContratoEntregavelTemporaria> ObterTodosEntregaiveisTemporaria()
        {
            var listaEntregaveis = db.ContratoEntregavelTemporaria.ToList();

            return listaEntregaveis;
        }

        #endregion

        #region || Métodos Privados

        private int ObterFrentePadrao(int idContrato_)
        {
            var frente = new bFrente(db).BuscaFrenteIdContrato(idContrato_)?.FirstOrDefault();
            return frente == null ? 0 : frente.IdFrente;
        }

        private string ObterNomeFrente(int idFrente_)
        {
            return db.Frente.Where(_ => _.IdFrente == idFrente_)?.FirstOrDefault()?.NmFrente;
        }

        private int GerarNumeroEntregavel(int idContrato_)
        {
            var entregaveis = ObterEntregaveis(idContrato_);
            return entregaveis.ColecaoVazia() ? 1 : entregaveis.Max(_ => _.Numero) + 1;
        }
        private int GerarNumeroEntregavelTemporaria(int idContrato_)
        {
            var entregaveis = ObterEntregaveisTemporaria(idContrato_);
            return entregaveis.ColecaoVazia() ? 1 : entregaveis.Max(_ => _.Numero) + 1;
        }

        #endregion

        #endregion
    }
}
