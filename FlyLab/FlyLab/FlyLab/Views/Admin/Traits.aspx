<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.Trait>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div class="row">
        <div id="traitcontainer" class="col-sm-12">
            <h2 class="adminheader">Traits</h2>
            <form action="/FlyLab/Admin/Filter" method="post" class="form-horizontal">
                <div id="filter" class="row">
                    <div class="form-group">
                        <label for="Categories" class="control-label col-sm-4 col-xs-0">Filter by:</label>
                        <div style="margin: 0 auto; width: 33%;">
                            <%: Html.DropDownList("Categories", (IEnumerable<SelectListItem>)ViewData["Menu"], "All Categories") %>
                        </div>
                    </div>
                </div>
            </form>
            <div id="traits"></div><br />
            <a href="/FlyLab/Admin/CreateTrait" class="btn btn-success">New Trait</a>
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
            text-align:center;
            padding-bottom: .5em;
        }

        th {
            white-space: nowrap;
        }
    </style>
    
    <script>
        $(function () {
            $('select').addClass('form-control');
            $('option').each(function () {
                $(this).attr('value', $(this).val().trim());
            });

            $.ajax({
                datatype: 'json',
                url: '/FlyLab/Admin/Filter',
                type: 'POST',
                data: {
                    catID: 0
                },
                complete: function (o) {
                    $('#traits').html(o.responseText);
                    $('#sidebar').click();
                }
            });

            $('#Categories').change(function (e) {
                e.preventDefault();

                $.ajax({
                    datatype: 'json',
                    url: '/FlyLab/Admin/Filter',
                    type: 'POST',
                    data: {
                        catID: $('#Categories').val().trim()
                    },
                    complete: function (o) {
                        $('#traits').html(o.responseText);
                        $('#sidebar').click();
                    }
                });
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
