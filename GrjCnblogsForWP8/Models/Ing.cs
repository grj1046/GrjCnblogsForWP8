using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class Ing
    {
        public string IngId { get; set; }
        public string IngBody { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string HomePageUri { get; set; }
        public string BlogUri { get; set; }
        public string BloggerAvatarImgUri { get; set; }
        public string IngDetailUri { get; set; }
        public string PublishTime { get; set; }
        public string MaxIngId { get; set; }//只有第一条闪有这个属性，不知道是干嘛的。
        public bool CanDelete { get; set; }
        public string IsIngLucky { get; set; }//★
        public string IsNewbie { get; set; }//❀ ♠ ♣ ❀★
        public string IsPrivate { get; set; }//私
        public string Script { get; set; }
        //487984 为Ing id
        //<script type="text/javascript">$(function(){ GetIngComments(487984,true);});</script>
        public bool NeedGetIngReply { get; set; }
        public string IngReplyCount { get; set; }

        public ObservableCollection<IngReply> IngReply { get; set; }
    }
}
