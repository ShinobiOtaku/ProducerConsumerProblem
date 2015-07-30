using System;
using System.Threading;
using ProducerConsumerProblem.Shared.Consumers;

namespace ProducerConsumerProblem.Shared.Producers
{
   public class FacebookProducer
   {
      readonly Thread _producerThread;
      IFacebookConsumer _consumer;

      public FacebookProducer()
      {
         _producerThread = new Thread(Produce);
         _producerThread.Start();
      }

      public void SubscribeToUpdates(IFacebookConsumer consumer)
      {
         _consumer = consumer;
      }

      void Produce()
      {
         var names = new []{"Simon", "Clare", "Chris", "Matthew", "Elsie"};
         var actions = new[] {"added a photo", "updated their status", "shared a link", "poked you"};

         var rand = new Random();
         int i = 0;

         Func<Update> genUpdate = () =>
         {
            var name = names[rand.Next(0, names.Length)];
            var action = actions[rand.Next(0, actions.Length)];

            return new Update($"{name} {action}", $"Facebook {i}");
         };

         while (true)
         {
            Thread.Sleep(rand.Next(0, 500));

            if(_consumer == null)
               continue;

            _consumer.OnNewFacebookUpdate(genUpdate());
            i++;
         }
      }
   }
}
