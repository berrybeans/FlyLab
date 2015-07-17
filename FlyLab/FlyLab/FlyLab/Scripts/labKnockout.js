/// <reference path="knockout-3.2.0.js" />
/// <reference path="jquery-1.11.1.min.js" />
var root = '';

$(function () {
    var globalModule = $('#Module_Call_id').val();
    var tList = [];
    var cList = [];
    var nonWilds = [];
    var imgDictionary = JSON.parse($('#imgDictionary').text());
    setupTable();

    function setupTable() {
        $.post('/FlyLab/Lab/GetLists', {}, function (o) {
            var traits = JSON.parse(o.traits),
                cats = JSON.parse(o.categories);
            tList = traits;
            cList = cats;
        }, 'json');
    };

    function UrlExists(url) {
        var http = new XMLHttpRequest();
        http.open('HEAD', url, false);
        http.send();
        return http.status != 404;
    }

    var Trait = function (id, name, chrom, dist, dom, pardom, lethal, het, imgpath) {
        //fields
        var self = this;
        self.ID = id;
        self.tname = name;
        self.chromonumber = chrom;
        self.chromodistance = dist;
        self.isDominant = dom;
        self.isPartialDominant = pardom;
        self.isLethal = lethal;
        self.isHeterozygous = het;
        self.image = imgpath;

        self.expressed = ko.computed(function () {
            if (!self.isDominant && self.isHeterozygous) {
                return false;
            }
            return true;
        }, self);

        //behaviours
        //the gottaSubscribe variable is here so we can observe the values of everything on this trait
        self.gottaSubscribe = ko.observable(false);
        self.isTraitDisabled = ko.computed(function () {
            if ((self.isLethal || self.chromonumber === 1 || self.isPartialDominant) && globalModule === "1") {
                return true;
            }
            return false;
        }, self);
        self.dispName = ko.computed(function () {
            return self.tname;
        }, self);
    };

    var Category = function (id, name) {
        //fields
        var self = this;
        self.ID = id;
        self.cname = ko.observable(name);
        self.traits = ko.observableArray([]);
        self.default = ko.observable('');
        self.selected = ko.observable('');
        self.nonWild = ko.observable(false);

        //behaviours
        self.isSelected = ko.computed(function () {
            if (self.selected().tname != "Wild") {
                return true;
            }
            return false;
        }, self);
        self.isDisabled = ko.observable(false);
        self.disableOption = function (option, item) {
            if (item.isTraitDisabled()) {
                $(option).remove();
            }
        }
    };

    var Fly = function (id, gender, generation) {
        var self = this;
        self.gender = gender;
        self.ID = id;
        self.freq = ko.observable(1);
        self.imgScale = ko.observable(100);
        self.generation = generation;
        self.categoryLists = ko.observableArray([]);
        self.numSelected = ko.computed(function () {
            var count = 0;
            for (var i = 0; i < self.categoryLists().length; i++) {
                if (self.categoryLists()[i].isSelected()) {
                    count++;
                }
            }
            return count;
        }, self);
        self.imageHTML = ko.computed(function () {
            var adjuster = 100.0;
            if (self.gender.toLowerCase() == "male") {
                adjuster = 105.0;
            }

            var height = 200 * (self.imgScale() / adjuster),
                width = 222 * (self.imgScale() / adjuster);

            var retHTML = '<img height="' + height + '" width="' + width + '" src="../../Assets/Parts/new.png">';
            var imgURLS = [];
            //imgDictionary is populated at the beginning of this file
            if (imgDictionary.length > 0) {
                for (var i in imgDictionary) {
                    var curImg = imgDictionary[i],
                        curURL = '';

                    for (var j in self.categoryLists()) {
                        var curCat = self.categoryLists()[j],
                            curTrt = curCat.selected();

                        if (curCat.cname().toLowerCase() == curImg.FirstCat.toLowerCase()) {
                            curURL = curTrt.image + curURL;
                        }

                        if (curCat.cname().toLowerCase() == curImg.SecCat.toLowerCase()) {
                            curURL = curURL + curTrt.image;
                        }
                    }
                    curURL = curImg.Prefix + curURL + curImg.Suffix;
                    imgURLS[i] = curURL;
                }
                for (var m in imgURLS) {
                    retHTML = retHTML + '<img height="' + height + '" width="' + width + '" src="' + imgURLS[m] + '">';
                    if (imgDictionary[m].FirstCat.toLowerCase() === "body color") {
                        retHTML = retHTML + '<img height="' + height + '" width="' + width + '" src="../../Assets/Parts/sex' + self.gender + '.png">';
                    }
                }
            }
            return retHTML;
        }, self);

        //prepopulate category/trait lists for a new fly
        if (generation === 0) {
            $.post("/FlyLab/Lab/GetLists",
               {},
                function (o) {
                    var clist = JSON.parse(o.categories),
                        tlist = JSON.parse(o.traits);

                    for (var i = 0; i < clist.length; i++) {
                        curArr = clist[i];
                        curCat = new Category(curArr.Id, curArr.CatName);

                        for (var j = 0; j < curArr.Traits.length; j++) {
                            curTrait = new Trait(
                                curArr.Traits[j].Id,
                                curArr.Traits[j].Name,
                                curArr.Traits[j].ChromosomeNumber,
                                curArr.Traits[j].Distance,
                                curArr.Traits[j].IsDominant,
                                curArr.Traits[j].IsIncompleteDominant,
                                curArr.Traits[j].IsLethal,
                                false,
                                curArr.Traits[j].ImagePath
                                );

                            if (curTrait.tname === "Wild") {
                                curCat.default(curTrait);
                                curCat.selected(curTrait);

                            }
                            //add category slots to image areas
                            curCat.traits.push(curTrait);
                        }

                        self.categoryLists.push(curCat);
                    }

                    self.readyFlag = true;
                }, "json");
        }
    };

    function labViewModel(traitlimit) {
        //raw vars
        var osMax = 1000000,
            self = this;
        //data
        self.resetFlag = ko.observable(false);
        self.imgFixer = ko.computed(function () {
            if (self.resetFlag()) {
                for (var r in self.flyList()) {
                    self.flyList()[r].imgScale(60);
                }
            }
        }, self)
        self.finalStage = ko.observable(false);
        self.workingFlag = ko.observable(false);
        self.limit = traitlimit;
        self.flyList = ko.observableArray([
            new Fly(0, 'Male', 0),
            new Fly(1, 'Female', 0)
        ]);
        self.masterCatList = ko.observableArray([]);
        //populate the list
        $.post("/FlyLab/Lab/GetLists",
               {}, function (o) {
                   var clist = JSON.parse(o.categories);
                   for (var i = 0; i < clist.length; i++) {
                       curArr = clist[i];
                       curCat = new Category(curArr.Id, curArr.CatName);
                       self.masterCatList.push(curCat);
                   }
               }, "json");

        //f0/f1 stuff
        self.offspringF1 = ko.observable(osMax);
        self.totalSelected = ko.computed(function () {
            return self.flyList()[0].numSelected() + self.flyList()[1].numSelected();
        }, self);

        //f1/f2 stuff
        self.offspringF2 = ko.observable(osMax);
        self.f1Father = ko.observable(self.flyList()[0]);
        self.f1Mother = ko.observable(self.flyList()[1]);

        //utility behaviours
        self.resetPage = function () {
            var fly1 = self.flyList()[0],
                fly2 = self.flyList()[1];

            for (var i = 0; i < fly1.categoryLists().length; i++) {
                fly1.categoryLists()[i].selected(fly1.categoryLists()[i].default());
                fly2.categoryLists()[i].selected(fly2.categoryLists()[i].default());

                fly1.categoryLists()[i].nonWild(false);
                fly2.categoryLists()[i].nonWild(false);
                self.masterCatList()[i].nonWild(false);
            }

            while (self.flyList().length > 2) {
                self.flyList().pop();
            }

            for (var r in self.flyList()) {
                self.flyList()[r].imgScale(100);
            }
            self.finalStage(false);
            self.toggleTables(false);

            nonWilds = [];
        };
        
        self.toggleTables = function (mated) {
            //any jquery cleanup between states goes here
            self.resetFlag(mated);
        }

        //F1 behaviours
        self.submitF1Page = function () {
            if (self.offspringF1() > osMax) {
                self.offspringF1(osMax);
            } else if (self.offspringF1() < 100) {
                self.offspringF1(100);
            }
            self.offspringF2(self.offspringF1())
            var fly1 = self.flyList()[0].categoryLists(),
                fly2 = self.flyList()[1].categoryLists(),
                arrFly1 = [],
                arrFly2 = [],
                genderArr = [self.flyList()[0].gender, self.flyList()[1].gender],
                flyArr = [];

            //build a formatted string
            var addedIndex = 0;

            for (var i = 0; i < fly1.length; i++) {
                arrFly1[i] = {
                    Id: fly1[i].selected().ID,
                    Name: fly1[i].selected().tname,
                    isHeterozygous: fly1[i].selected().isHeterozygous,
                    ChromosomeNumber: fly1[i].selected().chromonumber
                };
                arrFly2[i] = {
                    Id: fly2[i].selected().ID,
                    Name: fly2[i].selected().tname,
                    isHeterozygous: fly2[i].selected().isHeterozygous,
                    ChromosomeNumber: fly2[i].selected().chromonumber
                };

                if (fly1[i].selected().tname.toLowerCase() != 'wild' || fly2[i].selected().tname.toLowerCase() != 'wild') {
                    fly1[i].nonWild(true);
                    fly2[i].nonWild(true);
                    self.masterCatList()[i].nonWild(true);
                    nonWilds[addedIndex++] = self.masterCatList()[i].cname();
                }
            }

            flyArr = [arrFly1, arrFly2];
            self.workingFlag(true);

            //parse up these flies and send them in for saving
            $.ajax({
                datatype: 'json',
                url: '/FlyLab/Lab/ModuleHub',
                type: 'post',
                data: {
                    p_generation: 1,
                    p_module: globalModule,
                    p_offspring: self.offspringF1(),
                    p_gender: JSON.stringify(genderArr),
                    p_traitStr: JSON.stringify(flyArr)
                },
                complete: function (o) {
                    //console.log(o.responseJSON);
                    if (o.responseJSON.success === undefined) {
                        var flies = JSON.parse(o.responseJSON.flies);

                        //we need new flies for these new flies
                        for (var i in flies) {
                            var currentTable = $('#f1tbl').html();
                            var fly = flies[i];
                            var listIndex = parseInt(i) + 2;
                            var koFly = new Fly(self.flyList().length, fly.Gender, 1);

                            for (var j in fly.Traits) {
                                var curTrait = fly.Traits[j];
                                for (var k in tList) {
                                    if (tList[k].Id == curTrait.Id) {
                                        //we have a hit, find its category in the masterlist and add a category ko Object
                                        for (var m in cList) {
                                            if (cList[m].Id == tList[k].CategoryId) {
                                                var cat = new Category(cList[m].Id, cList[m].CatName),
                                                    name = '',
                                                    img = '';

                                                if ($.inArray(cList[m].CatName, nonWilds) != -1) {
                                                    cat.nonWild(true);
                                                }

                                                if (!tList[k].IsDominant && curTrait.IsHeterozygous) {
                                                    name = 'Wild';
                                                    img = 'Wild';
                                                } else if (tList[k].IsIncompleteDominant && curTrait.IsHeterozygous) {
                                                    name = 'Light ' + tList[k].Name;
                                                    img = 'Light' + tList[k].ImagePath;
                                                } else {
                                                    name = tList[k].Name;
                                                    img = tList[k].ImagePath;
                                                }

                                                cat.selected(new Trait(
                                                    tList[k].Id,
                                                    name,
                                                    tList[k].ChromosomeNumber,
                                                    tList[k].Distance,
                                                    tList[k].IsDominant,
                                                    tList[k].isPartialDominant,
                                                    tList[k].IsLethal,
                                                    curTrait.IsHeterozygous,
                                                    img));
                                                koFly.categoryLists.push(cat);
                                            }
                                        }
                                    }
                                }
                            }
                            koFly.freq(fly.Frequency);
                            self.flyList.push(koFly);
                        }
                        self.workingFlag(false);
                        self.toggleTables(true);
                    } else {
                        window.location.assign('/FlyLab/Error/InternalError');
                    }
                },
                error: function (o) {
                    alert("CRITICAL ERROR: PAGE WILL NOW REFRESH");
                    location.reload();
                }
            });
        };

        //F2 behaviours
        self.submitF2Page = function () {
            if (self.offspringF1() > osMax || self.offspringF1() < 100) {
                self.offspringF1(osMax);
            }
            var fly1 = self.f1Father().categoryLists(),
                fly2 = self.f1Mother().categoryLists(),
                arrFly1 = [],
                arrFly2 = [],
                genderArr = [self.f1Father().gender, self.f1Mother().gender],
                flyArr = [];

            //build a formatted string
            for (var i = 0; i < fly1.length; i++) {
                arrFly1[i] = {
                    Id: fly1[i].selected().ID,
                    Name: fly1[i].selected().tname,
                    isHeterozygous: fly1[i].selected().isHeterozygous,
                    ChromosomeNumber: fly1[i].selected().chromonumber
                };
                arrFly2[i] = {
                    Id: fly2[i].selected().ID,
                    Name: fly2[i].selected().tname,
                    isHeterozygous: fly2[i].selected().isHeterozygous,
                    ChromosomeNumber: fly2[i].selected().chromonumber
                };
            }

            flyArr = [arrFly1, arrFly2];
            self.workingFlag(true);

            //parse up these flies and send them in for saving
            $.ajax({
                datatype: 'json',
                url: '/FlyLab/Lab/ModuleHub',
                type: 'post',
                data: {
                    p_generation: 2,
                    p_module: globalModule,
                    p_offspring: self.offspringF1(),
                    p_gender: JSON.stringify(genderArr),
                    p_traitStr: JSON.stringify(flyArr)
                },
                complete: function (o) {
                    if (o.responseJSON.success === undefined) {
                        var flies = JSON.parse(o.responseJSON.flies);
                        //we need new flies for these new flies
                        for (var i in flies) {
                            var currentTable = $('#f1tbl').html();
                            var fly = flies[i];
                            var listIndex = parseInt(i) + 2;
                            var koFly = new Fly(self.flyList().length, fly.Gender, 2);

                            for (var j in fly.Traits) {
                                var curTrait = fly.Traits[j];
                                for (var k in tList) {
                                    if (tList[k].Id == curTrait.Id) {
                                        //we have a hit, find its category in the masterlist and add a category ko Object
                                        for (var m in cList) {
                                            if (cList[m].Id == tList[k].CategoryId) {
                                                var cat = new Category(cList[m].Id, cList[m].CatName),
                                                    name = '';

                                                if ($.inArray(cList[m].CatName, nonWilds) != -1) {
                                                    cat.nonWild(true);
                                                }

                                                if (!tList[k].IsDominant && curTrait.IsHeterozygous) {
                                                    name = 'Wild';
                                                    img = 'Wild';
                                                } else if (tList[k].IsIncompleteDominant && curTrait.IsHeterozygous) {
                                                    name = 'Light ' + tList[k].Name;
                                                    img = 'Light' + tList[k].ImagePath;
                                                } else {
                                                    name = tList[k].Name;
                                                    img = tList[k].ImagePath;
                                                }

                                                cat.selected(new Trait(
                                                    tList[k].Id, name,
                                                    tList[k].ChromosomeNumber,
                                                    tList[k].Distance,
                                                    tList[k].IsDominant,
                                                    tList[k].isPartialDominant,
                                                    tList[k].IsLethal,
                                                    curTrait.IsHeterozygous,
                                                    img));
                                                koFly.categoryLists.push(cat);
                                            }
                                        }
                                    }
                                }
                            }
                            if (fly.Frequency > 0) {
                                koFly.freq(fly.Frequency);
                                self.flyList.push(koFly);
                            }
                        }
                        self.workingFlag(false);
                        self.finalStage(true);
                    } else {
                        window.location.assign('/FlyLab/Error/InternalError');
                    }
                },
                error: function (o) {
                    alert("CRITICAL ERROR: PAGE WILL NOW REFRESH");
                    location.reload();
                }
            });
            
        };

        self.f1Parent = function (data) {
            //brainstromin
            // 0. make sure global module is 3 or 4
            // 1. check generation
            // 2. check gender
            // 3. pick opposite one
            if (data.gender.toLowerCase() == "male") {
                self.f1Father(data);
            } else {
                self.f1Mother(data);
            }
        };
    }

    ko.bindingHandlers.updateDisabledList = {
        init: function (element, valueAccessor) {},
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            try {
                var value = valueAccessor,
                    i = value().id,
                    fly1 = value().context.flyList()[0];
                    fly2 = value().context.flyList()[1];
                if (fly2.categoryLists().length > 0 && i < fly2.categoryLists().length) {
                    fly2.categoryLists()[i].isDisabled(value().picked != "Wild" && fly2.categoryLists()[i].selected().tname == "Wild");
                    fly1.categoryLists()[i].isDisabled(value().picked != "Wild" && fly1.categoryLists()[i].selected().tname == "Wild");
                }
            } catch (e) {
                //only has potential for error if this gets run before flies are fully loaded
                //so is only possible on page start, so just replace url till it works
                window.location.replace(window.location.pathname);
            }
        }
    };

    //apply bindings based on module
    if (globalModule === "3") {
        root = new labViewModel(2);
        ko.applyBindings(root);
    } else {
        root = new labViewModel(3);
        ko.applyBindings(root);
    }
});

var f1Parents = function (flyIndex) {
    root.f1Parents(flyIndex);
};
