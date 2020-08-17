using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;

namespace App.Components
{
    /// <summary>
    /// Windows 全文检索。使用前需做以下配置：
    /// （1）开启 Windows Search 服务。若服务不存在需先安装（windows server）。
    /// （2）将指定目录加入到索引：控制面板》索引选项》修改》将需要加入的目录打勾。
    /// SURFSKY 2019-11
    /// </summary>
    public class WinSearch
    {
        // 属性
        public string ItemName { get; set; }
        public string ItemPathDisplay { get; set; }
        public string ItemUrl { get; set; }
        public string ItemType { get; set; }
        public string ItemTypeText { get; set; }
        public string FileName { get; set; }
        public string KindText { get; set; }
        public string SearchSummary { get; set; }
        public int    SearchRank { get; set; }
        public int    SearchHitCount { get; set; }
        public long   Size { get; set; }
        public DateTime DateModified { get; set; }

        /// <summary>转化为 SQL 安全字符串，避免SQL注入</summary>
        static string ToSqlSafeString(string txt)
        {
            return txt.Replace("'", "").Replace("\"", "");
        }

        /// <summary>检索指定目录（含子目录）下的所有文件</summary>
        /// <param name="folder">已经编制索引的目录</param>
        /// <param name="keywords">关键字</param>
        /// <remarks>该方法在windows10上测试ok，在windows server 2008r2上无法检索多个关键字</remarks>
        public static List<WinSearch> Search(string folder, List<string> keywords, int pageIndex=0, int pageSize = 100)
        {
            var data = new List<WinSearch>();
            var keys = keywords.Cast(t => ToSqlSafeString(t));
            if (keys.Count == 0)
                return data;

            // 文件名匹配
            var sb = new StringBuilder();
            foreach (var key in keys)
                sb.Append($"System.ItemName like '%{key}%' and ");
            var nameCondition = sb.ToString().TrimEnd("and ");

            // 内容匹配
            sb = new StringBuilder();
            foreach (var key in keys)
                //sb.Append($"contains('%{key}%') and ");                       // 该语句在windows 2008上无法检索到内容
                sb.Append($"System.Search.AutoSummary like '%{key}%' and ");    // 用该语句在windows 2008上可以检索一个关键字，多个关键字会报错
            var textCondition = sb.ToString().TrimEnd("and ");
            var condition = $" ({nameCondition})\r\n  or ({textCondition})";

            // 构建检索SQL，注意不支持AS表达式
            var sql = $@"
SELECT top 1000
    System.ItemName               ,
    System.ItemPathDisplay        ,
    System.ItemType               ,
    System.ItemTypeText           ,
    System.ItemUrl                ,
    System.Filename               ,
    System.KindText               ,
    System.Search.AutoSummary     ,
    System.Search.Rank            ,
    System.Search.HitCount        ,
    System.DateModified           ,
    System.Size                   ,
    System.ItemDate               ,
    System.DateAccessed           ,

    System.ItemNameDisplay,
    System.Search.EntryID,
    System.Search.GatherTime,
    System.Search.Store,
    System.FileExtension,
    System.ItemFolderPathDisplay,
    System.ContentType,
    System.ApplicationName,
    System.Kind,
    System.ParsingName,
    System.SFGAOFlags,
    System.ThumbnailCacheId
FROM SystemIndex
WHERE scope ='file:{folder}' 
AND System.ItemType <> 'Directory'
AND 
(
    {condition}
)
order by System.Search.Rank desc
";

            try
            {
                var connstr = @"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""";
                using (var conn = new OleDbConnection(connstr))
                {
                    conn.Open();
                    var cmd = new OleDbCommand(sql, conn);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            data.Add(new WinSearch()
                            {
                                ItemName = r["System.ItemName"].ToText(),
                                ItemPathDisplay = r["System.ItemPathDisplay"].ToText(),
                                ItemUrl = r["System.ItemUrl"].ToText(),
                                ItemType = r["System.ItemType"].ToText(),
                                ItemTypeText = r["System.ItemTypeText"].ToText(),
                                KindText = r["System.KindText"].ToText(),
                                FileName = r["System.Filename"].ToText(),
                                SearchSummary = r["System.Search.AutoSummary"].ToText(),
                                SearchRank = r["System.Search.Rank"].To<int>(),
                                SearchHitCount = r["System.Search.HitCount"].To<int>(),
                                Size = r["System.Size"].To<int>(),
                                DateModified = r["System.DateModified"].To<DateTime>()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogDb("WinSearchFail", $"{folder}, {keywords}, {ex.Message}");
            }

            return data.AsQueryable().Page(pageIndex, pageSize).ToList();
        }

    }
}
