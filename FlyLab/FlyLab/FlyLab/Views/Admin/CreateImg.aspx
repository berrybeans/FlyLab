<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.ImageSettings>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
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
            Image Path
        </legend>
    </fieldset>

    <%: Html.HiddenFor(model => model.FirstCat) %>
    <%: Html.HiddenFor(model => model.SecCat) %>

    <div id="imagecontainer">
        <form class="form-horizontal" role="form" action="/FlyLab/Admin/CreateImg" method="post">
            <%: Html.HiddenFor(model => model.Id) %>

            <div class="form-group row">
                <label class="col-sm-2 col-sm-offset-1 control-label" for="Prefix"><%: Html.DisplayNameFor(model => model.Prefix) %></label>
                <div class="col-sm-6">
                    <%: Html.TextBoxFor(model => model.Prefix, new { @class = "form-control"}) %>
                    <%: Html.ValidationMessageFor(model => model.Prefix) %>
                </div>
            </div>

            <div class="form-group row">
                <label class="col-sm-2 col-sm-offset-1 control-label" for="FirstCat"><%: Html.DisplayNameFor(model => model.FirstCat) %></label>
                <div class="col-sm-6">
                    <%: Html.DropDownList("FirstCat") %>
                </div>
            </div>

            <div class="form-group row">
                <label class="col-sm-2 col-sm-offset-1 control-label" for="SecCat"><%: Html.DisplayNameFor(model => model.SecCat) %></label>
                <div class="col-sm-6">
                    <%: Html.DropDownList("SecCat", (IEnumerable<SelectListItem>)ViewData["Menu"], "N/A" ) %>
                </div>
            </div>

            <div class="form-group row">
                <label class="col-sm-2 col-sm-offset-1 control-label" for="Suffix"><%: Html.DisplayNameFor(model => model.Suffix) %></label>
                <div class="col-sm-6">
                    <%: Html.TextBoxFor(model => model.Suffix, new { @class = "form-control" }) %>
                    <%: Html.ValidationMessageFor(model => model.Suffix) %>
                </div>
            </div>

            <div class="form-group row">
                <div class="col-sm-6 col-sm-offset-3">
                    URL Preview: <span id="preview"></span>
                </div>
            </div>

            <div class="form-group row">
                <div id="btnarea" class="col-sm-6 col-sm-offset-3">
                    <input class="btn btn-success" id="savebtn" value="Submit" type="submit"/>
                    <a href="/FlyLab/Admin/Images" class="btn btn-default">Back</a>
                </div>
            </div>
        </form>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        label {
            position: relative;
            font-size: 14px;
        }
        input.check-box {
            position: relative;
            top: .5em;
        }
        .btn {
            text-align: right;
        }
        .field-validation-error {
            color:indianred;
        }
    </style>
    <script>
        $(function () {
            $('select').addClass('form-control');
            $('select#FirstCat').children('option[value="' + $('input#FirstCat').val().trim() + '"]')
                .attr('selected', '');
            $('select#SecCat').children('option[value="' + $('input#SecCat').val().trim() + '"]')
                .attr('selected', '');

            updatePreview();

            function updatePreview() {
                var prefix = $('#Prefix').val(),
                    fCat = $('select#FirstCat').val(),
                    sCat = $('select#SecCat').val(),
                    suffix = $('#Suffix').val(),
                    formatted = '';

                formatted = formatted + prefix + '[' + fCat + ']';

                if (sCat != "") {
                    formatted = formatted + '[' + sCat + ']';
                }

                formatted = formatted + suffix;

                $('#preview').text(formatted);
            }

            $('.form-control').change(function () {
                updatePreview();
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
