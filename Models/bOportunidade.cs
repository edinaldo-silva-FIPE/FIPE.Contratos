using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bOportunidade
    {
        private FIPEContratosContext db { get; set; }

        public bOportunidade(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<Oportunidade> BuscarOportunidade()
        {
            var oportunidades = db.Oportunidade
                .Where(p => p.DtLimiteEntregaProposta != null)
              //.OrderBy(w => w.DtLimiteEntregaProposta)    //EGS 12.06.2020 Ordenar por IDProposta recente
                .OrderByDescending(w => w.IdOportunidade)
                .ToList();

            var oportunidadesSemDataLimite = db.Oportunidade
                .Where(p => p.DtLimiteEntregaProposta == null)
              //.OrderBy(w => w.IdProposta)                //EGS 12.06.2020 Ordenar por IDProposta recente
                .OrderByDescending(w => w.IdProposta)
                .ToList();

            oportunidades.AddRange(oportunidadesSemDataLimite);

            return oportunidades;
        }

        public List<Oportunidade> BuscarOportunidadesSemResp()
        {
            var allOportunidades = new List<Oportunidade>();

            #region Oportunidades com data limite
            var oportunidades = db.Oportunidade
                .Where(p => p.DtLimiteEntregaProposta != null)
              //.OrderBy(w => w.DtLimiteEntregaProposta)          //EGS 12.06.2020 Ordenar por IDProposta recente
                .OrderByDescending(w => w.IdOportunidade)
                .ToList();
            foreach (var oportunidade in oportunidades)
            {
                var opResponsavel = db.OportunidadeResponsavel.Where(w => w.IdOportunidade == oportunidade.IdOportunidade).ToList();

                if (opResponsavel.Count == 0)
                {
                    allOportunidades.Add(oportunidade);
                }
            }
            #endregion

            #region Oportunidades sem data limite
            var oportunidadesSemDataLimite = db.Oportunidade
                .Where(p => p.DtLimiteEntregaProposta == null)
              //.OrderBy(w => w.IdProposta)                    //EGS 12.06.2020 Ordenar por IDProposta recente
                .OrderByDescending(w => w.IdProposta)         
                .ToList();
            foreach (var opSemDataLimite in oportunidadesSemDataLimite)
            {
                var opResp = db.OportunidadeResponsavel.Where(w => w.IdOportunidade == opSemDataLimite.IdOportunidade).ToList();

                if (opResp.Count == 0)
                {
                    allOportunidades.Add(opSemDataLimite);
                }
            }
            #endregion

            return allOportunidades;
        }

        public Oportunidade BuscarOportunidadeId(int id)
        {

            var item = db.Oportunidade
                .Include(i => i.OportunidadeCliente)
                .Include(i => i.OportunidadeResponsavel)
                .Where(w => w.IdOportunidade == id).FirstOrDefault();

            return item;
        }

        public OportunidadeContato BuscarContatoId(int id)
        {

            var item = db.OportunidadeContato.Where(w => w.IdOportunidadeContato == id).FirstOrDefault();

            return item;
        }

        public void UpdateContato(OportunidadeContato item)
        {
            var itemContato = BuscarContatoId(item.IdOportunidadeContato);

            itemContato.IdOportunidadeContato = item.IdOportunidadeContato;
            itemContato.NmContato = item.NmContato;
            itemContato.NuCelular = item.NuCelular;
            itemContato.NuTelefone = item.NuTelefone;
            itemContato.NmDepartamento = item.NmDepartamento;
            itemContato.CdEmail = item.CdEmail;
            itemContato.IdTipoContato = item.IdTipoContato;


            db.SaveChanges();

        }

        public List<Cliente> BuscarCliente()
        {

            var itens = db.Cliente.ToList();

            return itens;
        }

        public void AddOportunidadeCli(List<OportunidadeCliente> itensCli)
        {

            db.OportunidadeCliente.AddRange(itensCli);

            db.SaveChanges();

        }

        public void ExcluirCliente(int id)
        {

            var itens = db.OportunidadeCliente.Where(w => w.IdOportunidade == id).ToList();

            foreach (var item in itens)
            {
                var cliente = db.Cliente.Where(w => w.IdCliente == item.IdCliente).FirstOrDefault();

                db.Cliente.Remove(cliente);
                db.OportunidadeCliente.Remove(item);
            }

            db.SaveChanges();

        }

        public List<Situacao> BuscarSituacao()
        {

            var itens = db.Situacao.Where(w => w.IcEntidade == "O").ToList();

            return itens;
        }

        public List<Estados> BuscarEstados()
        {
            //EGS 30.08.2020 Trazer todos os estados ORDENADOS, mas primeiro SP
            var itensUF = db.Estados.Where(p => p.Uf == "SP").ToList();
            var semSP   = db.Estados.Where(p => p.Uf != "SP").OrderBy(x => x.Uf).ToList();
            itensUF.AddRange(semSP);
            return itensUF;
        }


        public List<Cidade> BuscarMunicipio()
        {

            var itens = db.Cidade.ToList();

            return itens;
        }


        public List<TipoOportunidade> BuscarTipoOportunidade()
        {

            var itens = db.TipoOportunidade.ToList();

            return itens;
        }



        public List<PessoaFisica> BuscarResponsaveis()
        {

            var itens = db.PessoaFisica.ToList();

            return itens;
        }

        public void AddOportunidadeResp(List<OportunidadeResponsavel> itensResp)
        {

            db.OportunidadeResponsavel.AddRange(itensResp);

            db.SaveChanges();

        }


        public List<TipoContato> BuscarTipoContato()
        {
            var itens = db.TipoContato.ToList();

            return itens;
        }

        public List<TipoContato> ListarTipoContatos(int id)
        {
            var itens = db.TipoContato.Where(w => w.IdTipoContato == id).ToList();

            return itens;
        }

        public void ExcluirResponsavel(int id)
        {

            var itens = db.OportunidadeResponsavel.Where(w => w.IdOportunidade == id).ToList();

            foreach (var item in itens)
            {
                db.OportunidadeResponsavel.Remove(item);
            }

            db.SaveChanges();

        }

        public void AddOportunidade(Oportunidade item)
        {

            db.Oportunidade.Add(item);

            db.SaveChanges();

            var idOp = item.IdOportunidade;


        }


        public void UpdateOportunidade(Oportunidade item)
        {
            var itemOportunidade = BuscarOportunidadeId(item.IdOportunidade);

            itemOportunidade.IdSituacao = item.IdSituacao;
            itemOportunidade.IdTipoOportunidade = item.IdTipoOportunidade;
            itemOportunidade.DsAssunto = item.DsAssunto;
            itemOportunidade.DsObservacao = item.DsObservacao;
            itemOportunidade.DtLimiteEntregaProposta = item.DtLimiteEntregaProposta;
            itemOportunidade.IdUsuarioUltimaAlteracao = AppSettings.constGlobalUserID;

            db.SaveChanges();


        }

        public void RemoveOportunidade(int id)
        {

            ExcluirCliente(id);
            ExcluirResponsavel(id);


            var itemOportunidade = BuscarOportunidadeId(id);
            var itemDocs = db.OportunidadeDocs.Where(w => w.IdOportunidade == id).ToList();
            var itemContatos = db.OportunidadeContato.Where(w => w.IdOportunidade == id).ToList();

            db.OportunidadeDocs.RemoveRange(itemDocs); //remove documentos
            db.OportunidadeContato.RemoveRange(itemContatos); // remove contatos

            db.Oportunidade.Remove(itemOportunidade);

            db.SaveChanges();

        }

        #region | Métodos Documento

        public void AddDocumento(OportunidadeDocs item)
        {

            db.OportunidadeDocs.Add(item);
            db.SaveChanges();


        }

        public List<OportunidadeDocs> BuscarDocumentos(int id)
        {

            var itens = db.OportunidadeDocs.Where(w => w.IdOportunidade == id).ToList();

            return itens;
        }


        public List<TipoDocumento> BuscarTipoDocumentos()
        {

            var itens = db.TipoDocumento.Where(w => w.IdEntidade == 1 || w.IdPrincipais == 3).ToList();

            return itens;
        }


        public void RemoveDocumento(int id)
        {

            var itemDoc = db.OportunidadeDocs.Where(w => w.IdOportunidadeDocs == id).FirstOrDefault();

            db.OportunidadeDocs.Remove(itemDoc);

            db.SaveChanges();

        }

        public OportunidadeDocs BuscarDocumentoId(int id)
        {

            var item = db.OportunidadeDocs
                .Where(w => w.IdOportunidadeDocs == id).FirstOrDefault();

            return item;
        }

        public List<OportunidadeCliente> GetAll()
        {
            var propostaCliente = db.OportunidadeCliente
                .Include(i => i.IdClienteNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.IdCidadeNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdCidadeNavigation)
                .ToList();

            return propostaCliente;
        }

        #endregion

        #region || Métodos de Contatos

        public void AddContato(OportunidadeContato item)
        {

            db.OportunidadeContato.Add(item);
            db.SaveChanges();


        }

        public List<OportunidadeContato> BuscarContato(int id)
        {

            var itens = db.OportunidadeContato.Where(w => w.IdOportunidade == id).ToList();

            return itens;
        }

        public void RemoverContato(int id)
        {
            var itemContato = db.OportunidadeContato.FirstOrDefault(_ => _.IdOportunidadeContato == id);

            if (itemContato != null)
            {
                db.OportunidadeContato.Remove(itemContato);
                db.SaveChanges();
            }
        }

        #endregion
    }
}