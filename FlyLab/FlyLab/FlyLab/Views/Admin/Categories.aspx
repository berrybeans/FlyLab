<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.Category>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div id="categoriescontainer" class="col-sm-10 col-sm-offset-1">
        <h2 class="adminheader">Categories</h2>
        <table class="table table-bordered table-hover" id="cattable">
            <thead>
                <tr>
                    <th>
                        <%: Html.DisplayNameFor(model => model.CatName) %>
                    </th>
                    <th># Traits
                    </th>
                    <th></th>
                </tr>
            </thead>

            <tbody>
                <% foreach (var item in Model)
                   { %>
                <tr>
                    <td>
                        <%: Html.DisplayFor(modelItem => item.CatName) %>
                    </td>
                    <td>
                        <%: Html.DisplayFor(modelItem => item.Traits.Count) %>
                    </td>
                    <td>
                        <a class="btn btn-xs btn-default" href="/FlyLab/Admin/CreateCat/<%: Html.Raw(item.Id) %>"><i class="glyphicon glyphicon-pencil"></i></a>
                        <a class="btn btn-xs btn-danger" href="/FlyLab/Admin/DeleteCat/<%: Html.Raw(item.Id) %>"><i class="glyphicon glyphicon-trash"></i></a>
                    </td>
                </tr>
                <% } %>
            </tbody>
        </table>
        <br />
        <a href="/FlyLab/Admin/CreateCat" class="btn btn-success">New Category</a>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.css" />
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.js"></script>
    <style>
        .adminheader {
            border-bottom: solid 1px lightgrey;
            text-align: center;
            padding-bottom: .5em;
        }

        ul li {
            background: none;
            padding: 0;
        }

        #traittable td:last-child {
            white-space: nowrap;
        }

        #filter {
            position: relative;
            padding-top: .6em;
            margin: 0 auto;
            /*right: 8%;*/
        }
    </style>
    <script>
        $(function () {
            $('#cattable').DataTable({
                searching: false,
                "dom": 'tr',
                "columnDefs": [
                    {
                        "targets": [1],
                        "width": 50
                    },
                    {
                        "targets": [2],
                        "bSortable": false,
                        "width": 32
                    }
                ]
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
