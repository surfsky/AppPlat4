using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using App.HttpApi;
using App.Components;
using App.Core;

namespace App
{
    public enum Sex
    {
        [UI("男")]  Male,
        [UI("女")]  Female
    }

    public class Person
    {
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public Sex Sex { get; set; }
        public Person Father { get; set; }
    }


    /// <summary>
    /// HttpApi Page：/HttpApi/Demo/api
    /// </summary>
    [History("2019-09-01", "SURFSKY", "Init")]
    public class Demo
    {
        //---------------------------------------------
        // 静态方法
        //---------------------------------------------
        [HttpApi("静态方法示例", Type = ResponseType.JSON)]
        public static object GetStaticObject()
        {
            return new { h = "3", a = "1", b = "2", c = "3" };
        }

        [HttpApi("Json结果包裹器示例", Wrap = true)]
        public static object TestWrapperDataResult()
        {
            return new { h = "3", a = "1", b = "2", c = "3" };
        }

        [HttpApi("默认方法参数示例", Remark = "p2的默认值为a")]
        public static object TestDefaultParameter(string p1, string p2="a")
        {
            return new { p1 = p1, p2 = p2};
        }

        [HttpApi("测试错误")]
        public static object TestError()
        {
            int n = 0;
            int m = 1 / n;
            return true;
        }

        //---------------------------------------------
        // 返回各种基础对象
        //---------------------------------------------
        [HttpApi("HelloWorld", CacheSeconds=30)]
        public string HelloWorld(string info)
        {
            System.Threading.Thread.Sleep(200);
            return string.Format("Hello world! {0} {1}", info, DateTime.Now);
        }

        [HttpApi("plist文件下载示例", CacheSeconds = 30, MimeType="text/plist", FileName="app.plist")]
        public string GetFile(string info)
        {
            System.Threading.Thread.Sleep(200);
            return string.Format("This is plist file demo! {0} {1}", info, DateTime.Now);
        }

        [HttpApi("输出系统时间", CacheSeconds=30)]
        public DateTime GetTime()
        {
            return System.DateTime.Now;
        }

        [HttpApi("输出DataTable->Json")]
        public DataTable GetDataTable()
        {
            DataTable dt = new DataTable("test");
            dt.Columns.Add("column1");
            dt.Columns.Add("column2");
            dt.Rows.Add("a1", "b1");
            dt.Rows.Add("a2", "b2");
            return dt;
        }

        [HttpApi("输出DataRow->Json")]
        public DataRow GetDataRow()
        {
            DataTable dt = new DataTable("test");
            dt.Columns.Add("column1");
            dt.Columns.Add("column2");
            dt.Rows.Add("a1", "b1");
            dt.Rows.Add("a2", "b2");
            return dt.Rows[0];
        }

        [HttpApi("输出图像", CacheSeconds=60)]
        public Image GetImage(string text)
        {
            Bitmap bmp = new Bitmap(200, 200);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString(
                text, 
                new Font("Arial", 16, FontStyle.Bold), 
                new SolidBrush(Color.FromArgb(255, 206, 97)), 
                new PointF(5, 5)
                );
            return bmp;
        }

        //---------------------------------------------
        // Token 机制
        // 1. 获取动态token
        // 2. 访问需要健权的接口，带上token参数
        // 3. 服务器端统一在 global 里面做token校验
        //---------------------------------------------
        [HttpApi("GetToken")]
        public string GetToken(string appKey, string appSecret)
        {
            return Token.Create(appKey, appSecret, 1);
        }


        [HttpApi("GetDate(NeedToken)", AuthToken=true)]
        public string GetDate()
        {
            var now = DateTime.Now;
            return now.ToString();
        }


        //---------------------------------------------
        // 控制访问权限
        //---------------------------------------------
        [HttpApi("登录")]
        public string Login()
        {
            AuthHelper.Login("Admin", new string[] { "Admins" }, DateTime.Now.AddDays(1));
            System.Threading.Thread.Sleep(200);
            return "访问成功（已登录）";
        }
 
        [HttpApi("注销")]
        public string Logout()
        {
            AuthHelper.Logout();
            System.Threading.Thread.Sleep(200);
            return "注销成功";
        }


        [HttpApi("用户必须登录后才能访问该接口，若无授权则返回401错误", AuthLogin=true)]
        public string LimitLogin()
        {
            System.Threading.Thread.Sleep(200);
            return "访问成功（已登录）";
        }

        [HttpApi("限制用户访问，若无授权则返回401错误", AuthUsers = "Admin,Kevin")]
        public string LimitUser()
        {
            System.Threading.Thread.Sleep(200);
            return "访问成功（限制用户Admin,Kevin）";
        }

        [HttpApi("限制角色访问，若无授权则返回401错误", AuthRoles = "Admins")]
        public string LimitRole()
        {
            System.Threading.Thread.Sleep(200);
            return "访问成功（限制角色Admins）";
        }


        //---------------------------------------------
        // 自定义类
        //---------------------------------------------
        [HttpApi("解析自定义类。father:{Name:'Kevin', Birth:'1979-12-01', Sex:0};")]
        public Person CreateGirl(Person father)
        {
            return new Person()
            {
                Name = father.Name + "'s dear daughter",
                Birth = System.DateTime.Now,
                Sex = Sex.Female,
                Father = father
            };
        }

        [HttpApi("null值处理")]
        public Person CreateNull()
        {
            return null;
        }

        [HttpApi("返回复杂对象")]
        public Person GetPerson()
        {
            return new Person() { Name = "Cherry" };
        }

        [HttpApi("返回复杂对象，并用DataResult进行封装", Wrap =true)]
        public Person GetPersonData()
        {
            return new Person() { Name = "Kevin" };
        }

        [HttpApi("返回DataResult对象")]
        public APIResult GetPersons()
        {
            List<Person> ps = new List<Person>(){
                new Person(),
                new Person()
            };
            return new APIResult(true, "", ps, null);
        }
    }
}
