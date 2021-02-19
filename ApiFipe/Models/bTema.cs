using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;
using static ApiFipe.Controllers.TemaController;

namespace ApiFipe.Models
{
    public class bTema
    {
        public FIPEContratosContext db { get; set; }

        public bTema(FIPEContratosContext db)
        {
            this.db = db;
        }

        public List<Tema> Get()
        {
            var temas = db.Tema.ToList();

            return temas;
        }

        public OutPutAddTema AddTema(InputAddTema item)
        {
            var retorno = new OutPutAddTema();

            var tema = new Tema();
            tema.DsTema = item.DsTema;

            db.Tema.Add(tema);
            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public OutPutUpdateTema UpdateTema(InputUpdateTema item)
        {
            var retorno = new OutPutUpdateTema();

            var tema = new bTema(db).BuscaTemaId(item.IdTema);
            tema.DsTema = item.DsTema;

            db.SaveChanges();

            retorno.Result = true;

            return retorno;
        }

        public Tema BuscaTemaId(int id)
        {
            var tema = db.Tema.Where(w => w.IdTema == id).FirstOrDefault();

            return tema;
        }

        public OutPutRemoveTema RemoveTemaId(int id)
        {
            var retorno = new OutPutRemoveTema();
            var tema = BuscaTemaId(id);

            db.Tema.Remove(tema);
            db.SaveChanges();

            return retorno;
        }
    }
}