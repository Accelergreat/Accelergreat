﻿using System;

#nullable disable

namespace Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models
{
    public class VEmployeeDepartment
    {
        public int BusinessEntityId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
