internal interface IHealthcheckWriter
{
    Task Healthy();
    void Unhealthy();
}
