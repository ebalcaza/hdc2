import { useState } from "react";
import { Link } from "react-router-dom";
import { dashboard } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import GraficoBarras from "../components/GraficoBarras.jsx";
import GraficoDona from "../components/GraficoDona.jsx";
import GraficoLinea from "../components/GraficoLinea.jsx";
import TarjetaMini from "../components/TarjetaMini.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { fecha, kg, porcentaje } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

function Tendencia({ valor }) {
  if (valor === null || valor === undefined) {
    return <span className="tendencia tendencia-neutra">Sin mes anterior para comparar</span>;
  }
  if (valor === 0) return <span className="tendencia tendencia-neutra">Igual que el mes anterior</span>;

  // En emisiones, subir es lo malo (rojo) y bajar lo bueno (verde)
  const sube = valor > 0;
  return (
    <span className={`tendencia ${sube ? "tendencia-sube" : "tendencia-baja"}`}>
      {sube ? "▲" : "▼"} {porcentaje(Math.abs(valor))} vs. mes anterior
    </span>
  );
}

// Pantalla principal de individuos y organizaciones
export default function Dashboard() {
  const { sesion } = useAuth();
  const { cargando, error, datos } = useCarga(() => dashboard.obtener(), []);
  const [modo, setModo] = useState("mensual");

  const acciones = (
    <div className="acciones">
      <Link to="/reportes/nuevo" className="btn btn-primario">+ Nuevo reporte</Link>
      <Link to="/reportes" className="btn btn-sec">Ver mis reportes</Link>
    </div>
  );

  const principal = datos?.porAlcance[0];
  const secundario = datos?.porAlcance[1];
  const fuentePrincipal = datos?.porFuente[0];

  return (
    <>
      <div className="titulo-fila">
        <div>
          <h1>Hola, {sesion.nombre}</h1>
          <p className="suave">Así van las emisiones de todos tus reportes.</p>
        </div>
        {acciones}
      </div>

      <Alerta>{error}</Alerta>
      {cargando && <Cargando />}

      {datos && datos.cantidadReportes === 0 && (
        <div className="tarjeta vacio">
          <p>Todavía no hay datos para mostrar. Creá tu primer reporte y acá vas a ver tus emisiones, su reparto por alcance y cómo evolucionan.</p>
          <Link to="/reportes/nuevo" className="btn btn-primario">Crear mi primer reporte</Link>
        </div>
      )}

      {datos && datos.cantidadReportes > 0 && (
        <>
          <section className="kpis" aria-label="Resumen">
            <TarjetaMini
              variante={1}
              icono="🌍"
              titulo="CO₂ total emitido"
              valor={kg(datos.totalCo2Kg)}
              detalle="Suma de todos tus reportes"
            />
            <TarjetaMini
              variante={2}
              icono="📄"
              titulo="Reportes"
              valor={datos.cantidadReportes}
              detalle={`${datos.cantidadRegistros} registros de consumo`}
            />
            <TarjetaMini
              variante={3}
              icono="📊"
              titulo="Promedio por reporte"
              valor={kg(datos.promedioPorReporteKg)}
              detalle="CO₂ medio de cada reporte"
            />
            <TarjetaMini
              variante={4}
              icono="📅"
              titulo="Este mes"
              valor={kg(datos.esteMesCo2Kg)}
              detalle={`Mes anterior: ${kg(datos.mesAnteriorCo2Kg)}`}
              pie={<Tendencia valor={datos.variacionMensualPorcentaje} />}
            />
            <TarjetaMini
              variante={5}
              icono="🎯"
              titulo="Alcance principal"
              valor={principal ? principal.etiqueta : "-"}
              detalle={principal ? `${porcentaje(principal.porcentaje)} de tus emisiones` : undefined}
            />
            <TarjetaMini
              variante={6}
              icono="🕒"
              titulo="Último reporte"
              valor={datos.ultimoReporteCo2Kg !== null ? kg(datos.ultimoReporteCo2Kg) : "-"}
              detalle={datos.ultimoReporteFecha ? fecha(datos.ultimoReporteFecha) : undefined}
            />
          </section>

          <div className="fila-graficos">
            <section className="tarjeta">
              <div className="tarjeta-cabecera">
                <h3>Emisiones por alcance</h3>
              </div>
              <p className="suave">Qué porcentaje del total aporta cada alcance.</p>
              <GraficoDona items={datos.porAlcance} total={datos.totalCo2Kg} />
            </section>

            <section className="tarjeta">
              <div className="tarjeta-cabecera">
                <h3>Evolución en el tiempo</h3>
                <div className="conmutador" role="group" aria-label="Tipo de curva">
                  <button type="button" className={modo === "mensual" ? "activo" : ""} onClick={() => setModo("mensual")}>Por mes</button>
                  <button type="button" className={modo === "acumulado" ? "activo" : ""} onClick={() => setModo("acumulado")}>Acumulado</button>
                </div>
              </div>
              <p className="suave">
                {modo === "mensual"
                  ? "kg de CO₂ de los reportes creados en cada mes."
                  : "kg de CO₂ sumados mes a mes desde tu primer reporte."}
              </p>
              <GraficoLinea
                serie={datos.serieMensual}
                campo={modo === "mensual" ? "totalCo2Kg" : "acumuladoCo2Kg"}
                color={modo === "mensual" ? "var(--c1)" : "var(--c2)"}
                idGradiente={`degrade-${modo}`}
              />
              {datos.serieMensual.length < 2 && (
                <p className="suave">Con más meses de datos vas a ver la curva completa.</p>
              )}
            </section>
          </div>

          <div className="grilla-graficos">
            <GraficoBarras titulo="Por tipo de emisión" items={datos.porTipoEmision} />
            <GraficoBarras titulo="Fuentes que más emiten" items={datos.porFuente} />

            <section className="tarjeta">
              <h3>Lectura rápida</h3>
              <ul className="lectura">
                {principal && (
                  <li>
                    El <strong>{principal.etiqueta}</strong> concentra el <strong>{porcentaje(principal.porcentaje)}</strong> de tus emisiones
                    ({kg(principal.totalCo2Kg)}).
                  </li>
                )}
                {secundario && (
                  <li>
                    Le sigue el <strong>{secundario.etiqueta}</strong> con el {porcentaje(secundario.porcentaje)}.
                  </li>
                )}
                {fuentePrincipal && (
                  <li>
                    La fuente que más emite es <strong>{fuentePrincipal.etiqueta}</strong> ({porcentaje(fuentePrincipal.porcentaje)} del total).
                  </li>
                )}
                {datos.variacionMensualPorcentaje !== null && datos.variacionMensualPorcentaje !== 0 && (
                  <li>
                    Este mes emitiste un{" "}
                    <strong>{porcentaje(Math.abs(datos.variacionMensualPorcentaje))}</strong>{" "}
                    {datos.variacionMensualPorcentaje > 0 ? "más" : "menos"} que el mes anterior.
                  </li>
                )}
              </ul>
            </section>
          </div>
        </>
      )}
    </>
  );
}
