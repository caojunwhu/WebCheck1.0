<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebMapCheck._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        请登录系统</h2>
    <p>
        &nbsp;</p>
<p>
        用户名：<asp:TextBox ID="tb_username" runat="server"></asp:TextBox>
    </p>
<p>
        密&nbsp;&nbsp; 码：<asp:TextBox ID="tb_password" runat="server" 
            TextMode="Password"></asp:TextBox>
    </p>
<p>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="bt_login" runat="server" onclick="bt_login_Click" Text="登  录" />
    </p>
    <p>
        &nbsp;</p>
</asp:Content>
