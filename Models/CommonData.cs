using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Models
{
    public static class CommonData
    {
        public static int ITEM_PER_PAGE = 8;
    }

    public class IFPIntId
    {
        public int id { get; set; }
    }

    public class IFPSettingData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
    }

    public class IFPParamPaginationData
    {
        public int page { get; set; }
    }

    public class IFPResponsePaginationData
    {
        public int current { get; set; }
        public int size { get; set; }
    }

}