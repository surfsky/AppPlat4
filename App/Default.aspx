<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="App.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>后台登录</title>
    <style type="text/css">
        #ie6-warning {
            background: rgb(255,255,225);
            position: fixed;
            top: 0;
            left: 0;
            font-size: 12px;
            color: #333;
            width: 97%;
            padding: 2px 15px 2px 23px;
            text-align: left;
            _height: 20px;
        }
        #ie6-warning a {
            text-decoration: none;
        }
        #ie6-warning a:link, #ie6-warning a:visited {
            text-decoration: none;
            color: #03F;
            display: inline;
        }
        #ie6-warning a:hover, #ie6-warning a:active {
            text-decoration: none;
            color: #69F;
            display: inline;
        }

        /*css restet*/
        body, div, ul, li { margin:0; padding:0; font-family:"微软雅黑"; font-size:14px;  color:#666; }
        img { border:0; }
        ol, ul { list-style:none; }
        a { text-decoration:none; }
        dl,dt,dd { margin:0;}
        .clear { clear:both; }

        .top{height:95px; background:url(/res/images/topBg.gif) repeat-x;}
        .middle{ height:512px; background:url(/res/images/bg.jpg) no-repeat;}
        .footer{ height:80px; padding-top:30px;text-align:center; line-height:28px; background:#eee; }
        .toplogo{width:980px; height:95px; margin:0 auto;}
        .toplogo img{ margin:15px;}
        .middle-box{width:980px; height:512px; margin:0 auto;}
        .middle-box-lt{ float:left; margin-top:137px; }
        .middle-box-lt dl dd{float:left; margin:52px 0 0 30px;}
        .middle-box-lt dl dd span img{margin-top:30px;}
        .middle-box-rt{float:right; width:330px; height:315px; margin:104px 24px 0 0; background:url(/res/images/formBg.png) no-repeat; }

        .logintit{ font-size:18px; color:#fff; font-weight:normal;}

        .middle-box-rt ul{margin:25px 0 0 42px;}
        .middle-box-rt ul li{margin:10px 0;}
        .user{ background:url(/res/images/formUser.gif) left no-repeat;}
        .user input{ margin-left:50px; padding-left:4px; width:190px; height:36px; line-height:36px; border:0; font-size:14px; color:#666;}

        .password{ background:url(/res/images/formpassword.gif) left no-repeat;}
        .password input{ margin-left:50px; padding-left:4px; width:190px; height:36px; line-height:36px; border:0; font-size:14px; color:#666;}

        .VFcode{ background:url(/res/images/formVFcode.gif) left no-repeat;}
        .VFcode input{ margin-left:50px; margin-right:10px; padding-left:4px; width:76px; height:36px; line-height:36px; border:0; font-size:14px; color:#666;}
        .FGtxt a{ margin-left:180px; display:block; width:70px; color:#fff;}
        .FGtxt a:hover{color:#f7de0b;}
        .login a{ display:block; width:245px; height:47px; text-indent:-9999em; background:url(/res/images/btnLogin.gif) no-repeat;}
        .login a:hover{background:url(/res/images/btnLogin-D.gif) no-repeat;}
    </style>

    <script src="Res/js/jquery-1.10.2.min.js" ></script>
    <script type="text/javascript">
        // 确保登录窗口是顶级窗口
        function SetTopWindow() {
            if (window.top != null && window.top.document.URL != document.URL) {
                window.top.location = document.URL;
            }
        }

        //页面重定向时判断是不是微信浏览器,跳转微信登录页
        $(function () {
            // 对浏览器的UserAgent进行正则匹配，不含有微信独有标识的则为其他浏览器
            var useragent = navigator.userAgent;
            if (useragent.match(/MicroMessenger/i) == 'MicroMessenger') {
                if (!!(window.attachEvent && !window.opera))
                { document.execCommand("stop"); }
                else
                { window.stop(); }

                window.location.href = ("/WeiXin/AuthPage.aspx");
                opened.opener = null;
                opened.close();

            }
        })
    </script>
    <script type="text/javascript" src="res/js/function.js"></script>
    <script type="text/javascript" src="res/js/dd_png.js"></script>

</head>


<body onload="SetTopWindow()">
    <input type="hidden" id="id1" value="cookie out time" />
    <!--[if lte IE 6]>
    <div id="ie6-warning">本网站不支持IE6浏览器，请您升级到 <a href="http://www.microsoft.com/china/windows/internet-explorer/" target="_blank">Internet Explorer 8</a> 或以下浏览器： <a href="http://www.google.com/chrome/?hl=zh-CN">Chrome</a> / <a href="http://www.mozillaonline.com/">Firefox</a> / <a href="http://www.apple.com.cn/safari/">Safari</a> / <a href="http://www.opera.com//">Opera</a></div>
    <script type="text/javascript">
    function position_fixed(el, eltop, elleft){
        DD_belatedPNG.fix('.tel,.head_content');
        // check if this is IE6
        if(!window.XMLHttpRequest)
            window.onscroll = function(){
                el.style.top = (document.documentElement.scrollTop + eltop)+"px";
                el.style.left = (document.documentElement.scrollLeft + elleft)+"px";
            }
        else 
            el.style.position = "fixed";
        }
        position_fixed(document.getElementByIdx_x_x("ie6-warning"),0, 0);
    </script>  
    <![endif]-->
    <form id="form1" runat="server">
        <div class="top">
            <div class="toplogo">
                <asp:Label runat="server" ID="lblTitle" Text="SiteTitle" Style="display: block; font-size: 40px; color: #B3B0B0; padding-top: 20px;" />
                <!--img src="res/images/logo.png" /-->
            </div>
        </div>
        <div class="middle">
            <div class="middle-box">
                <div class="middle-box-lt">
                    <dl>
                        <%--<dt>
                        <img src="res/images/txt.png" width="425" height="68" /></dt>
                    <dd>
                        <img src="res/images/code.png" width="117" height="116" /></dd>
                    <dd>
                        <span><a href="#">
                            <img src="res/images/btn01.png" width="115" height="33" /></a></span></dd>--%>
                    </dl>
                </div>
                <div class="middle-box-rt">
                    <ul>
                        <li class="logintit">用户登录</li>
                        <li class="user">
                            <input id="txtUserName" name="txtUserName" type="text" />
                            <li class="password">
                                <input name="txtPassword" id="txtPassword" type="password" />
                                <li class="VFcode">
                                    <input name="txtCode" id="txtCode" type="text" />
                                    <img name="imgCode" id="imgCode"
                                        width="100" height="39" alt="点击切换验证码" title="点击切换验证码"
                                        style="margin-top: 0px; margin-left: 0px; vertical-align: top; cursor: pointer;"
                                        src="/HttpApi/Common/VerifyImage"
                                        onclick="resetVerifyImg(); return false;" />
                                </li>
                                <li class="login"><a href="javascript:login();" id="aLogin">登录</a></li>
                                <li class="FGtxt"><a href="#" style="visibility: hidden">忘记密码？</a></li>
                            </li>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="footer">
            <span style="font-family: arial;">
                <asp:Label runat="server" ID="lblDomain" />&nbsp;
                <asp:Label runat="server" ID="lblICPNumber" />
                <br />
                Copyright &copy; 2017-2018  All Rights Reserved
            </span>
        </div>
    </form>

</body>
</html>


<script type="text/javascript">
    // 按键事件
    $(document).keyup(function (event) {
        switch (event.keyCode) {
            case 13: login(); break;
        }
    });

    // 登录
    function login() {
        // 参数校验
        var userName = $("#txtUserName").val();
        var password = $("#txtPassword").val();
        var code = $("#txtCode").val();
        if (userName == "") { alert("用户名不能为空"); return; }
        if (password == "") { alert("密码不能为空"); return; }
        if (code == "") { alert("验证码不能为空"); return; }

        // 去服务器端验证
        $.ajax({
            type: "POST",
            url: "/HttpApi/User/Login",
            data: { userName: userName, password: password, verifyImage: code }
        }).always(function (json) {
            if (json.Result == true) {
                top.location.href = "/pages/Index.aspx";
            }
            else {
                alert(json.Info);
                $("#txtCode").val("");
                resetVerifyImg();
            }
        });
    }

    // 重置验证码
    function resetVerifyImg() {
        var url = "/HttpApi/Common/VerifyImage?" + Math.random();  // 避免某些浏览器会缓存图片
        $("#imgCode").attr('src', url);
    }
</script>
