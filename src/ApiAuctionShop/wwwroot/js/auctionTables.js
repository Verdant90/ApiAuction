﻿
// All auctions table init
$('#activeAuctionsTab').click(function () {
    if ($.fn.dataTable.isDataTable('#allAuctions')) {
        $('#allAuctions').DataTable().ajax.reload(null, false);
    }
});


// Archived
$('#archieveAuctionsTab').click(function () {
    if (!$.fn.dataTable.isDataTable('#archievedAuctions')) {
        $('#archievedAuctions').DataTable({
            "language": (window.res.language == "pl") ? polish : english,
            searching: true,
            ordering: true,
            paging: true,
            processing: false,
            serverSide: true,
            orderMulti: false,
            order: [[2, "asc"]],      //default sort
            "ajax": {
                url: "/api/APIAuctions/ended",
                type: "GET",
                dataType: 'json',
            },
            "columnDefs": [
                    { className: "auctionTitle", "targets": [0] },
                    { className: "auctionPrice", "targets": [1] }
            ],
            columns: [
               { data: 'title' },
               { data: 'currentPrice' },
               { data: 'endDate' },
               { data: 'bidCount' },
               { data: 'state' },
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var imagestr = "<div class='col-md-3'><img class='miniatureAuctionList img-thumbnail img-responsive center-block' src='" + aData['imageData'] + "'></div> "
                var titlestr = "<div class='col-md-9 auctionTitleAuthor'><b class='col-xs-12 auctionTitle'><a href='" + aData['url'] + "'>" + aData['title'] + "</a></b><div class='col-xs-12 auctionAuthor'>("+window.res.author + ": " + aData['signupEmail'] + ")</div></div>"
                var title = "<div class='row auctionTitleRow'>" + imagestr + titlestr + "</div>";
                var bidCount = aData['bidCount'] ? aData['bidCount'] : '-';
                var winnerEmail = aData['winnerEmail'] ? aData['winnerEmail'] : '-';

                $('td:eq(0)', nRow).html(title);
                $('td:eq(1)', nRow).html(formatter.format(aData['currentPrice']));
                $('td:eq(2)', nRow).html(aData['endDate']);
                $('td:eq(3)', nRow).html(bidCount);
                $('td:eq(4)', nRow).html(winnerEmail);
            }
        });
    } else {
        $('#archievedAuctions').DataTable().ajax.reload(null, false);
    }
});
//$(document).ready(function () {
//    $('#watchedAuctions').DataTable({
//        "order": [[2, "asc"]]
//    });
//});


