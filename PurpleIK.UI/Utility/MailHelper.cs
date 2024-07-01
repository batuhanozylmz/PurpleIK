using System.Net.Mail;
using System.Net;
using System.Text;
using PurpleIK.UI.Utility;

namespace PurpleIK.UI.Utility
{
    public static class MailHelper
    {
        //"PurpleIK.123"  --- email şifresi
        //vlsh vwvf drrx jyue    --- uygulama şifresi
        public static void SendMail(string to, string subject, string mailBody, byte[] attachmentData, string attachmentFileName, string from = "purpleikmanager@gmail.com", string? cc = null, string? bcc = null)
        {
            {
                try
                {
                    MailMessage message = new MailMessage(from, to);
                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.BodyEncoding = UTF8Encoding.UTF8;
                    message.Body = mailBody;

                    // Attach the file
                    if (attachmentData != null && attachmentData.Length > 0)
                    {
                        MemoryStream ms = new MemoryStream(attachmentData);
                        message.Attachments.Add(new Attachment(ms, attachmentFileName));
                    }

                    if (!string.IsNullOrWhiteSpace(cc))
                    {

                        var ccList = cc.Split(";");
                        foreach (var _cc in ccList)
                        {
                            message.CC.Add(_cc);
                        }

                    }

                    if (!string.IsNullOrWhiteSpace(bcc))
                    {

                        var bccList = bcc.Split(";");
                        foreach (var _bcc in bccList)
                        {
                            message.Bcc.Add(_bcc);
                        }

                    }

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("purpleikmanager@gmail.com", "vlshvwvfdrrxjyue");

                    client.Send(message);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

    }
}

//// E-posta gönderme
//string mailBody = $"Zimmet Formu Eklendi:<br>Ürün Adı: {p.ProductName}<br>Alma Tarihi: {p.ReceiptDate}";

//byte[] pdfBytes = p.DebitForm;

//// Sabit dosya adı: Zimmet Formu
//string fileName = "Zimmet Formu.pdf";

//// E-posta gönderme işlemi
//string toEmail = p.Person?.PersonalEmail; // Person nesnesinin PersonalEmail alanını al

//MailHelper.SendMail(toEmail, "Zimmet Formu Eklendi", mailBody, null, fileName);

//TempData["SuccessMessage"] = "Zimmet Formu başarıyla eklendi.";