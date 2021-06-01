using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.Models
{
    public class bPessoaJuridica
    {
        private FIPEContratosContext db { get; set; }

        public bPessoaJuridica(FIPEContratosContext db)
        {
            this.db = db;
        }

        public void AddPessoaJuridica(PessoaJuridica item)
        {            
            db.PessoaJuridica.Add(item);
            db.SaveChanges();

            var pessoa = new Pessoa();
            pessoa.IdPessoaJuridica = item.IdPessoaJuridica;

            db.Pessoa.Add(pessoa);
            db.SaveChanges();
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

        public List<Entidade> BuscarEntidade(int tipoEntidade)
        {
            var itens = db.Entidade.Where(w => w.IdTipoEntidade == tipoEntidade).ToList();

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

        public List<PessoaJuridica> BuscarPessoaJuridica()
        {
            var pessoasJuridicas = db.PessoaJuridica                
                .OrderBy(w => w.NmFantasia).ToList();

            return pessoasJuridicas;
        }

        public PessoaJuridica BuscarPessoaJuridicaId(int id)
        {
            var item = db.PessoaJuridica.Where(w => w.IdPessoaJuridica == id).FirstOrDefault();

            return item;
        }

        public HistoricoPessoaJuridica BuscarHistId(int id)
        {
            var item = db.HistoricoPessoaJuridica.Where(w => w.IdHistPessoaJuridica == id).FirstOrDefault();

            return item;
        }

        public void UpdatePessoaJuridica(PessoaJuridica item)
        {
            var itemPessoaJuridica = BuscarPessoaJuridicaId(item.IdPessoaJuridica);

            itemPessoaJuridica.IdPessoaJuridica = item.IdPessoaJuridica;
            itemPessoaJuridica.NmFantasia = item.NmFantasia;
            itemPessoaJuridica.RazaoSocial = item.RazaoSocial;
            itemPessoaJuridica.Cnpj = item.Cnpj;
            itemPessoaJuridica.Cep = item.Cep;
            itemPessoaJuridica.Uf = item.Uf;
            itemPessoaJuridica.IdCidade = item.IdCidade;
            itemPessoaJuridica.Endereco = item.Endereco;            
            itemPessoaJuridica.NmBairro = item.NmBairro;
            itemPessoaJuridica.IdEsferaEmpresa = item.IdEsferaEmpresa;
            itemPessoaJuridica.IdTipoAdministracao = item.IdTipoAdministracao;
            itemPessoaJuridica.IdEntidade = item.IdEntidade;
            itemPessoaJuridica.IdClassificacaoEmpresa = item.IdClassificacaoEmpresa;
            itemPessoaJuridica.IdPais = item.IdPais;
            itemPessoaJuridica.DsInternacional = item.DsInternacional;
            itemPessoaJuridica.NuEndereco = item.NuEndereco;
            itemPessoaJuridica.Complemento = item.Complemento;

            db.SaveChanges();

        }

        public PessoaJuridica ObterPessoaJuridicaPorCNPJ(string cnpj_)
        {
            return db.PessoaJuridica.Where(_ => _.Cnpj == cnpj_).FirstOrDefault();
        }

        //Historico Pessoa Juridica

        public List<HistoricoPessoaJuridica> BuscarHistoricoPessoaJuridica(int id)
        {
            var item = db.HistoricoPessoaJuridica.Where(w => w.IdPessoaJuridica == id).ToList();

            return item;
        }

        public void AddHistoricoPessoaJuridica(HistoricoPessoaJuridica item)
        {
            db.HistoricoPessoaJuridica.Add(item);
            db.SaveChanges();
        }
    }
}
