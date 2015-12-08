using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace Memory.Helpers
{
    class ToastHelper
    {
        public static ToastNotification PopToast(string title, string content)
        {
            string xml = $@"<toast activationType='foreground'>
                                <visual>
                                    <binding template='ToastGeneric'>
                                    </binding>
                                </visual>
                            </toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var binding = doc.SelectSingleNode("//binding");

            var el = doc.CreateElement("text");
            el.InnerText = title;

            binding.AppendChild(el);

            el = doc.CreateElement("text");
            el.InnerText = content;
            binding.AppendChild(el);

            return PopCustomToast(doc);
        }
        
        public static ToastNotification PopCustomToast(XmlDocument doc)
        {
            ToastNotification toast = new ToastNotification(doc);

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            return toast;
        }
    }
}
