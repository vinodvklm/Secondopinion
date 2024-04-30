using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace CommonUtils
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            string attrval = name;
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        attrval = attr.Description;
                    }
                }
            }
            return attrval;
        }
        public static List<Utility> GetEnumList<T>()
        {
            List<Utility> list = new List<Utility>();
            var type = typeof(T);
            foreach (Enum e in Enum.GetValues(typeof(T)))
            {
                var desc = e.GetDescription();
                var text = e.ToString();
                list.Add
                (
                    new Utility
                    {
                        Key = desc,
                        Value = ((int)Enum.Parse(type, text)).ToString(),
                    }
                );
            }
            return list;
        }
        public static List<CollectionBase> GetEnumCollectionBase<T>()
        {
            List<CollectionBase> list = new List<CollectionBase>();
            var type = typeof(T);
            foreach (Enum e in Enum.GetValues(type))
            {
                var desc = e.GetDescription();
                var text = e.ToString();
                list.Add
                (
                    new CollectionBase
                    {
                        Text = desc,
                        Value = ((int)Enum.Parse(type, text)).ToString(),
                        ValueInt = ((int)Enum.Parse(type, text)),
                    }
                );
            }
            return list;
        }
    }
    public enum MessageType
    {
        [Description("Success")]
        Success = 1,
        [Description("Failed")]
        Failed,
        [Description("Warning")]
        Warning,
        [Description("Error")]
        Error,
        [Description("Message")]
        Message,
        [Description("Service error")]
        ServiceError,
        [Description("No records")]
        NoRecords,
        [Description("Unauthorized")]
        Unauthorized,
        [Description("Inactive")]
        Inactive,
        [Description("Exception")]
        Exception,
        [Description("Not Registered")]
        NotRegistered,
        [Description("Not Approved")]
        NotApproved,
    }
    public enum LoginType
    {
        [Description("Normal")]
        Normal = 1,
        //[Description("Facebook")]
        //Facebook,
        //[Description("Google")]
        //Google,
        //[Description("Twitter")]
        //Twitter,
        //[Description("LinkedIn")]
        //LinkedIn,
    }
    public enum LogonType
    {
        [Description("App")]
        App = 1,
        [Description("Web")]
        Web,
    }
    public enum Priority
    {
        [Description("Medium")]
        Medium = 1,
        [Description("Low")]
        Low,
        [Description("High")]
        High,
        [Description("Critical")]
        Critical,
    }
    public enum DateTimeValue
    {
        [Description("Older date")]
        OlderDate = 1,
        [Description("Newer date")]
        NewerDate,
        [Description("Today")]
        Today,
        [Description("Yesterday")]
        Yesterday,
        [Description("Tomorrow")]
        Tomorrow,
        [Description("This week")]
        ThisWeek,
        [Description("Last week")]
        LastWeek,
        [Description("Next week")]
        NextWeek,
        [Description("This month")]
        ThisMonth,
        [Description("Last month")]
        LastMonth,
        [Description("Next month")]
        NextMonth,
        [Description("This year")]
        ThisYear,
        [Description("Last year")]
        LastYear,
        [Description("Next year")]
        NextYear,
    }

    public enum CRUDType
    {
        //Create, Read, Update, Delete
        [Description("Create")]
        Create = 1,
        [Description("Read")]
        Read = 2,
        [Description("Update")]
        Update = 3,
        [Description("Delete")]
        Delete = 4,
    }
    //fileupload
    public enum UserType
    {
        User = 1,
    }
    public enum DocumentType
    {
        //Image, Audio, Video, Doc
        [Description("Image")]
        Image = 1,
        [Description("Audio")]
        Audio = 2,
        [Description("Video")]
        Video = 3,
        [Description("Doc")]
        Doc = 4,
    }
    public enum Role
    {
        [Description("None")]
        None = 0,
        [Description("Admin")]
        Admin = 1,
        [Description("Normal User")]
        NormalUser = 2,
        [Description("Normal User")]
        Manager = 3,
    }
    public enum NotificationType
    {
        [Description("ForceLogout")]
        ForceLogout = 1,
    }
    public enum UploadFileType
    {
        [Description("Product")]
        Product = 1,
    }
    public enum AuditAction
    {
        // Insert, Update, Delete, Other
        [Description("Insert")]
        Insert = 1,
        [Description("Update")]
        Update = 2,
        [Description("Delete")]
        Delete = 3,
        [Description("Logged In")]
        LoggedIn = 5,
        [Description("Logged Out")]
        LoggedOut = 6,
        [Description("Other")]
        Other = 4,
    }
    public enum Language
    {
        // Arabic, English
        [Description("English")]
        English = 1,
        [Description("Arabic")]
        Arabic = 2,
    }
    public enum ComparisonType
    {
        [Description("Less than")]
        LessThan = 1,
        [Description("Less than or equal to")]
        LessThanOrEqualTo = 2,
        [Description("Greater than")]
        GreaterThan = 3,
        [Description("Greater than or equal to")]
        GreaterThanOrEqualTo = 4,
        [Description("Equal to")]
        EqualTo = 5,
    }
    public enum Month
    {
        [Description("Jan")]
        January = 1,
        [Description("Feb")]
        February = 2,
        [Description("Mar")]
        March = 3,
        [Description("Apr")]
        April = 4,
        [Description("May")]
        May = 5,
        [Description("Jun")]
        June = 6,
        [Description("Jul")]
        July = 7,
        [Description("Aug")]
        August = 8,
        [Description("Sep")]
        September = 9,
        [Description("Oct")]
        October = 10,
        [Description("Nov")]
        November = 11,
        [Description("Dec")]
        December = 12,
    }
    public enum CounterType
    {
        [Description("Order")]
        Order = 1,
        [Description("Receipt")]
        Receipt = 2
    }
    public enum OrderStatus
    {
        [Description("New")]
        New = 1,
        //[Description("Rejected")]
        //Rejected = 6,
    }
    public enum PaymentMethod
    {
        [Description("COD")]
        COD = 1,
        [Description("Online")]
        Online = 2,
    }
    public enum SectionNumbers
    {
        [Description("1")]
        Section1 = 1,
        [Description("2")]
        Section2 = 2,
        [Description("PainLocations")]
        PainLocations = 3,
        [Description("Pain Medications")]
        PainMedications =8
    }
}