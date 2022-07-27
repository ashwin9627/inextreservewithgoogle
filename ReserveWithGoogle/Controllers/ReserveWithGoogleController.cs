using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReserveWithGoogle.Model;

namespace ReserveWithGoogle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReserveWithGoogleController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReserveWithGoogleController(IHostingEnvironment hostingEnvironment)
        {
                _hostingEnvironment = hostingEnvironment;            
        }
        [HttpGet]
        [Route("reserve")]
        public string reserve()
        {
            try
            {
                //Read a merchant details json
                var rootPath = _hostingEnvironment.ContentRootPath; //get the root path
                //var fullPath = Path.Combine(rootPath, "Json/Merchent.json"); 
                var fullPath = Path.Combine(rootPath, "Json/Merchent1.json"); //combine the root path with that of our json file inside mydata directory
                var jsonData = System.IO.File.ReadAllText(fullPath); //read all the content inside the file                

                //object formed from merchant json                
                Merchants merchants = JsonConvert.DeserializeObject<Merchants>(jsonData.ToString());
                List<string> MerchantList =new List<string>();// storing the merchantName

                //Looping mechants and create json file for each merchant
                foreach (var merchantname in merchants.merchants)
                {
                    //Filtered matched merchant name with provided merchant name.
                    Merchant filterMerchant = merchants.merchants.Where(x => x.service_id == merchantname.service_id).FirstOrDefault();
                    Availabilityclass availabilityFilecontent = new Availabilityclass();

                    //Filtere result has data
                    if (filterMerchant != null)
                    {
                        DateTime dateTimeHR = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 11, 0, 0);
                        int curTimeStamp = Convert.ToInt32(new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
                        int curTimeStampHR = Convert.ToInt32(new DateTimeOffset(dateTimeHR).ToUnixTimeSeconds());
                        availabilityFilecontent.metadata = new Metadata();
                        availabilityFilecontent.metadata.processing_instruction = "PROCESS_AS_COMPLETE";
                        availabilityFilecontent.metadata.shard_number = 0;
                        availabilityFilecontent.metadata.total_shards = 1;
                        availabilityFilecontent.metadata.nonce = "9371383477";
                        availabilityFilecontent.metadata.generation_timestamp = curTimeStampHR;//Convert.ToInt64(DateTime.Now.ToFileTime());//1652193212;

                        availabilityFilecontent.service_availability = new List<ServiceAvailability>();
                        List<Availability> availabilityList = new List<Availability>();
                        // looping with number of restaurents from same merchant
                        foreach (var x in filterMerchant.details)
                        {
                            
                            availabilityList.Add(new Availability
                            {
                                service_id = filterMerchant.service_id,
                                spots_open = x.open_slots,
                                spots_total = x.total_slots,
                                confirmation_mode = "CONFIRMATION_MODE_SYNCHRONOUS",
                                duration_sec = 3600,
                                resources = new Resources { party_size = x.party_size }
                            });
                            
                            //curTimeStampHR = curTimeStampHR + 3600;
                        }                        
                        availabilityFilecontent.service_availability.Add(new ServiceAvailability { availability = availabilityList });
                        
                    }
                    MerchantList.Add(merchantname.service_id);
                    //serializing object to json string an store into the json file
                    var json = JsonConvert.SerializeObject(availabilityFilecontent, Formatting.Indented);
                    System.IO.File.WriteAllText(@"Availability/" + merchantname.service_id+".json", json);
                    //Send the file through ftp
                    //sendfile(@"Json/CreateJson.json");
                    //return json;
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        [Route("sendfile")]
        public string sendfile(string filePath)
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