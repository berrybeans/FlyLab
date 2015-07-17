function Trait(t) {
    var self = this;
    if (t == null) {
        self.Id = 0;
        self.Name = null;
        self.CategoryId = 0;
        self.Distance = 0;
        self.IsLethal = null;
        self.IsDominant = null;
        self.IsIncompleteDominant = null;
        self.IsHeterozygous = false;
        self.ChromosomeNumber = 0;
        self.ImagePath = null;
    } else if (!(t instanceof Trait)) { //came from ajax
        self.Id = t.Id;
        self.Name = t.Name;
        self.CategoryId = t.CategoryId;
        self.Distance = t.Distance;
        self.IsLethal = t.IsLethal;
        self.IsDominant = t.IsDominant;
        self.IsIncompleteDominant = t.IsIncompleteDominant;
        self.IsHeterozygous = t.IsHeterozygous;
        self.ChromosomeNumber = t.ChromosomeNumber;
        self.ImagePath = t.ImagePath;
    }

    self.RealImagePath = ko.computed(function () {
        if (self.IsHeterozygous && !self.IsDominant) {
            return "Wild";
        } else if (self.IsHeterozygous && self.IsIncompleteDominant) {
            return "Light" + self.ImagePath;
        } else {
            return self.ImagePath;
        }
    });

    self.DisplayName = ko.computed(function () {
        if (self.IsHeterozygous && !self.IsDominant) {
            return "Wild";
        } else if (self.IsHeterozygous && self.IsIncompleteDominant) {
            return "Light " + self.Name;
        } else {
            return self.Name;
        }
    });
}