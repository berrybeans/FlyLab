<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FlyLab.Models.Module>" %>

    
    <%: Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>

    <%: Html.HiddenFor(model => model.Id) %>

    <div class="form-group row">
        <label class="col-sm-3 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.ModuleName) %></label>
        <div class="col-sm-6">
            <input type="text" value="<%=Model.ModuleName %>" class="form-control" id="ModuleName" name="ModuleName"/>
        </div>
    </div>

    <div class="form-group row">
        <label class="col-sm-3 col-sm-offset-1 control-label"><%: Html.DisplayNameFor(model => model.Call_id) %></label>
        <div class="col-sm-6">
            <input type="number" value="<%=Model.Call_id %>" class="form-control" id="Call_id" name="Call_id" min="1" max="4" />
        </div>
    </div>
