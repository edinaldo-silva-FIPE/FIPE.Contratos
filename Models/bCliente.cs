using ApiFipe.DTOs;
using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bCliente
    {
        public FIPEContratosContext db { get; set; }

        public bCliente(FIPEContratosContext db)
        {
            this.db = db;
        }

        public void AddCliente(Cliente item)
        {

            db.Cliente.Add(item);
            db.SaveChanges();
            var idCli = item.IdCliente;

        }

        public List<EsferaEmpresa> BuscarEsfera(int id)
        {

            var itens = db.EsferaEmpresa.Where(w => w.IdClassificacaoEmpresa == id).ToList();

            return itens;
        }

        public List<TipoAdministracao> BuscarAdm()
        {
            var itens = db.TipoAdministracao.ToList();

            return itens;
        }

        public List<Entidade> BuscarEntidade()
        {
            var itens = db.Entidade.ToList();

            return itens;
        }

        public List<Pais> BuscarPaises()
        {
            var itens = db.Pais.ToList();

            return itens;
        }


        public List<Cidade> BuscarCidade(string Uf)
        {
            return db.Cidade.Where(w => w.Uf == Uf).ToList();
        }

        public Cidade ObterCidade(int idCidade_)
        {
            return db.Cidade.FirstOrDefault(_ => _.IdCidade == idCidade_);
        }

        public string ObterNomeCidade(int idCidade_)
        {
            return ObterCidade(idCidade_).NmCidade;
        }

        public List<Cliente> BuscarCliente()
        {
            return db.Cliente.ToList();
        }

        public Cliente BuscarClienteId(int id)
        {

            var item = db.Cliente.Where(w => w.IdCliente == id).FirstOrDefault();

            return item;
        }

        public Cliente BuscarClienteIdPessoa(int id)
        {

            var item = db.Cliente.Where(w => w.IdPessoa == id).FirstOrDefault();

            return item;
        }

        public void UpdateCliente(Cliente item)
        {
            var itemCliente = BuscarClienteId(item.IdCliente);

            itemCliente.IdCliente = item.IdCliente;
            //itemCliente.NmFantasia = item.NmFantasia;
            //itemCliente.RazaoSocial = item.RazaoSocial;
            //itemCliente.Cnpj = item.Cnpj;
            //itemCliente.Cep = item.Cep;
            //itemCliente.Uf = item.Uf;
            //itemCliente.IdCidade = item.IdCidade;
            //itemCliente.Endereco = item.Endereco;
            //itemCliente.IdEsfera = item.IdEsfera;
            //itemCliente.NmBairro = item.NmBairro;
            //itemCliente.IdEsferaEmpresa = item.IdEsferaEmpresa;
            //itemCliente.IdTipoAdministracao = item.IdTipoAdministracao;
            //itemCliente.IdEntidade = item.IdEntidade;
            //itemCliente.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;
            //itemCliente.IdPais = item.IdPais;
            //itemCliente.DsInternacional = item.DsInternacional;
            //itemCliente.Complemento = item.Complemento;
            //itemCliente.Endereco = item.Endereco;

            db.SaveChanges();

        }

        public Cliente GetById(int id)
        {
            var cliente = db.Cliente
                .Where(w => w.IdCliente == id)
                .Single();

            return cliente;
        }

        public PropostaCliente GetByPropostaCliente(int idProposta, int idCliente)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdProposta == idProposta && w.IdCliente == idCliente)
                .FirstOrDefault();

            return propostaCliente;
        }

        public List<ClienteDTO> ObterClientes(int idContrato_)
        {
            //EGS 30.10.2020 Estava dando erro...
            if (idContrato_ == 0) return null;

            var clientes = new List<ClienteDTO>();
            var contrato = new bContrato(db).GetContratoById(idContrato_);
            var cdsClientes = contrato.ClientesPagadores;

            cdsClientes.ForEach(_ => clientes.Add(ObterCliente(_, idContrato_)));

            return clientes;
        }

        private ClienteDTO ObterCliente(int idCliente_, int idContrato_)
        {
            var clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idCliente_ && _.IdContrato == idContrato_)?
                .Include(w => w.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation)
                .FirstOrDefault();
            if (String.IsNullOrEmpty(clienteContrato.NmFantasia))
            {
                clienteContrato.NmFantasia = clienteContrato.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.NmPessoa;
            }
            return new ClienteDTO()
            {
                Id = clienteContrato.IdContratoCliente,
                NmCliente = clienteContrato.NmFantasia
            };
        }

        public List<Controllers.OutPutGetCliente> ListaContratoClientes(int id)
        {
            var listacliente = new List<Controllers.OutPutGetCliente>();
            var contratoClientes = new bContratoCliente(db).BuscarClienteFisico(id);

            foreach (var item in contratoClientes)
            {
                var cliente = new Controllers.OutPutGetCliente();
                cliente.IcSomentePagador = item.IcSomentePagador;
                if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisica != null)
                {
                    cliente.IdCliente = item.IdCliente;
                    cliente.IdPessoa = item.IdClienteNavigation.IdPessoa;
                    cliente.NmCliente = item.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.NmPessoa;
                    cliente.IdContratoCliente = item.IdContratoCliente;
                }
                if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridica != null)
                {
                    cliente.IdCliente = item.IdCliente;
                    cliente.IdPessoa = item.IdClienteNavigation.IdPessoa;
                    if (item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdEsferaEmpresaNavigation != null)
                    {
                        cliente.DsEsferaEmpresa = item.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdEsferaEmpresaNavigation.DsEsferaEmpresa;
                    }
                    cliente.NmCliente = item.NmFantasia;
                    cliente.IdContratoCliente = item.IdContratoCliente;
                }

                listacliente.Add(cliente);
            }
            return listacliente;
        }

        public ClienteDTO ObterClienteContrato(int idContratoCliente_)
        {
            var clienteContrato = db.ContratoCliente.Where(_ => _.IdContratoCliente == idContratoCliente_)?.FirstOrDefault();
            var nmCliente = "";
            if (clienteContrato.NmFantasia == null)
            {
                var cliente = new bCliente(db).BuscarClienteId(clienteContrato.IdCliente);
                var pessoa = db.Pessoa.Where(w => w.IdPessoa == cliente.IdPessoa).FirstOrDefault();
                var pessoaFisica = new bPessoaFisica(db).BuscarPessoaId(pessoa.IdPessoaFisica.Value);
                nmCliente = pessoaFisica.NmPessoa;
            }
            else
            {
                nmCliente = clienteContrato.NmFantasia;
            }
            return new ClienteDTO()
            {
                Id = clienteContrato.IdContratoCliente,
                NmCliente = nmCliente
            };
        }
    }
}