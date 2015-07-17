<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FlyLab.Models.Trait>>" %>

<table class="table table-hover table-bordered" id="traittable">
    <thead>
        <tr>
            <th>
                <%: Html.DisplayNameFor(model => model.Name) %>
            </th>
            <th>
                <%: Html.DisplayNameFor(model => model.Category) %>
            </th>
            <th>
                <%: Html.DisplayName("Dom") %>
            </th>
            <th>
                <%: Html.DisplayName("Inc Dom") %>
            </th>
            <th>
                <%: Html.DisplayNameFor(model => model.IsLethal) %>
            </th>
            <th>
                <%: Html.DisplayNameFor(model => model.ChromosomeNumber) %>
            </th>
            <th>
                <%: Html.DisplayNameFor(model => model.Distance) %>
            </th>
            <th></th>
        </tr>
    </thead>

    <tbody>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: Html.DisplayFor(modelItem => item.Name) %>
            </td>
            <td>
                <%: Html.DisplayFor(modelItem => item.Category.CatName) %>
            </td>
            <td>
                <%if (item.IsDominant)
                  { %>
                             True 
                        <%}
                  else
                  { %>
                             False
                        <%} %>
            </td>
            <td>
                <%if (item.IsIncompleteDominant)
                  { %>
                             True 
                        <%}
                  else
                  { %>
                             False
                        <%} %>
            </td>
            <td>
                <%if (item.IsLethal)
                  { %>
                             True 
                        <%}
                  else
                  { %>
                             False
                        <%} %>
            </td>
            <td>
                <%: Html.DisplayFor(modelItem => item.ChromosomeNumber) %>
            </td>
            <td>
                <%: Html.DisplayFor(modelItem => item.Distance) %>
            </td>
            <td>
                <a class="btn btn-xs btn-default" href="/FlyLab/Admin/Edit/<%: Html.Raw(item.Id) %>"><i class="glyphicon glyphicon-pencil"></i></a>
                <a class="btn btn-xs btn-danger" href="/FlyLab/Admin/Delete/<%: Html.Raw(item.Id)%>"><i class="glyphicon glyphicon-trash"></i></a>
            </td>
        </tr>
        <% } %>
    </tbody>

</table>



<style>
    ul li {
        background: none;
        padding: 0;
    }

    #traittable td:last-child {
        white-space: nowrap;
    }
</style>
<script>
    $(function () {
        $('#Categories').addClass('form-control');
        $('#traittable').DataTable({
            searching: false,
            "order": [[1, "asc"]],
            "dom": 'tpir',
            /*"scrollX": true,*/
            "columnDefs": [
                {
                    "targets": [7],
                    "bSortable": false,
                    "width": 30
                }
            ]
        });
    });
</script>
