﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Models
{
    public class SaleDetailDbModel
    {
        //public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Tax { get; set; }
        public int Quantity { get; set; }
    }
}
