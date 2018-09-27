using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;

namespace RecipeBox.Controllers
{
public class CategoryController : Controller
{
  [HttpGet("/categories")]
  public ActionResult Index()
  {
    List<Category> allCategory = Category.GetAll();
    return View(allCategory);
  }

  [HttpGet("/categories/new")]
  public ActionResult CreateForm()
  {
    return View();
  }

  [HttpPost("/categories")]
  public ActionResult Create()
  {
    Category newCategory = new Category(Request.Form["category-name"], Request.Form["category-tag"]);
    newCategory.Save();
    return RedirectToAction("Index");
  }

  [HttpGet("/categories/{id}")]
  public ActionResult Details(int id)
  {
    Dictionary<string, object> model = new Dictionary<string, object>();
    Category selectedCategory = Category.Find(id);
    List<Recipe> categoryRecipes = selectedCategory.GetRecipes();
    List<Recipe> allRecipes = Recipe.GetAll();
    model.Add("selectedCategory", selectedCategory);
    model.Add("categoryRecipes", categoryRecipes);
    model.Add("allRecipes", allRecipes);
    return View(model);
  }

  [HttpGet("/categories/{id}/recipes/new")]
  public ActionResult CreateRecipeForm()
  {
    return View("~/Views/Recipes/CreateForm.cshtml");
  }

  [HttpPost("/categories/{categoryId}/recipes/new")]
  public ActionResult AddRecipe(int categoryId)
  {
    Category category = Category.Find(categoryId);
    Recipe recipe = Recipe.Find(Int32.Parse(Request.Form["recipe-id"]));
    category.AddRecipe(recipe);
    return RedirectToAction("Index");
  }

  [HttpGet("/categories/{categoryId}/update")]
  public ActionResult UpdateForm(int categoryId)
  {
     Category thisCategory = Category.Find(categoryId);
     return View(thisCategory);
  }

  [HttpPost("/categories/{categoryId}/update")]
  public ActionResult Update(int categoryId)
  {
    Category thisCategory = Category.Find(categoryId);
    thisCategory.Edit(Request.Form["new-category-name"], Request.Form["new-category-tag"]);
    return RedirectToAction("Index");
  }

  [HttpGet("/categories/{categoryid}/delete")]
  public ActionResult DeleteOne(int categoryId)
  {
    Category thisCategory = Category.Find(categoryId);
    thisCategory.Delete();
    return RedirectToAction("Index");
  }
}
}
