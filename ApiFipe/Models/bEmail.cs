using ApiFipe.Models.Context;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ApiFipe.Utilitario;


namespace ApiFipe.Models
{
    public class bEmail
    {
        GravaLog _GLog = new GravaLog();
        private string sEmailErroDestiny1 = "marcio.rodrigues@fipe.org.br";
        private string sEmailErroDestiny2 = "edinaldo.silva@fipe.org.br";
        private string sEmailErroName     = "Edinaldo, Marcio Rodrigues";

        private FIPEContratosContext db;
        public static string ConnnectionString { get; set; }
        public bEmail(FIPEContratosContext db)
        {
            this.db = db;
        }

        public void EnviarEmail(string[] EmailDestinatario, string[] NomeDestinatario, string Assunto, string Corpo)
        {
            for (int i = 0; i < EmailDestinatario.Length; i++)
            {
                EnviarEmail(EmailDestinatario[i], NomeDestinatario[i], Assunto, Corpo, null, string.Empty);
            }
        }
        public EmailConfigurado GetEmailById(int idEmail)
        {
            var emailConfigurado = db.EmailConfigurado.Where(w => w.IdEmail == idEmail).FirstOrDefault();

            return emailConfigurado;
        }




       /* ===========================================================================================
        *  Edinaldo FIPE
        *  Maio/2020 
        *  Envia o email com destinatario, corpo feito pela TKSI
        ===========================================================================================*/
        public void EnviarEmail(string EmailDestinatario, string NomeDestinatario, string Assunto, string Corpo, byte[] Anexo, string NomeAnexo)
        {
            //EGS 30.05.2020 - Se for Homologacao, então envio email para Edinaldo
            //===========================================================================================================================
            //===========================================================================================================================
            if (!FIPEContratosContext.EnvironmentIsProduction)
                EmailDestinatario = sEmailErroDestiny1;

            if (Assunto.Contains("ERROR"))
                EmailDestinatario = sEmailErroDestiny2;

            SmtpClient cliente       = new SmtpClient("smtp.gmail.com", 587);
            MailAddress remetente    = new MailAddress("noreply@fipe.org.br", "fipe.Contratos");
            MailAddress destinatario = new MailAddress(EmailDestinatario, NomeDestinatario);
            MailMessage mensagem     = new MailMessage(remetente, destinatario);
            if (!FIPEContratosContext.EnvironmentIsProduction)
            {
                MailAddress bcc = new MailAddress(sEmailErroDestiny2);
                mensagem.Bcc.Add(bcc);
            }
            if (Anexo != null)
            {
                byte[] data = Anexo;
                MemoryStream ms = new MemoryStream(data);
                mensagem.Attachments.Add(new Attachment(ms, NomeAnexo));
            }

            //EGS 30.12.2020 Nao mandar nenhum email para Marcio se for HML
            if ((!FIPEContratosContext.EnvironmentIsProduction) && (destinatario.Address.Contains(sEmailErroDestiny1)))
            {
                return;
            }


            mensagem.Body         = Corpo;
            mensagem.IsBodyHtml   = true;
            cliente.EnableSsl     = true;
            cliente.Credentials   = new NetworkCredential("noreply@fipe.org.br", "lata1234");
            if ( FIPEContratosContext.EnvironmentIsProduction) mensagem.Subject = "SGPC-"     + Assunto;
            if (!FIPEContratosContext.EnvironmentIsProduction) mensagem.Subject = "SGPC HML-" + Assunto;
            cliente.Send(mensagem);
            //===========================================================================================================================
            //===========================================================================================================================
        }


        public void EnviarEmailSenha(string EmailDestinatario, string NomeDestinatario, string Assunto, string Corpo, byte[] Anexo, string NomeAnexo)
        {

            if (Assunto.Contains("ERROR"))
                EmailDestinatario = sEmailErroDestiny2;

            SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
            MailAddress remetente = new MailAddress("noreply@fipe.org.br", "fipe.Contratos");
            MailAddress destinatario = new MailAddress(EmailDestinatario, NomeDestinatario);
            MailMessage mensagem = new MailMessage(remetente, destinatario);
            if (!FIPEContratosContext.EnvironmentIsProduction)
            {
                MailAddress bcc = new MailAddress(sEmailErroDestiny2);
                mensagem.Bcc.Add(bcc);
            }
            if (Anexo != null)
            {
                byte[] data = Anexo;
                MemoryStream ms = new MemoryStream(data);
                mensagem.Attachments.Add(new Attachment(ms, NomeAnexo));
            }




            mensagem.Body = Corpo;
            mensagem.IsBodyHtml = true;
            cliente.EnableSsl = true;
            cliente.Credentials = new NetworkCredential("noreply@fipe.org.br", "lata1234");
            if (FIPEContratosContext.EnvironmentIsProduction) mensagem.Subject = "SGPC-" + Assunto;
            if (!FIPEContratosContext.EnvironmentIsProduction) mensagem.Subject = "SGPC HML-" + Assunto;
            cliente.Send(mensagem);
            //===========================================================================================================================
            //===========================================================================================================================
        }

        /* ===========================================================================================
        *  Edinaldo FIPE
        *  Maio/2020 
        *  Envia o email com o erro encontrado - TKSI
        ==============================================================================================*/
        public void EnviarEmailTratamentoErro(Exception ex, string pRotina="")
        {
            if ((!FIPEContratosContext.EnvironmentIsProduction) && (ex.ToString().Contains("Timeout expired"              ))) { return; }
            if ((!FIPEContratosContext.EnvironmentIsProduction) && (ex.ToString().Contains("Sequence contains no elements"))) { return; }

            string sMsgErro = ex.ToString() + " " + ex.InnerException;
            string sSubject = "*ERROR [ID:" + AppSettings.constGlobalUserID + "] - " + pRotina;
            if (sMsgErro.Length > 5001) { sMsgErro = sMsgErro.Substring(0, 5000); }
            string corpo    = " SQL==> " + ConnnectionString.Substring(0, 80) + " " + sMsgErro;

            _GLog._GravaLog(AppSettings.constGlobalUserID, sSubject + corpo);

            try
            {
                var parametro = db.Parametro.FirstOrDefault();
                if (parametro != null)
                {
                    var emails      = parametro.EmailsNotificacao.Split(";");
                    for (int i = 0; i < emails.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(emails[i]))
                        {
                            new bEmail(db).EnviarEmail(emails[i], emails[i], sSubject, corpo, null, string.Empty);
                        }
                    }
                }
                else
                {
                    new bEmail(db).EnviarEmail(sEmailErroDestiny2, sEmailErroName, sSubject + " Sem Parametro em ", corpo, null, string.Empty);
                }
            }
            catch (Exception exx)
            {
                new bEmail(db).EnviarEmail(sEmailErroDestiny2, sEmailErroName, sSubject + " Sem Acesso ao Banco em ", corpo + exx.Message + " " + exx.InnerException, null, string.Empty);
            }

        }
    }
}
