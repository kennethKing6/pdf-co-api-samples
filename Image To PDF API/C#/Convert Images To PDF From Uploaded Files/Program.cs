//*******************************************************************************************//
//                                                                                           //
// Download Free Evaluation Version From: https://bytescout.com/download/web-installer       //
//                                                                                           //
// Also available as Web API! Get Your Free API Key: https://app.pdf.co/signup               //
//                                                                                           //
// Copyright © 2017-2020 ByteScout, Inc. All rights reserved.                                //
// https://www.bytescout.com                                                                 //
// https://pdf.co                                                                            //
//                                                                                           //
//*******************************************************************************************//


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ByteScoutWebApiExample
{
	class Program
	{
		// The authentication key (API Key).
		// Get your own by registering at https://app.pdf.co/documentation/api
		const String API_KEY = "***********************************";
		
		// Source image files
		static string[] ImageFiles = new string[] { @".\image1.png", @".\image2.jpg" };
		// Destination PDF file name
		const string DestinationFile = @".\result.pdf";

		static void Main(string[] args)
		{
			// Create standard .NET web client instance
			WebClient webClient = new WebClient();

			// Set API Key
			webClient.Headers.Add("x-api-key", API_KEY);

			// 1. UPLOAD FILES TO CLOUD

			List<string> uploadedFiles = new List<string>();

			try
			{
				foreach (string imageFile in ImageFiles)
				{
					// 1a. RETRIEVE THE PRESIGNED URL TO UPLOAD THE FILE.
					
					// Prepare URL for `Get Presigned URL` API call
					string query = Uri.EscapeUriString(string.Format(
						"https://api.pdf.co/v1/file/upload/get-presigned-url?contenttype=application/octet-stream&name={0}",
						Path.GetFileName(imageFile)));

					// Execute request
					string response = webClient.DownloadString(query);

					// Parse JSON response
					JObject json = JObject.Parse(response);

					if (json["error"].ToObject<bool>() == false)
					{
						// Get URL to use for the file upload
						string uploadUrl = json["presignedUrl"].ToString();
						// Get URL of uploaded file to use with later API calls
						string uploadedFileUrl = json["url"].ToString();

						// 1b. UPLOAD THE FILE TO CLOUD.

						webClient.Headers.Add("content-type", "application/octet-stream");
						webClient.UploadFile(uploadUrl, "PUT", imageFile); // You can use UploadData() instead if your file is byte[] or Stream
						
						uploadedFiles.Add(uploadedFileUrl);
					}
					else
					{
						Console.WriteLine(json["message"].ToString());
					}
				}

				if (uploadedFiles.Count > 0)
				{
					// 2. CREATE PDF DOCUMENT FROM UPLOADED IMAGE FILES

					// Prepare URL for `Image To PDF` API call
					string query = Uri.EscapeUriString(string.Format(
						"https://api.pdf.co/v1/pdf/convert/from/image?name={0}&url={1}",
						Path.GetFileName(DestinationFile),
						string.Join(",", uploadedFiles)));

					// Execute request
					string response = webClient.DownloadString(query);

					// Parse JSON response
					JObject json = JObject.Parse(response);

					if (json["error"].ToObject<bool>() == false)
					{
						// Get URL of generated PDF file
						string resultFileUrl = json["url"].ToString();

						// Download PDF file
						webClient.DownloadFile(resultFileUrl, DestinationFile);

						Console.WriteLine("Generated PDF file saved as \"{0}\" file.", DestinationFile);
					}
					else
					{
						Console.WriteLine(json["message"].ToString());
					}
				}
			}
			catch (WebException e)
			{
				Console.WriteLine(e.ToString());
			}

			webClient.Dispose()


			Console.WriteLine();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
