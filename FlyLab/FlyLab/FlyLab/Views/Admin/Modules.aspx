<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.Module>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <!-- Modal -->
    <div class="modal fade" id="modeditmodal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title">Edit Module</h4>
                </div>
                <div class="modal-body">
                    <form class="form-horizontal" action="/FlyLab/Admin/CreateMod" method="post" role="form" id="moduleform">
                    </form>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-lg btn-success" type="submit" form="moduleform">Save</button>
                    <button class="btn btn-lg btn-danger" id="btn-delete">Delete</button>
                    <button type="button" class="btn btn-lg btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div id="modulecontainer" class="col-sm-10 col-sm-offset-1">
            <h2 class="adminheader">Modules</h2>
            <table class="table table-bordered" id="moduletable">
                <thead>
                    <tr>
                        <th><%: Html.DisplayNameFor(model => model.ModuleName) %></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <%foreach (var item in Model)
                      { %>
                        <tr>
                            <td><%: Html.DisplayFor(model => item.ModuleName) %></td>
                            <td>
                                <% if (item.Active)
                                   { %>
                                       <a title="Deactivate module <%=item.ModuleName %>" class="btn btn-xs btn-danger" href="/FlyLab/Admin/DeactivateModule/<%=item.Id %>"><i class="glyphicon glyphicon-minus"></i></a>
                                <% } else { %>
                                       <a title="Activate module <%=item.ModuleName %>" class="btn btn-xs btn-success" href="/FlyLab/Admin/ActivateModule/<%=item.Id %>"><i class="glyphicon glyphicon-plus"></i></a>
                                <% } %>
                            </td>
                        </tr>
                    <%} %>
                </tbody>
            </table>
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

        ul li {
            background: none;
            padding: 0;
        }

        p {
            text-align: center;
        }

        #moduletable td:last-child {
            white-space: nowrap;
        }

        .btn-controls {
            margin-top: 1em;
        }
    </style>
    <script>
        $(function () {
            //this fixes the sidebar wanting to shorten too much
            $('#sidebar').click();

            $('#moduletable').DataTable({
                searching: false,
                "bSort": false,
                "dom": 'tr',
                "columnDefs": [
                    {
                        "targets": [0, 1],
                        "bSortable": false
                    },
                    {
                        "targets": [1],
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
