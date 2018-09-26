using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System;

namespace RecipeBox.Controllers
{
    public class RecipeController : Controller
    {
        [HttpGet("/categories/{categoryId}/recipes/{recipeId}")]
        public ActionResult Details(int categoryId, int recipeId)
        {
          Recipe recipe = Recipe.Find(recipeId);
          Dictionary<string, object> model = new Dictionary<string, object>();
          Category category = Category.Find(categoryId);
          model.Add("recipe", recipe);
          model.Add("category", category);
          return View(recipe);
        }

        [HttpGet("/recipes")]
        public ActionResult Index()
        {
            List<Recipe> allRecipe = Recipe.GetAll();
            return View(allRecipe);
        }

        [HttpGet("/recipes/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/recipes")]
        public ActionResult Create()
        {
            Recipe newRecipe = new Recipe(Request.Form["recipe-name"], Request.Form["recipe-instruction"]);
            newRecipe.Save();
            return RedirectToAction("Index");
        }

        [HttpGet("/recipes/{id}/delete")]
        public ActionResult DeleteOne(int id)
        {
          Recipe thisRecipe = Recipe.Find(id);
          thisRecipe.Delete();
          return RedirectToAction("Index");
        }

        [HttpGet("/recipes/{id}")]
        public ActionResult Details(int id)
        {
            Dictionary<string, object> model = new Dictionary<string, object>();
            Recipe selectedRecipe = Recipe.Find(id);
            List<Category> recipeCategory = selectedRecipe.GetCategory();
            List<Category> allCategory = Category.GetAll();
            model.Add("selectedRecipe", selectedRecipe);
            model.Add("recipeCategory", recipeCategory);
            model.Add("allCategory", allCategory);
            return View(model);

        }
        [HttpGet("/recipes/{id}/update")]
        public ActionResult UpdateForm(int id)
        {
            Recipe thisRecipe = Recipe.Find(id);
            return View(thisRecipe);
        }

        [HttpPost("/recipes/{id}/update")]
        public ActionResult Update(int id)
        {
          Recipe thisRecipe = Recipe.Find(id);
          thisRecipe.Edit(Request.Form["new-name"], Request.Form["new-instruction"]);
          return RedirectToAction("Details", new {id = thisRecipe.GetId()});
        }

        [HttpPost("/recipes/{recipeId}/categories/new")]
        public ActionResult AddCategory(int recipeId)
        {
            Recipe recipe = Recipe.Find(recipeId);
            Category category = Category.Find(Int32.Parse(Request.Form["category-id"]));
            recipe.AddCategory(category);
            return RedirectToAction("Index");
        }
    }
}
