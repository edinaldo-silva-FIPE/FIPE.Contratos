using ApiFipe.Controllers;
using ApiFipe.Models.Context;
using System.Collections.Generic;
using System.Linq;

namespace ApiFipe.Models
{
    public class bContratoDoc
    {
        public FIPEContratosContext db { get; set; }

        public bContratoDoc(FIPEContratosContext db)
        {
            this.db = db;
        }

        public ContratoDoc GetById(int id)
        {
            var contratoDoc = db.ContratoDoc
                .Where(w => w.IdContratoDoc == id)
                .Single();

            return contratoDoc;
        }

        public ContratoDoc GetPropostaMinutaByContrato(int idContrato)
        {
            var contratoDoc = db.ContratoDoc
                         .Where(p => p.IdContrato == idContrato && p.IdTipoDoc == 3)
                         .FirstOrDefault();

            return contratoDoc;
        }

        public ContratoDocPrincipal GetPropostaFinalByContrato(int idContrato)
        {
            var contratoDoc = db.ContratoDocPrincipal
                .Where(p => p.IdContrato == idContrato && p.IdTipoDoc == 4)
                .FirstOrDefault();

            return contratoDoc;
        }

        public ContratoDocPrincipal BuscarTermoId(int id)
        {
            var item = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 1).FirstOrDefault();

            return item;
        }

        public ContratoDocPrincipal BuscarFinalId(int id)
        {
            var item = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 4).FirstOrDefault();

            return item;
        }

        public ContratoDocPrincipal BuscaDocumentoAditivo(int id)
        {
            var item = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 23).FirstOrDefault();

            return item;
        }

        public ContratoDocPrincipal BuscaContratoAssinado(int id)
        {
            var item = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 7).FirstOrDefault();

            return item;
        }

        public ContratoDocPrincipal BuscaOrdemInicio(int id)
        {
            var item = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 16).FirstOrDefault();

            return item;
        }

        /* ===========================================================================================
         *  Edinaldo FIPE
         *  Setembro/2020 
         *  Pesquisa os nomes dos arquivos de ADITIVO do Contrato pelo ID
         ===========================================================================================*/
        public OutPutDocumentoPrincipal BuscaArquivosAditivos(int id)
        {
            OutPutDocumentoPrincipal lstListaDocs = new OutPutDocumentoPrincipal();

            var lstTipoDoc = db.TipoDocumento.Where(x => x.DsTipoDoc.Contains("ADITIVO") && x.DsTipoDoc.Contains("FINAL")).FirstOrDefault();
            if (lstTipoDoc != null)
            {
                var itemDocs = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == lstTipoDoc.IdTipoDoc).OrderBy(x => x.DtUpLoad).FirstOrDefault();
                if (itemDocs != null)
                {
                    lstListaDocs.IdContratoDocPrincipal = itemDocs.IdContratoDocPrincipal;
                    lstListaDocs.IdContrato             = itemDocs.IdContrato;
                    lstListaDocs.NmDocumento            = itemDocs.NmDocumento;
                    lstListaDocs.DtUpLoad               = itemDocs.DtUpLoad.ToString();
                    lstListaDocs.NmCriador              = itemDocs.NmCriador;
                    lstListaDocs.DocFisico              = itemDocs.DocFisico;
                    lstListaDocs.DsTipoDocumento        = itemDocs.IdTipoDocNavigation.DsTipoDoc;
                }
            }
            return lstListaDocs;
        }
    }
}