using System;

namespace Notification
{
    public class NotificationService
    {
        private void Notify(string input, int type)
        {
            string NotificationTextThing = input;
            string Toast = "";
            switch (type)
            {
                case 1:
                    {
                        //Basic Toast
                        Toast = "<toast><visual><binding template=\"ToastImageAndText01\"><text id = \"1\" >";
                        Toast += NotificationTextThing;
                        Toast += "</text></binding></visual></toast>";
                        break;
                    }
                default:
                    {
                        Toast = "<toast><visual><binding template=\"ToastImageAndText01\"><text id = \"1\" >";
                        Toast += "Default Text String";
                        Toast += "</text></binding></visual></toast>";
                        break;
                    }
            }
            XmlDocument tileXml = new XmlDocument();
            tileXml.LoadXml(Toast);
            var toast = new ToastNotification(tileXml);
            ToastNotificationManager.CreateToastNotifier("New Toast Thing").Show(toast);
        }

    }
}
