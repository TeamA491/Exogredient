using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace TeamA.Exogredient.DataHelpers.Upload
{
    public class ContinueResponse
    {
        public string Message { get; set; }
        public bool ExceptionOccurred { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
        public double Price { get; set; }
        public string PriceUnit { get; set; }
        public string IngredientName { get; set; }
        public string Category { get; set; }
    }
}
