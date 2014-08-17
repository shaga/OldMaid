using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace OldMaidWpf.View
{
    public class ListBoxExtender : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(ListBoxExtender), new UIPropertyMetadata(default(bool), OnAutScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        public static void OnAutScrollToEndChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var listBox = obj as ListBox;

            if (listBox == null) return;

            var items = listBox.Items;
            if (items == null) return;

            var data = items.SourceCollection as INotifyCollectionChanged;

            if (data == null) return;

            var scrollToEndHandler = new NotifyCollectionChangedEventHandler((s, args) =>
            {
                if (items.Count > 0)
                {
                    var lastItem = items[items.Count - 1];
                    listBox.Dispatcher.Invoke(() => listBox.ScrollIntoView(lastItem), System.Windows.Threading.DispatcherPriority.Send);
                }
            });

            var newWtate = (bool)e.NewValue;
            var oldState = (bool)e.OldValue;

            if (newWtate)
            {
                if (listBox.Tag is NotifyCollectionChangedEventHandler)
                {
                    data.CollectionChanged -= listBox.Tag as NotifyCollectionChangedEventHandler;
                }

                data.CollectionChanged += scrollToEndHandler;
                listBox.Tag = scrollToEndHandler;
            }
            else if (oldState && listBox.Tag is NotifyCollectionChangedEventHandler)
            {
                data.CollectionChanged -= listBox.Tag as NotifyCollectionChangedEventHandler;
                listBox.Tag = null;
            }
        }
    }
}
