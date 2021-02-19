using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiFipe.Utilitario;


namespace ApiFipe
{
    public class AutenticacaoActionFilter : IActionFilter
    {
        GravaLog _GLog = new GravaLog();

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            /* ===========================================================================================
            *  Edinaldo FIPE
            *  Agosto/2020 
            *  Usuario global para gravar nas tabelas. Hoje pega usuario incorreta em diversas transacoes
            ===========================================================================================*/
            AppSettings.constGlobalUserID = 0;
            using (var db = new FIPEContratosContext())
            {
                var usuarioValido = new bUsuarioToken(db).ValidaUsuarioByToken(context.HttpContext.Request.Headers["Token"]);

                if (usuarioValido)
                {
                    AppSettings.constGlobalUserID = new bUsuarioToken(db).RetornaIDUsuarioByToken(context.HttpContext.Request.Headers["Token"]);
                  //_GLog._GravaLog(AppSettings.constGlobalUserID, "AutenticacaoActionFilter com usuario  [" + AppSettings.constGlobalUserID + "]  encontrado");
                }
                else { 
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new ContentResult()
                    {
                        Content = "401 Unathourized."
                    };
                }
            }
        }
    }
}
