using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace RecipeBox.Models
{
  public class Category
  {
    private string _name;
    private int _id;
    private string _tag;
    public Category(string name, string tag, int id = 0)
    {
      _name = name;
      _id = id;
      _tag = tag;
    }
    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        return this.GetId().Equals(newCategory.GetId());
      }
    }
    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }
    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }
    public string GetTag()
    {
      return _tag;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category (name, tag) VALUES (@name, @tag);";

      MySqlParameter newName = new MySqlParameter();
      newName.ParameterName = "@name";
      newName.Value = this.GetName();
      cmd.Parameters.Add(newName);

      MySqlParameter newTag = new MySqlParameter();
      newTag.ParameterName = "@tag";
      newTag.Value = this.GetTag();
      cmd.Parameters.Add(newTag);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public static List<Category> GetAll()
    {
      List<Category> allCategorys = new List<Category> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM category;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int CategoryId = rdr.GetInt32(0);
        string CategoryName = rdr.GetString(1);
        string CategoryTag = rdr.GetString(2);
        Category newCategory = new Category(CategoryName, CategoryTag, CategoryId);
        allCategorys.Add(newCategory);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCategorys;
    }
    public static Category Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM category WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int CategoryId = 0;
      string CategoryName = "";
      string CategoryTag = "";

      while(rdr.Read())
      {
        CategoryId = rdr.GetInt32(0);
        CategoryName = rdr.GetString(1);
        CategoryTag = rdr.GetString(2);
      }
      Category newCategory = new Category(CategoryName, CategoryTag, CategoryId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newCategory;
    }
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"TRUNCATE TABLE category;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM category WHERE id = @CategoryId; DELETE FROM category_recipe WHERE category_id = @CategoryId;", conn);
      MySqlParameter categoryIdParameter = new MySqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();

      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Edit(string newName, string newTag)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE category SET name = @newName, tag = @newTag WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      MySqlParameter tag = new MySqlParameter();
      tag.ParameterName = "@newTag";
      tag.Value = newTag;
      cmd.Parameters.Add(tag);

      cmd.ExecuteNonQuery();
      _name = newName;
      _tag =newTag;

      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public void UpdateCategory(string newCategory)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE category SET name = @newCategory WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newCategory";
      name.Value = newCategory;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newCategory;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }

    public void AddRecipe(Recipe newRecipe)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category_recipe (category_id, recipe_id) VALUES (@CategoryId, @RecipeId);";

      MySqlParameter categoryid = new MySqlParameter();
      categoryid.ParameterName = "@CategoryId";
      categoryid.Value = _id;
      cmd.Parameters.Add(categoryid);

      MySqlParameter recipe_id = new MySqlParameter();
      recipe_id.ParameterName = "@RecipeId";
      recipe_id.Value = newRecipe.GetId();
      cmd.Parameters.Add(recipe_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public List<Recipe> GetRecipes()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT recipe.* FROM category
      JOIN category_recipe ON (category.id = category_recipe.category_id)
      JOIN recipe ON (category_recipe.recipe_id = recipe.id)
      WHERE category.id = @CategoryId;";

      MySqlParameter categorydParameter = new MySqlParameter();
      categorydParameter.ParameterName = "@CategoryId";
      categorydParameter.Value = _id;
      cmd.Parameters.Add(categorydParameter);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Recipe> Recipe = new List<Recipe>{};

      while(rdr.Read())
      {
        int recipeId = rdr.GetInt32(0);
        string recipeName = rdr.GetString(1);
        string recipeInstructions = rdr.GetString(2);


        Recipe newRecipe = new Recipe(recipeName, recipeInstructions, recipeId);
        Recipe.Add(newRecipe);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return Recipe;
    }
  }
}
