namespace ProducerConsumerProblem.Shared.Consumers
{
   public interface IFacebookConsumer
   {
      void OnNewFacebookUpdate(IUpdate update);
   }

   public interface ITwitterConsumer
   {
      void OnNewTwitterUpdate(IUpdate update);
   }
}
