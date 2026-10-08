using System.Globalization;
using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.DAL.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace backend.BLL.Services;

public interface IReportePdfService
{
    Task<ArchivoDto> GenerarAsync(long idUsuario, long idReporte, CancellationToken ct = default);
}

public class ReportePdfService : IReportePdfService
{
    private static readonly CultureInfo Cultura = CrearCultura();

    private readonly IReporteService _reportes;
    private readonly IUsuarioRepository _usuarios;

    static ReportePdfService()
    {
        // Licencia Community de QuestPDF (gratuita para uso individual y organizaciones pequeñas).
        // Revisar las condiciones vigentes en https://www.questpdf.com/license/
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ReportePdfService(IReporteService reportes, IUsuarioRepository usuarios)
    {
        _reportes = reportes;
        _usuarios = usuarios;
    }

    // RFR07: descargar el reporte seleccionado en PDF
    public async Task<ArchivoDto> GenerarAsync(long idUsuario, long idReporte, CancellationToken ct = default)
    {
        // ObtenerAsync ya verifica que el reporte exista y sea del usuario
        var reporte = await _reportes.ObtenerAsync(idUsuario, idReporte, ct);
        var usuario = await _usuarios.ObtenerPorIdAsync(idUsuario, ct)
            ?? throw new NotFoundException("El usuario no existe.");

        var titular = $"{usuario.Nombre} {usuario.Apellido}".Trim();
        var contenido = Construir(reporte, titular);

        return new ArchivoDto(contenido, $"reporte-huella-carbono-{reporte.IdReporte}.pdf", "application/pdf");
    }

    private static byte[] Construir(ReporteDetalleDto reporte, string titular)
    {
        var documento = Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(36);
                pagina.DefaultTextStyle(x => x.FontSize(9));

                pagina.Header().Column(col =>
                {
                    col.Item().Text($"Reporte de huella de carbono N° {reporte.IdReporte}").FontSize(18).Bold();
                    col.Item().Text($"Titular: {titular}");
                    col.Item().Text($"Creado el {reporte.FechaCreacion.ToString("dd/MM/yyyy HH:mm", Cultura)} (UTC)");
                    if (reporte.FechaModificacion is not null)
                        col.Item().Text($"Última modificación: {reporte.FechaModificacion.Value.ToString("dd/MM/yyyy HH:mm", Cultura)} (UTC)");
                });

                pagina.Content().PaddingVertical(14).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);   // fuente
                            c.RelativeColumn(3);   // domicilio
                            c.RelativeColumn(2);   // alcance
                            c.RelativeColumn(2);   // consumo
                            c.RelativeColumn(2);   // factor
                            c.RelativeColumn(2);   // CO2
                        });

                        tabla.Header(cabecera =>
                        {
                            foreach (var titulo in new[] { "Fuente", "Domicilio", "Alcance", "Consumo", "Factor", "CO2 (kg)" })
                                cabecera.Cell().Element(CeldaEncabezado).Text(titulo).SemiBold();
                        });

                        foreach (var r in reporte.Registros)
                        {
                            tabla.Cell().Element(CeldaCuerpo).Text(r.Fuente);
                            tabla.Cell().Element(CeldaCuerpo).Text(r.Domicilio);
                            tabla.Cell().Element(CeldaCuerpo).Text(r.Alcance ?? "-");
                            tabla.Cell().Element(CeldaCuerpo).AlignRight()
                                .Text($"{r.Valor.ToString("N2", Cultura)} {r.Unidad}");
                            tabla.Cell().Element(CeldaCuerpo).AlignRight()
                                .Text(r.FactorEmision.ToString("0.######", Cultura));
                            tabla.Cell().Element(CeldaCuerpo).AlignRight()
                                .Text(r.Co2Kg.ToString("N2", Cultura));
                        }
                    });

                    col.Item().AlignRight()
                        .Text($"Total emitido: {reporte.TotalCo2Kg.ToString("N2", Cultura)} kg de CO2")
                        .FontSize(13).Bold();

                    col.Item().Text("CO2 (kg) = consumo × factor de emisión de la fuente.")
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                pagina.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            });
        });

        return documento.GeneratePdf();
    }

    private static IContainer CeldaEncabezado(IContainer c) =>
        c.Background(Colors.Grey.Lighten3).Padding(4);

    private static IContainer CeldaCuerpo(IContainer c) =>
        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4);

    private static CultureInfo CrearCultura()
    {
        try { return new CultureInfo("es-AR"); }
        catch (CultureNotFoundException) { return CultureInfo.InvariantCulture; }
    }
}
