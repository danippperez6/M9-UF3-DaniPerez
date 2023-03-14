using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.WindowsAPICodePack.Net;
using System.Xml;
using HtmlAgilityPack;

namespace M9_UF3_P1_DaniPerez
{
    public partial class Form1 : Form
    {
      
        public Form1()
        {
        InitializeComponent();
        this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string UserName = Environment.UserName;
            UsernameData.Text = UserName;
            string HostName = Dns.GetHostName();
            HostnameData.Text = HostName;

            //MAc
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    string MAC = nic.GetPhysicalAddress().ToString();
                    MacData.Text = MAC;
                }
            }

            string IpHost = Dns.GetHostByName(HostName).AddressList[0].ToString();
            IphostData.Text = IpHost;

            //Ssid
            var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected);
            foreach (var network in networks)
            {

                if (network.IsConnected)
                {
                    SsidStatus.Text = "Connected";
                    SsidStatus.ForeColor = Color.Green;

                }
                if (!network.IsConnected)
                {
                    SsidStatus.Text = "Disconnected";
                    SsidStatus.ForeColor = Color.Red;
                    SsidName.Text = "No network detected";
                    SsidName.ForeColor = Color.Red;
                }
                SsidName.Text = network.Name;

            }

            //Ping
            Ping ping = new Ping();
            string cloudflare = "1.1.1.1";
            byte[] Byte = new byte[32];
            int timeout = 1000;
            int pingcounter = 0;
            for (int i = 0; i < 4; i++)
            {
                PingReply reply = ping.Send(cloudflare, timeout, Byte, new PingOptions());

                if (reply.Status == IPStatus.Success)
                {
                    pingcounter++;

                    if (pingcounter.Equals(4))
                    {
                        PingTextData.Text = "Established";
                        PingTextData.ForeColor = Color.Green;
                    }
                    if (pingcounter > 0 && pingcounter < 4)
                    {
                        PingTextData.Text = "Unstable";
                        PingTextData.ForeColor = Color.Yellow;
                    }
                }
                if (reply.Status != IPStatus.Success)
                {
                    PingTextData.Text = "Disconnected";
                    PingTextData.ForeColor = Color.Red;
                }
            }
            //IPGateway


            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                GatewayIPAddressInformationCollection addresses = adapterProperties.GatewayAddresses;
                if (addresses.Count > 0)
                {

                    foreach (GatewayIPAddressInformation address in addresses)
                    {
                        IpGatewayData.Text = address.Address.ToString();
                    }

                }

            }

            //VersionVB
            string cmdVBtext;
            //
            cmdVBtext = "/C" + "\"C:\\Program Files\\Oracle\\virtualbox\\VBoxManage.exe\" --version";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = cmdVBtext;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            //
            string versionVB = "";
            while (!process.HasExited)
            {
                versionVB = versionVB + process.StandardOutput.ReadToEnd();
            }

            VBInstalledData.Text = versionVB;
            if (VBInstalledData.Equals(""))
            {
                VBInstalledData.Text = "No";
                VBInstalledData.ForeColor = Color.Red;
                VbVersionDataUp.Text = "N/A";
                VbVersionDataUp.ForeColor = Color.Red;
            }

            //Webscrapping

            string urlVB = "https://www.virtualbox.org/wiki/Downloads";
            string lastVersionVB = "";

            WebClient client = new WebClient();
            
                string html = client.DownloadString(urlVB);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                HtmlNode versionNode = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'downloadTable')]/tbody/tr[1]/td[1]");
                string latestVersion = versionNode?.InnerText.Trim();

                lastVersionVB = latestVersion;
           

            if (versionVB == latestVersion)
            {
                VbVersionDataUp.Text = versionVB + " " + "---Last version installed---";
                VbVersionDataUp.ForeColor = Color.Green;
            }
            else
            {
                VbVersionDataUp.Text = versionVB + " " + "---Outdated---";
                VbVersionDataUp.ForeColor = Color.Orange;
            }

        }

    }
}
