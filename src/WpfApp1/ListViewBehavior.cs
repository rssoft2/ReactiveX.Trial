using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    internal class ListViewBehavior
    {
        private static readonly Dictionary<ListView, Capture> Associations =
            new Dictionary<ListView, Capture>();

        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem",
                typeof(bool),
                typeof(ListViewBehavior),
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool) obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        public static void OnScrollOnNewItemChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var listView = d as ListView;
            if (listView == null) return;
            bool oldValue = (bool) e.OldValue, newValue = (bool) e.NewValue;
            if (newValue == oldValue) return;
            if (newValue)
            {
                listView.Loaded += ListView_Loaded;
                listView.Unloaded += ListView_Unloaded;
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listView)["ItemsSource"];
                itemsSourcePropertyDescriptor.AddValueChanged(listView, ListView_ItemsSourceChanged);
            }
            else
            {
                listView.Loaded -= ListView_Loaded;
                listView.Unloaded -= ListView_Unloaded;
                if (Associations.ContainsKey(listView))
                    Associations[listView].Dispose();
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listView)["ItemsSource"];
                itemsSourcePropertyDescriptor.RemoveValueChanged(listView, ListView_ItemsSourceChanged);
            }
        }

        private static void ListView_ItemsSourceChanged(object sender, EventArgs e)
        {
            var listView = (ListView) sender;
            if (Associations.ContainsKey(listView))
                Associations[listView].Dispose();
            Associations[listView] = new Capture(listView);
        }

        private static void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView) sender;
            if (Associations.ContainsKey(listView))
                Associations[listView].Dispose();
            listView.Unloaded -= ListView_Unloaded;
        }

        private static void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView) sender;
            listView.Loaded -= ListView_Loaded;
            Associations[listView] = new Capture(listView);
        }

        private class Capture : IDisposable
        {
            private readonly INotifyCollectionChanged _incc;
            private readonly ListView _listView;

            public Capture(ListView listView)
            {
                _listView = listView;
                _incc = listView.ItemsSource as INotifyCollectionChanged;
                if (_incc != null) _incc.CollectionChanged += incc_CollectionChanged;
            }

            public void Dispose()
            {
                if (_incc != null)
                    _incc.CollectionChanged -= incc_CollectionChanged;
            }

            private void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    _listView.ScrollIntoView(e.NewItems[0]);
                    _listView.SelectedItem = e.NewItems[0];
                }
            }
        }
    }
}