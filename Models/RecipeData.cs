using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inforpatissien_api.Models
{

    public enum IPRecipeSuccess : int
    {
        RSFlop,
        RSTop
    }

    public enum IPRecipeDifficulty : int
    {
        RDEasy,
        RDMedium,
        RDDifficult
    }

    public class IPRecipeData
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public IPRecipeSuccess success { get; set; }
        public IPRecipeDifficulty difficulty { get; set; }
        public TimeSpan time { get; set; }
        public int cost { get; set; }
        public List<IPRecipePhotoData> photos { get; set; }
    }

    public class IPMiniRecipeData
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public IPRecipePhotoData mainPhoto { get; set; }
    }

    public class IPRecipePhotoData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public bool main { get; set; }
        public IPRecipePhotoPositionData position { get; set; }
    }

    public class IPRecipePhotoPositionData
    {
        public int vertically { get; set; }
        public int horizontally { get; set; }
    }

    public class IPResponseRecipeData : IPResponsePaginationData
    {
        public List<IPMiniRecipeData> data { get; set; }
    }
}