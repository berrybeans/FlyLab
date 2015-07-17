<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FlyLab.Models.UseInstance>" %>
<p>
    <span class="tblhead">Module</span><span class="rightside"><%: Html.DisplayFor(model => model.Module.ModuleName) %></span><br />

    <span class="tblhead"><%: Html.DisplayNameFor(model => model.Stage) %></span><span class="rightside"><%: Html.DisplayFor(model => model.Stage) %></span><br />

    <span class="tblhead"><%: Html.DisplayNameFor(model => model.Browser) %></span><span class="rightside"><%: Html.DisplayFor(model => model.Browser) %></span><br />

    <span class="tblhead"><%: Html.DisplayNameFor(model => model.OS) %></span><span class="rightside"><%: Html.DisplayFor(model => model.OS) %></span><br />

    <span class="tblhead"><%: Html.DisplayNameFor(model => model.IP) %></span><span class="rightside"><%: Html.DisplayFor(model => model.IP) %></span><br /><br />

    <% if (Model.Flies.Count == 0)
       { %>
           <i>There are no flies still associated with this session. This is most likely a result of a category, module, or gender deletion.</i>
    <% } %>


    <% for (int i = 0; i < Model.Flies.Count; i++)
       { %>
         <span class="tblhead">
             Fly <%= i + 1 %>
         </span>
         <span class="rightside">
             <% if (Model.Flies.ElementAt(i).Traits.Count(t => t.Name.ToLower() != "wild") != 0)
                { %>
                    <%foreach (var trait in Model.Flies.ElementAt(i).Traits.Where(t => t.Name.ToLower() != "wild"))
                   { %>
                       &nbsp<%=trait.Name%>
                 <%} %>
             <% }
                else
                { %>
                    Wild-Type
             <% } %>
             
         </span><br />
    <% } %>
</p>
