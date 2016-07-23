namespace Magisterka.Infrastructure.RaportGenerating
{
    public interface IRaportGenerator
    {
        string GenerateRaport(IRaportCommand command);
    }
}
