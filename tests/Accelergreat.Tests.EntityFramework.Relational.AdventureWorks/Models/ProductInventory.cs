﻿using System;

#nullable disable

namespace Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models
{
    public class ProductInventory
    {
        public int ProductId { get; set; }
        public short LocationId { get; set; }
        public string Shelf { get; set; }
        public byte Bin { get; set; }
        public short Quantity { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Location Location { get; set; }
        public Product Product { get; set; }
    }
}
