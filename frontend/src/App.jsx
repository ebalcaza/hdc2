import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout.jsx";
import RutaProtegida from "./components/RutaProtegida.jsx";
import { useAuth } from "./context/AuthContext.jsx";
import { ROLES, USUARIOS_FINALES } from "./utils/constantes.js";
import Login from "./pages/Login.jsx";
import Registro from "./pages/Registro.jsx";
import Dashboard from "./pages/Dashboard.jsx";
import MisReportes from "./pages/MisReportes.jsx";
import ReporteDetalle from "./pages/ReporteDetalle.jsx";
import ReporteForm from "./pages/ReporteForm.jsx";
import Domicilios from "./pages/Domicilios.jsx";
import AdminEstadisticas from "./pages/AdminEstadisticas.jsx";
import AdminUsuarios from "./pages/AdminUsuarios.jsx";
import AdminUsuarioDetalle from "./pages/AdminUsuarioDetalle.jsx";
import AdminFuentes from "./pages/AdminFuentes.jsx";

// "/" lleva a cada tipo de usuario a su pantalla principal
function Inicio() {
  const { sesion } = useAuth();
  if (sesion.rol === ROLES.administrador) return <Navigate to="/admin/estadisticas" replace />;
  return <Navigate to="/inicio" replace />;
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/registro" element={<Registro />} />

      <Route element={<Layout />}>
        <Route path="/" element={<Inicio />} />

        <Route path="/inicio" element={<RutaProtegida roles={USUARIOS_FINALES}><Dashboard /></RutaProtegida>} />
        <Route path="/reportes" element={<RutaProtegida roles={USUARIOS_FINALES}><MisReportes /></RutaProtegida>} />
        <Route path="/reportes/nuevo" element={<RutaProtegida roles={USUARIOS_FINALES}><ReporteForm /></RutaProtegida>} />
        <Route path="/reportes/:id" element={<RutaProtegida roles={USUARIOS_FINALES}><ReporteDetalle /></RutaProtegida>} />
        <Route path="/reportes/:id/editar" element={<RutaProtegida roles={USUARIOS_FINALES}><ReporteForm /></RutaProtegida>} />
        <Route path="/domicilios" element={<RutaProtegida roles={USUARIOS_FINALES}><Domicilios /></RutaProtegida>} />

        <Route path="/admin/estadisticas" element={<RutaProtegida roles={[ROLES.administrador]}><AdminEstadisticas /></RutaProtegida>} />
        <Route path="/admin/usuarios" element={<RutaProtegida roles={[ROLES.administrador]}><AdminUsuarios /></RutaProtegida>} />
        <Route path="/admin/usuarios/:id" element={<RutaProtegida roles={[ROLES.administrador]}><AdminUsuarioDetalle /></RutaProtegida>} />
        <Route path="/admin/factores" element={<RutaProtegida roles={[ROLES.administrador]}><AdminFuentes /></RutaProtegida>} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
