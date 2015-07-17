function Category(c) {
    var self = this;
    if (c == null) {
        self.Id = 0;
        self.CatName = null;
        self.Traits = ko.observableArray([]);
        self.Selected = ko.observable(new Trait());
    } else if (!(c instanceof Category)) { //came from ajax
        self.Id = c.Id;
        self.CatName = c.CatName;
        self.Traits = ko.observableArray(c.Traits.filter(function (t) {
            return module_id !== 1 || (!t.IsIncompleteDominant && !t.IsLethal && t.ChromosomeNumber !== 1);
        }).map(function (t) {
            return new Trait(t);
        }));
        self.Selected = ko.observable(new Trait(c.Traits.filter(function (t) { return t.Name == "Wild" })[0]));
    }

    self.Wild = ko.computed(function () {
        return ko.utils.arrayFirst(self.Traits(), function (t) { return t.Name == "Wild"; });
    }, self);
}