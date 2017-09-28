using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WcfServiceLibrary1
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
        private HtmlNode bodyNode;
        private HtmlNodeCollection collection;

        private void InitializationOfCollectionNode()
        {
            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            bodyNode = doc.DocumentNode.SelectSingleNode("//span[@class='current-weather__col current-weather__info']");
            doc.LoadHtml(bodyNode.InnerHtml);
            collection = doc.DocumentNode.SelectNodes("//div");
        }

        private async Task Initialization()
        {
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                var task = myHttpWebRequest.GetResponseAsync();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)await task;
                StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
                html = myStreamReader.ReadToEnd();
                //InitializationOfCollectionNode();

                Thread thread = new Thread(InitializationOfCollectionNode);
                thread.Start();
                thread.Join();
            }
            catch (Exception)
            {
                throw new Exception("Ошибка инициализации:(");
            }

        }
        private string GetTemperature()
        {
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode bodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='current-weather__thermometer current-weather__thermometer_type_now']");
            return bodyNode.InnerText.Remove(3, 1);
        }

        private string GetSpeed()
        {

            if (collection != null)
            {
                return collection[1].InnerText;
            }
            else throw new Exception("Не удалось получить данные:(");

        }

        private string GetPressure()
        {

            if (collection != null)
            {
                return collection[3].InnerText;
            }
            else throw new Exception("Не удалось получить данные:(");
        }

        private string GetData()
        {

            if (collection != null)
            {
                return collection[4].InnerText;
            }
            else throw new Exception("Не удалось получить данные:(");
        }

        private string GetHumadity()
        {

            if (collection != null)
            {
                return collection[2].InnerText;
            }
            else throw new Exception("Не удалось получить данные:(");
        }

        private WheatherService()
        {

        }


        public async Task<WheatherInfo> GetWhetherInfo(string city)
        {
            //Console.WriteLine("Last location: {0}", LastLocation);
            this.city = city;
            url += city;
            WheatherInfo result = new WheatherInfo();
            await Initialization();

            result.temperature = "Сейчас: " + GetTemperature();
            result.speed = GetSpeed();
            result.humadity = GetHumadity();
            result.pressure = GetPressure();
            result.lastUpdate = GetData();

            return result;
        }


        static void Main()
        {
            var host = new ServiceHost(typeof(WheatherService));
            host.Open();
            Console.WriteLine("Press ENTER to stop the service");
            Console.ReadLine();
        }

    }
}
