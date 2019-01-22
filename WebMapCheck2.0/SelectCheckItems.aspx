<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectCheckItems.aspx.cs" Inherits="WebMapCheck.SelectCheckItems" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<link href="/resources/css/examples.css" rel="stylesheet" />
    <script type="text/javascript">
        var formatHours = function (v) {
            if (v < 1) {
                return Math.round(v * 60) + " mins";
            } else if (Math.floor(v) !== v) {
                var min = v - Math.floor(v);
                return Math.floor(v) + "h " + Math.round(min * 60) + "m";
            } else {
                return v + " hour" + (v === 1 ? "" : "s");
            }
        };

        var handler = function (grid, rowIndex, colIndex, actionItem, event, record, row) {
            Ext.Msg.alert('Editing' + (record.get('done') ? ' completed task' : '') , record.get('task'));
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <ext:ResourceManager runat="server" />
 <ext:TreePanel
            ID="TreePanel1"
            runat="server"
            Title="设置质量模型"
            Width="900"
            Height="450"
            Collapsible="true"
            UseArrows="true"
            RootVisible="false"
            MultiSelect="true"
            SingleExpand="true"
            FolderSort="true">
            <Fields>
                <ext:ModelField Name="项目类型" />
                <ext:ModelField Name="成果种类" />
                <ext:ModelField Name="检查项" />
                <ext:ModelField Name="质量元素权" />
                <ext:ModelField Name="质量子元素权" />
                <ext:ModelField Name="选择" Type="Boolean" />
            </Fields>
            <ColumnModel>
                <Columns>
                    <ext:TreeColumn
                        runat="server"
                        Text="项目类型"
                        Flex="2"
                        Sortable="true"
                        DataIndex="项目类型" />
                    <ext:Column
                        runat="server"
                        Text="成果种类"
                        Flex="1"
                        Sortable="true"
                        DataIndex="成果种类" />
                    <ext:Column
                        runat="server"
                        Text="检查项"
                        Flex="1"
                        Sortable="true"
                        DataIndex="检查项" /> 
                    <ext:Column
                        runat="server"
                        Text="质量元素权"
                        Flex="1"
                        Sortable="true"
                        DataIndex="质量元素权" />
                    <ext:Column
                        runat="server"
                        Text="质量子元素权"
                        Flex="1"
                        Sortable="true"
                        DataIndex="质量子元素权" />
                    <ext:CheckColumn runat="server"
                       Text="选择"
                       DataIndex="选择"
                       Width="60"
                       Editable="true"
                       StopSelection="false" />
                    <ext:ActionColumn runat="server"
                        Text="Edit"
                        Width="50"
                        MenuDisabled="true"
                        Align="Center">
                        <Items>
                            <ext:ActionItem Tooltip="Edit task" Icon="PageWhiteEdit" Handler="handler">
                                <IsDisabled Handler="return !record.data.leaf;" />
                            </ext:ActionItem>
                        </Items>
                    </ext:ActionColumn>
                </Columns>

            </ColumnModel>

        </ext:TreePanel>

</asp:Content>
