using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GrjCnblogsForWP8
{
    internal class Constants
    {
        /// <summary>
        /// 主页
        /// </summary>
        internal static readonly string MainPageUri = "http://www.cnblogs.com";
        /**************************************************************/
        /// <summary>
        /// 主页文章
        /// </summary>
        internal static readonly string HomeArticleUri = "http://www.cnblogs.com/p{0}/";
        /// <summary>
        /// 精华区
        /// </summary>
        internal static readonly string PickArticleUri = "http://www.cnblogs.com/pick/{0}/";
        /// <summary>
        /// 候选区
        /// </summary>
        internal static readonly string CandidateArticleUri = "http://www.cnblogs.com/candidate/page{0}";
        /// <summary>
        /// 新闻区
        /// </summary>
        internal static readonly string NewsArticleUri = "http://www.cnblogs.com/news/{0}/";
        /// <summary>
        /// 我关注的
        /// </summary>
        internal static readonly string MyFollowingArticleUri = "http://www.cnblogs.com/myfollowing/{0}/";
        /**************************************************************/

        /// <summary>
        /// 最新发布
        /// </summary>
        internal static readonly string NewsLatestArticleUri = "http://news.cnblogs.com/n/page/{0}/";
        /// <summary>
        /// 推荐新闻
        /// </summary>
        internal static readonly string NewsRecommendArticleUri = "http://news.cnblogs.com/n/recommend?page={0}";// "http://home.cnblogs.com/news/recommend/page/{0}/";
        /// <summary>
        /// 热门新闻
        /// Mark: 时间格式 2013/10/28
        /// </summary>
        internal static readonly string NewsHotArticleUri = "http://news.cnblogs.com/n/digg?page={0}&startdate={1}";
        /**************************************************************/
        //博问
        /**************************************************************/
        //闪存 全部
        internal static readonly string IngAllUri = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=all&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 回复我
        /// </summary>
        internal static readonly string IngReplyToMe = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=ReplyToMe&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 提到我
        /// </summary>
        internal static readonly string IngMentionedMe = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=MentionedMe&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 新回应
        /// </summary>
        /// http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=reply&PageIndex=1&PageSize=30&Tag=&_=1383733653318
        internal static readonly string IngNewReply = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=reply&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 我回应
        /// </summary>
        /// http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=myreply&PageIndex=1&PageSize=30&Tag=&_=1383733697197
        internal static readonly string IngMyReply = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=myreply&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 我的
        /// </summary>
        /// http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=my&PageIndex=1&PageSize=30&Tag=&_=1383733754673
        internal static readonly string IngMy = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=my&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 关注
        /// </summary>
        /// http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=recent&PageIndex=1&PageSize=30&Tag=&_=1383733796949
        internal static readonly string IngRecent = "http://home.cnblogs.com/ajax/ing/GetPagedIngList?IngListType=recent&PageIndex={0}&PageSize={1}&t={2}";
        /// <summary>
        /// 发布闪存
        /// </summary>
        internal static readonly string IngPublishIng = "http://home.cnblogs.com/ajax/ing/Publish";
        /// <summary>
        /// 提交闪存回复
        /// </summary>
        internal static readonly string IngReplyUri = "http://home.cnblogs.com/ajax/ing/PostComment";

        /**************************************************************/

        internal static readonly string strHtmlSkeleton = "<html><head><meta http-equiv=\"Content-type\" content=\"text/html;charset=utf-8\" /><meta id=\"viewport\" name=\"viewport\" content=\"width=device-width,initial-scale=1.0,minimum-scale=0.5,maximum-scale=2.0,user-scalable=yes\"/> </head><body style=\"margin-bottom: 50px;\"><div><h1>{0}</h1><hr /></div>{1}</body></html>";

        /// <summary>
        /// 登录
        /// </summary>
        internal static readonly string LoginUri = "http://m.cnblogs.com/mobileLoginPost.aspx";
        /// <summary>
        /// 我关注的人
        /// </summary>
        internal static readonly string FolloweesUri = "http://home.cnblogs.com/followees/";
        /// <summary>
        /// 我的粉丝
        /// </summary>
        internal static readonly string FollowersUri = "http://home.cnblogs.com/followers/";
        internal static readonly string UserAgent = "WP8 Client by grjsoft.com";
        internal static readonly CookieContainer CookieContainer = new CookieContainer();
    }
}
