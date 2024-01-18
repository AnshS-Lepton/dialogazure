using Models.TempUpload;
using NetTopologySuite.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.util;

namespace Models
{
    public class DataImport
    {
        [JsonProperty("adb")]
        public List<ImportData<TempADB>> lstTempADB { get; set; }
        [JsonProperty("antenna")]
        public List<ImportData<TempAntenna>> lstTempAntenna { get; set; }

        [JsonProperty("fdc")]
        public List<ImportData<TempBDB>> lstTempBDB { get; set; }

        [JsonProperty("building")]
        public List<ImportData<TempBuilding>> lstTempBuilding { get; set; }

        [JsonProperty("cabinet")]
        public List<ImportData<TempCabinet>> lstTempCabinet { get; set; }

        [JsonProperty("feedercable")]
        public List<ImportData<TempCable>> lstTempCable { get; set; }

        [JsonProperty("cdb")]
        public List<ImportData<TempCDB>> lstTempCDB { get; set; }

        [JsonProperty("coupler")]
        public List<ImportData<TempCoupler>> lstTempCoupler { get; set; }

        [JsonProperty("duct")]
        public List<ImportData<TempDuct>> lstTempDuct { get; set; }

        [JsonProperty("fat")]
        public List<ImportData<TempFDB>> lstTempFDB { get; set; }

        [JsonProperty("fms")]
        public List<ImportData<TempFMS>> lstTempFMS { get; set; }

        [JsonProperty("htb")]
        public List<TempHTB> lstTempHTB { get; set; }

        [JsonProperty("landbase")]
        public List<TempLandBase> lstTempLandBase { get; set; }

        [JsonProperty("manhole")]
        public List<ImportData<TempManhole>> lstTempManhole { get; set; }

        [JsonProperty("mpod")]
        public List<ImportData<TempMPOD>> lstTempMPOD { get; set; }

        [JsonProperty("ont")]
        public List<ImportData<TempONT>> lstTempONT { get; set; }

        [JsonProperty("patchpanel")]
        public List<ImportData<TempPatchPanel>> lstTempPatchPanel { get; set; }

        [JsonProperty("pop")]
        public List<ImportData<TempPOD>> lstTempPOD { get; set; }

        [JsonProperty("pole")]
        public List<ImportData<TempPole>> lstTempPole { get; set; }

        [JsonProperty("room")]
        public List<ImportData<TempRoom>> lstTempRoom { get; set; }

        [JsonProperty("sector")]
        public List<ImportData<TempSector>> lstTempSector { get; set; }

        [JsonProperty("spliceclosure")]
        public List<ImportData<TempSpliceClosure>> lstTempSpliceClosure { get; set; }

        [JsonProperty("splitter")]
        public List<ImportData<TempSplitter>> lstTempSplitter { get; set; }

        [JsonProperty("structure")]
        public List<ImportData<TempStructure>> lstTempStructure { get; set; }

        [JsonProperty("tower")]
        public List<ImportData<TempTower>> lstTempTower { get; set; }

        [JsonProperty("tree")]
        public List<ImportData<TempTree>> lstTempTree { get; set; }

        [JsonProperty("trench")]
        public List<ImportData<TempTrench>> lstTempTrench { get; set; }

        [JsonProperty("vault")]
        public List<ImportData<TempVault>> lstTempVault { get; set; }

        [JsonProperty("wallmount")]
        public List<ImportData<TempWallMount>> lstTempWallMount { get; set; }
    }

    public class GeoJsonGeometry
    {
        public string type { get; set; }

        //[JsonProperty("coordinates")]
        //public List<List<List<double>>> coordinates { get; set; }
        public List<object> coordinates { get; set; }
    }
    public class ImportData<T>
    {
        public string type { get; set; }
        public int id { get; set; }
        public GeoJsonGeometry geometry { get; set; }
        public T properties { get; set; }
    }

    public class PlanUserList
    {
        public int plan_id { get; set; }
        public string plan_name { get; set; }
        public string plan_type { get; set; }
        public string user_name { get; set; }
    }
    public class Root<T>
    {
        public string type { get; set; }
        public List<Feature<T>> features { get; set; }
    }

    public class GeoJson<T>
    {
        public string type { get; set; }
        public List<Feature<T>> features { get; set; }
    }

    public class Feature<T>
    {
        public string type { get; set; }
        public GeoJsonGeometry geometry { get; set; }
        public int id { get; set; }
        public T properties { get; set; }
    }

    public class EntityFeature<T>
    {
        public string type { get; set; }
        public string entity_type { get; set; }
        public string layer_title { get; set; }
        public GeoJsonGeometry geometry { get; set; }
        public int id { get; set; }
        public T properties { get; set; }
    }

    public class VectorFeatures<T>
    {
        public string layer { get; set; }
        public EntityFeature<T> feature { get; set; }
    }

    public class LayerAttribute
    {
        public string layer_name { get; set; }
        public string layer_title { get; set; }
        public string layer_abbr { get; set; }
        public List<LayerStyle> LayerStyle { get; set; }
    }
    public class LayerStyle
    {
        public string color_code_hex { get; set; }
        public string outline_color_hex { get; set; }
        public string opacity { get; set; }
        public string label_font_size { get; set; }
        public string label_color_hex { get; set; }
        public string label_bg_color_hex { get; set; }
        public string icon_base_path { get; set; }
        public string icon_file_name { get; set; }
        public string line_width { get; set; }
        public string entity_category { get; set; }
        public string entity_sub_category { get; set; }
        public string color_code_alpha_hex { get; set; }
        public string outline_color_code_alpha_hex { get; set; }
        public string label_expression { get; set; }
    }
}
