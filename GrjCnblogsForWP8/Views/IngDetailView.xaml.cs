using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GrjCnblogsForWP8.Model;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;

namespace GrjCnblogsForWP8.Views
{
    public partial class IngDetailView : PhoneApplicationPage, INotifyPropertyChanged
    {
        HttpWebRequest request;
        HttpWebRequest postRequest;

        private string ingDetailUri;

        private IngDetail ingDetails;
        public IngDetail IngDetails
        {
            get { return ingDetails; }
            set
            {
                ingDetails = value;
                NotifyPropertyChanged("IngDetails");
            }
        }

        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        public IngDetailView()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (this.borderReplyForm.Visibility == System.Windows.Visibility.Visible)
            {
                this.borderReplyForm.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
            if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
            {
                var dialogResult = MessageBox.Show("您尚未登录，请先登录。", "提示", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                    this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                else
                    if (this.NavigationService.CanGoBack)
                        this.NavigationService.GoBack();
            }
            else
            {

                if (this.NavigationContext.QueryString.ContainsKey("IngDetailUri"))
                {
                    IsLoading = true;
                    ingDetailUri = this.NavigationContext.QueryString["IngDetailUri"];
                    GetDetail(ingDetailUri);
                }
            }
        }

        private void content_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            if (block == null)
                return;
            IngDetail detail = block.DataContext as IngDetail;
            if (detail == null)
                return;

            string strContent = detail.IngContent;
            if (strContent.Length > 15)
                strContent = strContent.Substring(0, 15) + "...";

            this.runReplyTo.Text = detail.AuthorName;
            this.runReplyContent.Text = strContent;

            this.borderReplyForm.Visibility = System.Windows.Visibility.Visible;
            this.txtReplyContent.Focus();
        }


        private void item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            if (border == null)
                return;
            IngReplyDetail replyDetail = border.DataContext as IngReplyDetail;
            if (replyDetail == null)
                return;
            this.runReplyTo.Text = replyDetail.AuthorName;
            string strContent = replyDetail.ReplyContent;
            if (strContent.Length > 15)
                strContent = strContent.Substring(0, 15) + "...";
            this.runReplyContent.Text = strContent;

            this.borderReplyForm.Visibility = System.Windows.Visibility.Visible;
            //this.txtReplyTo.Visibility = System.Windows.Visibility.Visible;
            this.txtReplyContent.Focus();
        }

        private void appBarSubmitReply_Click(object sender, EventArgs e)
        {
            //回复内容不能超过200字 ContentId ReplyTo ParentCommentId Content 
            // Content=1 ContentId=488942  ParentCommentId=0  ReplyTo=289132
            PostIngReply();


        }

