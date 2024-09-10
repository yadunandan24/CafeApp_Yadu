using System.Net;

namespace CafeApi.Models.DataModels
{
    public class ResponseModel<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }

        public string? Message { get; set; }
    }
}
