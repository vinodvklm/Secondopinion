using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.IO;

namespace CommonUtils
{
    public class ImageUpload
    {
        // set default size here
        public int Width { get; set; }

        public int Height { get; set; }

        // folder for the upload, you can put this in the web.config
        public string UploadPath { get; set; }
        public string Prepend { get; set; }

        public bool isToBeScaled { get; set; }
        public ResultClass<ImageResult> RenameUploadFile(HttpPostedFileBase file, Int32 counter = 0)
        {
            var fileName = Path.GetFileName(file.FileName);
            Prepend += (counter > 0) ? ((counter).ToString()) + "_" : "";
            //string prepend = "item_";
            string finalFileName = MakeValidFileName((Prepend + fileName).Replace(" ", ""));
            if (System.IO.File.Exists(HttpContext.Current.Request.MapPath(UploadPath + finalFileName)))
            {
                //file exists => add count try again
                return RenameUploadFile(file, ++counter);
            }
            //file doesn't exist, upload item but validate first
            return UploadFile(file, finalFileName);
        }

        private ResultClass<ImageResult> UploadFile(HttpPostedFileBase file, string fileName)
        {
            ImageResult imageResult = new ImageResult { MessageType = MessageType.Success, ErrorMessage = null };
            fileName = fileName.Replace(" ", "");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + UploadPath, fileName);
            //var path = Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);
            string extension = Path.GetExtension(file.FileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.MessageType = MessageType.Failed;
                imageResult.ImageName = "";
                imageResult.ErrorMessage = "Invalid Extension";
                return new ResultClass<ImageResult>
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
                if (extension == ".svg")
                {
                    isToBeScaled = false;
                }
                if (isToBeScaled)
                {
                    Image imgOriginal = Image.FromFile(path);

                    //pass in whatever value you want 
                    Image imgActual = Scale(imgOriginal);
                    imgOriginal.Dispose();
                    imgActual.Save(path);
                    imgActual.Dispose();
                }
                imageResult.ImageName = UploadPath + fileName;

                return new ResultClass<ImageResult>
                {
                    Result = imageResult,
                    MessageType = CommonUtils.MessageType.Success,
                    Message = new CommonUtils.Utility { Key = CommonUtils.MessageType.Success.ToString(), Value = "Image has been uploaded" }
                }; ;
            }
            catch (Exception ex)
            {
                // you might NOT want to show the exception error for the user
                // this is generaly logging or testing

                imageResult.MessageType = MessageType.Exception;
                imageResult.ErrorMessage = ex.Message;
                return new ResultClass<ImageResult>
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
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                case ".svg":
                    return true;
                default:
                    return false;
            }
        }

        private Image Scale(Image imgPhoto)
        {
            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0;
            float destWidth = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // force resize, might distort image
            if (Width != 0 && Height != 0)
            {
                destWidth = Width;
                destHeight = Height;
            }
            // change size proportially depending on width or height
            else if (Height != 0)
            {
                destWidth = (float)(Height * sourceWidth) / sourceHeight;
                destHeight = Height;
            }
            else
            {
                destWidth = Width;
                destHeight = (float)(sourceHeight * Width / sourceWidth);
            }

            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight,
                                        PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
    public class ImageResult
    {
        public MessageType MessageType { get; set; }
        public string MessageTypeText { get { try { return MessageType.ToString(); } catch { return MessageType.Exception.ToString(); } } }
        public string ImageName { get; set; }
        public string ErrorMessage { get; set; }
        public UploadFileType UploadFileType { get; set; }

    }
}
