import { reportes } from "../api/servicios.js";

// RFR07: baja el PDF y lo guarda en el dispositivo
export async function descargarPdfReporte(id) {
  const { blob, nombre } = await reportes.pdf(id);
  const url = URL.createObjectURL(blob);
  const enlace = document.createElement("a");
  enlace.href = url;
  enlace.download = nombre || `reporte-${id}.pdf`;
  document.body.appendChild(enlace);
  enlace.click();
  enlace.remove();
  setTimeout(() => URL.revokeObjectURL(url), 1000);
}
