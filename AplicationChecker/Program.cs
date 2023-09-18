using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Timers;
using Timer = System.Threading.Timer;

namespace  AplicationChecker;


public class Program
{
     public static async Task Main(string[] args)
     {
          Program program = new Program();
          List<string> fullPaths = new();
          List<(string, bool)> applicationResult = new();
          List<Task> Tasks = new List<Task>();
          fullPaths.Add("C:\\Program Files\\TeamViewer\\TeamViewer.exe");
          fullPaths.Add("C:\\Users\\romsk\\AppData\\Local\\Discord\\app-1.0.9017\\Discord.exe");
          
          Task.Run(() =>
          {
               while (true)
               {
                    Parallel.ForEach(fullPaths, path =>
                    {
                         bool result = program.ProgramIsRunning(path);
                         if (result==false)
                         {
                              var isExcist = applicationResult.Any(x => x.Item1 == path);
                              if (isExcist)
                                  return;
                              
                              applicationResult.Add((path, result));
                              Console.WriteLine(result);
                         }

                         if (result == true && applicationResult.Any(x => x.Item1 == path))
                         {
                              applicationResult.RemoveAt(applicationResult.FindIndex(index => index.Item1 == path));
                         }
                         Console.WriteLine(result);
                    });
                    Thread.Sleep(5000);
               }
          });

          
          while (true)
          {
              
               foreach(var result in applicationResult)
               {
                    await Task.Run(()=>program.IfAppNotRunn(result.Item1, result.Item2));
               };

               Thread.Sleep(50000);
          }
     }
     
     private  bool ProgramIsRunning(string FullPath)
     {
          string FilePath =  Path.GetDirectoryName(FullPath);
          string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
          bool isRunning = false;

          Process[] pList = Process.GetProcessesByName(FileName);

          foreach (Process p in pList) {
               if (p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
               {
                    isRunning = true;
                    break;
               }
          }
          return isRunning;
     }
     
     private  void IfAppNotRunn(string appname,bool isRunned)
     {
          if (!isRunned)
          {
               SenToEmail(appname);
          }
     }
     
     private bool SenToEmail(string appname)
     {
          var reciver = "mi";
          var subject = "Alert Application";
          var body = $"Application{appname} is stopped";
          var username = "noreply3@mygps.ge";
          var pw = "sgpzhupqepdtcpob";
          MailMessage mail = new MailMessage();
          mail.From = new MailAddress(username);
          mail.Sender = new MailAddress(username);
          mail.To.Add(reciver);
          mail.IsBodyHtml = true;
          mail.Subject = subject;
          mail.Body = body;

          SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
          smtp.UseDefaultCredentials = false;

          smtp.Credentials = new System.Net.NetworkCredential(username, pw);
          smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
          smtp.EnableSsl = true;
            
          smtp.Send(mail);
          return true;
     }
}


