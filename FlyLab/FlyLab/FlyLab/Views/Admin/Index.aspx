<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FlyLab.Models.LabUserViewModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <!-- Modal -->
    <div class="modal fade" id="usermodal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            
        </div>
    </div>

    <div id="labusercontainer" class="row col-sm-10 col-sm-offset-1">
        <div id="tablearea">
            <h2 class="adminheader">Lab Users</h2>
            <table class="table table-hover table-bordered" id="usertable">
                <thead>
                    <tr>
                        <th>
                            User
                        </th>
                        <th>
                            Last Lab Information
                        </th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <%foreach (var item in Model)
                      { %>
                        <tr>
                            <td>
                                <%: Html.DisplayFor(model => item.User.Name) %><br />
                                <%: Html.DisplayFor(model => item.User.GID) %><br />
                                Labs Completed: <%: Html.DisplayFor(model => item.LabsCompleted) %>
                            </td>
                            <td>
                                Module:<span class="rightside"><%: Html.DisplayFor(model => item.Module.ModuleName) %></span><br />
                                Started:<span class="rightside"><%: Html.DisplayFor(model => item.LastStart) %></span><br />
                                Finished:<span class="rightside"><%: Html.DisplayFor(model => item.LastFinish) %></span>
                            </td>
                            <td>
                                <a href="#" class="btn btn-xs btn-default btn-labinfo verticalcenter" title="View full lab details for <%= item.User.GID %>" id="<%=item.User.Id %>"><i class="glyphicon glyphicon-eye-open"></i></a>
                            </td>
                        </tr>
                    <%} %>
                </tbody>
            </table>
        </div>

        <div class="alert alert-danger alert-dismissable confirm-warning buffer" role="alert">
            Purging the database will hide all active records. This function is meant for use at the end of the semester.
        </div>
        <%if ( Model.ToList().Count > 0)
          { %>
        <div id="controls">
            <button type="button" id="purgebutton" data-loading-text="Confirming..." class="btn btn-danger" style="float:left">Purge Records</button>
            <div id="confirmarea" style="left:4px;position:relative;">
                <div id="confirmbutton" class="btn btn-primary">Confirm Purge</div>
                <div id="cancelbutton" class="btn btn-default">Cancel Purge</div>
            </div>
        </div>
        <%} %>
        
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.css" />
    <script src="//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js"></script>
    <script src="//cdn.datatables.net/plug-ins/a5734b29083/integration/bootstrap/3/dataTables.bootstrap.js"></script>
    <style>
        #useless {
            visibility: hidden;
        }
        .adminheader {
            border-bottom: solid 1px lightgrey;
            text-align: center;
            padding-bottom: .5em;
        }

        .divider {
            border-bottom: 1px lightgrey solid;
        }

        .rightside {
            float: right;
        }

        .leftside {
            float: left;
        }

        .labcell {
            padding-bottom: 5px;
            margin-bottom: 5px;
            border-bottom: solid 1px lightgrey;
        }

        .modaldisplay {
            width: 60%;
            margin: 0 auto;
        }

        .verticalcenter {
            margin-top: 60%;
        }

        .buffer {
            margin-top: 1em;
        }

        .dataTables_info {
            padding-bottom: .5em;
        }

        .dataTables_scrollHeadInner {
            visibility: hidden;
        }

        ul li {
            background: none;
            padding: 0;
        }

        
    </style>

    <script>
        var count = 3;
        $(function () {
            $('#confirmarea').hide();
            $('.alert').hide();

            $('#usertable').DataTable({
                searching: true,
                "order": [[2,"none"]],
                "columnDefs": [
                    {
                        "targets": [0, 1, 2],
                        "bSortable": false
                    },
                    {
                        "targets": [2],
                        "width": 1
                    }
                ]
            });

            $('.btn-labinfo').click(function (e) {
                e.preventDefault();

                var id = $(this).attr('id').trim();
                $.ajax({
                    datatype: "json",
                    url: "/FlyLab/Admin/ModalUserInfo",
                    type: "post",
                    data: {
                        id: id
                    },
                    complete: function (o) {
                        var modal = o.responseText;
                        $('div.modal-dialog').html(modal);
                        $('#usermodal').modal();
                    }
                });
            });

            $('#purgebutton').click(function (e) {
                var btn = $(this);
                btn.button('loading');
                $('.alert').show('slow');
                $("#confirmarea").show('slow');
            });

            $('#cancelbutton').click(function () {
                $('#purgebutton').button('reset');
                $('#confirmarea').hide('slow');
                $('.alert').hide('slow');
            });

            $('#confirmbutton').click(function () {
                $.post('/FlyLab/Admin/PurgeUsers', {}, function (o) {
                    if (o.success) {
                        $('#tablearea').hide();
                        $('#controls').hide();
                        $('.alert').removeClass('alert-danger').addClass('alert-success').html(o.msg);
                        $('#num').text(count);
                        var counter = setInterval(timer, 1000)
                    } else {
                        $('.alert').html(o.errormsg);
                    }
                }, 'json')
            });

            $('.alert').on('hide', function () {
                $('#sidebar').click();
            });
        });
        var timer = function () {
            $('#num').text(count-1);
            count = count - 1;
            if (count <= 0) {
                window.location.assign('/FlyLab/Admin');
            }
        };

        (function ($) {
            $.each(['show', 'hide'], function (i, ev) {
                var el = $.fn[ev];
                $.fn[ev] = function () {
                    this.trigger(ev);
                    return el.apply(this, arguments);
                };
            });
        })(jQuery);
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
