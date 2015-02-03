using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication.Models.EF;
namespace MvcApplication.Models
{
    /// <summary>
    /// 输出参数
    /// </summary>
    public class OutputParametes
    {
        public string status { get; set; }
        public string imgUrl { get; set; }
        public string result { get; set; }
        public string success { get; set; }
        public string errorMsg { get; set; }
        public string forgetPassUrl { get; set; }
        public string passName { get; set; }
        public string errorCode { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string value { get; set; }
        public List<User> users { get; set; }
        public List<PhoneList> phoneList { get; set; }
    }
}