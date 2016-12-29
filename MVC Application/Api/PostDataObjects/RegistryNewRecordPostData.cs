﻿using Monei.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monei.MvcApplication.Api.PostDataObjects
{
    public class RegistryNewRecordPostData //: RegistryRecord
    {
        public DateTime Date { get; set; }
        public OperationType Operation { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }        
        public OperationType OperationType { get; set; }
        public string Note { get; set; }
        public bool IsSpecialEvent { get; set; }
        public bool IsTaxDeductible { get; set; }
    }
}