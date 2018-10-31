using DBViewerExample.DAO;
using DBViewerExample.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.IO;

namespace DBViewerExample.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public partial class HomeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the HomeViewModel class.
        /// </summary>

        #region properties and variables

        private string _selectedcolumn;

        public string selectedcolumn
        {
            get
            {
                return _selectedcolumn;
            }
            set
            {
                Set(nameof(selectedcolumn), ref _selectedcolumn, value);
            }
        }

        private string _selectedID;

        public string SelectedID
        {
            get
            {
                return _selectedID;
            }
            set
            {
                Set(nameof(SelectedID), ref _selectedID, value);
            }
        }

        private object _nowindex;

        public object nowindex
        {
            get
            {
                return _nowindex;
            }
            set
            {
                Set(nameof(nowindex), ref _nowindex, value);
            }
        }

        private int _nowtab;

        public int nowtab
        {
            get
            {
                return _nowtab;
            }
            set
            {
                Set(nameof(nowtab), ref _nowtab, value);
            }
        }

        private DataTable _Items;

        public DataTable Items
        {
            get
            {
                return _Items;
            }
            set
            {
                Set(nameof(Items), ref _Items, value);
            }
        }

        private ObservableCollection<string> _Columns;

        public ObservableCollection<string> Columns
        {
            get
            {
                return _Columns;
            }
            set
            {
                Set(nameof(Columns), ref _Columns, value);
            }
        }

        private ObservableCollection<string> _TableNames;

        public ObservableCollection<string> TableNames
        {
            get
            {
                return _TableNames;
            }
            set
            {
                Set(nameof(TableNames), ref _TableNames, value);
            }
        }

        private string _nowcolumnname;

        public string nowcolumnname
        {
            get
            {
                return _nowcolumnname;
            }
            set
            {
                Set(nameof(nowcolumnname), ref _nowcolumnname, value);
            }
        }

        private string _nowtable;

        public string nowtable
        {
            get
            {
                return _nowtable;
            }
            set
            {
                Set(nameof(nowtable), ref _nowtable, value);
            }
        }

        private string _mergeable;

        public string mergeable
        {
            get
            {
                return _mergeable;
            }

            set
            {
                Set(nameof(mergeable), ref _mergeable, value);
            }
        }

        public TabControl mainTabControl;

        public ICommand AddCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ClickCommand { get; set; }

        public ICommand TreeClickCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand TableChangeCommand { get; set; }

        public ICommand TryMergeCommand { get; set; }
        public ICommand TryUpdateCommand { get; set; }

        private string connectionstringproto = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3}";

        #endregion properties and variables

        #region constructor

        public HomeViewModel()
        {
            SetConnection();
            MergeNotify = "";
            MergeItems = new ObservableCollection<TreeNode>();
            TableNames = new ObservableCollection<string>();
            Items = new DataTable();
            Columns = new ObservableCollection<string>();
            SetTable();

            AddCommand = new RelayCommand(() => ExecuteAddCommand());
            ModifyCommand = new RelayCommand(() => ExecuteModifyCommand());
            SearchCommand = new RelayCommand(() => ExecuteSearchCommand());
            ClickCommand = new RelayCommand(() => ExecuteClickCommand());
            DeleteCommand = new RelayCommand(() => ExecuteDeleteCommand());
            LoadedCommand = new RelayCommand<object>(ExecuteLoadedCommand);
            TreeClickCommand = new RelayCommand<TreeNode>(ExecuteTreeClickCommand);
            TableChangeCommand = new RelayCommand<ComboBox>(ExecuteTableChangeCommand);
            TryMergeCommand = new RelayCommand(() => ExecuteTryMergeCommand());
            TryUpdateCommand = new RelayCommand(() => ExecuteTryUpdateCommand());

            names = new Collection<string>();
            FilePath = "";
            canwork = false;
            try
            {
                if (GlobalJSONData.contentJArray.ToString().Length > 3) canwork = true;
            }
            catch
            {
                try
                {
                    if (GlobalJSONData.contentJObject.ToString().Length > 3) canwork = true;
                }
                catch
                {
                }
            }
            loadingstring = "Hidden";
            FileOpenCommand = new RelayCommand(() => ExecuteFileOpenCommand());
            urlmodeornot = true;
            canuseornot = true;
            if (GlobalJSONData.filepath != null)
            {
                if (GlobalJSONData.filepath.Length > 3)
                {
                    FilePath = GlobalJSONData.filepath;
                    System.Console.WriteLine("prevfilepath: " + FilePath);
                }
            }
            if (GlobalJSONData.nowencoding == Encoding.UTF8) currentencoding = "UTF8->EUC-KR";
            else currentencoding = "EUC-KR->UTF-8";
            GlobalJSONData.filepath = "";
            dlg = new OpenFileDialog();
            StartTreeView();
        }

        private void SetConnection()
        {
            StringBuilder reader = new StringBuilder();
            string IP = "";
            string Catalog = "";
            string ID = "";
            string PW = "";
            try
            {
                FileStream fs = new FileStream("C:\\test\\data.ini", FileMode.Open);
                long x=GetPrivateProfileString("DATA", "IP", "0.0.0.0", reader, 1024, "C:\\test\\data.ini");
                Console.WriteLine(x);
                IP = reader.ToString();
                reader.Clear();
                GetPrivateProfileString("DATA", "Catalog", "NONE", reader, 32, "C:\\test\\data.ini");
                Catalog = reader.ToString();
                reader.Clear();
                GetPrivateProfileString("DATA", "ID", "NONE", reader, 32, "C:\\test\\data.ini");
                ID = reader.ToString();
                reader.Clear();
                GetPrivateProfileString("DATA", "PW", "NONE", reader, 32, "C:\\test\\data.ini");
                PW = reader.ToString();
                reader.Clear();
            }
            catch
            {
                MessageBox.Show("C:\\test\\data.ini 파일에서 값을 읽을 수 없습니다.");
                WritePrivateProfileString("DATA", "IP", "123.123.123.123", "C:\\test\\data.ini");
                WritePrivateProfileString("DATA", "Catalog", "xxxxx", "C:\\test\\data.ini");
                WritePrivateProfileString("DATA", "ID", "xxxxx", "C:\\test\\data.ini");
                WritePrivateProfileString("DATA", "PW", "xxxxxxxxxxx", "C:\\test\\data.ini");
                Application.Current.Shutdown();
                return;
            }
            connectionstringproto = string.Format(connectionstringproto,IP,Catalog,ID,PW);
            if(MessageBox.Show(connectionstringproto + "\n을 이용하여 DB에 접속을 시도합니다. 계속 하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool result = MainDAO.InjectConnectionString(connectionstringproto);
                if (result == false)
                {
                    MessageBox.Show("올바르지 않은 주소와 ID, 비밀번호입니다.");
                    Application.Current.Shutdown();
                    return;
                }
            }
            else
            {
                Application.Current.Shutdown();
                return;
            }
            
        }


        private void SetTable()
        {
            DataSet ds = MainDAO.GetTableList();
            try
            {
                foreach (DataRow x in ds.Tables[0].Rows)
                {
                    if (x["TABLE_NAME"] != null)
                    {
                        Console.WriteLine("Table exists : " + x["TABLE_NAME"]);
                        TableNames.Add(x["TABLE_NAME"].ToString());
                    }
                }
                nowtable = TableNames[0];
                ExecuteTableChangeCommand();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder val, int size, string filepath);
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        #endregion constructor

        #region executecommands

        private void ExecuteLoadedCommand(object obj)
        {
            if (obj is TabControl)
            {
                mainTabControl = obj as TabControl;
                ResetTabPage();
            }
            else
            {
                return;
            }
        }

        private void ExecuteTableChangeCommand()
        {
            MainDAO.SelectTable(nowtable);
            mergeable = "False";
            Items = MainDAO.GetEveryItem().Tables[0];
            if (mainTabControl != null)
            {
                mainTabControl.Items.Clear();
                mainTabControl.SelectedIndex = 0;
                if (Columns != null) Columns.Clear();
                ResetTabPage();
            }
        }

        private void ExecuteTableChangeCommand(ComboBox cb)
        {
            MainDAO.SelectTable(nowtable);
            mergeable = "False";
            Items = MainDAO.GetEveryItem().Tables[0];
            if (mainTabControl != null)
            {
                mainTabControl.Items.Clear();
                mainTabControl.SelectedIndex = 0;
                if (Columns != null) Columns.Clear();
                ResetTabPage();
                cb.SelectedIndex = 0;
            }
        }

        private void ExecuteTryMergeCommand()
        {
            if (MergeItems != null)
            {
                int i = 0;
                Collection<string> uppercolumns = new Collection<string>();
                foreach (string x in Columns)
                {
                    uppercolumns.Add(x.ToUpper());
                }
                foreach (TreeNode x in MergeItems)
                {
                    Collection<KeyValuePair<string, string>> items = new Collection<KeyValuePair<string, string>>();
                    foreach (TreeNode y in x.Children)
                    {
                        if (uppercolumns.Contains(y.Key))
                        {
                            items.Add(new KeyValuePair<string, string>(y.Key, y.Value));
                        }
                    }
                    bool b = MainDAO.NewItem(items);
                    if (b) i++;
                }
                MessageBox.Show(MergeItems.Count + "건 중 " + i + " 건이 정상적으로 삽입되었습니다.");
                Items = MainDAO.GetEveryItem().Tables[0];
            }
            else return;
        }

        void ExecuteTryUpdateCommand()
        {
            if (MergeItems != null)
            {
                int i = 0;
                Collection<string> uppercolumns = new Collection<string>();
                foreach (string x in Columns)
                {
                    uppercolumns.Add(x.ToUpper());
                }
                foreach (TreeNode x in MergeItems)
                {
                    Collection<KeyValuePair<string, string>> items = new Collection<KeyValuePair<string, string>>();
                    foreach (TreeNode y in x.Children)
                    {
                        if (uppercolumns.Contains(y.Key))
                        {
                            items.Add(new KeyValuePair<string, string>(y.Key, y.Value));
                        }
                    }
                    bool b = MainDAO.UpdateItemundercolumn(items,Columns[0]);
                    if (b) i++;
                }
                MessageBox.Show(MergeItems.Count + "건 중 " + i + " 건이 정상적으로 수정되었습니다.");
                Items = MainDAO.GetEveryItem().Tables[0];
            }
            else return;
        }

        private void ExecuteAddCommand()
        {
            Collection<KeyValuePair<string, string>> values = ValueExtract();
            if (values != null)
            {
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    Console.WriteLine("kvp - " + kvp.Key + " " + kvp.Value);
                }
                bool res = MainDAO.NewItem(values);
                if (res == true)
                {
                    MessageBox.Show("새로운 행을 추가했습니다.");
                    Items = MainDAO.GetEveryItem().Tables[0];
                }
                else MessageBox.Show("행 추가를 할 수 없습니다. 테이블의 기본키 컬럼에서 동일한 값을 넣은 것이 아닌지 확인해 보십시오.");
            }
            else return;
        }

        private void ExecuteModifyCommand()
        {
            Collection<KeyValuePair<string, string>> values = ValueExtract();
            if (values != null)
            {
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    Console.WriteLine("kvp - " + kvp.Key + " " + kvp.Value);
                }
                bool res = MainDAO.UpdateItemundercolumn(values, nowcolumnname); //(values);
                if (res == true)
                {
                    MessageBox.Show("행을 정상적으로 변경했습니다.");
                    Items = MainDAO.GetEveryItem().Tables[0];
                }
                else MessageBox.Show("행을 변경할 수 없습니다. 기준이 되는 컬럼 값이나, 테이블의 기본키를 변경한게 아닌지 확인해보십시오.");
            }
            else return;
        }

        private void ExecuteSearchCommand()
        {
            DataSet ds = MainDAO.GetCertainItembyCertainColumn(nowcolumnname, selectedcolumn);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 1) MessageBox.Show("하나 이상의 행이 검색되었습니다. 검색하는데 쓰인 값이 기본키가 아닌 것 같습니다.\n첫 번째로 검색된 값만 사용합니다.");
                else if (ds.Tables[0].Rows.Count < 1)
                {
                    MessageBox.Show("검색된 값이 없습니다.");
                    return;
                }
                ValueInject(ds.Tables[0].Rows[0]);
            }
            else
            {
                string res1 = nowcolumnname;
                string res2 = selectedcolumn;
                if (res1 == null) res1 = "(선택되지 않음)";
                if (res2 == null) res2 = "(입력되지 않음)";
                MessageBox.Show("컬럼 " + res1 + "의 값 " + res2 + "은 검색되지 않았습니다.");
            }
        }

        private void ExecuteDeleteCommand()
        {
            DataSet ds = MainDAO.GetCertainItembyCertainColumn(nowcolumnname, selectedcolumn);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 1)
                {
                    MessageBox.Show("복수의 행이 검색되었습니다. 검색하는데 쓰인 컬럼이 기본키가 아닌 것 같습니다.\n두 개 이상의 값이 검색되었을 경우엔 삭제할 수 없습니다.");
                    return;
                }
                else if (ds.Tables[0].Rows.Count < 1)
                {
                    MessageBox.Show("검색된 값이 없습니다.");
                    return;
                }
                foreach (DataColumn x in ds.Tables[0].Columns)
                {
                    Console.WriteLine("result Column:" + x.ColumnName);
                    Console.WriteLine("result Row:" + ds.Tables[0].Rows[0][x.ColumnName]);
                }
                MainDAO.DeleteItembyCertainColumn(nowcolumnname, selectedcolumn);
                MessageBox.Show("삭제 작업이 완료되었습니다.");
                Items = MainDAO.GetEveryItem().Tables[0];
            }
            else
            {
                string res1 = nowcolumnname;
                string res2 = selectedcolumn;
                if (res1 == null) res1 = "(선택되지 않음)";
                if (res2 == null) res2 = "(입력되지 않음)";
                MessageBox.Show("컬럼 " + res1 + "의 값 " + res2 + "은 검색되지 않았습니다.");
            }
        }

        private void ExecuteClickCommand()
        {
            Console.WriteLine(nowindex.GetType());
            DataRowView drv = nowindex as DataRowView;
            ValueInject(drv.Row);
        }

        private void ExecuteTreeClickCommand(TreeNode node)
        {
            MergeItems = null;
            MergeNotify = "";
            mergeable = "False";
            Console.WriteLine("Node Key: " + node.Key + " Node Value: " + node.Value + " Node Children Count: " + node.Children.Count);
            if (node.Children.Count > 0) Console.WriteLine("Grandchild Node Children Count: " + node.Children[0].Children.Count);
            if (node.Children.Count == 0) return;
            if (node.Children[0].Children.Count == node.Children[1].Children.Count)
            {
                Console.WriteLine("배열형 추측 가능");

                bool decide = false;
                foreach (TreeNode x in node.Children[0].Children)
                {
                    if (Columns.Contains(x.Key))
                    {
                        decide = true;
                        break;
                    }
                }
                if (decide)
                {
                    Console.WriteLine("겹치는 열 이름 있음. 대기 시작");
                    MergeNotify = "테이블 " + nowtable + " - 현재 노드 " + node.Key;
                    MergeItems = node.Children;
                    mergeable = "True";
                }
            }
        }


        #endregion executecommands

        #region methods

        void ResetTabPage()
        {
            DataColumnCollection dcc = Items.Columns;
            foreach (DataColumn x in dcc)
            {
                Columns.Add(x.ColumnName);
            }
            TabItem ti;
            Grid gr;
            WrapPanel wp = null;
            WrapPanel wpe;
            TextBox tx;
            Label lb;
            for (int i = 0; i < dcc.Count; i++)
            {
                if (i % 8 == 0)
                {
                    ti = new TabItem();
                    gr = new Grid();
                    wp = new WrapPanel();
                    wp.Orientation = Orientation.Vertical;
                    wp.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    wp.Height = 234;
                    ti.Header = "Pg" + ((int)i / 8 + 1).ToString();
                    ti.Content = gr;
                    gr.Children.Add(wp);
                    mainTabControl.Items.Add(ti);
                }
                if (wp != null)
                {
                    wpe = new WrapPanel();
                    wpe.Orientation = Orientation.Horizontal;
                    tx = new TextBox();
                    tx.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    tx.Height = 23;
                    tx.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    tx.Width = 190;
                    tx.MaxWidth = 190;
                    tx.Name = "TxtBox";
                    tx.AcceptsReturn = true;
                    tx.AcceptsTab = true;

                    lb = new Label();
                    lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    lb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    lb.Content = dcc[i].ToString();
                    wpe.Name = lb.Content.ToString();
                    lb.FontSize = 10;
                    lb.MaxWidth = 100;
                    lb.MinWidth = 100;
                    wpe.Children.Add(lb);
                    wpe.Children.Add(tx);
                    wp.Children.Add(wpe);
                }
            }
        }

        private void ValueInject(DataRow drv)
        {
            DataColumnCollection dcc = Items.Columns;
            mainTabControl.ApplyTemplate();

            for (int i = 0; i < dcc.Count; i++)
            {
                object find = LogicalTreeHelper.FindLogicalNode(mainTabControl, dcc[i].ToString());
                Console.WriteLine("now find name: " + dcc[i].ToString());
                if (find != null)
                {
                    WrapPanel wpe = find as WrapPanel;
                    TextBox txtbox = LogicalTreeHelper.FindLogicalNode(wpe, "TxtBox") as TextBox;
                    Console.WriteLine("now text " + drv[dcc[i].ToString()]);
                    Console.WriteLine("now text type " + drv[dcc[i].ToString()].GetType());
                    if (drv[dcc[i].ToString()] != null && drv[dcc[i].ToString()].ToString().Length > 0)
                        txtbox.Text = drv[dcc[i].ToString()].ToString();
                    else
                        txtbox.Text = "";
                }
            }
            nowtab = 0;
        }

        private Collection<KeyValuePair<string, string>> ValueExtract()
        {
            DataColumnCollection dcc = Items.Columns;
            Collection<KeyValuePair<string, string>> values = new Collection<KeyValuePair<string, string>>();

            mainTabControl.ApplyTemplate();

            for (int i = 0; i < dcc.Count; i++)
            {
                object find = LogicalTreeHelper.FindLogicalNode(mainTabControl, dcc[i].ToString());
                Console.WriteLine("now find name: " + dcc[i].ToString());
                if (find != null)
                {
                    WrapPanel wpe = find as WrapPanel;
                    TextBox txtbox = LogicalTreeHelper.FindLogicalNode(wpe, "TxtBox") as TextBox;
                    Console.WriteLine("now text " + txtbox.Text);
                    if (txtbox.Text != null && txtbox.Text.Length > 0)
                        values.Add(new KeyValuePair<string, string>(dcc[i].ToString(), txtbox.Text));
                    else continue;
                }
            }
            return values;
        }

        #endregion methods
    }
}