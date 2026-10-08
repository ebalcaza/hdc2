import { useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { reportes } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { descargarPdfReporte } from "../utils/descargas.js";
import { decimal, factor, fecha, kg } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

// RFR06: detalle de un reporte
export default function ReporteDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { cargando, error, datos } = useCarga(() => reportes.obtener(id), [id]);
  const [errorAccion, setErrorAccion] = useState("");

  async function descargar() {
    setErrorAccion("");
    try {
      await descargarPdfReporte(id);
    } catch (err) {
      setErrorAccion(err.message);
    }
  }

  async function eliminar() {
    if (!window.confirm(`¿Eliminar el reporte N° ${id}? Esta acción no se puede deshacer.`)) return;
    try {
      await reportes.eliminar(id);
      navigate("/reportes", { replace: true });
    } catch (err) {
      setErrorAccion(err.message);
    }
  }

  return (
    <>
      <p><Link to="/reportes">← Volver a mis reportes</Link></p>
      <Alerta>{error || errorAccion}</Alerta>
      {cargando && <Cargando />}

      {datos && (
        <>
          <div className="titulo-fila">
            <h1>Reporte N° {datos.idReporte}</h1>
            <div className="acciones">
              <Link to={`/reportes/${id}/editar`} className="btn btn-sec">Editar</Link>
              <button type="button" className="btn btn-primario" onClick={descargar}>Descargar PDF</button>
              <button type="button" className="btn btn-peligro" onClick={eliminar}>Eliminar</button>
            </div>
          </div>

          <div className="resumen">
            <div className="tarjeta dato">
              <span className="suave">CO₂ total emitido</span>
              <strong className="dato-grande">{kg(datos.totalCo2Kg)}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Creado</span>
              <strong>{fecha(datos.fechaCreacion)}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Última modificación</span>
              <strong>{fecha(datos.fechaModificacion)}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Registros</span>
              <strong>{datos.registros.length}</strong>
            </div>
          </div>

          <h2>Registros de consumo</h2>
          <table className="responsive">
            <thead>
              <tr>
                <th>Fuente</th>
                <th>Domicilio</th>
                <th>Alcance</th>
                <th>Consumo</th>
                <th>Factor</th>
                <th>CO₂</th>
              </tr>
            </thead>
            <tbody>
              {datos.registros.map((r) => (
                <tr key={r.idRegistro}>
                  <td data-label="Fuente">{r.fuente}</td>
                  <td data-label="Domicilio">{r.domicilio}</td>
                  <td data-label="Alcance">{r.alcance ?? "-"}</td>
                  <td data-label="Consumo">{decimal(r.valor)} {r.unidad}</td>
                  <td data-label="Factor">{factor(r.factorEmision)}</td>
                  <td data-label="CO₂"><strong>{kg(r.co2Kg)}</strong></td>
                </tr>
              ))}
            </tbody>
          </table>
          <p className="suave">CO₂ (kg) = consumo × factor de emisión de la fuente.</p>
        </>
      )}
    </>
  );
}
