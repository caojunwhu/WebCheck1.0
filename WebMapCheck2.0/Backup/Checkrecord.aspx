<%@ Page Language="C#" Title = "检测记录" AutoEventWireup="true" CodeBehind="Checkrecord.aspx.cs" MasterPageFile="~/Site.master"   Inherits="WebMapCheck.Checkrecord" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
    &nbsp;
        <br />
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="样本平面与高程检测记录表:"></asp:Label>
        <br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="True">
    </asp:GridView>
        <br />
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="打印样本平面与高程误差统计表" Width="208px" />
        <br />
        <br />
        <asp:Label ID="Label3" runat="server" Text="样本间距检测记录表："></asp:Label>
        <br />
        <asp:GridView ID="GridView2" runat="server">
        </asp:GridView>
        <br />
        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" 
            Text="打印样本间距误差统计表" Width="184px" />
    </div>
 </asp:Content>


