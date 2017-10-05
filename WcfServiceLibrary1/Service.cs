using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WcfService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple)]

    public class WheatherService : IWheatherService
    {
        private string url = "https://yandex.by/pogoda/";
        private string city;
        private string html;
        private HtmlDocument doc;

        private async Task<bool> Initialization()
        {
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                var task = myHttpWebRequest.GetResponseAsync();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)await task;
                StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
                html = myStreamReader.ReadToEnd();
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
        
                return true;
            }
            catch (Exception)
            {
                return false;
                throw new Exception("Ошибка инициализации:(");
            }
        }

        private string GetTemperature()
        {
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//span[@class='temp__value']");
            return bodyNode.InnerText;
        }

        private string GetSpeed()
        {
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//dl[@class='term term_orient_v fact__wind-speed']");
            return bodyNode.InnerText.Insert(5,": ");
        }

        private string GetPressure()
        {
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//dl[@class='term term_orient_v fact__pressure']");
            return bodyNode.InnerText.Insert(8,": ");
        }

        private string GetHumadity()
        {
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//dl[@class='term term_orient_v fact__humidity']");
            return bodyNode.InnerText.Insert(9,": ");
        }

        public async Task<WheatherInfo> GetWhetherInfo(string city)
        {
            this.city = city;
            url += city;
            WheatherInfo result = new WheatherInfo();
            await Initialization();

            result.temperature = "Сейчас: " + GetTemperature();
            result.speed = GetSpeed();
            result.humadity = GetHumadity();
            result.pressure = GetPressure();
       
            return result;
        }

        public async Task<string> GetWhetherInfoSerialized(string city)
        {
            this.city = city;
            url += city;
            WheatherInfo result = new WheatherInfo();
            if (await Initialization())
            {
                result.temperature = GetTemperature();
                result.speed = GetSpeed();
                result.humadity = GetHumadity();
                result.pressure = GetPressure();
                string resultStr;
                using (var output = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
                    {
                        var dataContractSerializer = new DataContractSerializer(typeof(WheatherInfo));
                        dataContractSerializer.WriteObject(writer, result);
                        resultStr = output.GetStringBuilder().ToString();
                    }
                }
                return resultStr;
            }
            return null;
        }

        static void Main()
        {
            var host = new ServiceHost(typeof(WheatherService));
            host.Open();
            host.Close();
        }

        public async void SendEmailAsync(string city, string receiversAddress)
        {
            string sendersAddress = "olencka11@yandex.ru";
            receiversAddress = "sichnenkoolga@gmail.com";
            const string sendersPassword = "sichnenkoolay1998";
            const string subject = "WeatherInfo";
            var result = await GetWhetherInfo(city);
            string body = "Здравствуйте!" + Environment.NewLine;
            body += "В " + city + " " + result.temperature + Environment.NewLine;
            body += result.speed + Environment.NewLine;
            body += result.pressure + Environment.NewLine;
            body += result.humadity + Environment.NewLine;
            body += result.lastUpdate;
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.yandex.ru ",
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(sendersAddress, sendersPassword),
                Timeout = 3000
            };

            MailMessage message = new MailMessage(sendersAddress, receiversAddress, subject, body);
            await smtp.SendMailAsync(message);
        }

    }
}



