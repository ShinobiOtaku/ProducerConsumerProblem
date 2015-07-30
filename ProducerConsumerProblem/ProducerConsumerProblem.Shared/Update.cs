namespace ProducerConsumerProblem.Shared
{
   public interface IUpdate
   {
      string Content { get; }
      string Id { get; }
   }

   class Update : IUpdate
   {
      public Update(string content, string id)
      {
         Content = content;
         Id = id;
      }

      public string Content { get; }
      public string Id { get; set; }
   }
}
