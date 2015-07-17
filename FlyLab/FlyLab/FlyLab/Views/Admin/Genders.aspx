<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.Gender>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div id="gendercontainer">
        <div class="col-sm-10 col-sm-offset-1">
            <h2 class="adminheader">Genders</h2>
            <p>
                For the application to work as intended, there must be exactly 2 genders named "Male" and "Female."
                <br />
                <br />
                If the genders already satisfy this condition, <strong>DO NOT CHANGE ANYTHING!</strong>
            </p>
            <table class="table table-bordered" id="gendertbl">
                <thead>
                    <tr>
                        <th>
                            <%: Html.DisplayNameFor(model => model.GenderName) %>
                        </th>
                        <th></th>
                    </tr>
                </thead>
                
                <tbody>
                <% foreach (var item in Model)
                       { %>
                    <tr>
                        <td>
                            <%: Html.DisplayFor(modelItem => item.GenderName) %>
                        </td>
                        <td>
                            <a class="btn btn-xs btn-default" href="/FlyLab/Admin/CreateGender/<%: Html.Raw(item.Id) %>"><i class="icon-pencil"></i></a>
                            <a class="btn btn-xs btn-danger" href="/FlyLab/Admin/DeleteGender/<%: Html.Raw(item.Id)%>"><i class="icon-trash"></i></a>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
                
            </table>
            <br />
            <a href="/FlyLab/Admin/CreateGender" class="btn btn-success">New Gender</a>
        </div>
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

        #gendertbl td:last-child {
            white-space: nowrap;
        }

        p {
            text-align: center;
        }
    </style>

    <script>
        $(function () {
            if ($('#gendertbl tbody').children('tr').length == 2) {
                $('.btn-success').hide();
            }

            $('#gendertbl').DataTable({
                searching: false,
                "dom": 'tr',
                "columnDefs": [
                    {
                        "targets": [1],
                        "bSortable": false,
                        "width": 1
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
