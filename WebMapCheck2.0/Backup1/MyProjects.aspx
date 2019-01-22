<%@ Page Title="检验项目管理" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="MyProjects.aspx.cs" Inherits="WebMapCheck.MyProjects" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>    
        这是我建立的检验项目：<br />
        <asp:GridView ID="GridView1" runat="server" BackColor="#CCCCCC" 
            BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" 
            CellSpacing="2" ForeColor="Black" AutoGenerateColumns="False">
            <Columns>
                <asp:HyperLinkField DataTextField="成果名称" HeaderText="成果名称" 
                    NavigateUrl="~/ProjectDetail.aspx" DataNavigateUrlFields="成果名称" 
                    DataNavigateUrlFormatString="ProjectDetail.aspx?project={0}" />
                <asp:BoundField DataField="批量" HeaderText="批量" SortExpression="批量" />
                <asp:HyperLinkField DataNavigateUrlFields="成果名称" 
                    DataNavigateUrlFormatString="SampleDetail.aspx?project={0}" 
                    DataTextField="样本数量" HeaderText="样本数量" />
                <asp:BoundField DataField="批量单位" HeaderText="批量单位" SortExpression="批量单位" />
                <asp:BoundField DataField="比例尺" HeaderText="比例尺" SortExpression="比例尺" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="Gray" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
    </p>

</asp:Content>