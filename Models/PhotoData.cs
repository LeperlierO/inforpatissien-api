using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Models
{
    public class IFPPhotoData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string hiddenUrl { get; set; }
        public bool main { get; set; }
        public IFPPhotoPositionData position { get; set; }
    }

    public class IFPPhotoPositionData
    {
        public int vertically { get; set; }
        public int horizontally { get; set; }
    }
}