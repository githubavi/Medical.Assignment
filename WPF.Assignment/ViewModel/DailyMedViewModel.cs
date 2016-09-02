using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Windows.Data;
using System.Threading;


namespace WPF.Assignment
{
    public class DailyMedViewModel : ViewModelBase, INotifyPropertyChanged, IDataErrorInfo
    {
        string basefeedUri = string.Empty;
        string suggestUri = string.Empty;
        List<Item> feedData;
        List<Suggest> suggestData;
        public ICommand NavigateCommand { get; private set; }
        public ICommand ShowResultsCommand { get; private set; }
        public ICommand SelectSuggestionBoxCommand { get; private set; }
        public DelegateCommand TestCommand { get; private set; }

        IView view;

        public DailyMedViewModel(IView view) 
        {
            //basefeedUri = Util.BaseUri;
            //suggestUri = Util.SuggestUri;

            IsNLMSearchSelected = true;
            IsGoogleSearchSelected = false;
            IsStatusBarVisible = false;
            IsMessagePanelVisible = false;

            this.NavigateCommand = new DelegateCommand<string>(Navigate);
            this.ShowResultsCommand = new DelegateCommand(LoadFeedAPI);
            this.SelectSuggestionBoxCommand = new DelegateCommand(() =>
            {
                if (this.SuggestData != null && isSuggestVisible)
                {
                    view.SetSuggestionBox();
                    this.SelectedSuggest = this.SuggestData.First(); //initializing selection with first item
                    ShowEntity(new EventArgs());
                }
            });

            

            this.view = view;
            //this.OnShowEvent += HandleShow;

            //compose parts
            AssemblyCatalog catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            CompositionContainer container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(this);
            container.Compose(batch);
        }

        private void HandleShow(object sender, EventArgs e)
        {
            MessageBox.Show("ShowEvet fired!!");
        }

        [Import]
        IWindowModel model;

        private void Navigate(string link)
        {
            Process.Start(link);
            if (view != null)
                view.SetCursor();
        }

        bool nlmSelected;
        public bool IsNLMSearchSelected
        {
            get
            {
                return nlmSelected;
            }
            set
            {
                nlmSelected = value;
                NotifyPropertyChanged("IsNLMSearchSelected");
            }
        }

        bool googselected;
        public bool IsGoogleSearchSelected
        {
            get
            {
                return googselected;
            }
            set
            {
                googselected = value;
                NotifyPropertyChanged("IsGoogleSearchSelected");
            }
        }

        bool stbarvisible;
        public bool IsStatusBarVisible
        {
            get
            {
                return stbarvisible;
            }
            set
            {
                stbarvisible = value;
                NotifyPropertyChanged("IsStatusBarVisible");
            }
        }

        bool msgPanelVisible;
        public bool IsMessagePanelVisible
        {
            get
            {
                return msgPanelVisible;
            }
            set
            {
                msgPanelVisible = value;
                NotifyPropertyChanged("IsMessagePanelVisible");
            }
        }

        string msg = string.Empty;
        public string Message
        {
            get
            {
                return msg;
            }
            set
            {
                msg = value;
                NotifyPropertyChanged("Message");
            }
        }

        private void LoadSuggestData(string query)
        {
            IsMessagePanelVisible = false;

            if(IsNLMSearchSelected)
            {
                //basefeedUri = Util.NLMSearchUri;
                suggestUri = Util.NLMSuggestUri;
            }
            else
            {
                //basefeedUri = Util.BaseUri;
                suggestUri = Util.SuggestUri;
            }

            if (suggestUri == null)
                throw new ValidationException("suggestUri is null, can not be found in config");

            string actualQuery = string.Format(suggestUri, query);
            WebRequest request = HttpWebRequest.Create(actualQuery);
            request.Credentials = CredentialCache.DefaultCredentials;

            IsStatusBarVisible = true;

            Task.Factory.StartNew<IEnumerable<SuggestDataEntity>>(() =>
                {
                    IEnumerable<SuggestDataEntity> data = new List<SuggestDataEntity>();
                    
                    try
                    {
                        if (!IsNLMSearchSelected)
                            data = model.GetSuggestDataXml(XDocument.Load(new StreamReader(request.GetResponse().GetResponseStream())));
                        else
                            data = model.GetSuggestDataString(request.GetResponse().GetResponseStream());
                    }
                    catch (Exception ex)
                    {
                        SynchronizationContext.Current.Post(new SendOrPostCallback((p) =>
                            {
                                IsMessagePanelVisible = true;
                                Message = ex.Message;
                                Util.LogMessage(ex);
                            }), null);
                    }
                    return data;
                }).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        IsMessagePanelVisible = true;
                        Message = task.Exception.Message;
                        Util.LogMessage(task.Exception);
                    }
                    else if (task.Result != null)
                    {
                        var feed = task.Result.ToList().FirstOrDefault();
                        if (feed != null)
                            this.SuggestData = new List<Suggest>(feed.Items);
                    }
                    else
                        this.SuggestData = null;

