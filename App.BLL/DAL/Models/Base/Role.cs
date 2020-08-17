using App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using App.Entities;
using System.Reflection;

/// <summary>
/// 用户-角色-权限 三级授权机制
/// 权限
///     系统只根据权限来限制操作，如修改按钮是否可用，删除按钮是否可用等
///     编写页面时无需考虑角色
///     权限是可以预料且内置的，如产品查看、产品修改、产品新建、产品删除权限
///     既然是固定的，可以用枚举来描述，也便于强类型编码，避免编码变更导致权限错乱的情况
///     提示：初期也可以简化权限，如系统管理、产品管理；在后期再根据需求拆分为产品新建、产品修改等。
///     插件化思路：每个插件都有自己的权限逻辑，只能动态添加，ID是不确定的，保留强类型编码可用静态成员
///     
/// 角色
///     角色是权限的集合
///     角色和权限的关系可在后台进行配置
///     角色可以动态增减，可在后台进行配置
///     
/// 用户
///     拥有多个角色
///     系统可根据用户拥有的角色对应的权限列表，来进行授权操作
///     注意：不推荐直接根据角色来授权，会导致逻辑不清晰
/// </summary>
namespace App.DAL
{
    /// <summary>
    /// 角色（用数据库管理，角色可由客户自由配置）
    /// </summary>
    [UI("系统", "角色")]
    public class Role : EntityBase<Role>, ICacheAll
    {
        // 属性
        [Key]
        [UI("ID", Mode = PageMode.View | PageMode.Edit, ReadOnly = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long ID { get; set; }

        [UI("名称")]  
        public string Name { get; set; }

        // 构造函数
        public Role() { }
        public Role(string name)
        {
            this.Name = name;
        }

        /// <summary>格式化为字符串</summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>根据名称获取角色对象（不存在则创建）</summary>
        public static Role Get(string name)
        {
            var item = All.FirstOrDefault(t => t.Name == name);
            return item ?? new Role(name).Save();
        }

        // 初始化
        /*
        public static Role Admin        => Get("系统管理员");
        public static Role AdminArticle => Get("文档管理员");
        public static Role AdminDept    => Get("部门管理员");
        public static Role AdminDir     => Get("目录管理员");
        public static Role ZhenQi       => Get("政企")      ;
        public static Role GongZhong    => Get("公众")      ;
        public override void Init()
        {
            foreach (var p in this.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (p.PropertyType == this.GetType())
                {
                    var item = p.GetValue(null) as Role;
                    IO.Debug(item.ToString());
                }
            }
        }
        */
    }


}