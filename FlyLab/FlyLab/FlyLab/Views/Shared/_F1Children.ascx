<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FlyLab.Models.FlyViewModel>>" %>

<div id="f1table" class="col-sm-6 col-sm-offset-3">
    <table class="table table-hover">
        <thead>
            <tr>
                <th><%: Html.DisplayName("Frequency") %></th>
                <th><%: Html.DisplayName("Sex") %></th>
                <%foreach (var trait in Model.ElementAt(0).Fly.Traits)
                  { %>
                      <th><%: Html.DisplayFor(model => trait.Category.CatName) %></th>
                <%} %>
            </tr>
        </thead>
        <tbody> 
            <%foreach (var item in Model)
              { %>
                  <tr>
                      <td class="f1flyhead"><%: Html.DisplayFor(model => item.Frequency) %></td>
                      <td><%: Html.DisplayFor(model => item.Fly.Gender.GenderName) %></td>
                      <%foreach (var trait in item.Fly.Traits)
                        { %>
                            <%if (!trait.IsDominant && (trait.IsHeterozygous ?? false))
                              { %>
                                  <td>Wild</td>
                            <%}
                              else
                              { %>
                                  <td><%: Html.DisplayFor(model => trait.Name) %></td>
                            <%} %>
                      <%} %>
                  </tr>
            <%} %>
        </tbody>
    </table>
</div>
