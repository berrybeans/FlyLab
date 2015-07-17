<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.Trait>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <legend>Deleting <%:Html.DisplayFor(model => model.Category.CatName) %> <%:Html.DisplayFor(model => model.Name)%></legend>
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
                        <%: Html.DisplayNameFor(model => model.Name) %>
                    </td>
                    <td id="traitName">
                        <%: Html.DisplayFor(model => model.Name) %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.Category) %>
                    </td>
                    <td>
                        <%: Html.DisplayFor(model => model.Category.CatName) %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.IsDominant) %>
                    </td>
                    <td>
                        <%if (Model.IsDominant)
                          { %>
                              True
                        <%}
                          else
                          { %>
                              False
                        <%} %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.IsIncompleteDominant) %>
                    </td>
                    <td>
                        <%if (Model.IsIncompleteDominant)
                          { %>
                              True
                        <%}
                          else
                          { %>
                              False
                        <%} %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.IsLethal) %>
                    </td>
                    <td>
                        <%if (Model.IsLethal)
                          { %>
                              True
                        <%}
                          else
                          { %>
                              False
                        <%} %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.ChromosomeNumber) %>
                    </td>
                    <td>
                        <%: Html.DisplayFor(model => model.ChromosomeNumber) %>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.DisplayNameFor(model => model.Distance) %>
                    </td>
                    <td>
                        <%: Html.DisplayFor(model => model.Distance) %>
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
        <div class="alert alert-danger hidden" role="alert" id="validationMessage">
            You cannot delete the "Wild" trait. Delete the trait's category instead.
        </div>
        <input class="btn btn-danger" type="submit" value="Delete" />
        <a class="btn btn-default" href="/FlyLab/Admin/Traits">Back</a>
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

        .hidden {
            visibility: hidden;
        }
    </style>

    <script>
        $(function () {
            $('.btn-danger').click(function (e) {
                if ($('#traitName').text().toLowerCase().trim() == "wild") {
                    e.preventDefault();
                    $('#validationMessage').removeClass('hidden');
                }
            });
        });
    </script>
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
