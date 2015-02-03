using MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace MvcApplication.AppTool
{
    public static class HttpRequestTool
    {
        //当前项目解决方案路径
        private static string CurrentSolutionPath = System.AppDomain.CurrentDomain.BaseDirectory;
        //private static string CurrentSolutionPath = @"C:\wwwroot\VBS2_7890";
        /// <summary>
        /// 根据url创建连接返回结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type">1、字符串2、byte数组</param>
        /// <returns></returns>
        public static string CreateHttpRequest(string url, int type = 1)
        {
            string strResponse = string.Empty;
            try
            {
                ResultType resultType = ResultType.String;
                byte[] resultByte = null;
                if (type != 1)
                {
                    resultType = ResultType.Byte;
                }
                HttpItem item = new HttpItem()
                {
                    URL = url,
                    ResultType = resultType
                };
                HttpHelper helper = new HttpHelper();
                HttpResult result = helper.GetHtml(item);
                //为图片时则把图片保存并返回图片路径
                if (type != 1)
                {
                    resultByte = result.ResultByte;
                    if (resultByte != null)
                    {
                        strResponse = @"\Content\app\authCodeImgs\" + Guid.NewGuid().ToString() + ".jpg";
                        //strResponse = @"\Mobile\Content\app\authCodeImgs\" + Guid.NewGuid().ToString() + ".jpg";
                        File.WriteAllBytes(CurrentSolutionPath + strResponse, resultByte);
                    }
                }
                else
                {
                    strResponse = result.Html;
                }
            }
            catch (Exception e)
            {
            }
            return strResponse;
        }
        /// <summary>
        /// 根据默认参数和输入参数拼接url
        /// </summary>
        /// <param name="def">默认参数</param>
        /// <param name="entry">输入参数</param>
        /// <returns></returns>
        public static string GetUrl(DefaultParametes def, EntryParametes entry)
        {
            string urlStr = string.Empty;
            string parmStr = string.Empty;
            string propertyName = string.Empty;
            object propertyObj = null;
            string propertyValue = string.Empty;
            //循环读取默认参数类属性，如果有则拼接到url
            foreach (var item in typeof(DefaultParametes).GetProperties())
            {
                propertyName = item.Name;
                propertyObj = item.GetValue(def, null);
                propertyValue = propertyObj == null ? "" : propertyObj.ToString();
                if (propertyValue != "")
                {
                    if (propertyName == "url")
                    {
                        urlStr += propertyValue + "?";
                    }
                    else
                    {
                        parmStr += "&" + propertyName + "=" + propertyValue;
                    }
                }
            }
            //循环读取输入参数类属性，如果有则拼接到url
            foreach (var item in typeof(EntryParametes).GetProperties())
            {
                propertyName = item.Name;
                propertyObj = item.GetValue(entry, null);
                propertyValue = propertyObj == null ? "" : propertyObj.ToString();
                if (propertyValue != "")
                {
                    parmStr += "&" + propertyName + "=" + propertyValue;
                }
            }
            return urlStr + parmStr.Substring(1);
        }
        /// <summary>
        /// 读取XML获取默认参数
        /// </summary>
        /// <param name="type">1、手机2、信用卡</param>
        /// <param name="step">当前步骤</param>
        /// <returns></returns>
        public static DefaultParametes GetDefaultParametes(int type, int step)
        {
            DefaultParametes def = new DefaultParametes();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(CurrentSolutionPath + "MobileParametesConfig.xml");
                //doc.Load(CurrentSolutionPath + @"\Mobile\MobileParametesConfig.xml");
                XmlElement root = doc.DocumentElement;
                foreach (XmlNode node in root.ChildNodes)
                {
                    XmlElement xmlElement = node as XmlElement;
                    switch (xmlElement.Name)
                    {
                        case "appid":
                            def.appId = xmlElement.InnerText;
                            break;
                        case "userid":
                            def.userId = xmlElement.InnerText;
                            break;
                        case "mobile":
                            if (type == 1)
                            {
                                foreach (XmlNode subNode in xmlElement.ChildNodes)
                                {
                                    XmlElement subXmlElement = subNode as XmlElement;
                                    if (Convert.ToInt32(subXmlElement.GetAttribute("step")) == step)
                                    {
                                        def.url = subXmlElement.InnerText.Trim();
                                    }
                                }
                            }
                            break;
                        case "creditcard":
                            break;
                    }
                }
                def.timeunit = GetTimeunit();
                //def.timeunit = "1417584978981";
                def.token = GetToken(def.timeunit);
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
            }
            return def;
        }
        /// <summary>
        /// 根据json字符串序列化成输出参数
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static OutputParametes GetOutputParametes(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<OutputParametes>(json);
        }
        /// <summary>
        /// 根据默认参数输入参数获取结果
        /// </summary>
        /// <param name="def"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static OutputParametes GetInputParametes(DefaultParametes def, EntryParametes entry)
        {
            string url = HttpRequestTool.GetUrl(def, entry);
            string responsStr = HttpRequestTool.CreateHttpRequest(url);
            OutputParametes inp = HttpRequestTool.GetOutputParametes(responsStr);
            return inp;
        }
        /// <summary>
        /// 判断输出结果并返回状态（0、成功1、下一步2、需图片验证码3、需短信验证码）
        ///（1）当errorcode!=0表示错误，不再进行后面的判断，错误代码参考接口请求错误码； 
        ///（2）当errorcode=0表示正确：
        ///      a，当success=false，表示响应失败
        ///      b，当success=true，表示响应成功
        ///      i，当status!=1，表示失败，errorMsg表示错误消息，imgUrl不为空，显示验证码地址，
        ///         如果为空，则需要重新执行第一步获取验证码地址
        ///      ii,当status=1,表示成功，
        ///  1. 当result=true时,表示没有下一步，此步登陆成功。
        ///  2. 当result=false时，表示还有下一步（比如动态密码或者图片验证码，
        ///     例如上海电信需要二次图片验证码）
        ///	     <1>.当imgUrl="getImg", 则执行第（3）步获取图片验证码
        ///      <2>. 当imgUrl!="getImg", 执行第（4）步发送动态口令
        /// </summary>
        /// <param name="inp">输出参数</param>
        /// <param name="step">当前步骤</param>
        /// <returns></returns>
        public static HttpRequestResult CheckInputParametes(OutputParametes inp, int step = 1)
         {
            HttpRequestResult result = new HttpRequestResult();
            result.state = -1;
            if (inp.errorCode == "0")
            {
                if (inp.success == "True")
                {
                    if (inp.result == "True" && step == 2)
                    {
                        result.state = 0;
                        return result;
                    }
                    if (inp.status == "1")
                    {

                        if (step == 1 || step ==3)
                        {
                            result.state = 1;
                            result.context = inp.imgUrl;
                        }
                        else if (step == 2)
                        {
                            if (inp.imgUrl == "getImg")
                            {
                                result.state = 2;
                            }
                            else
                            {
                                result.state = 3;
                            }
                        }
                        else
                        {
                            if (inp.imgUrl == "")
                            {
                                result.state = 0;
                            }
                            else
                            {
                                result.state = 1;
                                result.context = inp.imgUrl;
                            }
                        }
                    }
                    else
                    {
                        result.state = -1;
                        result.context = inp.errorMsg;
                    }
                }
                else
                {
                    result.state = -1;
                    result.context = inp.errorMsg;
                }
            }
            else
            {
                result.state = -1;
                switch (inp.errorCode)
                {
                    #region 系统级错误
                    case "10001":
                        result.context = "appId为空";
                        break;
                    case "10002":
                        result.context = "timeunit为空";
                        break;
                    case "10003":
                        result.context = "timeunit过期";
                        break;
                    case "10004":
                        result.context = "timeunit属于未来时间，无效";
                        break;
                    case "10005":
                        result.context = "token生成错误";
                        break;
                    case "10006":
                        result.context = "appId对应的appkey不存在";
                        break;
                    case "10007":
                        result.context = "该接口不存在或其他访问异常";
                        break;
                    case "10008":
                        result.context = "userId为空";
                        break;
                    case "10009":
                        result.context = "userId不正确";
                        break;
                    case "10010":
                        result.context = "页面过期，请刷新页面重试";
                        break;
                    case "10011":
                        result.context = "userId长度超过45个字符";
                        break;
                    #endregion

                    #region 用户级错误
                    case "20004":
                        result.context = "手机号码为空";
                        break;
                    case "20006":
                        result.context = "手机服务密码为空";
                        break;
                    case "20007":
                        result.context = "手机号码位数不正确";
                        break;
                    case "20008":
                        result.context = "手机号码格式不正确";
                        break;
                    #endregion

                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeunit()
        {
            TimeSpan ts = new TimeSpan(System.DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return ((long)ts.TotalMilliseconds).ToString();
        }

        /// <summary>
        ///根据时间戳获取访问令牌
        /// </summary>
        /// <param name="timeunit">时间戳</param>
        /// <returns></returns>
        public static string GetToken(string timeunit)
        {
            string token = string.Empty;
            string appKey = System.Configuration.ConfigurationManager.AppSettings["appKey"];
            string all = "timeunit=" + timeunit + "appkey=" + appKey;

            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(all));
            foreach (byte b in bytes)
            {
                token += b.ToString("x2");
            }
            return token;
        }
    }
}