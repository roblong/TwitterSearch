using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace TwitterSearch
{
    public class Twitter
    {
        public Twitter()
        {

                // TODO: Add constructor logic here

        }

        public class TwitSearchResult
        {

            public TwitSearchResult()
            {

                // TODO: Add constructor logic here

            }

            private string _UserName;
            private string _Status;
            private DateTime _CreatedAt;
            private string _ProfileImageUrl;
            private string _LanguageCode;
            private long _Id;


            public string UserName
            {
                get { return _UserName; }
                set { _UserName = value; }
            }

            public string Status
            {
                get { return _Status; }
                set { _Status = value; }
            }

            public string ProfileImageUrl
            {
                get { return _ProfileImageUrl; }
                set { _ProfileImageUrl = value; }
            }

            public DateTime CreatedAt
            {
                get { return _CreatedAt; }
                set { _CreatedAt = value; }
            }

            public string LanguageCode
            {
                get { return _LanguageCode; }
                set { _LanguageCode = value; }
            }

            public long Id
            {
                get { return _Id; }
                set { _Id = value; }
            }
        }


        public class TwitSearchDates
        {

            public TwitSearchDates()
            {

            }

            private DateTime _fromDate;
            private DateTime _toDate;
            private string _strFromDate;
            private string _strToDate;

            public DateTime FromDate
            {
                get { return _fromDate; }
                set { _fromDate = value; }
            }

            public DateTime ToDate
            {
                get { return _toDate; }
                set { _toDate = value; }
            }

            public string strFromDate
            {
                get { return _strFromDate; }
                set { _strFromDate = value; }
            }

            public string strToDate
            {
                get { return _strToDate; }
                set { _strToDate = value; }
            }


        }

        [DataContract]
        public class SearchResult
        {

            [DataMember(Name = "text", Order = 0)]
            public string Text { get; set; }

            [DataMember(Name = "to_user_id", Order = 1)]
            public long ToUserId { get; set; }

            [DataMember(Name = "from_user", Order = 2)]
            public string FromUser { get; set; }

            [DataMember(Name = "id", Order = 3)]
            public long Id { get; set; }

            [DataMember(Name = "from_user_id", Order = 4)]
            public long FromUserId { get; set; }

            [DataMember(Name = "iso_language_code", Order = 5)]
            public string IsoLanguageCode { get; set; }

            [DataMember(Name = "profile_image_url", Order = 6)]
            public string ProfileImageUrl { get; set; }

            [DataMember(Name = "created_at", Order = 7)]
            public string CreatedAt { get; set; }


        }

        [DataContract]
        public class SearchResults
        {
            public SearchResults()
            {
                this.Results = new List<SearchResult>();
            }

            [DataMember(Name = "results")]
            public List<SearchResult> Results { get; set; }

        }


    }
}