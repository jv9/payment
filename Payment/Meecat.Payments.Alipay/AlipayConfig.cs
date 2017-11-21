using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace Meecat.Payments.Alipay
{
    public class Config
    {

        //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

        // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public static readonly string partner = "2088621885480248";// "2088621881316963";//"2088101122136241";

        // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public static readonly string key = "ys8bgm4buakbn649cs9nls26qub15ufg";//"mqp5uy1mvt2jaw01sd476x86pad0oxm0";//"760bdzec6y9goq7ctyx96ezkz78287de";

        // 服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
        public static readonly string notify_url = "http://localhost:56039/home/notify";

        // 页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        public static readonly string return_url = "http://localhost:56039/home/return";

        // 签名方式
        public static readonly string sign_type = "MD5";

        // 调试用，创建TXT日志文件夹路径，见AlipayCore.cs类中的LogResult(string sWord)打印方法。
        public static readonly string log_path = HttpRuntime.AppDomainAppPath.ToString() + "log\\";

        // 字符编码格式 目前支持 gbk 或 utf-8
        public static readonly string input_charset = "utf-8";

        // 调用的接口名，无需修改
        public static readonly string service = "create_forex_trade";

    }
}