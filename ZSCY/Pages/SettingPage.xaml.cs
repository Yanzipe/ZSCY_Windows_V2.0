﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZSCY.Util;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace ZSCY.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private ApplicationDataContainer appSetting;
        public SettingPage()
        {
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            UmengSDK.UmengAnalytics.TrackPageStart("SettingPage");
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

        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageEnd("SettingPage");
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
        }

        private async void importKB2calendarButton_Click(object sender, RoutedEventArgs e)
        {
            var dig = new MessageDialog("订阅课表为实验室功能，我们无法保证此功能100%可用与数据100%正确性，我们期待您的反馈。\n\n是否继续尝试？", "警告");
            var btnOk = new UICommand("是");
            dig.Commands.Add(btnOk);
            var btnCancel = new UICommand("否");
            dig.Commands.Add(btnCancel);
            var result = await dig.ShowAsync();
            if (null != result && result.Label == "是")
            {
                Frame.Navigate(typeof(ImportKB2CalendarPage));
            }
            else if (null != result && result.Label == "否")
            {
            }
        }

        private void AboutAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var dig = new MessageDialog("若应用无法使用，请尝试清除数据，清除数据后会自动退出应用。\n\n是否继续？", "警告");
            var btnOk = new UICommand("是");
            dig.Commands.Add(btnOk);
            var btnCancel = new UICommand("否");
            dig.Commands.Add(btnCancel);
            var result = await dig.ShowAsync();
            if (null != result && result.Label == "是")
            {
                appSetting.Values.Clear();
                IStorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                IStorageFile storageFileWR = await applicationFolder.CreateFileAsync("kb", CreationCollisionOption.OpenIfExists);
                try
                {
                    await storageFileWR.DeleteAsync();
                }
                catch (Exception)
                {
                    Debug.WriteLine("设置 -> 重置应用异常");
                }
                Application.Current.Exit();
            }
            else if (null != result && result.Label == "否")
            {
            }
        }

        private void SearchFreeTime_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchFreeTimeNumPage));
        }

        //private async void OpacityToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        //{
        //    Uri logo1 = null;
        //    Uri logo2 = null;

        //    var useLogo1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Logo.scale-240.png", UriKind.Absolute));
        //    var useLogo2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Square71x71Logo.scale-240.png", UriKind.Absolute));

        //    try
        //    {
        //        if (OpacityToggleSwitch.IsOn != null && OpacityToggleSwitch.IsOn == true)
        //        {

        //            logo1 = new Uri("ms-appx:///Assets/AlphaLogo/Logo.scale-240.png");
        //            logo2 = new Uri("ms-appx:///Assets/AlphaLogo/Square71x71Logo.scale-240.png");
        //            await useLogo1.CopyAndReplaceAsync(await StorageFile.GetFileFromApplicationUriAsync(logo1));
        //            await useLogo2.CopyAndReplaceAsync(await StorageFile.GetFileFromApplicationUriAsync(logo2));

        //        }
        //        else if (OpacityToggleSwitch.IsOn != null && OpacityToggleSwitch.IsOn == false)
        //        {
        //            logo1 = new Uri("ms-appx:///Assets/BlueLogo/Logo.scale-240.png");
        //            logo2 = new Uri("ms-appx:///Assets/BlueLogo/Square71x71Logo.scale-240.png");
        //            await useLogo1.CopyAndReplaceAsync(await StorageFile.GetFileFromApplicationUriAsync(logo1));
        //            await useLogo2.CopyAndReplaceAsync(await StorageFile.GetFileFromApplicationUriAsync(logo2));

        //        }


        //        //Uri logo1 = new Uri("ms-appx:///Assets/AlphaLogo/Logo.scale-240.png");
        //        string tileString150 = "<tile>" +
        //                        "<visual version=\"2\">" +
        //                            "<binding template=\"TileSquare150x150Image\">" +
        //                                "<image id=\"1\" src=\"" + logo1 + "\" alt=\"alt text\"/>" +
        //                            "</binding>" +
        //                        "</visual>" +
        //                    "</tile>";
        //        XmlDocument tileXML150 = new XmlDocument();
        //        tileXML150.LoadXml(tileString150);
        //        TileNotification newTile150 = new TileNotification(tileXML150);
        //        TileUpdater updater150 = TileUpdateManager.CreateTileUpdaterForApplication();
        //        updater150.EnableNotificationQueue(false);
        //        updater150.Update(newTile150);


        //        string tileString71 = "<tile>" +
        //                        "<visual version=\"2\">" +
        //                            "<binding template=\"TileSquare71x71Image\">" +
        //                                "<image id=\"1\" src=\"" + logo2 + "\" alt=\"alt text\"/>" +
        //                            "</binding>" +
        //                        "</visual>" +
        //                    "</tile>";

        //        XmlDocument tileXML71 = new XmlDocument();
        //        tileXML71.LoadXml(tileString71);
        //        TileNotification newTile71 = new TileNotification(tileXML71);
        //        TileUpdater updater71 = TileUpdateManager.CreateTileUpdaterForApplication();
        //        updater71.EnableNotificationQueue(false);
        //        updater71.Update(newTile71);
        //    }
        //    catch (Exception) { }




        //    //XmlDocument TileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
        //    //XmlNodeList TileImage = TileXml.GetElementsByTagName("image");
        //    //((XmlElement)TileImage[0]).SetAttribute("src", "ms-appx:///Assets/AlphaLogo/Logo.scale-240.png");

        //    //var TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
        //    //ScheduledTileNotification Schedule = new ScheduledTileNotification(TileXml, DateTimeOffset.Now.AddSeconds(5));
        //    //TileUpdater.Clear();
        //    //TileUpdater.EnableNotificationQueue(true);
        //    //TileUpdater.AddToSchedule(Schedule);


        //    //TileNotification newTile = new TileNotification(TileXml);
        //    //TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
        //    //updater.EnableNotificationQueue(false);
        //    //updater.Update(newTile);


        //    //var Logo1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/SmallLogo.scale-240.png", UriKind.Absolute));
        //    //var Logo2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Square71x71Logo.scale-240.png", UriKind.Absolute));

        //}

    }
}
