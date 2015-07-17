<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.LabUserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div id="usercontainer" class="row col-sm-10 col-sm-offset-1">
        <a href="#" id="<%=Model.User.Id %>" class="userIDField"></a>
        <h4 class="adminheader">Detail View <% if (Model.User.GID.ToLower() == "daa0006block")
                                               { %>
                                                   ( ͡° ͜ʖ ͡°)
                                            <% } %></h4>
        <div id="displayarea" class="row">
            <div class="col-sm-6 leftcol">
                <div class="panel panel-default panel-text">
                    User Information<br />
                    &nbsp - Name: <span class="rightside"><%= Model.User.Name ?? "N/A" %></span><br />
                    &nbsp - GID: <span class="rightside"><%= Model.User.GID ?? "N/A" %></span><br />
                    &nbsp - Attempts: <span class="rightside"><%= Model.User.UseInstances.Count(t => t.Stage.ToLower() == "start"  && (t.Active ?? true)) %></span><br />
                    &nbsp - Completed: <span class="rightside"><%= Model.User.UseInstances.Count(t => t.Stage.ToLower() == "finish"  && (t.Active ?? true)) %></span><br />
                </div>
                <div class="panel panel-info panel-default" id="infopanel">
                    <div class="panel-heading">
                        Selected Session
                    </div>
                    <div class="panel-body">
                        <i>Click a session on the right to display detailed information</i>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="btncontainer">
                        <a href="/FlyLab/Admin" class="btn btn-default userbtn">Return to Index</a>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="btncontainer">
                        <a href="#" class="btn btn-warning userbtn" id="userdelbtn">Remove User</a>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 rightcol panel panel-default">
                <table class="table" id="instancetable">
                    <thead>
                        <tr>
                            <th>
                                Module
                            </th>
                            <th>
                                Date
                            </th>
                        </tr>
                    </thead>
                    <tbody id="instancebody">
                    <% foreach (var item in Model.User.UseInstances)
                       {
                           if (item.Active ?? true)
                           { %>
                               <tr id="<%=item.Id %>" class="sessionrow noselect">
                                   <td>
                                       <%= item.Module.ModuleName %>
                                   </td>
                                   <td>
                                       <%= item.Time %>
                                   </td>
                               </tr>
                        <% }    
                       } %>
                    </tbody>
                </table>
                <% if (Model.User.UseInstances.Count(t => t.Active ?? true) > 0)
                   { %>
                       <a href="#" class="btn btn-sm btn-warning recordsbtn buffer" id="delbtn">Clear Records</a>
                <% } %>
                
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.css" />
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.js"></script>
    <script src="../../Scripts/user.js"></script>
    <style>
        #infopanel {
            min-height: 353.563px;
        }

        .btncontainer {
            margin: 0 auto;
            width: 99%;
        }

        .userbtn {
            width: 100%;
        }

        .adminheader {
            border-bottom: solid 1px lightgrey;
            text-align:center;
            padding-bottom: .5em;
        }

        .rightside {
            float: right;
        }

        .leftside {
            float: left;
        }

        .buffer {
            margin-top: 1em;
        }

        .sBuffer {
            margin-top: 1em;
            width: 25%;
            float: right;
            position: relative;
            right: 25%;
        }

        .panel-text {
            padding: 1em;
        }

        .tblhead {
            font-weight: bold;
        }

        .panel-body {
            padding-bottom: 0;
            min-height: 265px;
        }

        .picked {
            background-color: #f9f9f9;
        }

        .rightcol {
            height: 492px;
        }
        .noselect {
            -webkit-touch-callout: none;
            -webkit-user-select: none;
            -khtml-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
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
