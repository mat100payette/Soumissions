using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightCustomControls
{
    public class SelectedValuesListBox : ListBox
    {
        /// <summary>
        /// Use the monitor to prevent stack overflow exceptions
        /// </summary>
        private bool monitor = true;


        /// <summary>
        /// The dependency property for selected values
        /// </summary>
        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.RegisterAttached("SelectedValues",
            typeof(IList), typeof(SelectedValuesListBox),
            new PropertyMetadata(OnValuesChanged));

        /// <summary>
        /// Property for the control
        /// </summary>
        public IList SelectedValues
        {
            get { return (IList)GetValue(SelectedValuesProperty); }
            set { SetValue(SelectedValuesProperty, value); }
        }

        /// <summary>
        /// When a list is bound to the control,
        /// tell the control to set all items as selected.
        /// Also, if this is an ObservableCollection (or any other INotify
        /// list for that matter), refresh the selection in the list.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void OnValuesChanged(DependencyObject dependencyObject,
                                            DependencyPropertyChangedEventArgs e)
        {
            // Set the selections in the control the first time the list is bound
            SelectedValuesListBox multi = dependencyObject as SelectedValuesListBox;
            multi.SetSelected(e.NewValue as IList);
            // For each change in the bound list, change the selection in the control
            if (e.NewValue is INotifyPropertyChanged)
                (e.NewValue as INotifyPropertyChanged).PropertyChanged +=
                  (dependencyObject as SelectedValuesListBox).MultiList_PropertyChanged;
        }

        /// <summary>
        /// This constructor will make sure the selection changes
        /// in the control are reflected in the bound list
        /// </summary>
        public SelectedValuesListBox()
        {
            SelectionChanged += new SelectionChangedEventHandler(MultiList_SelectionChanged);
        }

        /// <summary>
        /// This event will send the selection changes from the control to the bound list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monitor && !string.IsNullOrEmpty(SelectedValuePath))
            {
                try
                {
                    monitor = false;
                    // Reset the selected values property
                    SelectedValues.Clear();
                    // Loop each selected item
                    // Add the value to the list based on the selected value path
                    foreach (object item in SelectedItems)
                    {
                        PropertyInfo property = item.GetType().GetProperty(SelectedValuePath);
                        if (property != null)
                            SelectedValues.Add(property.GetValue(item, null));
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    monitor = true;
                }
            }
        }

        /// <summary>
        /// There was a change in the list that was bound.
        /// Change the selection of the items in the control!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MultiList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetSelected(sender as IList);
        }

        /// <summary>
        /// Set the selection in the control based on a bound list
        /// </summary>
        /// <param name="list"></param>
        public void SetSelected(IList list)
        {
            if (monitor && !string.IsNullOrEmpty(SelectedValuePath)
                        && list != null && list.Count > 0)
            {
                try
                {
                    monitor = false;
                    // Loop each item
                    foreach (object item in Items)
                    {
                        // Get the property based on the selected value path
                        PropertyInfo property = item.GetType().GetProperty(SelectedValuePath);
                        if (property != null)
                        {
                            // Match the value from the bound list to an item in the control
                            if (list.Contains(property.GetValue(item, null)))
                                SelectedItems.Add(item);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    monitor = true;
                }
            }
        }
    }
}