// My auctions table init
$(document).ready(function () {
    if (!$.fn.dataTable.isDataTable('#allAuctions')) {
        $('#allAuctions').DataTable({
            "language": (window.res.language == "pl") ? polish : english,
            searching: true,
            ordering: true,
            paging: true,
            processing: false,
            serverSide: true,
            orderMulti: false,
            order: [[2, "asc"]],      //default sort
            "ajax": {
                url: "/api/APIAuctions",
                type: "GET",
                dataType: 'json',
            },
            "columnDefs": [
                    { className: "auctionTitle", "targets": [0] },
                    { className: "auctionPrice", "targets": [1] }
            ],
            columns: [
               { data: 'title' },
               { data: 'currentPrice' },
               { data: 'endDate' },
               { data: 'bidCount' },
               { data: 'state' },
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var imagestr = "<div class='col-md-3'><img class='miniatureAuctionList img-thumbnail img-responsive center-block' src='" + aData['imageData'] + "'></div> "
                var titlestr = "<div class='col-md-9 auctionTitleAuthor'><b class='col-xs-12 auctionTitle'><a href='" + aData['url'] + "'>" + aData['title'] + "</a></b><div class='col-xs-12 auctionAuthor'>("+window.res.author + ": " + aData['signupEmail'] + ")</div></div>"
                var title = "<div class='row auctionTitleRow'>" + imagestr + titlestr + "</div>";
                var bidCount = aData['bidCount'] ? aData['bidCount'] : '-';
                var starState = aData['isWatched'] ? 'active' : 'inactive';
                var star = "<img onclick='toggleAuctionWatch(this.id, " + aData['id'] + ")' id='star-all-auctions-" + aData['id'] + "' class='star-" + starState + "' src='/images/star-" + starState + ".png' title='"+window.res.startWatching+"'>";
                var timeLeft = (aData['timeLeft']['howManyLeft'] !== -1) ? '(' + aData['timeLeft']['howManyLeft'] + ' ' + aData['timeLeft']['timeMeasure'] + ' ' + window.res.left + ')' : '('+window.res.auctionEnded+')';

                $('td:eq(0)', nRow).html(title);
                $('td:eq(1)', nRow).html(formatter.format(aData['currentPrice']));
                $('td:eq(2)', nRow).html(aData['endDate'] + '<div class="timeLeft">' + timeLeft + '</div>');
                $('td:eq(3)', nRow).html(bidCount);
                $('td:eq(4)', nRow).html(star);
            }
        });
    } else {
        $('#allAuctions').DataTable().ajax.reload(null, false);
    }

    if (!$.fn.dataTable.isDataTable('#myAuctions')) {
        var oTable = $('#myAuctions').DataTable({
            "language": (window.res.language == "pl") ? polish : english,
            searching: true,
            ordering: true,
            paging: true,
            processing: false,
            serverSide: true,
            orderMulti: false,
            order: [[2, "asc"]],      //default sort
            "ajax": {
                url: "/api/APIAuctions/mine",
                type: "GET",
                dataType: 'json',
            },
            "columnDefs": [
                    { className: "auctionTitle", "targets": [0] },
                    { className: "auctionPrice", "targets": [1] }
            ],
            columns: [
               { data: 'title' },
               { data: 'currentPrice' },
               { data: 'endDate' },
               { data: 'bidCount' },
               { data: 'state' },
               { data: 'state' }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var imagestr = "<div class='col-md-3'><img class='miniatureAuctionList img-thumbnail img-responsive center-block' src='" + aData['imageData'] + "'></div> "
                var titlestr = "<div class='col-md-9 auctionTitleAuthor'><b class='col-xs-12 auctionTitle'><a href='" + aData['url'] + "'>" + aData['title'] + "</a></b><div class='col-xs-12 auctionAuthor'></div></div>"
                var title = "<div class='row auctionTitleRow'>" + imagestr + titlestr + "</div>";
                var bidCount = aData['bidCount'] ? aData['bidCount'] : '-';
                var state = " <div title='State:  " + aData['state'] + "'><img src='/images/auction_" + aData['state'] + ".png'></div>";
                var actions = (aData['editable'] && aData['state'] === 'active') ? "<input type='button' class='btn btn-xs btn-default' value='Zakończ' onclick='window.location.href='/pl-PL/Auction/End/" + aData['id'] + "''> " : "";
                var timeLeft = (aData['timeLeft']['howManyLeft'] !== -1) ? '(' + aData['timeLeft']['howManyLeft'] + ' ' + aData['timeLeft']['timeMeasure'] + ' ' + window.res.left + ')' : '(' + window.res.auctionEnded + ')';

                $('td:eq(0)', nRow).html(title);
                $('td:eq(1)', nRow).html(formatter.format(aData['currentPrice']));
                $('td:eq(2)', nRow).html(aData['endDate'] + '<div class="timeLeft">' + timeLeft + '</div>');
                $('td:eq(3)', nRow).html(bidCount);
                $('td:eq(4)', nRow).html(state);
                $('td:eq(5)', nRow).html(actions);
            }
        });
    }
});

$('#myAuctionsTab').click(function () {
    if ($.fn.dataTable.isDataTable('#myAuctions')) {
        $('#myAuctions').DataTable().ajax.reload(null, false);
    }
});

