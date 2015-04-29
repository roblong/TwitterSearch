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
using TweetSharp;


namespace TwitterSearch
{
    public partial class form1 : Form
    {

        private const string TwitterJsonUrl = "http://search.twitter.com/search.json";
        private const string TwitterUser = "roblong99";
        private const string TwitterPass = "c90s90wfd0-dsf0-";
        private string searchterm;
        private string errormessage = "Oops,something bad happened.";
        private string AppName = "Twitter Search Tool";
        private string blanksearch = "Search/Folder name cannot be blank";
        private int numberresults;
        private string fileproblem = "problem with that file or folder";
        //private Twitter tweets = new Twitter();
        
        private Twitter.TwitSearchDates Dates = new Twitter.TwitSearchDates();
        
            
        public form1()
        {
           
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today.AddDays(-7);
            dateTimePicker2.Value = DateTime.Today;  
            

        }

        private void DoWork(string search, int searchlen)
        {
            List<Twitter.TwitSearchResult> userstatus = new List<Twitter.TwitSearchResult>();
            int pages;
            StreamWriter file;
            int totalcount = 0;
            
            //to do: create a file property
            string filePath = txtFilePath.Text + "\\" + txtFileName.Text + ".csv";

            //Get an encoded search query string
            search = EncodeUrl(search);

            //determine # of pages needed
           
            pages = GetPages(searchlen);
           
            if (pages>15)
            {
                pages = 15;
            }

            try
            {
                //Do the search
                for (int i = 1; i<=pages;i++)
                {
                    if (i == 1)
                    {
                        file = new StreamWriter(filePath, false);
                        file.Close();
                        file = File.AppendText(filePath);
                    }
                    else
                    {
                        file = File.AppendText(filePath);
                    }

                    userstatus = TwitterSearch(search, Convert.ToString(i));
                    totalcount = totalcount + userstatus.Count;
                    
                    //write to a file
                    CSVUtil csvwriter = new CSVUtil();
                    csvwriter.writetofile(userstatus, file);
                }
            }
            catch (System.Exception ex)
            {

                MessageBox.Show(
                       errormessage,
                       AppName,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk);
                return;
            }

            //Display a message
            DisplayMessage(totalcount, filePath);

        }

