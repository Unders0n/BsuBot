using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using BsuBot.Dialogs;
using BsuBot.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace BsuBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ILifetimeScope _scope;
        private RootDialog _rootDialog;

        public MessagesController()
        {
            

        }
        public MessagesController(ILifetimeScope scope)
        {
            SetField.NotNull(out _scope, nameof(scope), scope);
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {

            
            //ignore bot
          //  if (activity?.From?.Name?.Contains("BsuBot") == true) return new HttpResponseMessage(HttpStatusCode.Accepted);

            using (var scope = DialogModule.BeginLifetimeScope(_scope, activity))
            {
                _rootDialog = scope.Resolve<RootDialog>();

                if (activity.Type == ActivityTypes.Message)
                {
                   // if (activity.Text.ToLower().Contains("/deleteprofile")) return new HttpResponseMessage(HttpStatusCode.Accepted);

                    if (activity.Text.ToLower().Contains("reset") || activity.Text.ToLower().Contains("menu") || activity.Text.ToLower().Contains("start"))
                    {
                        //reset stack first
                        var botData = scope.Resolve<IBotData>();
                        await botData.LoadAsync(CancellationToken.None);
                        var _task = scope.Resolve<IDialogTask>();
                        _task.Reset();

                        await Conversation.SendAsync(activity,
                            () => new ExceptionHandlerDialog<object>(_rootDialog));

                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }

                    await Conversation.SendAsync(activity, () => new ExceptionHandlerDialog<object>(_rootDialog));
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else if (activity.Type == ActivityTypes.ConversationUpdate)
                {
                        // return new HttpResponseMessage(HttpStatusCode.Accepted);
                        //if user connects - we start


                    //for web chat - special occasion on starting
                    if (activity.ChannelId == "webchat")
                    {
                        IConversationUpdateActivity iConversationUpdated = activity as IConversationUpdateActivity;
                        if (iConversationUpdated != null)
                        {
                            ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));

                            foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                            {
                                // if the bot is added, then 
                                if (member.Id == iConversationUpdated.Recipient.Id)
                                {
                                    var reply = ((Activity)iConversationUpdated).CreateReply($"Hi! I'm AGI Virtual Agent. I can answer questions specific to Above Guideline Increase (AGI). Type anything to start a chat.");
                                    await connector.Conversations.ReplyToActivityAsync(reply);
                                }

                            }
                        }
                     }
                    else
                    {
                        IConversationUpdateActivity iConversationUpdated = activity as IConversationUpdateActivity;
                        if (iConversationUpdated != null)
                        {
                            ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));

                            foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                            {
                                // if the bot is added, then 
                                if (member.Id == iConversationUpdated.Recipient.Id)
                                {
                                    await Conversation.SendAsync(activity,
                                        () => new ExceptionHandlerDialog<object>(_rootDialog));
                                    }
                            }
                        }
                    }
                   


                        //if bot - we skip
                        //  if (activity?.MembersAdded?.FirstOrDefault().Id.Contains("testWebUser1") == true) return new HttpResponseMessage(HttpStatusCode.Accepted);

                        //  return new HttpResponseMessage(HttpStatusCode.Accepted);
                        //igrnore is added user is webchat, already working with
                        //  if (activity?.MembersAdded?.FirstOrDefault().Id == "testWebUser1") return new HttpResponseMessage(HttpStatusCode.Accepted);

                        //start if just joined

                        /*if (activity.MembersAdded.Count == 1)
                    {
                        /*var botData = scope.Resolve<IBotData>();
                        await botData.LoadAsync(CancellationToken.None);
                        var _task = scope.Resolve<IDialogTask>();
                        _task.Reset();#1#

                        // var act = activity.AsMessageActivity();
                        /* using (var scope = DialogModule.BeginLifetimeScope(this.scope, act))
                         {
                            var _rootDialog = scope.Resolve<RootDialog>();#1#



                        await Conversation.SendAsync(activity,
                            () => new ExceptionHandlerDialog<object>(_rootDialog));
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                        /*}#1#
                    }*/

                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                }
                else
                {
                    HandleSystemMessage(activity);
                }

                return new HttpResponseMessage(HttpStatusCode.Accepted);
                
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}