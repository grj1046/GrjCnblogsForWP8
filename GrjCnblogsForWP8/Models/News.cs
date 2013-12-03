using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8.Model
{
    public class News
    {
        /// <summary>
        /// 新闻id
        /// </summary>
        public string NewsId { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 新闻地址
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 投递人
        /// </summary>
        public string Deliver { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public string CommentNum { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public string Views { get; set; }
        /// <summary>
        /// 推荐数
        /// </summary>
        public string Digg { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; }
        /// <summary>
        /// 标题Title
        /// </summary>
        public string TopicTitle { get; set; }
        /// <summary>
        /// 标题Uri
        /// </summary>
        public string TopicUri { get; set; }
        /// <summary>
        /// 标题logo
        /// </summary>
        public string TopicImg { get; set; }
        /// <summary>
        /// 标签title
        /// </summary>
        public string newsTagTitle { get; set; }
        /// <summary>
        /// 标签Uri
        /// </summary>
        public string newsTagUri { get; set; }

    }
}
