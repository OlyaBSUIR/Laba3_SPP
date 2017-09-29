using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfServiceClient.ServiceReference1;

namespace WcfServiceClient
{
    public partial class frmMain : Form
    {
        private static WhetherServiceClient client = new WhetherServiceClient();

        public frmMain()
        {
            InitializeComponent();
            timer.Enabled = true;
            timer.Start();
            cbCities.SelectedIndex = 0;

        }

        private async void InvokeAsyncMethod()
        {
            string city = cbCities.Items[cbCities.SelectedIndex].ToString();
            string result = await client.GetWhetherInfoSerializedAsync(city);
            WcfServiceClient.ServiceReference1.WheatherInfo weatherInfo;
            try
            {
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
                lbData.Text = weatherInfo.lastUpdate;
                lbTime.Text = string.Format("Время обновления: {0}", DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
            timer.Stop();
        }

        private void cbCities_SelectedIndexChanged(object sender, EventArgs e)
        {
           // InvokeAsyncMethod();
        }

        private void btnMatch_Click(object sender, EventArgs e)
        {
            try
            {
                client.SendEmailAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          //  InvokeAsyncMethod();
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
           //InvokeAsyncMethod();
     
        }
    }
}
