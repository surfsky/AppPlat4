using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Linq.Expressions;
using FineUIPro;
using App.Utils;

namespace App.Controls
{
    /// <summary>
    /// UI 操作 (FineUI) 辅助类
    /// </summary>
    public partial class UI
    {

        //------------------------------------------------------
        // Get
        //------------------------------------------------------
        /// <summary>获取 Label 整数值</summary>
        public static long? GetLong(Label lbl)
        {
            if (long.TryParse(lbl.Text, out long n))
                return n;
            else
                return null;
        }

        /// <summary>获取整数值</summary>
        public static int? GetInt(RealTextField tb)
        {
            if (int.TryParse(tb.Text, out int n))
                return n;
            else
                return null;
        }

        /// <summary>获取整数值</summary>
        public static Int64? GetLong(RealTextField tb)
        {
            if (Int64.TryParse(tb.Text, out Int64 n))
                return n;
            else
                return null;
        }

        /// <summary>获取下拉框整数值</summary>
        public static int? GetInt(DropDownList ddl)
        {
            return ddl.SelectedItemArray.Length != 0 ? (int?)int.Parse(ddl.SelectedValue) : null;
        }

        /// <summary>获取下拉框整数值</summary>
        public static long? GetLong(DropDownList ddl)
        {
            return ddl.SelectedItemArray.Length != 0 ? (long?)long.Parse(ddl.SelectedValue) : null;
        }

        /// <summary>获取下拉框布尔值("true", "false")</summary>
        public static bool? GetBool(DropDownList ddl)
        {
            return ddl.SelectedItemArray.Length != 0 ? (bool?)bool.Parse(ddl.SelectedValue) : null;
        }

        /// <summary>获取下拉框枚举值(单选框列表)</summary>
        public static T? GetEnum<T>(DropDownList ddl) where T : struct
        {
            return GetInt(ddl).ToEnum<T>();
        }


        /// <summary>获取下拉框文本</summary>
        public static string GetText(DropDownList ddl)
        {
            return ddl.SelectedText;
        }
        /// <summary>获取单拉框文本</summary>
        public static string GetText(RadioButtonList ddl)
        {
            return ddl.SelectedValue;
        }
        /// <summary>获取弹出框值</summary>
        public static long? GetLong(PopupBox tb)
        {
            var ids = tb.GetSelectValues();
            return (ids.Count > 0) ? (long?)ids[0] : null;
        }

        /// <summary>获取弹出框文本</summary>
        public static string GetText(PopupBox tb)
        {
            return tb.Text;
        }

        /// <summary>获取弹出框值</summary>
        public static List<long> GetLongs(PopupBox tb)
        {
            return tb.GetSelectValues();
        }

        /// <summary>获取文本框 Trim 文本（RealTextField 的子类有：TextBox, NumberBox, DatePicker）</summary>
        public static string GetText(RealTextField tb)
        {
            return tb.Text.Trim();
        }

        /// <summary>获取 Html 文本框 Trim 文本</summary>
        public static string GetText(HtmlEditor tb)
        {
            return tb.Text.Trim();
        }

        /// <summary>获取文本框整型数据（RealTextField 的子类有：TextBox, NumberBox, DatePicker）</summary>
        public static int? GetInt(RealTextField tb, int? defaultValue = null)
        {
            return GetText(tb).ParseInt() ?? defaultValue;
        }
        /// <summary>获取文本框整型数据（RealTextField 的子类有：TextBox, NumberBox, DatePicker）</summary>
        public static long? GetLong(RealTextField tb, long? defaultValue = null)
        {
            return GetText(tb).ParseLong() ?? defaultValue;
        }
        /// <summary>获取文本框Double数据（RealTextField 的子类有：TextBox, NumberBox, DatePicker）</summary>
        public static double? GetDouble(RealTextField tb, double? defaultValue = null)
        {
            return GetText(tb).ParseDouble() ?? defaultValue;
        }

        /// <summary>获取日期时间</summary>
        public static DateTime? GetDate(DatePicker dp)
        {
            return dp.SelectedDate;
        }

        /// <summary>获取日期时间</summary>
        public static DateTime? GetDate(DateTimePicker dp)
        {
            return dp.SelectedDate;
        }

        /// <summary>获取复选框信息</summary>
        public static bool GetBool(CheckBox cb)
        {
            return cb.Checked;
        }

