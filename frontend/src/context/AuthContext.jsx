import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { alNoAutorizado, borrarSesion, guardarSesion, leerSesion } from "../api/cliente.js";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [sesion, setSesion] = useState(() => leerSesion());

  const iniciar = useCallback((datos) => {
    guardarSesion(datos);
    setSesion(datos);
  }, []);

  const cerrar = useCallback(() => {
    borrarSesion();
    setSesion(null);
  }, []);

  // Si la API responde 401, se cierra la sesión y las rutas redirigen al login
  useEffect(() => {
    alNoAutorizado(cerrar);
  }, [cerrar]);

  const valor = useMemo(() => ({ sesion, iniciar, cerrar }), [sesion, iniciar, cerrar]);
  return <AuthContext.Provider value={valor}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const contexto = useContext(AuthContext);
  if (!contexto) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return contexto;
}
