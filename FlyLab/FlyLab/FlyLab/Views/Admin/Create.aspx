<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.Trait>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">

<% using (Html.BeginForm("Edit", "Admin"))
   { %>
    <%: Html.AntiForgeryToken()%>
    <%: Html.ValidationSummary(true)%>

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
            Trait
        </legend>

        <%: Html.HiddenFor(model => model.Id) %>

        <div class="form-group row">
            <label for="Name" class="col-sm-2 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.Name) %></label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="Name" name="Name" autofocus/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="Name" name="Name" value="<%: Html.Raw(Model.Name) %>" autofocus/>
                <%} %>

                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
        </div>

        <div class="form-group row">
            <label for="CategoryId" class="col-sm-2 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.Category) %></label>
            <div class="col-sm-6">
                <%: Html.DropDownList("CategoryId", String.Empty)%>
                <%: Html.ValidationMessageFor(model => model.CategoryId)%>
            </div>
        </div>

        <div class="form-group row">
            <label for="IsDominant" class="col-sm-2 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.IsDominant) %>?</label>
            <div class="col-sm-1">
                <%: Html.EditorFor(model => model.IsDominant)%>
                <%: Html.ValidationMessageFor(model => model.IsDominant)%>
            </div>
            <div class="col-sm-8" id="incdom">
                <label for="IsIncompleteDominant" class="col-sm-5 control-label"><%: Html.DisplayNameFor(model => model.IsIncompleteDominant) %>?</label>
                <div class="col-sm-1">
                    <%: Html.EditorFor(model => model.IsIncompleteDominant)%>
                    <%: Html.ValidationMessageFor(model => model.IsIncompleteDominant)%>
                </div>
            </div>
        </div>

        <div class="form-group row">
            <label for="IsLethal" class="col-sm-2 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.IsLethal) %>?</label>
            <div class="col-sm-6">
                <%: Html.EditorFor(model => model.IsLethal)%>
                <%: Html.ValidationMessageFor(model => model.IsLethal)%>
            </div>
        </div>

        <div class="form-group row">
            <label for="ChromosomeNumber" class="col-sm-3 control-label"><%: Html.DisplayNameFor(model => model.ChromosomeNumber) %></label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="ChromosomeNumber" name="ChromosomeNumber"/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="ChromosomeNumber" name="ChromosomeNumber" value="<%: Html.Raw(Model.ChromosomeNumber) %>"/>
                <%} %>

                <%: Html.ValidationMessageFor(model => model.ChromosomeNumber) %>
            </div>
        </div>

        <div class="form-group row">
            <label for="Distance" class="col-sm-3 control-label"><%: Html.DisplayNameFor(model => model.Distance) %></label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="Distance" name="Distance"/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line nonwild" id="Distance" name="Distance" value="<%: Html.Raw(Model.Distance) %>"/>
                <%} %>
                <%: Html.ValidationMessageFor(model => model.Distance) %>
            </div>
        </div>

        <div class="form-group row">
            <label class="col-sm-3 control-label" for="ImagePath"><%: Html.DisplayNameFor(model => model.ImagePath) %></label>
            <div class="col-sm-6">
                <%if (Model == null)
                  { %>
                      <input type="text" class="form-control text-box single-line" id="ImagePath" name="ImagePath"/>
                <%}
                  else
                  { %>
                      <input type="text" class="form-control text-box single-line" id="ImagePath" name="ImagePath" value="<%: Html.Raw(Model.ImagePath) %>"/>
                <%} %>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6 col-sm-offset-3">
                <input class="btn btn-success" type="submit" value="Submit" />
                <a class="btn btn-default" href="/FlyLab/Admin/Traits">Back</a>
            </div>
        </div>
    </fieldset>
<% } %>

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
            if (!document.getElementById('IsDominant').checked) {
                $('#incdom').hide();
            }
            $('form').addClass('form-horizontal');
            $('select').addClass('form-control');

            wildFlag = $('#Name').val().toLowerCase().trim() === 'wild';

            if (wildFlag) {
                $('span[data-valmsg-for="Name"]').addClass('field-validation-error').text("This is a wild trait. The only changes that can be made are to the ImagePath.");
            }

            $('#IsDominant').click(function () {
                if (this.checked) {
                    $('#incdom').show();
                } else {
                    $('#incdom').hide();
                    $('#IsIncompleteDominant').removeAttr('checked');
                }
            });

            $('.btn-success').click(function (e) {
                var name = $('#Name').val().toLowerCase().trim();
                if (name == "wild" && !wildFlag) {
                    e.preventDefault();
                    $('span[data-valmsg-for="Name"]').addClass('field-validation-error').text("Do not create or edit a wild trait here. The wild trait is generated when the category is created.");
                }
            });
        })
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
