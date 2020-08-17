using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Utils;

namespace App.Components
{
    /// <summary>
    /// IP 地址查询(http://user.ip138.com/ip/doc)
    /// </summary>
    public class IPQuerier
    {
        // http://api.ip138.com/query/?ip=8.8.8.8&datatype=txt&token=67d486185ff0efac832def3bb9c3637f
        /// <summary>查询IP地址</summary>
        public static string Query(string ip)
        {
            var token = "67d486185ff0efac832def3bb9c3637f";
            var url = string.Format("http://api.ip138.com/query/?ip={0}&datatype=txt&token={1}", ip, token);
            return HttpHelper.Get(url);
        }
    }
}