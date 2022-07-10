using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReserveWithGoogle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReserveWithGoogleController : ControllerBase
    {
        [HttpGet]
        [Route("sendfile")]
        public string sendfile()
        {
            string PureFileName = new FileInfo("TotalAmount").Name;
            String uploadUrl = String.Format("{0}/{1}/{2}", "ftp://ftp.sachinkalia.com", "sachin_folder", PureFileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            // This example assumes the FTP site uses anonymous logon.  
            request.Credentials = new NetworkCredential("username", "password");
            request.Proxy = null;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Copy the contents of the file to the request stream.  
            StreamReader sourceStream = new StreamReader(@"D:\Sapmle applications\TotalAmount.txt");
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
            return null;
        }
    }
}