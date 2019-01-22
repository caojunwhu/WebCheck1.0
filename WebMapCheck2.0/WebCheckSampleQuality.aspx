<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebCheckSampleQuality.aspx.cs" Inherits="WebMapCheck.WebCheckSampleQuality" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/resources/css/examples.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
       <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Default" />
       <ext:Store ID="Store1" runat="server"  >
            <Model>
                <ext:Model runat="server">
                    <Fields>
                        <ext:ModelField Name="mapnumber" />
                        <ext:ModelField Name="mapid" />
                        <ext:ModelField Name="score" />
                        <ext:ModelField Name="qualityitem"/>
                        <ext:ModelField Name="qitemweight"/>
                        <ext:ModelField Name="qitemscore"  />
                        <ext:ModelField Name="subqualityitem" />
                        <ext:ModelField Name="subqitemweight" />
                        <ext:ModelField Name="subqitemscore" />
                        <ext:ModelField Name="faulta" />
                        <ext:ModelField Name="faultb" />
                        <ext:ModelField Name="faultc" />
                        <ext:ModelField Name="faultd" />
                        <ext:ModelField Name="projectid" />
                    </Fields>
                </ext:Model>
            </Model>
        </ext:Store>

         <ext:GridPanel runat="server" 
            Title="检验项目样本列表" 
            StoreID="Store1"
            Width="900"
            Height="420">
            <ColumnModel runat="server">
                <Columns>
                    <ext:Column
                        runat="server"
                        Text="样本号"
                        Flex="2"
                        DataIndex="mapnumber">
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="样本得分"
                        DataIndex="score"
                        Flex="1"/>
                    <ext:Column
                        runat="server"
                        Text="质量元素"
                         Flex="1"
                        DataIndex="qualityitem">                       
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="质量元素权"
                         Flex="1"
                        DataIndex="qitemweight">    
                    </ext:Column>     
                    <ext:Column
                        runat="server"
                        Text="质量元素得分"
                        Flex="1"
                        DataIndex="qitemscore">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="质量子元素"
                        Flex="1"
                        DataIndex="subqualityitem">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="质量子元素权"
                       Flex="1"
                        DataIndex="subqitemweight">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="质量子元素得分"
                        Flex="1"
                        DataIndex="subqitemscore">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="A类错"
                        Flex="1"
                        DataIndex="faulta" DefaultType="float">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="B类错"
                        Flex="1"
                        DataIndex="faultb" DefaultType="float">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="C类错"
                        Flex="1"
                        DataIndex="faultc" DefaultType="float">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="D类错"
                        Flex="1"
                        DataIndex="faultd" DefaultType="float">               
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="项目ID"
                        Width="0"
                        Visiable="false"
                        DataIndex="projectid">      
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="样本ID"
                        Width="0"
                        Visiable="false"
                        DataIndex="mapid">
                    </ext:Column>
                </Columns>
            </ColumnModel>
            <SelectionModel>
                <ext:RowSelectionModel runat="server" Mode="Single" />
            </SelectionModel>

            </ext:GridPanel>
</asp:Content>
