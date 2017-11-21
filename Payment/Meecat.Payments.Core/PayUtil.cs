using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Meecat.Payments.Core
{
    public static class PayUtil
    {
        public static string GetAlipayUrl(bool sandbox)
        {
            //product: https://mapi.alipay.com/gateway.do, https://www.alipay.com/cooperate/gateway.do, https://openapi.alipay.com/gateway.do

            return sandbox ?
                "https://openapi.alipaydev.com/gateway.do" :
                "https://mapi.alipay.com/gateway.do";
        }

        public static string Sign(this IEnumerable<string> source, string key)
        {
            var ps = string.Join("&", BubbleSort(source));

            ps += key;

            return ps.MD5();
        }

        public static string Sign(this NameValueCollection source, string key)
        {
            return Sign(source.ToDictionary(), key);
        }

        public static string Sign<T>(this IDictionary<string, T> source, string key)
        {
            var ps = string.Join("&", source.Keys.BubbleSort()
                .Select(x => string.Format("{0}={1}", x, source[x])));

            ps += key;

            var md5 = ps.MD5();

            return md5;
        }

        private static string[] BubbleSort(this IEnumerable<string> source)
        {
            var values = source.ToArray();
            for (int index1 = 0; index1 < values.Length; ++index1)
            {
                bool flag = false;
                for (int index2 = values.Length - 2; index2 >= index1; --index2)
                {
                    if (string.CompareOrdinal(values[index2 + 1], values[index2]) < 0)
                    {
                        string str = values[index2 + 1];
                        values[index2 + 1] = values[index2];
                        values[index2] = str;
                        flag = true;
                    }
                }
                if (!flag)
                    break;
            }
            return values;
        }


        public static Dictionary<string, string> ToDictionary(this NameValueCollection source)
        {
            return source.AllKeys.SelectMany(
                source.GetValues,
                (k, v) => new KeyValuePair<string, string>(k, v))
                .ToDictionary(k => k.Key, v => v.Value);
        }

        public static string ToQueryString(this NameValueCollection collection)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < collection.Count; i++)
            {
                string curItemStr = string.Format("{0}={1}", collection.Keys[i],
                                                               collection[collection.Keys[i]]);
                if (i != 0)
                {
                    sb.Append("&");
                }
                sb.Append(curItemStr);
            }

            return sb.ToString();
        }

        public static string MD5(this string s)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                {
                    //builder.Append(b.ToString("x2").ToLower());
                    builder.Append(b.ToString("x").PadLeft(2, '0'));
                }

                return builder.ToString();
            }
        }

        public static string HttpGet(string url, int timeout = 120000)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = timeout;
                using (var streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), Encoding.Default))
                {
                    var stringBuilder = new StringBuilder();
                    while (-1 != streamReader.Peek())
                    {
                        stringBuilder.Append(streamReader.ReadLine());
                    }

                    return stringBuilder.ToString();
                }

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


    }
}
