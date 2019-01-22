using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;

namespace Authentication
{

    public static class IPMac
    {
        ///////////////////////////////
        //返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        public static NetworkInterface[] NetCardInfo()
        {
          return NetworkInterface.GetAllNetworkInterfaces();
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
          List<string> macs =new List<string>();
          NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
          foreach (NetworkInterface ni in interfaces)
          {
            macs.Add(ni.GetPhysicalAddress().ToString());
          }
          return macs;
        }

        public static string GetEternetPhysicalAddress()
        {
            //List<string> macs = new List<string>();
            string mac = "";
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType.ToString() == "Ethernet")
                    mac = ni.GetPhysicalAddress().ToString();
                //macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return mac;
        }
        
        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        [DllImport("ws2_32.dll")]
        private static extern int inet_addr(string cp);
        [DllImport("IPHLPAPI.dll")]
        private static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 pMacAddr, ref Int32 PhyAddrLen);

       public  static string GetIP()
        {
            string ip = "";
            string hostname = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(hostname);
            //string hostinfo = [0].ToString();
            for (int i = 0; i < ipaddress.Length; i++)
            {
                string lip = ipaddress[i].ToString();
                if (lip.IndexOf('.') > 0)
                    ip = lip.ToString();
            }
            return ip;
        }

       public static string GetMacAddress()
       {
           string macadress = "";
           ManagementObjectSearcher nisc = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
           foreach (ManagementObject nic in nisc.Get())
           {
               if (Convert.ToBoolean(nic["ipEnabled"]) == true)
               {
                   Console.WriteLine("{0} - {1}", nic["ServiceName"], nic["MACAddress"]);
               }
           }
           return macadress;

       }

        public static string GetMacAddress(string hostip)//获取远程IP（不能跨网段）的MAC地址
        {
            string Mac = "";
            try
            {
                Int32 ldest = inet_addr(hostip); //将IP地址从 点数格式转换成无符号长整型
                Int64 macinfo = new Int64();
                Int32 len = 6;
                SendARP(ldest, 0, ref macinfo, ref len);
                string TmpMac = Convert.ToString(macinfo, 16).PadLeft(12, '0');//转换成16进制　　注意有些没有十二位
                Mac = TmpMac.Substring(0, 2).ToUpper();//
                for (int i = 2; i < TmpMac.Length; i = i + 2)
                {
                    Mac = TmpMac.Substring(i, 2).ToUpper() + "-" + Mac;
                }
            }
            catch (Exception Mye)
            {
                Mac = "获取远程主机的MAC错误：" + Mye.Message;
            }
            return Mac;
        }

        
    }
  
    
}