<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.LabAreaViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div id="canvas" hidden>
        <div style="width: 100%; text-align: center;">
            <canvas id="canvasRegn" width="600" height="600" style="margin: 0 auto;"></canvas>
            <br />
            <br />
            <a id="faster" class="btn btn-primary" style="">FASTER</a>
        </div>
    </div>
    
    <%: Html.HiddenFor(model => model.Module.Call_id) %>
        <div id="labzone" class="row">
            <h2>Fly Lab<br />
                <small><%: Html.DisplayFor(model => model.Module.ModuleName) %> Cross</small><br />
            </h2>
            <div class="row" id="basegeneration">

                <div class="flycontainer male col-md-4 col-md-offset-1 col-xs-10 col-xs-offset-1">

                    <div class="fly f0male panel panel-default" data-bind="with: flyList()[0], visible: !resetFlag()">
                        <div class="panel-heading"><span data-bind="text: gender"></span> Parent</div>
                        <div class="flypicf0 picarea" data-bind="html: imageHTML()">
                            <img title="WHY DO I HAVE BENEDICT CUMBERBATCH'S FACE WHAT HAVE YOU DONE TO MEEEEE" src="../../Assets/Imgs/cumberfly.png" />

                        </div>
                        <table class="table">
                            <tbody data-bind="foreach: categoryLists()">
                                <tr>
                                    <td>
                                        <label class="control-label col-sm-4"><span data-bind="text: cname"></span></label>
                                        <div class="col-sm-8">
                                            <select data-bind="options: traits,
    value: selected,
    optionsText: 'tname',
    disable: $root.resetFlag() || isDisabled() || (isSelected() == false && $root.totalSelected() == $root.limit),
    updateDisabledList: { id: $index(), picked: $data.selected().tname, context: $root },
    optionsAfterRender: disableOption"
                                                class="form-control">
                                            </select>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="f1male panel panel-default" data-bind="with: f1Father, visible: resetFlag() && !finalStage()">
                        <div class="panel-heading"><span data-bind="text: gender"></span> Parent</div>
                        <div class="row">
                            <div class="col-xs-6 f1tbl">
                                <table class="table">
                                    <tbody data-bind="foreach: categoryLists()" class="f2selected male">
                                        <tr>
                                            <td data-bind="text: cname" class="nowrap"></td>
                                            <td data-bind="text: selected().dispName()" class="nowrap"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="col-xs-6 flypicf1 picarea" data-bind="html: imageHTML()">
                                <img title="EXISTENCE IS PAIN TO THE CUMBERFLY" src="../../Assets/Imgs/cumberfly2.png" />
                            </div>
                        </div>
                    </div>

                </div>

                <div class="col-xs-4 col-xs-offset-4 col-md-2 col-md-offset-0 row" data-bind="css: { mediancontainerf0: !resetFlag(), mediancontainerf1: resetFlag() && !finalStage(), mediancontainerfinal: finalStage() }">

                    <div data-bind="visible: !workingFlag()">
                        <div id="notfinalmedian" data-bind="visible: !finalStage()">
                            <div class="median row">
                                <button class="btn btn-default btnmedian" data-bind="enable: totalSelected() > 0 || resetFlag(), click: resetPage">Reset</button>
                                <button class="btn btn-success btnmedian" title="Mate the F0 generation parents" data-bind="visible: !resetFlag(), click: submitF1Page" id="f1submit">Mate</button>
                                <button class="btn btn-success btnmedian" title="Mate the F1 generation parents" data-bind="visible: resetFlag(), click: submitF2Page" id="f2submit">Mate</button>
                            </div>

                            <div class="median row form-group">
                                <br />
                                <h5 data-bind="visible: !resetFlag()">F1 Offspring</h5>
                                <h5 data-bind="visible: resetFlag()">F2 Offspring</h5>
                                <input type="number" name="f1limit" value="" class="form-control" min="100" data-bind="visible: !resetFlag(), value: offspringF1" />
                                <input type="number" name="f2limit" value="" class="form-control" min="100" data-bind="visible: resetFlag(), value: offspringF2" />
                            </div>
                        </div>
                        <div id="finalmedian" data-bind="visible: finalStage()">
                            <div class="median row">
                                <button class="btn btn-default btnfinalmedian" data-bind="click: resetPage">Reset</button>
                            </div>
                        </div>
                    </div>

                    <div data-bind="visible: workingFlag()">
                        <div class="loadingcontainer median row">
                            <img title="Processing..." src="../../Assets/Imgs/loading.gif" class="loadingpic" />
                        </div>
                    </div>
                </div>

                <div class="flycontainer female col-md-4 col-md-offset-0 col-xs-10 col-xs-offset-1">

                    <div class="fly f0female panel panel-default" data-bind="with: flyList()[1], visible: !resetFlag()">
                        <div class="panel-heading"><span data-bind="text: gender"></span> Parent</div>
                        <div class="flypicf0 picarea" data-bind="html: imageHTML()">
                            <img title="I'M JUST A TEST FLY DON'T HURT ME" src="../../Assets/Imgs/cumberfly.png" />

                        </div>
                        <table class="table">
                            <tbody data-bind="foreach: categoryLists()">
                                <tr>
                                    <td>
                                        <label class="control-label col-sm-4"><span data-bind="text: cname"></span></label>
                                        <div class="col-sm-8">
                                            <select data-bind="options: traits,
    value: selected,
    optionsText: 'tname',
    disable: $root.resetFlag() || isDisabled() || (isSelected() == false && $root.totalSelected() == $root.limit),
    updateDisabledList: { id: $index(), picked: $data.selected().tname, context: $root },
    optionsAfterRender: disableOption"
                                                class="form-control">
                                            </select>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="f1female panel panel-default" data-bind="with: f1Mother, visible: resetFlag() && !finalStage()">
                        <div class="panel-heading"><span data-bind="text: gender"></span> Parent</div>
                        <div class="row">
                            <div class="col-xs-6 f1tbl">
                                <table class="table">
                                    <tbody data-bind="foreach: categoryLists()" class="f2selected male">
                                        <tr>
                                            <td data-bind="text: cname" class="nowrap"></td>
                                            <td data-bind="text: selected().dispName()" class="nowrap"></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="col-xs-6 flypicf1 picarea" data-bind="html: imageHTML()">
                                <img title="NOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO" src="../../Assets/Imgs/cumberfly2.png" />

                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div id="f1generation" class="row" data-bind="visible: resetFlag() && !finalStage()">
                <div class="col-xs-10 col-xs-offset-1 col-md-8 col-md-offset-2">
                    <table id="f1tbl" class="table table-bordered">
                        <thead>
                            <tr data-bind="foreach: masterCatList()">
                                <!-- ko if:$index() == 0 -->
                                <th>Generation</th>
                                <th>Frequency</th>
                                <th>Sex</th>
                                <!-- /ko -->
                                <!-- ko if:nonWild() -->
                                <th data-bind="text: cname"></th>
                                <!-- /ko -->
                            </tr>
                        </thead>
                        <tbody data-bind="foreach: flyList()">
                            <tr data-bind="foreach: categoryLists()">
                                <!-- ko if:$index() == 0 -->
                                <td data-bind="text: $parent.generation"></td>
                                <td data-bind="text: $parent.freq()"></td>
                                <td data-bind="text: $parent.gender"></td>
                                <!-- /ko -->
                                <!-- ko if:nonWild() -->
                                <td data-bind="text: selected().dispName(), css: { selectedcell: selected().tname.toLowerCase() != 'wild' }"></td>
                                <!-- /ko -->
                                <!-- ko if:$index() == $parent.categoryLists().length-1 -->
                                <td class="f1selector">
                                    <a data-bind="visible: $parent.ID != $root.f1Father().ID && $parent.ID != $root.f1Mother().ID, click: $root.f1Parent($parent)"><i class="glyphicon glyphicon-plus"></i></a>
                                    <a data-bind="visible: $parent.ID == $root.f1Father().ID || $parent.ID == $root.f1Mother().ID"><i class="glyphicon glyphicon-ok"></i></a>
                                </td>
                                <!-- /ko -->
                            </tr>
                        </tbody>
                    </table>
                    <div>
                        *Cells in <span class="selectedcell">green</span> are mutant (non-wild) traits.
                    </div>
                </div>
            </div>

            <div id="f2generation" class="row" data-bind="visible: finalStage()">
                <div class="col-xs-10 col-xs-offset-1 col-md-8 col-md-offset-2">
                    <table id="f2tbl" class="table table-bordered">
                        <thead>
                            <tr data-bind="foreach: masterCatList()">
                                <!-- ko if:$index() == 0 -->
                                <th>Generation</th>
                                <th>Frequency</th>
                                <th>Sex</th>
                                <!-- /ko -->
                                <!-- ko if:nonWild() -->
                                <th data-bind="text: cname"></th>
                                <!-- /ko -->
                            </tr>
                        </thead>
                        <tbody data-bind="foreach: flyList()">
                            <tr data-bind="foreach: categoryLists(), css: { topborder: $index() == 4 || $index() == 2 }">
                                <!-- ko if:$index() == 0 -->
                                <td data-bind="text: $parent.generation"></td>
                                <td data-bind="text: $parent.freq()"></td>
                                <td data-bind="text: $parent.gender"></td>
                                <!-- /ko -->
                                <!-- ko if:nonWild() -->
                                <td data-bind="text: selected().dispName(), css: { selectedcell: selected().tname.toLowerCase() != 'wild' }"></td>
                                <!-- /ko -->
                            </tr>
                        </tbody>
                    </table>
                    <div>
                        *Cells in <span class="selectedcell">green</span> are mutant (non-wild) traits.
                    </div>
                </div>
            </div>
        </div>

        <!-- THIS AREA WILL HOLD THE STRING FOR THE IMAGE PROCESSING METHOD IN labKnockout.js -->
        <span id="imgDictionary" style="visibility: hidden"><%: Html.Raw(Model.ImageGuide) %></span>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        
        .flypicf0 {
            width: 90%;
            margin: 0 auto 0 auto;
            text-align: center;
            padding-top: 1em;
            padding-bottom: .5em;
            height: 225px;
        }
        .flypicf1 {
            text-align: center;
            padding-top: 1em;
            padding-bottom: .5em;
        }
        .loadingcontainer {
            width: 99%;
            margin: 0 auto;
        }
        .loadingpic {
            text-align: center;
            height: 90px;
            width: 90px;
        }
        .median {
            margin: 0 auto 0 auto;
            width: 90%;
            white-space: nowrap;
            text-align: center;
        }
        .mediancontainerf0 {
            margin-top: 13.3em;
            padding-left: 0;
        }
        .mediancontainerf1 {
            margin-top: 4.1em;
            padding-left:0;
        }
        .mediancontainerfinal {
            padding-bottom: 1em;
        }
        .btnresetmedian {
            width: 80%;
        }
        .btnmedian {
            width: 49%;
        }
        .btnfinalmedian {
            width: 98%;
        }
        .f1tbl {
            padding-top: .5em;
        }
        .panel-heading {
            text-align: center;
            font-weight: 900;
            font-size: medium;
        }
        h1,h2,h3,h4,h5 {
            text-align:center;
            padding-bottom: .7em;
            margin-bottom: .7em;
        }
        .hidden {
            width: 0;
            height: 0;
            padding: 0;
            margin: 0;
        }
        .selectedcell {
            background-color: darkseagreen;
        }
        .topborder {
            border-top: 5px solid #ddd;
        }

        .flypicf1 img:not(:first-child) {
            position: absolute;
            left: 53.5px;
        }
        .flypicf0 img:not(:first-child) {
            position: absolute;
            right: 87px;
        }

        .nowrap {
            white-space: nowrap;
        }

        .f1selector {
            width: 32px;
        }
    </style>
    <script src="../../Scripts/labKnockout.js"></script>
    <script src="../../Scripts/eastereggs/konami.js"></script>
    <script>
        $(function () {
            var ctx;
            var imgBg;
            var imgDrops;
            var x = 0;
            var y = 0;
            var noOfDrops = 50;
            var fallingDrops = [];
            var reverse = false;

            function drawBackground() {
                ctx.drawImage(imgBg, 0, 0); //Background
            }

            function draw() {
                drawBackground();

                for (var i = 0; i < noOfDrops; i++) {
                    ctx.drawImage(fallingDrops[i].image, fallingDrops[i].x, fallingDrops[i].y); //The rain drop

                    if (!reverse) {
                        fallingDrops[i].y += fallingDrops[i].speed; //Set the falling speed

                        if (fallingDrops[i].y > 600) {  //Repeat the raindrop when it falls out of view
                            fallingDrops[i].y = -25 //Account for the image size
                            fallingDrops[i].x = Math.random() * 600;    //Make it appear randomly along the width    
                        }
                    } else {
                        fallingDrops[i].y -= fallingDrops[i].speed; //Set the falling speed

                        if (fallingDrops[i].y < 0) {  //Repeat the raindrop when it falls out of view
                            fallingDrops[i].y = 625 //Account for the image size
                            fallingDrops[i].x = Math.random() * 600;    //Make it appear randomly along the width    
                        }
                    }
                    

                }
            }

            function setup() {
                var canvas = document.getElementById('canvasRegn');

                if (canvas.getContext) {
                    ctx = canvas.getContext('2d');
                    if (!launched) {
                        imgBg = new Image();
                        imgBg.src = "http://lorempixel.com/600/600/sports/";
                    }
                    setInterval(draw, 36);
                    for (var i = 0; i < noOfDrops; i++) {
                        var fallingDr = new Object();
                        fallingDr["image"] = new Image(100, 100);
                        fallingDr.image.src = '../../Assets/Imgs/basedfly.png';

                        fallingDr["x"] = Math.random() * 600;
                        fallingDr["y"] = Math.random() * 5;
                        fallingDr["speed"] = 3 + Math.random() * 5;
                        fallingDrops.push(fallingDr);
                    }

                }
            }

            var easter_egg = new Konami();
            easter_egg.code = function () {
                start_show();
            }
            easter_egg.load();

            var fly_peep_show = false;
            var launched = false;
            var hits = 0;

            function faster() {
                if (hits < 15) {
                    setup();
                    var modulo = Math.floor((Math.random() * 20) + 1);
                    var character = (++hits % modulo === 0) ? '1' : '!';
                    character = (hits % modulo === 7) ? 'one' : character;
                    character = (hits % modulo === 5) ? 'eleven' : character;
                    $('#faster').text($('#faster').text() + character);
                    if (hits % modulo === 0) {
                        reverse = !reverse;
                        var text = $('#faster').text();
                        $('#faster').text('REVERSE REVERSE').addClass('disabled');
                        setTimeout(function () { $('#faster').text(text).removeClass('disabled'); }, 500);
                    }
                } else {
                    $('#faster').addClass('disabled');
                    $('#faster').text('SHE CAN\'T GO ANY FASTER');
                }
            }

            function start_show() {
                if (fly_peep_show) {
                    $('#labzone').show();
                    $('#canvas').attr('hidden', '');
                } else {
                    $('#labzone').hide();
                    $('#canvas').removeAttr('hidden');
                    if (!launched) {
                        setup();
                        launched = true;
                    }
                }
                fly_peep_show = !fly_peep_show;
            }

            $('#faster').click(function () { faster(); });
        });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="sidebar" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="breadcrumb" runat="server">
    <%if (CWSToolkit.AuthUser.IsGlobalAdmin())
      { %>
        <%if (Model.dev)
          { %>
            <div style="text-align: right">
                <%: Html.ActionLink("Prod Version", "Module", "Lab", new { id = Model.Module.Call_id}, null) %>
            </div>
        <%} else { %>
            <div style="text-align: right">
                <%: Html.ActionLink("Dev Version", "Module", "Lab", new { id = Model.Module.Call_id, dev = true}, null) %>
            </div>
        <%} %>
    <%} %>
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
