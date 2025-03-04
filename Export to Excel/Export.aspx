<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="Society.Export" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    Export Data
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
    <asp:DropDownList ID="ddlTableSelection" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTableSelection_SelectedIndexChanged">
        <asp:ListItem Text="Bills" Value="bills" Selected="True"></asp:ListItem>
        <asp:ListItem Text="Visitors" Value="visitors"></asp:ListItem>
        <asp:ListItem Text="Complaints" Value="complaints"></asp:ListItem>
    </asp:DropDownList>

    <asp:TextBox ID="txtFromDate" runat="server" placeholder="From Date" TextMode="Date"></asp:TextBox>
    <asp:TextBox ID="txtToDate" runat="server" placeholder="To Date" TextMode="Date"></asp:TextBox>
    <asp:Button ID="btnFilter" runat="server" Text="Filter" OnClick="btnFilter_Click" />

    <br /><br />

    <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="True" OnRowDataBound="gvData_RowDataBound">
    </asp:GridView>

    <br />
    <asp:Button ID="btnExportAll" runat="server" Text="Export All Filtered Data" OnClick="btnExportAll_Click" />

</asp:Content>