using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiFipe.Models
{
    public class bPessoaFisica
    {
        public FIPEContratosContext db { get; set; }

        public bPessoaFisica(FIPEContratosContext db)
        {
            this.db = db;
        }

        public void AddPessoaFisica(PessoaFisica item)
        {
            db.PessoaFisica.Add(item);
            db.SaveChanges();
        }

        public void AddPessoaFisicaHistorico(HistoricoPessoaFisica item)
        {
            db.HistoricoPessoaFisica.Add(item);
            db.SaveChanges();
        }

        public List<HistoricoPessoaFisica> BuscarPessoaHistorico(int id)
        {
            var itens = db.HistoricoPessoaFisica.Where(w => w.IdPessoaFisica == id).ToList();

            return itens;
        }

        public List<PessoaFisica> BuscarPessoa()
        {
            var itens = db.PessoaFisica.OrderBy(w => w.NmPessoa).ToList();

            return itens;
        }

        public PessoaFisica BuscarPessoaId(int id)
        {

            var item = db.PessoaFisica.Where(w => w.IdPessoaFisica == id).FirstOrDefault();

            return item;
        }

        public HistoricoPessoaFisica BuscaHistId(int id)
        {

            var item = db.HistoricoPessoaFisica.Where(w => w.IdHstPessoaFisica == id).FirstOrDefault();

            return item;
        }

        public void UpdatePessoa(PessoaFisica item)
        {
            var itemPessoa = BuscarPessoaId(item.IdPessoaFisica);

            itemPessoa.IdPessoaFisica = item.IdPessoaFisica;
            itemPessoa.NmPessoa = item.NmPessoa;
            itemPessoa.NuCpf = item.NuCpf;
            itemPessoa.DtNascimento = item.DtNascimento;
            itemPessoa.CdSexo = item.CdSexo;
            itemPessoa.CdEmail = item.CdEmail;
            itemPessoa.NuCep = item.NuCep;
            itemPessoa.DsEndereco = item.DsEndereco;
            itemPessoa.NuEndereco = item.NuEndereco;
            itemPessoa.DsComplemento = item.DsComplemento;
            itemPessoa.NmBairro = item.NmBairro;
            itemPessoa.SgUf = item.SgUf;
            itemPessoa.IdCidade = item.IdCidade;
            itemPessoa.CdCvLattes = item.CdCvLattes;
            itemPessoa.CdLinkedIn = item.CdLinkedIn;
            itemPessoa.NuTelefoneComercial = item.NuTelefoneComercial;
            itemPessoa.NuTelefoneFixo = item.NuTelefoneFixo;
            itemPessoa.NuCelular = item.NuCelular;
            itemPessoa.IdTipoVinculo = item.IdTipoVinculo;

            db.SaveChanges();

        }

        public Cidade ObterCidade(int idCidade_)
        {
            return db.Cidade.FirstOrDefault(_ => _.IdCidade == idCidade_);
        }

        public string ObterNomeCidade(int? idCidade_)
        {
            return idCidade_ == null ? string.Empty : ObterCidade(idCidade_.Value).NmCidade;
        }

        public PessoaFisica ObterPessoaPorCPF(string cpf_)
        {
            return db.PessoaFisica.Where(_ => _.NuCpf == cpf_).FirstOrDefault();
        }

        public PessoaFisica VerificaCPFCadastrado(string cpf_, int idPessoaFisica_)
        {
            return db.PessoaFisica.Where(_ => _.NuCpf == cpf_ && _.IdPessoaFisica != idPessoaFisica_).FirstOrDefault();
        }

        public PessoaFisica GetById(int id)
        {
            var pessoaFisica = db.PessoaFisica
                .Where(w => w.IdPessoaFisica == id)
                .Single();

            return pessoaFisica;
        }
    }
}
