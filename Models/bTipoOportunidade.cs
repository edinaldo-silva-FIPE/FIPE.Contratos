using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TipoOportunidadeController;

namespace ApiFipe.Models
{
    public class bTipoOportunidade
    {
        public FIPEContratosContext db { get; set; }

        public bTipoOportunidade(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<TipoOportunidade> Get()
        {
            var TipoOportunidades = db.TipoOportunidade.Where(w => w.IcModulo == "O").ToList();

            return TipoOportunidades;
        }

        public List<TipoOportunidade> GetAll()
        {
            var TipoOportunidades = db.TipoOportunidade.ToList();

            return TipoOportunidades;
        }

        public List<TipoOportunidade> GetTipoProposta()
        {
            var TipoOportunidades = db.TipoOportunidade.Where(w => w.IcModulo == "P").ToList();

            return TipoOportunidades;
        }

        public OutPutAddTipoOportunidade AddTipoOportunidade(InputAddTipoOportunidade item)
        {
            var retorno = new OutPutAddTipoOportunidade();

            var TipoOportunidade = new TipoOportunidade();
            TipoOportunidade.DsTipoOportunidade = item.DsTipoOportunidade;
            TipoOportunidade.IcModulo = item.IcModulo;

            db.TipoOportunidade.Add(TipoOportunidade);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public OutPutUpdateTipoOportunidade UpdateTipoOportunidade(InputUpdateTipoOportunidade item)
        {
            var retorno = new OutPutUpdateTipoOportunidade();

            var TipoOportunidade = new bTipoOportunidade(db).BuscaTipoOportunidadeId(item.IdTipoOportunidade);
            TipoOportunidade.DsTipoOportunidade = item.DsTipoOportunidade;
            TipoOportunidade.IcModulo = item.IcModulo;

            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public TipoOportunidade BuscaTipoOportunidadeId(int id)
        {
            var TipoOportunidade = db.TipoOportunidade.Where(w => w.IdTipoOportunidade == id).FirstOrDefault();

            return TipoOportunidade;
        }

        public OutPutRemoveTipoOportunidade RemoveTipoOportunidadeId(int id)
        {
            var retorno = new OutPutRemoveTipoOportunidade();
            var TipoOportunidade = BuscaTipoOportunidadeId(id);

            db.TipoOportunidade.Remove(TipoOportunidade);
            db.SaveChanges();

            return retorno;
        }
    }
}