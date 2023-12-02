namespace APPVIEW.ViewModels
{
    public class FilterData
    {
        public List<string> Colors { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> PriceRanges { get; set; }
        public class PriceRange
        {
            public int Min { get; set; }
            public int Max { get; set; }
        }
    }
}
