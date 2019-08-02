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
            var ListView = d as ListView;
            if (ListView == null) return;
            bool oldValue = (bool) e.OldValue, newValue = (bool) e.NewValue;
            if (newValue == oldValue) return;
            if (newValue)
            {
                ListView.Loaded += ListView_Loaded;
                ListView.Unloaded += ListView_Unloaded;
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(ListView)["ItemsSource"];
                itemsSourcePropertyDescriptor.AddValueChanged(ListView, ListView_ItemsSourceChanged);
            }
            else
            {
                ListView.Loaded -= ListView_Loaded;
                ListView.Unloaded -= ListView_Unloaded;
                if (Associations.ContainsKey(ListView))
                    Associations[ListView].Dispose();
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(ListView)["ItemsSource"];
                itemsSourcePropertyDescriptor.RemoveValueChanged(ListView, ListView_ItemsSourceChanged);
            }
        }

        private static void ListView_ItemsSourceChanged(object sender, EventArgs e)
        {
            var ListView = (ListView) sender;
            if (Associations.ContainsKey(ListView))
                Associations[ListView].Dispose();
            Associations[ListView] = new Capture(ListView);
        }

        private static void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView) sender;
            if (Associations.ContainsKey(ListView))
                Associations[ListView].Dispose();
            ListView.Unloaded -= ListView_Unloaded;
        }

        private static void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var ListView = (ListView) sender;
            var incc = ListView.Items as INotifyCollectionChanged;
            if (incc == null) return;
            ListView.Loaded -= ListView_Loaded;
            Associations[ListView] = new Capture(ListView);
        }

        private class Capture : IDisposable
        {
            private readonly INotifyCollectionChanged incc;
            private readonly ListView ListView;

            public Capture(ListView ListView)
            {
                this.ListView = ListView;
                incc = ListView.ItemsSource as INotifyCollectionChanged;
                if (incc != null) incc.CollectionChanged += incc_CollectionChanged;
            }

            public void Dispose()
            {
                if (incc != null)
                    incc.CollectionChanged -= incc_CollectionChanged;
            }

            private void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    ListView.ScrollIntoView(e.NewItems[0]);
                    ListView.SelectedItem = e.NewItems[0];
                }
            }
        }
    }
}