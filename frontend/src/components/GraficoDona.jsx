import { colorDe } from "../utils/colores.js";
import { compacto, kg, porcentaje } from "../utils/formato.js";

const RADIO = 15.9155;   // con este radio la circunferencia mide 100: los trazos se dibujan directamente en %

/** Gráfico de dona con su leyenda: sirve para comparar qué parte del total aporta cada ítem. */
export default function GraficoDona({ items, total, textoCentro = "kg CO₂" }) {
  const suma = items.reduce((acc, i) => acc + i.totalCo2Kg, 0);
  let acumulado = 0;

  const descripcion = items.map((i) => `${i.etiqueta}: ${porcentaje(i.porcentaje)}`).join(", ");

  return (
    <div className="dona">
      <div className="dona-svg-wrap">
        <svg viewBox="0 0 42 42" role="img" aria-label={`Reparto de emisiones. ${descripcion}`}>
          <circle className="dona-fondo" cx="21" cy="21" r={RADIO} fill="none" strokeWidth="6" />
          {items.map((item, i) => {
            const pct = suma > 0 ? (item.totalCo2Kg / suma) * 100 : 0;
            const largo = items.length > 1 ? Math.max(pct - 0.8, 0) : pct;   // pequeña separación entre tramos
            const trazo = (
              <circle
                key={item.etiqueta}
                cx="21"
                cy="21"
                r={RADIO}
                fill="none"
                stroke={colorDe(item.etiqueta, i)}
                strokeWidth="6"
                strokeDasharray={`${largo} ${100 - largo}`}
                strokeDashoffset={25 - acumulado}
              >
                <title>{`${item.etiqueta}: ${kg(item.totalCo2Kg)} (${porcentaje(item.porcentaje)})`}</title>
              </circle>
            );
            acumulado += pct;
            return trazo;
          })}
        </svg>
        <div className="dona-centro">
          <strong>{compacto(total)}</strong>
          <span>{textoCentro}</span>
        </div>
      </div>

      <ul className="leyenda">
        {items.map((item, i) => (
          <li key={item.etiqueta}>
            <span className="punto" style={{ background: colorDe(item.etiqueta, i) }} />
            <span>{item.etiqueta}</span>
            <span className="leyenda-valor">{kg(item.totalCo2Kg)} · <strong>{porcentaje(item.porcentaje)}</strong></span>
            <div className="leyenda-barra">
              <div style={{ width: `${Math.min(100, item.porcentaje)}%`, background: colorDe(item.etiqueta, i) }} />
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
