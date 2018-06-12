using System;
using System.Collections;
using System.Linq;
using System.Messaging;

public class MessagingQueueManager
{
    MessageQueue MSMQManager = null;

    public void CreateMessagingQueue(string queueName = "MSMQueue")
    {

        if (MessageQueue.Exists(@".\Private$\"+ queueName))
        {
            MSMQManager = new MessageQueue(@".\Private$\"+queueName);
        }
        else
        {
            MSMQManager = MessageQueue.Create(@".\Private$\"+ queueName);
            MSMQManager.Label = "This is the RightMechanics Messaging Queue";
        }
    }
    public void SendMessage(string message)
    {
        if (MSMQManager == null)
            CreateMessagingQueue();

        MSMQManager.Send(message, "Test");

    }
    public string ReceiveMessage()
    {
        string message = null;
        if (MSMQManager == null)
            CreateMessagingQueue();
        int count = MessageCounts();
        if (count > 0)
        {
            var MSMQmessage = MSMQManager.Receive();
            MSMQmessage.Formatter = new XmlMessageFormatter(new String[] { "System.String, mscorlib" });

            message = MSMQmessage.Body.ToString();
        }
        return message;
    }

    public int MessageCounts()
    {
        int result = 0;
        result = MSMQManager.Cast<System.Messaging.Message>().Count();
        return result;
    }

    public void DeleteTheQueue()
    {
        if(MSMQManager != null)
        {
            MessageQueue.Delete(MSMQManager.Path);
            MSMQManager.Dispose();
            MSMQManager = null;
        }
    }
}
