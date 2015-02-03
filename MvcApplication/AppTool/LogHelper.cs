using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.AppTool
{
    public class LogHelper
    {
        /// <summary>
        /// 静态只读实体对象info信息
        /// </summary>
        public static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("loginfo");
        /// <summary>
        ///  静态只读实体对象error信息
        /// </summary>
        public static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");
        /// <summary>
        ///  添加info信息
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfo(string info)
        {
            Console.WriteLine(info);
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(info);
            }
        }
        /// <summary>
        /// 添加异常信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void WriteError(string info, Exception se)
        {
            Console.WriteLine(info);
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(info, se);
            }
        }
    }
}