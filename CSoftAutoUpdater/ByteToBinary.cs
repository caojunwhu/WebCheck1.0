using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSoftAutoUpdater
{
    public class CommonUtil
    {
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
/// <summary>
         /// 
         /// </summary>
         /// <param name="x"></param>
         /// <returns></returns>
         private static string ToHex(string x)
         {
             switch (x)
             {
                 case "10":
                     return "A";
                 case "11":
                     return "B";
                 case "12":
                     return "C";
                 case "13":
                     return "D";
                 case "14":
                     return "E";
                 case "15":
                     return "F";
                 default:
                     return x;
             }
         }


        /** 
         * byte数组转换成16进制字符串 
         * @param src 
         * @return 
         */
        public static String bytesToHexString(byte[] src)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (src == null || src.Length <= 0)
            {
                return null;
            }
            for (int i = 0; i < src.Length; i++)
            {
                int v = src[i] & 0xFF;
                String hv = ToHex(Convert.ToString(v));
                if (hv.Length < 2)
                {
                    stringBuilder.Append(0);
                }
                stringBuilder.Append(hv);
            }
            return stringBuilder.ToString();
        }
    }


    public class ByteToBinary
    {
        /**
         * 把byte数组转化成2进制字符串
         * @param bArr
         * @return
         */
        public String getBinaryStrFromByteArr(byte[] bArr){
        String result ="";
        foreach(byte b in bArr ){
            result += getBinaryStrFromByte(b);
        }
        return result;  
    }
        /**
         * 把byte转化成2进制字符串
         * @param b
         * @return
         */
        public String getBinaryStrFromByte(byte b)
        {
            String result = "";
            byte a = b; ;
            for (int i = 0; i < 8; i++)
            {
                byte c = a;
                a = (byte)(a >> 1);//每移一位如同将10进制数除以2并去掉余数。
                a = (byte)(a << 1);
                if (a == c)
                {
                    result = "0" + result;
                }
                else
                {
                    result = "1" + result;
                }
                a = (byte)(a >> 1);
            }
            return result;
        }

        /**
         * 把byte转化成2进制字符串
         * @param b
         * @return
         */
        public String getBinaryStrFromByte2(byte b)
        {
            String result = "";
            byte a = b; ;
            for (int i = 0; i < 8; i++)
            {
                result = (a % 2) + result;
                a = (byte)(a >> 1);
            }
            return result;
        }

        /**
         * 把byte转化成2进制字符串
         * @param b
         * @return
         */
        public String getBinaryStrFromByte3(byte b)
        {
            String result = "";
            byte a = b; ;
            for (int i = 0; i < 8; i++)
            {
                result = (a % 2) + result;
                a = (byte)(a / 2);
            }
            return result;
        }
    }
}
