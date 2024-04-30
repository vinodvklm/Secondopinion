using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Configuration;

namespace CommonUtils
{
    public class BulkUpload
    {
        // folder for the upload, you can put this in the web.config
        public string UploadPath { get; set; }
        public string Prepend { get; set; }
        public string ConString { get; set; }

        public ResultClass<BulkUploadResult> RenameUploadFile(HttpPostedFileBase file, Int32 counter = 0)
        {
            var fileName = Path.GetFileName(file.FileName);
            Prepend += (counter > 0) ? ((counter).ToString()) + "_" : "";
            string finalFileName = MakeValidFileName((Prepend + fileName).Replace(" ", ""));
            if (System.IO.File.Exists(HttpContext.Current.Request.MapPath(UploadPath + finalFileName)))
            {
                //file exists => add count try again
                return RenameUploadFile(file, ++counter);
            }
            //file doesn't exist, upload item but validate first
            return UploadFile(file, finalFileName);
        }

        private ResultClass<BulkUploadResult> UploadFile(HttpPostedFileBase file, string fileName)
        {
            BulkUploadResult imageResult = new BulkUploadResult { MessageType = MessageType.Success, ErrorMessage = null };
            fileName = fileName.Replace(" ", "");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + UploadPath, fileName);
            //var path = Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.MessageType = MessageType.Failed;
                imageResult.Name = "";
                imageResult.ErrorMessage = "Invalid Extension";
                return new ResultClass<BulkUploadResult>
                {
                    Result = null,
                    MessageType = CommonUtils.MessageType.Error,
                    Message = new CommonUtils.Utility { Key = CommonUtils.MessageType.Error.ToString(), Value = "Invalid Extension" }
                };
            }

            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + UploadPath))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + UploadPath);
                }
                file.SaveAs(path);
                
                imageResult.Name = UploadPath + fileName;

                return new ResultClass<BulkUploadResult>
                {
                    Result = imageResult,
                    MessageType = CommonUtils.MessageType.Success,
                    Message = new CommonUtils.Utility { Key = CommonUtils.MessageType.Success.ToString(), Value = "File has been uploaded" }
                }; ;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generaly logging or testing

                imageResult.MessageType = MessageType.Exception;
                imageResult.ErrorMessage = ex.Message;
                return new ResultClass<BulkUploadResult>
                {
                    Result = null,
                    MessageType = CommonUtils.MessageType.Error,
                    Message = new CommonUtils.Utility { Key = CommonUtils.MessageType.Error.ToString(), Value = ex.Message.ToString() }
                }; ;
            }
        }

        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".xls": //Excel 97-03
                    ConString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    return true;
                case ".xlsx": //Excel 07 or higher
                    ConString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                    return true;
                default:
                    return false;
            }
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
    public class BulkUploadResult
    {
        public MessageType MessageType { get; set; }
        public string MessageTypeText { get { try { return MessageType.ToString(); } catch { return MessageType.Exception.ToString(); } } }
        public string Name { get; set; }
        public string ErrorMessage { get; set; }
        public UploadFileType UploadFileType { get; set; }

    }
}
