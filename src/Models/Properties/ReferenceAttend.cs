namespace FluGASv25.Models.Properties
{
    public class ReferenceAttend: Reference
    {
        public string UserFasta { get; set; }
        public bool IsStartEndInfluA { get; set; }
        public bool IsStartEndInfluB { get; set; }
        public bool IsAcceptNCountInfluA { get; set; }
        public bool IsAcceptNCountInfluB { get; set; }
        public bool IsUpdateFirstMappingReference { get; set; }
    }
}
