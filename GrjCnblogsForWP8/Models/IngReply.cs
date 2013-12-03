using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class IngReply
    {
        public IngReply(Ing parentIng)
        {
            this.ParentIng = parentIng;
            this.ParentIngId = parentIng.IngId;
        }
        public Ing ParentIng { get; private set; }
        public string ParentIngId { get; private set; }
        public List<string> IngALink { get; set; }

        public string Id { get; set; }
        public string ParentReplyId { get; set; }
        public string CommentAuthorName { get; set; }
        public string CommentAuthorHomeUri { get; set; }
        public string ReplyBody { get; set; }
        public bool CanDelete { get; set; }
        public string ReplyTime { get; set; }
    }
}
