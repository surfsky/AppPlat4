using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Collections;
using App.Utils;

namespace App
{
    /// <summary>
    /// UEditor编辑器通用上传类（根据三方Demo改的，可以优化简化）
    /// </summary>
    public class UMUploader
    {
        string state = "SUCCESS";

        string URL = null;
        string currentType = null;
        string uploadpath = null;
        string filename = null;
        string originalName = null;
        HttpPostedFile uploadFile = null;

        /**
        * 上传文件的主处理方法
        */
        public Hashtable upFile(HttpContext cxt, string folder, string[] filetype, int size)
        {
            uploadpath = cxt.Server.MapPath(folder); //文件上传路径

            try
            {
                uploadFile = cxt.Request.Files[0];
                originalName = uploadFile.FileName;
                createFolder();

                //格式验证
                if (checkType(filetype))
                    state = "不允许的文件类型";
                //大小验证
                if (checkSize(size))
                    state = "文件大小超出网站限制";
                //保存图片
                if (state == "SUCCESS")
                {
                    filename = reName();
                    uploadFile.SaveAs(uploadpath + filename);
                    URL = folder + filename;
                }
            }
            catch 
            {
                state = "未知错误";
                URL = "";
            }
            return getUploadInfo();
        }

        /**
        * 上传涂鸦的主处理方法
        */
        public Hashtable upScrawl(HttpContext cxt, string pathbase, string tmppath, string base64Data)
        {
            pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
            uploadpath = cxt.Server.MapPath(pathbase);//获取文件上传路径
            FileStream fs = null;
            try
            {
                createFolder();
                filename = System.Guid.NewGuid().ToString("N") + ".png";
                fs = File.Create(uploadpath + filename);
                byte[] bytes = Convert.FromBase64String(base64Data);
                fs.Write(bytes, 0, bytes.Length);

                URL = pathbase + filename;
            }
            catch 
            {
                state = "未知错误";
                URL = "";
            }
            finally
            {
                fs.Close();
                deleteFolder(cxt.Server.MapPath(tmppath));
            }
            return getUploadInfo();
        }

        /**
        * 获取文件信息
        */
        public string getOtherInfo(HttpContext cxt, string field)
        {
            string info = null;
            if (cxt.Request.Form[field] != null && !String.IsNullOrEmpty(cxt.Request.Form[field]))
            {
                info = field == "fileName" ? cxt.Request.Form[field].Split(',')[1] : cxt.Request.Form[field];
            }
            return info;
        }

        /**
         * 获取上传信息
         * @return Hashtable
         */
        private Hashtable getUploadInfo()
        {
            Hashtable infos = new Hashtable();
            infos.Add("state", state);
            infos.Add("url", URL);
            infos.Add("originalName", originalName);
            infos.Add("name", Path.GetFileName(URL));
            infos.Add("size", uploadFile.ContentLength);
            infos.Add("type", Path.GetExtension(originalName));
            return infos;
        }

        /**
         * 重命名文件
         * @return string
         */
        private string reName()
        {
            //return Guid.NewGuid().ToString("N") + getFileExt();
            return string.Format("{0}{1}", SnowflakeID.Instance.NewID(), getFileExt());
        }

        /**
         * 文件类型检测
         * @return bool
         */
        private bool checkType(string[] filetype)
        {
            currentType = getFileExt();
            return Array.IndexOf(filetype, currentType) == -1;
        }

        /**
         * 文件大小检测
         * @param int
         * @return bool
         */
        private bool checkSize(int size)
        {
            return uploadFile.ContentLength >= (size * 1024 * 1024);
        }

        /**
         * 获取文件扩展名
         * @return string
         */
        private string getFileExt()
        {
            string[] temp = uploadFile.FileName.Split('.');
            return "." + temp[temp.Length - 1].ToLower();
        }

        /**
         * 创建存储文件夹
         */
        private void createFolder()
        {
            if (!Directory.Exists(uploadpath))
                Directory.CreateDirectory(uploadpath);
        }

        /**
         * 删除存储文件夹
         * @param string
         */
        public void deleteFolder(string path)
        {
            //if (Directory.Exists(path))
            //    Directory.Delete(path, true);
        }
    }
}

