using inforpatissien_api.Helpers;
using inforpatissien_api.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Services
{
    public class RecipeService
    {
        public static IFPRecipeData GetRecipe(int _id)
        {
            IFPRecipeData recipe = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT R.*, D.*, S.*, ST.*, " +
                                "(SELECT STRING_AGG(CONVERT(NVARCHAR(max), CONCAT(E.EQPID, '—', E.EQPNAME)), '¯') FROM IFPRECIPEEQUIPMENT RE INNER JOIN IFPEQUIPMENT E ON RE.EQPID = E.EQPID WHERE RE.RCPID = R.RCPID) AS RCPEQUIPMENTS, " +
                                "(SELECT STRING_AGG(CONVERT(NVARCHAR(max), CONCAT(I.IGDID, '—', I.IGDNAME, '—', RI.RCIQUANTITY, '—', U.UNTCODE, '—', I.IGDORDER)), '¯') FROM IFPRECIPEINGREDIENT RI INNER JOIN IFPINGREDIENT I ON RI.IGDID = I.IGDID INNER JOIN IFPUNIT U ON I.UNTID = U.UNTID WHERE RI.RCPID = R.RCPID) AS RCPINGREDIENTS, " +
                                "(SELECT STRING_AGG(CONVERT(NVARCHAR(max), CONCAT(E.EQPID, '—', E.EQPNAME)), '¯') FROM IFPSTEPEQUIPMENT SE INNER JOIN IFPEQUIPMENT E ON SE.EQPID = E.EQPID WHERE SE.STPID = S.STPID) AS STPEQUIPMENTS, " +
                                "(SELECT STRING_AGG(CONVERT(NVARCHAR(max), CONCAT(I.IGDID, '—', I.IGDNAME, '—', SI.STIQUANTITY, '—', U.UNTCODE)), '¯') FROM IFPSTEPINGREDIENT SI INNER JOIN IFPINGREDIENT I ON SI.IGDID = I.IGDID INNER JOIN IFPUNIT U ON I.UNTID = U.UNTID WHERE SI.STPID = S.STPID) AS STPINGREDIENTS " +
                                "FROM IFPRECIPE R " +
                                "INNER JOIN IFPDIFFICULTY D ON R.DFCID = D.DFCID " +
                                "LEFT JOIN IFPSTEP S ON R.RCPID = S.RCPID " +
                                "LEFT JOIN IFPSTEPTYPE ST ON S.STTID = ST.STTID " +
                                "WHERE R.RCPID = @id";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.Parameters.AddWithValue("@id", _id);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    if (recipe == null)
                    {
                        recipe = SqlDataReaderToRecipe(areader);
                    }

                    recipe.steps.Add(SqlDataReaderToStep(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return recipe;
        }

        public static List<IFPMiniRecipeData> GetMiniRecipes()
        {
            IFPUserData session = Common.GetSession();

            List<IFPMiniRecipeData> recipes = new List<IFPMiniRecipeData>();

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPRECIPE R INNER JOIN IFPDIFFICULTY D ON R.DFCID = D.DFCID ORDER BY RCPNAME";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    recipes.Add(SqlDataReaderToMiniRecipe(areader));
                }

                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return recipes;
        }

        public static IFPRecipeData SqlDataReaderToRecipe(SqlDataReader _reader)
        {
            IFPRecipeData recipe = new IFPRecipeData();
            recipe.id = Convert.ToInt32(_reader["RCPID"]);
            recipe.code = Convert.ToString(_reader["RCPCODE"]);
            recipe.name = Convert.ToString(_reader["RCPNAME"]);
            recipe.description = Convert.ToString(_reader["RCPDESCRIPTION"]);
            recipe.mystery = Convert.ToBoolean(_reader["RCPMYSTERY"]);
            recipe.difficulty = Common.SqlDataReaderToDifficulty(_reader);
            recipe.equipments = Convert.ToString(_reader["RCPEQUIPMENTS"]).Split('¯').Where(e => e.Any()).Select(e => new IFPEquipmentData { id = Convert.ToInt32(e.Split('—')[0]), name = e.Split('—')[1] }).ToList();
            recipe.ingredients = Convert.ToString(_reader["RCPINGREDIENTS"]).Split('¯').Where(e => e.Any()).Select(e => new IFPIngredientData { id = Convert.ToInt32(e.Split('—')[0]), name = e.Split('—')[1], quantity = Convert.ToInt32(e.Split('—')[2]), unit = e.Split('—')[3], order = Convert.ToInt32(e.Split('—')[4]) }).OrderBy(i => i.order).ToList();
            recipe.steps = new List<IFPStepData>();

            return recipe;
        }

        public static IFPMiniRecipeData SqlDataReaderToMiniRecipe(SqlDataReader _reader)
        {
            IFPMiniRecipeData recipe = new IFPMiniRecipeData();
            recipe.id = Convert.ToInt32(_reader["RCPID"]);
            recipe.code = Convert.ToString(_reader["RCPCODE"]);
            recipe.name = Convert.ToString(_reader["RCPNAME"]);
            recipe.mystery = Convert.ToBoolean(_reader["RCPMYSTERY"]);
            recipe.difficulty = Common.SqlDataReaderToDifficulty(_reader);
            return recipe;
        }

        public static IFPStepData SqlDataReaderToStep(SqlDataReader _reader)
        {
            IFPStepData step = new IFPStepData();
            step.id = Convert.ToInt32(_reader["STPID"]);
            step.name = Convert.ToString(_reader["STPNAME"]);
            step.description = Convert.ToString(_reader["STPDESCRIPTION"]);
            step.time = TimeSpan.FromMinutes(Convert.ToInt32(_reader["STPTIME"]));
            step.comment = Convert.ToString(_reader["STPCOMMENT"]);
            step.order = Convert.ToInt32(_reader["STPORDER"]);
            step.equipments = Convert.ToString(_reader["STPEQUIPMENTS"]).Split('¯').Where(e => e.Any()).Select(e => new IFPEquipmentData { id = Convert.ToInt32(e.Split('—')[0]), name = e.Split('—')[1] }).ToList();
            step.ingredients = Convert.ToString(_reader["STPINGREDIENTS"]).Split('¯').Where(e => e.Any()).Select(e => new IFPIngredientData { id = Convert.ToInt32(e.Split('—')[0]), name = e.Split('—')[1], quantity = Convert.ToInt32(e.Split('—')[2]), unit = e.Split('—')[3] }).ToList();
            return step;
        }
    }
}