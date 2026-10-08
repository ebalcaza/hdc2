// Paleta de los gráficos (las variables --c1..--c6 se definen en styles.css para el tema claro y el oscuro)
const PALETA = ["var(--c1)", "var(--c2)", "var(--c3)", "var(--c4)", "var(--c5)", "var(--c6)"];

/** "Alcance 2" siempre usa el mismo color; el resto se reparte por posición. */
export function colorDe(etiqueta, indice = 0) {
  const numero = Number((etiqueta.match(/\d+/) ?? [])[0]);
  const posicion = Number.isFinite(numero) && numero > 0 ? numero - 1 : indice;
  return PALETA[posicion % PALETA.length];
}
