using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace inforpatissien_api.Models
{
    public class IFPRecipeData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public bool mystery { get; set; }
        public IFPSettingData difficulty { get; set; }
        public List<IFPStepData> steps { get; set; }
        public List<IFPIngredientData> ingredients { get; set; }
        public List<IFPEquipmentData> equipments { get; set; }
    }

    public class IFPMiniRecipeData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public bool mystery { get; set; }
        public IFPSettingData difficulty { get; set; }
    }

    public class IFPStepData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public TimeSpan time { get; set; }
        public string comment { get; set; }
        public IFPSettingData type { get; set; }
        public List<IFPIngredientData> ingredients { get; set; }
        public List<IFPEquipmentData> equipments { get; set; }
        public IFPPhotoData photo { get; set; }
        public int order { get; set; }
    }

    public class IFPIngredientData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public string unit { get; set; }
        [JsonIgnore]
        public int order { get; set; }
    }

    public class IFPEquipmentData
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}