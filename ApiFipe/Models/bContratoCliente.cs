using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContratoCliente
    {
        public FIPEContratosContext db { get; set; }

        public bContratoCliente(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<ContratoCliente> BuscarClienteFisico(int id)
        {
            var item = db.ContratoCliente  
                .Include(i => i.IdClienteNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdEsferaEmpresaNavigation)
                .Where(w => w.IdContrato == id).ToList();
            return item;           
        }

        public List<ContratoCliente> GetAll()
        {
            var ContratoCliente = db.ContratoCliente
                .Include(i => i.IdClienteNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaFisicaNavigation.IdCidadeNavigation)
                .Include(i => i.IdClienteNavigation.IdPessoaNavigation.IdPessoaJuridicaNavigation.IdCidadeNavigation)
                .ToList();

            return ContratoCliente;
        }

        public ContratoCliente GetById(int id)
        {
            var ContratoCliente = db.ContratoCliente
                .Where(w => w.IdContratoCliente == id)
                .Single();

            return ContratoCliente;
        }

        public List<ContratoCliente> GetByIdContrato(int id)
        {
            var ContratoCliente = db.ContratoCliente
                .Where(w => w.IdContrato == id)
                .ToList();

            return ContratoCliente;
        }

        public ContratoCliente GetByContratoPessoa(int idContrato, int idCliente)
        {
            var ContratoCliente = db.ContratoCliente
                .Where(w => w.IdContrato == idContrato && w.IdContratoCliente == idCliente)
                .FirstOrDefault();

            return ContratoCliente;
        }

        public ContratoCliente GetByContratoCliente(int idContrato, int idCliente)
        {
            var ContratoCliente = db.ContratoCliente
                .Where(w => w.IdContrato == idContrato && w.IdCliente == idCliente)
                .FirstOrDefault();

            return ContratoCliente;
        }

        public List<ContratoCliente> GetByContrato(int id)
        {
            var ContratoCliente = db.ContratoCliente
                .Where(w => w.IdContrato == id).ToList();

            return ContratoCliente;
        }
    }
}