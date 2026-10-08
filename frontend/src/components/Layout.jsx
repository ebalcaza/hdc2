import { Navigate, NavLink, Outlet } from "react-router-dom";
import BotonTema from "./BotonTema.jsx";
import { useAuth } from "../context/AuthContext.jsx";
import { ROLES } from "../utils/constantes.js";

export default function Layout() {
  const { sesion, cerrar } = useAuth();
  if (!sesion) return <Navigate to="/login" replace />;

  const esAdmin = sesion.rol === ROLES.administrador;
  const clase = ({ isActive }) => (isActive ? "activo" : undefined);

  return (
    <div className="app">
      <header className="cabecera">
        <div className="cabecera-fila">
          <span className="marca">🌱 Huella de carbono</span>
          <div className="usuario">
            <span className="usuario-nombre">{sesion.nombre}</span>
            <BotonTema />
            <button type="button" className="btn btn-sec btn-chico" onClick={cerrar}>Salir</button>
          </div>
        </div>

        <nav className="menu" aria-label="Principal">
          {esAdmin ? (
            <>
              <NavLink to="/admin/estadisticas" className={clase}>Estadísticas</NavLink>
              <NavLink to="/admin/usuarios" className={clase}>Usuarios</NavLink>
              <NavLink to="/admin/factores" className={clase}>Factores de emisión</NavLink>
            </>
          ) : (
            <>
              <NavLink to="/inicio" className={clase}>Inicio</NavLink>
              <NavLink to="/reportes" end className={clase}>Mis reportes</NavLink>
              <NavLink to="/reportes/nuevo" className={clase}>Nuevo reporte</NavLink>
              <NavLink to="/domicilios" className={clase}>Domicilios</NavLink>
            </>
          )}
        </nav>
      </header>

      <main className="contenido">
        <Outlet />
      </main>
    </div>
  );
}
