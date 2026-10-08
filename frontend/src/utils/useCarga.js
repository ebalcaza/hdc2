import { useCallback, useEffect, useState } from "react";

/** Ejecuta una función async al montar el componente y expone { cargando, error, datos, recargar }. */
export function useCarga(funcion, dependencias = []) {
  const [estado, setEstado] = useState({ cargando: true, error: "", datos: null });

  // eslint-disable-next-line react-hooks/exhaustive-deps
  const recargar = useCallback(async () => {
    setEstado((e) => ({ ...e, cargando: true, error: "" }));
    try {
      const datos = await funcion();
      setEstado({ cargando: false, error: "", datos });
    } catch (err) {
      setEstado({ cargando: false, error: err.message, datos: null });
    }
  }, dependencias);

  useEffect(() => {
    recargar();
  }, [recargar]);

  return { ...estado, recargar };
}
