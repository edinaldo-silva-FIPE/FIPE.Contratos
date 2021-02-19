using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ApiFipe.Models.bFrenteCoordenador;

namespace ApiFipe.Models
{
    public class bLog
    {
        public FIPEContratosContext db { get; set; }

        public bLog(FIPEContratosContext db)
        {
            this.db = db;
        }


        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Agosto/2020 
        *  Insere o ERRO na tabela de LOG
        ===========================================================================================*/
        public bool InsereLog(int pIDUsuario, string pMensagem)
        {
            try
            {
                Log _Log        = new Log();
                _Log.IdUsuario  = pIDUsuario;
                _Log.DsMensagem = pMensagem;
                _Log.DtLog      = DateTime.Now;
                db.Log.Add(_Log);
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }
}
