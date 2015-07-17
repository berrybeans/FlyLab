<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Nested.Master" Inherits="System.Web.Mvc.ViewPage<FlyLab.Models.LabAreaViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="server">
    <div id="titlearea">
        <h3><%: Model.Module.ModuleName %> Cross</h3>
    </div>
    <div id="jsreqwarning">
        <div id="loadercontainer2">
            <div class="loader"></div>
        </div>
        <span style="position: relative; bottom: 100px;">Please wait, the lab is loading...</span>
    </div>
    <div id="labarea" class="row hidden">
        <!-- ko if:Generation() != 3 -->
        <div class="col-xs-10 col-xs-offset-1 col-md-6 col-md-offset-0">
            <!-- FATHER AREA -->
            <div class="panel panel-default">
                <div class="panel-heading" style="text-align: right;">
                    <strong class="panel-title"><span class="pull-left">&#9794;</span><span data-bind="text: (Generation() == 1) ? 'Male Fly' : 'Select Father'"></span></strong>
                </div>
                <!-- ko if:Generation() == 1 -->
                <div class="panel-body" data-bind="with: $root.F1Father">
                    <div class="col-sm-8">
                        <div data-bind="foreach: Categories">
                            <div class="row">
                                <div class="col-sm-5">
                                    <span data-bind="text: CatName"></span>
                                </div>
                                <div class="col-sm-7">
                                    <select data-bind="options: Traits, optionsText: 'Name', value: Selected, attr: { disabled: $parent.Gender == $root.mappingRestrictor() || (Selected().Name == 'Wild' && ($root.restrictedCategories()[CatName] || $root.restrictedCategories().count >= $root.traitLimit)) || $root.Working() }" class="form-control input-sm category"></select>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4 hidden-xs" style="text-align: right">
                        <div data-bind="foreach: imageURLs" class="flypic">
                            <img src="#" data-bind="attr: { src: $data.path, width: 200 }" />
                        </div>
                    </div>
                </div>
                <!-- /ko -->
                <!-- ko if:Generation() == 2 -->
                <div class="panel-body" data-bind="with: $root.F1Options()">
                    <div class="col-sm-8" data-bind="with: $root.F1Options()">
                        <div data-bind="foreach: Males">
                            <div data-bind="click: $root.SelectF2Parent">
                                <div class="col-sm-12 f1choice" data-bind="css: { f1selected: Id == $root.F2Father().Id, 'f1choice-break': $index() == 0 }">
                                    <!-- ko foreach: Categories -->
                                    <div class="col-lg-6" style="padding: 5px 0;" data-bind="css: { 'f1trait-nonwild': Selected().Name != 'Wild' }">
                                        <strong><span data-bind="text: CatName"></span>:</strong>
                                        <span data-bind="text: Selected().DisplayName" class="trait-align"></span> 
                                    </div>
                                    <!-- /ko -->
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4 hidden-xs" data-bind="with: $root.F2Father">
                        <div data-bind="foreach: imageURLs" class="flypic f1fix">
                            <img src="#" data-bind="attr: { src: $data.path, width: 200 }" />
                        </div>
                    </div>
                </div>
                <!-- /ko -->
            </div>
        </div>
        <div class="col-xs-10 col-xs-offset-1 col-md-6 col-md-offset-0">
            <!-- MOTHER AREA -->
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong class="panel-title"><span data-bind="text: (Generation() == 1) ? 'Female Fly' : 'Select Mother'"></span><span class="pull-right">&#9792;</span></strong>
                </div>
                <!-- ko if:Generation() == 1 -->
                <div class="panel-body" data-bind="with: $root.F1Mother">
                    <div class="col-sm-4 hidden-xs">
                        <div data-bind="foreach: imageURLs" class="flypic">
                            <img src="#" data-bind="attr: { src: $data.path, width: 200 }" />
                        </div>
                    </div>
                    <div class="col-sm-8">
                        <div data-bind="foreach: Categories">
                            <div class="row">
                                <div class="col-sm-5">
                                    <span data-bind="text: CatName"></span>
                                </div>
                                <div class="col-sm-7">
                                    <select data-bind="options: Traits, optionsText: 'Name', value: Selected, attr: { disabled: $parent.Gender == $root.mappingRestrictor() || (Selected().Name == 'Wild' && ($root.restrictedCategories()[CatName] || $root.restrictedCategories().count >= $root.traitLimit)) || $root.Working() }" class="form-control input-sm category"></select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- /ko -->
                <!-- ko if:Generation() == 2 -->
                <div class="panel-body">
                    <div class="col-sm-4 hidden-xs" data-bind="with: $root.F2Mother">
                        <div data-bind="foreach: imageURLs" class="flypic f1fix">
                            <img src="#" data-bind="attr: { src: $data.path, width: 200 }" />
                        </div>
                    </div>
                    <div class="col-sm-8" data-bind="with: $root.F1Options()">
                        <div data-bind="foreach: Females">
                            <div data-bind="click: $root.SelectF2Parent">
                                <div class="col-sm-12 f1choice" data-bind="css: { f1selected: Id == $root.F2Mother().Id, 'f1choice-break': $index() == 0 }">
                                    <!-- ko foreach: Categories -->
                                    <div class="col-lg-6" style="padding: 5px 0;" data-bind="css: { 'f1trait-nonwild': Selected().Name != 'Wild' }">
                                        <strong><span data-bind="text: CatName"></span>:</strong>
                                        <span data-bind="text: Selected().DisplayName" class="trait-align"></span> 
                                    </div>
                                    <!-- /ko -->
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- /ko -->
            </div>
        </div>
        <!-- /ko -->
        <!-- ko if:Generation() == 3-->
        <div class="col-xs-10 col-xs-offset-1 col-md-6 col-md-offset-0">
            <div id="results-panel" class="panel panel-default" data-bind="with: $root.Results">
                <div class="panel-heading">
                    <strong class="panel-title">Lab Results</strong>
                </div>
                <div class="panel-body" data-bind="foreach: $root.Results">
                    <h4><span data-bind="text: id"></span> Generation</h4>
                    <!-- ko if:parents -->
                    <div class="parents-panel col-md-10 col-md-offset-1 hidden-sm hidden-xs" data-bind="with: parents" >
                        <div class="result-fly-parent col-md-12">
                            <div data-bind="with:father">
                                <div id="father-small" class="result-small col-md-5" data-bind="css: { 'result-selected': $data.Id == $root.ResultsViewDetails().Id }, click: $root.SelectResultView">
                                    <div class="small-fly-id" data-bind="style: { 'background-color': MathFinal() ? 'rgba(30, 110, 20, .85)' : '' }">
                                        <span data-bind="text: Id"></span>
                                    </div>
                                    <div class="fly-info-short-small">
                                        <span data-bind="text: (Gender == 'Male') ? 'Father': 'Mother'"></span>
                                    </div>
                                    <div data-bind="foreach: imageURLs" class="flypictiny pull-right">
                                        <img src="#" data-bind="attr: { src: $data.path, width: 35 }" />
                                    </div>
                                </div>
                            </div>
                            <div data-bind="with: mother">
                                <div id="mother-small" class="result-small col-md-5 col-md-offset-2" data-bind="css: { 'result-selected': $data.Id == $root.ResultsViewDetails().Id }, click: $root.SelectResultView">
                                    <div class="small-fly-id" data-bind="style: { 'background-color': MathFinal() ? 'rgba(30, 110, 20, .85)' : '' }">
                                        <span data-bind="text: Id"></span>
                                    </div>
                                    <div class="fly-info-short-small">
                                        <span data-bind="text: (Gender == 'Male') ? 'Father' : 'Mother'"></span>
                                    </div>
                                    <div data-bind="foreach: imageURLs" class="flypictiny pull-right">
                                        <img src="#" data-bind="attr: { src: $data.path, width: 35 }" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- /ko -->
                    <div class="result-panel row" data-bind="foreach: results">
                        <div class="result-fly col-md-10 col-md-offset-1" style="padding-left: 0;" data-bind="css: { 'result-selected': $data.Id == $root.ResultsViewDetails().Id }, click: $root.SelectResultView">
                            <div class="fly-id" data-bind="style: { 'background-color': MathFinal() ? 'rgba(30, 110, 20, .85)' : '' }">
                                <span data-bind="text: Id"></span>
                            </div>
                            <div class="fly-info-short">
                                <span style="font-weight: 800;">Gender: </span><span data-bind="text: Gender" class="pull-right"></span><br />
                                <span style="font-weight: 800;">Frequency: </span><span data-bind="text:Frequency" class="pull-right"></span>
                            </div>
                            <div data-bind="foreach: imageURLs" class="flypicsmall pull-right" style="padding:0; height: 50px;">
                                <img src="#" data-bind="attr: { src: $data.path, width: 55 }" />
                            </div>
                        </div>
                        <!-- ko if:$data.Id == $root.ResultsViewDetails().Id -->
                        <div class="visible-sm visible-xs result-fly-sm">
                            <div class="shrinker">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="result-misc">
                                            <strong>Gender:</strong><span data-bind="text: Gender" class="pull-right"></span>
                                            <br />
                                            <strong>Frequency:</strong><span data-bind="text: Frequency" class="pull-right"></span>
                                        </div>
                                        <div class="result-traits-title">
                                            Traits
                                        </div>
                                        <div class="result-traits" data-bind="foreach: Categories">
                                            <strong><span data-bind="text: CatName"></span>:</strong><span data-bind="    text: Selected().DisplayName" class="pull-right"></span>
                                            <br />
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div data-bind="foreach: imageURLs" class="flypic pull-right">
                                            <img src="#" data-bind="attr: { src: $data.path, width: 250 }" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <h4>Chi<sup>2</sup> Test</h4>
                                    </div>
                                    <div class="col-sm-8">
                                        <textarea class="form-control" rows="2" data-bind="textInput: MathString, disable: MathFinal()" placeholder="(real - expected) ^ 2 / expected" style="margin-bottom: 5px;"></textarea>
                                    </div>
                                    <div class="col-sm-4" style="text-align:right">
                                        = <input type="text" disabled style="width: 90%" data-bind="value: MathDisplay, style: { 'background-color': MathDisplay() == 'Invalid' ? 'darkred' : '', 'color': MathDisplay() == 'Invalid' ? 'white' : '' }" />
                                        <button class="btn btn-xs btn-success" style="margin-top: 7px" data-bind="click: MathCompute">Submit</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- /ko -->
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-6 col-md-offset-0 hidden-sm hidden-xs">
            <div id="results-details" class="panel panel-default" data-bind="with: ResultsViewDetails">
                <div class="panel-heading">
                    <strong class="panel-title">Details for Fly <span data-bind="text: Id"></span><span class="pull-right" data-bind="html: (Gender == 'Male') ? '&#9794;' : '&#9792;'"></span></strong>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="result-misc">
                                <strong>Gender:</strong><span data-bind="text: Gender" class="pull-right"></span>
                                <br />
                                <strong>Frequency:</strong><span data-bind="text: Frequency" class="pull-right"></span>
                            </div>
                            <div class="result-traits-title">
                                Traits
                            </div>
                            <div class="result-traits" data-bind="foreach: Categories">
                                <strong><span data-bind="text: CatName"></span>:</strong><span data-bind="text: Selected().DisplayName" class="pull-right"></span>
                                <br />
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div data-bind="foreach: imageURLs" class="flypic pull-right">
                                <img src="#" data-bind="attr: { src: $data.path, width: 300 }" />
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <hr />
                            <h4>Chi<sup>2</sup> Test</h4>
                        </div>
                        <div class="col-md-8">
                            <textarea class="form-control" rows="2" data-bind="textInput: MathString, disable: MathFinal()" placeholder="(real - expected) ^ 2 / expected"></textarea>
                        </div>
                        <div class="col-md-4" style="text-align:right">
                            = <input type="text" disabled style="width: 90%" data-bind="value: MathDisplay, style: { 'background-color': MathDisplay() == 'Invalid' ? 'darkred' : '', 'color': MathDisplay() == 'Invalid' ? 'white' : '' }" />
                            <button class="btn btn-xs btn-success" style="margin-top: 7px" data-bind="click: MathCompute">Submit</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- /ko -->
        <div class="row">
            <div class="col-xs-6 col-xs-offset-3 col-md-2 col-md-offset-5 controls">
                <!-- BUTTON AREA -->
                <div id="fly-control-panel" class="row" data-bind="visible: !Working()">
                    <div class="col-xs-12" style="padding-bottom: 5px;" data-bind="visible: Generation() < 3">
                        <strong>Offspring:</strong>
                        <input data-bind="value: Offspring" type="number" class="form-control"/>
                    </div>
                    <div class="col-xs-12">
                        <div class="col-lg-6" style="padding: 0 0 5px 0">
                            <button data-bind="click: Submit, enable: Generation() < 3, visible: Generation() < 3" class="btn btn-success btn-controls">Mate</button>
                            <button data-bind="click: SubmitLab, enable: LabComplete(), visible: Generation() == 3" class="btn btn-success btn-controls" style="padding-left: 0; padding-right: 0;">Submit Lab</button>
                        </div>
                        <div class="col-lg-6" style="padding: 0">
                            <button data-bind="click: Reset" class="btn btn-default btn-controls">Reset</button>
                        </div>
                    </div>
                </div>             
                <div id="fly-loading-panel" class="row" data-bind="visible: Working()">
                    <div class="col-xs-12">
                        <div id="loadercontainer">
                            <div class="loader"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Styles/lab.css" rel="stylesheet"/>
    <link href="../../Content/Styles/spinner.css" rel="stylesheet" />
    <script src="../../Scripts/Utilities/script_load.js" type="text/javascript"></script>
    <script src="../../Scripts/Utilities/underscore-min.js" type="text/javascript"></script>
    <script>
        var module_id = parseInt('<%: Model.Module.Call_id %>');
        var debug = '<%: Model.debug %>' == 'True';
        var dev = '<%: Model.dev %>' == 'True';
        var imgLib;
        var bindings;
        var catLib;

        $(function () {
            var scrollHit = 0;
            imgLib = $.parseJSON('<%: Model.ImageGuide %>'.split('&quot;').join('"'));
            catLib = $.parseJSON('<%: Model.CatTemplate %>'.split('&quot;').join('"'));
            //init bindings with a dummy ViewModel to fix weird ko bugs on page load
            bindings = ko.observable({
                Reset       : function () { alertify.error('Something has gone wrong. Please refresh the page.'); },
                Submit      : function () { alertify.error('Something has gone wrong. Please refresh the page.'); },
                SubmitLab   : function () { return false; },
                Offspring   : function () { return 1000; },
                Working     : function () { return false; },
                Generation  : function () { return 1; },
                Loaded      : function () { return false; },
                LabComplete : function () { return false; }
            });
            ko.applyBindings(bindings);

            $.script_load({
                script_lib      : [
                                      { path: "ViewModels/LabViewModel.js" },
                                      { path: "Objects/Trait.js" },
                                      { path: "Objects/Category.js" },
                                      { path: "Objects/Fly.js" }
                                  ],
                alertify        : true,
                debug           : debug,
                timeout         : 2.5,
                script_loc      : "../../Scripts/",
                callback        : function () { bindings(new LabViewModel(catLib)); },
                success_msg     : "Lab loaded.",
                fail_msg        : "Lab failed to load."
            });

            if (readCookie('devwarning') != "true" && dev) {
                alertify.alert("This is a development version of the application."
                    + "<br>To view a working version of the application, "
                    + "click <a href=\"/Flylab/lab/module/" + module_id + "\">here</a>.");
                createCookie('devwarning', true, 1);
            }

            $(window).scroll(function () {
                try {
                    var bottom_offset_panel = $(document).height() - $('#results-panel').outerHeight() - $('#results-panel').offset().top;
                    var bottom_offset_details = $(document).height() - $('#results-details').outerHeight() - $('#results-details').offset().top;
                    if (bottom_offset_details <= bottom_offset_panel && scrollHit < $(window).scrollTop()) {
                        scrollHit = $(window).scrollTop();
                        $('#results-details')
                            .css('width', $('#results-details').outerWidth() + 'px')
                            .css('position', 'absolute')
                            .css('top', ($('#results-panel').height() - $('#results-details').height()) + 'px')
                            .css('bottom', '');
                    } else if ($(window).scrollTop() >= $('#results-panel').offset().top - 10) {
                        scrollHit = 0;
                        $('#results-details')
                            .css('width', $('#results-details').outerWidth() + 'px')
                            .css('position', 'fixed')
                            .css('top', '10px')
                            .css('bottom', '');
                    } else {
                        $('#results-details')
                            .css('width', '')
                            .css('position', 'inherit')
                            .css('top', '')
                            .css('bottom', '');
                    }
                } catch (e) {

                }
            });
        });
    </script>
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
