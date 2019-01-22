<%@ Page Language="C#" Title="抽样详情" AutoEventWireup="true" MasterPageFile="~/Site.master"  CodeBehind="SampleDetail.aspx.cs" Inherits="WebMapCheck.SampleDetail" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
    
        &nbsp;<br />
    
        <asp:Label ID="Label1" runat="server" Text="抽样详情："></asp:Label>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="抽样分区" HeaderText="抽样分区" SortExpression="抽样分区" />
                <asp:BoundField DataField="流水号" HeaderText="流水号" SortExpression="流水号" />
                <asp:HyperLinkField DataNavigateUrlFields="图幅号" 
                    DataNavigateUrlFormatString="Checkrecord.aspx?mapnumber={0}" 
                    DataTextField="图幅号" HeaderText="图幅号" />
                <asp:BoundField DataField="点位中误差" HeaderText="点位中误差" SortExpression="点位中误差" />
                <asp:BoundField DataField="点位粗差比率" HeaderText="点位粗差比率" 
                    SortExpression="点位粗差比率" />
                <asp:BoundField DataField="点位精度得分" HeaderText="点位精度得分" 
                    SortExpression="点位精度得分" />
                <asp:BoundField DataField="高程中误差" HeaderText="高程中误差" SortExpression="高程中误差" />
                <asp:BoundField DataField="高程粗差比率" HeaderText="高程粗差比率" 
                    SortExpression="高程粗差比率" />
                <asp:BoundField DataField="高程精度得分" HeaderText="高程精度得分" 
                    SortExpression="高程精度得分" />
                <asp:BoundField DataField="等高线中误差" HeaderText="等高线中误差" 
                    SortExpression="等高线中误差" />
                <asp:BoundField DataField="等高线粗差比率" HeaderText="等高线粗差比率" 
                    SortExpression="等高线粗差比率" />
                <asp:BoundField DataField="等高线精度得分" HeaderText="等高线精度得分" 
                    SortExpression="等高线精度得分" />
                <asp:BoundField DataField="间距中误差" HeaderText="间距中误差" SortExpression="间距中误差" />
                <asp:BoundField DataField="间距粗差比率" HeaderText="间距粗差比率" 
                    SortExpression="间距粗差比率" />
                <asp:BoundField DataField="间距精度得分" HeaderText="间距精度得分" 
                    SortExpression="间距精度得分" />
                <asp:BoundField DataField="备注" HeaderText="备注" SortExpression="备注" />
            </Columns>
        </asp:GridView>
        <br />
        <asp:Button ID="btn_compPosition" runat="server" 
            onclick="btn_compPosition_Click" Text="计算抽样位置精度" Width="146px" />
&nbsp;&nbsp;
        <asp:Button ID="btn_relativeComp" runat="server" 
            onclick="btn_relativeComp_Click" Text="计算抽样间距精度" Width="135px" />
        <br />
        <br />
    
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="打印位置精度统计表" Width="145px" />
&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Text="打印间距精度统计表" Width="135px" 
            onclick="Button2_Click" />
        &nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" onclick="Button3_Click" 
            Text="样本图幅检测精度统计表打印" Width="200px" />
        <br />
    
    </div>
        <div id="ProgressBarSide" style="position:absolute;height:21px;width:100px;color:Silver;border-width:1px;border-style:Solid;display:none">
        <div id="ProgressBar" style="position:absolute;height:21px;width:0%;background-color: green"></div>
        <div id="ProgressText" style="position:absolute;height:21px;width:100%;text-align:center"></div>
    </div>
</asp:Content>
