using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF.Assignment
{
    public class SuggestDataEntity
    {
        public SuggestDataEntity()
        {
            this.Items = new List<Suggest>();
        }
        public IEnumerable<Suggest> Items { get; set; }
    }

    public class Suggest
    {
        public string Name { get; set; }
        public string NumResult { get; set; }
    }
}