        private void appBarCancel_Click(object sender, EventArgs e)
        {
            if (this.borderReplyForm.Visibility == System.Windows.Visibility.Visible)
            {
                this.borderReplyForm.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }

        private void appBarRefresh_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ingDetailUri))
            {
                this.ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
                GetDetail(ingDetailUri);
            }
        }
        #region 获取闪存内容

        private void GetDetail(string uri)
        {
            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            request = HttpWebRequest.CreateHttp(uri);
            request.Method = "GET";
            request.CookieContainer = Constants.CookieContainer;
            request.UserAgent = Constants.UserAgent;
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private void GetResponseCallback(IAsyncResult ar)
        {
            try
            {
                request = ar.AsyncState as HttpWebRequest;
                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string strHtml = sr.ReadToEnd();
                sr.Close();
                stream.Close();
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GetIngDetail(strHtml);
                    this.ContentPanel.DataContext = IngDetails;
                    if (this.ContentPanel.Visibility == System.Windows.Visibility.Collapsed)
                        this.ContentPanel.Visibility = System.Windows.Visibility.Visible;
                    IsLoading = false;
                }));
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() => IsLoading = false));
                if (ex is WebException)
                {
                    WebException webExp = ex as WebException;
                    if (webExp.Status == WebExceptionStatus.RequestCanceled)
                    {
                        //用户取消了查询
                    }
                }
            }
        }

        private void GetIngDetail(string strHtml)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(strHtml);

            var mainNode = doc.GetElementbyId("main");
            if (mainNode == null || !mainNode.HasChildNodes)
                return;
            IngDetails = new IngDetail();
            foreach (var mainChildNode in mainNode.ChildNodes)
            {
                if (mainChildNode.Name == "div" && mainChildNode.Attributes["class"] != null)
                {
                    if (mainChildNode.Attributes["class"].Value == "postion_nav")
                    {
                        var lastNode = mainChildNode.LastChild;
                        if (lastNode.Name == "a" && lastNode.Attributes["href"] != null)
                        {
                            var authorIngPageUri = lastNode.Attributes["href"].Value;
                            IngDetails.AuthorIngUri = "http://home.cnblogs.com/" + authorIngPageUri;
                        }
                    }
                    else if (mainChildNode.Attributes["class"].Value == "feed_block")
                    {
                        foreach (var feedBlockNode in mainChildNode.ChildNodes)
                        {
                            if (feedBlockNode.Name == "div" && feedBlockNode.Attributes["class"] != null)
                            {
                                if (feedBlockNode.Attributes["class"].Value == "ing_detail_title")
                                {
                                    foreach (var ingDetailTitle in feedBlockNode.ChildNodes)
                                    {
                                        if (ingDetailTitle.Name == "a" && ingDetailTitle.Attributes["class"] == null)
                                        {
                                            var homeUri = ingDetailTitle.Attributes["href"].Value;
                                            var avatarImgUri = ingDetailTitle.ChildNodes["img"].Attributes["src"].Value;
                                            IngDetails.AuthorHomeUri = "http://home.cnblogs.com" + homeUri;
                                            IngDetails.AuthorAvatarImgUri = avatarImgUri;
                                        }
                                        else if (ingDetailTitle.Name == "a"
                                            && ingDetailTitle.Attributes["class"] != null
                                            && ingDetailTitle.Attributes["class"].Value == "ing_item_author")
                                        {
                                            var authorName = ingDetailTitle.InnerText;
                                            IngDetails.AuthorName = authorName;
                                        }
                                        else if (ingDetailTitle.Name == "br")
                                        {
                                            var publishTime = ingDetailTitle.NextSibling.InnerText;
                                            IngDetails.PublishTime = publishTime.Replace("\r\n", "").Trim();
                                        }
                                        else if (ingDetailTitle.Name == "a"
                                            && ingDetailTitle.Attributes["class"] != null
                                            && ingDetailTitle.Attributes["class"].Value == "recycle")
                                        {
                                            IngDetails.CanDelete = true;
                                        }
                                    }
                                }
                                else if (feedBlockNode.Attributes["class"].Value == "ing_detail_block")
                                {
                                    foreach (var ingDetailBlock in feedBlockNode.ChildNodes)
                                    {
                                        if (ingDetailBlock.Name == "div"
                                            && ingDetailBlock.Attributes["id"] != null
                                            && ingDetailBlock.Attributes["id"].Value == "ing_detail_body")
                                        {
                                            foreach (var ingDetailBody in ingDetailBlock.ChildNodes)
                                            {
                                                if (ingDetailBody.Name == "img" && ingDetailBody.Attributes["class"] != null)
                                                {
                                                    if (ingDetailBody.Attributes["class"].Value == "ing_icon_lucky")
                                                        IngDetails.IsLucky = "★";
                                                    else if (ingDetailBody.Attributes["class"].Value == "ing_icon_newbie")
                                                        IngDetails.IsNewbie = "❀";
                                                }
                                                else if (ingDetailBody.Name == "a")
                                                {
                                                    if (IngDetails.Links == null)
                                                        IngDetails.Links = new List<string>();
                                                    IngDetails.Links.Add(ingDetailBody.Attributes["href"].Value);
                                                }
                                            }
                                            var ingContent = ingDetailBlock.InnerText;
                                            IngDetails.IngContent = ingContent;
                                        }
                                        else if (ingDetailBlock.Name == "div"
                                            && ingDetailBlock.Attributes["class"] != null
                                            && ingDetailBlock.Attributes["class"].Value == "comment_list_block")
                                        {
                                            foreach (var commentListBlock in ingDetailBlock.ChildNodes)
                                            {
                                                if (commentListBlock.Name == "div"
                                                    && commentListBlock.Attributes["class"] != null
                                                    && commentListBlock.Attributes["class"].Value == "ing_comment_count")
                                                {
                                                    var commentCount = commentListBlock.InnerText;
                                                    IngDetails.IngCommentCount = commentCount;
                                                }
                                                else if (commentListBlock.Name == "ul")
                                                {
                                                    var currentIngId = commentListBlock.Attributes["id"].Value.Remove(0, "comment_block_".Length);
                                                    IngDetails.IngId = currentIngId;
                                                    if (commentListBlock.HasChildNodes)
                                                    {
                                                        IngDetails.IngReplyDetail = new ObservableCollection<IngReplyDetail>();
                                                        foreach (var liNode in commentListBlock.ChildNodes)
                                                        {
                                                            if (liNode.Name != "li")
                                                                continue;
                                                            IngReplyDetail ingReplyDetail = new IngReplyDetail();
                                                            foreach (var divNode in liNode.ChildNodes["div"].ChildNodes)
                                                            {
                                                                if (divNode.Name == "a"
                                                                    && divNode.Attributes["class"] == null
                                                                    && divNode.Attributes["id"] == null)
                                                                {
                                                                    var replyImgUri = divNode.ChildNodes["img"].Attributes["src"].Value;
                                                                    ingReplyDetail.AuthorAvatarImgUri = replyImgUri;
                                                                }
                                                                else if (divNode.Name == "a"
                                                                    && divNode.Attributes["id"] != null
                                                                    && divNode.Attributes["id"].Value.StartsWith("comment_author_"))
                                                                {
                                                                    var authorId = divNode.Attributes["id"].Value.Remove(0, "comment_author_".Length);
                                                                    var authorIngUri = divNode.Attributes["href"].Value;
                                                                    var authorName = divNode.InnerText;
                                                                    ingReplyDetail.CurrentReplyId = authorId;
                                                                    ingReplyDetail.AuthorIngUri = "http://home.cnblogs.com" + authorIngUri;
                                                                    ingReplyDetail.AuthorName = authorName;
                                                                    string replyContent = string.Empty;
                                                                    HtmlAgilityPack.HtmlNode nextContent = divNode.NextSibling;
                                                                    while (true)
                                                                    {
                                                                        if (nextContent.Name == "span"
                                                                        && nextContent.Attributes["class"] != null
                                                                        && nextContent.Attributes["class"].Value == "text_green")
                                                                        {
                                                                            break;
                                                                        }
                                                                        replyContent += nextContent.InnerText;
                                                                        nextContent = nextContent.NextSibling;
                                                                    }

                                                                    Regex regex = new Regex(@"\s+");
                                                                    replyContent = regex.Replace(replyContent, " ");
                                                                    ingReplyDetail.ReplyContent = replyContent.Replace("\r\n", "").Trim();
                                                                }
                                                                else if (divNode.Name == "span"
                                                                    && divNode.Attributes["class"] != null
                                                                    && divNode.Attributes["class"].Value == "text_green")
                                                                {
                                                                    var replyTime = divNode.Attributes["title"].Value;
                                                                    ingReplyDetail.ReplyTime = replyTime;
                                                                }
                                                                else if (divNode.Name == "a" && divNode.InnerText == "回复")
                                                                {
                                                                    ingReplyDetail.CanReply = true;
                                                                }
                                                            }
                                                            IngDetails.IngReplyDetail.Add(ingReplyDetail);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        private void PostIngReply()
        {
            if (postRequest != null && IsLoading)
            {
                MessageBox.Show("上次提交未完成...");
            }
            postRequest = HttpWebRequest.CreateHttp(Constants.IngReplyUri);
            postRequest.Method = "POST";
            postRequest.ContentType = "application/json; charset=utf-8";
            postRequest.CookieContainer = Constants.CookieContainer;
            postRequest.UserAgent = Constants.UserAgent;
            postRequest.BeginGetRequestStream(new AsyncCallback(PostRequestCallback), postRequest);
        }

        private void PostRequestCallback(IAsyncResult ar)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}