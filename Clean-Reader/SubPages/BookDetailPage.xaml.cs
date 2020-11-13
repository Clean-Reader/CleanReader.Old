﻿using Clean_Reader.Models.Core;
using Richasy.Controls.UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yuenov.SDK.Models.Share;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Clean_Reader.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BookDetailPage : RichasyPage
    {
        AppViewModel vm = App.VM;
        private Book _currentBook;
        public BookDetailPage() : base()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is Book webBook)
                {
                    if (webBook != _currentBook)
                        await PageInit(webBook);
                }
            }
            base.OnNavigatedTo(e);
        }

        public async Task PageInit(Book data)
        {
            Container.Visibility = Visibility.Collapsed;
            NoDataBlock.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = true;
            _currentBook = data;
            var response = await vm._yuenovClient.GetBookDetailAsync(data.BookId);
            if (response.Result.Code == Yuenov.SDK.Enums.ResultCode.Success)
            {
                var detail = response.Data;
                DetailCard.Data = detail;
                DescriptionBlock.Text = detail.Description;
                Container.Visibility = Visibility.Visible;
            }
            else
            {
                NoDataBlock.Visibility = Visibility.Visible;
            }
            LoadingRing.IsActive = false;
        }
    }
}