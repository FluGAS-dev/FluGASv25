using System;

namespace FluGASv25.Models.Properties
{
    public class Sequencer
    {
        public static Sequencer MinION { get; internal set; }
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string DATE { get; set; }

    }
}
