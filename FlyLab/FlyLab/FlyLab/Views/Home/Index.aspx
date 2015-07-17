<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.Module>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <div id="index">
        <h2>Fly Lab Portal</h2>
        <div class="row">
            <div class="col-sm-6 col-sm-offset-3">
                <%: Html.ContentArea("PortalConMan", false) %>
                <div id="modulepicker" class="row">
                    <br />
                        <%foreach (var module in Model)
                          { %>
                            <div>
                            <%: Html.ActionLink(module.ModuleName + " Cross", "Module", "Lab", new { id = module.Call_id }, null) %> 
                                <%if (CWSToolkit.AuthUser.IsGlobalAdmin())
                                  { %>
                                      ( <%: Html.ActionLink("dev", "Module", "Lab", new { id = module.Call_id, dev = true }, null) %> )
                                <%} %>
                                
                                <br />
                            </div>
                        <%} %>            
                    <br />
                    <a href="/Lab/testcrosses"><strong>RUN TEST</strong></a>
                </div>
            </div>
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        h1,h2,h3 {
            border-bottom: solid 1px lightgrey;
            text-align:center;
            padding-bottom: .7em;
            margin-bottom: .7em;
        }
        #modulepicker {
            text-align: center;
        }
        ul li {
            background: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="sidebar" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="breadcrumb" runat="server">
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="navbar" runat="server">
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="mainHeading" runat="server">
</asp:Content>

<asp:Content ID="Content9" ContentPlaceHolderID="subHeading" runat="server">
</asp:Content>

<asp:Content ID="Content10" ContentPlaceHolderID="subFooter" runat="server">
</asp:Content>
