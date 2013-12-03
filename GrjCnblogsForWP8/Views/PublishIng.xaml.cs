using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Text.RegularExpressions;

namespace GrjCnblogsForWP8.Views
{
    public partial class PublishIng : PhoneApplicationPage
    {
        HttpWebRequest request;
        //ApplicationBarIconButton appBarPublish;
        private string ingContent;
        private string publicFlag = "1";

        public PublishIng()
        {
            InitializeComponent();
        }
        public bool IsLoading
        {
            get { return SystemTray.ProgressIndicator.IsVisible; }
            set { SystemTray.ProgressIndicator.IsVisible = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
            {
                var dialogResult = MessageBox.Show("您尚未登录，请先登录。", "提示", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rdb = sender as RadioButton;
            switch (rdb.Content.ToString())
            {
                case "私有":
                    publicFlag = "0";
                    break;
                case "公开":
                default:
                    publicFlag = "1";
                    break;
            }
        }

        private void appBarCancel_Click(object sender, EventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }

        private void appBarPublish_Click(object sender, EventArgs e)
        {
            if (Constants.CookieContainer == null || Constants.CookieContainer.Count == 0)
            {
                var dialogResult = MessageBox.Show("您尚未登录，请先登录。", "提示", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                }
                else
                {
                    return;
                }
            }
            //http://home.cnblogs.com/ajax/ing/Publish
            //JSON格式
            //content= 闪存内容
            //publicFlag= 1 公开 0 私有

            //返回 Json格式{"IsSuccess":true,"responseText":"Hello World"}
            //IsSuccess 成功或者失败
            //responseText 闪存内容？

            string strContent = this.txtIngContent.Text;
            if (string.IsNullOrEmpty(strContent))
            {
                MessageBox.Show("请输入内容", "提示", MessageBoxButton.OK);
                return;
            }
            Regex regex = new Regex(@"/(http|ftp|https):\/\/([^\/:,，]+)(:\d+)?(\/[^\u0391-\uFFE5\s,]*)?/ig");
            int length = regex.Replace(strContent, "").Replace(@"/(\s)+/ig", "").Length;
            if (length > 300)
            {
                MessageBox.Show("闪存内容不能超过300个字符！当前字符数: " + length, "提示", MessageBoxButton.OK);
                return;
            }
            regex = new Regex(@"\s+");
            ingContent = regex.Replace(strContent, " ");

            if (request != null && IsLoading)
            {
                request.Abort();
                IsLoading = false;
            }
            IsLoading = true;
            request = HttpWebRequest.CreateHttp(Constants.IngPublishIng);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.CookieContainer = Constants.CookieContainer;
            request.UserAgent = Constants.UserAgent;
            request.BeginGetRequestStream(new AsyncCallback(GetRequestCallback), request);
        }

        private void GetRequestCallback(IAsyncResult ar)
        {
            try
            {
                //string strContent = string.Format("{\"content\":\"{0}\", \"publicFlag\":\"{1}\" }", ingContent, publicFlag);
                string strContent = "{\"content\":\"" + ingContent + "\", \"publicFlag\":\"" + publicFlag + "\" }";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strContent);

                request = ar.AsyncState as HttpWebRequest;
                Stream stream = request.EndGetRequestStream(ar);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);

            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException webExp = ex as WebException;
                    if (webExp.Status == WebExceptionStatus.RequestCanceled)
                    {
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() => { IsLoading = false; }));
            }
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
                    IsLoading = false;
                    if (strHtml.Contains("\"IsSuccess\":true") && this.NavigationService.CanGoBack)
                        this.NavigationService.GoBack();
                    //this.NavigationService.Navigate(new Uri("/Views/IngView.xaml?from=PublishIngPage", UriKind.Relative));
                    else if (strHtml.Contains("相同闪存已发布"))
                        MessageBox.Show("相同闪存已发布", "提示", MessageBoxButton.OK);
                    else
                        MessageBox.Show("发生异常：\r\n" + strHtml);
                }));
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException webExp = ex as WebException;
                    if (webExp.Status == WebExceptionStatus.RequestCanceled)
                    {
                    }
                }
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() => { IsLoading = false; }));
            }
        }
    }
}