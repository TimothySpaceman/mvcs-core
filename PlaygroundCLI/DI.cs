// using Core.DI;
//
// var services = new ServiceContainer();
// services.Use<ILogger>(new ConsoleLogger());
// // services.Use<ILogger, ConsoleLogger>();
// services.Use<IActor, Actor>();
//
// var main = services.Resolve<Main>();
// main.Run();
//
// interface ILogger
// {
//     void Log(string message);
// }
//
// class ConsoleLogger : ILogger
// {
//     private Guid _id = Guid.NewGuid();
//
//     public void Log(string message)
//     {
//         Console.WriteLine($"[{_id}] {message}");
//     }
// }
//
// interface IActor
// {
//     void Act();
// }
//
// class Actor : IActor
// {
//     private ILogger _logger;
//
//     public Actor(ILogger logger)
//     {
//         _logger = logger;
//     }
//
//     public void Act()
//     {
//         _logger.Log("Hello World!");
//     }
// }
//
// class Main
// {
//     private IActor _actor;
//     private ILogger _logger;
//
//     public Main(IActor actor, ILogger logger)
//     {
//         _actor = actor;
//         _logger = logger;
//     }
//
//     public void Run()
//     {
//         _actor.Act();
//         _logger.Log("Main finished");
//     }
// }