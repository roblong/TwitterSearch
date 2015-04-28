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
    class CSVUtil
    {

        public void writetofile(List<Twitter.TwitSearchResult> users, StreamWriter file)
        {
            // write to a file
            string link;

            foreach (Twitter.TwitSearchResult u in users)
            {
                StringBuilder builder = new StringBuilder();
                string temp;

                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (u.UserName.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", u.UserName.Replace("\"", "\"\""));
                else
                {
                    builder.Append(u.UserName);
                    builder.Append(',');
                }

                builder.Append(u.CreatedAt);
                builder.Append(',');

                builder.Append(u.LanguageCode);
                builder.Append(',');

                builder.Append(u.Id);
                builder.Append(',');

                temp = u.Status;
                if (temp.IndexOfAny(new char[] { '\n' }) != -1)
                {
                    temp = temp.Replace("\n", " ").Trim();
                }

                if (temp.IndexOfAny(new char[] { '"', ',' }) != -1)
                {
                    builder.AppendFormat("\"{0}\"", temp.Replace("\"", "\"\""));
                    builder.Append(',');
                }
                else
                {
                    builder.Append(temp);
                    builder.Append(',');
                }

                Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
                MatchCollection ms = regx.Matches(u.Status);
                if (ms.Count > 0)
                {
                    link = ms[0].Value.ToString();
                }
                else
                {
                    link = "NA";
                }

                builder.Append(link);
                builder.Append(',');


                file.WriteLine(builder.ToString());
            }

            file.Close();
        }
    }

}
