﻿#nullable disable

namespace Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models
{
    public class VStoreWithAddress
    {
        public int BusinessEntityId { get; set; }
        public string Name { get; set; }
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvinceName { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegionName { get; set; }
    }
}
