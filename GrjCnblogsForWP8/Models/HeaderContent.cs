using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class HeaderContent
    {
        public Article EditorRecommend { get; set; }
        public Article MaximumRecommend { get; set; }
        public Article MaximumComment { get; set; }
        public Article TopNews { get; set; }
        public Article RecommendNews { get; set; }
    }
}
