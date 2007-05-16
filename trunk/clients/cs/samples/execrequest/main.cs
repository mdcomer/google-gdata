/* Copyright (c) 2006 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
#define USE_TRACING
#define DEBUG

using System;
using System.IO;
using System.Xml; 
using System.Net; 
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Calendar;


namespace Google.GData.Client.Samples
{
    /// <summary>
    /// simple pull app for a calendar
    /// </summary>
    class ExecRequest
    {
        /// <summary>name of this application</summary>
        public const string ApplicationName = "ExecRequest/1.0.0";  
            
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("Not enough parameters. Usage is ExecRequest <service> <cmd> <uri> <username> <password>, where cmd is QUERY, UPDATE, INSERT, DELETE");
                return; 
            }

            string s         = args[0];
            string cmd       = args[1];

            string targetUri = args[2];

            string userName = args[3];
            string passWord = args[4];
            

            Service service = new Service(s, ApplicationName);
            service.setUserCredentials(userName, passWord);

            try
            {
                if (cmd.Equals("QUERY"))
                {
                    Stream result = service.Query(new Uri(targetUri));
                    DumpStream(result);
                }
                if (cmd.Equals("DELETE"))
                {
                    service.Delete(new Uri(targetUri));
                    Console.WriteLine("successfully deleted: " + targetUri);
                }
                if (cmd.Equals("POST"))
                {
                    String input = Console.In.ReadToEnd();
                    Console.Write(input);
                    service.StreamSend(new Uri(targetUri), input, GDataRequestType.Insert);
                }
                if (cmd.Equals("UPDATE"))
                {
                    String input = Console.In.ReadToEnd();
                    Console.Write(input);
                    service.StreamSend(new Uri(targetUri), input, GDataRequestType.Update);
                }
            } catch (GDataRequestException e)
            {
                HttpWebResponse response = e.Response as HttpWebResponse;
                Console.WriteLine("Error executing request for Verb: " + cmd + ", Errorcode: " + response.StatusCode);
                Console.WriteLine(response.StatusDescription);
                Console.WriteLine(e.ResponseString);
            }
        }


    	static void DumpStream(Stream s) 
        {
            const int size = 4096;
            byte[] bytes = new byte[4096];
            int numBytes;
    
            while((numBytes = s.Read(bytes, 0, size)) > 0)
            {
                String responseData = System.Text.Encoding.ASCII.GetString(bytes, 0, numBytes);
                Console.Write(responseData);
            }
        }
    }
}
