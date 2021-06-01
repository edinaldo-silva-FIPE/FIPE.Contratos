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
        public OutPutDoctoAditivoPrincipal BuscaArquivosAditivos(int id)
        {
            OutPutDoctoAditivoPrincipal lstListaDocs = new OutPutDoctoAditivoPrincipal();

            var lstTipoDoc = db.TipoDocumento.Where(x => x.DsTipoDoc.Contains("ADITIVO") && x.DsTipoDoc.Contains("FINAL")).FirstOrDefault();
            if (lstTipoDoc != null)
            {
                var itemDocs = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == lstTipoDoc.IdTipoDoc).OrderBy(x => x.DtUpLoad).FirstOrDefault();
                if (itemDocs == null)
                {
                    //EGS 30.03.2021 - Se não achou como ADITIVO FINAL, procura apenas como ADITIVO, ID 23
                    var itemDocs23 = db.ContratoDocPrincipal.Where(w => w.IdContrato == id && w.IdTipoDoc == 23).OrderBy(x => x.DtUpLoad).FirstOrDefault();
                    if (itemDocs23 != null)
                    {
                        var IDTipoDoc23 = new bTipoDocumento(db).GetById(itemDocs23.IdTipoDoc);

                        lstListaDocs.IdContratoDocPrincipal  = itemDocs23.IdContratoDocPrincipal;
                        lstListaDocs.IdContrato              = itemDocs23.IdContrato;
                        lstListaDocs.NmDocumento             = itemDocs23.NmDocumento;
                        lstListaDocs.DtUpLoad                = itemDocs23.DtUpLoad.ToString();
                        lstListaDocs.NmCriador               = itemDocs23.NmCriador;
                        lstListaDocs.DocFisico               = itemDocs23.DocFisico;
                        lstListaDocs.DsTipoDocumento         = IDTipoDoc23.DsTipoDoc; //itemDocs23.IdTipoDocNavigation.DsTipoDoc;
                    }
                } else {

                    var IDTipoDoc = new bTipoDocumento(db).GetById(itemDocs.IdTipoDoc);
                    lstListaDocs.IdContratoDocPrincipal = itemDocs.IdContratoDocPrincipal;
                    lstListaDocs.IdContrato             = itemDocs.IdContrato;
                    lstListaDocs.NmDocumento            = itemDocs.NmDocumento;
                    lstListaDocs.DtUpLoad               = itemDocs.DtUpLoad.ToString();
                    lstListaDocs.NmCriador              = itemDocs.NmCriador;
                    lstListaDocs.DocFisico              = itemDocs.DocFisico;
                    lstListaDocs.DsTipoDocumento        = IDTipoDoc.DsTipoDoc;     //itemDocs.IdTipoDocNavigation.DsTipoDoc;
                }
            }
            return lstListaDocs;
        }
    }
}