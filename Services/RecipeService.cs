using inforpatissien_api.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Services
{
    public class RecipeService
    {
        public static List<IPRecipeData> GetRecipes()
        {
            List<IPRecipeData> recipes = new List<IPRecipeData>();
            MySqlConnection connect = new MySqlConnection(ConfigurationManager.ConnectionStrings["InforpatissienConnectionString"].ToString());

            string sqlRequest = "SELECT * FROM IPRECIPE";

            MySqlCommand cmd = new MySqlCommand(sqlRequest, connect);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;
            try
            {
                connect.Open();
                // On exécute notre requête
                MySqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    recipes.Add(OleDbDataReaderToRecipe(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (OleDbException ex)
            {
                connect.Close();
            }

            return recipes;
        }

        public static IPRecipeData OleDbDataReaderToRecipe(MySqlDataReader _reader)
        {
            IPRecipeData recipe = new IPRecipeData();
            recipe.id = Convert.ToInt32(_reader["RCPID"]);
            recipe.name = Convert.ToString(_reader["RCPNAME"]);
            recipe.description = Convert.ToString(_reader["RCPDESCRIPTION"]);
            return recipe;
        }

    }
}