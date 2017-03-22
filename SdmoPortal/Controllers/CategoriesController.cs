using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SdmoPortal.DataLayer;
using SdmoPortal.Models;
using SdmoPortal.ViewModels;
using System.Data.Entity.Infrastructure;

namespace SdmoPortal.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext _applicationDbContext = new ApplicationDbContext();

        private List<Category> GetListOfNodes()
        {
            List<Category> sourceCategories = _applicationDbContext.Categories.ToList();
            List<Category> categories = new List<Category>();
            foreach (Category sourceCategory in sourceCategories)
            {
                Category c = new Category();
                c.Id = sourceCategory.Id;
                c.CategoryName = sourceCategory.CategoryName;
                if(sourceCategory.ParentCategoryId != null)
                {
                    c.Parent = new Category();
                    c.Parent.Id = (int)sourceCategory.ParentCategoryId;
                }
                categories.Add(c);
            }
            return categories;
        }

        private string EnumerateNodes(Category parent)
        {
            string content = String.Empty;
            // Add <li> category name
            content += "<li class\"treenode\">";
            content += parent.CategoryName;
            content += String.Format("<a href=\"/Categories/Edit/{0}\" class=\"btn btn-primary btn-xs treenodeeditbutton\">Edit</a>", parent.Id);
            content += String.Format("<a href=\"/Categories/Delete/{0}\" class=\"btn btn-danger btn-xs treenodedeletebutton\">Delete</a>", parent.Id);
            // if there no children end <li>
            if (parent.Children.Count == 0)
                content += "</li>";
            else // if there are children start a <ul>
                content += "<ul>";

            // Loop one past the number of children
            int numberOfChildren = parent.Children.Count;
            for(int i = 0; i <= numberOfChildren; i++)
            {
                // If this iteration children points to a child then called this function recursively
                if(numberOfChildren > 0 && i < numberOfChildren)
                {
                    Category child = parent.Children[i];
                    content += EnumerateNodes(child);
                }
                // If this iteration's index points past the children, end the <ul>
                if (numberOfChildren > 0 && i == numberOfChildren)
                    content += "</ul>";
            }
            return content;
        }
        //TODO: Add new validation
        private void ValidateParentsAreParentless(Category category)
        {
            //There is no parent
            if (category.ParentCategoryId == null)
                return;

            //The parent has a parent
            Category parentCategory = _applicationDbContext.Categories.Find(category.ParentCategoryId);
            if (parentCategory.ParentCategoryId != null)
                throw new InvalidOperationException("You can not nest this category more than two levels deep");

            //The parent does NOT have a parent but the category being nested has children
            int numberOfChildren = _applicationDbContext.Categories.Count(c => c.ParentCategoryId == category.ParentCategoryId);
            if (numberOfChildren > 0)
                throw new InvalidOperationException("You can NOT nest this category's children more than two levels deep");
        }

        private SelectList PopulateParentCategorySelectList(int? id)
        {
            SelectList selectList;
            if (id == null)
                selectList = new SelectList(_applicationDbContext.Categories.Where(c => c.ParentCategoryId == null), "Id", "CategoryName");
            else if (_applicationDbContext.Categories.Count(c => c.ParentCategoryId == id) == 0)
                selectList = new SelectList(_applicationDbContext.Categories.Where(c=>c.ParentCategoryId == null && c.Id != id), "Id", "CategoryName");
            else 
                selectList = new SelectList(_applicationDbContext.Categories.Where(c => false), "Id", "CategoryName");

            return selectList;
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            string fullString = "<ul>";
            IList<Category> listOfNodes = GetListOfNodes();
            IList<Category> topLevelCategories = TreeUtility.TreeHelper.ConvertToForest(listOfNodes);
            foreach(var category in topLevelCategories)
            {
                fullString += EnumerateNodes(category);
            }
            fullString += "</ul>";
            return View((object)fullString);
            //return View(await _applicationDbContext.Categories.ToListAsync());
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            ViewBag.ParentCategoryIdSelectList = PopulateParentCategorySelectList(null); //new SelectList(_applicationDbContext.Categories, "Id", "CategoryName" );
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ParentCategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ValidateParentsAreParentless(category);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.ParentCategoryIdSelectList = PopulateParentCategorySelectList(null); //new SelectList(_applicationDbContext.Categories, "Id", "CategoryName");
                    return View("Edit", category);

                }
                _applicationDbContext.Categories.Add(category);
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await _applicationDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            //TODO: Wind-up a Category view model
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.Id = category.Id;
            categoryViewModel.ParentCategoryId = category.ParentCategoryId;
            categoryViewModel.CategoryName = category.CategoryName;

            ViewBag.ParentCategoryIdSelectList = PopulateParentCategorySelectList(categoryViewModel.Id); //new SelectList(_applicationDbContext.Categories, "Id", "CategoryName");
            return View(categoryViewModel);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ParentCategoryId,CategoryName")] CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                //TODO: Unwind back to category inside a try-catch to trap the exception "cannot be its own parent" rule.

                Category editedCategory = new Category();
                try
                {
                    editedCategory.Id = categoryViewModel.Id;
                    editedCategory.ParentCategoryId = categoryViewModel.ParentCategoryId;
                    editedCategory.CategoryName = categoryViewModel.CategoryName;
                    ValidateParentsAreParentless(editedCategory);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ViewBag.ParentCategoryIdSelectList = PopulateParentCategorySelectList(categoryViewModel.Id); //new SelectList(_applicationDbContext.Categories, "Id", "CategoryName");
                    return View("Edit", categoryViewModel);
                }
                _applicationDbContext.Entry(editedCategory).State = EntityState.Modified;
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategoryIdSelectList = PopulateParentCategorySelectList(categoryViewModel.Id); //new SelectList(_applicationDbContext.Categories, "Id", "CategoryName");
            return View(categoryViewModel);
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await _applicationDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Category category = await _applicationDbContext.Categories.FindAsync(id);
            try
            {
                _applicationDbContext.Categories.Remove(category);
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "You attempted to delete a category that had child categories.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Delete", category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _applicationDbContext.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
