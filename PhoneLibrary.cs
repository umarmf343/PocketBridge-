using System;using System.Collections.Generic;using System.Linq;
namespace PocketBridge {
 public sealed class PhoneFile {public string id{get;set;}public string path{get;set;}public long size{get;set;}}
 public sealed class PhoneSelection {public string device{get;set;}public List<PhoneFile> files{get;set;}}
 public sealed class PhoneJob {public string id;public string file;public string path;public string state="queued";}
 public sealed class PhoneLibrary {
  readonly object gate=new object();string owner;List<PhoneFile> files=new List<PhoneFile>();readonly List<PhoneJob> jobs=new List<PhoneJob>();public DateTime LastPoll;public string Device="No phone paired";
  public void Publish(string token,PhoneSelection selection){if(selection==null||selection.files==null||selection.files.Count>10000)throw new ArgumentException("Choose at most 10,000 files per selection.");var unique=new HashSet<string>();foreach(var f in selection.files){if(f==null||String.IsNullOrWhiteSpace(f.id)||f.id.Length>100||!unique.Add(f.id)||f.size<0||String.IsNullOrWhiteSpace(f.path)||f.path.Length>1000)throw new ArgumentException("Invalid phone file manifest.");f.path=SafeRelative(f.path);}lock(gate){if(jobs.Any(j=>j.state=="queued"||j.state=="sending"))throw new ArgumentException("Finish or cancel pending copy jobs before replacing the phone selection.");owner=token;Device=String.IsNullOrWhiteSpace(selection.device)?"Phone browser":selection.device.Substring(0,Math.Min(80,selection.device.Length));files=selection.files;LastPoll=DateTime.UtcNow;jobs.Clear();}}
  public static string SafeRelative(string path){string[] pieces=(path??"").Replace('\\','/').Split('/');if(pieces.Length>30||pieces.Any(p=>p=="."||p==".."||String.IsNullOrEmpty(p)))throw new ArgumentException("Invalid relative file path.");return String.Join("/",pieces.Select(Bridge.SafeName));}
  public List<PhoneFile> Files {get{lock(gate)return files.ToList();}}
  public bool Online{get{lock(gate)return owner!=null&&DateTime.UtcNow-LastPoll<TimeSpan.FromSeconds(10);}}
  public int Queue(IEnumerable<string> ids){lock(gate){if(!Online)throw new InvalidOperationException("Keep the paired phone page open and awake before requesting files.");int count=0;foreach(string id in ids){var file=files.FirstOrDefault(f=>f.id==id);if(file!=null&&!jobs.Any(j=>j.file==id&&(j.state=="queued"||j.state=="sending"))){jobs.Add(new PhoneJob{id=Bridge.Secret(),file=id,path=file.path});count++;}}return count;}}
  public List<PhoneJob> Poll(string token){lock(gate){if(owner!=token)return new List<PhoneJob>();LastPoll=DateTime.UtcNow;return jobs.Where(j=>j.state=="queued").Take(4).ToList();}}
  public string Start(string token,string id){lock(gate){var job=jobs.FirstOrDefault(j=>j.id==id);if(owner!=token||job==null||job.state!="queued")throw new ArgumentException("Copy request expired or belongs to another phone.");job.state="sending";return job.path;}}
  public void Complete(string id,bool success){lock(gate){var job=jobs.FirstOrDefault(j=>j.id==id);if(job!=null)job.state=success?"complete":"failed";}}
  public void CancelPending(){lock(gate)foreach(var job in jobs.Where(j=>j.state=="queued"))job.state="canceled";}
  public void Revoke(string token){lock(gate)if(owner==token)Clear();}
  public void Clear(){lock(gate){owner=null;files.Clear();jobs.Clear();Device="Phone disconnected";}}
 }
}
