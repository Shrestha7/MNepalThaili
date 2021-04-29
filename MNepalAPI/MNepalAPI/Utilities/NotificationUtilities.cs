using MNepalAPI.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static MNepalAPI.Models.Notifications;

namespace MNepalAPI.Utilities
{
    public class NotificationUtilities
    {
        public static int Notification(NotificationModel resNotificationInfo)
        {
            var objresPaypointNotificationModel = new NotificationUserModel();
            var objresPaypointNotificationInfo = new NotificationModel
            {
                imageName = resNotificationInfo.imageName,
                title = resNotificationInfo.title,
                text = resNotificationInfo.text,
                messageId = resNotificationInfo.messageId,
                redirectUrl = resNotificationInfo.redirectUrl,
                pushNotificationDate = resNotificationInfo.pushNotificationDate,
                
            };
            return objresPaypointNotificationModel.ResponseNotificationInfo(objresPaypointNotificationInfo);
        }


    }

}