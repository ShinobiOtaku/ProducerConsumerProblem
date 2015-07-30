using System;
using System.Collections.Generic;
using System.Threading;
using ProducerConsumerProblem.Shared;
using ProducerConsumerProblem.Shared.Consumers;
using ProducerConsumerProblem.Shared.Producers;

namespace ProducerConsumerProblem
{
   class AggregatedRepository : IFacebookConsumer, ITwitterConsumer
   {
      readonly IList<IUpdate> _aggregatedUpdates = new List<IUpdate>();
      readonly IList<ClientConsumer> _clients = new List<ClientConsumer>();
      bool _stopped;

      readonly object _consumerLock = new object();
      readonly object _stopLock = new object();
      readonly object _updateLock = new object();

      public void OnNewFacebookUpdate(IUpdate update)
      {
         lock (_stopLock)
         {
            if (_stopped)
               return;

            lock (_updateLock)
            {
               _aggregatedUpdates.Add(update);
            }

            lock(_consumerLock)
            {
               foreach (var client in _clients)
                  client.OnNewUpdate(update);
            }
         }
      }

      public void OnNewTwitterUpdate(IUpdate update)
      {
         lock (_stopLock)
         {
            if (_stopped)
               return;

            lock (_updateLock)
            {
               _aggregatedUpdates.Add(update);
            }

            lock (_consumerLock)
            {
               foreach (var client in _clients)
                  client.OnNewUpdate(update);
            }
         }
      }

      public void SubscribeToAggregatedUpdates(ClientConsumer client)
      {
         lock (_consumerLock)
         {
            _clients.Add(client);

            foreach (var update in _aggregatedUpdates)
               client.OnNewUpdate(update);
         }
      }

      public void StopConsuming()
      {
         lock (_stopLock)
         {
            _stopped = true;
         }
      }
   }

   class ClientConsumer
   {
      public void OnNewUpdate(IUpdate newData)
      {
         Console.WriteLine(newData.Content);
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         var aggregatedRepository = new AggregatedRepository();

         var facebookProducer = new FacebookProducer();
         var twitterProducer  = new TwitterProducer();

         facebookProducer.SubscribeToUpdates(aggregatedRepository);
         twitterProducer.SubscribeToUpdates(aggregatedRepository);

         var client = new ClientConsumer();
         aggregatedRepository.SubscribeToAggregatedUpdates(client);

         Thread.Sleep(3000);
         aggregatedRepository.StopConsuming();

         Console.Read();
      }
   }
}
