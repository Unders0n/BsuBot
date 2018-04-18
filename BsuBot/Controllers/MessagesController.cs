using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using BsuBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace BsuBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ILifetimeScope scope;

        public MessagesController()
        {
            

        }
        public MessagesController(ILifetimeScope scope)
        {
            SetField.NotNull(out this.scope, nameof(scope), scope);
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
            {


                if (activity.Type == ActivityTypes.Message)
                {
                    if (activity.Text == "reset")
                    {
                        //reset stack first
                        var botData = scope.Resolve<IBotData>();
                        await botData.LoadAsync(CancellationToken.None);
                        var _task = scope.Resolve<IDialogTask>();
                        _task.Reset();

                        await Conversation.SendAsync(activity,
                            () => new RootDialog());

                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }

                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }

                return new HttpResponseMessage(HttpStatusCode.Accepted);
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