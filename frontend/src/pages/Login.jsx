import { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { auth } from "../api/servicios.js";
import Alerta from "../components/Alerta.jsx";
import BotonTema from "../components/BotonTema.jsx";
import { useAuth } from "../context/AuthContext.jsx";

export default function Login() {
  const { sesion, iniciar } = useAuth();
  const [email, setEmail] = useState("");
  const [contrasena, setContrasena] = useState("");
  const [error, setError] = useState("");
  const [enviando, setEnviando] = useState(false);

  if (sesion) return <Navigate to="/" replace />;

  async function enviar(e) {
    e.preventDefault();
    setError("");
    setEnviando(true);
    try {
      iniciar(await auth.login({ email, contrasena }));
    } catch (err) {
      setError(err.message);
      setEnviando(false);
    }
  }

  return (
    <div className="pantalla-centrada">
      <BotonTema flotante />
      <form className="tarjeta formulario-angosto" onSubmit={enviar}>
        <h1>🌱 Huella de carbono</h1>
        <p className="suave">Iniciá sesión para ver y crear tus reportes.</p>

        <Alerta>{error}</Alerta>

        <label className="campo">
          <span>Email</span>
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required autoComplete="email" />
        </label>

        <label className="campo">
          <span>Contraseña</span>
          <input type="password" value={contrasena} onChange={(e) => setContrasena(e.target.value)} required autoComplete="current-password" />
        </label>

        <button type="submit" className="btn btn-primario" disabled={enviando}>
          {enviando ? "Ingresando…" : "Ingresar"}
        </button>

        <p className="centrado">¿No tenés cuenta? <Link to="/registro">Registrate</Link></p>
      </form>
    </div>
  );
}