                    IsStatusBarVisible = false;
                        
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void LoadFeedAPI()
        {
            if(this.SelectedSuggest != null)
                this.Query = this.SelectedSuggest.Name;

            //make suggest window hide
            isSuggestVisible = false;
            NotifyPropertyChanged("IsSuggestVisible");

            if (view != null)
                view.SetCursor();

            if (IsNLMSearchSelected)
            {
                basefeedUri = Util.NLMSearchUri;
            }
            else
            {
                basefeedUri = Util.BaseUri;
            }

            IsMessagePanelVisible = false;
            
            string actualQuery = string.Format(basefeedUri, this.Query);
            WebRequest request2 = HttpWebRequest.Create(actualQuery);
            request2.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request2).UserAgent = ".NET Client";
            request2.Timeout = 60000;

            IsStatusBarVisible = true;

            Task.Factory.StartNew<SearchResult>(() =>
                {
                    SearchResult data = new SearchResult();
                    try
                    {
                        if (IsNLMSearchSelected)
                        {
                            data = model.GetQueryDataNLM(request2.GetResponse().GetResponseStream(), this.Query);
                        }
                        else
                        {
                            var stream = request2.GetResponse().GetResponseStream();
                            data = model.GetQueryData(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        SynchronizationContext.Current.Post(new SendOrPostCallback((p) =>
                        {
                            IsMessagePanelVisible = true;
                            Message = ex.Message;
                            Util.LogMessage(ex);
                        }), null);
                    }
                    return data;
                }).ContinueWith(task =>
                    {
                        IsStatusBarVisible = false;

                        if (task.Exception != null)
                        {
                            IsMessagePanelVisible = true;
                            Message = task.Exception.Message;
                            Util.LogMessage(task.Exception);
                        }
                        else if (task.Result != null && task.Result.Results != null)
                        {
                            var feeds = task.Result.Results.ToList();
                            if (feeds != null)
                                this.FeedData = new List<Item>(feeds);
                        }
                        
                            
                    },TaskScheduler.FromCurrentSynchronizationContext());
        }

        bool itemSelected = false;

        bool isSuggestVisible = true;
        public bool IsSuggestVisible
        {
            get { return isSuggestVisible; }
        }

        Suggest selectedSuggest;
        public Suggest SelectedSuggest
        {
            get
            {
                return selectedSuggest;
            }
            set
            {
                if (value != null)
                {
                    selectedSuggest = value;
                    itemSelected = true;

                    this.Query = selectedSuggest.Name;

                    NotifyPropertyChanged("SelectedSuggest");
                }
            }
        }

        public List<Suggest> SuggestData
        {
            get
            {
                return suggestData;
            }
            set
            {
                suggestData = value;
                NotifyPropertyChanged("SuggestData");
            }
        }

        public List<Item> FeedData
        {
            get
            {
                return feedData == null ? null : (feedData = new List<Item>(feedData));
            }
            set{
                feedData = value;
                NotifyPropertyChanged("FeedData");
            }
        }

        private string query = string.Empty;
        public string Query
        {
            get
            {
                return query;
            }
            set
            {
                query = value;

                
                if (itemSelected)
                    //reset flag
                    itemSelected = false;
                else
                {
                    if (!isSuggestVisible)
                    {
                        //make suggest window visible
                        isSuggestVisible = true;
                        NotifyPropertyChanged("IsSuggestVisible");
                    }

                    if (!string.IsNullOrEmpty(query) && query.Length >= 4)
                        LoadSuggestData(query);
                }
                NotifyPropertyChanged("Query");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get { 
                    string result = string.Empty;
                    if (string.Compare(columnName, "Query", true) == 0)
                    {
                        if (!string.IsNullOrEmpty(this.Query) && !Util.IsValid(this.Query))
                            result = "Please enter valid characters";
                    }
                    this.SearchToolTipText = result;
                    return result;
            }
        }

        private string tooltiptext;
        public string SearchToolTipText
        {
            get
            {
                return string.IsNullOrEmpty(tooltiptext) ? null : tooltiptext;
            }
            set
            {
                tooltiptext = value;
                NotifyPropertyChanged("SearchToolTipText");
            }
        }
    }
}
