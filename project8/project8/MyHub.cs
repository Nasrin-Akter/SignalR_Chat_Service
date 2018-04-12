using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using project8.Models;


namespace project8
{
    public class MyHub : Hub
    {
        static List<RegisterViewModel> connectedUsers = new List<RegisterViewModel>();
        ApplicationDbContext db = new ApplicationDbContext();
        public void Connect(string name)
        {
            
            //using(TransactionScope scope=new TransactionScope())
            //{

    
               // UserInfo user = new UserInfo { connectTime = DateTime.Now.ToString(), connectTionID = Context.ConnectionId, Name = name };
            //connectedUsers.Add(user);
           // db.UserInfo.Add(user);
            var req=new RequestInfo{
                 Approved=false,
                 GroupName=name,
                  ReqDateTime=DateTime.Now,
                  UserName= Context.User.Identity.Name
             
            };

            db.RequestInfos.Add(req);
            db.SaveChanges();
            Clients.Others.ReceivedMessage("Server",name);
            Clients.Caller.ReceivedMessage("ME",name);

            //scope.Complete();
            //}
        }


        public void Send(string msg)
        {

           // var uid= db.UserInfo.Where(u=>u.connectTionID== Context.ConnectionId).SingleOrDefault().Id;

          //  var uname = connectedUsers.SingleOrDefault(s => s.connectTionID == Context.ConnectionId).Name;

            string gName = db.RequestInfos.Where(u => u.UserName.Equals(Context.User.Identity.Name)).SingleOrDefault().GroupName;


            var messag = new MessageInfo {  MessageBody=msg, PostDateTime=DateTime.Now.ToString(), UserName= Context.User.Identity.Name};
            db.MessageInfos.Add(messag);
            if (db.SaveChanges() > 0)
            {
                Groups.Add(Context.ConnectionId, gName);
                Clients.OthersInGroup(gName).Received(messag.UserName, msg, "str");

                Clients.Caller.Received("ME", msg, "str");
            }           


        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            //var user = connectedUsers.SingleOrDefault(s => s.connectTionID == Context.ConnectionId);
            //if(user!=null)
            //{
            //    connectedUsers.Remove(user);
            //    Clients.All.ReceivedMessage("SERVER", user.Name + " logout now");
            //}
            Clients.All.ReceivedMessage("SERVER",Context.User.Identity.Name + " logout now");
            return base.OnDisconnected(stopCalled);
        }
    }
}