        private static void DisplayMessage(int tweetcount, string filepath)
        {
            Uri path = new Uri(filepath);

            string mess = "We found " + tweetcount.ToString() + " tweets." + "\r\n";
            if (tweetcount > 0)
            {
                
                MessageBox.Show(
                    mess + "Results are at: " + path.AbsolutePath.ToString(),
                    "Twitter Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }
            else
            {
                 MessageBox.Show(
                    mess,
                    "Twitter Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }

        }

        private int GetPages(int searchlength)
        {
            int numpages;
            int foo = searchlength % 100;
            searchlength = searchlength - foo;
            if (foo == 0)
            {
                numpages = searchlength / 100;
            }
            else
            {
                numpages = searchlength / 100 + 1;
            }
            return numpages;
        }

        public List<Twitter.TwitSearchResult> TwitterSearch(string search, string page)
        {
            
            string resultsstring = null;
          
                string url =
                    string.Format(TwitterJsonUrl + "?&q={0}" + "&rpp=100" + "&include_entities=true" + "&result_type=mixed" + "&since={1}" + "&until={2}" + "&page=" + page.ToString(), search, Dates.strFromDate, Dates.strToDate);

            //Returns the search results string
             resultsstring = BuildResultsString(url);


            try
            {
                //return the results in a UserStatus List object 
                List<Twitter.TwitSearchResult> users = GetSearchResultsObj(resultsstring);
                return users;
            }
            catch (System.Exception ex)
            {
                
                throw ex;
            }

        }

        //method to get the raw results string
        private string BuildResultsString(string url)
        {
            Stream resStream;
            string tempString = null;
            int count = 0;
            string response1;
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                resStream = response.GetResponseStream();
            }
            catch (System.Exception ex)
            {
                System.Exception argEx = new System.Exception("Something went wrong", ex);
                throw argEx;
            }

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            } while (count > 0); // any more data to read?

            response1 = sb.ToString();

            return response1;

        }

        //this puts the results into the UserStatus List object
        public List<Twitter.TwitSearchResult> GetSearchResultsObj(string response1)
        {
            byte[] brr = ASCIIEncoding.UTF8.GetBytes(response1);
            StreamReader reader = new StreamReader(new MemoryStream(brr));
            DataContractJsonSerializer serializer =
              new DataContractJsonSerializer(typeof(Twitter.SearchResults));

            Twitter.SearchResults searchResults =
              (Twitter.SearchResults)serializer.ReadObject(reader.BaseStream);
            reader.Close();

            List<Twitter.TwitSearchResult> users =
              (from u in searchResults.Results
               select new Twitter.TwitSearchResult
               {
                   UserName = u.FromUser,
                   ProfileImageUrl = u.ProfileImageUrl,
                   Status = HttpUtility.HtmlDecode(u.Text),
                   CreatedAt = DateTimeOffset.Parse(u.CreatedAt).UtcDateTime,
                   LanguageCode = u.IsoLanguageCode,
                   Id = u.Id
               }).ToList();

            return users;
        }


        //
        //
        // Everything below is misc button click handlers and functions
        ////////////////////////////////////////////////////////////////
        //
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //fromdate = dateTimePicker1.Value.Year.ToString() + "-" + dateTimePicker1.Value.Month.ToString() + "-" +
              //         dateTimePicker1.Value.Day.ToString();
            //todate = dateTimePicker2.Value.Year.ToString() + "-" + dateTimePicker2.Value.Month.ToString() + "-" +
              //         dateTimePicker2.Value.Day.ToString();
            if (txtSearch.Text.Length != 0 && txtFilePath.Text.Length != 0 && txtFileName.Text.Length != 0)
            {
                if (txtNumResults.Text.Length == 0)
                {
                    numberresults = 100;
                }
                else
                {
                    numberresults = Convert.ToInt32(txtNumResults.Text);
                    if (numberresults > 1500)
                    {
                        numberresults = 1500;
                    }

                }
                searchterm = txtSearch.Text;
                DoWork(searchterm, numberresults);
            }
            else
            {

                MessageBox.Show(
                       blanksearch,
                       AppName,
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Asterisk);
                return;
            }
        }


        //For encoding
        private static string[,] _chars = new string[,]
        {
        { "%", "%25" },     // this is the first one
        { "$" , "%24" },
        { "&", "%26" },
        { "+", "%2B" },
        { ",", "%2C" },
        { "/", "%2F" },
        { ":", "%3A" },
        { ";", "%3B" },
        { "=", "%3D" },
        { "?", "%3F" },
        { "@", "%40" },
        { " ", "%20" },
        { "\"" , "%22" },
        { "<", "%3C" },
        { ">", "%3E" },
        { "#", "%23" },
        { "{", "%7B" },
        { "}", "%7D" },
        { "|", "%7C" },
        { "\\", "%5C" },
        { "^", "%5E" },
        { "~", "%7E" },
        { "[", "%5B" },
        { "]", "%5D" },
        { "`", "%60" } };

        public static string EncodeUrl(string url)
        {
            for (int i = 0; i < _chars.GetUpperBound(0); i++)
                url = url.Replace(_chars[i, 0], _chars[i, 1]);

            return url;
        }

        public static string DecodeUrl(string url)
        {
            for (int i = 0; i < _chars.GetUpperBound(0); i++)
                url = url.Replace(_chars[i, 1], _chars[i, 0]);

            return url;
        }

        private void btnTweet1_Click(object sender, EventArgs e)
        {
            TweetSharp.TwitterService twit = new TwitterService("NzAtCXo7uJ8u8Xjn3oCig", "YJ2kCfRpzxHhw2d1WcjBKqbFeXdlIsXj2LZpyqZLAqc", "14263615-yWxdPSPCiMh4T3h92ebczn7XLTksFpJixmw7B7UvM", "kMYLyq58FYgO9aoY3NuYLeSOEGe1sO8XJCzZjogsW7w");
            string foo = "";
            string foo1;
            var tweets = twit.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
            foreach (var tweet in tweets)
            {
               foo1 = string.Format("{0}" + " says " + "{1}", tweet.User.ScreenName, tweet.Text);
               foo = foo + foo1 + "\n";
            }
            MessageBox.Show(
                           foo,
                           AppName,
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Asterisk);
            return;

           // twit.SendTweet(TwitterStatus SendTweet(new SendTweetOptions { Status = "Hello, world!" });
        }

        /// <summary>
        /// Gets/sets the character used for column delimiters.
        /// </summary>
        public char Delimiter
        {
            get { return SpecialChars[DelimiterIndex]; }
            set { SpecialChars[DelimiterIndex] = value; }
        }

        /// <summary>
        /// These are special characters in CSV files. If a column contains any
        /// of these characters, the entire column is wrapped in double quotes.
        /// </summary>
        protected char[] SpecialChars = new char[] { ',', '"', '\r', '\n' };

        // Indexes into SpecialChars for characters with specific meaning
        private const int DelimiterIndex = 0;
        private const int QuoteIndex = 1;

        private void button1_Click(object sender, EventArgs e)
        {
             DirectoryInfo dir2;

            try
            {
                if (String.IsNullOrEmpty(txtFilePath.Text))
                {

                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        txtFilePath.Text = folderBrowserDialog1.SelectedPath;
                    }
                }
                else
                {
                    try
                    {
                        dir2 = new DirectoryInfo(txtFilePath.Text);
                    }
                    catch (Exception e1)
                    {
                        MessageBox.Show(
                        fileproblem + txtFilePath.Text + "\n" + e1.Message,
                        AppName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                        return;
                    }

                    if (dir2.Exists)
                    {
                        folderBrowserDialog1.SelectedPath = dir2.FullName;
                        if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                        {
                            txtFilePath.Text = folderBrowserDialog1.SelectedPath;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "That file or directory does not exist.",
                            AppName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
                        return;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                       fileproblem + "\n" + exc.Message,
                         AppName,
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Asterisk);
                return;
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Navigate to a URL.
            if (txtFilePath.Text.Length != 0 && txtFileName.Text.Length != 0)
            {
                try
                {
                    //System.Diagnostics.Process.Start(@"C:\Users\roblong\Desktop\MyTwitterSearch.csv");
                    System.Diagnostics.Process.Start(txtFilePath.Text + @"\" + txtFileName.Text + ".csv");
                }
                catch
                {
                    MessageBox.Show(
                   "Oops, something bad happened.",
                   "TwitterSearch",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Asterisk);
                    return;
                }
            }
            else
            {
                MessageBox.Show(
                    "Do a search first, then I show results",
                    "TwitterSearch",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }
            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Dates.FromDate = dateTimePicker1.Value;
            Dates.strFromDate = dateTimePicker1.Value.Year.ToString() + "-" + dateTimePicker1.Value.Month.ToString() + "-" +
                      dateTimePicker1.Value.Day.ToString();
            
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Dates.ToDate = dateTimePicker2.Value;
            Dates.strToDate = dateTimePicker2.Value.Year.ToString() + "-" + dateTimePicker2.Value.Month.ToString() + "-" +
                      dateTimePicker2.Value.Day.ToString();
        }
        }

}
