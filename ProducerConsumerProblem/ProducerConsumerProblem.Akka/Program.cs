using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using ProducerConsumerProblem.Shared;
using ProducerConsumerProblem.Shared.Consumers;
using ProducerConsumerProblem.Shared.Producers;

namespace ProducerConsumerProblem.Akka
{
   class FacebookConsumerActor : ReceiveActor, IFacebookConsumer
   {
      readonly IActorRef _repositoryActor;

      public FacebookConsumerActor(IActorRef repositoryActor, FacebookProducer producer)
      {
         _repositoryActor = repositoryActor;
         producer.SubscribeToUpdates(this);
      }

      public void OnNewFacebookUpdate(IUpdate update)
      {
         _repositoryActor.Tell(update);
      }
   }

   class TwitterConsumerActor : ReceiveActor, ITwitterConsumer
   {
      readonly IActorRef _repositoryActor;

      public TwitterConsumerActor(IActorRef repositoryActor, TwitterProducer producer)
      {
         _repositoryActor = repositoryActor;
         producer.SubscribeToUpdates(this);
      }

      public void OnNewTwitterUpdate(IUpdate update)
      {
         _repositoryActor.Tell(update);
      }
   }

   class AggregatedRepositoryActor : ReceiveActor
   {
      readonly IList<IUpdate> _aggregatedUpdates = new List<IUpdate>();
      readonly IList<IActorRef> _clients = new List<IActorRef>();

      public AggregatedRepositoryActor()
      {
         StartConsuming();
      }

      public void StartConsuming()
      {
         Become(Consuming);
      }

      public void StopConsuming()
      {
         Become(NotConsuming);
      }

      void Consuming()
      {
         ListenToClientSubscriptions();

         Receive<IUpdate>(update =>
         {
            _aggregatedUpdates.Add(update);

            foreach (var client in _clients)
               client.Tell(update);
         });

         Receive<bool>(shouldStop => shouldStop, _ => StopConsuming());
      }

      void NotConsuming()
      {
         ListenToClientSubscriptions();
      }

      void ListenToClientSubscriptions()
      {
         Receive<IActorRef>(client =>
         {
            _clients.Add(client);

            foreach (var update in _aggregatedUpdates)
               client.Tell(update);
         });
      }
   }

   class ClientConsumer : ReceiveActor
   {
      public ClientConsumer()
      {
         Receive<IUpdate>(update => Console.WriteLine($"({update.Id}) : {update.Content}"));
      }
   }

   class Program
   {
      static void Main(string[] args)
      {
         var system = ActorSystem.Create("MyActorSystem");

         var aggregatedRepository = system.ActorOf(Props.Create<AggregatedRepositoryActor>());

         system.ActorOf(Props.Create(() => new FacebookConsumerActor(aggregatedRepository, new FacebookProducer())));
         system.ActorOf(Props.Create(() => new TwitterConsumerActor(aggregatedRepository, new TwitterProducer())));

         var client = system.ActorOf(Props.Create<ClientConsumer>());
         aggregatedRepository.Tell(client);

         Thread.Sleep(3000);
         aggregatedRepository.Tell(true);

         system.AwaitTermination();
      }
   }
}
