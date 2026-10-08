import { useState } from "react";
import { admin, catalogos } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { factor } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

const FORM_VACIO = { idFuente: null, nombre: "", idUnidad: "", idTipoEmision: "", idAlcance: "", valorFactorEmision: "", enUso: false };

// Ingreso y mantenimiento de las fuentes de emisión y sus factores
export default function AdminFuentes() {
  const { cargando, error, datos, recargar } = useCarga(async () => {
    const [fuentes, unidades, alcances, tipos] = await Promise.all([
      admin.fuentes.listar(),
      catalogos.unidades(),
      catalogos.alcances(),
      catalogos.tiposEmision()
    ]);
    return { fuentes, unidades, alcances, tipos };
  }, []);

  const [form, setForm] = useState(FORM_VACIO);
  const [mostrarForm, setMostrarForm] = useState(false);
  const [errorForm, setErrorForm] = useState("");
  const [errorAccion, setErrorAccion] = useState("");
  const [mensaje, setMensaje] = useState("");
  const [guardando, setGuardando] = useState(false);

  const editando = form.idFuente !== null;
  const cambiar = (campo) => (e) => setForm({ ...form, [campo]: e.target.value });

  function nueva() {
    setForm(FORM_VACIO);
    setErrorForm("");
    setMensaje("");
    setMostrarForm(true);
  }

  function editar(f) {
    setForm({
      idFuente: f.idFuente,
      nombre: f.nombre,
      idUnidad: String(f.idUnidad),
      idTipoEmision: String(f.idTipoEmision),
      idAlcance: f.idAlcance ? String(f.idAlcance) : "",
      valorFactorEmision: String(f.valorFactorEmision),
      enUso: f.enUso
    });
    setErrorForm("");
    setMensaje("");
    setMostrarForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  async function guardar(e) {
    e.preventDefault();
    setErrorForm("");
    setGuardando(true);

    const cuerpo = {
      nombre: form.nombre,
      idUnidad: Number(form.idUnidad),
      idTipoEmision: Number(form.idTipoEmision),
      idAlcance: form.idAlcance === "" ? null : Number(form.idAlcance),
      valorFactorEmision: Number(form.valorFactorEmision)
    };

    try {
      if (editando) await admin.fuentes.actualizar(form.idFuente, cuerpo);
      else await admin.fuentes.crear(cuerpo);
      setMensaje(editando ? "Fuente actualizada." : "Fuente agregada.");
      setMostrarForm(false);
      setForm(FORM_VACIO);
      await recargar();
    } catch (err) {
      setErrorForm(err.message);
    } finally {
      setGuardando(false);
    }
  }

  async function eliminar(f) {
    if (!window.confirm(`¿Eliminar la fuente "${f.nombre}"?`)) return;
    setErrorAccion("");
    setMensaje("");
    try {
      await admin.fuentes.eliminar(f.idFuente);
      setMensaje("Fuente eliminada.");
      await recargar();
    } catch (err) {
      setErrorAccion(err.message);
    }
  }

  const faltanCatalogos = datos && (datos.unidades.length === 0 || datos.tipos.length === 0);

  return (
    <>
      <div className="titulo-fila">
        <h1>Factores de emisión</h1>
        {!mostrarForm && datos && !faltanCatalogos && (
          <button type="button" className="btn btn-primario" onClick={nueva}>+ Agregar fuente</button>
        )}
      </div>

      <Alerta tipo="ok">{mensaje}</Alerta>
      <Alerta>{error || errorAccion}</Alerta>
      {cargando && !datos && <Cargando />}

      {faltanCatalogos && (
        <Alerta tipo="aviso">
          Para agregar fuentes tienen que estar cargadas las tablas UNIDAD_MEDIDA y TIPO_EMISION en la base de datos.
        </Alerta>
      )}

      {mostrarForm && datos && (
        <form className="tarjeta" onSubmit={guardar}>
          <h2>{editando ? "Editar fuente" : "Nueva fuente de emisión"}</h2>
          <Alerta>{errorForm}</Alerta>
          {editando && form.enUso && (
            <Alerta tipo="aviso">
              Esta fuente ya se usa en reportes: el factor y la unidad no se pueden cambiar. Para un valor nuevo, agregá otra fuente.
            </Alerta>
          )}

          <div className="grilla-form">
            <label className="campo">
              <span>Nombre *</span>
              <input value={form.nombre} onChange={cambiar("nombre")} required maxLength={100} />
            </label>
            <label className="campo">
              <span>Unidad de medida *</span>
              <select value={form.idUnidad} onChange={cambiar("idUnidad")} required disabled={editando && form.enUso}>
                <option value="">Elegí una unidad…</option>
                {datos.unidades.map((u) => <option key={u.id} value={u.id}>{u.nombre}</option>)}
              </select>
            </label>
            <label className="campo">
              <span>Valor del factor (kg CO₂ por unidad) *</span>
              <input
                type="number"
                inputMode="decimal"
                min="0.000001"
                step="any"
                value={form.valorFactorEmision}
                onChange={cambiar("valorFactorEmision")}
                required
                disabled={editando && form.enUso}
              />
            </label>
            <label className="campo">
              <span>Tipo de emisión *</span>
              <select value={form.idTipoEmision} onChange={cambiar("idTipoEmision")} required>
                <option value="">Elegí un tipo…</option>
                {datos.tipos.map((t) => <option key={t.id} value={t.id}>{t.nombre}</option>)}
              </select>
            </label>
            <label className="campo">
              <span>Alcance</span>
              <select value={form.idAlcance} onChange={cambiar("idAlcance")}>
                <option value="">Sin alcance</option>
                {datos.alcances.map((a) => <option key={a.id} value={a.id}>{a.nombre}</option>)}
              </select>
            </label>
          </div>

          <div className="acciones">
            <button type="submit" className="btn btn-primario" disabled={guardando}>{guardando ? "Guardando…" : "Guardar"}</button>
            <button type="button" className="btn btn-sec" onClick={() => setMostrarForm(false)}>Cancelar</button>
          </div>
        </form>
      )}

      {datos && datos.fuentes.length === 0 && <p className="suave">Todavía no hay fuentes cargadas.</p>}

      {datos && datos.fuentes.length > 0 && (
        <table className="responsive">
          <thead>
            <tr>
              <th>Fuente</th>
              <th>Factor</th>
              <th>Unidad</th>
              <th>Tipo</th>
              <th>Alcance</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {datos.fuentes.map((f) => (
              <tr key={f.idFuente}>
                <td data-label="Fuente">
                  <strong>{f.nombre}</strong>
                  {f.enUso && <span className="insignia insignia-info">En uso</span>}
                </td>
                <td data-label="Factor">{factor(f.valorFactorEmision)}</td>
                <td data-label="Unidad">{f.unidad}</td>
                <td data-label="Tipo">{f.tipoEmision}</td>
                <td data-label="Alcance">{f.alcance ?? "-"}</td>
                <td data-label="Acciones" className="acciones">
                  <button type="button" className="btn btn-sec btn-chico" onClick={() => editar(f)}>Editar</button>
                  <button
                    type="button"
                    className="btn btn-peligro btn-chico"
                    disabled={f.enUso}
                    title={f.enUso ? "Ya tiene registros en reportes" : undefined}
                    onClick={() => eliminar(f)}
                  >
                    Eliminar
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  );
}
