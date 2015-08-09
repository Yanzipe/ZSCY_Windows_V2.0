﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZSCY.Data;
using ZSCY.Util;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace ZSCY.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class JWContentPage : Page
    {
        public JWContentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var JWItem = (JWList)e.Parameter;

            if (JWItem.Content == "加载中...")
                getContent(JWItem.ID);

            TitleTextBlock.Text = JWItem.Title;
            ContentTextBlock.Text = JWItem.Content;
            DateTextBlock.Text = JWItem.Date;
            ReadTextBlock.Text = JWItem.Read;

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
        }


        private async void getContent(string ID)
        {
            List<KeyValuePair<String, String>> contentparamList = new List<KeyValuePair<String, String>>();
            contentparamList.Add(new KeyValuePair<string, string>("id", ID));
            string jwContent = await NetWork.getHttpWebRequest("api/jwNewsContent", contentparamList);
            Debug.WriteLine("jwContent->" + jwContent);
            if (jwContent != "")
            {
                string JWContentText = jwContent.Replace("(\r?\n(\\s*\r?\n)+)", "\r\n");
                while (JWContentText.StartsWith("\r\n "))
                    JWContentText = JWContentText.Substring(3);
                while (JWContentText.StartsWith("\r\n"))
                    JWContentText = JWContentText.Substring(2);
                JObject jwContentobj = JObject.Parse(JWContentText);
                if (Int32.Parse(jwContentobj["status"].ToString()) == 200)
                    ContentTextBlock.Text = jwContentobj["data"]["content"].ToString();
                else
                    ContentTextBlock.Text = "加载失败";
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }
    }
}
