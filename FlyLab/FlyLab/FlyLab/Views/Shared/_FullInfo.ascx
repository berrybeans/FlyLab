<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FlyLab.Models.LabUserViewModel>" %>

<div class="modal-content">
    <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
        <h4 class="modal-title">Detail View for <%:Html.DisplayFor(model => model.User.GID) %> <% if (Model.User.GID.ToLower() == "daa0006block") { %>
                                                      ( ͡° ͜ʖ ͡°)
                                               <% } %></h4>
    </div>
    <div class="modal-body">
        <div class="row">
            <div class="col-sm-5 col-sm-offset-1">
                <div class="labcell">
                    <br />
                    <%: Html.DisplayFor(model => model.User.Name) %> (<%: Html.DisplayFor(model => model.User.GID) %>)<br />
                    Labs Completed: <span class="rightside"><%: Html.DisplayFor(model => model.LabsCompleted) %></span><br />
                    Total Attempts: <span class="rightside"><%= Model.User.UseInstances.Count(t => t.Stage.ToLower() == "start"  && (t.Active ?? true)) %></span>
                </div>
                <div>
                    Last Session Info<br />
                    &nbsp - Browser: <span class="rightside"><%= Model.lastInstance.Browser ?? "N/A" %></span><br />
                    &nbsp - OS: <span class="rightside"><%= Model.lastInstance.OS ?? "N/A" %></span><br />
                    &nbsp - IP Address: <span class="rightside"><%= Model.lastInstance.IP ?? "N/A" %></span>
                </div>
                <div style="width:80%;margin:0 auto;padding-top:6em;">
                    <a href="/FlyLab/Admin/FullUserInfo/<%=Model.User.Id %>" class="btn btn-info" title="View additional information about <%= Model.User.GID %>'s labs" style="width:100%">Full User Details</a>
                </div>
            </div>
            <div class="col-sm-6" style="border-left:1px dashed lightgrey">
                <table class="table" id="instancetable">
                    <thead id="useless">

                    </thead>
                    <tbody>
                    <%foreach (var log in Model.User.UseInstances)
                      {  if (log.Active ?? true) {%>
                    
                        <tr class="useinstance">
                            <td>
                                <%: Html.DisplayFor(model => log.Module.ModuleName) %><br />
                                <%: Html.DisplayFor(model => log.Stage) %>  
                            </td>
                            <td>
                                <span class="rightside">
                                    <%= log.Time.ToShortDateString() %><br />
                                    <%= log.Time.ToLongTimeString() %>
                                </span>
                            </td>
                        </tr>
                    <%  }
                      } %>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-lg btn-default" data-dismiss="modal">Close</button>
    </div>
</div>

<script>
    $('#instancetable').DataTable({
        "scrollY": "200px",
        "scrollCollapse": true,
        "paging": false,
        "dom": 't<"buffer"f>',
        "order": [[1, "desc"]],
        "columnDefs": [
            {
                "targets": [1]
            }
        ],
        "oLanguage": {
            "sEmptyTable": "User has no lab sessions on record."
        }
    });
</script>
