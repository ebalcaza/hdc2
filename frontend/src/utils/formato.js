const numero = new Intl.NumberFormat("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
const numeroLargo = new Intl.NumberFormat("es-AR", { maximumFractionDigits: 6 });

export const kg = (n) => `${numero.format(n ?? 0)} kg CO₂`;
export const decimal = (n) => numero.format(n ?? 0);
export const factor = (n) => numeroLargo.format(n ?? 0);

// La API devuelve fechas en UTC; algunas llegan sin la "Z" final
export function fecha(iso) {
  if (!iso) return "-";
  const texto = /(Z|[+-]\d{2}:\d{2})$/i.test(iso) ? iso : `${iso}Z`;
  return new Date(texto).toLocaleString("es-AR", { dateStyle: "short", timeStyle: "short" });
}

// Convierte "" en null para no enviar campos vacíos a la API
export function limpiar(objeto) {
  const salida = {};
  Object.entries(objeto).forEach(([clave, valor]) => {
    if (valor && typeof valor === "object" && !Array.isArray(valor)) salida[clave] = limpiar(valor);
    else salida[clave] = typeof valor === "string" && valor.trim() === "" ? null : valor;
  });
  return salida;
}

const compactoFmt = new Intl.NumberFormat("es-AR", { notation: "compact", maximumFractionDigits: 1 });
const porcentajeFmt = new Intl.NumberFormat("es-AR", { maximumFractionDigits: 1 });

/** 12.500 -> "12,5 mil" */
export const compacto = (n) => compactoFmt.format(n ?? 0);

/** 37.54 -> "37,5 %" */
export const porcentaje = (n) => `${porcentajeFmt.format(n ?? 0)} %`;

/** "2026-10" -> "oct 26" */
export function mesCorto(clave) {
  const [anio, mes] = clave.split("-").map(Number);
  return new Date(anio, mes - 1, 1).toLocaleDateString("es-AR", { month: "short", year: "2-digit" }).replace(".", "");
}
