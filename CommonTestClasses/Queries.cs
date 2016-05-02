using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestClasses
{
    public class Metadata
    {
        public String Key { get; set; }

        public String Value { get; set; }

    }

    public class ClassWithMetadata
    {
        public String Id { get; set; }

        public Dictionary<String, Metadata> Metadata { get; set; }
    }
}
