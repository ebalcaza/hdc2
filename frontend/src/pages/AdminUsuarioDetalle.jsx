import { useState } from "react";
import { Link, useParams } from "react-router-dom";
import { admin } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import Cargando from "../components/Cargando.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { fecha } from "../utils/formato.js";
import { useCarga } from "../utils/useCarga.js";

function Dato({ etiqueta, valor }) {
  if (valor === null || valor === undefined || valor === "") return null;
  return (
    <div className="dato-lineal">
      <span className="suave">{etiqueta}</span>
      <strong>{valor}</strong>
    </div>
  );
}

export default function AdminUsuarioDetalle() {
  const { id } = useParams();
  const { sesion } = useAuth();
  const { cargando, error, datos, recargar } = useCarga(() => admin.usuarios.obtener(id), [id]);
  const [errorAccion, setErrorAccion] = useState("");
  const [ocupado, setOcupado] = useState(false);

  async function cambiarEstado() {
    const pregunta = datos.activo
      ? `¿Dar de baja la cuenta de ${datos.nombre}? No podrá iniciar sesión.`
      : `¿Reactivar la cuenta de ${datos.nombre}?`;
    if (!window.confirm(pregunta)) return;

    setErrorAccion("");
    setOcupado(true);
    try {
      await (datos.activo ? admin.usuarios.darDeBaja(id) : admin.usuarios.darDeAlta(id));
      await recargar();
    } catch (err) {
      setErrorAccion(err.message);
    } finally {
      setOcupado(false);
    }
  }

  const esUnoMismo = datos && datos.idUsuario === sesion.idUsuario;

  return (
    <>
      <p><Link to="/admin/usuarios">← Volver a usuarios</Link></p>
      <Alerta>{error || errorAccion}</Alerta>
      {cargando && !datos && <Cargando />}

      {datos && (
        <>
          <div className="titulo-fila">
            <h1>{datos.nombre}</h1>
            {!esUnoMismo && (
              <button type="button" className={`btn ${datos.activo ? "btn-peligro" : "btn-primario"}`} disabled={ocupado} onClick={cambiarEstado}>
                {datos.activo ? "Dar de baja" : "Reactivar cuenta"}
              </button>
            )}
          </div>

          <div className="tarjeta">
            <h2>Cuenta</h2>
            <Dato etiqueta="Email" valor={datos.email} />
            <Dato etiqueta="Tipo" valor={datos.tipoUsuario === "Organizacion" ? "Organización" : datos.tipoUsuario} />
            <div className="dato-lineal">
              <span className="suave">Estado</span>
              <span className={`insignia ${datos.activo ? "insignia-ok" : "insignia-baja"}`}>{datos.activo ? "Activo" : "De baja"}</span>
            </div>
            <Dato etiqueta="Fecha de alta" valor={fecha(datos.fechaAlta)} />
            <Dato etiqueta="Fecha de baja" valor={datos.fechaBaja ? fecha(datos.fechaBaja) : null} />
            <Dato etiqueta="Reportes activos" valor={datos.cantidadReportes} />
          </div>

          {(datos.dni || datos.apellido || datos.genero || datos.fechaNacimiento) && (
            <div className="tarjeta">
              <h2>Datos personales</h2>
              <Dato etiqueta="Apellido" valor={datos.apellido} />
              <Dato etiqueta="DNI" valor={datos.dni} />
              <Dato etiqueta="Fecha de nacimiento" valor={datos.fechaNacimiento ? new Date(datos.fechaNacimiento).toLocaleDateString("es-AR") : null} />
              <Dato etiqueta="Género" valor={datos.genero} />
            </div>
          )}

          {datos.razonSocial && (
            <div className="tarjeta">
              <h2>Datos de la organización</h2>
              <Dato etiqueta="Razón social" valor={datos.razonSocial} />
              <Dato etiqueta="CUIT" valor={datos.cuit} />
              <Dato etiqueta="Área" valor={datos.area} />
              <Dato etiqueta="Cantidad de miembros" valor={datos.cantidadMiembros} />
              <Dato etiqueta="Descripción" valor={datos.descripcion} />
            </div>
          )}

          <h2>Domicilios</h2>
          {datos.domicilios.length === 0 && <p className="suave">Sin domicilios cargados.</p>}
          <div className="lista-tarjetas">
            {datos.domicilios.map((d) => (
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
      )}
    </>
  );
}
