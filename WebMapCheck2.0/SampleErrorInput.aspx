<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SampleErrorInput.aspx.cs" Inherits="WebMapCheck.SampleErrorInput" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript" src="Scripts/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery.autocomplete.min.js"></script>
    <link href="Styles/jquery.autocomplete.css" rel="stylesheet" />

     <script type="text/javascript">
 
         $(document).ready(function () {            
             $("#TextBox1").autocomplete("AutoComplete.ashx", {
                 max: 100,             //列表里的条目数
                 minChars: 1,         //自动完成激活之前填入的最少字符
                 scrollWidth: 400,    //提示的宽度，溢出隐藏
                 scrollHeight: 120,   //提示的高度，溢出显示滚动条
                 scroll: true,        //是否显示滚动条
                 matchContains: true, //包含匹配，是否只显示匹配项
                 autoFill: false,     //自动填充
                 //此处实际请求的URL为"AutoComplete.ashx?q='[你在txtAutoComplete中输入的值]'&action='autoComplete'&value='guo'"
                 extraParams: { action: "autoComplete", value: $("#TextBox1").val() },
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
    <script type="text/javascript">
    function setCss(opt){
        var sr = document.getElementById("TextBox1");
        var len=sr.value.length;
        setSelectionRange(sr,len,len); //将光标定位到文本最后 
    }

    function setSelectionRange(input, selectionStart, selectionEnd) {
        if (input.setSelectionRange) {  
        input.focus();  
        input.setSelectionRange(selectionStart, selectionEnd);  
        }  
        else if (input.createTextRange) {  
        var range = input.createTextRange();  
        range.collapse(true);  
        range.moveEnd('character', selectionEnd);  
        range.moveStart('character', selectionStart);  
        range.select();  
        }  
    }  
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager runat="server" ID="ScriptManager1" ></asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
            <asp:Label ID="Label1" runat="server" Text="录入问题描述："></asp:Label>
            &nbsp;&nbsp;&nbsp;
            <asp:TextBox ID="TextBox1" runat="server"  ClientIDMode="Static"   OnTextChanged="TextBox1_TextChanged"  Font-Names="宋体" Font-Size="Medium" Height="22px" Width="400px"></asp:TextBox> 
            &nbsp; 
            <asp:Button ID="Button1" runat="server" Height="28px" OnClick="Button1_Click" Text="智能建议" Width="71px" />
                <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="所属质量元素："></asp:Label>
            &nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="tbFClass" runat="server" Font-Size="Medium" Height="22px" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="tbFClass_SelectedIndexChanged" ></asp:DropDownList>
            &nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label5" runat="server" Text="所属质量子元素："></asp:Label>
            &nbsp;<asp:DropDownList ID="tbSClass" runat="server" Font-Size="Medium" Height="22px" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="tbSClass_SelectedIndexChanged"></asp:DropDownList>
            &nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label6" runat="server" Text="检查项："></asp:Label>
            <asp:DropDownList ID="tbCheckItem" runat="server" Font-Size="Medium" Height="22px" Width="100px"></asp:DropDownList>
            <asp:Label ID="Label3" runat="server" Text="错漏类别："></asp:Label>
            &nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="tbErrorClass" runat="server" Font-Size="Medium" Height="22px" Width="100px" AutoPostBack="True" OnSelectedIndexChanged="tbErrorClass_SelectedIndexChanged">
                <asp:ListItem Value="A"></asp:ListItem>
                <asp:ListItem Value="B"></asp:ListItem>
                <asp:ListItem Value="C"></asp:ListItem>
                <asp:ListItem Value="D"></asp:ListItem>
                <asp:ListItem></asp:ListItem>
    </asp:DropDownList>
                <br />
            <br />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label4" runat="server" Text="参   考   描    述："></asp:Label>            &nbsp;
            <asp:DropDownList ID="DropDownList3" runat="server" Height="22px" Width="400px" Font-Size="Medium" OnSelectedIndexChanged="DropDownList3_SelectedIndexChanged" AutoPostBack="True">
            </asp:DropDownList>
                 </ContentTemplate>
            </asp:UpdatePanel>            
               <br />
                <br />
                <hr />
               
               <ext:ResourceManager ID="ResourceManager1" runat="server" Theme="Default" />
               <ext:Store ID="Store1" runat="server" OnReadData="Main_ReadData">
                    <Model>
                        <ext:Model runat="server">
                            <Fields>
                                <ext:ModelField Name="序号" />
                                <ext:ModelField Name="质量元素" />
                                <ext:ModelField Name="质量子元素" />
                                 <ext:ModelField Name="检查项" />
                                <ext:ModelField Name="错漏类别"/>
                                <ext:ModelField Name="错漏描述"  />
                                <ext:ModelField Name="处理意见" />
                                <ext:ModelField Name="复查情况" />
                                <ext:ModelField Name="修改情况"/>
                                <ext:ModelField Name="检查者"  />
                                <ext:ModelField Name="检查日期"  />
                            </Fields>
                        </ext:Model>
                    </Model>
                </ext:Store>

                 <ext:GridPanel runat="server" 
                     ID="GridPanel1"
                    Title="检查意见记录表" 
                    Width="900"
                    Height="420">
                   <Buttons>
                        <ext:Button
                            runat="server"
                            Text="增加输入检查意见到列表"
                            EnableToggle="true"
                            ToggleGroup="set"
                            Pressed="true">
                            <DirectEvents>
                                <Click OnEvent="AddCustomCheckState"  ></Click>
                            </DirectEvents>
                        </ext:Button>

                        <ext:Button
                            runat="server"
                            id="btn1"
                            Text="增加标准参考描述到列表"
                            EnableToggle="true"
                            ToggleGroup="set">
                            <DirectEvents>
                                <Click OnEvent="AddStandardCheckState"  ></Click>
                            </DirectEvents>
                        </ext:Button>
                </Buttons>
                    <ColumnModel runat="server">
                        <Columns>
                           <ext:Column
                                runat="server"
                                Text="序号"
                                DataIndex="序号"
                                Width="40"
                                />
                            <ext:Column
                                runat="server"
                                Text="质量元素"
                                DataIndex="质量元素"
                                Width="60"
                                />
                            <ext:Column
                                runat="server"
                                Text="质量子元素"
                                Width="60"
                                DataIndex="质量子元素">
                            </ext:Column>
                            <ext:Column
                                runat="server"
                                Text="检查项"
                                Width="60"
                                DataIndex="检查项">
                            </ext:Column>
                            <ext:Column
                                runat="server"
                                Text="错漏类别"
                                Width="60"
                                DataIndex="错漏类别">                       
                            </ext:Column>
                            <ext:Column
                                runat="server"
                                Text="错漏描述"
                                Width="200"
                                DataIndex="错漏描述">                        
                            </ext:Column>
                              <ext:Column
                                runat="server"
                                Text="处理意见"
                                Width="60"
                                DataIndex="处理意见">                        
                            </ext:Column>
                              <ext:Column
                                runat="server"
                                Text="复查情况"
                                Width="60"
                                DataIndex="复查情况">                        
                            </ext:Column>
                              <ext:Column
                                runat="server"
                                Text="修改情况"
                                Width="60"
                                DataIndex="修改情况">                        
                            </ext:Column>
                              <ext:Column
                                runat="server"
                                Text="检查者"
                                Width="60"
                                DataIndex="检查者">                        
                            </ext:Column>
                            <ext:Column
                                runat="server"
                                Text="检查日期"
                                Width="70"
                                DataIndex="检查日期"/>
                            <ext:CommandColumn runat="server" Flex ="1">
                                <Commands>
                                    <ext:GridCommand Icon="Delete" CommandName="Delete" Text="Delete" />
                                    <ext:GridCommand Icon="NoteEdit" CommandName="Edit" Text="Edit" />
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
