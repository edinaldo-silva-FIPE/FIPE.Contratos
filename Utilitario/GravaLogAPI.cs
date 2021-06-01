using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using System.Configuration;
using ApiFipe.Models;
using ApiFipe.Models.Context;
using ApiFipe.Utilitario.Extensoes;
//using static ApiFipe.Models.bLog;


namespace ApiFipe.Utilitario
{
    public class GravaLog
    {
        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Mai/2020 
        *  Métodos para gravar em arquivo texto imediatamente
        ===========================================================================================*/
        public void _GravaLog(int pIDUsuario, string pMensagem)
        {
            //EGS 30.08.2019 - Remover quebra de linha
            pMensagem = SubstituirCaracter._RemoveCaracterSpecial(pMensagem);
            if (pMensagem.Length > 5000) { pMensagem = pMensagem.Substring(0, 4999); }
            if (pIDUsuario == 0) { pIDUsuario = AppSettings.constGlobalUserID;  }
            try
            {
                using (var db = new FIPEContratosContext())
                {
                    var llog = new bLog(db).InsereLog(pIDUsuario, pMensagem);
                }
            }
            catch (Exception ex)
            {
                _GravaTxtLog(pIDUsuario, pMensagem + "--" + ex.Message + " " + ex.InnerException );
            }
        }






        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Mai/2020 
        *  Métodos para gravar em arquivo texto imediatamente
        ===========================================================================================*/
        public void _GravaTxtLog(int pIDUsuario, string pMensagem)
        {
            //---------------------------------------------------------- Pega o Diretorio do Servidor ---------------------------------------------------------
            string camDrive    = AppDomain.CurrentDomain.BaseDirectory + "\\LogEventos\\";
            if (camDrive.Contains("D:\\SISTEMAS\\FIPE\\FIPE.Contratos")) { camDrive = "C:\\TEMP\\LogEventos\\"; }
            string NomeArquivo = camDrive + "SGPC_" + DateTime.Now.ToString("yyyyMM") + ".LOG";

            if (!Directory.Exists(camDrive))
            {
                //Criamos um com o nome folder
                Directory.CreateDirectory(camDrive);
            }
            if (!File.Exists(NomeArquivo))
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@NomeArquivo, true))
                {
                    writer.WriteLine("+------------------------------------------------------------------------------------------------------------------------------------------------------------------------+");
                    writer.WriteLine("|                                                                                                                                                                        |");
                    writer.WriteLine("|                                                                SGPC - SISTEMA DE CONTROLE DE CONTRATOS                                                                 |");
                    writer.WriteLine("|                                                                      LOG EVENTOS:   " + DateTime.Now.ToString("dd/MM/yyyy") + "                                                                         |");
                    writer.WriteLine("|                                                                                                                                                                        |");
                    writer.WriteLine("+------------------------------------------------------------------------------------------------------------------------------------------------------------------------+");
                    writer.WriteLine("|         DATA        | ID Usu |                                                                                                                                         |");
                  //writer.WriteLine("| 30/04/2020 18:59:59 | ID Usu | 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345 |");
                    writer.WriteLine("+------------------------------------------------------------------------------------------------------------------------------------------------------------------------+");
                    writer.Close();
                }
            }

            //EGS 30.01.2019 - Remover quebra de linha
            if (pIDUsuario == 0) { pIDUsuario = AppSettings.constGlobalUserID; }

            string pUsuario = pIDUsuario.ToString().PadLeft(6,'0') + " | ";
            var tab   = '\u0009';
            pMensagem = pMensagem.Replace("  ", " ");
            pMensagem = pMensagem.Replace("=\r\n", "");
            pMensagem = pMensagem.Replace(";\r\n", "");
            pMensagem = pMensagem.Replace("\r\n", "");
            pMensagem = pMensagem.Replace("\t", " ");
            pMensagem = pMensagem.Replace(tab.ToString(), "");

            try
            {

                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@NomeArquivo, true))
                {
                    writer.WriteLine("| " + DateTime.Now.ToString() + " | " + pUsuario + pMensagem.PadRight(135, ' ') + " |");
                    writer.Close();
                    //Limpando a referencia dele da memória
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                _GravaTxtLog(pIDUsuario, pMensagem);
            }
        }


    }
}