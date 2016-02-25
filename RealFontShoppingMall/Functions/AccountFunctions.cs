using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace RealFontShoppingMall.Functions
{
    public class AccountFunctions
    {
        // 메일 보내기
        public static string SendMail(string address, string title, string message)
        {
            string SMTPaddress = "smtp.gmail.com";
            string SMTPid = "stoctoct";
            string SMTPpass = "ucoms_0330";
            string senderID = "stoctoct@gmail.com";
            string senderNAME = "리얼폰트고객센터";

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(SMTPaddress);
            mail.From = new MailAddress(senderID, senderNAME, System.Text.Encoding.UTF8);
            mail.To.Add(address);
            mail.Subject = title;
            mail.Body = message;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(SMTPid, SMTPpass);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);

            return "ok";
        }
    }
}