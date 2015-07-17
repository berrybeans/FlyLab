using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Objects.SqlClient;
using System.Text.RegularExpressions;
using FlyLab.Models;
using Newtonsoft.Json;
using CWSToolkit;
using Elmah;

namespace FlyLab.Controllers
{
    public class LabController : Controller
    {
        private LabEntityContainer db = new LabEntityContainer();

        struct CrossList
        {
            private CrossTrait[] traits;
            private int count;

            public CrossTrait[] Traits
            {
                get { return traits; }
                set { traits = value; }
            }
            public int Count
            {
                get { return count; }
                set { count = value; }
            }            
        }

        /// <summary>
        /// Takes the module number of the desired cross type and builds the view with ModuleId set to id
        /// </summary>
        /// <param name="id">The module call_id of the desired lab</param>
        /// <param name="dev">Set to true to use DevModule</param>
        /// <param name="debug">Determines whether or not to show alertify notifications</param>
        /// <returns>A view of the lab area</returns>
        public ActionResult Module(int id = 0, bool dev = false, bool debug = false)
        {
            if (!dev)
            {
                try
                {
                    //the entity proxy generator breaks JSON Serialization, so we disable it for the duration of this function
                    db.Configuration.ProxyCreationEnabled = false;

                    string imgList = JsonConvert.SerializeObject(db.ImageSettings.ToArray());
                    List<Category> lcat = db.Category.Include(t => t.Traits).ToList();
                    foreach (var curcat in lcat)
                    {
                        foreach (var curtrt in curcat.Traits)
                        {
                            curtrt.Category = null;
                        }
                    }
                    string catList = JsonConvert.SerializeObject(lcat.ToArray());

                    Module module = db.Module.First(t => t.Call_id == id && t.Active);
                    ImageSettings[] ImageLib = db.ImageSettings.ToArray();
                    LabAreaViewModel lavm = new LabAreaViewModel()
                    {
                        Module = module,
                        ImageLib = ImageLib,
                        ImageGuide = imgList,
                        CatTemplate = catList,
                        debug = debug,
                        dev = false
                    };
                    return View("NewLab", lavm);
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    return RedirectToAction("NotFound", "Error");
                }
            }
            else
            {
                return DevModule(id, debug);
            }
        }

        /// <summary>
        /// If you're gonna mess with anything, do it here if you can. Access this by calling
        /// website.com/path/to/module/{id}?dev=true
        /// </summary>
        /// <param name="id">The call_id of the module to pull up</param>
        /// <param name="debug">Enable/disable debug variable on page</param>
        /// <returns></returns>
        private ActionResult DevModule(int id, bool debug)
        {
            try
            {
                //the entity proxy generator breaks JSON Serialization, so we disable it for the duration of this function
                db.Configuration.ProxyCreationEnabled = false;

                string imgList = JsonConvert.SerializeObject(db.ImageSettings.ToArray());
                List<Category> lcat = db.Category.Include(t => t.Traits).ToList();
                foreach (var curcat in lcat)
                {
                    foreach (var curtrt in curcat.Traits)
                    {
                        curtrt.Category = null;
                    }
                }
                string catList = JsonConvert.SerializeObject(lcat.ToArray());

                Module module = db.Module.First(t => t.Call_id == id && t.Active);
                ImageSettings[] ImageLib = db.ImageSettings.ToArray();
                LabAreaViewModel lavm = new LabAreaViewModel() { 
                    Module = module,
                    ImageLib = ImageLib,
                    ImageGuide = imgList,
                    CatTemplate = catList,
                    debug = debug,
                    dev = true
                };
                return View("NewLab", lavm);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("NotFound", "Error");
            }
        }

