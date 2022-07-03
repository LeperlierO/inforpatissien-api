using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Models
{
    public static class Common
    {
        public static int ITEM_PER_PAGE = 8;
    }

    public class IPParamPaginationData
    {
        public int page { get; set; }
    }

    public class IPResponsePaginationData
    {
        public int current { get; set; }
        public int size { get; set; }
    }
}