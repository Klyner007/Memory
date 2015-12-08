using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Memory
{
    class Highscore
    {
        private List<User> highScore;
        private XmlDocument scoreXmlDocument;
        private static String xmlPath = @"Assets/Highscore/Highscore.xml";
        
        private class User
        {
            private int score;
            private string username;

            public int Score
            {
                get { return score; }
                set { score = value; }
            }

            public string Username
            {
                get { return username; }
                set { username = value; }
            }

            public User(string username, int score)
            {
                this.username = username;
                this.score = score;
            }
        }
        
        public void fetchHighscore()
        {
            scoreXmlDocument = new XmlDocument();
            XDocument doc = XDocument.Load(xmlPath);
            scoreXmlDocument.LoadXml(doc.ToString());
            string content = scoreXmlDocument.InnerXml;
            highScore = new List<User>();
            foreach (XmlNode userNode in scoreXmlDocument.GetElementsByTagName("user"))
            {
                String userName = userNode["username"].InnerText;
                String score = userNode["score"].InnerText;
                highScore.Add(new User((userName),
                Convert.ToInt32(score)));
            }
        }

        private void addRecord(String username, int score)
        {
            if (scoreXmlDocument.HasChildNodes)
            {
                using (FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    XmlNode userElement = scoreXmlDocument.CreateNode(XmlNodeType.Element, "user", null);//scoreXmlDocument.CreateElement("user");

                    XmlElement userNameElement = scoreXmlDocument.CreateElement("username");
                    XmlElement scoreElement = scoreXmlDocument.CreateElement("score");

                    userNameElement.InnerText = username;
                    scoreElement.InnerText = score.ToString();

                    userElement.AppendChild(userNameElement);
                    userElement.AppendChild(scoreElement);

                    scoreXmlDocument.DocumentElement.AppendChild(userElement);

                    scoreXmlDocument.Save(fs);
                }
            }
        }
    }
}
