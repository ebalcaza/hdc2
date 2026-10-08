import { useEffect, useState } from "react";
import { admin, catalogos } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import GraficoBarras from "../components/GraficoBarras.jsx";
import { fecha, kg } from "../utils/formato.js";

const FILTROS_VACIOS = {
  desde: "",
  hasta: "",
  tipoUsuario: "",
  idFuente: "",
  idAlcance: "",
  idTipoEmision: "",
  provincia: "",
  ciudad: ""
};

// RFR02: el administrador arma un reporte estadístico según distintas métricas
export default function AdminEstadisticas() {
  const [filtros, setFiltros] = useState(FILTROS_VACIOS);
  const [listas, setListas] = useState({ fuentes: [], alcances: [], tipos: [] });
  const [resultado, setResultado] = useState(null);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  async function consultar(valores) {
    setError("");
    setCargando(true);
    try {
      setResultado(await admin.estadisticas(valores));
    } catch (err) {
      setError(err.message);
    } finally {
      setCargando(false);
    }
  }

  useEffect(() => {
    let cancelado = false;
    (async () => {
      try {
        const [fuentes, alcances, tipos] = await Promise.all([
          catalogos.fuentes(),
          catalogos.alcances(),
          catalogos.tiposEmision()
        ]);
        if (!cancelado) setListas({ fuentes, alcances, tipos });
      } catch (err) {
        if (!cancelado) setError(err.message);
      }
    })();
    consultar(FILTROS_VACIOS);
    return () => { cancelado = true; };
  }, []);

  const cambiar = (campo) => (e) => setFiltros({ ...filtros, [campo]: e.target.value });

  function enviar(e) {
    e.preventDefault();
    consultar(filtros);
  }

  function limpiar() {
    setFiltros(FILTROS_VACIOS);
    consultar(FILTROS_VACIOS);
  }

  return (
    <>
      <h1>Estadísticas de emisiones</h1>

      <form className="tarjeta" onSubmit={enviar}>
        <h2>Filtros</h2>
        <div className="grilla-form">
          <label className="campo">
            <span>Desde</span>
            <input type="date" value={filtros.desde} onChange={cambiar("desde")} />
          </label>
          <label className="campo">
            <span>Hasta</span>
            <input type="date" value={filtros.hasta} onChange={cambiar("hasta")} />
          </label>
          <label className="campo">
            <span>Tipo de usuario</span>
            <select value={filtros.tipoUsuario} onChange={cambiar("tipoUsuario")}>
              <option value="">Todos</option>
              <option value="Individuo">Individuo</option>
              <option value="Organizacion">Organización</option>
            </select>
          </label>
          <label className="campo">
            <span>Fuente de emisión</span>
            <select value={filtros.idFuente} onChange={cambiar("idFuente")}>
              <option value="">Todas</option>
              {listas.fuentes.map((f) => <option key={f.idFuente} value={f.idFuente}>{f.nombre}</option>)}
            </select>
          </label>
          <label className="campo">
            <span>Alcance</span>
            <select value={filtros.idAlcance} onChange={cambiar("idAlcance")}>
              <option value="">Todos</option>
              {listas.alcances.map((a) => <option key={a.id} value={a.id}>{a.nombre}</option>)}
            </select>
          </label>
          <label className="campo">
            <span>Tipo de emisión</span>
            <select value={filtros.idTipoEmision} onChange={cambiar("idTipoEmision")}>
              <option value="">Todos</option>
              {listas.tipos.map((t) => <option key={t.id} value={t.id}>{t.nombre}</option>)}
            </select>
          </label>
          <label className="campo">
            <span>Provincia</span>
            <input value={filtros.provincia} onChange={cambiar("provincia")} maxLength={100} />
          </label>
          <label className="campo">
            <span>Ciudad</span>
            <input value={filtros.ciudad} onChange={cambiar("ciudad")} maxLength={200} />
          </label>
        </div>
        <div className="acciones">
          <button type="submit" className="btn btn-primario" disabled={cargando}>Generar estadísticas</button>
          <button type="button" className="btn btn-sec" onClick={limpiar} disabled={cargando}>Limpiar filtros</button>
        </div>
      </form>

      <Alerta>{error}</Alerta>
      {cargando && <Cargando texto="Calculando…" />}

      {resultado && !cargando && (
        <>
          <div className="resumen">
            <div className="tarjeta dato">
              <span className="suave">CO₂ total</span>
              <strong className="dato-grande">{kg(resultado.totalCo2Kg)}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Reportes</span>
              <strong className="dato-grande">{resultado.cantidadReportes}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Registros</span>
              <strong className="dato-grande">{resultado.cantidadRegistros}</strong>
            </div>
            <div className="tarjeta dato">
              <span className="suave">Generado</span>
              <strong>{fecha(resultado.generadoEn)}</strong>
            </div>
          </div>

          <div className="grilla-graficos">
            <GraficoBarras titulo="Por fuente de emisión" items={resultado.porFuente} />
            <GraficoBarras titulo="Por alcance" items={resultado.porAlcance} />
            <GraficoBarras titulo="Por tipo de emisión" items={resultado.porTipoEmision} />
            <GraficoBarras titulo="Por provincia" items={resultado.porProvincia} />
            <GraficoBarras titulo="Por tipo de usuario" items={resultado.porTipoUsuario} />
            <GraficoBarras titulo="Por mes" items={resultado.porMes} />
          </div>
        </>
      )}
    </>
  );
}
