using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Models.bProposta;

namespace ApiFipe.Models
{
    public class bPropostaCliente
    {
        public FIPEContratosContext db { get; set; }

        public bPropostaCliente(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<PropostaCliente> GetAll()
        {
            var propostaCliente = db.PropostaCliente
                .Include(i => i.IdClienteNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.IdCidadeNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdCidadeNavigation)
                .ToList();

            return propostaCliente;
        } 

        public PropostaCliente GetById(int id)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdPropostaCliente == id)
                .Single();

            return propostaCliente;
        }

        public List<PropostaCliente> GetByIdProp(int id)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdProposta == id)
                .ToList();

            return propostaCliente;
        }

        public PropostaCliente GetByPropostaPessoa(int idProposta, int idCliente)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdProposta == idProposta && w.IdCliente == idCliente)
                .FirstOrDefault();

            return propostaCliente;
        }

        public PropostaCliente GetByPropostaCliente(int idProposta, int idCliente)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdProposta == idProposta && w.IdCliente == idCliente)
                .FirstOrDefault();

            return propostaCliente;
        }

        public List<PropostaCliente> GetByProposta(int id)
        {
            var propostaCliente = db.PropostaCliente
                .Where(w => w.IdProposta == id).ToList();                

            return propostaCliente;
        }
   
    }
}