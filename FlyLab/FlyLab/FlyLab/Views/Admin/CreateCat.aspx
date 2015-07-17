<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.Category>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <form class="form-horizontal" role="form" action="/FlyLab/Admin/CreateCat" method="post">
        <%: Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>
                <%if (Model == null)
                  { %>
                      Create 
                <%}
                  else
                  { %>
                      Edit 
                <%} %>
                Category
            </legend>
        </fieldset>

        <%: Html.HiddenFor(model => model.Id) %>

        <div class="form-group row">
            <label for="Name" class="col-sm-2 col-sm-offset-1 control-label">Name</label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line" id="CatName" name="CatName" autofocus tabindex="1"/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line" id="CatName" name="CatName" value="<%: Html.Raw(Model.CatName) %>" autofocus tabindex="1"/>
                <%} %>
                <%: Html.ValidationMessageFor(model => model.CatName) %>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6 col-sm-offset-3">
                <%if (Model == null)
                  { %>
                <div class="alert alert-info alert-dismissable fade in" role="alert">
                    <button type="button" class="close" data-dismiss="alert" tabindex="-1"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    Creating a category will also create an associated "Wild" trait.
                </div>
                <%} %>
                
                <input class="btn btn-success" type="submit" value="Submit" tabindex="3"/>
                <a class="btn btn-default" href="/FlyLab/Admin/Categories" tabindex="4">Back</a>
            </div>
        </div>
    </form>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        .field-validation-error {
            color:indianred;
        }
    </style>
    <script>
        $(function () {
            $('.alert').on('closed.bs.alert', function () {
                $('#CatName').focus();
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
