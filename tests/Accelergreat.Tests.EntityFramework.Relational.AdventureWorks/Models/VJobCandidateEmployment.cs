﻿using System;

#nullable disable

namespace Accelergreat.Tests.EntityFramework.Relational.AdventureWorks.Models
{
    public class VJobCandidateEmployment
    {
        public int JobCandidateId { get; set; }
        public DateTime? EmpStartDate { get; set; }
        public DateTime? EmpEndDate { get; set; }
        public string EmpOrgName { get; set; }
        public string EmpJobTitle { get; set; }
        public string EmpResponsibility { get; set; }
        public string EmpFunctionCategory { get; set; }
        public string EmpIndustryCategory { get; set; }
        public string EmpLocCountryRegion { get; set; }
        public string EmpLocState { get; set; }
        public string EmpLocCity { get; set; }
    }
}
