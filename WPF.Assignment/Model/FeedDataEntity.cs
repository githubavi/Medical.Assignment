using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WPF.Assignment
{
    [DataContract]
    public class ResponseData
    {

        [DataMember(Name = "responseData")]
        public SearchResult Response { get; set; }
    }

    [DataContract]
    public class SearchResult
    {
        [DataMember(Name = "results")]
        public IList<Item> Results { get; set; }
    }

    [DataContract]
    public class Item
    {
        string title;
        [DataMember(Name = "titleNoFormatting")]
        public string Title
        {
            get
            {
                return System.Net.WebUtility.HtmlDecode(title);
            }
            set
            {
                title = value;
            }
        }

        [DataMember(Name = "url")]
        public string Link { get; set; }

        [DataMember(Name = "unescapedUrl")]
        public string UnEscapedLink { get; set; }

        string content = string.Empty;
        [DataMember(Name = "content")]
        public string Description
        {
            get
            {
                return System.Net.WebUtility.HtmlDecode(content).Replace("<b>","")
                                                                .Replace("</b>","")
                                                                .Replace("</br>","")
                                                                .Replace("<br>","")
                                                                .Replace("&amp","&")
                                                                .Replace("&quot",@"""");
            }
            set
            {
                content = value;
            }
        }
        
    }
}
