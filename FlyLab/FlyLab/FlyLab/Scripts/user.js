/// <reference path="knockout-3.2.0.js" />
/// <reference path="jquery-1.11.1.min.js" />

$(function () {
    $('#instancetable').DataTable({
        "scrollY": "400px",
        "scrollCollapse": true,
        "paging": false,
        "dom": 't<"sBuffer"f>',
        "order": [[1, "desc"]],
        "columnDefs": [
            {
                "targets": [0, 1],
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "User has no lab sessions on record.",
            "sSearch": "",
            "sSearchPlaceholder": "Search Records..."
        }
    });

    $('.sessionrow').click(function () {
        var $this = $(this),
            rows = $('#instancebody').children('tr.sessionrow');
        
        if (!$this.hasClass('picked')) {
            fullInfo($this.attr('id'));
            for (var i = 0; i < rows.length; i++) {
                $(rows[i]).removeClass('picked');
            }
            $this.addClass('picked');
        } else {
            $this.removeClass('picked');
            $('.panel-body').html('<i>Click a session on the right to display detailed information</i>');
        }
    });

    $('.recordsbtn').click(function () {
        var $this = $(this);
        if ($this.attr('id').toLowerCase() === "delbtn") {
            $this.removeClass('btn-warning')
                .addClass('btn-danger')
                .attr('onclick', 'clearRecords(' + $('.userIDField').attr('id') + ')')
                .attr('id', 'confirmbtn')
                .html('Are you sure?');
        } else if ($this.attr('id').toLowerCase() === "confirmbtn") {
            $this.html('Processing...')
                .attr('disabled', '');
        }
    });

    $('.userbtn').click(function () {
        var $this = $(this);
        if ($this.attr('id').toLowerCase() === "userdelbtn") {
            $this.removeClass('btn-warning')
                .addClass('btn-danger')
                .attr('onclick', 'removeUser(' + $('.userIDField').attr('id') + ')')
                .attr('id', 'userconfirmbtn')
                .html('Are you sure?');
        } else if ($this.attr('id').toLowerCase() === "userconfirmbtn") {
            $this.html('Processing...')
                .attr('disabled', '');
        }
    });
});

var fullInfo = function (id) {
    $.ajax({
        datatype: 'json',
        url: '/FlyLab/Admin/SessionInfo',
        type: 'post',
        data: {
            id: id
        },
        complete: function (o) {
            if (o.success == false) {
                $('.panel-body').html('There was a problem retrieving information from the server. Please refresh your browser.');
            } else {
                $('.panel-body').html(o.responseText);
            }
        }
    });
}

var clearRecords = function (id) {
    $.ajax({
        datatype: 'json',
        url: '/FlyLab/Admin/ClearRecords',
        type: 'post',
        data: {
            id: id
        },
        complete: function (o) {
            if (o.success == false) {
                window.location.assign('/FlyLab/Error/InternalError');
            } else {
                window.location.reload();
            }
        }
    });
}

var removeUser = function (id) {
    $.ajax({
        datatype: 'json',
        url: '/FlyLab/Admin/RemoveUser',
        type: 'post',
        data: {
            id: id
        },
        complete: function (o) {
            if (o.success == false) {
                window.location.assign('/FlyLab/Error/InternalError');
            } else {
                window.location.assign('/FlyLab/Admin');
            }
        }
    });
}
