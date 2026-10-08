import { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { auth } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import BotonTema from "../components/BotonTema.jsx";
import FormularioDomicilio, { DOMICILIO_VACIO } from "../components/FormularioDomicilio.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { limpiar } from "../utils/formato.js";

const INDIVIDUO_VACIO = { nombre: "", apellido: "", dni: "", fechaNacimiento: "", genero: "" };
const ORGANIZACION_VACIA = { razonSocial: "", cuit: "", area: "", descripcion: "", cantidadMiembros: "" };

export default function Registro() {
  const { sesion, iniciar } = useAuth();
  const [tipo, setTipo] = useState("individuo");
  const [cuenta, setCuenta] = useState({ email: "", contrasena: "" });
  const [individuo, setIndividuo] = useState(INDIVIDUO_VACIO);
  const [organizacion, setOrganizacion] = useState(ORGANIZACION_VACIA);
  const [domicilio, setDomicilio] = useState(DOMICILIO_VACIO);
  const [error, setError] = useState("");
  const [enviando, setEnviando] = useState(false);

  if (sesion) return <Navigate to="/" replace />;

  const cambiar = (estado, setEstado, campo) => (e) => setEstado({ ...estado, [campo]: e.target.value });

  async function enviar(e) {
    e.preventDefault();
    setError("");
    setEnviando(true);
    try {
      let respuesta;
      if (tipo === "individuo") {
        respuesta = await auth.registrarIndividuo(limpiar({ ...cuenta, ...individuo, domicilio }));
      } else {
        const datos = {
          ...cuenta,
          ...organizacion,
          cantidadMiembros: organizacion.cantidadMiembros === "" ? null : Number(organizacion.cantidadMiembros),
          domicilio
        };
        respuesta = await auth.registrarOrganizacion(limpiar(datos));
      }
      iniciar(respuesta);
    } catch (err) {
      setError(err.message);
      setEnviando(false);
    }
  }

  return (
    <div className="pantalla-centrada">
      <BotonTema flotante />
      <form className="tarjeta formulario-ancho" onSubmit={enviar}>
        <h1>Crear cuenta</h1>

        <div className="selector-tipo" role="group" aria-label="Tipo de cuenta">
          <button type="button" className={tipo === "individuo" ? "activo" : ""} onClick={() => setTipo("individuo")}>Individuo</button>
          <button type="button" className={tipo === "organizacion" ? "activo" : ""} onClick={() => setTipo("organizacion")}>Organización</button>
        </div>

        <Alerta>{error}</Alerta>

        <h2>Acceso</h2>
        <div className="grilla-form">
          <label className="campo">
            <span>Email *</span>
            <input type="email" value={cuenta.email} onChange={cambiar(cuenta, setCuenta, "email")} required maxLength={100} autoComplete="email" />
          </label>
          <label className="campo">
            <span>Contraseña * (mínimo 8)</span>
            <input type="password" value={cuenta.contrasena} onChange={cambiar(cuenta, setCuenta, "contrasena")} required minLength={8} maxLength={100} autoComplete="new-password" />
          </label>
        </div>

        <h2>{tipo === "individuo" ? "Datos personales" : "Datos de la organización"}</h2>
        {tipo === "individuo" ? (
          <div className="grilla-form">
            <label className="campo">
              <span>Nombres *</span>
              <input value={individuo.nombre} onChange={cambiar(individuo, setIndividuo, "nombre")} required maxLength={100} />
            </label>
            <label className="campo">
              <span>Apellidos *</span>
              <input value={individuo.apellido} onChange={cambiar(individuo, setIndividuo, "apellido")} required maxLength={200} />
            </label>
            <label className="campo">
              <span>DNI *</span>
              <input value={individuo.dni} onChange={cambiar(individuo, setIndividuo, "dni")} required pattern="\d{7,8}" title="7 u 8 números" inputMode="numeric" />
            </label>
            <label className="campo">
              <span>Fecha de nacimiento</span>
              <input type="date" value={individuo.fechaNacimiento} onChange={cambiar(individuo, setIndividuo, "fechaNacimiento")} />
            </label>
            <label className="campo">
              <span>Género</span>
              <input value={individuo.genero} onChange={cambiar(individuo, setIndividuo, "genero")} maxLength={20} />
            </label>
          </div>
        ) : (
          <div className="grilla-form">
            <label className="campo">
              <span>Razón social o nombre *</span>
              <input value={organizacion.razonSocial} onChange={cambiar(organizacion, setOrganizacion, "razonSocial")} required maxLength={150} />
            </label>
            <label className="campo">
              <span>CUIT *</span>
              <input value={organizacion.cuit} onChange={cambiar(organizacion, setOrganizacion, "cuit")} required maxLength={20} />
            </label>
            <label className="campo">
              <span>Área *</span>
              <input value={organizacion.area} onChange={cambiar(organizacion, setOrganizacion, "area")} required maxLength={100} />
            </label>
            <label className="campo">
              <span>Cantidad de miembros</span>
              <input type="number" min="1" value={organizacion.cantidadMiembros} onChange={cambiar(organizacion, setOrganizacion, "cantidadMiembros")} />
            </label>
            <label className="campo campo-ancho">
              <span>Descripción</span>
              <textarea rows={3} value={organizacion.descripcion} onChange={cambiar(organizacion, setOrganizacion, "descripcion")} maxLength={500} />
            </label>
          </div>
        )}

        <h2>Domicilio</h2>
        <FormularioDomicilio valor={domicilio} alCambiar={setDomicilio} />

        <button type="submit" className="btn btn-primario" disabled={enviando}>
          {enviando ? "Creando cuenta…" : "Crear cuenta"}
        </button>
        <p className="centrado">¿Ya tenés cuenta? <Link to="/login">Iniciá sesión</Link></p>
      </form>
    </div>
  );
}
