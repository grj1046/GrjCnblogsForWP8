using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class IngDetail
    {
        /// <summary>
        /// 闪存ID
        /// </summary>
        /// <ul id=comment_block_488300>
        public string IngId { get; set; }
        /// <summary>
        /// 发布者Home页
        /// </summary>
        public string AuthorHomeUri { get; set; }
        /// <summary>
        /// 发布者闪存主页
        /// </summary>
        public string AuthorIngUri { get; set; }
        /// <summary>
        /// 发布者头像URI
        /// </summary>
        public string AuthorAvatarImgUri { get; set; }
        /// <summary>
        /// 发布者名称
        /// </summary>
        public string AuthorName { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; }
        /// <summary>
        /// 闪存内容
        /// </summary>
        public string IngContent { get; set; }
        //收藏按钮是否启用
        public string IngCommentCount { get; set; }
        /// <summary>
        /// 是否幸运闪
        /// </summary>
        public string IsLucky { get; set; }
        /// <summary>
        /// 是否新人闪
        /// </summary>
        public string IsNewbie { get; set; }
        /// <summary>
        /// 是否可删除
        /// </summary>
        public bool CanDelete { get; set; }
        public List<string> Links { get; set; }
        public ObservableCollection<IngReplyDetail> IngReplyDetail { get; set; }
    }

    public class IngReplyDetail
    {
        /// <summary>
        /// 当前回复的闪存ID
        /// </summary>
        public string CurrentReplyId { get; set; }
        /// <summary>
        /// 被回复的闪存ID
        /// </summary>
        public string PreviousReplyId { get; set; }
        /// <summary>
        /// 回复者头像URI
        /// </summary>
        public string AuthorAvatarImgUri { get; set; }
        /// <summary>
        /// 回复者的闪存主页
        /// </summary>
        public string AuthorIngUri { get; set; }
        /// <summary>
        /// 回复者昵称
        /// </summary>
        public string AuthorName { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public string ReplyTime { get; set; }
        ///// <summary>
        ///// 是否可删除
        ///// </summary>
        //public bool CanDelete { get; set; }
        /// <summary>
        /// 是否可回复
        /// </summary>
        public bool CanReply { get; set; }
    }
}
