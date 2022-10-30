using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace OracleDB
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            DBConnection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {                    
                    await context.Response.WriteAsync("Welcome to .NetCore!");
                });
            });
            
            
        }
        public void DBConnection()
        {
            string oradb = "Data Source = (DESCRIPTION = " +
            "(ADDRESS = (PROTOCOL = TCP)(HOST = 10.100.82.226)(PORT = 1521))" +
            "(CONNECT_DATA =" +
            "  (SERVER = fndbqa)" +
            "  (SERVER_NAME = fndbqa) " +
            "  (SERVICE_NAME = fndbqa) " +
            ")" +
            ");User Id = soumya;password=soumya;";
            OracleConnection OConn = new OracleConnection(oradb);
            try
            {
                if (OConn.State != ConnectionState.Open)
                {
                    OConn.Open();
                    Console.WriteLine("DB opened");
                }
            }
            catch (OracleException ex)
            {
                if (OConn.State == ConnectionState.Open)
                {
                    OConn.Close();
                    //throw new Exception(ex.Message);
                }
                Console.WriteLine(ex.Message.ToString());
                sendwarningmail(ex.Message.ToString());
            }
        }
        public void sendwarningmail( string exception)
        {
            try
            {
                SendHtmlFormattedEmail( "Dataabse Dwon", exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
       
        public void SendHtmlFormattedEmail( string subject, string body)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("no-reply@mrhe.com");// new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.To.Add(new MailAddress("schandanala@mbrhe.ae"));
                    mailMessage.To.Add(new MailAddress("finesseme945@gmail.com"));

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com"; //ConfigurationManager.AppSettings["SMTPHost"];
                    smtp.EnableSsl = true; //Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSLENABLED"]);
                    smtp.Credentials = new NetworkCredential("gemtestdubai@gmail.com", "Gems@2020.");
                    smtp.Port = 465; //Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //smtp.UseDefaultCredentials = false;
                    smtp.Send(mailMessage);

                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
