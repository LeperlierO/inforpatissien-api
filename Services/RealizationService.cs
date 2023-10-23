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
    public class RealizationService
    {
        public static IFPResponseRealizationData GetRealizations(IFPParamPaginationData _param, IFPRealizationFilterData _filter)
        {
            IFPUserData session = Common.GetSession();

            IFPResponseRealizationData response = new IFPResponseRealizationData();
            List<IFPRealizationData> realizations = new List<IFPRealizationData>();
            IFPRealizationData realization = null;

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT *,COUNT(*) OVER() AS TOTAL " +
                                "FROM IFPREALIZATION R " +
                                "INNER JOIN IFPPHOTO P ON R.RLZID = P.RLZID AND P.PHTMAIN = 1 " +
                                "INNER JOIN IFPSUCCESS S ON S.SCSID = R.SCSID ";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            if (_param == null) _param = new IFPParamPaginationData { page = 1 };
            _param.page = Math.Max(1, _param.page);

            response.current = _param.page;

            if(session != null)
            {
                sqlRequest += "WHERE R.USRID = @userId ";
                cmd.Parameters.AddWithValue("@userId", session.id);
            }
            else
            {
                sqlRequest += "WHERE R.USRID = (SELECT USRID FROM IFPUSER WHERE USROWNER=1) ";
            }

            if(_filter != null && !String.IsNullOrEmpty(_filter.search))
            {
                sqlRequest += "AND (" +
                              "RLZCODE COLLATE Latin1_general_CI_AI LIKE @search COLLATE Latin1_general_CI_AI OR " +
                              "RLZNAME COLLATE Latin1_general_CI_AI LIKE @search COLLATE Latin1_general_CI_AI OR " +
                              "RLZDESCRIPTION COLLATE Latin1_general_CI_AI LIKE @search COLLATE Latin1_general_CI_AI OR " +
                              "FORMAT (RLZDATE, 'dd/MM/yyyy') COLLATE Latin1_general_CI_AI LIKE @search COLLATE Latin1_general_CI_AI) ";

                cmd.Parameters.AddWithValue("@search", "%" + _filter.search + "%");
            }

            sqlRequest += "ORDER BY RLZDATE DESC OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";
            cmd.Parameters.AddWithValue("@offset", CommonData.ITEM_PER_PAGE * (_param.page - 1));
            cmd.Parameters.AddWithValue("@limit", CommonData.ITEM_PER_PAGE);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    if (response.size <= 0) response.size = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((double)Convert.ToInt32(areader["TOTAL"]) / (double)CommonData.ITEM_PER_PAGE)));

                    if(realization == null || realization.id != Convert.ToInt32(areader["RLZID"]))
                    {
                        if (realization != null) realizations.Add(realization);

                        realization = SqlDataReaderToRealization(areader);
                        realization.photos = new List<IFPPhotoData>();
                    }

                    realization.photos.Add(SqlDataReaderToPhoto(areader));
                }

                realizations.Add(realization);
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
                realizations.Add(new IFPRealizationData { description = ex.Message });
            }

            response.data = realizations;

            return response;
        }

        public static List<IFPMiniRealizationData> GetMiniRealizations()
        {
            IFPUserData session = Common.GetSession();

            List<IFPMiniRealizationData> realizations = new List<IFPMiniRealizationData>();

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPREALIZATION R " +
                                "INNER JOIN IFPPHOTO P ON R.RLZID = P.RLZID AND P.PHTMAIN = 1 ";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            if (session != null)
            {
                sqlRequest += "WHERE R.USRID = @userId ";
                cmd.Parameters.AddWithValue("@userId", session.id);
            }
            else
            {
                sqlRequest += "WHERE R.USRID = (SELECT USRID FROM IFPUSER WHERE USROWNER=1) ";
            }

            sqlRequest += "ORDER BY RLZDATE DESC ";

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                while (areader.Read())
                {
                    realizations.Add(SqlDataReaderToMiniRealization(areader));
                }

                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return realizations;
        }

        public static IFPRealizationData GetRealization(int _id)
        {
            IFPRealizationData realization = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPREALIZATION R " +
                                "INNER JOIN IFPPHOTO P ON R.RLZID = P.RLZID " +
                                "INNER JOIN IFPSUCCESS S ON S.SCSID = R.SCSID " +
                                "WHERE R.RLZID=@id " +
                                "ORDER BY PHTORDER ";
            
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
                    if(realization == null)
                    {
                        realization = SqlDataReaderToRealization(areader);
                        realization.photos = new List<IFPPhotoData>();
                    }

                    realization.photos.Add(SqlDataReaderToPhoto(areader));
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return realization;
        }

        public static IFPRealizationData CreateRealization(IFPBodyRealizationData _realization)
        {
            IFPUserData session = Common.GetSession();

            IFPRealizationData realization = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "DECLARE @id INT; " +
                                "BEGIN TRANSACTION " +
                                "INSERT INTO IFPREALIZATION VALUES(@code,@name,@description,@date,@top,@time,@cost,@userId); " +
                                "SET @id = SCOPE_IDENTITY(); " +
                                "INSERT INTO IFPPHOTO VALUES(@id,@photoName,@photoDescription,@photoUrl,@photoMain,@photoVertically,@photoHorizontally, '', 0); " +
                                "SELECT * FROM IFPREALIZATION R INNER JOIN IFPPHOTO P ON R.RLZID = P.RLZID INNER JOIN IFPSUCCESS S ON S.SCSID = R.SCSID WHERE R.RLZID = @id; " +
                                "COMMIT TRANSACTION; ";

             SqlCommand cmd = new SqlCommand(String.Empty, connect);

            // IFPREALIZATION

            cmd.Parameters.AddWithValue("code", _realization.code);
            cmd.Parameters.AddWithValue("name", _realization.name);
            cmd.Parameters.AddWithValue("description", _realization.description);
            cmd.Parameters.AddWithValue("date", DateTime.Parse(_realization.date));
            cmd.Parameters.AddWithValue("top", _realization.success.id);
            cmd.Parameters.AddWithValue("time", Convert.ToInt32(_realization.time.TotalMinutes));
            cmd.Parameters.AddWithValue("cost", _realization.cost);
            cmd.Parameters.AddWithValue("userId", session.id);

            // IFPPHOTO
            cmd.Parameters.AddWithValue("photoName", "");
            cmd.Parameters.AddWithValue("photoDescription", "");
            cmd.Parameters.AddWithValue("photoUrl", _realization.mainPhoto);
            cmd.Parameters.AddWithValue("photoMain", 1);
            cmd.Parameters.AddWithValue("photoVertically", 50);
            cmd.Parameters.AddWithValue("photoHorizontally", 50);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                if (areader.Read())
                {
                    realization = SqlDataReaderToRealization(areader);
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return realization;
        }

        public static void SetRealization(int _id, IFPBodyRealizationData _realization)
        {
            IFPUserData session = Common.GetSession();

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "UPDATE IFPREALIZATION SET RLZCODE=@code, RLZNAME=@name, RLZDESCRIPTION=@description, RLZDATE=@date, SCSID=@success WHERE RLZID=@id AND USRID=@user";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            cmd.Parameters.AddWithValue("code", _realization.code);
            cmd.Parameters.AddWithValue("name", _realization.name);
            cmd.Parameters.AddWithValue("description", _realization.description);
            cmd.Parameters.AddWithValue("date", DateTime.Parse(_realization.date));
            cmd.Parameters.AddWithValue("success", _realization.success.id);
            cmd.Parameters.AddWithValue("userId", session.id);
            cmd.Parameters.AddWithValue("id", _id);
            cmd.Parameters.AddWithValue("user", session.id);

            try
            {
                connect.Open();
                cmd.ExecuteNonQuery();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }
        }

        public static void DeleteRealization(int _id)
        {
            IFPUserData session = Common.GetSession();

            IFPRealizationData realization = GetRealization(_id);

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "BEGIN TRANSACTION " +
                                "DELETE FROM IFPREALIZATION WHERE RLZID=@id AND USRID=@usrId; " +
                                "DELETE FROM IFPPHOTO WHERE RLZID=@id AND (SELECT USRID FROM IFPREALIZATION WHERE RLZID=@id)=@usrId; " +
                                "COMMIT TRANSACTION; ";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.Parameters.AddWithValue("@id", _id);
            cmd.Parameters.AddWithValue("@usrId", session.id);

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                int rows = cmd.ExecuteNonQuery();
                // IF 0 SEND ERROR
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            if(realization != null)
            {
                string path = String.Format(ConfigurationManager.AppSettings["config:imageDirectoryPath"].ToString(), session.name.ToLower(), realization.code);
                string allPath = HttpContext.Current.Server.MapPath(String.Format("~/{0}", path));
                if (Directory.Exists(allPath))
                {
                    Directory.Delete(allPath, true);
                }
            }
        }


        public static IFPPhotoData GetRealizationPhoto(int _rlzId, int _order)
        {
            IFPPhotoData photo = null;
            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());

            string sqlRequest = "SELECT * " +
                                "FROM IFPPHOTO P " +
                                "WHERE P.RLZID=@rlzId AND P.PHTORDER=@order";

            SqlCommand cmd = new SqlCommand(String.Empty, connect);

            cmd.Parameters.AddWithValue("@rlzId", _rlzId);
            cmd.Parameters.AddWithValue("@order", _order);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                SqlDataReader areader = cmd.ExecuteReader();
                if (areader.Read())
                {
                    photo = SqlDataReaderToPhoto(areader);
                }
                areader.Close();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return photo;
        }

        public static IFPResponseUploadPhotoData UploadPhoto()
        {
            IFPUserData session = Common.GetSession();
            string realizationCode = HttpContext.Current.Request.Form["realizationCode"];

            //Create the Directory.
            string halfPath = String.Format(ConfigurationManager.AppSettings["config:photoDirectoryPath"].ToString() + "/", session.name.ToLower(), realizationCode);
            string path = HttpContext.Current.Server.MapPath(String.Format("~/{0}", halfPath));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            //Fetch the File Name.
            string fileName = HttpContext.Current.Request.Form["fileName"];

            //Save the File.
            postedFile.SaveAs(path + fileName);

            //Send OK Response to Client.
            return new IFPResponseUploadPhotoData { link = String.Format("https://inforpatissien-api.azurewebsites.net/{0}{1}", halfPath, fileName) };
        }

        public static IFPRealizationData UpsertPhoto(int _rlzId, bool _main, int _order)
        {
            string url = UploadPhoto().link;
            IFPPhotoData photo = GetRealizationPhoto(_rlzId, _order);

            IFPUserData session = Common.GetSession();

            SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECT"].ToString());
            SqlCommand cmd = new SqlCommand(String.Empty, connect);
            string sqlRequest = "";

            if (photo == null)
            {
                sqlRequest = "INSERT INTO IFPPHOTO VALUES(@id,@photoName,@photoDescription,@photoUrl,@photoMain,@photoVertically,@photoHorizontally,'',@photoOrder); ";

                // IFPPHOTO
                cmd.Parameters.AddWithValue("id", _rlzId);
                cmd.Parameters.AddWithValue("photoName", "");
                cmd.Parameters.AddWithValue("photoDescription", "");
                cmd.Parameters.AddWithValue("photoUrl", url);
                cmd.Parameters.AddWithValue("photoMain", _main);
                cmd.Parameters.AddWithValue("photoVertically", 50);
                cmd.Parameters.AddWithValue("photoHorizontally", 50);
                cmd.Parameters.AddWithValue("photoOrder", _order);
            }
            else
            {
                sqlRequest = "UPDATE IFPPHOTO SET PHTURL=@photoUrl WHERE RLZID=@rlzId AND PHTORDER=@order";

                // IFPPHOTO
                cmd.Parameters.AddWithValue("photoUrl", url);
                cmd.Parameters.AddWithValue("rlzId", _rlzId);
                cmd.Parameters.AddWithValue("order", _order);
            }


            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sqlRequest;

            try
            {
                connect.Open();
                cmd.ExecuteNonQuery();
                connect.Close();
            }
            catch (SqlException ex)
            {
                connect.Close();
            }

            return GetRealization(_rlzId);
        }

        public static IFPRealizationData SqlDataReaderToRealization(SqlDataReader _reader)
        {
            IFPRealizationData realization = new IFPRealizationData();
            realization.id = Convert.ToInt32(_reader["RLZID"]);
            realization.code = Convert.ToString(_reader["RLZCODE"]);
            realization.name = Convert.ToString(_reader["RLZNAME"]);
            realization.description = Convert.ToString(_reader["RLZDESCRIPTION"]);
            realization.date = Convert.ToDateTime(_reader["RLZDATE"]);
            realization.success = Common.SqlDataReaderToSuccess(_reader);
            realization.time = TimeSpan.FromMinutes(Convert.ToInt32(_reader["RLZTIME"]));
            realization.cost = Convert.ToInt32(_reader["RLZCOST"]);
            return realization;
        }

        public static IFPMiniRealizationData SqlDataReaderToMiniRealization(SqlDataReader _reader)
        {
            IFPMiniRealizationData realization = new IFPMiniRealizationData();
            realization.id = Convert.ToInt32(_reader["RLZID"]);
            realization.code = Convert.ToString(_reader["RLZCODE"]);
            realization.name = Convert.ToString(_reader["RLZNAME"]);
            realization.description = Convert.ToString(_reader["RLZDESCRIPTION"]);
            realization.mainPhoto = SqlDataReaderToPhoto(_reader);
            return realization;
        }

        public static IFPPhotoData SqlDataReaderToPhoto(SqlDataReader _reader)
        {
            IFPPhotoData photo = new IFPPhotoData();
            photo.id = Convert.ToInt32(_reader["PHTID"]);
            photo.name = Convert.ToString(_reader["PHTNAME"]);
            photo.description = Convert.ToString(_reader["PHTDESCRIPTION"]);
            photo.url = Convert.ToString(_reader["PHTURL"]);
            photo.hiddenUrl = Convert.ToString(_reader["PHTHIDDENURL"]);
            photo.main = Convert.ToBoolean(_reader["PHTMAIN"]);
            photo.position = new IFPPhotoPositionData { horizontally = Convert.ToInt32(_reader["PHTVERTICALLY"]), vertically = Convert.ToInt32(_reader["PHTHORIZONTALLY"]) };
            return photo;
        }

    }
}