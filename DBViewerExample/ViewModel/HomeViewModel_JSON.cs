using DBViewerExample.Model;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DBViewerExample.ViewModel
{
    public partial class HomeViewModel : ViewModelBase
    {
        #region properties and variables

        private OpenFileDialog dlg;

        public bool canwork { get; set; }

        private bool _canuseornot;

        public bool canuseornot
        {
            get
            {
                return _canuseornot;
            }
            set
            {
                Set(nameof(canuseornot), ref _canuseornot, value);
            }
        }

        private bool _urlmodeornot;

        public bool urlmodeornot
        {
            get
            {
                return _urlmodeornot;
            }
            set
            {
                Set(nameof(urlmodeornot), ref _urlmodeornot, value);
            }
        }

        private string _currentencoding;

        public string currentencoding
        {
            get
            {
                return _currentencoding;
            }
            set
            {
                Set(nameof(currentencoding), ref _currentencoding, value);
            }
        }

        private string _FilePath;

        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                Set(nameof(FilePath), ref _FilePath, value);
            }
        }

        private string _loadingstring;

        public string loadingstring
        {
            get
            {
                return _loadingstring;
            }
            set
            {
                Set(nameof(loadingstring), ref _loadingstring, value);
            }
        }

        private string _mergenotify;

        public string MergeNotify
        {
            get
            {
                return _mergenotify;
            }
            set
            {
                Set(nameof(MergeNotify), ref _mergenotify, value);
            }
        }

        private ObservableCollection<TreeNode> _TreeViewItems;

        public ObservableCollection<TreeNode> TreeViewItems

        {
            get
            {
                return _TreeViewItems;
            }

            set
            {
                Set(nameof(TreeViewItems), ref _TreeViewItems, value);
            }
        }

        private Collection<string> names;

        private ObservableCollection<TreeNode> MergeItems;

        public ICommand FileOpenCommand { get; set; }

        #endregion properties and variables

        #region executecommands

        private void ExecuteFileOpenCommand()
        {
            loadingstring = "Visible";
            GlobalJSONData.prevURL = "";
            InitializeJSONbyFile();
            StartTreeView();
            loadingstring = "Hidden";
        }

        #endregion executecommands

        #region methods

        private void StartTreeView()
        {
            if (GlobalJSONData.contentJObject == null && GlobalJSONData.contentJArray == null) return;
            TreeNode root;
            if (GlobalJSONData.Type == 0)
            {
                if (GlobalJSONData.contentJObject == null) return;
                root = MakeTreeDataChildren(GlobalJSONData.contentJObject);
                TreeViewItems = root.Children;
                SetCountforChildrens(root);
            }
            else
            {
                if (GlobalJSONData.contentJArray == null) return;
                root = MakeTreeDataChildren(GlobalJSONData.contentJArray);
                TreeViewItems = root.Children;
                SetCountforChildrens(root);
            }
            foreach (string x in names)
            {
                Console.WriteLine("name : " + x);
            }
        }

        private void SetCountforChildrens(TreeNode parent)
        {
            parent.setCount();
            foreach (TreeNode i in parent.Children) SetCountforChildrens(i);
        }

        private TreeNode MakeTreeDataChildrenWork(object node, TreeNode result)
        {
            TreeViewItems = new ObservableCollection<TreeNode>();
            if (node is JProperty)
            {
                JProperty jp = (JProperty)node;
                result.Key = jp.Name;
                names.Add(result.Key);
                result.Children = MakeTreeDataChildren(jp).Children;
                return result;
            }
            else if (node is JValue)
            {
                JValue jv = (JValue)node;

                if (jv.Value != null)
                    result.Value = jv.Value.ToString();
                else
                {
                    result.Value = "null";
                }

                return result;
            }
            else if (node is JObject)
            {
                JObject jo = (JObject)node;
                foreach (var x in jo)
                {
                    result.Children.Add(MakeTreeDataChildren(x.Value, x.Key));
                }
                return result;
            }
            else if (node is JArray)
            {
                JArray ja = (JArray)node;
                int i = 0;
                foreach (var x in ja)
                {
                    result.Children.Add(MakeTreeDataChildren(x, i));
                    i++;
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        private TreeNode MakeTreeDataChildren(object node, string keyname)
        {
            TreeNode result = new TreeNode();
            result.Key = keyname;
            if (names.Contains(result.Key) == false) names.Add(result.Key);
            return MakeTreeDataChildrenWork(node, result);
        }

        private TreeNode MakeTreeDataChildren(object node, int index)
        {
            TreeNode result = new TreeNode();
            result.Key = "Element " + index;
            return MakeTreeDataChildrenWork(node, result);
        }

        private TreeNode MakeTreeDataChildren(object node)
        {
            TreeNode result = new TreeNode();
            return MakeTreeDataChildrenWork(node, result);
        }

        private void InitializeJSONbyFile()
        {
            string importedfilestring = "";
            dlg.Reset();
            dlg.DefaultExt = ".JSON";
            dlg.Filter = "JSON files (*.JSON)|*.JSON";
            dlg.ShowDialog();
            if (dlg.FileName.Length > 3)
            {
                try
                {
                    GlobalJSONData.filepath = dlg.FileName;
                }
                catch
                {
                    MessageBox.Show("Please select JSON file before start.");
                    return;
                }
                finally
                {
                    dlg.Reset();
                }
            }
            else
            {
                MessageBox.Show("Please select file before start.");
                return;
            }

            using (FileStream fs = new FileStream(GlobalJSONData.filepath, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs, GlobalJSONData.nowencoding);
                importedfilestring = sr.ReadToEnd();
            }
            ParseJSON(importedfilestring);
        }

        private void ParseJSON(string importedstring)
        {
            try
            {
                GlobalJSONData.Type = 0;
                JObject obj = JObject.Parse(importedstring);
                GlobalJSONData.contentJObject = obj;
            }
            catch
            {
                try
                {
                    GlobalJSONData.Type = 1;
                    JArray obj = JArray.Parse(importedstring);
                    GlobalJSONData.contentJArray = obj;
                }
                catch
                {
                    try
                    {
                        GlobalJSONData.Type = 0;
                        importedstring = importedstring.Replace("\":\"\",\"", "\":\" \",\"");
                        importedstring = importedstring.Replace("\":\"\"", "\":\"");
                        importedstring = importedstring.Replace("\"\",\"", "\",\"");
                        importedstring = importedstring.Replace("\\", "\\\\");
                        importedstring = importedstring.Replace("'", "\\'");
                        string jsonResult = importedstring;
                        JObject obj = JObject.Parse(jsonResult);
                        GlobalJSONData.contentJObject = obj;
                    }
                    catch
                    {
                        try
                        {
                            GlobalJSONData.Type = 1;
                            importedstring = importedstring.Replace("\":\"\",\"", "\":\" \",\"");
                            importedstring = importedstring.Replace("\":\"\"", "\":\"");
                            importedstring = importedstring.Replace("\"\",\"", "\",\"");
                            importedstring = importedstring.Replace("\\", "\\\\");
                            importedstring = importedstring.Replace("'", "\\'");
                            string jsonResult = importedstring;
                            JArray obj = JArray.Parse(jsonResult);
                            GlobalJSONData.contentJArray = obj;
                        }
                        catch
                        {
                            MessageBox.Show("Please use valid JSON file");
                            GlobalJSONData.filepath = "";
                            GlobalJSONData.prevURL = "";
                            try
                            {
                                GlobalJSONData.contentJArray.Clear();
                            }
                            catch
                            {
                            }
                            try
                            {
                                GlobalJSONData.contentJObject.RemoveAll();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        #endregion methods
    }
}