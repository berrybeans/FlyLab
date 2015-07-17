using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Objects.SqlClient;
using FlyLab.Models;
using Newtonsoft.Json;
using CWSToolkit;


namespace FlyLab.Controllers
{
    [AuthorizeUser("Admin")]
    public class AdminController : Controller
    {
        private LabEntityContainer db = new LabEntityContainer();

        public ActionResult Index()
        {
            List<LabUser> allLabs = db.LabUser.ToList();
            List<LabUserViewModel> allLabsVM = new List<LabUserViewModel>();
            foreach (LabUser user in allLabs)
            {
                if (user.Active)
                {
                    LabUserViewModel luvm = new LabUserViewModel();
                    luvm.User = user;
                    UseInstance laststart = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_1 && (t.Active ?? true));
                    UseInstance lastfinish = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_3 && (t.Active ?? true));
                    if (laststart == null || lastfinish == null)
                    {
                        Module dummy = new Module() { ModuleName = Constants.Null_Instance };
                        luvm.Module = dummy;
                        luvm.LastStart = Constants.Null_Instance;
                        luvm.LastFinish = Constants.Null_Instance;
                        luvm.LabsCompleted = 0;
                    }
                    else
                    {
                        luvm.Module = laststart.Module;
                        luvm.LabsCompleted = user.UseInstances.Count(t => t.Stage.ToLower() == Constants.Stage_3 && (t.Active ?? true));
                        luvm.LastStart = laststart.Time.ToString();
                        luvm.LastFinish = DateTime.Compare(lastfinish.Time, laststart.Time) > 0 ? lastfinish.Time.ToString() : Constants.Null_Instance;
                    }
                    allLabsVM.Add(luvm);
                }
            }
            return View(allLabsVM);
        }

        /// <summary>
        /// This region contains all methods relating to trait CRUD.
        /// Note that these methods do not have suffixes like Img and Cat
        /// </summary>
        #region traitmethods
        public ActionResult Traits()
        {
            var trait = db.Trait.Include(t => t.Category);
            ViewBag.Categories = new SelectList(db.Category.ToList(), "Id", "CatName", String.Empty);
            return View(trait.ToList());
        }

        public ActionResult CreateTrait()
        {
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CatName");
            return View("Create");
        }

        //
        // GET: /Trait/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Trait trait = db.Trait.Find(id);
            if (trait == null)
            {
                //catch new trait condition
                ViewBag.CategoryId = new SelectList(db.Category, "Id", "CatName");
                return View("Create");
            }
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CatName", trait.CategoryId);
            return View("Create", trait);
        }

        //
        // POST: /Trait/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Trait trait)
        {
            try
            {
                //new trait
                if (trait.Id == 0 && trait.Name.ToLower() != Constants.Wild.ToLower())
                {
                    if (!trait.IsDominant)
                    {
                        trait.IsIncompleteDominant = false;
                    }
                    db.Trait.Add(trait);
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Traits");
                    }
                    catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                        ViewBag.CategoryId = new SelectList(db.Category, "Id", "CatName", trait.CategoryId);
                        return View("Create", trait);
                    }
                }
                //old trait
                else if (ModelState.IsValid)
                {
                    Trait existing = db.Trait.Find(trait.Id);
                    if (existing.Name.ToLower() == Constants.Wild.ToLower())
                    {
                        //we cant allow anything but imagepath to be changed
                        existing.ImagePath = trait.ImagePath;
                    }
                    else
                    {
                        existing.Name = trait.Name;
                        existing.Category = trait.Category;
                        existing.CategoryId = trait.CategoryId;
                        existing.ChromosomeNumber = trait.ChromosomeNumber;
                        existing.Distance = trait.Distance;
                        existing.Flies = trait.Flies;
                        existing.ImagePath = trait.ImagePath;
                        existing.IsDominant = trait.IsDominant;
                        existing.IsHeterozygous = trait.IsHeterozygous;
                        existing.IsIncompleteDominant = (trait.IsDominant) ? trait.IsIncompleteDominant : false;
                        existing.IsLethal = trait.IsLethal;
                    }
                    db.Entry(existing).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Traits");
                    }
                    catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                        return RedirectToAction("InternalError", "Error");
                    }
                }
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            //failed trait
            ViewBag.CategoryId = new SelectList(db.Category, "Id", "CatName", trait.CategoryId);
            return View("Create", trait);
        }

        //
        // GET: /Trait/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Trait trait = db.Trait.Find(id);
            if (trait == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(trait);
        }

        //
        // POST: /Trait/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trait trait = db.Trait.Find(id);

            foreach (var fly in trait.Flies)
            {
                fly.Traits.Remove(trait);
            }
            db.Trait.Remove(trait);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("InternalError", "Error");
            }
            
            return RedirectToAction("Traits");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult Filter(int catID = 0)
        {
            ViewBag.Categories = db.Category.Select(product => new SelectListItem
            {
                Value = SqlFunctions.StringConvert((double)product.Id),
                Text = product.CatName
            });

            if (catID != 0)
            {
                var trait = db.Trait.Include(t => t.Category).Where(t => t.CategoryId == catID);
                return PartialView("_Filtered", trait.ToList());
            }
            var trait_reset = db.Trait.Include(t => t.Category);
            return PartialView("_Filtered", trait_reset);
        }
        #endregion

        /// <summary>
        /// This region contains all methods relating to category CRUD.
        /// </summary>
        #region categorymethods
        public ActionResult Categories()
        {
            List<Category> cats = db.Category.ToList();
            return View(cats);
        }

        //
        // GET: /Category/Create

        public ActionResult CreateCat(int id = 0)
        {
            Category cat = db.Category.Find(id);

            return View(cat);
        }

        //
        // POST: /Category/Create

        [HttpPost]
        public ActionResult CreateCat(Category category)
        {
            if (category.Id == 0)
            {
                if (db.Category.Count(t => t.CatName.ToLower().Equals(category.CatName.ToLower())) != 0)
                {
                    return RedirectToAction("Categories");
                }

                db.Category.Add(category);
                //we also need a wild trait for this category
                Trait wild = new Trait() {
                    Id = 0, 
                    Category = category, 
                    CategoryId = category.Id,
                    ChromosomeNumber = 0,
                    Distance = 200,
                    IsDominant = false,
                    IsIncompleteDominant = false,
                    IsLethal = false,
                    Name = Constants.Wild,
                    ImagePath = Constants.Wild
                };
                db.Trait.Add(wild);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Categories");
                }
                catch (Exception e)
                {
                    Exception ex = new InvalidOperationException("There was an issue adding category " + category.CatName + ".", e);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return View(category);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Categories");
                }
            }

            return View(category);
        }

        //
        // GET: /Category/Delete/5

        public ActionResult DeleteCat(int id = 0)
        {
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("DeleteCat")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCatConfirmed(int id)
        {
            Category category = db.Category.Find(id);

            //delete all traits in this category to avoid SQL meltdown
            var traits = category.Traits;
            foreach (Trait trait in traits.ToList())
            {
                foreach (Fly fly in trait.Flies)
                {
                    fly.Traits.Remove(trait);
                }
                db.Trait.Remove(trait);
            }

            db.Category.Remove(category);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new Exception("This exception was generated in Admin/DeleteCatConfirmed.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("InternalError", "Error");
            }
            return RedirectToAction("Categories");
        }
        #endregion

        /// <summary>
        /// This region contains all methods relating to module CRUD.
        /// </summary> 
        #region modulemethods

        /// <summary>
        /// Builds a list of modules from the database
        /// </summary>
        /// <returns>A view</returns>
        public ActionResult Modules()
        {
            List<Module> modules = db.Module.ToList();
            return View(modules);
        }

        public ActionResult ActivateModule(int id = 0)
        {
            Module module = db.Module.Find(id);
            try
            {
                module.Active = true;
                db.SaveChanges();
                return RedirectToAction("Modules");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("There was a problem activating module "+id+".", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("InternalError", "Error");
            }
        }


        public ActionResult DeactivateModule(int id = 0)
        {
            Module module = db.Module.Find(id);
            try
            {
                module.Active = false;
                db.SaveChanges();
                return RedirectToAction("Modules");
            }
            catch (Exception e)
            {
                Exception ex = new Exception("There was a problem deactivating module "+id+".", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("InternalError", "Error");
            }
        }
        
        #endregion

        /// <summary>
        /// This region contains all methods relating to gender CRUD.
        /// </summary> 
        #region gendermethods

        /// <summary>
        /// Make and return the view of the Genders DB listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Genders()
        {
            List<Gender> genders = db.Gender.ToList();
            return View(genders);
        }

        /// <summary>
        /// Make and return the view for creating/editing genders
        /// </summary>
        /// <param name="id">Id of the gender to edit, if 0 returns a null Model</param>
        /// <returns></returns>
        public ActionResult CreateGender(int id = 0)
        {
            Gender gender = db.Gender.Find(id);
            return View(gender);
        }

        /// <summary>
        /// Create or edit a gender depending on the ID of the incoming model
        /// </summary>
        /// <param name="gender">The model from the CreateGender POST, will have ID = 0 if a new entry, otherwise edits existing entry</param>
        /// <returns>A view</returns>
        [HttpPost]
        public ActionResult CreateGender(Gender gender)
        {
            if (gender.Id == 0)
            {
                db.Gender.Add(gender);
                try
                {
                    if (db.Gender.ToList().Count <= 2)
                    {
                        db.SaveChanges();                        
                    }
                }
                catch (Exception e)
                {
                    Exception ex = new InvalidOperationException("There was an issue adding gender " + gender.GenderName + ".", e);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return View(gender);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(gender).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Genders");
                }
            }
            return RedirectToAction("Genders");
        }

        /// <summary>
        /// Builds and returns a view containing the model with teh specified ID
        /// </summary>
        /// <param name="id">ID of the gender being confirmed for deletion</param>
        /// <returns>A view</returns>
        public ActionResult DeleteGender(int id = 0)
        {
            Gender gender = db.Gender.Find(id);
            if (gender == null)
            {
                //no gender gonna match that
                Exception e = new InvalidOperationException("Tried to delete a nonexistent gender.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("InternalError", "Error");
            }

            return View(gender);
        }

        /// <summary>
        /// Finds the gender with the specified ID, deletes all flies associated with that gender, then deletes the specified gender
        /// </summary>
        /// <param name="id">ID of the gender being deleted</param>
        /// <returns>A view</returns>
        [HttpPost, ActionName("DeleteGender")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGenderConfirmed(int id = 0)
        {
            Gender gender = db.Gender.Find(id);
            if (gender == null)
            {
                //no gender gonna match that
                Exception e = new InvalidOperationException("Tried to delete a nonexistent gender.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("InternalError", "Error");
            }

            foreach (var fly in gender.Flies.ToList())
            {
                foreach (var trait in fly.Traits.ToList())
                {
                    fly.Traits.Remove(trait);
                }
                db.Fly.Remove(fly);
            }
            db.Gender.Remove(gender);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("There was an issue deleting gender " + gender.GenderName + ".", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("InternalError", "Error");
            }

            return RedirectToAction("Genders");
        }

        #endregion

        /// <summary>
        /// This region contains all methods relating to User CRUD.
        /// </summary>        
        #region usermethods

        /// <summary>
        /// Finds the specified user in the database and returns a partial view of detailed user information.
        /// The partial view is named "_FullInfo.ascx."
        /// </summary>
        /// <param name="id">The ID of the user to display info for</param>
        /// <returns>A partial view if successful, a JSON object {"success": false} otherwise</returns>
        [HttpPost]
        public ActionResult ModalUserInfo(int id = 0)
        {
            LabUser user = db.LabUser.Find(id);
            if (user == null)
            {
                return Json(new { success = false });
            }
            LabUserViewModel luvm = new LabUserViewModel();

            luvm.User = user;
            UseInstance laststart = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_1 && (t.Active ?? true));
            UseInstance lastfinish = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_3 && (t.Active ?? true));
            if (laststart == null || lastfinish == null)
            {
                Module dummyMod = new Module() { 
                    ModuleName = Constants.Null_Instance 
                };
                UseInstance dummyIns = new UseInstance() { 
                    Browser = Constants.Null_Instance, 
                    OS = Constants.Null_Instance, 
                    IP = Constants.Null_Instance 
                };
                luvm.Module = dummyMod;
                luvm.LastStart = Constants.Null_Instance;
                luvm.LastFinish = Constants.Null_Instance;
                luvm.LabsCompleted = 0;
                luvm.lastInstance = dummyIns;
            }
            else
            {
                luvm.Module = laststart.Module;
                luvm.LabsCompleted = user.UseInstances.Count(t => t.Stage.ToLower() == Constants.Stage_3 && (t.Active ?? true));
                luvm.LastStart = laststart.Time.ToString();
                luvm.LastFinish = DateTime.Compare(lastfinish.Time, laststart.Time) > 0 ? lastfinish.Time.ToString() : Constants.Null_Instance;
                luvm.lastInstance = laststart;
            }

            return PartialView("_FullInfo", luvm);
        }

        /// <summary>
        /// The same as ModalUserInfo but with additional information about specific sessions available.
        /// </summary>
        /// <param name="id">The user ID to display info for.</param>
        /// <returns>A view</returns>
        public ActionResult FullUserInfo(int id = 0)
        {
            LabUser user = db.LabUser.Find(id);
            LabUserViewModel luvm = new LabUserViewModel();
            if (user == null)
            {
                Exception e = new Exception("Cannot get full information of non-existent user.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("NotFound", "Error");
            }

            luvm.User = user;
            UseInstance laststart = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_1);
            UseInstance lastfinish = user.UseInstances.LastOrDefault(t => t.Stage.ToLower() == Constants.Stage_3);
            if (laststart == null || lastfinish == null)
            {
                luvm.LastStart = Constants.Null_Instance;
                luvm.LastFinish = Constants.Null_Instance;
            }
            else
            {
                luvm.Module = laststart.Module;
                luvm.LabsCompleted = user.UseInstances.Count(t => t.Stage.ToLower() == Constants.Stage_3);
                luvm.LastStart = laststart.Time.ToString();
                luvm.LastFinish = DateTime.Compare(lastfinish.Time, laststart.Time) > 0 ? lastfinish.Time.ToString() : Constants.Null_Instance;
                luvm.lastInstance = laststart;
            }
            return View(luvm);
        }

        /// <summary>
        /// Grabs specific session info from the database and turns it into an easily readable list of info.
        /// </summary>
        /// <param name="id">The ID of the seession being examined</param>
        /// <returns>A partial view if successful, a JSON object {"success": false} otherwise</returns>
        [HttpPost]
        public ActionResult SessionInfo(int id = 0)
        {
            UseInstance session = db.UseInstance.Find(id);
            if (session == null)
            {
                Exception e = new InvalidOperationException("There is no user session with ID " + id + " in the database.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { success = false }); //check for success flag on js side, if not there display session info table partial
            }
            return PartialView("_SessionInfo", session);
        }

        /// <summary>
        /// Crawls through the database of lab users, deactivating all associated sessions and the user account itself.
        /// </summary>
        /// <returns>A JSON object detailing what the client should do/display</returns>
        [HttpPost]
        public ActionResult PurgeUsers()
        {
            var activeUsers = db.LabUser.Where(t => t.Active == true);

            if (activeUsers == null)
            {
                return Json(new { 
                    success = false, 
                    errormsg = "There are no active records to delete." 
                });
            }

            foreach (LabUser user in activeUsers.ToList())
            {
                foreach (UseInstance ins in user.UseInstances)
                {
                    ins.Active = false;
                }
                user.Active = false;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("There was a problem purging the database.", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new { 
                    success = false, 
                    errormsg = "Internal Server Error (500)" 
                });
            }
            return Json(new { 
                success = true, 
                msg = "Records purged successfully. You will be redirected in <span id=\"num\"></span> second(s)." 
            });
        }

        /// <summary>
        /// Used to deactivate all sessions on a specific user without deactivating the user logs themselves. Not sure how useful this is, but it was easy to add so I did.
        /// </summary>
        /// <param name="id">The ID of the user having his/her records cleared.</param>
        /// <returns>A JSON object</returns>
        [HttpPost]
        public ActionResult ClearRecords(int id = 0)
        {
            LabUser user = db.LabUser.Find(id);
            if (user == null)
            {
                Exception e = new InvalidOperationException("There is no user with id " + id + " to clear records from.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { success = false });
            }

            foreach (UseInstance instance in user.UseInstances.ToList())
            {
                instance.Active = false;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("There was an error purging records of user " + user.GID + ".", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new { success = false });                
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Same as PurgeUsers but for a single LabUser entry. 
        /// </summary>
        /// <param name="id">The ID of the user in question.</param>
        /// <returns>A JSON object</returns>
        [HttpPost]
        public ActionResult RemoveUser(int id = 0)
        {
            LabUser user = db.LabUser.Find(id);
            if (user == null)
            {
                Exception e = new InvalidOperationException("There is no user with id " + id + " to clear records from.");
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { success = false });
            }

            foreach (UseInstance instance in user.UseInstances.ToList())
            {
                instance.Active = false;
            }
            user.Active = false;

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new InvalidOperationException("There was an error purging records of user " + user.GID + ".", e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return Json(new { success = false }); 
            }

            return Json(new { success = true });
        }

        #endregion

        /// <summary>
        /// This region contains all methods relating to image path CRUD.
        /// </summary>
        #region imagemethods

        /// <summary>
        /// Make and return a list of all image path dictionary entries
        /// </summary>
        /// <returns>A view</returns>
        public ActionResult Images()
        {
            List<ImageSettings> images = db.ImageSettings.ToList();
            return View(images);
        }

        /// <summary>
        /// Make and return the data for a sepecific image dictionary entry
        /// </summary>
        /// <param name="id">ID of the entry to edit/create</param>
        /// <returns>A view</returns>
        public ActionResult CreateImg(int id = 0)
        {
            ViewBag.FirstCat = new SelectList(db.Category, "CatName", "CatName");
            ViewBag.SecCat = new SelectList(db.Category, "CatName", "CatName");
            ImageSettings img = db.ImageSettings.Find(id);
            
            return View(img);
        }

        [HttpPost]
        public ActionResult CreateImg(ImageSettings img)
        {
            img.SecCat = (img.SecCat == null) ? "" : img.SecCat;
            if (img.Id == 0)
            {
                //new settings, add em in
                db.ImageSettings.Add(img);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Images");
                }
                catch (Exception e)
                {
                    Exception ex = new InvalidOperationException("There was an issue adding image settings to the database.", e);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    ViewBag.FirstCat = new SelectList(db.Category, "CatName", "CatName");
                    ViewBag.SecCat = new SelectList(db.Category, "CatName", "CatName");
                    return View(img);
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Entry(img).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Images");
                }
            }

            return View(img);
        }

        [HttpPost]
        public ActionResult DeleteImg(int id = 0)
        {
            ImageSettings img = db.ImageSettings.Find(id);
            if (img != null)
            {
                db.ImageSettings.Remove(img);
                try
                {
                    db.SaveChanges();
                    return Json(new { 
                        success = true, 
                        url = "/FlyLab/Admin/Images" 
                    });
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    return Json(new { 
                        success = false, 
                        url = "/FlyLab/Error/InternalError" 
                    });
                }
            }
            else
            {
                return Json(new { 
                    success = false, 
                    url = "/FlyLab/Error/InternalError" 
                });
            }
        }

        #endregion
    }
}
