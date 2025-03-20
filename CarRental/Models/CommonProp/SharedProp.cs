namespace CarRental.Models.CommonProp
{
    public class SharedProp
    {
        public bool? IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
