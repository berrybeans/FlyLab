<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.ImageSettings>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <div id="imagesettingscontainer" class="col-sm-10 col-sm-offset-1">
        <h2 class="adminheader">Image Paths</h2>
        <div id="tablearea">
            <table class="table table-bordered fixed">
                <thead>
                    <tr>
                        <th>Image Path</th>
                        <th class="btns"></th>
                    </tr>
                </thead>
                <tbody>
                    <% foreach (var item in Model)
                       { %>
                           <tr>
                               <td>
                               <% if (item.SecCat != "")
                                  { %>
                                      <%: Html.DisplayFor(t => item.Prefix) %>[<%: Html.DisplayFor(t => item.FirstCat) %>][<%: Html.DisplayFor(t => item.SecCat) %>]<%: Html.DisplayFor(t => item.Suffix) %>
                               <% }
                                  else
                                  { %>
                                      <%: Html.DisplayFor(t => item.Prefix) %>[<%: Html.DisplayFor(t => item.FirstCat) %>]<%: Html.DisplayFor(t => item.Suffix) %>
                               <% } %>
                                   
                               </td>
                               <td class="btns">
                                   <a class="btn btn-xs btn-default" href="/FlyLab/Admin/CreateImg/<%: Html.Raw(item.Id) %>"><i class="glyphicon glyphicon-pencil"></i></a>
                                   <a class="btn btn-xs btn-danger delbtn" href="#" id="<%: Html.Raw(item.Id) %>"><i class="glyphicon glyphicon-trash"></i></a>
                               </td>
                           </tr>
                    <% } %>
                </tbody>
            </table>
        </div>
        <div id="buttonarea">
            <a href="/FlyLab/Admin/CreateImg" class="btn btn-success">New Path Definition</a>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        .adminheader {
            border-bottom: solid 1px lightgrey;
            text-align: center;
            padding-bottom: .5em;
        }

        .btns {
            width:70px;
        }
    </style>
    <script>
        $(function () {
            $('.delbtn').click(function (e) {
                e.preventDefault();
                var id = $(this).attr('id').trim();
                $.post('/FlyLab/Admin/DeleteImg', { id: id },
                    function (o) {
                        window.location.href = o.url;
                    }, 'json');
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
