using System;
using System.Collections.Generic;
using TwitterSearch;
using Xunit;

public class TwitterTests
{
    public class EmptyTestBoxTests
    {
        //test that GetSearchResults returns correct userstatus for a given known JSON input
        [Fact]
        public void TestKnownJSONInput()
        {
            //Arrange
            string jsonInput = "{\"completed_in\":0.047,\"max_id\":317531419280818176,\"max_id_str\":\"317531419280818176\",\"page\":1,\"query\":\"%22excel+services%22+since%3A2013-3-29+until%3A2013-3-31\",\"refresh_url\":\"?since_id=317531419280818176&q=%22excel%20services%22%20since%3A2013-3-29%20until%3A2013-3-31&result_type=mixed&include_entities=1\",\"results\":[{\"created_at\":\"Fri, 29 Mar 2013 06:59:24 +0000\",\"entities\":{\"hashtags\":[],\"urls\":[{\"url\":\"http://t.co/eWqi37v6An\",\"expanded_url\":\"http://sco.lt/8JFoTB\",\"display_url\":\"sco.lt/8JFoTB\",\"indices\":[115,137]}],\"user_mentions\":[{\"screen_name\":\"scoopit\",\"name\":\"Scoop.it\",\"id\":209484168,\"id_str\":\"209484168\",\"indices\":[106,114]}]},\"from_user\":\"PlexHosted\",\"from_user_id\":259377740,\"from_user_id_str\":\"259377740\",\"from_user_name\":\"PlexHosted\",\"geo\":null,\"id\":317531419280818176,\"id_str\":\"317531419280818176\",\"iso_language_code\":\"en\",\"metadata\":{\"result_type\":\"recent\"},\"profile_image_url\":\"http://a0.twimg.com/profile_images/1258848358/Logo_plex_ball_normal.jpg\",\"profile_image_url_https\":\"https://si0.twimg.com/profile_images/1258848358/Logo_plex_ball_normal.jpg\",\"source\":\"&lt;a href=&quot;http://www.scoop.it&quot;&gt;Scoop.it&lt;/a&gt;\",\"text\":\"Best Business Intelligence SharePoint 2013 Hosting Provider,PowerPivot SharePoint 2013 Hosting Provider | @scoopit http://t.co/eWqi37v6An\"}],\"results_per_page\":100,\"since_id\":0,\"since_id_str\":\"0\"}";
            //string jsonInput = "foo";

            form1 dog = new form1();

            //Act - return the results in a UserStatus List object 
            List<Twitter.TwitSearchResult> users = dog.GetSearchResultsObj(jsonInput);

            //Assert
            Assert.Equal(users.Count, 1);
            Assert.Equal(users[0].UserName, "PlexHosted");
            Assert.Equal(users[0].CreatedAt.ToString(), "3/29/2013 6:59:24 AM");
            Assert.Equal(users[0].Status, "Best Business Intelligence SharePoint 2013 Hosting Provider,PowerPivot SharePoint 2013 Hosting Provider | @scoopit http://t.co/eWqi37v6An");
            Assert.Equal(users[0].LanguageCode, "en");
            Assert.Equal(users[0].Id, 317531419280818176);
 
        }
    }
}

