import { useState } from "react";
import { Link } from "react-router-dom";
import { reportes } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { descargarPdfReporte } from "../utils/descargas.js";
import { fecha, kg } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

// RFR05: lista de reportes. RFR04: eliminar. RFR07: descargar PDF.
export default function MisReportes() {
  const { cargando, error, datos, recargar } = useCarga(() => reportes.listar(), []);
  const [errorAccion, setErrorAccion] = useState("");
  const [ocupado, setOcupado] = useState(null);

  async function eliminar(reporte) {
    if (!window.confirm(`¿Eliminar el reporte N° ${reporte.idReporte}? Esta acción no se puede deshacer.`)) return;
    setErrorAccion("");
    setOcupado(reporte.idReporte);
    try {
      await reportes.eliminar(reporte.idReporte);
      await recargar();
    } catch (err) {
      setErrorAccion(err.message);
    } finally {
      setOcupado(null);
    }
  }

  async function descargar(reporte) {
    setErrorAccion("");
    setOcupado(reporte.idReporte);
    try {
      await descargarPdfReporte(reporte.idReporte);
    } catch (err) {
      setErrorAccion(err.message);
    } finally {
      setOcupado(null);
    }
  }

  return (
    <>
      <div className="titulo-fila">
        <h1>Mis reportes</h1>
        <Link to="/reportes/nuevo" className="btn btn-primario">+ Nuevo reporte</Link>
      </div>

      <Alerta>{error || errorAccion}</Alerta>
      {cargando && <Cargando />}

      {datos && datos.length === 0 && (
        <div className="tarjeta vacio">
          <p>Todavía no creaste ningún reporte.</p>
          <Link to="/reportes/nuevo" className="btn btn-primario">Crear mi primer reporte</Link>
        </div>
      )}

      {datos && datos.length > 0 && (
        <table className="responsive">
          <thead>
            <tr>
              <th>N°</th>
              <th>Creado</th>
              <th>Registros</th>
              <th>CO₂ total</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {datos.map((r) => (
              <tr key={r.idReporte}>
                <td data-label="N°">{r.idReporte}</td>
                <td data-label="Creado">{fecha(r.fechaCreacion)}</td>
                <td data-label="Registros">{r.cantidadRegistros}</td>
                <td data-label="CO₂ total"><strong>{kg(r.totalCo2Kg)}</strong></td>
                <td data-label="Acciones" className="acciones">
                  <Link to={`/reportes/${r.idReporte}`} className="btn btn-sec btn-chico">Ver</Link>
                  <Link to={`/reportes/${r.idReporte}/editar`} className="btn btn-sec btn-chico">Editar</Link>
                  <button type="button" className="btn btn-sec btn-chico" disabled={ocupado === r.idReporte} onClick={() => descargar(r)}>PDF</button>
                  <button type="button" className="btn btn-peligro btn-chico" disabled={ocupado === r.idReporte} onClick={() => eliminar(r)}>Eliminar</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
