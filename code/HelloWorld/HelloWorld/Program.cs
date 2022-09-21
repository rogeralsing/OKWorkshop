using System;
using System.Threading.Tasks;
using Proto;

var system = new ActorSystem();
var context = system.Root;
var props = Props.FromProducer(() => new HelloActor());
var pid = context.Spawn(props);
context.Send(pid, new HelloMessage("Abax"));
Console.ReadLine();

internal record HelloMessage(string Name);

internal class HelloActor : IActor
{
    public Task ReceiveAsync(IContext context)
    {
        return context.Message switch
        {
            Started          => OnStarted(),
            HelloMessage msg => OnHelloMessage(msg),
            _                => Task.CompletedTask
        };
    }

    private Task OnStarted()
    {
        Console.WriteLine("Actor Started");
        return Task.CompletedTask;
    }

    private Task OnHelloMessage(HelloMessage msg)
    {
        Console.WriteLine($"Hello {msg.Name}");
        return Task.CompletedTask;
    }
}