using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DataGridExtensions
{
    public static class DataGridFilter
    {
        #region IsFilterEnabled Attached


        public static bool GetIsFilterEnabled(this DataGrid obj)
        {
            return (bool)obj.GetValue(IsFilterEnabledProperty);
        }

        public static void SetIsFilterEnabled(this DataGrid obj, bool value)
        {
            obj.SetValue(IsFilterEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsFilterEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFilterEnabledProperty =
            DependencyProperty.RegisterAttached("IsFilterEnabled", typeof(bool), typeof(DataGridFilter), new PropertyMetadata(false,(s,e)=> {
                //创建DataGrid逻辑操作类DataGridFilterHost
                var dataGrid = s as DataGrid;
                dataGrid?.GetFilterHost().OnEnabled(true.Equals(e.NewValue));
            }));


        #endregion

        #region FilterHost Attached


        public static DataGridFilterHost GetFilterHost(this DataGrid obj)
        {
            var value = (DataGridFilterHost)obj.GetValue(FilterHostProperty);
            if (value == null)
            { value = new DataGridFilterHost(obj);
                obj.SetValue(FilterHostProperty, value);
            }
            return value;
        }

        // Using a DependencyProperty as the backing store for FilterHost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterHostProperty =
            DependencyProperty.RegisterAttached("FilterHost", typeof(DataGridFilterHost), typeof(DataGridFilterHost));

        #endregion
    }
}
