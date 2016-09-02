using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WPF.Assignment
{
    [Export(typeof(IWindowModel))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public class DailyMedModel : IWindowModel
    {
        public DailyMedModel() { }
       
        
        public SearchResult GetQueryData(Stream streamData)
        {
            if (!streamData.CanRead || streamData == null)
                throw new InvalidDataException("Data is not valid");

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ResponseData));
            return ((ResponseData)serializer.ReadObject(streamData)).Response;
        }

        public SearchResult GetQueryDataNLM(Stream streamData, string searchtext)
        {
            if (!streamData.CanRead || streamData == null)
                throw new InvalidDataException("Data is not valid");
            
            SearchResult res = new SearchResult();

            try
            {
                using (StreamReader reader = new StreamReader(streamData))
                {
                    var data = reader.ReadToEnd();

                    var matchedurls = Regex.Matches(data, @"\bsetid=[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    res.Results = new List<Item>();

                    foreach (Match murl in matchedurls)
                    {
                        if (murl.Groups.Count > 0)
                        {
                            var sLink = string.Format(Util.NLMLookUpBaseUri, murl.Groups[0].ToString());
                            var item = new Item { Link = sLink, UnEscapedLink = sLink, Description = string.Empty, 
                                Title = string.Format("Daily Medicine Reference for {0}", searchtext) };
                            res.Results.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return res;
        }

        public IEnumerable<SuggestDataEntity> GetSuggestDataString(Stream streamData)
        {
            if (!streamData.CanRead || streamData == null)
                throw new InvalidDataException("Data is not valid");
                
                try
                {
                    using (StreamReader reader = new StreamReader(streamData))
                    {
                        var nlmsuggestdata = reader.ReadToEnd();
                        //parse newlines
                        var suggests = nlmsuggestdata.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                            .Select(s => new Suggest { Name = s, NumResult = "20" });
                        return new List<SuggestDataEntity> { new SuggestDataEntity { Items = suggests } };
                    }
                }
                catch(Exception ex)
                {
                    throw;
                }
            
        }

        public IEnumerable<SuggestDataEntity> GetSuggestDataXml(XDocument xdoc)
        {
            if (xdoc != null)
            {
                return from channels in xdoc.Descendants("toplevel")
                       select new SuggestDataEntity
                       {
                           Items = from items in channels.Descendants("CompleteSuggestion")
                                   select new Suggest
                                   {
                                       Name = items.Element("suggestion") != null ? items.Element("suggestion").Attribute("data").Value : string.Empty,
                                       NumResult = items.Element("num_queries") != null ? items.Element("num_queries").Attribute("int").Value : "0"
                                   }
                       };
            }
            else
                return null;
        }
    }
}
