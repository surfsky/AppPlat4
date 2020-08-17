using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using App.Utils;

namespace App.DAL
{
    /// <summary>
    /// 初始化数据库数据
    /// </summary>
    public class AppDatabaseInitializer : DropCreateDatabaseIfModelChanges<AppContext>  
    {
        protected override void Seed(AppContext context)
        {
            GetDepts().ForEach(d => context.Depts.Add(d));
            GetUsers().ForEach(u => context.Users.Add(u));
            GetTitles().ForEach(t => context.Titles.Add(t));
            context.SaveChanges();

            // 添加菜单时需要指定ViewPower，所以上面需要先保存到数据库
            GetMenus(context).ForEach(m => context.Menus.Add(m));
        }


        // 初始配置：头衔
        private static List<Title> GetTitles()
        {
            var titles = new List<Title>()
            {
                new Title() {Name = "总经理"},
                new Title() {Name = "部门经理"},
                new Title() {Name = "业务员"},
                new Title() {Name = "营业员"},
                new Title() {Name = "工程师"}
            };

            return titles;
        }


        // 初始配置：用户
        private static List<User> GetUsers()
        {
            var users = new List<User>();
            users.Add(new User
            {
                Name = "admin",
                Gender = "男",
                Password = PasswordHelper.CreateDbPassword("admin"),
                RealName = "超级管理员",
                Email = "admin@examples.com",
                InUsed = true,
                CreateDt = DateTime.Now
            });
            users.Add(new User
            {
                Name = "test",
                Gender = "女",
                Password = PasswordHelper.CreateDbPassword("123456"),
                RealName = "测试",
                Email = "test@examples.com",
                InUsed = true,
                CreateDt = DateTime.Now
            });
            return users;
        }


        // 初始配置：部门
        private static List<Dept> GetDepts()
        {
            var depts = new List<Dept> { 
                new Dept {Name = "研发部", Seq = 1, Remark = "顶级部门", Children = new List<Dept> { 
                        new Dept {Name = "开发部",  Seq = 1,  Remark = "二级部门"},
                        new Dept {Name = "测试部",  Seq = 2,  Remark = "二级部门"}
                    }
                },
                new Dept {Name = "销售部", Seq = 2, Remark = "顶级部门", Children = new List<Dept> { 
                        new Dept {Name = "直销部",  Seq = 1,  Remark = "二级部门"},
                        new Dept {Name = "渠道部",  Seq = 2,  Remark = "二级部门"}
                    }
                },
                new Dept { Name = "客服部", Seq = 3, Remark = "顶级部门", Children = new List<Dept> { 
                        new Dept { Name = "实施部", Seq = 1, Remark = "二级部门" },
                        new Dept { Name = "售后服务部", Seq = 2, Remark = "二级部门" },
                        new Dept { Name = "大客户服务部", Seq = 3, Remark = "二级部门" }
                    }
                },
                new Dept { Name = "财务部", Seq = 4, Remark = "顶级部门" },
                new Dept { Name = "行政部", Seq = 5, Remark = "顶级部门", Children = new List<Dept> { 
                        new Dept { Name = "人事部", Seq = 1, Remark = "二级部门" },
                        new Dept { Name = "后勤部", Seq = 2, Remark = "二级部门" },
                        new Dept { Name = "运输部", Seq = 3, Remark = "二级部门", Children = new List<Dept>{
                                new Dept{ Name = "省内运输部", Seq = 1, Remark = "三级部门"},
                                new Dept{ Name = "国内运输部", Seq = 2, Remark = "三级部门"},
                                new Dept{ Name = "国际运输部", Seq = 3, Remark = "三级部门"}
                            }
                        }
                    }
                }
            };

            return depts;
        }



        // 初始配置：菜单
        private static List<Menu> GetMenus(AppContext context)
        {
            var menus = new List<Menu> {
                new Menu
                {
                    Name = "首页",
                    Seq = -1,
                    ImageUrl = "~/res/icon/page.png",
                    NavigateUrl = "~/pages/welcome.aspx"
                },
                new Menu
                {
                    Name = "报表",
                    Seq = 2,
                    Remark = "",
                    ImageUrl = "~/res/icon/folder.png",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "GDP",
                            Seq = 10,
                            NavigateUrl = "~/pages/reports/GDP.aspx",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = null
                        }
                    }
                },
                new Menu
                {
                    Name = "系统",
                    Seq = 3,
                    Remark = "",
                    ImageUrl = "~/res/icon/folder.png",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "用户",
                            Seq = 10,
                            NavigateUrl = "~/pages/base/Users.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.UserView
                        },
                        new Menu
                        {
                            Name = "部门",
                            Seq = 20,
                            Remark = "",
                            NavigateUrl = "~/pages/base/Depts.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.DeptEdit
                        },
                        new Menu
                        {
                            Name = "职务",
                            Seq = 30,
                            NavigateUrl = "~/pages/base/Titles.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.TitleEdit
                        },
                        new Menu
                        {
                            Name = "角色",
                            Seq = 40,
                            Remark = "",
                            NavigateUrl = "~/pages/base/RolePowers.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower =  Powers.RolePowerEdit
                        },
                        new Menu
                        {
                            Name = "菜单",
                            Seq = 50,
                            NavigateUrl = "~/pages/base/Menus.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.Menu
                        },
                        new Menu
                        {
                            Name = "日志",
                            Seq = 60,
                            NavigateUrl = "~/pages/base/Logs.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.Log
                        },
                        new Menu
                        {
                            Name = "公告",
                            Seq = 70,
                            NavigateUrl = "~/pages/base/newsgrid.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.ArticleEdit
                        },
                        new Menu
                        {
                            Name = "区域",
                            Seq = 75,
                            NavigateUrl = "~/pages/base/Areas.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.AreaEdit
                        },
                        new Menu
                        {
                            Name = "在线用户",
                            Seq = 80,
                            NavigateUrl = "~/pages/maintains/Onlines.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.Online
                        },
                        new Menu
                        {
                            Name = "系统配置",
                            Seq = 90,
                            NavigateUrl = "~/pages/configs/Configs.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.ConfigSite
                        },
                        new Menu
                        {
                            Name = "接口清单",
                            Seq = 100,
                            NavigateUrl = "~/pages/devs/API.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.Admin
                        },
                        new Menu
                        {
                            Name = "数据字典",
                            Seq = 110,
                            NavigateUrl = "~/pages/devs/DbSchema.ashx",
                            ImageUrl = "~/res/icon/tag_blue.png",
                            ViewPower = Powers.Admin
                        },
                        new Menu
                        {
                            Name = "修改密码",
                            Seq = 120,
                            NavigateUrl = "~/pages/base/UserChangePassword.aspx",
                            ImageUrl = "~/res/icon/tag_blue.png"
                        }
                    }
                }
            };

            return menus;
        }
    }
}