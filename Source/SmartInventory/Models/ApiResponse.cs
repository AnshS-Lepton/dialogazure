
namespace Lepton.Entities
{
    public class ApiResponse<T> where T : class
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public T results { get; set; }
    }
}
