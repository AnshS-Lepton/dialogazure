namespace Models
{
    public class NearByEntitiesIn
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int bufferInMtrs { get; set; }
        public string entity_name { get; set; }
        public int userId { get; set; }
        public int ticket_id { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_type { get; set; }
    }
    public class NearByEntitiesByType
    {
        public int c_system_id { get; set; }
        public string c_entity_type { get; set; }
        public int bufferInMtrs { get; set; }
        public string search_entity_type { get; set; }
    }
}
