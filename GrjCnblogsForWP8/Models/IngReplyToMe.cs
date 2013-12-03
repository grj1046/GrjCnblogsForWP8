using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class IngReplyToMe
    {
        public string CommentId { get; set; }
        public string MaxCommentId { get; set; }
        public string IngDetailUri { get; set; }
        public string BloggerAvatarImgUri { get; set; }
        public string ReplyAuthorName { get; set; }
        public string ReplyAuthorHomeUri { get; set; }
        /// <summary>
        /// 被评论的回复
        /// </summary>
        public string CommentBody { get; set; }
        /// <summary>
        /// 评论的内容
        /// </summary>
        public string ReplyBody { get; set; }
        public List<string> ReplyBodyUrls { get; set; }
        public string ReplyTime { get; set; }
    }
}
