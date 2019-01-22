<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AjaxPage.aspx.cs" Inherits="WebMapCheck.AjaxPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript" src="Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.autocomplete.min.js"></script>
    <link href="Styles/jquery.autocomplete.css" rel="stylesheet" />

     <script type="text/javascript">
 
         $(document).ready(function () {            
             $("#txtAutoComplete").autocomplete("AutoComplete.ashx", {
                 max: 10,             //列表里的条目数
                 minChars: 1,         //自动完成激活之前填入的最少字符
                 scrollWidth: 173,    //提示的宽度，溢出隐藏
                 scrollHeight: 200,   //提示的高度，溢出显示滚动条
                 scroll: true,        //是否显示滚动条
                 matchContains: true, //包含匹配，是否只显示匹配项
                 autoFill: false,     //自动填充
                 //此处实际请求的URL为"AutoComplete.ashx?q='[你在txtAutoComplete中输入的值]'&action='autoComplete'&value='guo'"
                 extraParams: { action: "autoComplete", value: $("#txtAutoComplete").val() },
                 //格式化列表中的条目 row:条目对象,i:当前条目index,max:总条目数
                 formatItem: function (row, i, max) {
                     //【不用转化为js对象，但必须返回row.toString()】
                     return row.toString();
                 },
                 //配合formatItem使用，作用在于，由于使用了formatItem，所以显示的条目内容有所改变，
                 //而我们要匹配的是原始的数据，所以用formatMatch做一个调整，使之匹配原始数据
                 formatMatch: function (row, i, max) {
                     //【不用转化为js对象，但必须返回row.toString()】            
                     return row.toString();
                 },
                 //设置用户选择某一条目后文本框显示的内容
                 formatResult: function (row) {
                     //【不用转化为js对象，但必须返回row.toString()】
                     return "文本框显示的结果：" + row.toString();
                 }
             }).result(function (event, row, formatted) {
                 //获取用户选择的条目
                 alert(row.toString());
             });
         });

         </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        请输入：<asp:TextBox   id="txtAutoComplete"  runat="server" />

    </div>
    </form>
</body>
</html>
