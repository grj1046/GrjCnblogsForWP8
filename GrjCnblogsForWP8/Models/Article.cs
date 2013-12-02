using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class Article
    {
        public string BlogerName { get; set; }
        public string BlogAddress { get; set; }
        public string FaceImage { get; set; }
        public string ArticleUri { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string PublishTime { get; set; }
        public string ArticleComment { get; set; }
        public string ArticleView { get; set; }
        public string Digg { get; set; }
        public string PostItemFooter { get { return PublishTime + ArticleComment + ArticleView; } }
        //public ArticleType ArticleType { get; set; }
    }

    //enum ArticleType
    //{
    //    Essay,
    //    News
    //}
}
