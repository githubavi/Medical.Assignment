using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace WPF.Assignment
{
    public interface IWindowModel
    {
        SearchResult GetQueryData(Stream streamData);
        IEnumerable<SuggestDataEntity> GetSuggestDataXml(XDocument xdoc);

        IEnumerable<SuggestDataEntity> GetSuggestDataString(Stream streamData);

        SearchResult GetQueryDataNLM(Stream streamData, string searchtext);
    }
}
