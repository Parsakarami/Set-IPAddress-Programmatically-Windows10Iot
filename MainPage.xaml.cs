using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Xaml;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Networking.Connectivity;
using System.IO;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsIotInternalServiceConsumer
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ChangeIPAddress(string ipAddress)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://127.0.0.1:8080/", UriKind.RelativeOrAbsolute);
            client.DefaultRequestHeaders.Authorization =
             new AuthenticationHeaderValue(
             "Basic",
             Convert.ToBase64String(
             System.Text.ASCIIEncoding.ASCII.GetBytes(
                 string.Format("{0}:{1}", "administrator", "YourPassword"))));

            NetworkConfiguration networkConfiguration = new NetworkConfiguration()
            {
                AdapterName = "{CF2A43ED-F038-4411-BB0D-EA71D5604B5E}",
                IPAddress = ipAddress,
                SubnetMask = "255.255.0.0",
                DefaultGateway = "172.24.10.1",
                PrimaryDNS = "0.0.0.0",
                SecondryDNS = "0.0.0.0"
            };
            string json = JsonConvert.SerializeObject(networkConfiguration);
            HttpContent content = new StringContent(json);
            var result = await client.PutAsync("api/networking/ipv4config", content);
        }

        public class NetworkConfiguration
        {
            public string AdapterName { get; set; }
            public string IPAddress { get; set; }
            public string SubnetMask { get; set; }
            public string DefaultGateway { get; set; }
            public string PrimaryDNS { get; set; }
            public string SecondryDNS { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeIPAddress(txt_IPAddress.Text);
        }

        private void btn_Check_Click(object sender, RoutedEventArgs e)
        {
            lbl_result.Text = GetLocalIp();
        }

        private static string GetLocalIp()
        {
            try
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();

                if (icp?.NetworkAdapter == null) return null;
                var hostname =
                    NetworkInformation.GetHostNames().SingleOrDefault(
                            hn => hn.IPInformation?.NetworkAdapter != null &&
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId);

                return hostname?.CanonicalName; // the ip address
            }
            catch (Exception ex)
            {
                return "127.0.0.1";
            }
        }
    }
}
