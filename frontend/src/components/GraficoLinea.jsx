import { useEffect, useRef, useState } from "react";
import { compacto, kg, mesCorto } from "../utils/formato.js";

const ALTO = 260;
const M = { izq: 46, der: 16, arriba: 16, abajo: 32 };

// Escala "redonda" para el eje vertical: 1, 2, 5 o 10 por una potencia de diez
function escalaRedonda(max) {
  if (max <= 0) return 1;
  const potencia = 10 ** Math.floor(Math.log10(max));
  const fraccion = max / potencia;
  const bonita = fraccion <= 1 ? 1 : fraccion <= 2 ? 2 : fraccion <= 5 ? 5 : 10;
  return bonita * potencia;
}

// Curva suave que pasa por todos los puntos (Catmull-Rom convertida a Bézier)
function curva(p, tope, base) {
  if (p.length === 0) return "";
  let d = `M ${p[0].x} ${p[0].y}`;
  for (let i = 0; i < p.length - 1; i++) {
    const p0 = p[i - 1] ?? p[i];
    const p1 = p[i];
    const p2 = p[i + 1];
    const p3 = p[i + 2] ?? p2;
    const c1x = p1.x + (p2.x - p0.x) / 6;
    const c1y = Math.min(base, Math.max(tope, p1.y + (p2.y - p0.y) / 6));
    const c2x = p2.x - (p3.x - p1.x) / 6;
    const c2y = Math.min(base, Math.max(tope, p2.y - (p3.y - p1.y) / 6));
    d += ` C ${c1x} ${c1y}, ${c2x} ${c2y}, ${p2.x} ${p2.y}`;
  }
  return d;
}

/** Gráfico de curva: evolución en el tiempo. `campo` elige la serie ("totalCo2Kg" o "acumuladoCo2Kg"). */
export default function GraficoLinea({ serie, campo, color = "var(--c1)", idGradiente = "degradeLinea" }) {
  const contenedor = useRef(null);
  const [ancho, setAncho] = useState(640);
  const [activo, setActivo] = useState(null);

  // El gráfico se dibuja al ancho real disponible, así el texto se lee igual en el celular y en la PC
  useEffect(() => {
    const el = contenedor.current;
    if (!el) return undefined;
    const observador = new ResizeObserver(([entrada]) => setAncho(Math.max(260, Math.round(entrada.contentRect.width))));
    observador.observe(el);
    return () => observador.disconnect();
  }, []);

  const anchoUtil = ancho - M.izq - M.der;
  const altoUtil = ALTO - M.arriba - M.abajo;
  const base = M.arriba + altoUtil;

  const maximo = escalaRedonda(Math.max(0, ...serie.map((p) => p[campo])));
  const x = (i) => M.izq + (serie.length === 1 ? anchoUtil / 2 : (i / (serie.length - 1)) * anchoUtil);
  const y = (valor) => M.arriba + altoUtil - (valor / maximo) * altoUtil;
  const puntos = serie.map((p, i) => ({ x: x(i), y: y(p[campo]) }));

  const trazo = curva(puntos, M.arriba, base);
  const ultimo = puntos[puntos.length - 1];
  const area = puntos.length > 1 ? `${trazo} L ${ultimo.x} ${base} L ${puntos[0].x} ${base} Z` : "";

  const maxEtiquetas = Math.max(2, Math.floor(anchoUtil / 64));
  const salto = Math.ceil(serie.length / maxEtiquetas);

  function mover(e) {
    const caja = e.currentTarget.getBoundingClientRect();
    const posicion = e.clientX - caja.left;
    let mejor = 0;
    let distancia = Infinity;
    puntos.forEach((pt, i) => {
      const d = Math.abs(pt.x - posicion);
      if (d < distancia) {
        distancia = d;
        mejor = i;
      }
    });
    setActivo(mejor);
  }

  const seleccionado = activo !== null ? serie[activo] : null;
  const tooltipX = activo !== null ? Math.min(Math.max(puntos[activo].x, 70), ancho - 70) : 0;

  return (
    <div className="linea-wrap" ref={contenedor}>
      <svg
        width={ancho}
        height={ALTO}
        role="img"
        aria-label={`Evolución de las emisiones por mes. ${serie.map((p) => `${mesCorto(p.mes)}: ${kg(p[campo])}`).join(", ")}`}
        onPointerMove={mover}
        onPointerDown={mover}
        onPointerLeave={() => setActivo(null)}
      >
        <defs>
          <linearGradient id={idGradiente} x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" style={{ stopColor: color, stopOpacity: 0.35 }} />
            <stop offset="100%" style={{ stopColor: color, stopOpacity: 0.02 }} />
          </linearGradient>
        </defs>

        {[0, 1, 2, 3, 4].map((t) => {
          const valor = (maximo * t) / 4;
          return (
            <g key={t}>
              <line className="eje-linea" x1={M.izq} x2={ancho - M.der} y1={y(valor)} y2={y(valor)} />
              <text className="eje-texto" x={M.izq - 8} y={y(valor) + 4} textAnchor="end">{compacto(valor)}</text>
            </g>
          );
        })}

        {serie.map((p, i) =>
          i % salto === 0 ? (
            <text key={p.mes} className="eje-texto" x={puntos[i].x} y={ALTO - 10} textAnchor="middle">{mesCorto(p.mes)}</text>
          ) : null
        )}

        {area && <path d={area} style={{ fill: `url(#${idGradiente})` }} />}
        {trazo && puntos.length > 1 && (
          <path d={trazo} fill="none" style={{ stroke: color }} strokeWidth="3" strokeLinecap="round" strokeLinejoin="round" />
        )}

        {puntos.map((pt, i) => (
          <circle key={serie[i].mes} cx={pt.x} cy={pt.y} r={activo === i ? 6 : 3.5} style={{ fill: color }} stroke="var(--tarjeta)" strokeWidth="2" />
        ))}

        {activo !== null && (
          <line className="eje-guia" x1={puntos[activo].x} x2={puntos[activo].x} y1={M.arriba} y2={base} />
        )}
      </svg>

      {seleccionado && (
        <div className="tooltip" style={{ left: tooltipX, top: puntos[activo].y }}>
          <strong>{mesCorto(seleccionado.mes)}</strong>
          <span>Del mes: {kg(seleccionado.totalCo2Kg)}</span>
          <br />
          <span>Acumulado: {kg(seleccionado.acumuladoCo2Kg)}</span>
        </div>
      )}
    </div>
  );
}
