//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::Xamarin.Forms.Xaml.XamlResourceIdAttribute("RecordAnalyzerApp.RecordStoreReports - Copy.xaml", "RecordStoreReports - Copy.xaml", typeof(global::RecordAnalyzerApp.RecordStoreReports))]

namespace RecordAnalyzerApp {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("RecordStoreReports - Copy.xaml")]
    public partial class RecordStoreReports : global::Xamarin.Forms.ContentPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.ToolbarItem btnOk;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.EntryCell myReportTitle;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::RecordAnalyzerApp.Controls.DatePairCell myDatePairSelector;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::RecordAnalyzerApp.Controls.DateCell fromDate;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::RecordAnalyzerApp.Controls.DateCell toDate;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::RecordAnalyzerApp.Controls.ColumnListCell sortByColumn;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.SwitchCell sortByDescending;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(RecordStoreReports));
            btnOk = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.ToolbarItem>(this, "btnOk");
            myReportTitle = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.EntryCell>(this, "myReportTitle");
            myDatePairSelector = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::RecordAnalyzerApp.Controls.DatePairCell>(this, "myDatePairSelector");
            fromDate = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::RecordAnalyzerApp.Controls.DateCell>(this, "fromDate");
            toDate = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::RecordAnalyzerApp.Controls.DateCell>(this, "toDate");
            sortByColumn = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::RecordAnalyzerApp.Controls.ColumnListCell>(this, "sortByColumn");
            sortByDescending = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.SwitchCell>(this, "sortByDescending");
        }
    }
}
