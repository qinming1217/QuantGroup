using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.Models
{
    public class HttpRequestResult
    {
        /// <summary>
        /// -1异常，0正常,1拿到验证码Url进行下一步,2需调用二次验证码接口,3需调用短信验证码接口
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 如需验证码则为验证码url，如异常则为异常信息
        /// </summary>
        public string context { get; set; }
    }
}