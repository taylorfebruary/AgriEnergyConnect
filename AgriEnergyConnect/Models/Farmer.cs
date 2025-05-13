﻿namespace AgriEnergyConnect.Models
{
    public class Farmer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