        /// <summary>获取图像控件的图像地址</summary>
        public static string GetUrl(FineUIPro.Image img, bool removeQueryString = true)
        {
            var url = img.ImageUrl.IsEmpty() ? "" : Asp.ResolveUrl(img.ImageUrl);
            if (removeQueryString)
                url = new Url(url).PurePath;
            return url;
        }

        /// <summary>获取su缩略图控件的图像地址</summary>
        public static string GetUrl(Thrumbnail img)
        {
            return img.ImageUrl;
        }
        /// <summary>获取su缩略图控件的图像地址</summary>
        public static string GetUrl(ImageUploader img)
        {
            return img.ImageUrl;
        }

        /// <summary>获取多选框值列表</summary>
        public static List<long> GetLongs(CheckBoxList cbl)
        {
            return cbl.SelectedValueArray.CastLong();
        }

        /// <summary>获取多选框值列表</summary>
        public static List<long> GetLongs(DropDownList ddl)
        {
            return ddl.SelectedValueArray.CastLong();
        }


        /// <summary>获取多选框值列表</summary>
        public static List<int> GetInts(CheckBoxList cbl)
        {
            return cbl.SelectedValueArray.CastInt();
        }
        /// <summary>获取多选框值列表</summary>
        public static List<T> GetEnums<T>(CheckBoxList cbl) where T : struct
        {
            return GetInts(cbl).CastEnum<T>();
        }

        /// <summary>获取整型值(单选框列表)</summary>
        public static int? GetInt(RadioButtonList rbl)
        {
            if (rbl.SelectedIndex == -1)
                return null;
            else
                return int.Parse(rbl.SelectedValue);
        }

        /// <summary>获取下拉框所有值列表</summary>
        public static List<T> GetAll<T>(DropDownList ddl) where T : struct
        {
            return ddl.Items.Cast(t => t.Value.To<T>());
        }

        /// <summary>获取下拉框所有值列表</summary>
        public static List<T> GetAll<T>(RadioButtonList rbl) where T : struct
        {
            return rbl.Items.Cast(t => t.Value.To<T>());
        }

        /// <summary>获取枚举值(单选框列表)</summary>
        public static T? GetEnum<T>(RadioButtonList rbl) where T : struct
        {
            return GetInt(rbl).ToEnum<T>();
        }

        /// <summary>获取布尔值(单选框列表)</summary>
        public static bool? GetBool(RadioButtonList rbl, bool? defaultValue)
        {
            return rbl.SelectedIndex != -1
                ? (bool?)bool.Parse(rbl.SelectedValue)
                : defaultValue
                ;
        }


        /// <summary>获取FineUI图标的url</summary>
        public static string GetIconUrl(FineUIPro.Icon icon)
        {
            return FineUIPro.IconHelper.GetIconUrl(icon).ResolveUrl();
        }



        //------------------------------------------------------
        // Set
        //------------------------------------------------------
        /// <summary>设置下拉框值（支持对象或枚举）</summary>
        public static void SetValue(DropDownList ddl, object value)
        {
            if (value is Enum)
                ddl.SelectedValue = (value == null) ? "" : Convert.ToInt32(value).ToString();
            else
                ddl.SelectedValue = (value == null) ? "" : value.ToString();
        }

        /// <summary>设置单选框值（支持对象或枚举）</summary>
        public static void SetValue(RadioButtonList rbl, object value)
        {
            if (value is Enum)
                rbl.SelectedValue = (value == null) ? "" : Convert.ToInt32(value).ToString();
            else
                rbl.SelectedValue = (value == null) ? "" : value.ToString();
        }

        /// <summary>设置单选框值</summary>
        public static void SetValue(CheckBox cb, bool value)
        {
            cb.Checked = value;
        }

        /// <summary>设置单选框值</summary>
        public static void SetValue(CheckBox cb, bool? value)
        {
            if (value != null)
                cb.Checked = value.Value;
        }

        /// <summary>设置文本</summary>
        public static void SetText(Label field, string txt)
        {
            field.Text = txt;
        }

        /// <summary>设置文本</summary>
        public static void SetText(ToolbarText field, string txt)
        {
            field.Text = txt;
        }

        /// <summary>设置文本</summary>
        public static void SetText(RealTextField field, string txt)
        {
            field.Text = txt;
        }

