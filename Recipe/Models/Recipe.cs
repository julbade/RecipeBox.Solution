using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace RecipeBox.Models
{
  public class Recipe
  {
    private string _name;
    private string _instruction;
    private int _rate;
    private int _id;


    public Recipe(string name, string instruction, int rate, int id = 0)
    {
      _name = name;
      _id = id;
      _instruction = instruction;
      _rate = rate;
    }

    public override bool Equals(System.Object otherRecipe)
    {
      if (!(otherRecipe is Recipe))
      {
        return false;
      }
      else
      {
        Recipe newRecipe = (Recipe) otherRecipe;
        bool idEquality = this.GetId() == newRecipe.GetId();
        bool nameEquality = this.GetName() == newRecipe.GetName();
        return (idEquality && nameEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetInstruction()
    {
      return _instruction;
    }
    public int GetRate()
    {
      return _rate;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO recipe (name, instruction, rate) VALUES (@name, @instruction, @rate);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter instruction = new MySqlParameter();
      instruction.ParameterName = "@instruction";
      instruction.Value = this._instruction;
      cmd.Parameters.Add(instruction);

      MySqlParameter newRate = new MySqlParameter();
      newRate.ParameterName = "@rate";
      newRate.Value = this.GetRate();
      cmd.Parameters.Add(newRate);


      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Recipe> GetAll()
    {
      List<Recipe> allRecipes = new List<Recipe> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM recipe;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int recipeId = rdr.GetInt32(0);
        string recipeName = rdr.GetString(1);
        string recipeInstructions = rdr.GetString(2);
        int recipeRate = rdr.GetInt32(3);

        Recipe newRecipe = new Recipe(recipeName, recipeInstructions, recipeRate, recipeId);
        allRecipes.Add(newRecipe);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allRecipes;
    }

    public static Recipe Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM recipe WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int recipeId = 0;
      string recipeName = "";
      string recipeInstructions = "";
      int recipeRate = 0;

      rdr.Read();
      recipeId = rdr.GetInt32(0);
      recipeName = rdr.GetString(1);
      recipeInstructions = rdr.GetString(2);
      recipeRate = rdr.GetInt32(3);

      Recipe newRecipe = new Recipe(recipeName, recipeInstructions, recipeRate, recipeId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

      return newRecipe;
    }

    public void UpdateName(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE recipe SET name = @newName WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      _name = newName;
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
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM recipe WHERE id = @RecipeId; DELETE FROM category_recipe WHERE recipe_id = @RecipeId;";

      MySqlParameter recipeIdParameter = new MySqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this.GetId();
      cmd.Parameters.Add(recipeIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM recipe;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
    public void AddCategory(Category newCategory)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO category_recipe (category_id, recipe_id) VALUES (@CategoryId, @RecipeId);";

      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = newCategory.GetId();
      cmd.Parameters.Add(category_id);

      MySqlParameter recipe_id = new MySqlParameter();
      recipe_id.ParameterName = "@RecipeId";
      recipe_id.Value = _id;
      cmd.Parameters.Add(recipe_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }


    public List<Category> GetCategory()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT category_id FROM category_recipe WHERE recipe_id = @recipeId;";

      MySqlParameter recipeIdParameter = new MySqlParameter();
      recipeIdParameter.ParameterName = "@recipeId";
      recipeIdParameter.Value = _id;
      cmd.Parameters.Add(recipeIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> categoryIds = new List<int> {};
      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        categoryIds.Add(categoryId);
      }
      rdr.Dispose();

      List<Category> category = new List<Category> {};
      foreach (int categoryId in categoryIds)
      {
        var categoryQuery = conn.CreateCommand() as MySqlCommand;
        categoryQuery.CommandText = @"SELECT * FROM category WHERE id = @CategoryId;";

        MySqlParameter categoryIdParameter = new MySqlParameter();
        categoryIdParameter.ParameterName = "@CategoryId";
        categoryIdParameter.Value = categoryId;
        categoryQuery.Parameters.Add(categoryIdParameter);

        var categoryQueryRdr = categoryQuery.ExecuteReader() as MySqlDataReader;
        while(categoryQueryRdr.Read())
        {
          int thisCategoryId = categoryQueryRdr.GetInt32(0);
          string categoryName = categoryQueryRdr.GetString(1);

          Category foundCategory = new Category(categoryName, thisCategoryId);
          category.Add(foundCategory);
        }
        categoryQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return category;
    }
    public void Edit(string newName, string newInstructions,  int newRate)
   {
     MySqlConnection conn = DB.Connection();
     conn.Open();
     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"UPDATE recipe SET name = @newName, instruction = @newInstructions, rate = @newRate WHERE id = @searchId;";

     MySqlParameter searchId = new MySqlParameter();
     searchId.ParameterName = "@searchId";
     searchId.Value = _id;
     cmd.Parameters.Add(searchId);

     MySqlParameter name = new MySqlParameter();
     name.ParameterName = "@newName";
     name.Value = newName;
     cmd.Parameters.Add(name);

     MySqlParameter instruction = new MySqlParameter();
     instruction.ParameterName = "@newInstructions";
     instruction.Value = newInstructions;
     cmd.Parameters.Add(instruction);

     MySqlParameter rate = new MySqlParameter();
     rate.ParameterName = "@newRate";
     rate.Value = newRate;
     cmd.Parameters.Add(rate);


     cmd.ExecuteNonQuery();
     _name = newName;
     _instruction = newInstructions;
     _rate = newRate;

     conn.Close();
     if (conn != null)
     {
       conn.Dispose();
     }
   }
  }
}
