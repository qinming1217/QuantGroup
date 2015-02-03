using MvcApplication.DAL;
using MvcApplication.AppTool;
using MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class DataController : Controller
    {
        BaseDAL _baseDAL = new BaseDAL();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Load(string phone)
        {
            return View("Data");
        }
        /// <summary>
        /// 获取手机账单信息
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns></returns>
        public string GetMobileBill(string phone)
        {
            try
            {
                LogHelper.WriteInfo("GetMobileBill");
                DefaultParametes def = HttpRequestTool.GetDefaultParametes(1, 6);
                EntryParametes entry = new EntryParametes();
                entry.loginName = phone;
                string url = HttpRequestTool.GetUrl(def, entry);
                string responsStr = HttpRequestTool.CreateHttpRequest(url);
                OutputParametes inp = HttpRequestTool.GetOutputParametes(responsStr);
                if (inp != null && inp.phoneList != null && inp.phoneList.Count > 0)
                {
                    _baseDAL.SavePhoneList(inp.phoneList[0]);
                }
                return responsStr;
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e.Message, e);
                return "";
            }
        }
    }
}
