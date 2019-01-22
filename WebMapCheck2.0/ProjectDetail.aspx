<%@ Page Title="项目详情"Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" CodeBehind="ProjectDetail.aspx.cs" Inherits="WebMapCheck.ProjectDetail" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="项目详情："></asp:Label>
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
        <br />
    
    </div>
</asp:Content>

