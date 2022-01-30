using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLinker.Core.SceneMaker.GlyphParsing
{
    public class ParsingFields
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public int LeftCut { get; set; }

        public int RightCut { get; set; }

        public string SupportedChars { get; set; }
    }
}
