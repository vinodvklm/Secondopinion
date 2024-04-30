using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace CommonUtils
{
    public class Utils
    {

        public static string GetAppSettingValue(string key)
        {
            return (ConfigurationManager.AppSettings[key] != null ? ConfigurationManager.AppSettings[key] : "").ToString();
        }
        public static List<string> GetAppSettingArrayValue(string key, char delimiterChar = ';')
        {
            char[] arrayDelimiterChars = { delimiterChar };
            return ConfigurationManager.AppSettings[key].ToString().Split(arrayDelimiterChars).ToList();
        }
        public static int GetAppSettingIntegerValue(string key)
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString());
        }
        public static double GetAppSettingDoubleValue(string key)
        {
            return Convert.ToDouble(ConfigurationManager.AppSettings[key].ToString());
        }
        public static bool GetAppSettingBooleanValue(string key)
        {
            return Convert.ToBoolean(ConfigurationManager.AppSettings[key].ToString());
        }
        public static NameValueCollection GetNameValueCollection(string sectionName)
        {
            NameValueCollection coll = ConfigurationManager.GetSection(sectionName) as NameValueCollection;
            return coll;
        }
        public static List<Utility> UtilityCollection(string sectionName)
        {
            List<Utility> list = new List<Utility>();
            try
            {
                NameValueCollection coll = GetNameValueCollection(sectionName);
                var items = coll.AllKeys.SelectMany(coll.GetValues, (k, v) => new { Key = k, Value = v });
                foreach (var item in items)
                {
                    list.Add(new Utility { Key = item.Key, Value = item.Value });
                }
            }
            catch { }
            return list;
        }
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return getrandom.Next(min, max);
            }
        }
        public static double RoundIQD(double _inputnumber)
        {
            int _matchNumber = 125;
            double _last3digit, _inputnumberwithout3digit, _rounded_last3digit;
            if (_inputnumber == 0 || _inputnumber == null)
            {
                return 0;
            }
            else
            {
                _inputnumber = Convert.ToInt32(Math.Round(_inputnumber));
                if (_inputnumber < 1000)
                {
                    if (_inputnumber == 0) return 0;
                    if (_inputnumber > 0 && _inputnumber < _matchNumber * 1) return 0;
                    if (_inputnumber >= _matchNumber * 1 && _inputnumber < _matchNumber * 3) return _matchNumber * 2;
                    if (_inputnumber >= _matchNumber * 3 && _inputnumber < _matchNumber * 5) return _matchNumber * 4;
                    if (_inputnumber >= _matchNumber * 5 && _inputnumber < _matchNumber * 7) return _matchNumber * 6;
                    if (_inputnumber >= _matchNumber * 7 && _inputnumber <= _matchNumber * 8) return _matchNumber * 8;
                }
                else if (_inputnumber > 1000)
                {
                    //convert integer to number
                    // you can also do "_inputnumber"
                    var _nu = _inputnumber.ToString();
                    //get last three digits of the number
                    _last3digit = _inputnumber % 1000;  //_nu.substring(_nu.length, 4);
                    //number - last three digits
                    //for example: 123,650
                    //_last3digit = 650
                    //_inputnumberwithout3digit = 123,650 - 650 = 123000
                    _inputnumberwithout3digit = Convert.ToInt32(_inputnumber - _last3digit);
                    //check last three digits
                    _rounded_last3digit = RoundIQD(_last3digit);
                    _inputnumber = _inputnumberwithout3digit + _rounded_last3digit;
                    return _inputnumber;
                }
                else if (_inputnumber == 1000) return 1000;
                return _inputnumber;
            }

        }
        public static string NewGuid { get { return Guid.NewGuid().ToString().Replace("-", ""); } }
        public static void SetErrorLog(Exception ex)
        {
            return;
        }

        private static string _GoogleApiP12File { get; set; }
        public static string GoogleApiP12File { get { return _GoogleApiP12File; } set { _GoogleApiP12File = value; } }


        private static long _maxRequestLength = 0;
        public static long MaxRequestLength
        {
            get
            {
                if (_maxRequestLength == 0)
                {
                    try
                    {
                        System.Web.Configuration.HttpRuntimeSection section =
                        ConfigurationManager.GetSection("system.web/httpRuntime") as System.Web.Configuration.HttpRuntimeSection;
                        if (section != null)
                            _maxRequestLength = Convert.ToInt64(section.MaxRequestLength);
                    }
                    catch { _maxRequestLength = 4194304; }
                }
                return _maxRequestLength;
            }
        }
        public static long MaxRequestLengthInMb { get { return MaxRequestLength / 1024 / 1024; } }
        #region appSettings
        public static string DatabaseConnectionString { get { try { return GetAppSettingValue("DatabaseConnectionString"); } catch { return string.Empty; } } }
        public static string SiteName { get { try { return GetAppSettingValue("SiteName"); } catch { return string.Empty; } } }
        public static string CompanyName { get { try { return GetAppSettingValue("CompanyName"); } catch { return string.Empty; } } }
        public static string CompanyUrl { get { try { return GetAppSettingValue("CompanyUrl"); } catch { return string.Empty; } } }
        public static string CompanyFaviconUrl { get { try { return GetAppSettingValue("CompanyFaviconUrl"); } catch { return string.Empty; } } }
        public static string CompanyPhone { get { try { return GetAppSettingValue("CompanyPhone"); } catch { return string.Empty; } } }
        public static string CompanyEmail { get { try { return GetAppSettingValue("CompanyEmail"); } catch { return string.Empty; } } }
        
        public static string RootUrl { get { try { return GetAppSettingValue("RootUrl"); } catch { return string.Empty; } } }
        //public static string RootUrl { get { try { return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority); } catch { return string.Empty; } } }
        public static string NoImage { get { try { return RootUrl + GetAppSettingValue("NoImage"); } catch { return string.Empty; } } }
        public static string GenericDateTimeFormat { get { try { return GetAppSettingValue("GenericDateTimeFormat"); } catch { return "dd/MM/yyyy hh:mm:ss tt"; } } }
        public static string GenericDateFormat { get { try { return GetAppSettingValue("GenericDateFormat"); } catch { return "dd/MM/yyyy"; } } }
        public static string GenericTimeFormat { get { try { return GetAppSettingValue("GenericTimeFormat"); } catch { return "hh:mm:ss tt"; } } }
        public static string GenericDateTime24Format { get { try { return GetAppSettingValue("GenericDateTime24Format"); } catch { return "dd/MM/yyyy HH:mm:ss"; } } }
        public static string GenericTime24Format { get { try { return GetAppSettingValue("GenericTime24Format"); } catch { return "HH:mm:ss"; } } }
        public static string GenericTime12Format { get { try { return GetAppSettingValue("GenericTime12Format"); } catch { return "hh:mm tt"; } } }
        public static string Mode_LocationUpdate { get { try { return GetAppSettingValue("Mode_LocationUpdate"); } catch { return string.Empty; } } }
        public static int IntervalMinutes_LocationUpdate { get { try { return GetAppSettingIntegerValue("IntervalMinutes_LocationUpdate"); } catch { return 0; } } }
        public static string DefaultLangCultureName { get { try { return GetAppSettingValue("DefaultLangCultureName"); } catch { return "en"; } } }
        public static string BulkUploadMDPPath { get { try { return GetAppSettingValue("BulkUploadMDPPath"); } catch { return ""; } } }
        //public static string BulkUploadOrderFile { get { try { return GetAppSettingValue("BulkUploadOrderFile"); } catch { return ""; } } }
        //public static string OrderResponseToEmail { get { try { return GetAppSettingValue("OrderResponseToEmail"); } catch { return ""; } } }
        public static int DefaultMaxBulkUploadFileSize { get { try { return GetAppSettingIntegerValue("DefaultMaxBulkUploadFileSize"); } catch { return 1; } } }
        public static string NoPhoto { get { try { return RootUrl + GetAppSettingValue("NoPhoto"); } catch { return string.Empty; } } }
        #endregion

        #region MailServer
        public static List<Utility> MailServer
        {
            get
            {
                List<Utility> list = UtilityCollection("MailServer");
                return list;
            }
        }
        public static Utility MailServerItem(string Key)
        {
            Utility obj = MailServer.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string MailServerItemValue(string Key)
        {
            Utility obj = MailServerItem(Key);
            return obj.Value;
        }
        public static List<string> MailServerItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = MailServerItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int MailServerItemIntegerValue(string Key)
        {
            Utility obj = MailServerItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool MailServerItemBooleanValue(string Key)
        {
            Utility obj = MailServerItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        public static DataTable InitMessageLibTable(string libpath)
        {
            DataSet _dtMessageLib = new DataSet("Message");
            _dtMessageLib.ReadXml(libpath);
            return _dtMessageLib.Tables[0];
        }
        public static DataRow GetMessageLibTable(DataTable messages, string Head)
        {
            DataRow dr = messages.Select("Head = '" + Head + "'").FirstOrDefault();
            return dr;
        }
        #endregion

        #region FileServer
        public static List<Utility> FileServer
        {
            get
            {
                List<Utility> list = UtilityCollection("FileServer");
                return list;
            }
        }
        public static Utility FileServerItem(string Key)
        {
            Utility obj = FileServer.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static Utility FileServerItem(UploadFileType Key)
        {
            return FileServerItem(Key.ToString());
        }
        public static string FileServerItemValue(string Key)
        {
            Utility obj = FileServerItem(Key);
            return obj.Value;
        }
        public static string FileServerItemValue(UploadFileType Key)
        {
            return FileServerItemValue(Key.ToString());
        }
        public static List<string> FileServerItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = FileServerItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int FileServerItemIntegerValue(string Key)
        {
            Utility obj = FileServerItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool FileServerItemBooleanValue(string Key)
        {
            Utility obj = FileServerItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        #endregion

        #region SmsServer
        public static List<Utility> SmsServer
        {
            get
            {
                List<Utility> list = UtilityCollection("SmsServer");
                return list;
            }
        }
        public static Utility SmsServerItem(string Key)
        {
            Utility obj = SmsServer.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string SmsServerItemValue(string Key)
        {
            Utility obj = SmsServerItem(Key);
            return obj.Value;
        }
        public static List<string> SmsServerItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = SmsServerItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int SmsServerItemIntegerValue(string Key)
        {
            Utility obj = SmsServerItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool SmsServerItemBooleanValue(string Key)
        {
            Utility obj = SmsServerItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        #endregion

        #region appSettings
        public static int NetPayDays { get { try { return AppSettingsItemIntegerValue("NetPayDays"); } catch { return 25; } } }
        public static int TotalDays { get { try { return AppSettingsItemIntegerValue("TotalDays"); } catch { return 30; } } }
        public static int PageSize { get { try { return AppSettingsItemIntegerValue("PageSize"); } catch { return 10; } } }
        public static int VisaExpiryDays { get { try { return AppSettingsItemIntegerValue("VisaExpiryDays"); } catch { return 15; } } }
        public static int PassportExpiryDays { get { try { return AppSettingsItemIntegerValue("PassportExpiryDays"); } catch { return 15; } } }
        public static int SLWithoutDocCount { get { try { return AppSettingsItemIntegerValue("SLWithoutDocCount"); } catch { return 5; } } }
        public static List<Utility> AppSettings
        {
            get
            {
                List<Utility> list = UtilityCollection("appSettings");
                return list;
            }
        }
        public static Utility AppSettingsItem(string Key)
        {
            Utility obj = AppSettings.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string AppSettingsItemValue(string Key)
        {
            Utility obj = AppSettingsItem(Key);
            return obj.Value;
        }
        public static List<string> AppSettingsItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = AppSettingsItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int AppSettingsItemIntegerValue(string Key)
        {
            Utility obj = AppSettingsItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool AppSettingsItemBooleanValue(string Key)
        {
            Utility obj = AppSettingsItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        public static TimeSpan AppSettingsItemTimeSpanValue(string Key)
        {
            Utility obj = AppSettingsItem(Key);
            return TimeSpan.Parse(obj.Value);
        }
        public static DateTime AppSettingsItemDateTimeValue(string Key)
        {
            Utility obj = AppSettingsItem(Key);
            return DateTime.Parse(obj.Value);
        }
        #endregion

        #region serviceSettings
        public static List<Utility> ServiceSettings
        {
            get
            {
                List<Utility> list = UtilityCollection("serviceSettings");
                return list;
            }
        }
        public static Utility ServiceSettingsItem(string Key)
        {
            Utility obj = ServiceSettings.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string ServiceSettingsItemValue(string Key)
        {
            Utility obj = ServiceSettingsItem(Key);
            return obj.Value;
        }
        public static List<string> ServiceSettingsItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = ServiceSettingsItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int ServiceSettingsItemIntegerValue(string Key)
        {
            Utility obj = ServiceSettingsItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool ServiceSettingsItemBooleanValue(string Key)
        {
            Utility obj = ServiceSettingsItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        public static TimeSpan ServiceSettingsItemTimeSpanValue(string Key)
        {
            Utility obj = ServiceSettingsItem(Key);
            return TimeSpan.Parse(obj.Value);
        }
        public static DateTime ServiceSettingsItemDateTimeValue(string Key)
        {
            Utility obj = ServiceSettingsItem(Key);
            return DateTime.Parse(obj.Value);
        }
        #endregion

        #region firebaseSettings
        public class Firebase
        {
            public static string SERVER_URL { get { try { return FirebaseSettingsItemValue("SERVER_URL"); } catch { return string.Empty; } } }
            public static string FIREBASE_SERVER_KEY { get { try { return FirebaseSettingsItemValue("FIREBASE_SERVER_KEY"); } catch { return string.Empty; } } }
            public static string SERVER_API_KEY { get { try { return FirebaseSettingsItemValue("SERVER_API_KEY"); } catch { return string.Empty; } } }
            public static string SENDER_ID { get { try { return FirebaseSettingsItemValue("SENDER_ID"); } catch { return string.Empty; } } }
        }
        public static List<Utility> FirebaseSettings
        {
            get
            {
                List<Utility> list = UtilityCollection("firebaseSettings");
                return list;
            }
        }
        public static Utility FirebaseSettingsItem(string Key)
        {
            Utility obj = FirebaseSettings.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string FirebaseSettingsItemValue(string Key)
        {
            Utility obj = FirebaseSettingsItem(Key);
            return obj.Value;
        }
        public static List<string> FirebaseSettingsItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = FirebaseSettingsItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int FirebaseSettingsItemIntegerValue(string Key)
        {
            Utility obj = FirebaseSettingsItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool FirebaseSettingsItemBooleanValue(string Key)
        {
            Utility obj = FirebaseSettingsItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        #endregion

        #region GoogleAnalyticsLinkSettings
        public class GoogleAnalyticsLink
        {
            private static string _ServiceProviderAndroid { get; set; }
            public static string ServiceProviderAndroid
            {
                get
                {
                    try { _ServiceProviderAndroid = GoogleAnalyticsLinkSettingsItemValue("ServiceProviderAndroid"); }
                    catch { }
                    if (string.IsNullOrEmpty(_ServiceProviderAndroid))
                    {
                        _ServiceProviderAndroid = "/Home/NotFound";
                    }
                    return _ServiceProviderAndroid;
                }
            }
            private static string _ServiceProvideriOS { get; set; }
            public static string ServiceProvideriOS
            {
                get
                {
                    try { _ServiceProvideriOS = GoogleAnalyticsLinkSettingsItemValue("ServiceProvideriOS"); }
                    catch { }
                    if (string.IsNullOrEmpty(_ServiceProvideriOS))
                    {
                        _ServiceProvideriOS = "/Home/NotFound";
                    }
                    return _ServiceProvideriOS;
                }
            }
            private static string _ServiceRequestorAndroid { get; set; }
            public static string ServiceRequestorAndroid
            {
                get
                {
                    try { _ServiceRequestorAndroid = GoogleAnalyticsLinkSettingsItemValue("ServiceRequestorAndroid"); }
                    catch { }
                    if (string.IsNullOrEmpty(_ServiceRequestorAndroid))
                    {
                        _ServiceRequestorAndroid = "/Home/NotFound";
                    }
                    return _ServiceRequestorAndroid;
                }
            }
            private static string _ServiceRequestoriOS { get; set; }
            public static string ServiceRequestoriOS
            {
                get
                {
                    try { _ServiceRequestoriOS = GoogleAnalyticsLinkSettingsItemValue("ServiceRequestoriOS"); }
                    catch { }
                    if (string.IsNullOrEmpty(_ServiceRequestoriOS))
                    {
                        _ServiceRequestoriOS = "/Home/NotFound";
                    }
                    return _ServiceRequestoriOS;
                }
            }
        }
        public static List<Utility> GoogleAnalyticsLinkSettings
        {
            get
            {
                List<Utility> list = UtilityCollection("GoogleAnalyticsLinkSettings");
                return list;
            }
        }
        public static Utility GoogleAnalyticsLinkSettingsItem(string Key)
        {
            Utility obj = GoogleAnalyticsLinkSettings.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string GoogleAnalyticsLinkSettingsItemValue(string Key)
        {
            Utility obj = GoogleAnalyticsLinkSettingsItem(Key);
            return obj.Value;
        }
        public static List<string> GoogleAnalyticsLinkSettingsItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = GoogleAnalyticsLinkSettingsItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int GoogleAnalyticsLinkSettingsItemIntegerValue(string Key)
        {
            Utility obj = GoogleAnalyticsLinkSettingsItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool GoogleAnalyticsLinkSettingsItemBooleanValue(string Key)
        {
            Utility obj = GoogleAnalyticsLinkSettingsItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        #endregion

        #region Currency
        public static List<Utility> Currency
        {
            get
            {
                List<Utility> list = UtilityCollection("Currency");
                return list;
            }
        }
        public static Utility CurrencyItem(string Key)
        {
            Utility obj = Currency.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static string CurrencyItemValue(string Key)
        {
            Utility obj = CurrencyItem(Key);
            return obj.Value;
        }
        public static List<string> CurrencyItemArrayValue(string Key, char delimiterChar = ';')
        {
            Utility obj = CurrencyItem(Key);
            char[] arrayDelimiterChars = { delimiterChar };
            return obj.Value.Split(arrayDelimiterChars).ToList();
        }
        public static int CurrencyItemIntegerValue(string Key)
        {
            Utility obj = CurrencyItem(Key);
            return Convert.ToInt32(obj.Value);
        }
        public static bool CurrencyItemBooleanValue(string Key)
        {
            Utility obj = SmsServerItem(Key);
            return Convert.ToBoolean(obj.Value);
        }
        #endregion

        public static bool IsEnumerableType(Type type)
        {
            return (type.GetInterface("IEnumerable") != null);
        }
        public static bool IsCollectionType(Type type)
        {
            return (type.GetInterface("ICollection") != null);
        }
        public static int GetCount(object type)
        {
            int retval = 0;
            try
            {
                if (type != null)
                {
                    if (CommonUtils.Utils.IsCollectionType(type.GetType()))
                    {
                        ICollection collection = type as ICollection;
                        retval = collection.Count;
                    }
                }
            }
            catch { }
            return retval;
        }
        public static string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        public static string MD5Hash(string text)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = MD5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region DateTime

        private static TimeSpan DTTimeSpan;
        private static string AddOrMinusTime = "+";
        private static string _DateTimeOffset = string.Empty;
        public static string DateTimeOffset
        {
            set
            {
                _DateTimeOffset = value;
                if (_DateTimeOffset.Contains('+')) AddOrMinusTime = "+";
                if (_DateTimeOffset.Contains('-')) AddOrMinusTime = "-";
                try
                {
                    var str = _DateTimeOffset.Replace(AddOrMinusTime, "");
                    var strs = str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (AddOrMinusTime == "+")
                        DTTimeSpan = new TimeSpan(int.Parse(strs[0]), int.Parse(strs[1]), 0);
                    else
                        DTTimeSpan = new TimeSpan(0 - (int.Parse(strs[0]) + 1), int.Parse(strs[1]), 0);
                }
                catch
                {
                    AddOrMinusTime = "+";
                    DTTimeSpan = new TimeSpan(0, 0, 0);
                }
            }
        }
        public static DateTime CurrentDateTimeEN
        {
            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
                var CurrentDateTime = DateTime.UtcNow.Add(DTTimeSpan);
                return CurrentDateTime;
            }
        }
        public static DateTime CurrentDateTime
        {
            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                return DateTime.UtcNow.Add(DTTimeSpan);
            }
        }
        public static DateTime CurrentUtcDateTime
        {
            get
            {
                return DateTime.UtcNow.Add(new TimeSpan(0, 0, 0));
            }
        }
        public static TimeSpan CurrentTimeSpan
        {
            get
            {
                return DTTimeSpan;
            }
        }
        public static DateTime DefaultStartDateTime { get { return new DateTime(2000, 1, 1); } }
        public static int GetWeekNumberOfTheYear(DateTime date)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            // option 1 
            var weekNo = currentCulture.Calendar.GetWeekOfYear(date, currentCulture.DateTimeFormat.CalendarWeekRule, currentCulture.DateTimeFormat.FirstDayOfWeek);
            // option 2 
            //var weekNo = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNo;
        }
        public static string DateTimeDiffText(DateTime date, string text = "Older date", string newertext = "Newer date")
        {
            var week1 = GetWeekNumberOfTheYear(date);
            var week2 = GetWeekNumberOfTheYear(CurrentDateTime);
            var weekdiff = week1 - week2;

            var day1 = date.Day;
            var day2 = CurrentDateTime.Day;
            var daydiff = day1 - day2;

            var month1 = date.Month;
            var month2 = CurrentDateTime.Month;
            var monthdiff = month1 - month2;

            var year1 = date.Year;
            var year2 = CurrentDateTime.Year;
            var yeardiff = year1 - year2;


            switch (yeardiff)
            {
                case 0:
                    #region weekdiff
                    switch (weekdiff)
                    {
                        case 0:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    #region daydiff
                                    switch (daydiff)
                                    {
                                        case 0:
                                            text = "Today";
                                            break;
                                        case -1:
                                            text = "Yesterday";
                                            break;
                                        case 1:
                                            text = "Tommorrow";
                                            break;
                                        default:
                                            text = "This week";
                                            break;
                                    }
                                    #endregion
                                    break;
                                case -1:
                                    text = "Last month";
                                    break;
                                case 1:
                                    text = "Next month";
                                    break;
                                default:
                                    text = newertext;
                                    break;
                            }
                            #endregion
                            break;
                        case 1:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = "Next week";
                                    break;
                                case 1:
                                    text = "Next month";
                                    break;
                                default:
                                    text = newertext;
                                    break;
                            }
                            #endregion
                            break;
                        case -1:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = "Last week";
                                    break;
                                case -1:
                                    text = "Last month";
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        default:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = "This month";
                                    break;
                                case 1:
                                    text = "Next month";
                                    break;
                                case -1:
                                    text = "Last month";
                                    break;
                                default:
                                    if (monthdiff > 1)
                                    {
                                        text = newertext;
                                    }
                                    break;
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case -1:
                    text = "Last year";
                    break;
                case 1:
                    text = "Next year";
                    break;
                default:
                    if (yeardiff > 1)
                    {
                        text = newertext;
                    }
                    break;
            }
            return text;
        }
        public static DateTimeValue DateTimeDiffValue(DateTime date, DateTimeValue text = DateTimeValue.OlderDate, DateTimeValue newertext = DateTimeValue.NewerDate)
        {
            var week1 = GetWeekNumberOfTheYear(date);
            var week2 = GetWeekNumberOfTheYear(CurrentDateTime);
            var weekdiff = week1 - week2;

            var day1 = date.Day;
            var day2 = CurrentDateTime.Day;
            var daydiff = day1 - day2;

            var month1 = date.Month;
            var month2 = CurrentDateTime.Month;
            var monthdiff = month1 - month2;

            var year1 = date.Year;
            var year2 = CurrentDateTime.Year;
            var yeardiff = year1 - year2;


            switch (yeardiff)
            {
                case 0:
                    #region weekdiff
                    switch (weekdiff)
                    {
                        case 0:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    #region daydiff
                                    switch (daydiff)
                                    {
                                        case 0:
                                            text = DateTimeValue.Today;
                                            break;
                                        case -1:
                                            text = DateTimeValue.Yesterday;
                                            break;
                                        case 1:
                                            text = DateTimeValue.Tomorrow;
                                            break;
                                        default:
                                            text = DateTimeValue.ThisWeek;
                                            break;
                                    }
                                    #endregion
                                    break;
                                case -1:
                                    text = DateTimeValue.LastMonth;
                                    break;
                                case 1:
                                    text = DateTimeValue.NextMonth;
                                    break;
                                default:
                                    text = newertext;
                                    break;
                            }
                            #endregion
                            break;
                        case 1:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = DateTimeValue.NextWeek;
                                    break;
                                case 1:
                                    text = DateTimeValue.NextMonth;
                                    break;
                                default:
                                    text = newertext;
                                    break;
                            }
                            #endregion
                            break;
                        case -1:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = DateTimeValue.LastWeek;
                                    break;
                                case -1:
                                    text = DateTimeValue.LastMonth;
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            break;
                        default:
                            #region monthdiff
                            switch (monthdiff)
                            {
                                case 0:
                                    text = DateTimeValue.ThisMonth;
                                    break;
                                case 1:
                                    text = DateTimeValue.NextMonth;
                                    break;
                                case -1:
                                    text = DateTimeValue.LastMonth;
                                    break;
                                default:
                                    if (monthdiff > 1)
                                    {
                                        text = newertext;
                                    }
                                    break;
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case -1:
                    text = DateTimeValue.LastYear;
                    break;
                case 1:
                    text = DateTimeValue.NextYear;
                    break;
                default:
                    if (yeardiff > 1)
                    {
                        text = newertext;
                    }
                    break;
            }
            return text;
        }
        public static string DateTimeDiffDescription(DateTime date, DateTimeValue text = DateTimeValue.OlderDate, DateTimeValue newertext = DateTimeValue.NewerDate)
        {
            text = DateTimeDiffValue(date, text, newertext);
            var retval = text.GetDescription();
            return retval;
        }
        public static void GetDatesBetween(DateTimeValue datetimevalue, ref DateTime begindate, ref DateTime enddate)
        {
            DateTime baseDate = Utils.CurrentDateTime;
            DateTime yesterday = baseDate.AddDays(-1);
            DateTime tomorrow = baseDate.AddDays(1);

            DateTime _thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            DateTime thisWeekStart = new DateTime(_thisWeekStart.Year, _thisWeekStart.Month, _thisWeekStart.Day, 0, 0, 0, 0);
            DateTime _thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            DateTime thisWeekEnd = new DateTime(_thisWeekEnd.Year, _thisWeekEnd.Month, _thisWeekEnd.Day, 23, 59, 59, 999);

            DateTime _lastWeekStart = thisWeekStart.AddDays(-7);
            DateTime lastWeekStart = new DateTime(_lastWeekStart.Year, _lastWeekStart.Month, _lastWeekStart.Day, 0, 0, 0, 0);
            DateTime _lastWeekEnd = thisWeekStart.AddSeconds(-1);
            DateTime lastWeekEnd = new DateTime(_lastWeekEnd.Year, _lastWeekEnd.Month, _lastWeekEnd.Day, 23, 59, 59, 999);

            DateTime _nextWeekStart = thisWeekStart.AddDays(7);
            DateTime nextWeekStart = new DateTime(_nextWeekStart.Year, _nextWeekStart.Month, _nextWeekStart.Day, 0, 0, 0, 0);
            DateTime _nextWeekEnd = thisWeekStart.AddDays(13);
            DateTime nextWeekEnd = new DateTime(_nextWeekEnd.Year, _nextWeekEnd.Month, _nextWeekEnd.Day, 23, 59, 59, 999);

            DateTime _thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            DateTime thisMonthStart = new DateTime(_thisMonthStart.Year, _thisMonthStart.Month, _thisMonthStart.Day, 0, 0, 0, 0);
            DateTime _thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            DateTime thisMonthEnd = new DateTime(_thisMonthEnd.Year, _thisMonthEnd.Month, _thisMonthEnd.Day, 23, 59, 59, 999);

            DateTime _lastMonthStart = thisMonthStart.AddMonths(-1);
            DateTime lastMonthStart = new DateTime(_lastMonthStart.Year, _lastMonthStart.Month, _lastMonthStart.Day, 0, 0, 0, 0);
            DateTime _lastMonthEnd = thisMonthStart.AddSeconds(-1);
            DateTime lastMonthEnd = new DateTime(_lastMonthEnd.Year, _lastMonthEnd.Month, _lastMonthEnd.Day, 23, 59, 59, 999);

            DateTime _nextMonthStart = thisMonthStart.AddMonths(1);
            DateTime nextMonthStart = new DateTime(_nextMonthStart.Year, _nextMonthStart.Month, _nextMonthStart.Day, 0, 0, 0, 0);
            DateTime _nextMonthEnd = thisMonthStart.AddMonths(2).AddDays(-1);
            DateTime nextMonthEnd = new DateTime(_nextMonthEnd.Year, _nextMonthEnd.Month, _nextMonthEnd.Day, 23, 59, 59, 999);

            DateTime thisYearStart = new DateTime(baseDate.Year, 1, 1, 0, 0, 0, 0);
            DateTime _thisYearEnd = thisYearStart.AddYears(1).AddSeconds(-1);
            DateTime thisYearEnd = new DateTime(_thisYearEnd.Year, _thisYearEnd.Month, _thisYearEnd.Day, 23, 59, 59, 999);

            DateTime _lastYearStart = thisYearStart.AddYears(-1);
            DateTime lastYearStart = new DateTime(_lastYearStart.Year, 1, 1, 0, 0, 0, 0);
            DateTime _lastYearEnd = thisYearStart.AddSeconds(-1);
            DateTime lastYearEnd = new DateTime(_lastYearEnd.Year, _lastYearEnd.Month, _lastYearEnd.Day, 23, 59, 59, 999);

            DateTime _nextYearStart = thisYearStart.AddYears(1);
            DateTime nextYearStart = new DateTime(_nextYearStart.Year, 1, 1, 0, 0, 0, 0);
            DateTime _nextYearEnd = thisYearStart.AddYears(2).AddSeconds(-1);
            DateTime nextYearEnd = new DateTime(_nextYearEnd.Year, _nextYearEnd.Month, _nextYearEnd.Day, 23, 59, 59, 999);

            DateTime olderDateStart = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            DateTime _olderDateEnd = lastYearStart.AddSeconds(-1);
            DateTime olderDateEnd = new DateTime(_olderDateEnd.Year, _olderDateEnd.Month, _olderDateEnd.Day, 23, 59, 59, 999);

            DateTime _newerDateStart = nextYearEnd.AddSeconds(1);
            DateTime newerDateStart = new DateTime(_newerDateStart.Year, 1, 1, 0, 0, 0, 0);
            DateTime newerDateEnd = new DateTime(9999, 12, 31, 23, 59, 59, 999);

            switch (datetimevalue)
            {
                case DateTimeValue.OlderDate:
                    begindate = olderDateStart;
                    enddate = olderDateEnd;
                    break;
                case DateTimeValue.NewerDate:
                    begindate = newerDateStart;
                    enddate = newerDateEnd;
                    break;
                case DateTimeValue.Today:
                    begindate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0, 0);
                    enddate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 23, 59, 59, 999);
                    break;
                case DateTimeValue.Yesterday:
                    begindate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0, 0);
                    enddate = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59, 999);
                    break;
                case DateTimeValue.Tomorrow:
                    begindate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0, 0);
                    enddate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 23, 59, 59, 999);
                    break;
                case DateTimeValue.ThisWeek:
                    begindate = thisWeekStart;
                    enddate = thisWeekEnd;
                    break;
                case DateTimeValue.LastWeek:
                    begindate = lastWeekStart;
                    enddate = lastWeekEnd;
                    break;
                case DateTimeValue.NextWeek:
                    begindate = nextWeekStart;
                    enddate = nextWeekEnd;
                    break;
                case DateTimeValue.ThisMonth:
                    begindate = thisMonthStart;
                    enddate = thisMonthEnd;
                    break;
                case DateTimeValue.LastMonth:
                    begindate = lastMonthStart;
                    enddate = lastMonthEnd;
                    break;
                case DateTimeValue.NextMonth:
                    begindate = nextMonthStart;
                    enddate = nextMonthEnd;
                    break;
                case DateTimeValue.ThisYear:
                    begindate = thisYearStart;
                    enddate = thisYearEnd;
                    break;
                case DateTimeValue.LastYear:
                    begindate = lastYearStart;
                    enddate = lastYearEnd;
                    break;
                case DateTimeValue.NextYear:
                    begindate = nextYearStart;
                    enddate = nextYearEnd;
                    break;
            }
        }

        public static string GetTime12Hour(TimeSpan time)
        {
            var timestring = string.Empty;
            try
            {
                DateTime date = DateTime.Today.Add(time);
                timestring = date.ToString(GenericTime12Format);
            }
            catch { }
            return timestring;
        }
        public static string GetTime24Hour(TimeSpan time)
        {
            var timestring = string.Empty;
            try
            {
                DateTime date = DateTime.Today.Add(time);
                timestring = date.ToString(GenericTime24Format);
            }
            catch { }
            return timestring;
        }
        public static string GetTime12Hour(TimeSpan? time)
        {
            var timestring = string.Empty;
            try
            {
                DateTime date = DateTime.Today.Add(time.Value);
                timestring = date.ToString(GenericTime12Format);
            }
            catch { }
            return timestring;
        }
        public static string GetTime24Hour(TimeSpan? time)
        {
            var timestring = string.Empty;
            try
            {
                DateTime date = DateTime.Today.Add(time.Value);
                timestring = date.ToString(GenericTime24Format);
            }
            catch { }
            return timestring;
        }

        public static string DateToString(DateTime date, string format = "dd/MM/yyyy")
        {
            string dateString = string.Empty;
            try
            {
                dateString = date.ToString(format);
            }
            catch
            {
                try
                {
                    dateString = date.ToString("dd/MM/yyyy");
                }
                catch
                {
                    dateString = CurrentUtcDateTime.ToString("dd/MM/yyyy");
                }
            }
            return dateString;
        }

        public static int GetMonth(string month) { try { return DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month; } catch { return 0; } }

        public static int GetYear(string year) { try { return int.Parse(year); } catch { return 0; } }

        #endregion

        #region XML
        public class XmlParser
        {
            public static T XmlToObject<T>(string xml, string root = "root")
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + xml;
                var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));
                using (var textReader = new StringReader(xml))
                {
                    using (var xmlReader = System.Xml.XmlReader.Create(textReader))
                    {
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
            public static XmlNode CreateXmlNode(string tagname, string value, XmlDocument doc)
            {
                XmlNode node = doc.CreateElement(tagname);
                node.InnerText = value;
                return node;
            }
            public static string XmlToString(XmlDocument xmlDoc)
            {
                try
                {
                    using (var stringWriter = new StringWriter())
                    {
                        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                        {
                            xmlDoc.WriteTo(xmlTextWriter);
                            xmlTextWriter.Flush();
                            return stringWriter.GetStringBuilder().ToString();
                        }
                    }
                }
                catch { }
                return "";
            }
            public static string RemoveTag(string xml, string tagtoberemoved)
            {
                var startindex = 0;
                var endindex = 0;
                var sub = "";
                xml = xml.Replace(System.Environment.NewLine, "");
                xml = xml.Replace("\r", "");
                xml = xml.Replace("\n", "");
                xml = xml.Replace("\t", "");
                //while (true)
                {
                    startindex = xml.IndexOf("<" + tagtoberemoved);
                    endindex = xml.IndexOf("</" + tagtoberemoved + ">");
                    //if (startindex < 0) break;
                    sub = xml.Substring(startindex, endindex + ("</" + tagtoberemoved + ">").Length);
                }
                return "";
            }
            public static string RemoveTag(XDocument root, string tagtoberemoved)
            {
                root.Element(tagtoberemoved).Remove();
                root.Descendants(tagtoberemoved).Remove();
                string result = root.ToString();
                return result;
            }
            public static string DeleteNode(string xml, string tagtoberemoved)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                return DeleteNode(xmlDoc, tagtoberemoved);
            }
            public static string DeleteNode(XmlDocument xmlDoc, string tagtoberemoved)
            {
                XmlElement _xmlElement = xmlDoc.DocumentElement;

                XmlNode _xmlNode = _xmlElement.SelectSingleNode("tagtoberemoved");
                _xmlElement.RemoveChild(_xmlNode);
                return _xmlElement.ToString();
            }
        }
        #endregion

    }
    public class Converter
    {
        public static Byte ToByte(object obj, Byte value = 0) { try { return Convert.ToByte(obj); } catch { return value; } }
        public static Int16 ToInt16(object obj, Int16 value = 0) { try { return Convert.ToInt16(obj); } catch { return value; } }
        public static Int32 ToInt32(object obj, Int32 value = 0) { try { return Convert.ToInt32(obj); } catch { return value; } }
        public static Int64 ToInt64(object obj, Int64 value = 0) { try { return Convert.ToInt64(obj); } catch { return value; } }
        public static Decimal ToDecimal(object obj, Decimal value = 0) { try { return Convert.ToDecimal(obj); } catch { return value; } }
        public static Single ToSingle(object obj, Single value = 0) { try { return Convert.ToSingle(obj); } catch { return value; } }
        public static DateTime ToDateTime(object obj, DateTime value) { try { return Convert.ToDateTime(obj); } catch { return value; } }
        public static DateTime ToDateTime(object obj) { try { return Convert.ToDateTime(obj); } catch { return new DateTime(1900, 1, 1); } }
    }
    public static class Extensions
    {
        public static Expression<Func<TElement, bool>> BuildOrExpression<TElement, TValue>
        (
           Expression<Func<TElement, TValue>> valueSelector,
           IEnumerable<TValue> values
        )
        {
            if (null == valueSelector)
                throw new ArgumentNullException("valueSelector");

            if (null == values)
                throw new ArgumentNullException("values");

            ParameterExpression p = valueSelector.Parameters.Single();


            if (!values.Any())
                return e => false;


            var equals = values.Select(value =>
                (Expression)Expression.Equal(
                     valueSelector.Body,
                     Expression.Constant(
                         value,
                         typeof(TValue)
                     )
                )
            );
            var body = equals.Aggregate<Expression>(
                     (accumulate, equal) => Expression.Or(accumulate, equal)
             );

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static List<T> GetListFromString<T>(this string value, char delimiter = ',')
        {
            var list = value.Split(',').Select(s => s.Trim()).Select(s => s.Replace(" ", "")).ToList();
            List<T> returnValue = new List<T>();
            Type t = typeof(T);
            foreach (var item in list)
            {
                returnValue.Add((T)Convert.ChangeType(item, typeof(T)));
            }
            return returnValue;
        }
        public static T ToObject<T>(this string jsonString)
        {
            return (T)JsonConvert.DeserializeObject(jsonString, typeof(T));
        }
        public static string CollectionToJson<T>(this List<T> list)
        {
            return JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
        public static T ToObjectDynamic<T>(this string jsonString)
        {
            Type type = typeof(T), argType;
            object myObjectList = Activator.CreateInstance(type);
            bool IsGeneric = false;
            try
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    argType = type.GetGenericArguments()[0];
                    IsGeneric = true;
                }
                else
                {
                    argType = type;
                }
                var start1 = jsonString.IndexOf("},\"length\":");
                start1 = start1 < 0 ? 0 : start1;
                var jsonString1 = jsonString;
                var jsonString3 = jsonString.Substring(start1);
                var length = Convert.ToInt32(jsonString3.Replace("},\"length\":", "").Replace("}", ""));
                for (int i = 0; i < length; i++)
                {
                    int start2 = 0, start3 = 0;
                    string replacestring = "";
                    if (i == 0)
                    {
                        replacestring = "{\"" + i + "\":";
                        start2 = jsonString.IndexOf(replacestring);
                        start3 = jsonString.IndexOf(",\"" + (i + 1) + "\":");
                    }
                    else if (length - i > 1)
                    {
                        replacestring = ",\"" + i + "\":";
                        start2 = jsonString.IndexOf(replacestring);
                        start3 = jsonString.IndexOf(",\"" + (i + 1) + "\":") + 1;
                    }
                    else
                    {
                        replacestring = ",\"" + i + "\":";
                        start2 = jsonString.IndexOf(replacestring);
                        start3 = start1 + 1;
                    }
                    var jsonString4 = "";
                    if (length == 1)
                    {
                        jsonString4 = jsonString.Substring(start2).Replace(replacestring, "").Replace(jsonString3, "}");
                    }
                    else
                    {
                        jsonString4 = jsonString.Substring(start2, start3 - start2).Replace(replacestring, "");
                    }
                    var myObject1 = JsonConvert.DeserializeObject(jsonString4, argType);
                    if (IsGeneric)
                    {
                        ((IList)myObjectList).Add(myObject1);
                    }
                    else
                    {
                        myObjectList = myObject1;
                    }
                }
            }
            catch { }
            return (T)myObjectList;
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static Utility GetValueByKey(this List<Utility> list, string Key)
        {
            Utility obj = list.FirstOrDefault(x => x.Key == Key) as Utility;
            return obj;
        }
        public static List<T> CopyList<T>(this List<T> lst)
        {
            List<T> listCopy = new List<T>();
            try
            {
                foreach (var item in lst)
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, item);
                            stream.Position = 0;
                            listCopy.Add((T)formatter.Deserialize(stream));
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listCopy;
        }
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static string ToComma<T, TU>(this IEnumerable<T> source, Func<T, TU> func, string separator = ",") { try { return string.Join(separator, source.Select(s => func(s).ToString()).ToArray()); } catch { return string.Empty; } }

        public static int WordWrapLength = 30;

        public static string WrapText(this string input)
        {
            return WrapText(input, WordWrapLength);
        }

        public static string WrapText(this string input, int length)
        {
            if (input == null) return "";
            if (input.Length <= length)
            {
                return input;
            }
            if (length > 0)
            {
                return input.Substring(0, length) + "...";
            }
            return input;
        }
        public static bool ContainsKey(this NameValueCollection collection, string key)
        {
            if (collection.Get(key) != null)
            {
                return collection.AllKeys.Contains(key);
            }

            return true;
        }
        public static object GetValue(this NameValueCollection collection, string key)
        {
            if (collection.Get(key) != null)
            {
                return collection[key];
            }
            return null;
        }

        #region DateTime
        public static bool IsSameDay(this DateTime datetime1, DateTime datetime2)
        {
            return datetime1.Year == datetime2.Year
                && datetime1.Month == datetime2.Month
                && datetime1.Day == datetime2.Day;
        }
        public static bool IsToday(this DateTime datetime1)
        {
            return IsSameDay(datetime1, CommonUtils.Utils.CurrentDateTime);
        }
        public static string GetDayNumber(this DateTime date)
        {
            string day = "Sunday";
            try
            {
                day = date.DayOfWeek.ToString();
            }
            catch
            {
                date = DateTime.Now;
                day = date.DayOfWeek.ToString();
            }
            if (day == "Sunday")
            {
                return "1";
            }
            else if (day == "Monday")
            {
                return "2";
            }
            else if (day == "Tuesday")
            {
                return "3";
            }
            else if (day == "Wednesday")
            {
                return "4";
            }
            else if (day == "Thursday")
            {
                return "5";
            }
            else if (day == "Friday")
            {
                return "6";
            }
            else
            {
                return "7";
            }
        }
        public static string GetGenericDateTimeFormat(this DateTime date) { try { return date.ToString(CommonUtils.Utils.GenericDateTimeFormat); } catch { return ""; } }
        public static string GetGenericDateFormat(this DateTime date) { try { return date.ToString(CommonUtils.Utils.GenericDateFormat); } catch { return ""; } }
        public static double ToTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        public static int GetDateDifferenceForDays(this string FirstDate, string SecondDate)
        {
            var NoOfDays = 0;
            try { NoOfDays = GetDateDifferenceForDays(Convert.ToDateTime(FirstDate), Convert.ToDateTime(SecondDate)); }
            catch { }
            return NoOfDays;
        }
        public static int GetDateDifferenceForDays(this DateTime FirstDate, DateTime SecondDate)
        {
            var NoOfDays = 0;
            try { NoOfDays = SecondDate.Subtract(FirstDate).Days + 1; }
            catch { }
            return NoOfDays;
        }

        public static Double LeaveCalculate(this DateTime date, Double TotalLeaves)
        {
            DateTime startDate = new DateTime(date.Year, 1, 1);
            var NoOfLeaveDays = startDate.GetDateDifferenceForDays(date) - 1;
            Double EmployeeLeave = (TotalLeaves / 365) * NoOfLeaveDays;
            Double BeforeLeave = Math.Round(EmployeeLeave, 2);
            //string[] str = BeforeLeave.ToString().Split('.');
            //if (Convert.ToInt32(str[1]) >= 50)
            //{
            //    BeforeLeave = Convert.ToDouble(str[0]) + 0.50;
            //}
            //else
            //{
            //    BeforeLeave = Convert.ToDouble(str[0]);
            //}
            return BeforeLeave;
        }
        public static Double LeaveCalculateByDate(this DateTime selectedDate, DateTime FromDate, Double TotalLeaves)
        {
            //DateTime startDate = CommonUtils.Utils.CurrentDateTime;
            // var NoOfLeaveDays = selectedDate.GetDateDifferenceForDays(startDate) - 1;
            
            var NoOfLeaveDays = FromDate.GetDateDifferenceForDays(selectedDate) - 1;
            Double EmployeeLeave = (TotalLeaves / 365) * NoOfLeaveDays;
            Double BeforeLeave = Math.Round(EmployeeLeave, 2);
            return BeforeLeave;
        }
        public static List<Utility> MonthsBetween(this DateTime startDate, DateTime endDate, double NoOfDays)
        {
            DateTime iterator;
            DateTime limit;
            List<Utility> list = new List<Utility>();
            double NoOfLeaveDays = 0;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }
            NoOfLeaveDays = startDate.GetDateDifferenceForDays(endDate) - NoOfDays;

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                var firstdate = new DateTime(iterator.Year, iterator.Month, 1);
                var lastday = 0;
                var currlastday = 0;
                double leaveday = 0;
                if (iterator.Month == startDate.Month)
                {
                    firstdate = startDate;
                }
                lastday = firstdate.LastDayOfMonth();
                currlastday = lastday;
                if (iterator.Month == endDate.Month)
                {
                    lastday = (iterator.Day == lastday) ? lastday : endDate.Day;
                }
                var lastdate = new DateTime(iterator.Year, iterator.Month, lastday);
                leaveday = firstdate.GetDateDifferenceForDays(lastdate);
                if (leaveday > NoOfLeaveDays)
                {
                    leaveday = leaveday - NoOfLeaveDays;
                    NoOfLeaveDays = 0;
                }
                else
                {
                    NoOfLeaveDays = NoOfLeaveDays - leaveday;
                    leaveday = 0;
                }
                list.Add(new Utility
                {
                    Key = dateTimeFormat.GetMonthName(iterator.Month),
                    Value = iterator.Year.ToString(),
                    AdditionalInfo = new Utility { Key = currlastday.ToString(), Value = (leaveday == 0) ? "0" : leaveday.ToString() },
                });
                iterator = iterator.AddMonths(1);
            }
            return list;
        }
        public static int LastDayOfMonth(this DateTime date)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
            return lastDayOfMonth;
        }
        public static DateTime MonthEnd(this DateTime date)
        {
            var lastDayOfMonth = new DateTime(date.Year, date.Month, date.LastDayOfMonth());
            return lastDayOfMonth;
        }
        public static ComparisonType DateCompare(this DateTime date1, DateTime date2)
        {
            var result = DateTime.Compare(date1.Date, date2.Date);
            return (date1.ToString("MMMM") == date2.Date.ToString("MMMM") && date1.Year == date2.Year) ? ComparisonType.EqualTo : (result < 0) ? ComparisonType.LessThan : (result == 0) ? ComparisonType.EqualTo : ComparisonType.GreaterThan;
        }

        public static bool IsParseExact(this string dateString, ref DateTime date)
        {
            bool retval = false;
            try
            {
                #region date formats
                string[] formats = new string[]
                {
                    #region "d/M/yyyy"
                    "d/M/yyyy",
                    "d/M/yyyy HH:mm:ss",
                    "d/M/yyyy hh:mm:ss tt",
                    "d/M/yyyy h:mm:ss tt",
                    "d/M/yyyy h:m:ss tt",
                    "d/M/yyyy hh:m:ss tt",
                    "d/M/yyyy hh:mm:s tt",
                    "d/M/yyyy h:m:ss tt",
                    "d/M/yyyy h:mm:s tt",
                    "d/M/yyyy hh:m:s tt",
                    "d/M/yyyy h:m:s tt",
                    "d/M/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "d/MM/yyyy"
                    "d/MM/yyyy",
                    "d/MM/yyyy HH:mm:ss",
                    "d/MM/yyyy hh:mm:ss tt",
                    "d/MM/yyyy h:mm:ss tt",
                    "d/MM/yyyy h:m:ss tt",
                    "d/MM/yyyy hh:m:ss tt",
                    "d/MM/yyyy hh:mm:s tt",
                    "d/MM/yyyy h:m:ss tt",
                    "d/MM/yyyy h:mm:s tt",
                    "d/MM/yyyy hh:m:s tt",
                    "d/MM/yyyy h:m:s tt",
                    "d/MM/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "d/MMM/yyyy"
                    "d/MMM/yyyy",
                    "d/MMM/yyyy HH:mm:ss",
                    "d/MMM/yyyy hh:mm:ss tt",
                    "d/MMM/yyyy h:mm:ss tt",
                    "d/MMM/yyyy h:m:ss tt",
                    "d/MMM/yyyy hh:m:ss tt",
                    "d/MMM/yyyy hh:mm:s tt",
                    "d/MMM/yyyy h:m:ss tt",
                    "d/MMM/yyyy h:mm:s tt",
                    "d/MMM/yyyy hh:m:s tt",
                    "d/MMM/yyyy h:m:s tt",
                    "d/MMM/yyyy HH:mm:ss",
                    #endregion
                     
                    #region "dd/M/yyyy"
                    "dd/M/yyyy",
                    "dd/M/yyyy HH:mm:ss",
                    "dd/M/yyyy hh:mm:ss tt",
                    "dd/M/yyyy h:mm:ss tt",
                    "dd/M/yyyy h:m:ss tt",
                    "dd/M/yyyy hh:m:ss tt",
                    "dd/M/yyyy hh:mm:s tt",
                    "dd/M/yyyy h:m:ss tt",
                    "dd/M/yyyy h:mm:s tt",
                    "dd/M/yyyy hh:m:s tt",
                    "dd/M/yyyy h:m:s tt",
                    "dd/M/yyyy HH:mm:ss",
                    #endregion
                     
                    #region "dd/MM/yyyy"
                    "dd/MM/yyyy",
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy hh:mm:ss tt",
                    "dd/MM/yyyy h:mm:ss tt",
                    "dd/MM/yyyy h:m:ss tt",
                    "dd/MM/yyyy hh:m:ss tt",
                    "dd/MM/yyyy hh:mm:s tt",
                    "dd/MM/yyyy h:m:ss tt",
                    "dd/MM/yyyy h:mm:s tt",
                    "dd/MM/yyyy hh:m:s tt",
                    "dd/MM/yyyy h:m:s tt",
                    "dd/MM/yyyy HH:mm:ss",
                    #endregion
                     
                    #region "dd/MMM/yyyy"
                    "dd/MMM/yyyy",
                    "dd/MMM/yyyy HH:mm:ss",
                    "dd/MMM/yyyy hh:mm:ss tt",
                    "dd/MMM/yyyy h:mm:ss tt",
                    "dd/MMM/yyyy h:m:ss tt",
                    "dd/MMM/yyyy hh:m:ss tt",
                    "dd/MMM/yyyy hh:mm:s tt",
                    "dd/MMM/yyyy h:m:ss tt",
                    "dd/MMM/yyyy h:mm:s tt",
                    "dd/MMM/yyyy hh:m:s tt",
                    "dd/MMM/yyyy h:m:s tt",
                    "dd/MMM/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "M/d/yyyy"
                    "M/d/yyyy",
                    "M/d/yyyy HH:mm:ss",
                    "M/d/yyyy hh:mm:ss tt",
                    "M/d/yyyy h:mm:ss tt",
                    "M/d/yyyy h:m:ss tt",
                    "M/d/yyyy hh:m:ss tt",
                    "M/d/yyyy hh:mm:s tt",
                    "M/d/yyyy h:m:ss tt",
                    "M/d/yyyy h:mm:s tt",
                    "M/d/yyyy hh:m:s tt",
                    "M/d/yyyy h:m:s tt",
                    "M/d/yyyy HH:mm:ss",
                    #endregion

                    #region "MMM/d/yyyy"
                    "MMM/d/yyyy",
                    "MMM/d/yyyy HH:mm:ss",
                    "MMM/d/yyyy hh:mm:ss tt",
                    "MMM/d/yyyy h:mm:ss tt",
                    "MMM/d/yyyy h:m:ss tt",
                    "MMM/d/yyyy hh:m:ss tt",
                    "MMM/d/yyyy hh:mm:s tt",
                    "MMM/d/yyyy h:m:ss tt",
                    "MMM/d/yyyy h:mm:s tt",
                    "MMM/d/yyyy hh:m:s tt",
                    "MMM/d/yyyy h:m:s tt",
                    "MMM/d/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "M/dd/yyyy"
                    "M/dd/yyyy",
                    "M/dd/yyyy HH:mm:ss",
                    "M/dd/yyyy hh:mm:ss tt",
                    "M/dd/yyyy h:mm:ss tt",
                    "M/dd/yyyy h:m:ss tt",
                    "M/dd/yyyy hh:m:ss tt",
                    "M/dd/yyyy hh:mm:s tt",
                    "M/dd/yyyy h:m:ss tt",
                    "M/dd/yyyy h:mm:s tt",
                    "M/dd/yyyy hh:m:s tt",
                    "M/dd/yyyy h:m:s tt",
                    "M/dd/yyyy HH:mm:ss",
                    #endregion                    
                    
                    #region "MMM/dd/yyyy"
                    "MMM/dd/yyyy",
                    "MMM/dd/yyyy HH:mm:ss",
                    "MMM/dd/yyyy hh:mm:ss tt",
                    "MMM/dd/yyyy h:mm:ss tt",
                    "MMM/dd/yyyy h:m:ss tt",
                    "MMM/dd/yyyy hh:m:ss tt",
                    "MMM/dd/yyyy hh:mm:s tt",
                    "MMM/dd/yyyy h:m:ss tt",
                    "MMM/dd/yyyy h:mm:s tt",
                    "MMM/dd/yyyy hh:m:s tt",
                    "MMM/dd/yyyy h:m:s tt",
                    "MMM/dd/yyyy HH:mm:ss",
                    #endregion                    
                    
                    #region "MM/d/yyyy"
                    "MM/d/yyyy",
                    "MM/d/yyyy HH:mm:ss",
                    "MM/d/yyyy hh:mm:ss tt",
                    "MM/d/yyyy h:mm:ss tt",
                    "MM/d/yyyy h:m:ss tt",
                    "MM/d/yyyy hh:m:ss tt",
                    "MM/d/yyyy hh:mm:s tt",
                    "MM/d/yyyy h:m:ss tt",
                    "MM/d/yyyy h:mm:s tt",
                    "MM/d/yyyy hh:m:s tt",
                    "MM/d/yyyy h:m:s tt",
                    "MM/d/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "MM/dd/yyyy"
                    "MM/dd/yyyy",
                    "MM/dd/yyyy HH:mm:ss",
                    "MM/dd/yyyy hh:mm:ss tt",
                    "MM/dd/yyyy h:mm:ss tt",
                    "MM/dd/yyyy h:m:ss tt",
                    "MM/dd/yyyy hh:m:ss tt",
                    "MM/dd/yyyy hh:mm:s tt",
                    "MM/dd/yyyy h:m:ss tt",
                    "MM/dd/yyyy h:mm:s tt",
                    "MM/dd/yyyy hh:m:s tt",
                    "MM/dd/yyyy h:m:s tt",
                    "MM/dd/yyyy HH:mm:ss",
                    #endregion
                    
                    #region "MMM/dd/yyyy"
                    "MMM/dd/yyyy",
                    "MMM/dd/yyyy HH:mm:ss",
                    "MMM/dd/yyyy hh:mm:ss tt",
                    "MMM/dd/yyyy h:mm:ss tt",
                    "MMM/dd/yyyy h:m:ss tt",
                    "MMM/dd/yyyy hh:m:ss tt",
                    "MMM/dd/yyyy hh:mm:s tt",
                    "MMM/dd/yyyy h:m:ss tt",
                    "MMM/dd/yyyy h:mm:s tt",
                    "MMM/dd/yyyy hh:m:s tt",
                    "MMM/dd/yyyy h:m:s tt",
                    "MMM/dd/yyyy HH:mm:ss",
                    #endregion
                };
                if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) { retval = true; }
                #endregion
            }
            catch
            {
            }
            return retval;
        }
        #endregion
    }
    public class Utility
    {
        private string _Key = "Message";
        public string Key { get { return _Key; } set { _Key = value; } }
        public string Value { get; set; }
        public object AdditionalInfo { get; set; }
        public string ToJsonFormat()
        {
            return "{" + this.Key + ":" + this.Value + "}";
        }
        public static string ToJsonFormat(Utility obj)
        {
            return "{" + obj.Key + ":" + obj.Value + "}";
        }
    }
    public class DefaultModel
    {
        public string Value { get; set; }
    }
    public class UtilityModel<T>
    {
        public object Key { get; set; }
        public T Value { get; set; }
    }
    public class ResultClass<T>
    {
        public ResultClass()
        {
        }
        public ResultClass(T result)
        {
            this.Result = result;
        }
        public CommonUtils.MessageType MessageType { get; set; }
        public string MessageTypeString { get { return this.MessageType.GetDescription(); } }
        public T Result { get; set; }

        private CommonUtils.Utility _Message = null;
        public CommonUtils.Utility Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        public int Count { get { return CommonUtils.Utils.GetCount(Result); } }
        public static ResultClass<T> Create(T result, MessageType messageType, CommonUtils.Utility message = null)
        {
            var obj = new ResultClass<T> { Result = result, MessageType = messageType, Message = message };
            return obj;
        }

    }
    public class CollectionBase
    {
        public object Value { get; set; }
        public long ValueInt { get; set; }
        public object Text { get; set; }
        public static List<CollectionBase> GetEnumList<T>()
        {
            List<CollectionBase> list = new List<CollectionBase>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                list.Add(new CollectionBase { Text = e.ToString(), Value = ((int)e).ToString(), ValueInt = ((int)e) });
            }
            return list;
        }
    }
    public class MessageBase
    {
        public object Message { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageAlertCss { get; set; }
        public string MessageIconCss { get; set; }
    }
}
