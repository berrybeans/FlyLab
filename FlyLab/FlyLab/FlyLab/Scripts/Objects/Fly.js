function Fly(f, generation) {
    var self = this;
    if (f == null) {
        self.Id = 0;
        self.Gender = null;
        self.Categories = ko.observableArray([]);
        self.Generation = 1;
        self.Frequency = 1;
        self.Expected = 1;
    } else if (!(f instanceof Fly)) { //came from ajax
        self.Id = f.Id;
        self.Gender = f.Gender;
        self.Categories = ko.observable(f.Categories);
        self.Generation = generation;
        self.Frequency = f.Frequency;
        self.Expected = f.Expected;

        if (f.Traits !== undefined) {
            $.each(f.Traits, function (i, trait) {
                var c = _.find(self.Categories(), function (c) { return c.Id == trait.CategoryId; });
                c.Selected(new Trait(trait));
            });
        }
    }

    self.MathString = ko.observable('');
    self.MathCompute = function () {
        try {
            if (self.MathString() != "" && mathjs.eval(self.MathString()) == self.ChiSquared) {
                self.MathFinal(true);
                alertify.success("Good job!");
            } else {
                alertify.error("Try again!");
            }
        } catch (e) {
            alertify.error("Try again with valid input!");
        }
    };
    self.MathDisplay = ko.computed(function () {
        var ret = 'Invalid'
        try {
            ret = mathjs.eval(self.MathString());
        } catch (e) { }
        
        ret = (ret != undefined) ? ('' + ret).slice(0, 8) : 'Type something!';

        return ret;
    }, self);
    self.MathFinal = ko.observable(false);
    self.Mutants = ko.computed(function () {
        return ko.utils.arrayFilter(self.Categories(), function (cat) {
            return cat.Selected().Name != 'Wild';
        });
    }, self);
    self.imageURLs = ko.computed(function () { 
        var ret = [{ path: '../../Assets/Parts/new.png' }];
        if (self.Gender != null) {
            ret.push({ path: '../../Assets/Parts/sex' + self.Gender + '.png' });
            $.each(imgLib, function (i, d) {
                var firstCat = ko.utils.arrayFirst(self.Categories(), function (c1) {
                    return (c1.CatName == d.FirstCat);
                }).Selected().RealImagePath();
                var secCat = (d.SecCat === '') ? '' : ko.utils.arrayFirst(self.Categories(), function (c2) {
                    return c2.CatName == d.SecCat;
                }).Selected().RealImagePath();
                ret.push({path: d.Prefix + firstCat + secCat + d.Suffix});
            });
        }
        return ret;
    }, self);
    self.ChiSquared = Math.pow((self.Frequency - self.Expected), 2) / self.Expected;
    self.Reset = function () {
        ko.utils.arrayForEach(self.Categories(), function (c) {
            c.Selected(c.Wild());
        })
    };
}