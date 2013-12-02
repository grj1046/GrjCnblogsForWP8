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
using System.IO.IsolatedStorage;
using System.Xml;

namespace GrjCnblogsForWP8.Views
{
    public partial class Login : PhoneApplicationPage
    {
        private HttpWebRequest request;
        private string requestData = string.Empty;
        private bool savePassword = false;

        public Login()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.scrollViewer.IsEnabled = true;
            this.txtErrorMsg.Text = string.Empty;
        }

        private void appBtnCancel_Click(object sender, EventArgs e)
        {
            if (SystemTray.ProgressIndicator.IsIndeterminate)
            {
                this.scrollViewer.IsEnabled = true;
                SystemTray.ProgressIndicator.IsIndeterminate = false;
                request.Abort();
                return;
            }
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
            else
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void appBtnLogin_Click(object sender, EventArgs e)
        {
            this.txtErrorMsg.Text = string.Empty;
            //登录中。。。
            if (SystemTray.ProgressIndicator.IsIndeterminate)
                return;
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            this.scrollViewer.IsEnabled = false;
            if (this.chkRemember.IsChecked.HasValue)
                savePassword = this.chkRemember.IsChecked.Value;

            requestData = string.Format("tbUserName={0}&tbPassword={1}", HttpUtility.UrlEncode(this.tbUserName.Text), HttpUtility.UrlEncode(this.tbPassword.Password));

            request = HttpWebRequest.CreateHttp(Constants.LoginUri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = Constants.UserAgent;
            request.CookieContainer = Constants.CookieContainer;
            request.BeginGetRequestStream(new AsyncCallback(RequestCallback), request);
        }


        private void RequestCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = ar.AsyncState as HttpWebRequest;

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(requestData);
                using (Stream stream = request.EndGetRequestStream(ar))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
            }
            catch (Exception)
            {

                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.scrollViewer.IsEnabled = true;
                    SystemTray.ProgressIndicator.IsIndeterminate = false;
                }));
            }
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = ar.AsyncState as HttpWebRequest;

                HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        string str = sr.ReadToEnd();
                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (str.IndexOf("登录失败") != -1)
                            {
                                //登录失败 //登录失败！用户名或密码错误！
                                this.txtErrorMsg.Text = "登录失败，用户名或密码错误！";
                            }
                            else
                            {
                                //登录成功
                                this.txtErrorMsg.Text = string.Empty;
                                var cookies = request.CookieContainer.GetCookies(new Uri(Constants.MainPageUri));
                                //var cookies = response.Cookies;//response.Cookies中没有cookies
                                Constants.CookieContainer.Add(new Uri(Constants.MainPageUri), cookies);

                                if (savePassword)
                                {
                                    //foreach (Cookie cookie in cookies)
                                    //{
                                    //    var v = cookie;

                                    //    Cookie c = new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
                                    //}
                                    using (IsolatedStorageFile isoStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                                    {
                                        string loginFileName = "loginCookies";
                                        if (isoStorageFile.FileExists(loginFileName))
                                        {
                                            isoStorageFile.DeleteFile(loginFileName);
                                        }
                                        using (IsolatedStorageFileStream fileStream = isoStorageFile.OpenFile(loginFileName, FileMode.OpenOrCreate, FileAccess.Write))
                                        {
                                            using (StreamWriter sw = new StreamWriter(fileStream))
                                            {
                                                XmlWriterSettings setting = new XmlWriterSettings();
                                                setting.Indent = true;
                                                XmlWriter xw = XmlWriter.Create(fileStream, setting);

                                                xw.WriteStartDocument();
                                                foreach (Cookie cookie in cookies)
                                                {
                                                    xw.WriteStartElement("Cookie");
                                                    xw.WriteAttributeString("CookieName", cookie.Name);
                                                    xw.WriteAttributeString("CookieValue", cookie.Value);
                                                    xw.WriteAttributeString("CookieDomain", cookie.Domain);
                                                    xw.WriteAttributeString("CookieExpired", cookie.Expired.ToString());
                                                    xw.WriteAttributeString("CookieExpires", cookie.Expires.ToString());
                                                    xw.WriteAttributeString("CookieHttpOnly", cookie.HttpOnly.ToString());
                                                    xw.WriteAttributeString("CookiePath", cookie.Path);
                                                    xw.WriteEndElement();
                                                }
                                                xw.WriteEndDocument();
                                                xw.Flush();
                                                sw.Close();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    using (IsolatedStorageFile isoStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                                    {
                                        string loginFileName = "loginCookies";
                                        if (isoStorageFile.FileExists(loginFileName))
                                            isoStorageFile.DeleteFile(loginFileName);
                                    }
                                }
                                if (this.NavigationService.CanGoBack)
                                    this.NavigationService.GoBack();
                                else
                                    this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                            }
                            this.scrollViewer.IsEnabled = true;
                            SystemTray.ProgressIndicator.IsIndeterminate = false;
                        }));
                    }
                }
                else
                {
                    //Error
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.txtErrorMsg.Text = response.StatusCode.ToString();
                    }));
                }
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.scrollViewer.IsEnabled = true;
                    SystemTray.ProgressIndicator.IsIndeterminate = false;
                    MessageBox.Show(ex.Message);
                }));
            }
        }
    }
}