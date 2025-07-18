﻿using System;

#nullable disable

namespace Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models
{
    public class SalesTerritoryHistory
    {
        public int BusinessEntityId { get; set; }
        public int TerritoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public SalesPerson BusinessEntity { get; set; }
        public SalesTerritory Territory { get; set; }
    }
}