        /// <summary>
        /// Serves a full list of traits and categories in JSON format
        /// To parse on the client side, use JSON.parse(response.traits) etc.
        /// Used on the lab page to build UI and keep knockout data consistent with DB
        /// </summary>
        /// <returns>A JSON object with the category and trait lists inside</returns>
        [HttpPost]
        public ActionResult GetLists()
        {
            //the entity proxy generator breaks JSON Serialization, so we disable it for the duration of this function
            db.Configuration.ProxyCreationEnabled = false;

            string traitList = JsonConvert.SerializeObject(db.Trait.ToArray());
            string catList = JsonConvert.SerializeObject(db.Category.OrderBy(t => t.Id).ToArray(),
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        //the self referential loop in Category's model blows JsonConvert's tiny mind, so we ignore it
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            return Json(new { traits = traitList, categories = catList });
        }

        /// <summary>
        /// Parses the client's choices for the Flies' traits into a readable list of new flies
        /// to be styled and turned into Knockout objects on the client's side
        /// </summary>
        /// <param name="p_generation">Marker for the generation to be processed (Valid values are 1 and 2)</param>
        /// <param name="p_module">The Call ID of the module being used</param>
        /// <param name="p_offspring">The total number of flies to produce</param>
        /// <param name="p_gender">A string containing the gender being used. May be "Male" or "Female." Case-insensitive.</param>
        /// <param name="p_traits">Stringified array of fly traits</param>
        /// <returns>New FlyViewModels for the client to parse up into HTML elements.</returns>
        /// 
        [HttpPost]
        public ActionResult ModuleHub(int p_generation, int p_module, int p_offspring, string p_gender, string p_traits)
        {
            //just use json.decode
            //var traitLists = System.Web.Helpers.Json.Decode<int[][]>(p_traitStr);
            ReducedTraitModel[][] traitLists = System.Web.Helpers.Json.Decode<ReducedTraitModel[][]>(p_traits);
            String[] genderList = System.Web.Helpers.Json.Decode<string[]>(p_gender);
            List<FlyViewModel> currentFlies = new List<FlyViewModel>();
            Module module = db.Module.First(t => t.Call_id == p_module);
            //p_offspring = (int)(.5 * p_offspring);
            
            if (module == null)
            {
                InvalidOperationException e = new InvalidOperationException("Someone tried to make a call to a nonexistent module. This exception was generated in LabController/ModuleHub.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { success = false });
            }
            //if we get here successfully we can consider the lab officially started
            UseInstance instance = new UseInstance();
            if (RecordLog(module, p_generation, out instance) > 0)
            {
                return Json(new { success = false });
            }

            //Fly f1 = LogFly(p_module, instance, genderList[0], traitLists[0]);
            //Fly f2 = LogFly(p_module, instance, genderList[1], traitLists[1]);

            List<CrossList> crossResults = new List<CrossList>();
            ReducedTraitModel[] f_arr; //father
            ReducedTraitModel[] m_arr; //mother
            List<ReducedTraitModel> m_arr_dist = new List<ReducedTraitModel>(); //mothers distance linekd traits
            List<ReducedTraitModel> f_arr_dist = new List<ReducedTraitModel>(); //fathers distance linked traits
            if (genderList[0].ToLower() == Constants.Male.ToLower())
            {
                f_arr = traitLists[0];
                m_arr = traitLists[1];
            }
            else
            {
                f_arr = traitLists[1];
                m_arr = traitLists[0];
            }
            //perform split into distance linked traits and non here

            for (int i = 0; i < f_arr.Length; i++)
            {
                var f = f_arr[i]; //father side trait
                var m = m_arr[i]; //mother side trait

                //checking chromosome number
                //all pairs will either be wild or 1 possible trait from that category,
                //so only check the non zero (non wild) chromo number
                int cnum_checking = (m.ChromosomeNumber > 1) ? m.ChromosomeNumber : f.ChromosomeNumber;
                if ((f_arr.Count(t => t.ChromosomeNumber == cnum_checking) 
                        + m_arr.Count(t => t.ChromosomeNumber == cnum_checking) > 1) && 
                     p_module > 2 &&
                     cnum_checking > 1)
                {
                    //cram em in there
                    m_arr_dist.Add(m);
                    f_arr_dist.Add(f);
                }
                else
                {
                    crossResults.Add(BasicCross(f_arr[i], m_arr[i]));
                }
            }

            //RUN THE DISTANCE CROSSER
            if (m_arr_dist.Count == 2)
            {
                //run 2 point cross method
                crossResults.Add(TwoPointCross(f_arr_dist.ToArray(), m_arr_dist.ToArray()));
            }
            if (m_arr_dist.Count == 3)
            {
                //run 3 point cross method
                crossResults.Add(ThreePointCross(f_arr_dist.ToArray(), m_arr_dist.ToArray()));
            }

            List<FlyViewModel> processedFlies = BuildChildren(p_module, p_offspring, p_generation, crossResults.ToArray());
            List<String> catsToDisplay = new List<String>();
            foreach (var fly in processedFlies)
            {
                var nonWilds = fly.Traits.Where(t => !t.Name.ToLower().Equals(Constants.Wild.ToLower()));
                foreach (var trait in nonWilds)
                {
                    String catName = db.Trait.Find(trait.Id).Category.CatName;
                    if (!catsToDisplay.Contains(catName))
                    {
                        catsToDisplay.Add(catName);
                    }
                }
            }

            processedFlies.RemoveAll(t => t.Frequency == 0);

            int offset = (int)(.01 * p_offspring); //initialize maxval to subtract at flylist population time
            Random randomizer = new Random();

            foreach (var adjusting in processedFlies)
            {
                //save expected value for chi^2 test
                adjusting.Expected = adjusting.Frequency;
                if (p_offspring <= 10000)
                {
                    adjusting.Frequency -= randomizer.Next((int)(.04 * adjusting.Frequency));
                }
                else
                {
                    adjusting.Frequency -= randomizer.Next((int)(.08 * adjusting.Frequency));
                }
            }

            string returnFlies = System.Web.Helpers.Json.Encode(processedFlies);
            string catList = System.Web.Helpers.Json.Encode(catsToDisplay);
            
            return Json(new { flies = returnFlies, catList = catList });
        }

        #region CROSS METHODS

        /// <summary>
        /// Returns an array of trait possibilities and their spawn percentages
        /// based on the provided trait pair
        /// </summary>
        /// <param name="p_fatherTrait">Father's trait</param>
        /// <param name="p_motherTrait">Mother's Trait</param>
        /// <returns>The processed list of child possibilities</returns>
        private CrossList BasicCross(ReducedTraitModel p_fatherTrait, ReducedTraitModel p_motherTrait)
        {
            Trait dbTrt1 = db.Trait.Find(p_fatherTrait.Id);
            Trait dbTrt2 = db.Trait.Find(p_motherTrait.Id);
            Trait trait1 = new Trait();
            Trait trait2 = new Trait();

            List<CrossTrait> returnArr = new List<CrossTrait>();
            int count = 0;

            #region Sex_Linked
            if (dbTrt1.ChromosomeNumber == 1 || dbTrt2.ChromosomeNumber == 1)
            {
                #region Sex_Trait_Prep
                //take in traits after detecting that they are sex linked (on chromosome 1)
                //make special sex trait and perform cross
                //parse it in children method
                //??????
                //profit

                //make trait1 heterozygous (Male XY) and trait2 homozygous (Female XX)
                trait1 = new Trait()
                {
                    Id = dbTrt1.Id,
                    Name = Constants.SexLinked,
                    Father = GetTrait(dbTrt1, p_fatherTrait.IsHeterozygous),
                    Mother = GetTrait(dbTrt2, p_motherTrait.IsHeterozygous),
                    IsHeterozygous = true,
                    ChromosomeNumber = 1,
                    CategoryId = dbTrt1.CategoryId
                };
                trait2 = new Trait()
                {
                    Id = dbTrt2.Id,
                    Name = Constants.SexLinked,
                    Father = GetTrait(dbTrt1, p_fatherTrait.IsHeterozygous),
                    Mother = GetTrait(dbTrt2, p_motherTrait.IsHeterozygous),
                    IsHeterozygous = false,
                    ChromosomeNumber = 1,
                    CategoryId = dbTrt2.CategoryId
                };
                
                //do some fancy processing and abuse CrossTrait's new Male and Female bools
                //we know the list contains a sex-linked trait, so find it

                Trait sexLinkedTrait = dbTrt1.ChromosomeNumber == 1 ? trait1 : trait2;

                Trait hetFatherTrait = GetTrait(sexLinkedTrait.Father, true);
                Trait homFatherTrait = GetTrait(sexLinkedTrait.Father, false);
                Trait hetMotherTrait = GetTrait(sexLinkedTrait.Mother, true);
                Trait homMotherTrait = GetTrait(sexLinkedTrait.Mother, false);

                #endregion 

                if (sexLinkedTrait.Mother.ChromosomeNumber == 1 && sexLinkedTrait.Father.ChromosomeNumber == 1)
                {
                    //we know they will be the same trait, so there are only 2 cross cases here...
                    if (sexLinkedTrait.Mother.IsHeterozygous ?? false)
                    {
                        //het - hemi cross
                        Trait wildTrait = db.Trait.First(t => t.CategoryId == sexLinkedTrait.Father.ChromosomeNumber && t.Name.ToLower() == Constants.Wild.ToLower());

                        returnArr.Add(new CrossTrait(homMotherTrait, .5, false, true)); //aff female
                        returnArr.Add(new CrossTrait(hetMotherTrait, .5, false, true)); //carrier female
                        returnArr.Add(new CrossTrait(homMotherTrait, .5, true, false)); //aff male
                        returnArr.Add(new CrossTrait(wildTrait     , .5, true, false)); //unaff male
                        count = 4;
                        return FinishCross(returnArr, count);
                    }
                    else
                    {
                        //hom - hemi cross
                        returnArr.Add(new CrossTrait(homMotherTrait, 1, true, false)); //aff male
                        returnArr.Add(new CrossTrait(homMotherTrait, 1, false, true)); //aff female
                        count = 2;
                        return FinishCross(returnArr, count);
                    }
                }
                if (sexLinkedTrait.Father.ChromosomeNumber == 1)
                {
                    //affected father
                    if (!(sexLinkedTrait.Mother.IsHeterozygous ?? false))
                    {
                        returnArr.Add(new CrossTrait(hetFatherTrait, 1, false, true)); //carrier females
                        returnArr.Add(new CrossTrait(homMotherTrait, 1, true, false)); //unaff males
                        count = 2;
                        return FinishCross(returnArr, count);
                    }
                }
                else
                {
                    //affected mother
                    if (sexLinkedTrait.Mother.IsHeterozygous ?? false)
                    {
                        //hetrec sex linked
                        //1 son wild, 1 son aff, 1 daughter wild, 1 daughter het
                        returnArr.Add(new CrossTrait(hetMotherTrait, .5, false, true)); //carrier female
                        returnArr.Add(new CrossTrait(homFatherTrait, .5, false, true)); //unaff female
                        returnArr.Add(new CrossTrait(homMotherTrait, .5, true, false)); //aff male
                        returnArr.Add(new CrossTrait(homFatherTrait, .5, true, false)); //unaff male
                        count = 4;
                        return FinishCross(returnArr, count);
                    }
                    else
                    {
                        returnArr.Add(new CrossTrait(hetMotherTrait, 1, false, true)); //carrier females
                        returnArr.Add(new CrossTrait(homMotherTrait, 1, true, false)); //aff males
                        count = 2;
                        return FinishCross(returnArr, count);
                    }
                }
            }
            #endregion
            #region Non_Sex_Linked
            else
            {
                #region Trait_Prep
                trait1 = GetTrait(dbTrt1, p_fatherTrait.IsHeterozygous);
                trait2 = GetTrait(dbTrt2, p_motherTrait.IsHeterozygous);
                #endregion
            }

            //het-het
            if ((trait1.IsHeterozygous ?? false) && (trait2.IsHeterozygous ?? false))
            {
                //we know theyre same trait so we return accordingly
                Trait wildTrait = db.Trait.First(t => t.Name.ToLower() == Constants.Wild.ToLower() && t.CategoryId == trait1.CategoryId);

                Trait homTrait = GetTrait(trait1, false);
                Trait hetTrait = GetTrait(trait1, true);

                returnArr.Add(new CrossTrait(homTrait, .25));
                returnArr.Add(new CrossTrait(hetTrait, .5));
                returnArr.Add(new CrossTrait(wildTrait, .25));
                count = 3;
                return FinishCross(returnArr, count);
            }
            //hom-hom
            if (!(trait1.IsHeterozygous ?? false) && !(trait2.IsHeterozygous ?? false))
            {
                if (trait1.Name.ToLower() == Constants.Wild.ToLower() && trait2.Name.ToLower() == Constants.Wild.ToLower())
                {
                    returnArr.Add(new CrossTrait(trait1, 1));
                    count = 1;
                    return FinishCross(returnArr, count);
                }
                //rec-wild
                else if (!trait1.IsDominant && trait2.Name.ToLower() == Constants.Wild.ToLower())
                {
                    Trait expressed = trait1;
                    expressed.IsHeterozygous = true;

                    returnArr.Add(new CrossTrait(expressed, 1));
                    count = 1;
                    return FinishCross(returnArr, count);
                }
                //wild-rec
                else if (!trait2.IsDominant && trait1.Name.ToLower() == Constants.Wild.ToLower())
                {
                    Trait expressed = trait2;
                    expressed.IsHeterozygous = true;

                    returnArr.Add(new CrossTrait(expressed, 1));
                    count = 1;
                    return FinishCross(returnArr, count);
                }
                //dom-rec*
                else if (trait1.IsDominant)
                {
                    Trait expressed = trait1;
                    expressed.IsHeterozygous = true;

                    returnArr.Add(new CrossTrait(expressed, 1));
                    count = 1;
                    return FinishCross(returnArr, count);
                }
                else if (trait2.IsDominant)
                {
                    Trait expressed = trait2;
                    expressed.IsHeterozygous = true;

                    returnArr.Add(new CrossTrait(expressed, 1));
                    count = 1;
                    return FinishCross(returnArr, count);
                }
            }
            //hom-het
            if ((!(trait1.IsHeterozygous ?? false) && (trait2.IsHeterozygous ?? false)) || 
                (!(trait2.IsHeterozygous ?? false) && (trait1.IsHeterozygous ?? false)))
            {
                Trait homTrait = (trait1.IsHeterozygous ?? false) ? trait2 : trait1;
                Trait hetTrait = (trait1.IsHeterozygous ?? false) ? trait1 : trait2;

                returnArr.Add(new CrossTrait(hetTrait, .5));
                returnArr.Add(new CrossTrait(homTrait, .5));
                count = 2;
                return FinishCross(returnArr, count);
            }
            #endregion

            return new CrossList();
        }

        /// <summary>
        /// Returns an aray of trait possibilites and their spawn percentages.
        /// Bad data/data that does not obey the algorithm will cause hilarious (read: annoying)
        /// bugs. Make sure the user is forced to follow the rules rigidly.
        /// </summary>
        /// <param name="p_fatherTraits">Father's traits</param>
        /// <param name="p_motherTraits">Mother's traits</param>
        /// <returns></returns>
        private CrossList TwoPointCross(ReducedTraitModel[] p_fatherTraits, ReducedTraitModel[] p_motherTraits)
        {
            List<CrossTrait> returnArr = new List<CrossTrait>();
            #region Trait_Prep
            //generate wild, het, hom for each trait & gender
            Trait[] wild = new Trait[2];

            Trait[] father_hets = new Trait[2];
            Trait[] father_homs = new Trait[2];

            Trait[] mother_hets = new Trait[2];
            Trait[] mother_homs = new Trait[2];

            Trait[] father_raw = new Trait[2];
            Trait[] mother_raw = new Trait[2];

            for (int i = 0; i < 2; i++)
            {
                Trait dbTrt1 = db.Trait.Find(p_fatherTraits[i].Id);
                Trait dbTrt2 = db.Trait.Find(p_motherTraits[i].Id);

                wild[i] = db.Trait.FirstOrDefault(t => t.CategoryId == dbTrt1.CategoryId && t.Name.ToLower() == Constants.Wild.ToLower());

                //father modified
                father_hets[i] = GetTrait(dbTrt1, true);
                father_homs[i] = GetTrait(dbTrt1, false);

                //mother modified
                mother_hets[i] = GetTrait(dbTrt2, true);
                mother_homs[i] = GetTrait(dbTrt2, false);

                //raw arrays
                father_raw[i] = GetTrait(dbTrt1, p_fatherTraits[i].IsHeterozygous);
                mother_raw[i] = GetTrait(dbTrt2, p_motherTraits[i].IsHeterozygous);
            }
            #endregion
            #region Distance_Linked
            //check father trait, then mother trait to determine case
            List<Trait> building = new List<Trait>();
            if ((!p_fatherTraits[0].IsHeterozygous && !p_fatherTraits[1].IsHeterozygous) &&
                (!p_motherTraits[0].IsHeterozygous && !p_motherTraits[1].IsHeterozygous))
            {
                //father hom-hom, mother hom-hom
                //one output: het-het
                building.Add((father_hets[0].Name != Constants.Wild) ? father_hets[0] : mother_hets[0]);
                building.Add((father_hets[1].Name != Constants.Wild) ? father_hets[1] : mother_hets[1]);
                returnArr.Add(new CrossTrait(building, 1));
                return FinishCross(returnArr, 1);
            }
            else
            {
                //2 traits must be heterozygous by necessity of the algorithm
                //calculate recombinance rates
                double dist1 = (p_fatherTraits[0].Name != Constants.Wild) ? father_raw[0].Distance : mother_raw[0].Distance;
                double dist2 = (p_fatherTraits[1].Name != Constants.Wild) ? father_raw[1].Distance : mother_raw[1].Distance;
                double recombined_rate   = Math.Abs(dist1 - dist2) / 100;
                double unrecombined_rate = 1 - recombined_rate;
                //build all four possibilities 
                building.Add(father_raw[0]);
                building.Add(father_raw[1]); //unrecombined
                returnArr.Add(new CrossTrait(building, unrecombined_rate / 2));
                building = new List<Trait>();
                building.Add(mother_raw[0]);
                building.Add(mother_raw[1]); //unrecombined
                returnArr.Add(new CrossTrait(building, unrecombined_rate / 2));
                building = new List<Trait>();
                building.Add(mother_raw[0]);
                building.Add(father_raw[1]); //recombined
                returnArr.Add(new CrossTrait(building, recombined_rate / 2));
                building = new List<Trait>();
                building.Add(father_raw[0]);
                building.Add(mother_raw[1]); //recombined
                returnArr.Add(new CrossTrait(building, recombined_rate / 2));

                return FinishCross(returnArr, 4);
            }
            #endregion

            //return new CrossList();
        }

        /// <summary>
        /// Returns an aray of trait possibilites and their spawn percentages.
        /// Bad data/data that does not obey the algorithm will cause hilarious (read: annoying)
        /// bugs. Make sure the user is forced to follow the rules rigidly.
        /// </summary>
        /// <param name="p_fatherTraits">Father's traits</param>
        /// <param name="p_motherTraits">Mother's traits</param>
        /// <returns></returns>
        private CrossList ThreePointCross(ReducedTraitModel[] p_fatherTraits, ReducedTraitModel[] p_motherTraits)
        {
            //FIX ORDER
            p_fatherTraits = p_fatherTraits.OrderBy(t => t.Distance).ToArray();
            p_motherTraits = p_motherTraits.OrderBy(t => t.Distance).ToArray();

            List<CrossTrait> returnArr = new List<CrossTrait>();
            #region Trait_Prep
            //generate wild, het, hom for each trait & gender
            Trait[] wild = new Trait[3];
            Trait[] hets = new Trait[3];
            Trait[] homs = new Trait[3];

            Trait[] father_raw = new Trait[3];
            Trait[] mother_raw = new Trait[3];

            for (int i = 0; i < 3; i++)
            {
                Trait dbTrt1 = db.Trait.Find(p_fatherTraits[i].Id);
                Trait dbTrt2 = db.Trait.Find(p_motherTraits[i].Id);

                wild[i] = db.Trait.FirstOrDefault(t => t.CategoryId == dbTrt1.CategoryId && t.Name.ToLower() == Constants.Wild.ToLower());
                hets[i] = GetTrait((dbTrt1.Name != Constants.Wild) ? dbTrt1 : dbTrt2, true);
                homs[i] = GetTrait((dbTrt1.Name != Constants.Wild) ? dbTrt1 : dbTrt2, false);
            }
            #endregion
            #region Distance_Linked
            //check father trait, then mother trait to determine case
            List<Trait> building = new List<Trait>();
            if ((!p_fatherTraits[0].IsHeterozygous && !p_fatherTraits[1].IsHeterozygous && !p_fatherTraits[2].IsHeterozygous) &&
                (!p_motherTraits[0].IsHeterozygous && !p_motherTraits[1].IsHeterozygous && !p_motherTraits[2].IsHeterozygous))
            {
                //father hom-hom, mother hom-hom
                //one output: het-het
                building.Add(hets[0]);
                building.Add(hets[1]);
                building.Add(hets[2]);
                returnArr.Add(new CrossTrait(building, 1));
                return FinishCross(returnArr, 1);
            }
            else
            {
                //2 traits must be heterozygous by necessity of the algorithm
                //calculate recombinance rates
                double dist1 = (p_fatherTraits[0].Name != Constants.Wild) ? p_fatherTraits[0].Distance : p_motherTraits[0].Distance;
                double dist2 = (p_fatherTraits[1].Name != Constants.Wild) ? p_fatherTraits[1].Distance : p_motherTraits[1].Distance;
                double dist3 = (p_fatherTraits[2].Name != Constants.Wild) ? p_fatherTraits[2].Distance : p_motherTraits[2].Distance;
                
                //TODO CHANGE!!
                double recombined_rate_1_2 = Math.Abs(dist1 - dist2) / 100;
                double unrecombined_rate_1_2 = 1 - recombined_rate_1_2;

                double recombined_rate_2_3 = Math.Abs(dist2 - dist3) / 100;
                double unrecombined_rate_2_3 = 1 - recombined_rate_2_3;

                #region no_recombine
                building = new List<Trait>();
                building.Add(homs[0]);
                building.Add(homs[1]);
                building.Add(homs[2]);
                returnArr.Add(new CrossTrait(building, (unrecombined_rate_1_2 * unrecombined_rate_2_3)));
                building = new List<Trait>();
                building.Add(hets[0]);
                building.Add(hets[1]);
                building.Add(hets[2]);
                returnArr.Add(new CrossTrait(building, (unrecombined_rate_1_2 * unrecombined_rate_2_3)));
                #endregion
                #region single_recombine
                building = new List<Trait>();
                building.Add(homs[0]);
                building.Add(hets[1]);
                building.Add(hets[2]);
                returnArr.Add(new CrossTrait(building, (recombined_rate_1_2 * unrecombined_rate_2_3)));
                building = new List<Trait>();
                building.Add(hets[0]);
                building.Add(hets[1]);
                building.Add(homs[2]);
                returnArr.Add(new CrossTrait(building, (unrecombined_rate_1_2 * recombined_rate_2_3)));
                building = new List<Trait>();
                building.Add(homs[0]);
                building.Add(homs[1]);
                building.Add(hets[2]);
                returnArr.Add(new CrossTrait(building, (unrecombined_rate_1_2 * recombined_rate_2_3)));
                building = new List<Trait>();
                building.Add(hets[0]);
                building.Add(homs[1]);
                building.Add(homs[2]);
                returnArr.Add(new CrossTrait(building, (recombined_rate_1_2 * unrecombined_rate_2_3)));
                #endregion
                #region double_recombine
                building = new List<Trait>();
                building.Add(hets[0]);
                building.Add(homs[1]);
                building.Add(hets[2]);
                returnArr.Add(new CrossTrait(building, (recombined_rate_1_2 * recombined_rate_2_3)));
                building = new List<Trait>();
                building.Add(homs[0]);
                building.Add(hets[1]);
                building.Add(homs[2]);
                returnArr.Add(new CrossTrait(building, (recombined_rate_1_2 * recombined_rate_2_3)));
                #endregion

                return FinishCross(returnArr, 8);
            }
            #endregion


            /*
             THERE ARE 4 POSSIBLE CASES
             * 1: NO RECOMBINE
             * 2: T1T2 RECOMBINE
             * 3: T1T2T3 RECOMBINE
             * 4: T2T3 RECOMBINE
             */


            //return new CrossList();
        }

        /// <summary>
        /// Called by BasicCross/distance crosses to encapsulate final CrossList build. Do not call elsewhere.
        /// </summary>
        /// <param name="p_possibilities">.</param>
        /// <param name="p_count">.</param>
        /// <returns>A built CrossList</returns>
        private CrossList FinishCross(List<CrossTrait> p_possibilities, int p_count)
        {
            CrossList returnList = new CrossList();
            returnList.Traits = p_possibilities.ToArray();
            returnList.Count = p_count;

            return returnList;
        }

        /// <summary>
        /// Main workhorse for the cross engine, actually arranges the results of the cross methods
        /// </summary>
        /// <param name="p_module">Call ID of module in use</param>
        /// <param name="p_offspring">Total number of offspring to create</param>
        /// <param name="p_generation">Which generation of fly is being created (1 or 2)</param>
        /// <param name="p_cross_results"></param>
        /// <returns></returns>
        private List<FlyViewModel> BuildChildren(int p_module, int p_offspring, int p_generation, CrossList[] p_cross_results)
        {
            //parse all lists and calculate total required flies from counts
            List<FlyViewModel> r_flies = new List<FlyViewModel>();

            //try something different
            List<List<CrossTrait>> all_possible_combinations = CombineTraits(p_cross_results);

            //need to iterate thru and build some flyviewmodels, we can make records later
            FlyViewModel cur = null;
            foreach (List<CrossTrait> working_set in all_possible_combinations)
            {
                //check for chromosome matches in case of module == 2 or 3
                //then perform distance crosses
                if (working_set.Count(t => t.DistanceBased) > 0 && p_module > 2)
                {
                    //iterate thru list and format appropriately
                    //make non-distance list first
                    List<ReducedTraitModel> reg_traits = working_set.Where(t => !t.DistanceBased).Select(g => new ReducedTraitModel(g.Trait)).ToList();
                    List<ReducedTraitModel> dis_traits = working_set.Single(t => t.DistanceBased).DistanceCrossedTraits.Select(g => new ReducedTraitModel(g)).ToList();
                    List<ReducedTraitModel> fin_traits = new List<ReducedTraitModel>();

                    fin_traits.AddRange(reg_traits);
                    fin_traits.AddRange(dis_traits);

                    //create freqs
                    double male_freq = .5 * p_offspring;
                    double female_freq = .5 * p_offspring;
                    foreach (CrossTrait traversing in working_set)
                    {
                        male_freq *= (traversing.Male) ? traversing.Rate : 0;
                        female_freq *= (traversing.Female) ? traversing.Rate : 0;
                    }

                    cur = new FlyViewModel()
                    {
                        Traits = fin_traits.OrderBy(t => t.CategoryId).ToArray(),
                        Frequency = (int)male_freq,
                        Gender = Constants.Male
                    };
                    r_flies.Add(cur);
                    cur = new FlyViewModel()
                    {
                        Traits = fin_traits.OrderBy(t => t.CategoryId).ToArray(),
                        Frequency = (int)female_freq,
                        Gender = Constants.Female
                    };
                    r_flies.Add(cur);
                }
                else
                {
                    cur = new FlyViewModel()
                    {
                        Traits = working_set.Select(g => new ReducedTraitModel(g.Trait)).ToArray(),
                        Frequency = (int)(.5 * working_set.Aggregate(p_offspring, (a, b) => (int)(a * (b.Male ? b.Rate : 0)))),
                        Gender = Constants.Male
                    };
                    r_flies.Add(cur);
                    cur = new FlyViewModel()
                    {
                        Traits = working_set.Select(g => new ReducedTraitModel(g.Trait)).ToArray(),
                        Frequency = (int)(.5 * working_set.Aggregate(p_offspring, (a, b) => (int)(a * (b.Female ? b.Rate : 0)))),
                        Gender = Constants.Female
                    };
                    r_flies.Add(cur);
                }
            }

            //need to filter out flies here

            SoftMatch(ref r_flies);
            return r_flies;
        }

        /// <summary>
        /// Turns a list of crosses into all possible fly trait combinations
        /// </summary>
        /// <param name="p_crosses">An array of cross possibilities</param>
        /// <returns>A phatty sorted list of trait combinations</returns>
        private List<List<CrossTrait>> CombineTraits(CrossList[] p_crosses)
        {
            //CHECK FOR DISTANCE BASED CROSSING IN HERE

            //combine an indiscriminate number of crosslists into a list of lists of possible combos w rates attached
            //use array bc its easier to access these by index in query syntax implementation of Cartesian Product
            CrossList[] non_wild = p_crosses.Where(t => t.Traits.Count(u => u.Trait.Name.ToLower() != "wild") != 0).ToArray();
            //use list so addrange works almost natively
            List<CrossTrait> wild = p_crosses.Where(t => !non_wild.Contains(t)).Select(g => g.Traits[0]).ToList();

            List<List<CrossTrait>> r_lists = new List<List<CrossTrait>>();
            List<CrossTrait> building = null;

            int num_of_lists = non_wild.Length;

            if (num_of_lists == 0)
            {
                //if there are no non wilds the flies will be 100% wild-type no variance
                r_lists.Add(wild);
            }
            if (num_of_lists == 1)
            {
                //add all non wilds then concatenate wilds onto them
                List<CrossTrait> split = (from trait in non_wild[0].Traits
                            select trait).ToList();

                foreach (CrossTrait current in split.ToList())
                {
                    building = new List<CrossTrait>();
                    building.Add(current);
                    building.AddRange(wild.AsEnumerable());
                    r_lists.Add(building.OrderBy(t => t.Trait.CategoryId).ToList());
                }
            }
            if (num_of_lists == 2)
            {
                //cartesian product of 2 lists
                //this syntax courtesy of Eric Lippert's MSDN blog

                //Selects 1 element from each list, makes combo,
                //iterates to next item in first list until all combos
                //are made and starts iterating thru next list

                /*EX:
                 * list1 = { A, B, C }
                 * list2 = { 1, 2, 3 }
                 * cartesian(list1, list2) = { {A, 1}, {B, 1}, {C, 1}, 
                 *                             {A, 2}, {B, 2}, {C, 2}, 
                 *                             {A, 3}, {B, 3}, {C, 3} }
                 */
                var split = (from trait1 in non_wild[0].Traits
                             from trait2 in non_wild[1].Traits
                             select new { trait1, trait2 });
                foreach (var current in split)
                {
                    building = new List<CrossTrait>();
                    building.Add(current.trait1);
                    building.Add(current.trait2);
                    building.AddRange(wild.AsEnumerable());
                    r_lists.Add(building.OrderBy(t => t.Trait.CategoryId).ToList());
                }
            }
            if (num_of_lists == 3)
            {
                //cartesian product of 3 lists
                //same concept as 2 list syntax and implementation but w a third layer
                var split = (from trait1 in non_wild[0].Traits
                             from trait2 in non_wild[1].Traits
                             from trait3 in non_wild[2].Traits
                             select new { trait1, trait2, trait3 });

                foreach (var current in split)
                {
                    building = new List<CrossTrait>();
                    building.Add(current.trait1);
                    building.Add(current.trait2);
                    building.Add(current.trait3);
                    building.AddRange(wild.AsEnumerable());
                    r_lists.Add(building.OrderBy(t => t.Trait.CategoryId).ToList());
                }
            }

            return r_lists;
        }

        #endregion

        #region HELPER METHODS

        /// <summary>
        /// Logs a lab's activity to a UseInstance, which is tied to a user profile. Stores a few browser metrics as well
        /// </summary>
        /// <param name="p_module">The Module being used in the current UseInstance</param>
        /// <param name="p_generation">The generation of flies being produced currently</param>
        /// <param name="p_instance">The Instance being updated with Fly data</param>
        /// <returns>A code, where 0 = success and anything else = failure </returns>
        private int RecordLog(Module p_module, int p_generation, out UseInstance p_instance)
        {
            UseInstance instance = new UseInstance();
            LabUser labUser = new LabUser();
            try
            {
                labUser = db.LabUser.OrderByDescending(t => t.Id).FirstOrDefault(t => t.GID == (CWSToolkit.AuthUser.GetUsername() ?? "_Guest_"));
            }
            catch (ArgumentNullException e)
            {
                Exception ex = new Exception("The log recorder exited with error code 2.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                p_instance = null;
                return 2;
            }
            catch (InvalidOperationException)
            {
                //no labuser profile, so we will make one here
                labUser.GID = CWSToolkit.AuthUser.GetUsername() ?? Constants.Guest;
                labUser.Name = CWSToolkit.AuthUser.GetFullName() ?? Constants.Guest;
                labUser.Active = (labUser.Name != Constants.Guest);
                db.LabUser.Add(labUser);
            }
            catch (Exception e)
            {
                Exception ex = new Exception("The log recorder exited with error code 3.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                p_instance = null;
                return 3;
            }

            //now we have a user to add an instance to

            //check if its an old user and reactivate them
            if (!labUser.Active && labUser.Name != Constants.Guest)
            {
                labUser.Active = true;
                db.Entry(labUser).State = EntityState.Modified;
            }

            instance.Module = p_module;
            instance.Time = DateTime.Now;
            switch (p_generation)
            {
                case 1:
                    instance.Stage = Constants.Stage_1;
                    break;
                case 2:
                    instance.Stage = Constants.Stage_2;
                    break;
                case 3:
                    instance.Stage = Constants.Stage_3;
                    break;
                default:
                    instance.Stage = "Error";
                    break;
            }

            //grab some browser info
            try
            {
                var bc = Request.Browser;
                instance.OS = bc.Platform;
                instance.Browser = bc.Type;
                instance.IP = Request.UserHostAddress;
                p_instance = instance;
            }
            catch (NotImplementedException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                instance.OS = "";
                instance.Browser = "";
                instance.IP = "";
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("The log recorder exited with error code 4.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                p_instance = null;
                return 4;
            }
            p_instance = instance;
            labUser.UseInstances.Add(instance);
            db.UseInstance.Add(instance);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new Exception("The log recorder exited with error code 5.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return 5;
            }

            return 0;
        }

        /// <summary>
        /// helper method to create a fly from the trait strings javascript sends over
        /// </summary>
        /// <param name="p_module">Module parameter for the new fly</param>
        /// <param name="p_gender">Gender parameter for the new fly</param>
        /// <param name="p_traits">Array of TraitIDs for the new fly</param>
        /// <returns>A new Fly, null if it fails</returns>
        private Fly LogFly(int p_module, UseInstance p_instance, string p_gender, ReducedTraitModel[] p_traits)
        {
            Fly toAdd = new Fly() { Id = 0 };
            try
            {
                toAdd.Gender = db.Gender.First(t => t.GenderName == p_gender);
                toAdd.Module = db.Module.First(t => t.Call_id == p_module);
                toAdd.UseInstance = p_instance;
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("Something in the configuration of gender or module is wrong. This should be easily fixable through the Admin Dashboard.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

            //foreach (ReducedTraitModel cur in p_traits)
            //{
            //    Trait newTrait = GetTrait( db.Trait.Find(cur.Id), cur.IsHeterozygous );
            //    toAdd.Traits.Add(newTrait);
            //    db.Entry(newTrait).State = EntityState.Detached;
            //}

            db.Fly.Add(toAdd);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("There was an issue storing a fly (Lab/NewFly).", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                return null;
            }

            return toAdd;
        }

        /// <summary>
        /// Does the same thing as filterflies but adds soft matches (diff traits, same expression)
        /// </summary>
        /// <param name="reference">The list to filter</param>
        private void SoftMatch(ref List<FlyViewModel> reference)
        {
            for (int i = 0; i < reference.Count - 1; i++)
            {
                for (int j = i + 1; j < reference.Count; j++)
                {
                    //we have to combine all homdom and hetdom
                    //and all homwild and hetrec
                    FlyViewModel fly1 = reference.ElementAt(i);
                    FlyViewModel fly2 = reference.ElementAt(j);

                    if (fly1.Frequency > 0 && fly2.Frequency > 0)
                    {
                        Boolean softMatch = true; //default to true, make false if miss
                        for (int k = 0; k < fly1.Traits.Length; k++)
                        {
                            ReducedTraitModel fly1Trt = fly1.Traits[k];
                            ReducedTraitModel fly2Trt = fly2.Traits[k];

                            if (!SoftMatchHelper(fly1Trt, fly2Trt))
                            {
                                softMatch = false;
                            }

                            if (fly1.Gender != fly2.Gender)
                            {
                                softMatch = false;
                            }

                            if (fly1Trt.IsIncompleteDominant || 
                                fly2Trt.IsIncompleteDominant || 
                                fly1Trt.IsLethal || 
                                fly2Trt.IsLethal)
                            {
                                softMatch = false;
                            }

                            //lethal trait kill = kill
                            if (fly1Trt.IsLethal && !fly1Trt.IsHeterozygous)
                            {
                                fly1.Frequency = 0;
                            }
                            if (fly2Trt.IsLethal && !fly2Trt.IsHeterozygous)
                            {
                                fly2.Frequency = 0;
                            }
                        }
                        if (softMatch)
                        {
                            fly2.Frequency += fly1.Frequency;
                            //select non wild traits into fly2
                            for (int m = 0; m < fly1.Traits.Length; m++)
                            {
                                if (fly2.Traits[m].Name == Constants.Wild)
                                {
                                    fly2.Traits[m] = fly1.Traits[m];
                                }
                            }
                            fly1.Frequency = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares two traits and determines whether they are a soft match (heterozygous recessive & wild || het dominant & hom dominant)
        /// </summary>
        /// <param name="p_trt1">A traitmod to compare</param>
        /// <param name="p_trt2">Another traitmod to compare</param>
        /// <returns>True if soft match, false otherwise</returns>
        private bool SoftMatchHelper(ReducedTraitModel p_trt1, ReducedTraitModel p_trt2)
        {
            Boolean t1Het = p_trt1.IsHeterozygous;
            Boolean t2Het = p_trt2.IsHeterozygous;
            Boolean t1Dom = p_trt1.IsDominant;
            Boolean t2Dom = p_trt2.IsDominant;

            return (((p_trt1.Name == p_trt2.Name) && (t1Het == t2Het)) ||
                    ((t1Het && t1Dom) && (!t2Het && t2Dom)) ||
                    ((!t1Het && t1Dom) && (t2Het && t2Dom)) ||
                    ((t1Het && !t1Dom) && (p_trt2.Name.ToLower() == Constants.Wild.ToLower())) ||
                    ((p_trt1.Name.ToLower() == Constants.Wild.ToLower()) && (t2Het && !t2Dom)));
        }

        /// <summary>
        /// Build a new trait object from an existing trait
        /// </summary>
        /// <param name="t">Trait to copy</param>
        /// <param name="isHet">Heterozygosity of new trait</param>
        /// <returns></returns>
        private Trait GetTrait(Trait t, bool isHet)
        {
            return new Trait()
                {
                    Id = t.Id,
                    Category = t.Category,
                    CategoryId = t.CategoryId,
                    ChromosomeNumber = t.ChromosomeNumber,
                    Distance = t.Distance,
                    IsDominant = t.IsDominant,
                    IsHeterozygous = isHet,
                    IsLethal = t.IsLethal,
                    Name = t.Name,
                    IsIncompleteDominant = t.IsIncompleteDominant,
                    ImagePath = t.ImagePath
                };
        }

        /// <summary>
        /// Called via ajax after a user successfully completes their chi^2 tests
        /// </summary>
        /// <param name="p_module">Call ID of module being completed</param>
        /// <returns></returns>
        public JsonResult FinishLab(int p_module)
        {
            Module module = db.Module.FirstOrDefault(t => t.Call_id == p_module);

            UseInstance instance = new UseInstance();
            if (RecordLog(module, 3, out instance) > 0)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }
        #endregion
    }
}




 /****************************************************************/
 /*******************_____    __    __   _____********************/
 /******************|     \  |  \  /  |      /********************/
 /******************|      \ |   \/   |     /*********************/
 /******************|      | |        |    /**********************/
 /******************|      / |        |   /***********************/
 /******************|_____/  |        |  /____********************/
/****************************************************************/

#region dmz
//        /// <summary>
//        /// Helper method to make children out of a given list of trait cross results
//        /// </summary>
//        /// <param name="p_module">The Call ID of the module to use</param>
//        /// <param name="p_offspring">The total number of offspring</param>
//        /// <param name="p_crosses">The list of trait crosses</param>
//        /// <returns></returns>
//        private List<FlyViewModel> Children(int p_module, int p_offspring, UseInstance p_instance, List<Trait[]> p_crosses)
//        {
//            var rawCrosses = p_crosses;
//            var workingFlies = new List<Fly>();
//            var workingTraits = new List<List<Trait>>();
//            var workingTList = new List<List<TraitMod>>();
//            var finalFlies = new List<FlyViewModel>();

//            ////build a trimmed list of all relevant traits
//            foreach (var category in rawCrosses)
//            {
//                var seen = new List<Trait>();
//                foreach (var candidate in category)
//                {
//                    if (!seen.Contains(candidate))
//                    {
//                        seen.Add(candidate);
//                    }
//                }
//                workingTraits.Add(seen);
//            }

//            //ok heres the plan i guess
//            //make a list 
//            //parse it
//            //magic
//            //get paid

//            //redo all of this in one step, should be feasible

//            int prevMax = 1;
//            for (int i = 0; i < workingTraits.Count; i++)
//            {
//                //lets find the max distinct traits
//                prevMax = workingTraits.ElementAt(i).Count * prevMax;
//            }
//            for (int j = 0; j < prevMax; j++)
//            {
//                //add a traitarr for each fly needed
//                workingTList.Add(new List<TraitMod>());
//            }

//            //ok ill build one of each needed fly, then double them in the next step
//            foreach (var traitPile in workingTraits)
//            {
//                int distinct = traitPile.Count;
//                int ofEachTrait = prevMax / distinct;
//                int used = 0;

//                foreach (var trait in traitPile)
//                {
//                    for (int k = used; k < ofEachTrait + used; k++)
//                    {
//                        workingTList.ElementAt(k).Add(new TraitMod() { Id = trait.Id, IsHeterozygous = trait.IsHeterozygous ?? false, Name = trait.Name });
//                    }
//                    used += ofEachTrait;
//                }
//            }

//            ////LETS DO IT BABY
//            var listOfTLists = workingTList.ToArray();
//            foreach (var tList in listOfTLists)
//            {
//                var tArr = tList.ToArray();
//                workingFlies.Add(NewFly(p_module, p_instance, "Male", tArr));
//                workingFlies.Add(NewFly(p_module, p_instance, "Female", tArr));
//            }

//            foreach (var fly in workingFlies)
//            {
//                var tempTraitArr = new List<TraitMod>();
//                foreach (var trait in fly.Traits)
//                {
//                    tempTraitArr.Add(new TraitMod() { Id = trait.Id, IsHeterozygous = trait.IsHeterozygous??false, Name = trait.Name });
//                }
//                finalFlies.Add(new FlyViewModel() { FlyID = fly.Id, Frequency = p_offspring, Gender = fly.Gender.GenderName, Traits = tempTraitArr.ToArray() });
//            }

//            return calculateFrequencies(p_offspring, finalFlies, p_crosses, workingTraits);
//        }

//        /// <summary>
//        /// Helper method to calculate the occurrence rate of each fly in a given list of FlyViewModels
//        /// </summary>
//        /// <param name="p_offspring">Total number of flies to divvie up</param>
//        /// <param name="p_flies">The list of FlyViewModels to operate on</param>
//        /// <param name="p_crosses">The raw cross data from several steps back in the process</param>
//        /// <param name="p_workingTraits">The list of lists of distinct traits used to determine which traits should be mated</param>
//        /// <returns>A list of FVMs with correct frequencies</returns>
//        private List<FlyViewModel> calculateFrequencies(int p_offspring, List<FlyViewModel> p_flies, List<Trait[]> p_crosses, List<List<Trait>> p_workingTraits)
//        {
            //var dice = new Random(DateTime.Now.Second);
            //var offset = Convert.ToInt32(p_offspring * 0.02); //shifts the maximum offset from specified offspring #, change this to change globally

//            var relevantCats = new List<int>();

//            foreach (var set in p_workingTraits)
//            {
//                if (set.Count > 1) //we found a non wild
//                {
//                    relevantCats.Add(set.ElementAt(0).CategoryId);
//                }
//            }

//            //we need the genders split here (this may have to be changed after
//            //var ungendered = p_flies.Where(t => t.Gender.ToLower() == "male").ToList();
//            foreach (var fly in p_flies)
//            {
//                //we can operate on each fly individually here

//                /* IDEAS
//                 *  1 - iterate thru crosses, find each (non wild) trait attached to fly in question
//                 *  2 - calculate occurrence of each non-wild trait and apply some mathematics
//                 *  3 - ????
//                 *  4 - profit */

//                // ** acquired nonwild list (will have to wait until F2 is ready, but I think 
//                // ** if something is 100% wild traits it will always be ~50/50)
//                // ** even if we do a tt x ww cross we wont have a fully wild fly, it'll be 100% tw

//                //we need a list of all relevant traits, including wilds for F2
//                var nonWilds = new List<TraitMod>();

//                foreach (var trait in fly.Traits)
//                {
//                    //get full trait info from db
//                    var tempTrt = db.Trait.Find(trait.Id);
//                    if (relevantCats.Contains(tempTrt.CategoryId))
//                    {
//                        nonWilds.Add(trait);
//                    }
//                }

//                double percent = 50;

//                foreach (var calculatingOn in nonWilds)
//                {
//                    //uuuugh
//                    var relevantCross = p_crosses.First(v => v.Contains(db.Trait.Find(calculatingOn.Id)));
//                    double matched = 0;
//                    foreach (var checking in relevantCross)
//                    {
//                        matched = (checking.Id == calculatingOn.Id) ? matched + 1 : matched;
//                    }
//                    percent = percent * (matched / 4.0);
//                }
//                fly.Frequency = Convert.ToInt32( p_offspring * (percent / 100.0) );
//                fly.Frequency -= dice.Next(offset);
//            }
//            return p_flies;
//        }



///// <summary>
//        /// Helper method to make children out of a given list of trait cross results
//        /// </summary>
//        /// <param name="p_module">The Call ID of the module to use</param>
//        /// <param name="p_offspring">The total number of offspring</param>
//        /// <param name="p_crosses">The list of trait crosses</param>
//        /// <returns></returns>
//        private List<FlyViewModel> Children(int p_module, int p_offspring, UseInstance p_instance, List<Trait[]> p_crosses)
//        {
//            //we have the crosses, make the potential children first
//            var wow = Combine(p_crosses);
//            return null;
//        }

//        private List<TraitMod[]> Combine(List<Trait[]> p_crosses)
//        {
//            var returnlist = new List<TraitMod[]>();
//            var numFlies = maxFlies(p_crosses);

//            return null;
//        }

//        private int maxFlies(List<Trait[]> p_crosses, int current = 1)
//        {
//            if (p_crosses.Count > 0)
//            {
//                var cur = p_crosses.ElementAt(0);
//                var seen = new List<Trait>();
//                foreach (var item in cur)
//                {
//                    if (!seen.Contains(item))
//                    {
//                        seen.Add(item);
//                    }
//                }
//                p_crosses.Remove(cur);
//                return maxFlies(p_crosses, (current * seen.Count));
//            }
//            return current;
//        }


/// <summary>
        ///// Turns a trait pair into 4 possible children in line with punnett square rules
        ///// </summary>
        ///// <param name="p_pair">Array of 2 traits to be processed, the first being the father's trait and the second the mother's</param>
        ///// <returns>An array of Traits representing possible children of given trait pair, null if error</returns>
        //private Trait[] Cross(ReducedTraitModel[] p_pair)
        //{
        //    Trait dbTrt1 = db.Trait.Find(p_pair[0].Id);
        //    Trait dbTrt2 = db.Trait.Find(p_pair[1].Id);
        //    Trait trait1 = new Trait();
        //    Trait trait2 = new Trait();

        //    if (dbTrt1.ChromosomeNumber == 1 || dbTrt2.ChromosomeNumber == 1)
        //    {
        //        //take in traits after detecting that they are sex linked (on chromosome 1)
        //        //make special sex trait and perform cross
        //        //parse it in children method
        //        //??????
        //        //profit

        //        //make trait1 heterozygous (Male XY) and trait2 homozygous (Female XX)
        //        trait1 = new Trait() {
        //            Id = dbTrt1.Id,
        //            Name = "SexTrait",
        //            Father = new Trait()
        //            {
        //                Id = dbTrt1.Id,
        //                Category = dbTrt1.Category,
        //                CategoryId = dbTrt1.CategoryId,
        //                ChromosomeNumber = dbTrt1.ChromosomeNumber,
        //                Distance = dbTrt1.Distance,
        //                IsDominant = dbTrt1.IsDominant,
        //                IsHeterozygous = p_pair[0].IsHeterozygous,
        //                IsLethal = dbTrt1.IsLethal,
        //                Name = dbTrt1.Name,
        //                IsIncompleteDominant = dbTrt1.IsIncompleteDominant,
        //                ImagePath = dbTrt1.ImagePath
        //            },
        //            Mother = new Trait()
        //            {
        //                Id = dbTrt2.Id,
        //                Category = dbTrt2.Category,
        //                CategoryId = dbTrt2.CategoryId,
        //                ChromosomeNumber = dbTrt2.ChromosomeNumber,
        //                Distance = dbTrt2.Distance,
        //                IsDominant = dbTrt2.IsDominant,
        //                IsHeterozygous = p_pair[1].IsHeterozygous,
        //                IsLethal = dbTrt2.IsLethal,
        //                Name = dbTrt2.Name,
        //                IsIncompleteDominant = dbTrt2.IsIncompleteDominant,
        //                ImagePath = dbTrt2.ImagePath
        //            },
        //            IsHeterozygous = true,
        //            ChromosomeNumber = 1,
        //            CategoryId = dbTrt1.CategoryId
        //        };
        //        trait2 = new Trait()
        //        {
        //            Id = dbTrt2.Id,
        //            Name = "SexTrait",
        //            Father = new Trait()
        //            {
        //                Id = dbTrt1.Id,
        //                Category = dbTrt1.Category,
        //                CategoryId = dbTrt1.CategoryId,
        //                ChromosomeNumber = dbTrt1.ChromosomeNumber,
        //                Distance = dbTrt1.Distance,
        //                IsDominant = dbTrt1.IsDominant,
        //                IsHeterozygous = p_pair[0].IsHeterozygous,
        //                IsLethal = dbTrt1.IsLethal,
        //                Name = dbTrt1.Name,
        //                IsIncompleteDominant = dbTrt1.IsIncompleteDominant,
        //                ImagePath = dbTrt1.ImagePath
        //            },
        //            Mother = new Trait()
        //            {
        //                Id = dbTrt2.Id,
        //                Category = dbTrt2.Category,
        //                CategoryId = dbTrt2.CategoryId,
        //                ChromosomeNumber = dbTrt2.ChromosomeNumber,
        //                Distance = dbTrt2.Distance,
        //                IsDominant = dbTrt2.IsDominant,
        //                IsHeterozygous = p_pair[1].IsHeterozygous,
        //                IsLethal = dbTrt2.IsLethal,
        //                Name = dbTrt2.Name,
        //                IsIncompleteDominant = dbTrt2.IsIncompleteDominant,
        //                ImagePath = dbTrt2.ImagePath
        //            },
        //            IsHeterozygous = false,
        //            ChromosomeNumber = 1,
        //            CategoryId = dbTrt2.CategoryId
        //        };
        //    }
        //    else
        //    {
        //        trait1 = new Trait()
        //        {
        //            Id = dbTrt1.Id,
        //            Category = dbTrt1.Category,
        //            CategoryId = dbTrt1.CategoryId,
        //            ChromosomeNumber = dbTrt1.ChromosomeNumber,
        //            Distance = dbTrt1.Distance,
        //            IsDominant = dbTrt1.IsDominant,
        //            IsHeterozygous = p_pair[0].IsHeterozygous,
        //            IsLethal = dbTrt1.IsLethal,
        //            Name = dbTrt1.Name,
        //            IsIncompleteDominant = dbTrt1.IsIncompleteDominant,
        //            ImagePath = dbTrt1.ImagePath
        //        };

        //        trait2 = new Trait()
        //        {
        //            Id = dbTrt2.Id,
        //            Category = dbTrt2.Category,
        //            CategoryId = dbTrt2.CategoryId,
        //            ChromosomeNumber = dbTrt2.ChromosomeNumber,
        //            Distance = dbTrt2.Distance,
        //            IsDominant = dbTrt2.IsDominant,
        //            IsHeterozygous = p_pair[1].IsHeterozygous,
        //            IsLethal = dbTrt2.IsLethal,
        //            Name = dbTrt2.Name,
        //            IsIncompleteDominant = dbTrt2.IsIncompleteDominant,
        //            ImagePath = dbTrt2.ImagePath
        //        };
        //    }

        //    Trait[] returnArr = new Trait[4];

        //    //het-het
        //    if ((trait1.IsHeterozygous ?? false) && (trait2.IsHeterozygous ?? false))
        //    {
        //        //we know theyre same trait so we return accordingly
        //        Trait wildTrait = db.Trait.First(t => t.Name.ToLower() == "wild" && t.CategoryId == trait1.CategoryId);

        //        Trait homTrait = new Trait() { 
        //            Id = trait1.Id, Category = trait1.Category, 
        //            CategoryId = trait1.CategoryId, 
        //            ChromosomeNumber = trait1.ChromosomeNumber, 
        //            Distance = trait1.Distance, 
        //            IsDominant = trait1.IsDominant, 
        //            IsHeterozygous = false, 
        //            IsLethal = trait1.IsLethal, 
        //            Name = trait1.Name, 
        //            IsIncompleteDominant = trait1.IsIncompleteDominant,
        //            ImagePath = trait1.ImagePath
        //        };

        //        Trait hetTrait = new Trait() { 
        //            Id = trait1.Id, 
        //            Category = trait1.Category, 
        //            CategoryId = trait1.CategoryId, 
        //            ChromosomeNumber = trait1.ChromosomeNumber, 
        //            Distance = trait1.Distance, 
        //            IsDominant = trait1.IsDominant, 
        //            IsHeterozygous = true, 
        //            IsLethal = trait1.IsLethal, 
        //            Name = trait1.Name, 
        //            IsIncompleteDominant = trait1.IsIncompleteDominant 
        //        };

        //        returnArr[0] = homTrait;
        //        returnArr[1] = hetTrait;
        //        returnArr[2] = hetTrait;
        //        returnArr[3] = wildTrait;
        //        return returnArr;
        //    }
        //    //hom-hom
        //    if (!(trait1.IsHeterozygous ?? false) && !(trait2.IsHeterozygous ?? false))
        //    {
        //        if (trait1.Name.ToLower() == "wild" && trait2.Name.ToLower() == "wild")
        //        {
        //            returnArr[0] = trait1;
        //            returnArr[1] = trait1;
        //            returnArr[2] = trait1;
        //            returnArr[3] = trait1;
        //            return returnArr;
        //        }
        //        //rec-wild
        //        else if (!trait1.IsDominant && trait2.Name.ToLower() == "wild")
        //        {
        //            Trait expressed = trait1;
        //            expressed.IsHeterozygous = true;

        //            returnArr[0] = expressed;
        //            returnArr[1] = expressed;
        //            returnArr[2] = expressed;
        //            returnArr[3] = expressed;
        //            return returnArr;
        //        }
        //        //wild-rec
        //        else if (!trait2.IsDominant && trait1.Name.ToLower() == "wild")
        //        {
        //            Trait expressed = trait2;
        //            expressed.IsHeterozygous = true;

        //            returnArr[0] = expressed;
        //            returnArr[1] = expressed;
        //            returnArr[2] = expressed;
        //            returnArr[3] = expressed;
        //            return returnArr;
        //        }
        //        //dom-rec*
        //        else if (trait1.IsDominant)
        //        {
        //            Trait expressed = trait1;
        //            expressed.IsHeterozygous = true;

        //            returnArr[0] = expressed;
        //            returnArr[1] = expressed;
        //            returnArr[2] = expressed;
        //            returnArr[3] = expressed;
        //            return returnArr;
        //        }
        //        else if (trait2.IsDominant)
        //        {
        //            Trait expressed = trait2;
        //            expressed.IsHeterozygous = true;

        //            returnArr[0] = expressed;
        //            returnArr[1] = expressed;
        //            returnArr[2] = expressed;
        //            returnArr[3] = expressed;
        //            return returnArr;
        //        }
        //    }
        //    //hom-het
        //    if ((!(trait1.IsHeterozygous ?? false) && (trait2.IsHeterozygous ?? false)) || (!(trait2.IsHeterozygous ?? false) && (trait1.IsHeterozygous ?? false)))
        //    {
        //        Trait homTrait = (trait1.IsHeterozygous ?? false) ? trait2 : trait1;
        //        Trait hetTrait = (trait1.IsHeterozygous ?? false) ? trait1 : trait2;

        //        returnArr[0] = homTrait;
        //        returnArr[1] = hetTrait;
        //        returnArr[2] = hetTrait;
        //        returnArr[3] = homTrait;
        //        return returnArr;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Helper method to make children out of a given list of trait cross results
        ///// </summary>
        ///// <param name="p_module">The Call ID of the module to use</param>
        ///// <param name="p_offspring">The total number of offspring</param>
        ///// <param name="p_crosses">The list of trait crosses</param>
        ///// <returns></returns>
        //private List<FlyViewModel> Children(int p_module, int p_offspring, int p_generation, UseInstance p_instance, List<Trait[]> p_crosses)
        //{
        //    Module module = db.Module.First(t => t.Call_id == p_module);
        //    //we need to find the non wild traits and make some pairs. if there are none return 2 wild types
        //    Trait[] wildTraits = db.Trait.Where(t => t.Name.ToLower() == "wild").OrderBy(t => t.CategoryId).ToArray();
        //    List<ReducedTraitModel> temp = new List<ReducedTraitModel>();
        //    foreach (Trait trt in wildTraits)
        //    {
        //        temp.Add(new ReducedTraitModel() { 
        //            Id = trt.Id, 
        //            IsHeterozygous = false, 
        //            Name = trt.Name, 
        //            IsDominant = trt.IsDominant,
        //            IsIncompleteDominant = trt.IsIncompleteDominant,
        //            IsLethal = trt.IsLethal,
        //            ChromosomeNumber = trt.ChromosomeNumber
        //        });
        //    }
        //    ReducedTraitModel[] wildMods = temp.ToArray();
        //    List<Trait[]> localCrosses = p_crosses;
        //    List<FlyViewModel> finalFlies = new List<FlyViewModel>();
        //    List<Fly> workingFlies = new List<Fly>();
        //    int code = FilterPairs(ref localCrosses);

        //    Random dice = new Random(DateTime.Now.Second);
        //    //shifts the maximum offset from specified offspring #, change this to change globally
        //    int offset = Convert.ToInt32(p_offspring * 0.02);

        //    //p_crosses now contains all non-wild trait sets
        //    if (code == 0)
        //    {
        //        if (p_generation == 2)
        //        {
        //            int logCode = RecordLog(module, 3, out p_instance);
        //        }

        //        //we have a pure wild pairing, return 2 wild type flies
        //        Fly mChild = LogFly(p_module, p_instance, "Male", wildMods);
        //        Fly fChild = LogFly(p_module, p_instance, "Female", wildMods);
        //        int frequency = Convert.ToInt32(p_offspring * (.5));
        //        frequency -= dice.Next(offset);

        //        finalFlies.Add(new FlyViewModel() { 
        //            FlyID = mChild.Id, 
        //            Gender = mChild.Gender.GenderName, 
        //            Traits = wildMods, 
        //            Frequency = frequency
        //        });

        //        frequency = Convert.ToInt32(p_offspring * (.5));
        //        frequency -= dice.Next(offset);

        //        finalFlies.Add(new FlyViewModel() { 
        //            FlyID = fChild.Id, 
        //            Gender = fChild.Gender.GenderName, 
        //            Traits = wildMods, 
        //            Frequency = frequency 
        //        });

        //        return finalFlies;
        //    }
        //    else
        //    {
        //        if (code == 1)
        //        {
        //            foreach (Trait nonWild1 in localCrosses.ElementAt(0))
        //            {
        //                List<ReducedTraitModel> tempMods = new List<ReducedTraitModel>();
        //                tempMods.Add(new ReducedTraitModel() { 
        //                    Id = nonWild1.Id, 
        //                    IsHeterozygous = nonWild1.IsHeterozygous ?? false, 
        //                    Name = nonWild1.Name, 
        //                    IsDominant = nonWild1.IsDominant,
        //                    IsIncompleteDominant = nonWild1.IsIncompleteDominant,
        //                    IsLethal = nonWild1.IsLethal,
        //                    ChromosomeNumber = nonWild1.ChromosomeNumber
        //                });

        //                //fly needs wild traits
        //                foreach (Trait wild in wildTraits)
        //                {
        //                    Boolean shouldAdd = true;
        //                    foreach (ReducedTraitModel trt in tempMods)
        //                    {
        //                        Trait trait = db.Trait.Find(trt.Id);
        //                        if (trait.CategoryId == wild.CategoryId)
        //                        {
        //                            shouldAdd = false;
        //                        }
        //                    }
        //                    if (shouldAdd)
        //                    {
        //                        tempMods.Add(new ReducedTraitModel() { 
        //                            Id = wild.Id, 
        //                            IsHeterozygous = false, 
        //                            Name = "Wild", 
        //                            IsDominant = false,
        //                            IsIncompleteDominant = false,
        //                            IsLethal = false,
        //                            ChromosomeNumber = 0
        //                        });
        //                    }
        //                }

        //                if (nonWild1.ChromosomeNumber == 1)
        //                {
        //                    //hit a sex linked fly, have to make male or female based on heterozygousity of SexTrait
        //                    SexLinkedHelper(tempMods, ref finalFlies, nonWild1);
        //                }
        //                else
        //                {
        //                    //gender anonymous
        //                    finalFlies.Add(new FlyViewModel()
        //                    {
        //                        FlyID = 0,
        //                        Frequency = 0,
        //                        Gender = "Male",
        //                        Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                    });
        //                    finalFlies.Add(new FlyViewModel()
        //                    {
        //                        FlyID = 0,
        //                        Frequency = 0,
        //                        Gender = "Female",
        //                        Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                    });
        //                }
        //            }
        //        }

        //        if (code == 2)
        //        {
        //            foreach (Trait nonWild1 in localCrosses.ElementAt(0))
        //            {
        //                foreach (Trait nonWild2 in localCrosses.ElementAt(1))
        //                {
        //                    List<ReducedTraitModel> tempMods = new List<ReducedTraitModel>();
        //                    tempMods.Add(new ReducedTraitModel() { 
        //                        Id = nonWild1.Id, 
        //                        IsHeterozygous = nonWild1.IsHeterozygous ?? false, 
        //                        Name = nonWild1.Name, 
        //                        IsDominant = nonWild1.IsDominant,
        //                        IsIncompleteDominant = nonWild1.IsIncompleteDominant,
        //                        IsLethal = nonWild1.IsLethal,
        //                        ChromosomeNumber = nonWild1.ChromosomeNumber
        //                    });
        //                    tempMods.Add(new ReducedTraitModel() { 
        //                        Id = nonWild2.Id, 
        //                        IsHeterozygous = nonWild2.IsHeterozygous ?? false, 
        //                        Name = nonWild2.Name, 
        //                        IsDominant = nonWild2.IsDominant,
        //                        IsIncompleteDominant = nonWild2.IsIncompleteDominant,
        //                        IsLethal = nonWild2.IsLethal,
        //                        ChromosomeNumber = nonWild2.ChromosomeNumber
        //                    });

        //                    //fly needs wild traits
        //                    foreach (Trait wild in wildTraits)
        //                    {
        //                        Boolean shouldAdd = true;
        //                        foreach (ReducedTraitModel trt in tempMods)
        //                        {
        //                            Trait trait = db.Trait.Find(trt.Id);
        //                            if (trait.CategoryId == wild.CategoryId)
        //                            {
        //                                shouldAdd = false;
        //                            }
        //                        }
        //                        if (shouldAdd)
        //                        {
        //                            tempMods.Add(new ReducedTraitModel() { 
        //                                Id = wild.Id, 
        //                                IsHeterozygous = false, 
        //                                Name = "Wild", 
        //                                IsDominant = false,
        //                                IsIncompleteDominant = false,
        //                                IsLethal = false,
        //                                ChromosomeNumber = 0
        //                            });
        //                        }
        //                    }

        //                    if (nonWild1.ChromosomeNumber == 1)
        //                    {
        //                        //hit a sex linked fly, have to make male or female based on heterozygousity of SexTrait
        //                        SexLinkedHelper(tempMods, ref finalFlies, nonWild1);
        //                    }
        //                    else if (nonWild2.ChromosomeNumber == 1)
        //                    {
        //                        SexLinkedHelper(tempMods, ref finalFlies, nonWild2);
        //                    }
        //                    else
        //                    {
        //                        //gender anonymous
        //                        finalFlies.Add(new FlyViewModel()
        //                        {
        //                            FlyID = 0,
        //                            Frequency = 0,
        //                            Gender = "Male",
        //                            Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                        });
        //                        finalFlies.Add(new FlyViewModel()
        //                        {
        //                            FlyID = 0,
        //                            Frequency = 0,
        //                            Gender = "Female",
        //                            Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        if (code == 3)
        //        {
        //            foreach (Trait nonWild1 in localCrosses.ElementAt(0))
        //            {
        //                foreach (Trait nonWild2 in localCrosses.ElementAt(1))
        //                {
        //                    foreach (Trait nonWild3 in localCrosses.ElementAt(2))
        //                    {
        //                        List<ReducedTraitModel> tempMods = new List<ReducedTraitModel>();
        //                        tempMods.Add(new ReducedTraitModel() { 
        //                            Id = nonWild1.Id, 
        //                            IsHeterozygous = nonWild1.IsHeterozygous ?? false,
        //                            Name = nonWild1.Name, 
        //                            IsDominant = nonWild1.IsDominant,
        //                            IsIncompleteDominant = nonWild1.IsIncompleteDominant,
        //                            IsLethal = nonWild1.IsLethal,
        //                            ChromosomeNumber = nonWild1.ChromosomeNumber
        //                        });
        //                        tempMods.Add(new ReducedTraitModel() { 
        //                            Id = nonWild2.Id, 
        //                            IsHeterozygous = nonWild2.IsHeterozygous ?? false,
        //                            Name = nonWild2.Name,
        //                            IsDominant = nonWild2.IsDominant,
        //                            IsIncompleteDominant = nonWild2.IsIncompleteDominant,
        //                            IsLethal = nonWild2.IsLethal,
        //                            ChromosomeNumber = nonWild2.ChromosomeNumber
        //                        });
        //                        tempMods.Add(new ReducedTraitModel() { 
        //                            Id = nonWild3.Id, 
        //                            IsHeterozygous = nonWild3.IsHeterozygous ?? false, 
        //                            Name = nonWild3.Name,
        //                            IsDominant = nonWild3.IsDominant,
        //                            IsIncompleteDominant = nonWild3.IsIncompleteDominant,
        //                            IsLethal = nonWild3.IsLethal,
        //                            ChromosomeNumber = nonWild3.ChromosomeNumber
        //                        });

        //                        //fly needs wild traits
        //                        foreach (Trait wild in wildTraits)
        //                        {
        //                            Boolean shouldAdd = true;
        //                            foreach (ReducedTraitModel trt in tempMods)
        //                            {
        //                                Trait trait = db.Trait.Find(trt.Id);
        //                                if (trait.CategoryId == wild.CategoryId)
        //                                {
        //                                    shouldAdd = false;
        //                                }
        //                            }
        //                            if (shouldAdd)
        //                            {
        //                                tempMods.Add(new ReducedTraitModel() { 
        //                                    Id = wild.Id, 
        //                                    IsHeterozygous = false, 
        //                                    Name = "Wild", 
        //                                    IsDominant = false,
        //                                    IsIncompleteDominant = false,
        //                                    IsLethal = false,
        //                                    ChromosomeNumber = 0
        //                                });
        //                            }
        //                        }

        //                        if (nonWild1.ChromosomeNumber == 1)
        //                        {
        //                            //hit a sex linked fly, have to make male or female based on heterozygousity of SexTrait
        //                            SexLinkedHelper(tempMods, ref finalFlies, nonWild1);
        //                        }
        //                        else if (nonWild2.ChromosomeNumber == 1)
        //                        {
        //                            SexLinkedHelper(tempMods, ref finalFlies, nonWild2);
        //                        }
        //                        else if (nonWild3.ChromosomeNumber == 1)
        //                        {
        //                            SexLinkedHelper(tempMods, ref finalFlies, nonWild3);
        //                        }
        //                        else
        //                        {
        //                            //gender anonymous
        //                            finalFlies.Add(new FlyViewModel()
        //                            {
        //                                FlyID = 0,
        //                                Frequency = 0,
        //                                Gender = "Male",
        //                                Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                            });
        //                            finalFlies.Add(new FlyViewModel()
        //                            {
        //                                FlyID = 0,
        //                                Frequency = 0,
        //                                Gender = "Female",
        //                                Traits = tempMods.OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
        //                            });
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        //clean em up

        //        //mendelian filter
        //        List<FlyViewModel> forDB = new List<FlyViewModel>();

        //        if (module.Call_id <= 2)
        //        {
        //            forDB = FilterFlies(finalFlies, p_offspring);
        //        }
        //        else
        //        {
        //            //forDB = DistanceFilter(finalFlies, p_offspring);
        //        }

        //        //match up and add up same expression traits
        //        SoftMatch(ref forDB);

        //        if (p_generation == 2)
        //        {
        //            int logCode = RecordLog(module, 3, out p_instance);
        //        }

        //        foreach (FlyViewModel fly in forDB.ToList())
        //        {
        //            if (fly.Frequency > 0)
        //            {
        //                //run thru newfly so it gets logged
        //                Fly dummy = LogFly(p_module, p_instance, fly.Gender, fly.Traits);
        //            }
        //            else
        //            {
        //                forDB.Remove(fly);
        //            }
        //        }
        //        return forDB;
        //    }
        //}

        ///// <summary>
        ///// Filters a given list of crosses into a set of non wild-type crosses
        ///// </summary>
        ///// <param name="p_crosses"></param>
        ///// <returns>The number of nonwild traits on the fly</returns>
        //private int FilterPairs(ref List<Trait[]> p_crosses)
        //{
        //    List<Trait[]> localCrosses = p_crosses;
        //    Int32 count = 0;


        //    foreach (Trait[] crossSet in localCrosses.ToList())
        //    {
        //        //we have a set of crosses, determine which ones contain wild traits
        //        Boolean wild =  crossSet[0].Name.ToLower() == "wild" &&
        //                    crossSet[1].Name.ToLower() == "wild" &&
        //                    crossSet[2].Name.ToLower() == "wild" &&
        //                    crossSet[3].Name.ToLower() == "wild";
        //        if (wild)
        //        {
        //            localCrosses.Remove(crossSet);
        //        }
        //        else
        //        {
        //            count++;
        //        }
        //    }

        //    return count;
        //}

        ///// <summary>
        ///// Filters a complete list of possible flies down to distinct flies, adding up the number of occurences of each type as it goes
        ///// </summary>
        ///// <param name="p_flies">The list of flies to filter</param>
        ///// <param name="p_offspring">The total number of offspring requested by the user</param>
        ///// <returns>The filtered list of flies</returns>
        //private List<FlyViewModel> FilterFlies(List<FlyViewModel> p_flies, int p_offspring)
        //{
        //    List<FlyViewModel> returnFlies = new List<FlyViewModel>();
        //    Random dice = new Random(DateTime.Now.Second);

        //    //ok the correct stuff is coming through so we need to filter it down
        //    foreach (FlyViewModel fly in p_flies)
        //    {
        //        if (!Contains(ref returnFlies, fly))
        //        {
        //            fly.Frequency++;
        //            returnFlies.Add(fly);
        //        }
        //    }

        //    //set the frequencies here
        //    foreach (FlyViewModel fly in returnFlies)
        //    {
        //        Int32 freq = fly.Frequency;
        //        Int32 freqFraction = (int)(((double)freq / p_flies.Count) * p_offspring);
        //        fly.Frequency = freqFraction -= dice.Next(Convert.ToInt32(freqFraction * 0.01));
        //    }
            
        //    return returnFlies;
        //}

        ///// <summary>
        ///// Searches a given list of flies (reference) for a specific fly (query) and increments the appropriate frequency if it finds one
        ///// </summary>
        ///// <param name="reference">The list to search through</param>
        ///// <param name="query">The fly being searched for</param>
        ///// <returns>True if found, false if not</returns>
        //private bool Contains(ref List<FlyViewModel> reference, FlyViewModel query)
        //{
        //    if (reference.Count == 0)
        //    {
        //        //nothing here, so it cant contain it
        //        return false;
        //    }
        //    foreach (FlyViewModel checking in reference)
        //    {
        //        //make sure theyre in order before we toss em around a bit
        //        List<ReducedTraitModel> refList = checking.Traits.ToList();
        //        List<ReducedTraitModel> qList = query.Traits.ToList();
        //        Boolean[] matched = new bool[refList.Count];
        //        Boolean failMatch = false;
        //        Int32 i = 0; 

        //        while (!failMatch && i < refList.Count)
        //        {
        //            failMatch = !(refList[i].Id == qList[i].Id && refList[i].IsHeterozygous == qList[i].IsHeterozygous);
        //            i++;
        //        }
        //        if (!failMatch)
        //        {
        //            //traits were a match, try gender
        //            if (checking.Gender == query.Gender)
        //            {
        //                checking.Frequency++;
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

#region lool
/*
 private void SexLinkedHelper(List<ReducedTraitModel> sexLinkedFly, ref List<FlyViewModel> finalFlies, Trait sexLinkedTrait)
        {
            //we know the list contains a sex-linked trait, so find it
            foreach (ReducedTraitModel checking in sexLinkedFly)
            {
                if (checking.ChromosomeNumber == 1 && checking.Id != sexLinkedTrait.Id)
                {
                    //not the first one we found, make this sucker wild ASAP
                    Int32 catID = db.Trait.Find(checking.Id).CategoryId;
                    Trait wildTrait = db.Trait.SingleOrDefault(t => t.CategoryId == catID && t.Name.ToLower() == Constants.Wild.ToLower());
                    checking.Name = Constants.Wild;
                    checking.Id = wildTrait.Id;
                    checking.IsDominant = false;
                    checking.IsHeterozygous = false;
                    checking.IsIncompleteDominant = false;
                    checking.IsLethal = false;
                }
            }
            
            Int32 found = sexLinkedFly.IndexOf(sexLinkedFly.Find(t => t.Id == sexLinkedTrait.Id));
            //begin the transformation

            ReducedTraitModel[] son = sexLinkedFly.ToArray();
            ReducedTraitModel[] daughter = sexLinkedFly.ToArray();

            ReducedTraitModel mother = new ReducedTraitModel()
            {
                Name = sexLinkedTrait.Mother.Name,
                Id = sexLinkedTrait.Mother.Id,
                ChromosomeNumber = sexLinkedTrait.Mother.ChromosomeNumber,
                IsDominant = sexLinkedTrait.Mother.IsDominant,
                IsHeterozygous = sexLinkedTrait.Mother.IsHeterozygous ?? false,
                IsIncompleteDominant = sexLinkedTrait.Mother.IsIncompleteDominant,
                IsLethal = sexLinkedTrait.Mother.IsLethal
            };
            ReducedTraitModel father = new ReducedTraitModel()
            {
                Name = sexLinkedTrait.Father.Name,
                Id = sexLinkedTrait.Father.Id,
                ChromosomeNumber = sexLinkedTrait.Father.ChromosomeNumber,
                IsDominant = sexLinkedTrait.Father.IsDominant,
                IsHeterozygous = sexLinkedTrait.Father.IsHeterozygous ?? false,
                IsIncompleteDominant = sexLinkedTrait.Father.IsIncompleteDominant,
                IsLethal = sexLinkedTrait.Father.IsLethal
            };

            if (sexLinkedTrait.Father.ChromosomeNumber == 1)
            {
                //affected father
                if (sexLinkedTrait.Father.IsDominant)
                {
                    if (!(sexLinkedTrait.Mother.IsHeterozygous ?? false))
                    {
                        //sons unaffected, daughters affected
                        daughter[found] = father;
                        daughter[found].IsHeterozygous = true;
                        son[found] = mother;
                        son[found].IsHeterozygous = false;

                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                    else if ((sexLinkedTrait.Mother.IsHeterozygous ?? false))
                    {
                        //split 1:1:2 affmale:unaffmale:afffemale

                        //males
                        son[found] = father;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        son[found] = new ReducedTraitModel()
                        {
                            Name = Constants.Wild,
                            Id = db.Trait.SingleOrDefault(t => t.CategoryId == sexLinkedTrait.CategoryId && t.Name.ToLower() == Constants.Wild.ToLower()).Id,
                            ChromosomeNumber = 0,
                            IsLethal = false,
                            IsDominant = false,
                            IsHeterozygous = false,
                            IsIncompleteDominant = false
                        };
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        //females
                        daughter[found] = father;
                        daughter[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found].IsHeterozygous = true;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                }
                else
                {
                    //we got a recessive one
                    if (!(sexLinkedTrait.Mother.IsHeterozygous ?? false))
                    {
                        son[found] = new ReducedTraitModel()
                        {
                            Name = Constants.Wild,
                            Id = db.Trait.SingleOrDefault(t => t.CategoryId == sexLinkedTrait.CategoryId && t.Name.ToLower() == Constants.Wild.ToLower()).Id,
                            ChromosomeNumber = 0,
                            IsLethal = false,
                            IsDominant = false,
                            IsHeterozygous = false,
                            IsIncompleteDominant = false
                        };
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = father;
                        daughter[found].IsHeterozygous = true;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                    else if ((sexLinkedTrait.Mother.IsHeterozygous ?? false))
                    {
                        //affected kids
                        son[found] = father;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = father;
                        daughter[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        //unaffected kids
                        son[found] = mother;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = mother;
                        daughter[found].IsHeterozygous = true;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                }
            }
            else
            {
                //affected mother
                if (sexLinkedTrait.Mother.IsDominant)
                {
                    if (sexLinkedTrait.Mother.IsHeterozygous ?? false)
                    {
                        //hetdom sex linked
                        //unaffected kids
                        son[found] = father;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = father;
                        daughter[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        //affected kids
                        son[found] = mother;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = mother;
                        daughter[found].IsHeterozygous = true;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                    else
                    {
                        //homdom sex linked
                        //affected kids
                        son[found] = mother;
                        son[found].IsHeterozygous = false;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Male,
                            Traits = son.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });

                        daughter[found] = mother;
                        daughter[found].IsHeterozygous = true;
                        finalFlies.Add(new FlyViewModel()
                        {
                            FlyID = 0,
                            Frequency = 0,
                            Gender = Constants.Female,
                            Traits = daughter.ToList().OrderBy(t => db.Trait.Find(t.Id).CategoryId).ToArray()
                        });
                    }
                }
            }
        }
 */

#endregion

#endregion
