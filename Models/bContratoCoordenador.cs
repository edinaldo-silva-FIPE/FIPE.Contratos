using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContratoCoordenador
    {
        public FIPEContratosContext db { get; set; }

        public bContratoCoordenador(FIPEContratosContext db)
        {
            this.db = db;
        }

        public ContratoCoordenador GetByContratoPessoa(int idContrato, int idPessoa)
        {
            var contratoCoordenador = db.ContratoCoordenador
                .Where(w => w.IdContrato == idContrato && w.IdPessoa == idPessoa)
                .FirstOrDefault();

            return contratoCoordenador;
        }
        public List<ContratoCoordenador> BuscarCoordenador(int id)
        {
            var item = db.ContratoCoordenador
               .Include(i => i.IdPessoaNavigation)
               .Where(w => w.IdContrato == id).ToList();
            return item;
        }

        public List<ContratoCoordenador> ListaCoordenadorGrid(int id)
        {
            var contratoCoordenador = db.ContratoCoordenador
                .Where(w => w.IdContrato == id)
                .ToList();

            return contratoCoordenador;
        }

    }
}