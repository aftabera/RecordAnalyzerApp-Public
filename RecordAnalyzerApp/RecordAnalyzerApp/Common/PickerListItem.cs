using System;
using System.Collections.Generic;
using System.Text;

namespace RecordAnalyzerApp.Common
{
    public class PickerListItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
