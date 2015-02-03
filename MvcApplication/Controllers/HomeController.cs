using MvcApplication.AppTool;
using MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 输入手机号进入下一步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Step1()
        {
            try
            {
                LogHelper.WriteInfo("Step1");
                string phone = HttpContext.Request["txtPhone"];
                if (phone == null) //第二步返回
                {
                    phone = Session["Phone"].ToString();
                }
                string imgUrl = string.Empty;
                DefaultParametes def = HttpRequestTool.GetDefaultParametes(1, 1);
                EntryParametes entry = new EntryParametes()
                {
                    index = "1",
                    phone = phone
                };
                OutputParametes inp = HttpRequestTool.GetInputParametes(def, entry);
                HttpRequestResult httpResult = HttpRequestTool.CheckInputParametes(inp);
                if (httpResult.state == 0)          //成功
                {
                    return View("Success");
                }
                else if (httpResult.state == 1)     //失败
                {
                    imgUrl = HttpRequestTool.CreateHttpRequest(httpResult.context, 2);
                    Session.Add("Phone", phone);
                    return View("Step2", (object)imgUrl);
                }
                else                                //异常
                {
                    ViewBag.Message = httpResult.context;
                    return View("Index");
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
                return View("Index");
            }
        }
        /// <summary>
        /// 输入验证码及服务码（验证码如有则必填）进入下一步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Step2()
        {
            try
            {
                LogHelper.WriteInfo("Step2");
                string imgUrl = string.Empty;
                string password = string.Empty;
                string phone = Session["Phone"].ToString();
                string authCode = HttpContext.Request.Form["txtAuthCode"];
                if (Session["Password"] != null)   //如Session不为空 则为第三步返回
                {
                    password = Session["Password"].ToString();
                }
                else if (HttpContext.Request.Form["txtPassword"] != null && HttpContext.Request.Form["txtPassword"] != "") //第二步正常点击下一步
                {
                    password = HttpContext.Request.Form["txtPassword"];
                }
                else  //第二步重新获取验证码
                {
                    Step1();
                    return View("Step2");
                }
                DefaultParametes def = HttpRequestTool.GetDefaultParametes(1, 2);
                EntryParametes entry = new EntryParametes()
                {
                    password = password,
                    authCode = authCode,
                    phone = phone
                };
                OutputParametes inp = HttpRequestTool.GetInputParametes(def, entry);
                HttpRequestResult httpResult = HttpRequestTool.CheckInputParametes(inp, 2);
                if (httpResult.state == 0)            //成功
                {
                    return View("Success");
                }
                else if (httpResult.state == 2)      //需图片验证码
                {
                    def = HttpRequestTool.GetDefaultParametes(1, 3);
                    entry = new EntryParametes()
                    {
                        index = "3",
                        phone = phone
                    };
                    inp = HttpRequestTool.GetInputParametes(def, entry);
                    httpResult = HttpRequestTool.CheckInputParametes(inp);
                    if (httpResult.state == 1)     //下一步
                    {
                        Session.Add("Password", password);
                        Session.Add("AuthCode", authCode);
                        imgUrl = HttpRequestTool.CreateHttpRequest(httpResult.context, 2);
                        return View("Step3", (object)imgUrl);
                    }
                    else                           //异常
                    {
                        ViewBag.Message = httpResult.context;
                        return View("Step2");
                    }
                }
                else if (httpResult.state == 3)  //需短信验证码
                {
                    Session.Add("AuthCode", authCode);
                    return View("Step4");
                }
                else                           //异常
                {
                    ViewBag.Message = httpResult.context;
                    return View("Step2");
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
                return View("Step2");
            }
        }
        /// <summary>
        /// 填写图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Step3()
        {
            try
            {
                LogHelper.WriteInfo("Step3");
                string imgUrl = string.Empty;
                string authCode = string.Empty;
                string phone = Session["Phone"].ToString();
                if (Session["AuthCode"] != null)
                {
                    authCode = Session["AuthCode"].ToString();
                }
                else if (HttpContext.Request.Form["txtAuthCode"] != null && HttpContext.Request.Form["txtAuthCode"] != "")
                {
                    authCode = HttpContext.Request.Form["txtAuthCode"];
                }
                else
                {
                    Step2();
                }
                DefaultParametes def = HttpRequestTool.GetDefaultParametes(1, 4);
                EntryParametes entry = new EntryParametes()
                {
                    authCode = authCode,
                    phone = phone
                };
                OutputParametes inp = HttpRequestTool.GetInputParametes(def, entry);
                HttpRequestResult httpResult = HttpRequestTool.CheckInputParametes(inp, 3);
                if (httpResult.state == 0)         //成功
                {
                    return View("Success");
                }
                else if (httpResult.state == 1)   //下一步
                {
                    Session.Add("AuthCode", authCode);
                    return View("Step4");
                }
                else                             //异常
                {
                    ViewBag.Message = httpResult.context;
                    return View("Step3");
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
                return View("Step3");
            }
        }
        /// <summary>
        /// 填写短信验证码   
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Step4()
        {
            try
            {
                LogHelper.WriteInfo("Step4");
                string imgUrl = string.Empty;
                string smsCode = HttpContext.Request.Form["txtSMSCode"];
                object authCodeObj = Session["AuthCode"];
                string authCode = authCodeObj != null ? authCodeObj.ToString() : "";

                if (smsCode == null || smsCode == "")
                {
                    Step3();
                    return View("Step4");
                }

                string phone = Session["Phone"].ToString();
                DefaultParametes def = HttpRequestTool.GetDefaultParametes(1, 5);
                EntryParametes entry = new EntryParametes()
                {
                    smsCode = smsCode,
                    authCode = authCode,
                    phone = phone
                };
                OutputParametes inp = HttpRequestTool.GetInputParametes(def, entry);
                HttpRequestResult httpResult = HttpRequestTool.CheckInputParametes(inp, 4);
                if (httpResult.state == 0)        //成功
                {
                    return View("Success");
                }
                else                              //异常
                {
                    ViewBag.Message = httpResult.context;
                    return View("Step4");
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
                return View("Step4");
            }
        }
    }
}
