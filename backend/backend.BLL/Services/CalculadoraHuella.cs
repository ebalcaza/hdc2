namespace backend.BLL.Services;

public static class CalculadoraHuella
{
    /// <summary>kg de CO2 = consumo × factor de emisión de la fuente.</summary>
    public static decimal CalcularCo2Kg(decimal consumo, decimal factorEmision) =>
        decimal.Round(consumo * factorEmision, 4, MidpointRounding.AwayFromZero);
}
