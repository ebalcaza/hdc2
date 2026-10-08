import { useState } from "react";
import { domicilios } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import FormularioDomicilio, { DOMICILIO_VACIO } from "../components/FormularioDomicilio.jsx";
import { limpiar } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

export default function Domicilios() {
  const { cargando, error, datos, recargar } = useCarga(() => domicilios.listar(), []);
  const [nuevo, setNuevo] = useState(DOMICILIO_VACIO);
  const [mostrarForm, setMostrarForm] = useState(false);
  const [errorForm, setErrorForm] = useState("");
  const [guardando, setGuardando] = useState(false);

  async function guardar(e) {
    e.preventDefault();
    setErrorForm("");
    setGuardando(true);
    try {
      await domicilios.crear(limpiar(nuevo));
      setNuevo(DOMICILIO_VACIO);
      setMostrarForm(false);
      await recargar();
    } catch (err) {
      setErrorForm(err.message);
    } finally {
      setGuardando(false);
    }
  }

  return (
    <>
      <div className="titulo-fila">
        <h1>Domicilios</h1>
        {!mostrarForm && (
          <button type="button" className="btn btn-primario" onClick={() => setMostrarForm(true)}>+ Agregar domicilio</button>
        )}
      </div>

      <Alerta>{error}</Alerta>
      {cargando && <Cargando />}

      {mostrarForm && (
        <form className="tarjeta" onSubmit={guardar}>
          <h2>Nuevo domicilio</h2>
          <Alerta>{errorForm}</Alerta>
          <FormularioDomicilio valor={nuevo} alCambiar={setNuevo} />
          <div className="acciones">
            <button type="submit" className="btn btn-primario" disabled={guardando}>{guardando ? "Guardando…" : "Guardar"}</button>
            <button type="button" className="btn btn-sec" onClick={() => setMostrarForm(false)}>Cancelar</button>
          </div>
        </form>
      )}

      {datos && datos.length === 0 && <p className="suave">No tenés domicilios cargados.</p>}

      <div className="lista-tarjetas">
        {datos?.map((d) => (
          <article className="tarjeta" key={d.idDomicilio}>
            <strong>{d.calle} {d.altura}</strong>
            <p>
              {[d.piso && `Piso ${d.piso}`, d.departamento && `Depto ${d.departamento}`, d.block && `Block ${d.block}`, d.manzana && `Manzana ${d.manzana}`]
                .filter(Boolean).join(" · ")}
            </p>
            <p>{d.ciudad}, {d.distrito}, {d.provincia} ({d.codigoPostal})</p>
            {d.telefono && <p className="suave">Tel: {d.telefono}</p>}
          </article>
        ))}
      </div>
    </>
  );
}
