<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.Gender>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

    <form class="form-horizontal" role="form" action="/FlyLab/Admin/CreateGender" method="post">
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
                Gender
            </legend>
        </fieldset>

        <%: Html.HiddenFor(model => model.Id) %>
        <div class="form-group row">
            <label for="Name" class="col-sm-2 col-sm-offset-1 control-label">Name</label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line" id="GenderName" name="GenderName" autofocus/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line" id="GenderName" name="GenderName" value="<%: Html.Raw(Model.GenderName) %>" autofocus/>
                <%} %>
                <%: Html.ValidationMessageFor(model => model.GenderName) %>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6 col-sm-offset-3">
                <input class="btn btn-success" type="submit" value="Submit" />
                <a class="btn btn-default" href="/FlyLab/Admin/Genders">Back</a>
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
