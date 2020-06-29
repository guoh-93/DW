using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace CPublic
{
    public class CSMTPmail
    {
        public static  bool SendMainl(string mailTo, string strBody)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

            msg.To.Add(mailTo);
            msg.From = new MailAddress("qudonghai@gmail.com", "qudonghai", System.Text.Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "Do you have a workflow to handle/你有一个流程要处理";//邮件标题 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
            msg.Body = strBody;//邮件内容 
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
            msg.IsBodyHtml = false;//是否是HTML邮件 
            msg.Priority = MailPriority.High;//邮件优先级 

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("qudonghai@gmail.com", "nbm8752291");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;//经过ssl加密 
            client.Port = 587;
            object userState = msg;
            try
            {
                client.SendAsync(msg, userState);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }
    }
}
