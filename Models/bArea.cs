using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Models
{
    public class bArea
    {
        public FIPEContratosContext db { get; set; }

        public bArea(FIPEContratosContext db)
        {
            this.db = db;
        }

        //public bool AddTipoVinculo(TipoVinculo item)
        //{
        //    try
        //    {
        //        db.TipoVinculo.Add(item);
        //        db.SaveChanges();
        //        var idCli = item.IdTipoVinculo;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public List<Area> ListaAreas()
        {
            var lstAreas = db.Area.ToList();

            return lstAreas;
        }
        
        public Area BuscaAreaId(int id)
        {
            var area = db.Area.Where(w => w.IdArea == id).FirstOrDefault();

            return area;
        }

        //public bool UpdateTipoVinculo(TipoVinculo item)
        //{

        //    try
        //    {
        //        db.TipoVinculo.Update(item);

        //        db.SaveChanges();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public bool RemoveTipoVinculo(int id)
        //{
        //    try
        //    {
        //        var tipoVinculo = db.TipoVinculo.Where(w => w.IdTipoVinculo == id).FirstOrDefault();

        //        db.TipoVinculo.Remove(tipoVinculo);
        //        db.SaveChanges();

        //        return true;

        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }
        //}
    }
}
