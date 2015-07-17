<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FlyLab.Models.Module>>" %>

<%foreach (var module in Model)
  { %>
<li><%: Html.ActionLink(module.ModuleName, "Module", "Lab", new { id = module.Call_id }, null) %></li>
<%} %>
