namespace React.Abstraction;

public interface ILatencyMetricSender
{
    void Send(string methodName, double latency);
}