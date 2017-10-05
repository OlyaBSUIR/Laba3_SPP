using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfServiceClient.ServiceReference1;

namespace WcfServiceClient
{
    public partial class frmMain : Form
    {
        WeatherServiceClient client = new WeatherServiceClient();

        public frmMain()
        {
            InitializeComponent();
            timer.Enabled = true;
            timer.Start();
            cbCities.SelectedIndex = 0;
        }

        private async void GetWeather()
        {
            try
            {
                string city = cbCities.Items[cbCities.SelectedIndex].ToString();
                string result = await client.GetWhetherInfoSerializedAsync(city);
                WcfServiceClient.ServiceReference1.WheatherInfo weatherInfo;
                using (Stream stream = new MemoryStream())
                {

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(result);

                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;

                    var dataContractSerializer = new DataContractSerializer(typeof(WcfServiceClient.ServiceReference1.WheatherInfo));

                    weatherInfo= (WcfServiceClient.ServiceReference1.WheatherInfo)dataContractSerializer.ReadObject(stream);

                }
                lbTemperature.Text = "Сейчас: " + weatherInfo.temperature;
                lbSpeed.Text = weatherInfo.speed;
                lbHumadity.Text = weatherInfo.humadity;
                lbPressure.Text = weatherInfo.pressure;
                lbTime.Text = string.Format("Время обновления: {0}", DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                timer.Stop();
                MessageBox.Show(ex.Message);
            }
        }

        private bool isInvalidMail(string email)
        {
            string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            if (Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        private void cbCities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(timer==null)
                timer.Start();
            GetWeather();
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            if (timer == null)
                timer.Start();
            if (!isInvalidMail(tbMail.Text))
            {
                MessageBox.Show("Вы ввели некорректный адрес:("+Environment.NewLine+"Проверьте введенные вами данные и повторите попытку");
                return;
            }
            try
            {                
               client.SendEmailAsync(cbCities.Items[cbCities.SelectedIndex].ToString(), tbMail.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            GetWeather();
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            GetWeather();
     
        }
    }
}