// Watched auctions table init
$('#watchedAuctionsTab').click(function () {
    if (!$.fn.dataTable.isDataTable('#watchedAuctionsTable')) {
        $('#watchedAuctionsTable').DataTable({
            "language": (window.res.language == "pl")? polish:english,
            searching: true,
            ordering: true,
            paging: true,
            processing: false,
            serverSide: true,
            orderMulti: false,
            order: [[2, "asc"]],      //default sort
            "ajax": {
                url: "/api/APIAuctions/mine/watched",
                type: "GET",
                dataType: 'json',
            },
            "columnDefs": [
                    { className: "auctionTitle", "targets": [0] },
                    { className: "auctionPrice", "targets": [1] }
            ],
            columns: [
               { data: 'title' },
               { data: 'currentPrice' },
               { data: 'endDate' },
               { data: 'bidCount' },
               { data: 'isWatched' },
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var imagestr = "<div class='col-md-3'><img class='miniatureAuctionList img-thumbnail img-responsive center-block' src='" + aData['imageData'] + "'></div> "
                var titlestr = "<div class='col-md-9 auctionTitleAuthor'><b class='col-xs-12 auctionTitle'><a href='" + aData['url'] + "'>" + aData['title'] + "</a></b><div class='col-xs-12 auctionAuthor'>(Autor: " + aData['signupEmail'] + ")</div></div>"
                var title = "<div class='row auctionTitleRow'>" + imagestr + titlestr + "</div>";
                var bidCount = aData['bidCount'] ? aData['bidCount'] : '-';
                var starState = aData['isWatched'] ? 'active' : 'inactive';
                var star = "<img onclick='toggleAuctionWatch(this.id, " + aData['id'] + ")' id='star-" + aData['id'] + "' class='star-" + starState + "' src='/images/star-" + starState + ".png' title='" + window.res.startWatching + "'> ";
                var timeLeft = (aData['timeLeft']['howManyLeft'] !== -1) ? '(' + aData['timeLeft']['howManyLeft'] + ' ' + aData['timeLeft']['timeMeasure'] + ' ' + window.res.left + ')' : '(' + window.res.auctionEnded + ')';

                $('td:eq(0)', nRow).html(title);
                $('td:eq(1)', nRow).html(formatter.format(aData['currentPrice']));
                $('td:eq(2)', nRow).html(aData['endDate'] + '<div class="timeLeft">' + timeLeft + '</div>');
                $('td:eq(3)', nRow).html(bidCount);
                $('td:eq(4)', nRow).html(star);
            }
        });
    } else {
        $('#watchedAuctionsTable').DataTable().ajax.reload(null, false);
    }
});

// Currency formeter for PLN
var formatter = new Intl.NumberFormat('pl-PL', {
    style: 'currency',
    currency: 'PLN',
    minimumFractionDigits: 2,
});




