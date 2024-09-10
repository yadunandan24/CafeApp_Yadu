namespace CafeApi.Models.TableModels
{
    public class Billmodel
    {
        public int id { get; set; }
        public string? uuid { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? contactNumber { get; set; }
        public string? paymentMethod { get; set; }
        public int? totalAmount { get; set; }
        public string? productDetails { get; set; }
        public string? createdby { get; set; }
    }
}
