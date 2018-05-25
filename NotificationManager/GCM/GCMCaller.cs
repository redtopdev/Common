
namespace RedTop.Common.NotificationManager.GCM
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;

    class GCMCaller
    {



        //Android push message to GCM server method                                                                                       

    private string AndroidPush()                                                                                                                      

      {                                                                                                                                                   

        // your RegistrationID paste here which is received from GCM server.                                                               

        string regId = "APA91bG_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx_V6hO2liMx-eIGAbG2cR4DiIgm5Q";                                                                                              

        // applicationID means google Api key                                                                                                     

        var applicationID = "AIzaSyDScBxxxxxxxxxxxxxxxxxxxpv66IfgA";                                                         

        // SENDER_ID is nothing but your ProjectID (from API Console- google code)//                                          
        var SENDER_ID = "77xxxxx625";                                                                                            


        var value = "Sample Push Message"; //message text box                                                                               


        WebRequest tRequest;                                                                                                           


        tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");                             


        tRequest.Method = "post";                                                                                                       


        tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";                             


        tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));                              


        tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));                                         


        //Data post to server                                                                                                                                         

        string postData =     
        "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message="         

        + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" +      
        regId + ""; 

      


        Console.WriteLine(postData);                                                                                             

        Byte[] byteArray = Encoding.UTF8.GetBytes(postData);                                                
        tRequest.ContentLength = byteArray.Length;                                                                  

        Stream dataStream = tRequest.GetRequestStream();                                                    
        dataStream.Write(byteArray, 0, byteArray.Length);                                                          
        dataStream.Close();
 

        WebResponse tResponse = tRequest.GetResponse();                                                    
        dataStream = tResponse.GetResponseStream();                                                              
 
        StreamReader tReader = new StreamReader(dataStream);                                            
        String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.  
        //Label3.Text = sResponseFromServer;      //Assigning GCM response to Label text 
 
        tReader.Close();                                                                                                                    

        dataStream.Close();                                                                                                             

        tResponse.Close();

        return sResponseFromServer;
      }


    public static string SendNotification(string deviceId, string message)
    {
        var GoogleAppID = ConfigurationManager.AppSettings["GoogleAppID"];//"AIzaSyB3TBmvc36EoYmYvnyN3ey7Dke9R4TUE1k";
        var SENDER_ID = ConfigurationManager.AppSettings["SENDER_ID"];//"762814597471";
        var value = message;

        Stream dataStream = null;
        WebResponse webResponse = null;
        StreamReader streamReader = null;

        try
        {
            var webRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");

            webRequest.Method = "post";
            webRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            webRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
            webRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
            var postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message="
                + value + "&registration_id=" + deviceId + "";
            Byte[] bytes = Encoding.UTF8.GetBytes(postData);
            webRequest.ContentLength = bytes.Length;
            dataStream = webRequest.GetRequestStream();
            dataStream.Write(bytes, 0, bytes.Length);
            dataStream.Close();
            webResponse = webRequest.GetResponse();
            dataStream = webResponse.GetResponseStream();
            streamReader = new StreamReader(dataStream);
            var responseFromServer = streamReader.ReadToEnd();
            //streamReader.Close();
            //dataStream.Close();
            //webResponse.Close();
            return responseFromServer;
        }
        catch (Exception ex)
        {
            return "Error Occurred while sending Notification :" + ex.ToString();
        }
        finally
        {
            if (dataStream != null)
            {
                dataStream.Close();
                dataStream.Dispose();
                dataStream = null;
            }

            if (webResponse != null)
            {
                webResponse.Close();
                webResponse.Dispose();
                webResponse = null;
            }

            if (streamReader != null)
            {
                streamReader.Close();
                streamReader.Dispose();
                streamReader = null;
            }
        }

    }
    }
}
