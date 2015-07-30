using System;
using System.Threading;
using ProducerConsumerProblem.Shared.Consumers;

namespace ProducerConsumerProblem.Shared.Producers
{
   public class TwitterProducer
   {
      readonly Thread _producerThread;
      ITwitterConsumer _consumer;

      public TwitterProducer()
      {
         _producerThread = new Thread(Produce);
         _producerThread.Start();
      }

      public void SubscribeToUpdates(ITwitterConsumer consumer)
      {
         _consumer = consumer;
      }

      void Produce()
      {
         var names = new[] { "Phil", "Quinton", "Iris", "Mark", "Alice" };
         var actions = new[] { "retweeted a post", "favourited a post", "tweeted", "messaged you" };

         var rand = new Random();
         int i = 0;

         Func<Update> genUpdate = () =>
         {
            var name = names[rand.Next(0, names.Length)];
            var action = actions[rand.Next(0, actions.Length)];

            return new Update($"{name} {action}", $"Twitter {i}");
         };

         while (true)
         {
            Thread.Sleep(rand.Next(0, 400));
            _consumer.OnNewTwitterUpdate(genUpdate());
            i++;
         }
      }
   }
}