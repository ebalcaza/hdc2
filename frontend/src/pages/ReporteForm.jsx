import { useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { catalogos, domicilios as domiciliosApi, reportes } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { decimal, kg } from "../utils/formato.js";

// RFR01: crear reporte. RFR03: editar los datos de consumo de un reporte existente.
export default function ReporteForm() {
  const { id } = useParams();
  const editando = Boolean(id);
  const navigate = useNavigate();
  const contador = useRef(0);

  const [fuentes, setFuentes] = useState([]);
  const [domicilios, setDomicilios] = useState([]);
  const [filas, setFilas] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [guardando, setGuardando] = useState(false);
  const [error, setError] = useState("");

  const nuevaFila = (datos = {}) => ({ clave: ++contador.current, idFuente: "", idDomicilio: "", valor: "", ...datos });

  useEffect(() => {
    let cancelado = false;

    async function cargar() {
      try {
        const [listaFuentes, listaDomicilios, reporte] = await Promise.all([
          catalogos.fuentes(),
          domiciliosApi.listar(),
          editando ? reportes.obtener(id) : Promise.resolve(null)
        ]);
        if (cancelado) return;

        setFuentes(listaFuentes);
        setDomicilios(listaDomicilios);

        if (reporte) {
          setFilas(reporte.registros.map((r) => nuevaFila({
            idFuente: String(r.idFuente),
            idDomicilio: String(r.idDomicilio),
            valor: String(r.valor)
          })));
        } else {
          // Si tiene un solo domicilio, ya queda elegido
          const unico = listaDomicilios.length === 1 ? String(listaDomicilios[0].idDomicilio) : "";
          setFilas([nuevaFila({ idDomicilio: unico })]);
        }
      } catch (err) {
        if (!cancelado) setError(err.message);
      } finally {
        if (!cancelado) setCargando(false);
      }
    }

    cargar();
    return () => { cancelado = true; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  const fuenteDe = (fila) => fuentes.find((f) => String(f.idFuente) === fila.idFuente);
  const co2De = (fila) => {
    const fuente = fuenteDe(fila);
    const valor = Number(fila.valor);
    return fuente && valor > 0 ? valor * fuente.valorFactorEmision : 0;
  };
  const total = filas.reduce((suma, fila) => suma + co2De(fila), 0);

  const cambiarFila = (clave, campo, valor) =>
    setFilas((actuales) => actuales.map((f) => (f.clave === clave ? { ...f, [campo]: valor } : f)));

  const agregarFila = () => {
    const ultimo = filas[filas.length - 1];
    setFilas([...filas, nuevaFila({ idDomicilio: ultimo?.idDomicilio ?? "" })]);
  };

  const quitarFila = (clave) => setFilas(filas.filter((f) => f.clave !== clave));

  async function guardar(e) {
    e.preventDefault();
    setError("");

    const registros = filas.map((f) => ({
      idFuente: Number(f.idFuente),
      idDomicilio: Number(f.idDomicilio),
      valor: Number(f.valor)
    }));

    if (registros.length === 0 || registros.some((r) => !r.idFuente || !r.idDomicilio || !(r.valor > 0))) {
      setError("Completá la fuente, el domicilio y un consumo mayor a 0 en todos los registros.");
      return;
    }

    setGuardando(true);
    try {
      const reporte = editando
        ? await reportes.actualizar(id, { registros })
        : await reportes.crear({ registros });
      navigate(`/reportes/${reporte.idReporte}`, { replace: true });
    } catch (err) {
      setError(err.message);
      setGuardando(false);
    }
  }

  if (cargando) return <Cargando />;

  const volver = editando ? `/reportes/${id}` : "/reportes";

  return (
    <>
      <p><Link to={volver}>← Volver</Link></p>
      <h1>{editando ? `Editar reporte N° ${id}` : "Nuevo reporte"}</h1>
      <p className="suave">Cargá cada consumo como un registro. El CO₂ se calcula como consumo × factor de emisión.</p>

      <Alerta>{error}</Alerta>
      {domicilios.length === 0 && (
        <Alerta tipo="aviso">Necesitás al menos un domicilio. <Link to="/domicilios">Agregá uno acá</Link>.</Alerta>
      )}

      <form onSubmit={guardar}>
        {filas.map((fila, indice) => {
          const fuente = fuenteDe(fila);
          return (
            <fieldset className="tarjeta fila-registro" key={fila.clave}>
              <legend>Registro {indice + 1}</legend>

              <label className="campo">
                <span>Fuente de emisión *</span>
                <select value={fila.idFuente} onChange={(e) => cambiarFila(fila.clave, "idFuente", e.target.value)} required>
                  <option value="">Elegí una fuente…</option>
                  {fuentes.map((f) => (
                    <option key={f.idFuente} value={f.idFuente}>{f.nombre} ({f.unidad})</option>
                  ))}
                </select>
              </label>

              <label className="campo">
                <span>Domicilio *</span>
                <select value={fila.idDomicilio} onChange={(e) => cambiarFila(fila.clave, "idDomicilio", e.target.value)} required>
                  <option value="">Elegí un domicilio…</option>
                  {domicilios.map((d) => (
                    <option key={d.idDomicilio} value={d.idDomicilio}>{d.calle} {d.altura}, {d.ciudad}</option>
                  ))}
                </select>
              </label>

              <label className="campo">
                <span>Consumo * {fuente ? `(${fuente.unidad})` : ""}</span>
                <input
                  type="number"
                  inputMode="decimal"
                  min="0.0001"
                  step="any"
                  value={fila.valor}
                  onChange={(e) => cambiarFila(fila.clave, "valor", e.target.value)}
                  required
                />
              </label>

              <div className="resultado-fila">
                <span className="suave">CO₂ de este registro</span>
                <strong>{kg(co2De(fila))}</strong>
                {fuente && <span className="suave">Factor: {decimal(fuente.valorFactorEmision)}</span>}
              </div>

              {filas.length > 1 && (
                <button type="button" className="btn btn-peligro btn-chico" onClick={() => quitarFila(fila.clave)}>
                  Quitar registro
                </button>
              )}
            </fieldset>
          );
        })}

        <button type="button" className="btn btn-sec" onClick={agregarFila}>+ Agregar registro</button>

        <div className="barra-total">
          <div>
            <span className="suave">Total estimado</span>
            <strong className="dato-grande">{kg(total)}</strong>
          </div>
          <button type="submit" className="btn btn-primario" disabled={guardando || domicilios.length === 0}>
            {guardando ? "Guardando…" : editando ? "Guardar cambios" : "Crear reporte"}
          </button>
        </div>
      </form>
    </>
  );
}
