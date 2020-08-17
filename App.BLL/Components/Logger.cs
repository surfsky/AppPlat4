using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using App.Utils;

namespace App.Components
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Logger
    {
        public static log4net.ILog _log;
        static Logger()
        {
            //var file = HttpContext.Current.Server.MapPath("~/Log4Net.config");
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(file));
            log4net.Config.XmlConfigurator.Configure();
            _log = log4net.LogManager.GetLogger("App");
            _log.Info("App logger start");
        }

        /// <summary>添加文本日志</summary>
        public static void Log(string from, string message, LogLevel level=LogLevel.Info)
        {
            //var info = new { From = from,  Message = message }.ToJson();
            var info = $"[{from}] {message}";
            switch (level)
            {
                case LogLevel.Debug: _log.Debug(info); break;
                case LogLevel.Info:  _log.Info(info);  break;
                case LogLevel.Warn:  _log.Warn(info);  break;
                case LogLevel.Error: _log.Error(info); break;
                case LogLevel.Fatal: _log.Fatal(info); break;
            }
        }

        /// <summary>记录网页匿名异常（到数据库）</summary>
        /// <param name="ignore404">是否跳过 404 文件不存在错误</param>
        public static void LogWebException(string from, Exception ex = null, bool ignore404=true)
        {
            if (!Asp.IsWeb)
                return;

            // 获取最后异常
            ex = ex ?? HttpContext.Current.Server.GetLastError();
            if (ex == null)
                return;

            // 跳过文件不存在错误
            if (ignore404)
                if (ex.Message.IndexOf("does not exist") > -1 || ex.Message.IndexOf("不存在") > -1)
                    return;

            // 保存错误信息到日志中
            var message = Asp.BuildRequestHtml(ex);
            LogDb(from, message, "", LogLevel.Error);
        }

        /// <summary>记录网页请求信息（到数据库）</summary>
        public static void LogWebRequest(string from, Exception ex = null, string operater = "")
        {
            if (!Asp.IsWeb)
                return;

            var level = (ex == null) ? LogLevel.Info : LogLevel.Error;
            var message = Asp.BuildRequestHtml(ex);
            LogDb(from, message, operater, level);
        }

        /// <summary>记录日志到数据库</summary>
        public static void LogDb(string from, string message = "", string operater = "", LogLevel level = LogLevel.Info)
        {
            // 存储到文本日志
            message = message.SubText(0, 1024 * 100); // 限制信息长度
            Log(from, message, level);

            // 判断该日志是否需要记录到数据库，不需要的话就算了
            if (!LogConfig.NeedLog(from))
                return;

            // 如果是网站环境则收集请求信息
            var url = "";
            var referrer = "";
            var method = "";
            var ip = "";
            if (Asp.IsWeb && Asp.IsRequestOk)
            {
                if (operater.IsEmpty())
                    operater = Asp.User?.Identity.Name;  //.user (Common.LoginUser != null) ? Common.LoginUser.NickName : "Unknown";
                if (ip.IsEmpty())
                    ip = Asp.ClientIP;
                method = Asp.Request.HttpMethod;
                referrer = Asp.Request.UrlReferrer.ToText();
                url = Asp.Url;
            }

            // 存储到数据库日志
            var log = new Log
            {
                Level = level,
                Operator = operater,
                Message = message,
                Summary = message.RemoveTag().HtmlDecode().RemoveBlankTranslator().Slim().Summary(50),
                LogDt = DateTime.Now,
                From = from,
                URL = url,
                Method = method,
                Referrer = referrer,
                IP = ip
            };
            log.Save(false);
        }
    }
}