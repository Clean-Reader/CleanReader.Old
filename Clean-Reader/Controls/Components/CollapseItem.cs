using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Clean_Reader.Controls.Components
{
    public sealed class CollapseItem : Control
    {
        public ListView InnerListView;
        private Grid _headerContainer;
        private Grid _detailContainer;
        private ProgressRing _loadingRing;
        private ContentPresenter _headerIcon;

        public event ItemClickEventHandler ItemClick;
        public event EventHandler HeaderTapped;
        public CollapseItem()
        {
            this.DefaultStyleKey = typeof(CollapseItem);
        }

        protected override void OnApplyTemplate()
        {
            _headerContainer = GetTemplateChild("HeaderContainer") as Grid;
            _detailContainer = GetTemplateChild("DetailContainer") as Grid;
            InnerListView = GetTemplateChild("DetailListView") as ListView;

            _loadingRing = GetTemplateChild("LoadingRing") as ProgressRing;
            _headerIcon = GetTemplateChild("HeaderIcon") as ContentPresenter;

            _headerContainer.Tapped += (_s, _e) => { HeaderTapped?.Invoke(this, EventArgs.Empty); };
            InnerListView.ItemClick += (_s, _e) => { ItemClick?.Invoke(this, _e); };

            base.OnApplyTemplate();
        }

        public void GoToLoading(bool isLoading = true)
        {
            if (isLoading)
            {
                IsEnabled = false;
                _headerIcon.Visibility = Visibility.Collapsed;
                _loadingRing.IsActive = true;
            }
            else
            {
                IsEnabled = true;
                _headerIcon.Visibility = Visibility.Visible;
                _loadingRing.IsActive = false;
            }
        }

        #region Dependencies


        public IconElement HeaderIcon
        {
            get { return (IconElement)GetValue(HeaderIconProperty); }
            set { SetValue(HeaderIconProperty, value); }
        }
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register("HeaderIcon", typeof(IconElement), typeof(CollapseItem), new PropertyMetadata(null));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(CollapseItem), new PropertyMetadata(""));

        public Thickness HeaderPadding
        {
            get { return (Thickness)GetValue(HeaderPaddingProperty); }
            set { SetValue(HeaderPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register("HeaderPadding", typeof(Thickness), typeof(CollapseItem), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CollapseItem), new PropertyMetadata(null));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CollapseItem), new PropertyMetadata(null));

        public bool IsExpand
        {
            get { return (bool)GetValue(IsExpandProperty); }
            set { SetValue(IsExpandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandProperty =
            DependencyProperty.Register("IsExpand", typeof(bool), typeof(CollapseItem), new PropertyMetadata(false, new PropertyChangedCallback(IsExpand_Changed)));

        public Visibility StatusIconVisibility
        {
            get { return (Visibility)GetValue(StatusIconVisibilityProperty); }
            set { SetValue(StatusIconVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusIconVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusIconVisibilityProperty =
            DependencyProperty.Register("StatusIconVisibility", typeof(Visibility), typeof(CollapseItem), new PropertyMetadata(Visibility.Visible));



        #endregion

        #region Events
        private static void IsExpand_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as CollapseItem;
            if ((bool)e.NewValue)
            {
                VisualStateManager.GoToState(instance, "Expand", true);
            }
            else
            {
                VisualStateManager.GoToState(instance, "Normal", true);
            }
        }
        #endregion
    }
}
