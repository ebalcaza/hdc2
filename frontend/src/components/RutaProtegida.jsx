import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

/** Deja pasar solo a los roles indicados. La sesión se verifica en Layout. */
export default function RutaProtegida({ roles, children }) {
  const { sesion } = useAuth();
  if (roles && !roles.includes(sesion?.rol)) return <Navigate to="/" replace />;
  return children;
}
