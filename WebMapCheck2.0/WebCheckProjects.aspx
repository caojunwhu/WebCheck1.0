<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebCheckProjects.aspx.cs" Inherits="WebMapCheck.WebCheckProjects" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/resources/css/examples.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
       <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Default" />

       <ext:Store ID="Store1" runat="server" OnReadData="Main_ReadData">
            <Model>
                <ext:Model runat="server">
                    <Fields>
                        <ext:ModelField Name="projectname" />
                        <ext:ModelField Name="producer" />
                        <ext:ModelField Name="owner"/>
                        <ext:ModelField Name="shared"  />
                        <ext:ModelField Name="lastupdatetime" />
                        <ext:ModelField Name="projectid" />
                    </Fields>
                </ext:Model>
            </Model>
        </ext:Store>

         <ext:GridPanel runat="server" 
            Title="我的检验项目" 
            StoreID="Store1"
            Width="900"
            Height="420">
            <ColumnModel runat="server">
                <Columns>
                    <ext:Column
                        runat="server"
                        Text="项目名称"
                        DataIndex="projectname"
                        Flex="1"
                        />
                    <ext:Column
                        runat="server"
                        Text="生产单位"
                        Width="60"
                        DataIndex="producer">
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="创建者"
                        Width="60"
                        DataIndex="owner">                       
                    </ext:Column>
                    <ext:Column
                        runat="server"
                        Text="参与者"
                        Flex="1"
                        DataIndex="shared">                        
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
                    <ext:CommandColumn runat="server" Flex ="1">
                        <Commands>
                            <ext:GridCommand Icon="Eye" CommandName="MathPrecision" Text="数学精度" />
                            <ext:GridCommand Icon="Eye" CommandName="Detail" Text="详情" />
                            <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="编辑" />
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
