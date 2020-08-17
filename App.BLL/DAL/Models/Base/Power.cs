using App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using App.Entities;
using System.Reflection;


namespace App.DAL
{
    /// <summary>
    /// 权限（用数据库存储，数据由初始化逻辑自动填充，且不允许删改。）
    /// </summary>
    [UI("系统", "权限")]
    public class Power : EntityBase<Power>, ICacheAll
    {
        // 属性
        [Key]
        [UI("ID", Mode = PageMode.View | PageMode.Edit, ReadOnly = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long ID { get; set; }

        [UI("分组")]  public string Group { get; set; }
        [UI("名称")]  public string Name { get; set; }

        // 构造函数
        public Power() { }
        public Power(string group, string name)
        {
            this.Group = group;
            this.Name = name;
        }

        /// <summary>格式化为字符串</summary>
        public override string ToString()
        {
            return $"{Group}-{Name}";
        }

        /// <summary>根据名称获取角色对象（不存在则创建）</summary>
        public static Power Get(string group, string name)
        {
            var item = All.FirstOrDefault(t => t.Group == group && t.Name == name);
            return item ?? new Power(group, name).Save();
        }

        //-----------------------------------------
        // 初始化
        //-----------------------------------------
        public static Power Backend           => Get("Core", "访问后台");
        public static Power Admin             => Get("Admin", "Admin专用");
        public static Power AdminMonitor      => Get("Admin", "监管");
        public static Power API               => Get("开发", "接口调测");
        public static Power Test              => Get("开发", "测试")      ;
        public static Power ConfigSite        => Get("配置", "站点")      ;
        public static Power RolePowerEdit     => Get("配置", "角色权限");
        public static Power Menu              => Get("配置", "菜单");
        public static Power Client            => Get("配置", "客户端");
        public static Power Sequence          => Get("配置", "序列号");
        public static Power Monitor           => Get("监管", "监管");
        public static Power MonitorLog        => Get("监管", "日志");
        public static Power Online            => Get("监管", "在线用户");
        public static Power Message           => Get("监管", "消息");
        public static Power Res               => Get("监管", "资源");
        public static Power IPFilter          => Get("监管", "IP黑名单");
        public static Power FeedBackNew       => Get("运维", "反馈新增");
        public static Power FeedBackView      => Get("运维", "反馈查看");
        public static Power FeedBackEdit      => Get("运维", "反馈修改");
        public static Power FeedBackDelete    => Get("运维", "反馈删除");
        public static Power ComplainNew       => Get("运维", "投诉新增");
        public static Power ComplainView      => Get("运维", "投诉查看");
        public static Power ComplainEdit      => Get("运维", "投诉修改");
        public static Power ComplainDelete    => Get("运维", "投诉删除");
        public override void Init()
        {
            foreach (var p in this.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (p.PropertyType == this.GetType())
                {
                    var item = p.GetValue(null) as Power;
                    IO.Debug(item.ToString());
                }
            }
        }
        
    }


}