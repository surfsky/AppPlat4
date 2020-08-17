<%@ Page  Language="C#"%>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="App" %>
<%@ Import Namespace="App.DAL" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>将aspx页面作为脚本引擎示例。模型数据由Context.Items传递。</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
</head>
<body>
    <table>
        <%
        // 执行引擎
        // HttpContext.Current.Items["Users"] = ...;
        // HttpContext.Current.Server.Transfer("TestWebFormView.aspx");
        var persons = (List<User>)Context.Items["Users"];
        foreach(var person in persons)
        {
        %>
        <tr><td><%=person.Name %></td><td><%=person.Age %></td></tr>
        <%
        }
        %>
    </table>
</body>
</html>