// auction watching function
function toggleAuctionWatch(id, idAuction) {
    var target = document.getElementById(id);
    if (target.src === location.origin + "/images/star-inactive.png") {
        $('#' + target.id).attr("src", "../../images/star-active.png");
        $('#' + target.id).addClass('star-active');
        $('#' + target.id).removeClass('star-inactive');
        var AuctionsUsersWatching = {
            AuctionId: idAuction,
            UserId: ''
        };
        $.ajax({
            url: '/api/APIAuctionsUsersWatchings',
            type: 'POST',
            data: JSON.stringify(AuctionsUsersWatching),
            dataType: "json",
            contentType: 'application/json',
            statusCode: {
                201: handle201,
                401: handle401,
                409: handle409
            }
        });

    }
    else {
        $('#' + target.id).attr("src", "../../images/star-inactive.png");
        $('#' + target.id).addClass('star-inactive');
        $('#' + target.id).removeClass('star-active');
        $.ajax({
            url: '/api/APIAuctionsUsersWatchings/' + idAuction,
            type: 'DELETE',
            dataType: "json",
            contentType: 'application/json',
            statusCode: {
                200: handle200,
                401: handle401,
                409: handle409
            }
        });

    }

}
var handle200 = function (data, textStatus, jqXHR) {
    $('#myalert').append("<div class='alert alert-warning sticky-alert'> <a class='close' data-dismiss='alert' href='#'>×</a>" + window.res.unWatched + ". </div>");
    $('#myalert').children('.alert-warning').click(function () {
        $(this).remove();
    });
    $("#myalert").children('.alert-warning').delay(4000).fadeOut(500, function () {
        $(this).remove();
    });
};
var handle401 = function (data, textStatus, jqXHR) {
    $('#myalert').append("<div class='alert alert-danger sticky-alert'> <a class='close' data-dismiss='alert' href='#'>×</a> " + window.res.noPermission + ". </div>");
    $('#myalert').children('.alert-danger').click(function () {
        $(this).remove();
    });
    $("#myalert").children('.alert-danger').delay(4000).fadeOut(500, function () {
        $(this).remove();
    });
};
var handle201 = function (data, textStatus, jqXHR) {
    $('#myalert').append("<div class='alert alert-success sticky-alert'> <a class='close' data-dismiss='alert' href='#'>×</a>" + window.res.watched + ". </div>");
    $('#myalert').children('.alert-success').click(function () {
        $(this).remove();
    });
    $("#myalert").children('.alert-success').delay(4000).fadeOut(500, function () {
        $(this).remove();
    });
};
var handle409 = function (data, textStatus, jqXHR) {
    $('#myalert').append("<div class='alert alert-danger sticky-alert'> <a class='close' data-dismiss='alert' href='#'>×</a> " + window.res.databaseError + ".</div>");
    $('#myalert').children('.alert-danger').click(function () {
        $(this).remove();
    });
    $("#myalert").children('.alert-danger').delay(4000).fadeOut(500, function () {
        $(this).remove();
    });
    //console.log(jqXHR);
};


var polish = {
    "processing":     "Przetwarzanie...",
    "search":         "Szukaj:",
    "lengthMenu":     "Pokaż _MENU_ pozycji",
    "info":           "Pozycje od _START_ do _END_ z _TOTAL_ łącznie",
    "infoEmpty":      "Pozycji 0 z 0 dostępnych",
    "infoFiltered":   "(filtrowanie spośród _MAX_ dostępnych pozycji)",
    "infoPostFix":    "",
    "loadingRecords": "Wczytywanie...",
    "zeroRecords":    "Nie znaleziono pasujących pozycji",
    "emptyTable":     "Brak danych",
    "paginate": {
        "first":      "Pierwsza",
        "previous":   "Poprzednia",
        "next":       "Następna",
        "last":       "Ostatnia"
    },
    "aria": {
        "sortAscending": ": aktywuj, by posortować kolumnę rosnąco",
        "sortDescending": ": aktywuj, by posortować kolumnę malejąco"
    }
}
var english = {
    "sEmptyTable": "No data available in table",
    "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
    "sInfoEmpty": "Showing 0 to 0 of 0 entries",
    "sInfoFiltered": "(filtered from _MAX_ total entries)",
    "sInfoPostFix": "",
    "sInfoThousands": ",",
    "sLengthMenu": "Show _MENU_ entries",
    "sLoadingRecords": "Loading...",
    "sProcessing": "Processing...",
    "sSearch": "Search:",
    "sZeroRecords": "No matching records found",
    "oPaginate": {
        "sFirst": "First",
        "sLast": "Last",
        "sNext": "Next",
        "sPrevious": "Previous"
    },
    "oAria": {
        "sSortAscending": ": activate to sort column ascending",
        "sSortDescending": ": activate to sort column descending"
    }
}