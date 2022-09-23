using OK.Messages;
using Proto;

namespace OK.CentralSystem;

public class ChargepointActor : ChargePointBase
{
    private PID? _pid;

    public ChargepointActor(IContext context) : base(context)
    {
    }

    public override Task HandleOnline(Online request)
    {
        _pid = request.LocalPid;
        return Task.CompletedTask;
    }

    public override Task HandleFromChargePoint (FromChargepoint request)
    {
        Console.WriteLine("TaDa!! " + request.Payload);
        //
        // Context.Send(_pid, new ToChargepoint()
        // {
        //     Payload = "ocpp...."
        // });

        return Task.CompletedTask;
    }
}