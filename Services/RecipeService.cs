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
        public static IPResponseRecipeData GetRecipes(IPParamPaginationData _param)
        {
            IPResponseRecipeData response = new IPResponseRecipeData();
            List<IPMiniRecipeData> recipes = new List<IPMiniRecipeData>();
            MySqlConnection connect = new MySqlConnection(ConfigurationManager.ConnectionStrings["InforpatissienConnectionString"].ToString());

            string sqlRequest = "SELECT *,COUNT(*) OVER() AS TOTAL " +
                                "FROM IPRECIPE R " +
                                "INNER JOIN IPRECIPEPHOTO RP ON R.RCPID = RP.RCPID AND RP.RPOMAIN = 1 " +
                                "ORDER BY RCPDATE DESC ";

            MySqlCommand cmd = new MySqlCommand(String.Empty, connect);

            if (_param == null) _param = new IPParamPaginationData { page = 1 };
            _param.page = Math.Max(1, _param.page);

            response.current = _param.page;

            sqlRequest += "LIMIT ? OFFSET ?";
            cmd.Parameters.AddWithValue("limit", Common.ITEM_PER_PAGE);
            cmd.Parameters.AddWithValue("offset", Common.ITEM_PER_PAGE * (_param.page - 1));

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                MySqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    if (response.size <= 0) response.size = Convert.ToInt32(areader["TOTAL"]) / Common.ITEM_PER_PAGE;

                    recipes.Add(SqlDataReaderToMiniRecipe(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (MySqlException ex)
            {
                connect.Close();
            }

            response.data = recipes;

            return response;
        }

        public static IPMiniRecipeData SqlDataReaderToMiniRecipe(MySqlDataReader _reader)
        {
            IPMiniRecipeData recipe = new IPMiniRecipeData();
            recipe.id = Convert.ToInt32(_reader["RCPID"]);
            recipe.name = Convert.ToString(_reader["RCPNAME"]);
            recipe.description = Convert.ToString(_reader["RCPDESCRIPTION"]);
            recipe.mainPhoto = SqlDataReaderToPhoto(_reader);
            return recipe;
        }

        public static IPRecipePhotoData SqlDataReaderToPhoto(MySqlDataReader _reader)
        {
            IPRecipePhotoData photo = new IPRecipePhotoData();
            photo.id = Convert.ToInt32(_reader["RPOID"]);
            photo.name = Convert.ToString(_reader["RPONAME"]);
            photo.description = Convert.ToString(_reader["RPODESCRIPTION"]);
            photo.url = Convert.ToString(_reader["RPOURL"]);
            photo.main = Convert.ToBoolean(_reader["RPOMAIN"]);
            photo.position = new IPRecipePhotoPositionData { horizontally = Convert.ToInt32(_reader["RPOVERTICALLY"]), vertically = Convert.ToInt32(_reader["RPOHORIZONTALLY"]) };
            return photo;
        }

    }
}