        /// <summary>设置下拉框文本（支持对象或枚举）</summary>
        public static void SetText(DropDownList ddl, object value)
        {
            if (value is Enum)
                ddl.Text = (value == null) ? "" : Convert.ToInt32(value).ToString();
            else
                ddl.Text = (value == null) ? "" : value.ToString();
        }

        /// <summary>设置超链接地址</summary>
        public static void SetValue(HyperLink lnk, string url)
        {
            lnk.NavigateUrl = url;
        }


        /// <summary>设置文本框值</summary>
        public static void SetValue(RealTextField tb, object value)
        {
            if (value != null)
                tb.Text = value.ToString();
        }

        /// <summary>设置Html编辑框值（没法合并，基类不同）</summary>
        public static void SetValue(HtmlEditor tb, object value)
        {
            if (value != null)
                tb.Text = value.ToString();
        }


        /// <summary>设置标签值</summary>
        public static void SetValue(Label tb, object value)
        {
            tb.Text = value?.ToString();
        }


        /// <summary>设置弹出框值</summary>
        public static void SetValue<T>(PopupBox pb, T data, Expression<Func<T, object>> idExpression, Expression<Func<T, object>> nameExpression)
        {
            if (data != null)
                pb.SetSelectedValue(new List<T>() { data }, idExpression, nameExpression);
        }
        
        /// <summary>设置弹出框值</summary>
        public static void SetValues<T>(PopupBox pb, List<T> data, Expression<Func<T, object>> idExpression, Expression<Func<T, object>> nameExpression)
        {
            pb.SetSelectedValue(data, idExpression, nameExpression);
        }

        /// <summary>设置日期框值</summary>
        public static void SetValue(DatePicker dp, DateTime? value)
        {
            if (value != null)
                dp.SelectedDate = value;
        }

        /// <summary>设置日期框值</summary>
        public static void SetValue(DateTimePicker dp, DateTime? value)
        {
            if (value != null)
                dp.SelectedDate = value;
        }


        /// <summary>设置单选列表的选中值（枚举）</summary>
        public static void SetValue<T>(RadioButtonList rbl, T? enumValue) where T : struct
        {
            rbl.SelectedValue = (enumValue == null) ? "" : Convert.ToInt32(enumValue).ToString();
        }

        /// <summary>设置单选列表的选中值（布尔）</summary>
        public static void SetValue(RadioButtonList rbl, bool? value)
        {
            rbl.SelectedValue = value.ToText();
        }


        /// <summary>设置复选列表的选中值</summary>
        public static void SetValues<T>(CheckBoxList cbl, List<T> values)
        {
            values = values ?? new List<T>();
            cbl.SelectedValueArray = values.CastString().ToArray();
        }

        /// <summary>设置下拉框多选值</summary>
        public static void SetValues<T>(DropDownList ddl, List<T> values)
        {
            values = values ?? new List<T>();
            ddl.SelectedValueArray = values.CastString().ToArray();
        }


        /// <summary>设置图片。如果图片参数为空，则尝试显示默认图片</summary>
        public static void SetValue(FineUIPro.Image img, string imageUrl, bool deleteOldImage=false, string defaultImageUrl = "")
        {
            if (deleteOldImage)
                Asp.DeleteWebFile(img.ImageUrl);
            if (imageUrl.IsNotEmpty())
                img.ImageUrl = imageUrl;
            else if (defaultImageUrl.IsNotEmpty())
                img.ImageUrl = defaultImageUrl;
        }

        /// <summary>设置图片。如果图片参数为空，则尝试显示默认图片</summary>
        public static void SetValue(Thrumbnail img, string imageUrl, bool deleteOldImage = false, string defaultImageUrl = "")
        {
            if (deleteOldImage)
                Asp.DeleteWebFile(img.ImageUrl);
            if (imageUrl.IsNotEmpty())
                img.ImageUrl = imageUrl;
            else if (defaultImageUrl.IsNotEmpty())
                img.ImageUrl = defaultImageUrl;
        }


        /// <summary>设置图片。如果图片参数为空，则尝试显示默认图片</summary>
        public static void SetValue(ImageUploader img, string imageUrl, bool deleteOldImage = false, string defaultImageUrl = "")
        {
            if (deleteOldImage)
                Asp.DeleteWebFile(img.ImageUrl);
            if (imageUrl.IsNotEmpty())
                img.ImageUrl = imageUrl;
            else if (defaultImageUrl.IsNotEmpty())
                img.ImageUrl = defaultImageUrl;
        }
    }
}