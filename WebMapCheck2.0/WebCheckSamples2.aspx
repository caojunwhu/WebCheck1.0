<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebCheckSamples2.aspx.cs" Inherits="WebMapCheck.WebCheckSamples2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/resources/css/examples.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
       <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Default" />
       <ext:Store ID="Store1" runat="server" OnReadData="Main_ReadData">
            <Model>
                <ext:Model runat="server">
                    <Fields>
                        <ext:ModelField Name="mapnumber" />
                        <ext:ModelField Name="mapid" />
                        <ext:ModelField Name="maptype" />
                        <ext:ModelField Name="checkqitems"/>
                        <ext:ModelField Name="score"  />
                        <ext:ModelField Name="level" />
                        <ext:ModelField Name="lastupdatetime" />
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
                        Text="成果类型"
                        Flex="1"
                        DataIndex="maptype">
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="样本号"
                        DataIndex="mapnumber"
                        Flex="2"/>
                    <ext:Column
                        runat="server"
                        Text="检验参数"
                        Flex="2"
                        DataIndex="checkqitems">                       
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="样本得分"
                        Flex="1"
                        DataIndex="score">    
                    </ext:Column>     
                    <ext:Column
                        runat="server"
                        Text="质量等级"
                        Flex="1"
                        DataIndex="level">               
                    </ext:Column>
                      <ext:DateColumn
                        runat="server"
                        Text="最近更新时间"
                        Flex="1"
                        DataIndex="lastupdatetime" Format="yyyy-MM-dd hh:mm:ss">                        
                    </ext:DateColumn>
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
                    <ext:CommandColumn runat="server" Flex ="5">
                        <Commands>
                            <ext:GridCommand Icon="Eye" CommandName="MathPrecision" Text="数学精度" />
                            <ext:GridCommand Icon="Eye" CommandName="CheckRecord" Text="质检意见" />
                            <ext:GridCommand Icon="Eye" CommandName="QualityModel" Text="质量模型" />
                            <ext:GridCommand Icon="Eye" CommandName="Quality" Text="质量统计" />
                            <ext:GridCommand Icon="Delete" CommandName="Delete" Text="删除" />
                        </Commands>
                        <DirectEvents>
                            <Command OnEvent="MyButtonClickHandler">
                                <ExtraParams> 
                                     <ext:Parameter  Name = "id"  Value = "record.data"  Mode = "Raw" /> 
                                     <ext:Parameter  Name = "command"  Value = "command"  Mode = "Raw" />
                                </ExtraParams> 
                            </Command>
                        </DirectEvents>
                    </ext:CommandColumn>
                </Columns>
            </ColumnModel>
            <SelectionModel>
                <ext:RowSelectionModel runat="server" Mode="Single" />
            </SelectionModel>

            </ext:GridPanel>
</asp:Content>
