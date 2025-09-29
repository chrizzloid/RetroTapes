namespace RetroTapes.ViewModels
{
    public class InventoryEditVm
    {
        public int RentalId { get; set; }
        public string CustomerName { get; set; } = "";
        public string StaffName { get; set; } = "";
        public byte StoreId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOverdue => DateTime.Now > DueDate;
    }
}
