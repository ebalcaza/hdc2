import { kg } from "../utils/formato.js";

/** Lista de barras horizontales hecha solo con CSS (sin librerías de gráficos). */
export default function GraficoBarras({ titulo, items }) {
  return (
    <section className="tarjeta">
      <h3>{titulo}</h3>
      {items.length === 0 ? (
        <p className="suave">Sin datos para los filtros elegidos.</p>
      ) : (
        <ul className="barras">
          {items.map((item) => (
            <li key={item.etiqueta}>
              <div className="barra-texto">
                <span>{item.etiqueta}</span>
                <span>{kg(item.totalCo2Kg)} · {item.porcentaje.toLocaleString("es-AR")}%</span>
              </div>
              <div className="barra-fondo">
                <div className="barra-valor" style={{ width: `${Math.min(100, item.porcentaje)}%` }} />
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
