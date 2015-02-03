using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.Models
{
    /// <summary>
    /// 输入参数
    /// </summary>
    public class EntryParametes
    {
        /// <summary>
        /// 服务码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 图片验证码
        /// </summary>
        public string authCode { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string idCard { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realName { get; set; }
        /// <summary>
        /// 电话号
        /// </summary>
        public string telNo { get; set; }
        /// <summary>
        /// 当前步骤
        /// </summary>
        public string index { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string smsCode { get; set; }
        /// <summary>
        /// 用户来源
        /// </summary>
        public string userSource { get; set; }
        /// <summary>
        /// 登陆名
        /// </summary>
        public string loginName { get; set; }
    }
}