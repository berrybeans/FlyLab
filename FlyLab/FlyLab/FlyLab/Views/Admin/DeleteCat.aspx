<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.Category>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

<legend>Deleting <%:Html.DisplayFor(model => model.CatName) %></legend>
    <div class="row">
        <div class="col-sm-offset-2 col-sm-8">
            <table class="table">
            <thead>
                <tr>
                    <th>
                        Field
                    </th>
                    <th>
                        Value
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.CatName) %>
                    </td>
                    <td>
                        <%: Html.DisplayFor(model => model.CatName) %>
                    </td>
                </tr>
                <tr>
                    <td>
                        Traits to be deleted
                    </td>
                    <td>
                        <%if (Model.Traits.Count != 0)
                          {
                              foreach (var trait in Model.Traits)
	                        { %>
		                        <%: trait.Name %><br />
	                      <%}
                          }
                          else
                          { %>
                              <i>None</i>
                        <%} %>
                    </td>
                </tr>
            </tbody>
        </table>
        </div>
    </div>

<% using (Html.BeginForm())
   { %>
    <%: Html.AntiForgeryToken() %>
    <div class="col-sm-6 col-sm-offset-2">
        <input class="btn btn-danger" type="submit" value="Delete" />
        <a class="btn btn-default" href="/FlyLab/Admin/Categories">Back</a>
    </div>
<% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            max-width: 60%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="sidebar" runat="server">
    <% Html.RenderPartial("~/Views/Shared/_AdminSidebar.ascx"); %>
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
