using inforpatissien_api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Helpers
{
    public class Common
    {
        public static IFPUserData GetSession()
        {
            return (IFPUserData)System.Web.HttpContext.Current.Items["User"];
        }

        public static IFPSettingData SqlDataReaderToSuccess(SqlDataReader _reader)
        {
            IFPSettingData success = new IFPSettingData();
            success.id = Convert.ToInt32(_reader["SCSID"]);
            success.name = Convert.ToString(_reader["SCSNAME"]);
            success.color = Convert.ToString(_reader["SCSCOLOR"]);
            return success;
        }

        public static IFPSettingData SqlDataReaderToDifficulty(SqlDataReader _reader)
        {
            IFPSettingData difficulty = new IFPSettingData();
            difficulty.id = Convert.ToInt32(_reader["DFCID"]);
            difficulty.name = Convert.ToString(_reader["DFCNAME"]);
            difficulty.color = Convert.ToString(_reader["DFCCOLOR"]);
            return difficulty;
        }
    }
}