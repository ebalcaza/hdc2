import { useState } from "react";
import { Link } from "react-router-dom";
import { admin } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { fecha } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

const FILTROS_INICIALES = { texto: "", tipoUsuario: "", activo: "", pagina: 1 };
const ADMIN_VACIO = { email: "", contrasena: "", nombre: "", apellido: "" };

const etiquetaTipo = (tipo) => (tipo === "Organizacion" ? "Organización" : tipo);

// Gestión de las cuentas de usuarios
export default function AdminUsuarios() {
  const { sesion } = useAuth();
  const [borrador, setBorrador] = useState(FILTROS_INICIALES);
  const [aplicados, setAplicados] = useState(FILTROS_INICIALES);
  const { cargando, error, datos, recargar } = useCarga(() => admin.usuarios.listar(aplicados), [aplicados]);

  const [errorAccion, setErrorAccion] = useState("");
  const [ocupado, setOcupado] = useState(null);

  const [mostrarAdmin, setMostrarAdmin] = useState(false);
  const [nuevoAdmin, setNuevoAdmin] = useState(ADMIN_VACIO);
  const [errorAdmin, setErrorAdmin] = useState("");
  const [creando, setCreando] = useState(false);
  const [mensaje, setMensaje] = useState("");

  const cambiar = (campo) => (e) => setBorrador({ ...borrador, [campo]: e.target.value });

  function buscar(e) {
    e.preventDefault();
    setAplicados({ ...borrador, pagina: 1 });
  }

  function limpiar() {
    setBorrador(FILTROS_INICIALES);
    setAplicados(FILTROS_INICIALES);
  }

  async function cambiarEstado(usuario) {
    const dar = usuario.activo ? "baja" : "alta";
    const pregunta = usuario.activo
      ? `¿Dar de baja la cuenta de ${usuario.nombre}? No podrá iniciar sesión.`
      : `¿Reactivar la cuenta de ${usuario.nombre}?`;
    if (!window.confirm(pregunta)) return;

    setErrorAccion("");
    setMensaje("");
    setOcupado(usuario.idUsuario);
    try {
      await (dar === "baja" ? admin.usuarios.darDeBaja(usuario.idUsuario) : admin.usuarios.darDeAlta(usuario.idUsuario));
      await recargar();
    } catch (err) {
      setErrorAccion(err.message);
    } finally {
      setOcupado(null);
    }
  }

  async function crearAdministrador(e) {
    e.preventDefault();
    setErrorAdmin("");
    setCreando(true);
    try {
      const creado = await admin.usuarios.crearAdministrador({
        ...nuevoAdmin,
        apellido: nuevoAdmin.apellido.trim() === "" ? null : nuevoAdmin.apellido
      });
      setMensaje(`Se creó el administrador ${creado.email}.`);
      setNuevoAdmin(ADMIN_VACIO);
      setMostrarAdmin(false);
      await recargar();
    } catch (err) {
      setErrorAdmin(err.message);
    } finally {
      setCreando(false);
    }
  }

  const totalPaginas = datos ? Math.max(1, Math.ceil(datos.total / datos.tamano)) : 1;
  const irA = (pagina) => setAplicados({ ...aplicados, pagina });

  return (
    <>
      <div className="titulo-fila">
        <h1>Usuarios</h1>
        {!mostrarAdmin && (
          <button type="button" className="btn btn-primario" onClick={() => setMostrarAdmin(true)}>+ Nuevo administrador</button>
        )}
      </div>

      {mostrarAdmin && (
        <form className="tarjeta" onSubmit={crearAdministrador}>
          <h2>Nuevo administrador</h2>
          <Alerta>{errorAdmin}</Alerta>
          <div className="grilla-form">
            <label className="campo">
              <span>Email *</span>
              <input type="email" value={nuevoAdmin.email} onChange={(e) => setNuevoAdmin({ ...nuevoAdmin, email: e.target.value })} required maxLength={100} />
            </label>
            <label className="campo">
              <span>Contraseña * (mínimo 8)</span>
              <input type="password" value={nuevoAdmin.contrasena} onChange={(e) => setNuevoAdmin({ ...nuevoAdmin, contrasena: e.target.value })} required minLength={8} maxLength={100} autoComplete="new-password" />
            </label>
            <label className="campo">
              <span>Nombre *</span>
              <input value={nuevoAdmin.nombre} onChange={(e) => setNuevoAdmin({ ...nuevoAdmin, nombre: e.target.value })} required maxLength={100} />
            </label>
            <label className="campo">
              <span>Apellido</span>
              <input value={nuevoAdmin.apellido} onChange={(e) => setNuevoAdmin({ ...nuevoAdmin, apellido: e.target.value })} maxLength={200} />
            </label>
          </div>
          <div className="acciones">
            <button type="submit" className="btn btn-primario" disabled={creando}>{creando ? "Creando…" : "Crear administrador"}</button>
            <button type="button" className="btn btn-sec" onClick={() => setMostrarAdmin(false)}>Cancelar</button>
          </div>
        </form>
      )}

      <form className="tarjeta" onSubmit={buscar}>
        <div className="grilla-form">
          <label className="campo">
            <span>Buscar (email, nombre, DNI, CUIT)</span>
            <input value={borrador.texto} onChange={cambiar("texto")} maxLength={100} />
          </label>
          <label className="campo">
            <span>Tipo de usuario</span>
            <select value={borrador.tipoUsuario} onChange={cambiar("tipoUsuario")}>
              <option value="">Todos</option>
              <option value="Individuo">Individuo</option>
              <option value="Organizacion">Organización</option>
              <option value="Administrador">Administrador</option>
            </select>
          </label>
          <label className="campo">
            <span>Estado</span>
            <select value={borrador.activo} onChange={cambiar("activo")}>
              <option value="">Todos</option>
              <option value="true">Activos</option>
              <option value="false">Dados de baja</option>
            </select>
          </label>
        </div>
        <div className="acciones">
          <button type="submit" className="btn btn-primario" disabled={cargando}>Buscar</button>
          <button type="button" className="btn btn-sec" onClick={limpiar} disabled={cargando}>Limpiar</button>
        </div>
      </form>

      <Alerta tipo="ok">{mensaje}</Alerta>
      <Alerta>{error || errorAccion}</Alerta>
      {cargando && <Cargando />}

      {datos && datos.items.length === 0 && <p className="suave">No hay usuarios con esos filtros.</p>}

      {datos && datos.items.length > 0 && (
        <>
          <p className="suave">{datos.total} usuario{datos.total === 1 ? "" : "s"}</p>
          <table className="responsive">
            <thead>
              <tr>
                <th>Usuario</th>
                <th>Tipo</th>
                <th>Estado</th>
                <th>Alta</th>
                <th>Reportes</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {datos.items.map((u) => (
                <tr key={u.idUsuario}>
                  <td data-label="Usuario">
                    <strong>{u.nombre}</strong>
                    <br />
                    <span className="suave">{u.email}</span>
                  </td>
                  <td data-label="Tipo">{etiquetaTipo(u.tipoUsuario)}</td>
                  <td data-label="Estado">
                    <span className={`insignia ${u.activo ? "insignia-ok" : "insignia-baja"}`}>{u.activo ? "Activo" : "De baja"}</span>
                  </td>
                  <td data-label="Alta">{fecha(u.fechaAlta)}</td>
                  <td data-label="Reportes">{u.cantidadReportes}</td>
                  <td data-label="Acciones" className="acciones">
                    <Link to={`/admin/usuarios/${u.idUsuario}`} className="btn btn-sec btn-chico">Ver</Link>
                    {u.idUsuario !== sesion.idUsuario && (
                      <button
                        type="button"
                        className={`btn btn-chico ${u.activo ? "btn-peligro" : "btn-sec"}`}
                        disabled={ocupado === u.idUsuario}
                        onClick={() => cambiarEstado(u)}
                      >
                        {u.activo ? "Dar de baja" : "Reactivar"}
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {totalPaginas > 1 && (
            <div className="paginacion">
              <button type="button" className="btn btn-sec btn-chico" disabled={datos.pagina <= 1 || cargando} onClick={() => irA(datos.pagina - 1)}>← Anterior</button>
              <span>Página {datos.pagina} de {totalPaginas}</span>
              <button type="button" className="btn btn-sec btn-chico" disabled={datos.pagina >= totalPaginas || cargando} onClick={() => irA(datos.pagina + 1)}>Siguiente →</button>
            </div>
          )}
        </>
      )}
    </>
  );
}
