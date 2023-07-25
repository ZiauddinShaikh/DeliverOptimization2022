﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_Update3Month
    {
        [Key]
        public int Id { get; set; }
        public string? PeopleManagerEmail { get; set; }
        public string? Month { get; set; }
        public string? PeopleManager { get; set; }
        public string? RevenueRegion { get; set; }
        public string? ServiceLine { get; set; }
        public int FullCount { get; set; }
        public string? UpdatedCount { get; set; }
    }
}
