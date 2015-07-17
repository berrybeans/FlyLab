function LabViewModel(catList) {
    //need flies to start
    var self = this;
    init = function () {
        init_parents = function () {
            self.F1Father = ko.observable(new Fly({
                Gender: "Male",
                Categories: catList.map(function (c) {
                    return new Category(c);
                }),
                Id: 1,
                Frequency: 1,
                Expected: 1
            }));
            self.F1Mother = ko.observable(new Fly({
                Gender: "Female",
                Categories: catList.map(function (c) {
                    return new Category(c);
                }),
                Id: 2,
                Frequency: 1,
                Expected: 1
            }));
            self.F2Father = ko.observable(new Fly({
                Gender: "Male",
                Categories: catList.map(function (c) {
                    return new Category(c);
                })
            }));
            self.F2Mother = ko.observable(new Fly({
                Gender: "Female",
                Categories: catList.map(function (c) {
                    return new Category(c);
                })
            }));
        };
        init_restrictions = function () {
            self.restrictedCategories = ko.computed(function () {
                var ret = {count: 0};        
                ko.utils.arrayForEach(self.F1Father().Mutants(), function (mut) {
                    ret[mut.CatName] = true;
                    ret.count++;
                });
                ko.utils.arrayForEach(self.F1Mother().Mutants(), function (mut) {
                    ret[mut.CatName] = true;
                    ret.count++;
                });

                return ret;
            }, self);
            self.traitLimit = (module_id === 3) ? 2 : 3;
            self.mappingRestrictor = ko.computed(function () {
                if (module_id > 2) {
                    if (self.F1Father().Mutants().length == 0 && self.F1Mother().Mutants().length != 0) {
                        return "Male";
                    } else if (self.F1Mother().Mutants().length == 0 && self.F1Father().Mutants().length != 0) {
                        return "Female";
                    }
                }
                return "None";
            })
        };
        init_environment = function () {
            self.Generation = ko.observable(1);
            self.Offspring = ko.observable(10000);
            self.F2List = ko.observableArray([]);
            self.Working = ko.observable(false);
            self.F1Options = ko.observable({
                Males: ko.observableArray([]),
                Females: ko.observableArray([])
            });
            self.Categories = catList.map(function (c) {
                return new Category(c);
            });
            self.Results = ko.observableArray([
                {
                    id: 'F0',
                    results: ko.observableArray([self.F1Father(), self.F1Mother()]),
                    parents: false
                },
                {
                    id: 'F1',
                    results: ko.observableArray([]),
                    parents: false
                },
                {
                    id: 'F2',
                    results: ko.observableArray([]),
                    parents: ko.observable({
                        mother: self.F2Mother(),
                        father: self.F2Father()
                    })
                }
            ]);
            self.ResultsViewDetails = ko.observable({});
            self.ResultList = ko.computed(function () {
                var ret = [];
                $.each(self.Results(), function (i, r) {
                    $.each(r.results(), function (i, f) {
                        ret.push(f);
                    });
                });
                return ret;
            }, self);
            self.LabComplete = ko.computed(function () {
                return self.ResultList().filter(function (d) { return !d.MathFinal() }).length == 0 && self.Generation() == 3;
            }, self)
            self.LastPress = -1;

            $('#jsreqwarning').addClass('hidden');
            $('#labarea').removeClass('hidden');
        };

        init_parents();
        init_restrictions();
        init_environment();
    }.call(this)
    self.Reset = function () {
        self.Generation(1);
        self.F1Father().Reset();
        self.F1Mother().Reset();
        self.F2Father().Reset();
        self.F2Mother().Reset();
        self.F1Options = ko.observable({ Males: ko.observableArray([]), Females: ko.observableArray([]) });
        self.F2List = ko.observableArray([]);
        self.Results = ko.observableArray([
            {
                id: 'F0',
                results: ko.observableArray([self.F1Father(), self.F1Mother()]),
                parents: false
            },
            {
                id: 'F1',
                results: ko.observableArray([]),
                parents: false
            },
            {
                id: 'F2',
                results: ko.observableArray([]),
                parents: ko.observable({
                    mother: self.F2Mother(),
                    father: self.F2Father()
                })
            }
        ]);
        self.ResultList = ko.computed(function () {
            var ret = [];
            $.each(self.Results(), function (i, r) {
                $.each(r.results(), function (i, f) {
                    ret.push(f);
                });
            });
            return ret;
        }, self);
        self.ResultsViewDetails = ko.observable({});
        self.Offspring(10000);
        self.LastPress = -1;
    }
    self.Submit = function () {
        //set working flag
        self.Working(true);
        //format trait strings to send
        var fatherTraits = (self.Generation() === 1) ?
            ko.toJS(self.F1Father().Categories().map(function (c) { return c.Selected() })) :
            ko.toJS(self.F2Father().Categories().map(function (c) { return c.Selected() }));
        var motherTraits = (self.Generation() === 1) ?
            ko.toJS(self.F1Mother().Categories().map(function (c) { return c.Selected() })) :
            ko.toJS(self.F2Mother().Categories().map(function (c) { return c.Selected() }));
        var traitArr = [];
        var genderArr = ["Male", "Female"];
        traitArr.push(fatherTraits);
        traitArr.push(motherTraits);

        var ajaxObject = {
            p_generation    : self.Generation,
            p_module        : module_id,
            p_offspring     : self.Offspring,
            p_gender        : JSON.stringify( genderArr ),
            p_traits        : JSON.stringify( traitArr )
        };

        $.post('/FlyLab/Lab/ModuleHub', ajaxObject, function (response, textStatus, jqXHR) {
            t = JSON.parse(response.flies);

            var categories = JSON.parse(response.catList);
            var JSONflies = JSON.parse(response.flies);

            if (self.Generation() == 1) {
                self.F1Options().Males.push(self.F1Father());
                self.F1Options().Females.push(self.F1Mother());
                
                $.each(JSONflies, function (i, f) {
                    f.Categories = catList.map(function (c) {
                        return new Category(c);
                    });
                    f.Id = 3 + i;
                    var adding = new Fly(f);
                    if (f.Gender == "Male") {
                        self.F1Options().Males.push(adding);
                    } else {
                        self.F1Options().Females.push(adding);
                    }
                    self.Results()[1].results.push(adding)
                });

                self.F2Father(self.F1Options().Males()[1]);
                self.F2Mother(self.F1Options().Females()[(module_id > 2) ? 0 : 1]);
                self.Generation(self.Generation() + 1);
            } else if (self.Generation() == 2) {
                JSONflies.shuffle();
                $.each(JSONflies, function (i, f) {
                    f.Categories = catList.map(function (c) {
                        return new Category(c);
                    });
                    f.Id = 5 + i;
                    self.Results()[2].results.push(new Fly(f));
                });
                self.ResultsViewDetails(self.Results()[0].results()[0]);
                self.Results()[2].parents().father = self.F2Father();
                self.Results()[2].parents().mother = self.F2Mother();

                //screen fix for details bar
                self.Generation(self.Generation() + 1);
                if ($(window).scrollTop() >= $('#results-panel').offset().top - 10) {
                    $('#results-details').css('width', $('#results-details').outerWidth() + 'px').css('position', 'fixed').css('top', '10px');
                }
            }
            self.LastPress = -1;
            self.Working(false);
        }).fail(function (response, textStatus, jqXHR) {
            alertify.error("Server Error 500: Please refresh the page and try again.")
            self.Working(false);
        });
    }
    self.SelectF2Parent = function (fly) {
        var path = "F2Father",
            altpath = "F2Mother",
            dist_path = "Males",
            alt_dist_path = "Females",
            temp = '',
            male = true;
        if (fly.Gender == "Female") {
            temp = path;
            path = altpath;
            altpath = temp;

            temp = dist_path;
            dist_path = alt_dist_path;
            alt_dist_path = temp;
            male = false;
        }
        self[path](fly);

        if (module_id > 2) {
            var index = -1;
            $.each(self.F1Options()[dist_path](), function (i, f) {
                if (f.Id == fly.Id) {
                    index = i;
                }
            });
            self[altpath](self.F1Options()[alt_dist_path]()[Math.abs(index - 1)]);
        }
    }
    self.SelectResultView = function (fly) {
        self.ResultsViewDetails(fly);
    }
    self.MoveResultViewer = function (direction) {
        var index = -1;
        $.each(self.ResultList(), function (i, r) {
            if (r.Id == self.ResultsViewDetails().Id) {      
                if (direction == "down") {
                    index = (i < self.ResultList().length - 1) ? i + 1 : i;
                } else {
                    index = (i > 0) ? i - 1 : i;
                }
            }
        });
        self.ResultsViewDetails(self.ResultList()[index]);
    }
    self.SubmitLab = function () {
        alertify.log("Not set up yet")
    }

    //jquery handlers
    var ar = new Array(38, 40);
    $(document).keydown(function (e) {
        var key = e.which;
        //console.log(key);
        //if(key==35 || key == 36 || key == 37 || key == 39)
        if ($.inArray(key, ar) > -1 && self.Generation() == 3) {
            if (key == 38) {
                self.MoveResultViewer('up');
            } else {
                self.MoveResultViewer('down');
            }
        }
        if (key == '13') {
            if (self.Generation() < 3) {
                self.Submit();
            } else {
                if (key == self.LastPress) {
                    self.Reset();
                } else {
                    alertify.log("Press enter again to restart the lab.");
                }
            }
        }
        self.LastPress = key;
        return true;
    });
}
