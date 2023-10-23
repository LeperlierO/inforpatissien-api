using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace inforpatissien_api.Models
{

    public class IFPRealizationData
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public IFPSettingData success { get; set; }
        public TimeSpan time { get; set; }
        public int cost { get; set; }
        public string source { get; set; }
        public string notes { get; set; }
        public List<IFPPhotoData> photos { get; set; }
    }

    public class IFPBodyRealizationData
    {
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public IFPIntId success { get; set; }
        public TimeSpan time { get; set; }
        public int cost { get; set; }
        public string mainPhoto { get; set; }
    }

    public class IFPBodyRealizationAdditionalsData
    {
        public string source { get; set; }
        public string notes { get; set; }
    }

    public class IFPMiniRealizationData
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IFPPhotoData mainPhoto { get; set; }
    }

    public class IFPRealizationFilterData
    {
        public string search { get; set; }
    }

    public class IFPResponseRealizationData : IFPResponsePaginationData
    {
        public List<IFPRealizationData> data { get; set; }
    }

    public class IFPResponseUploadPhotoData
    {
        public string link { get; set; }
    }
}